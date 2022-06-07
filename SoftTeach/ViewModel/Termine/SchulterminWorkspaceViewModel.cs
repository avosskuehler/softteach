namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using System.Windows.Input;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// ViewModel for managing Termin
  /// </summary>
  public class SchulterminWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Termin currently selected
    /// </summary>
    private SchulterminViewModel currentTermin;

    /// <summary>
    /// Die Jahrgangsstufe, dessen Termine nur dargestellt werden sollen.
    /// </summary>
    private SchuljahrViewModel schuljahrFilter;

    /// <summary>
    /// Der Termintyp, dessen Termine nur dargestellt werden sollen.
    /// </summary>
    private Termintyp? termintypFilter;


    /// <summary>
    /// Speichert das zuletzt verwendete Datum
    /// </summary>
    public static DateTime ZuletztVerwendetesDatum;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="SchulterminWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SchulterminWorkspaceViewModel()
    {
      this.AddTerminCommand = new DelegateCommand(this.AddTermin);
      this.ResetSchuljahrFilterCommand = new DelegateCommand(() => this.SchuljahrFilter = null, () => this.SchuljahrFilter != null);
      this.ResetTermintypFilterCommand = new DelegateCommand(() => this.TermintypFilter = null, () => this.TermintypFilter != null);
      this.AddMultipleDayTerminCommand = new DelegateCommand(this.AddMultipleDayTermin, () => this.CurrentTermin != null);
      this.DeleteTerminCommand = new DelegateCommand(this.DeleteCurrentTermin, () => this.CurrentTermin != null);
      ModifiedTermine = new List<ModifiedTermin>();

      this.TermineViewSource = new CollectionViewSource() { Source = App.MainViewModel.Schultermine };
      using (this.TermineViewSource.DeferRefresh())
      {
        this.TermineViewSource.Filter += this.TermineViewSource_Filter;
        this.TermineViewSource.SortDescriptions.Add(new SortDescription("SchulterminDatum", ListSortDirection.Ascending));
      }

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;

      ZuletztVerwendetesDatum = DateTime.Now;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Schultermine.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentTermin))
        {
          this.CurrentTermin = null;
        }
      };
    }


    /// <summary>
    /// Holt oder setzt die Termine that are changed during a dialog session
    /// </summary>
    public static List<ModifiedTermin> ModifiedTermine { get; set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Termin
    /// </summary>
    public DelegateCommand AddTerminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetSchuljahrFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl den termintyp filter zu entfernen
    /// </summary>
    public DelegateCommand ResetTermintypFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new multiple day Termin
    /// </summary>
    public DelegateCommand AddMultipleDayTerminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Termin
    /// </summary>
    public DelegateCommand DeleteTerminCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die TermineViewSource
    /// </summary>
    public CollectionViewSource TermineViewSource { get; set; }

    /// <summary>
    /// Holt ein gefiltertes View der Termine
    /// </summary>
    public ICollectionView TermineView => this.TermineViewSource.View;

    /// <summary>
    /// Holt oder setzt die termin currently selected in this workspace
    /// </summary>
    public SchulterminViewModel CurrentTermin
    {
      get
      {
        return this.currentTermin;
      }

      set
      {
        if (this.currentTermin == value)
        {
          return;
        }

        this.currentTermin = value;
        this.RaisePropertyChanged("CurrentTermin");
        this.DeleteTerminCommand.RaiseCanExecuteChanged();
        this.AddMultipleDayTerminCommand.RaiseCanExecuteChanged();

        if (value != null)
        {
          ZuletztVerwendetesDatum = value.SchulterminDatum;
        }
      }
    }

    /// <summary>
    /// Holt oder setzt die fach filter for the module list.
    /// </summary>
    public SchuljahrViewModel SchuljahrFilter
    {
      get
      {
        return this.schuljahrFilter;
      }

      set
      {
        this.schuljahrFilter = value;
        this.RaisePropertyChanged("SchuljahrFilter");
        this.TermineView.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt den Termintyp Filter
    /// </summary>
    public Termintyp? TermintypFilter
    {
      get
      {
        return this.termintypFilter;
      }

      set
      {
        this.termintypFilter = value;
        this.RaisePropertyChanged("TermintypFilter");
        this.TermineView.Refresh();
      }
    }

    /// <summary>
    /// Adds the given terminviewmodel to the list of modified termine,
    /// to get track of the updates when we want to update the jahrespläne
    /// with the modified termine.
    /// </summary>
    /// <param name="terminViewModel">The <see cref="TerminViewModel"/> to be
    /// added to the list.</param>
    /// <param name="terminUpdateType">The <see cref="SchulterminUpdateType"/> for this termin.</param>
    /// <param name="parameter"> If the <see cref="SchulterminUpdateType"/> is ChangedWithNewDay this
    /// contains the old termin, if it is BetroffeneKlasseChanged it contains
    /// the list of changes, otherwise it is null.</param>
    public static void AddToModifiedList(SchulterminViewModel terminViewModel, SchulterminUpdateType terminUpdateType, object parameter)
    {
      if (ModifiedTermine == null)
      {
        return;
      }

      var modifiedTermin = new ModifiedTermin(terminViewModel, terminUpdateType, parameter);
      if (!ModifiedTermine.Contains(modifiedTermin))
      {
        ModifiedTermine.Add(modifiedTermin);
      }
    }

    /// <summary>
    /// This method is used to update the jahrespläne with the modified, added, deleted termine
    /// after showing the AddSchulterminDialog.
    /// </summary>
    public static void UpdateJahrespläne()
    {
      App.SetCursor(Cursors.Wait);
      var aktualisierteLerngruppen = new List<LerngruppeViewModel>();

      foreach (var termin in ModifiedTermine)
      {
        var terminViewModel = termin.SchulterminViewModel;
        var lerngruppen = App.MainViewModel.Lerngruppen;

        //IEnumerable<LerngruppenterminViewModel> lerngruppentermine;
        IEnumerable<LerngruppeViewModel> betroffeneLerngruppen;
        if (termin.SchulterminUpdateType == SchulterminUpdateType.BetroffeneKlasseChanged)
        {
          var eventArgs = termin.Parameter as NotifyCollectionChangedEventArgs;

          switch (eventArgs.Action)
          {
            case NotifyCollectionChangedAction.Add:
              // Dem Termin wurde eine e betroffenen Klasse hinzugefügt
              // Hole also alle  betroffenen Lerngruppen und ergänze den Termin.
              betroffeneLerngruppen = lerngruppen.Where(o => o == ((BetroffeneLerngruppeViewModel)eventArgs.NewItems[0]).BetroffeneLerngruppeLerngruppe);

              if (eventArgs.NewItems.Count > 1)
              {
                throw new ArgumentOutOfRangeException("More than one new item.");
              }

              // Skip if there is nothing to do
              if (!betroffeneLerngruppen.Any())
              {
                continue;
              }

              // Ergänze den Termin für alle betroffenen Lerngruppen
              foreach (var lerngruppe in betroffeneLerngruppen)
              {
                if (!aktualisierteLerngruppen.Contains(lerngruppe)) aktualisierteLerngruppen.Add(lerngruppe);

                //lerngruppentermine = lerngruppe.Lerngruppentermine.Where(o => o.LerngruppenterminDatum == terminViewModel.SchulterminDatum);

                //if (!lerngruppentermine.Any())
                //{
                AddTerminToLerngruppe(lerngruppe, terminViewModel);
                //}
                //else
                //{
                //  UpdateTerminInLerngruppe(lerngruppe, terminViewModel, terminViewModel.TerminBeschreibung);
                //}
              }

              break;
            case NotifyCollectionChangedAction.Remove:
              // Aus dem Termin wurden betroffene Klasse entfernt
              // Hole also alle nicht mehr betroffenen Lerngruppen und lösche den Termin.
              var betroffeneLerngruppenDesTermins = eventArgs.OldItems;
              var betroffeneLerngruppenToRemoveTermin = new List<LerngruppeViewModel>();
              foreach (var betroffeneLerngruppe in betroffeneLerngruppenDesTermins)
              {
                var test = betroffeneLerngruppe as BetroffeneLerngruppeViewModel;
                var lerngruppe = lerngruppen.FirstOrDefault(o => o == test.BetroffeneLerngruppeLerngruppe);
                if (lerngruppe != null)
                {
                  betroffeneLerngruppenToRemoveTermin.Add(lerngruppe);
                }
              }

              betroffeneLerngruppen = betroffeneLerngruppenToRemoveTermin;

              //betroffeneLerngruppen = lerngruppen.Where(o => o == ((BetroffeneLerngruppeViewModel)eventArgs.NewItems[0]).BetroffeneLerngruppeLerngruppe);

              ////if (eventArgs.OldItems.Count > 1)
              ////{
              ////  throw new ArgumentOutOfRangeException("More than one new item.");
              ////}

              // Skip if there is nothing to do
              if (!betroffeneLerngruppen.Any())
              {
                continue;
              }

              foreach (var lerngruppe in betroffeneLerngruppen)
              {
                if (!aktualisierteLerngruppen.Contains(lerngruppe)) aktualisierteLerngruppen.Add(lerngruppe);
                RemoveTerminFromLerngruppe(lerngruppe, terminViewModel);
              }

              break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
              throw new NotImplementedException("The BetroffeneKlasse collection changed too much to adapt jahrespläne");
          }
        }
        else
        {
          // Vom Schultermin hat sich nicht die Liste der betroffenen Klassen geändert
          // Er ist entweder ganz , entfernt oder verändert
          // Hole zuerst alle Lerngruppen, die vom Schultermin betroffen sind
          betroffeneLerngruppen = terminViewModel.BetroffeneLerngruppen.Select(o => o.BetroffeneLerngruppeLerngruppe);
          //lerngruppen.Where(o=> o == terminViewModel.BetroffeneKlassen.Any(betroffeneLerngruppe => betroffeneLerngruppe.Model.Klasse == plan.JahresplanKlasse.Model)));

          // Skip if there is nothing to do
          if (!betroffeneLerngruppen.Any())
          {
            continue;
          }

          foreach (var lerngruppe in betroffeneLerngruppen)
          {
            if (!aktualisierteLerngruppen.Contains(lerngruppe)) aktualisierteLerngruppen.Add(lerngruppe);
            switch (termin.SchulterminUpdateType)
            {
              case SchulterminUpdateType.Added:
                // Der Termin ist 
                AddTerminToLerngruppe(lerngruppe, terminViewModel);
                break;
              case SchulterminUpdateType.Removed:
                // Der Termin wurde entfernt
                RemoveTerminFromLerngruppe(lerngruppe, terminViewModel);
                break;
              case SchulterminUpdateType.Changed:
                // Der Termin wurde verändert, ohne die Beschreibung oder das Datum zu verändern
                UpdateTerminInLerngruppe(lerngruppe, terminViewModel);
                break;
              case SchulterminUpdateType.ChangedBeschreibung:
                // Der Termin wurde verändert, und die Beschreibung auch 
                UpdateTerminInLerngruppe(lerngruppe, terminViewModel, (string)termin.Parameter, null);
                break;
              case SchulterminUpdateType.ChangedWithNewDay:
                // Der Termin wurde verändert, und das Datum auch
                UpdateTerminInLerngruppe(lerngruppe, terminViewModel, null, (DateTime)termin.Parameter);
                break;
            }
          }
        }
      }

      // Empty list cause we have updated all termine
      ModifiedTermine.Clear();

      var message = "Die folgenden Jahrespläne wurden angepasst:\n";
      foreach (var lerngruppeViewModel in aktualisierteLerngruppen)
      {
        message += lerngruppeViewModel + "\n";
        var jahresplan = App.MainViewModel.Jahrespläne.FirstOrDefault(o => o.Lerngruppe == lerngruppeViewModel);
        if (jahresplan != null)
        {
          jahresplan.KalenderErstellen();
        }
      }

      App.SetCursor(null);

      new InformationDialog("Jahrespläne aktualisiert", message, false).ShowDialog();
    }

    /// <summary>
    /// Handles deletion of the given Termin
    /// </summary>
    /// <param name="terminViewModel">The termin View Model to be removed
    /// from the list.</param>
    public void DeleteTermin(SchulterminViewModel terminViewModel)
    {
      AddToModifiedList(terminViewModel, SchulterminUpdateType.Removed, null);
      App.MainViewModel.Schultermine.RemoveTest(terminViewModel);
      this.CurrentTermin = null;
    }

    private static void UpdateTerminInLerngruppe(LerngruppeViewModel lerngruppe, SchulterminViewModel terminViewModel, string alteBeschreibung = null, DateTime? alterTermin = null)
    {
      var lerngruppenTerminToUpdate = lerngruppe.Lerngruppentermine.FirstOrDefault(vm => vm.TerminBeschreibung == terminViewModel.TerminBeschreibung && vm.LerngruppenterminDatum == terminViewModel.SchulterminDatum);
      if (alteBeschreibung != null)
      {
        // Termin hat e Beschreibung bekommen
        lerngruppenTerminToUpdate = lerngruppe.Lerngruppentermine.FirstOrDefault(vm => vm.TerminBeschreibung == alteBeschreibung && vm.LerngruppenterminDatum == terminViewModel.SchulterminDatum);
      }
      if (alterTermin != null)
      {
        // Termin hat neues Datum bekommen
        lerngruppenTerminToUpdate = lerngruppe.Lerngruppentermine.FirstOrDefault(vm => vm.TerminBeschreibung == terminViewModel.TerminBeschreibung && vm.LerngruppenterminDatum == alterTermin);
      }

      if (lerngruppenTerminToUpdate != null)
      {
        lerngruppenTerminToUpdate.TerminBeschreibung = terminViewModel.TerminBeschreibung;
        lerngruppenTerminToUpdate.TerminErsteUnterrichtsstunde = terminViewModel.TerminErsteUnterrichtsstunde;
        lerngruppenTerminToUpdate.TerminLetzteUnterrichtsstunde = terminViewModel.TerminLetzteUnterrichtsstunde;
        lerngruppenTerminToUpdate.TerminTermintyp = terminViewModel.TerminTermintyp;
        lerngruppenTerminToUpdate.TerminOrt = terminViewModel.TerminOrt;
        lerngruppenTerminToUpdate.LerngruppenterminDatum = terminViewModel.SchulterminDatum;
      }
    }

    private static void RemoveTerminFromLerngruppe(LerngruppeViewModel lerngruppe, SchulterminViewModel terminViewModel)
    {
      if (lerngruppe == null) return;

      var lerngruppenTerminViewModel = lerngruppe.Lerngruppentermine.SingleOrDefault(o => o.LerngruppenterminDatum == terminViewModel.SchulterminDatum && o.TerminBeschreibung == terminViewModel.TerminBeschreibung);

      if (lerngruppe.Lerngruppentermine.Contains(lerngruppenTerminViewModel))
      {
        lerngruppe.Lerngruppentermine.Remove(lerngruppenTerminViewModel);
      }
    }

    private static void AddTerminToLerngruppe(LerngruppeViewModel lerngruppe, SchulterminViewModel terminViewModel)
    {
      if (lerngruppe == null)
      {
        return;
      }

      if (lerngruppe.Lerngruppentermine.Any(o => o.LerngruppenterminDatum == terminViewModel.SchulterminDatum && o.TerminBeschreibung == terminViewModel.TerminBeschreibung))
      {
        // already added by another method
        return;
      }

      var erLerngruppenTermin = new Lerngruppentermin
      {
        Beschreibung = terminViewModel.TerminBeschreibung,
        ErsteUnterrichtsstunde = terminViewModel.TerminErsteUnterrichtsstunde.Model,
        LetzteUnterrichtsstunde = terminViewModel.TerminLetzteUnterrichtsstunde.Model,
        Termintyp = terminViewModel.TerminTermintyp,
        Ort = terminViewModel.TerminOrt,
        Lerngruppe = lerngruppe.Model,
        Datum = terminViewModel.SchulterminDatum
      };

      var lerngruppenTerminViewModel = new LerngruppenterminViewModel(erLerngruppenTermin);

      if (!lerngruppe.Lerngruppentermine.Contains(lerngruppenTerminViewModel))
      {
        //App.UnitOfWork.Context.Termine.Add(lerngruppenTermin);
        //App.MainViewModel.Lerngruppentermine.Add(lerngruppenTerminViewModel);
        lerngruppe.Lerngruppentermine.Add(lerngruppenTerminViewModel);
      }
    }

    /// <summary>
    /// Filtert die Termine nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Das Terminobjekt, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private void TermineViewSource_Filter(object sender, FilterEventArgs e)
    {
      var schulterminViewModel = e.Item as SchulterminViewModel;
      if (schulterminViewModel == null)
      {
        e.Accepted = false;
        return;
      }

      if (this.schuljahrFilter != null)
      {
        if (schulterminViewModel.SchulterminSchuljahr != this.schuljahrFilter)
        {
          e.Accepted = false;
          return;
        }
      }

      if (this.termintypFilter != null)
      {
        if (schulterminViewModel.TerminTermintyp != this.termintypFilter)
        {
          e.Accepted = false;
          return;
        }
      }

      e.Accepted = true;
      return;
    }

    /// <summary>
    /// Tritt auf, wenn die Selection verändert wurde.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die PropertyChangedEventArgs mit den Infos.</param>
    private void SelectionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Schuljahr")
      {
        this.SchuljahrFilter = Selection.Instance.Schuljahr;
      }
    }

    /// <summary>
    /// Handles addition a new Termin to the workspace and model
    /// </summary>
    private void AddTermin()
    {
      var termin = new Schultermin
      {
        Beschreibung = "Neuer Termin",
        Datum = ZuletztVerwendetesDatum,
        ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[0].Model
      };
      var letzte = Math.Min(App.MainViewModel.Unterrichtsstunden.Count - 1, 9);
      termin.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[letzte].Model;
      if (Selection.Instance.Schuljahr != null)
      {
        termin.Schuljahr = Selection.Instance.Schuljahr.Model;
      }

      termin.Termintyp = Termintyp.Besprechung;
      //App.UnitOfWork.Context.Termine.Add(termin);
      var vm = new SchulterminViewModel(termin);

      using (new UndoBatch(App.MainViewModel, string.Format("Termin {0} erstellt.", vm), false))
      {
        App.MainViewModel.Schultermine.Add(vm);
        this.CurrentTermin = vm;

        AddToModifiedList(vm, SchulterminUpdateType.Added, null);
      }
    }

    private void AddMultipleDayTermin()
    {
      var dlg = new AskForTerminSpanDialog(this.CurrentTermin.SchulterminDatum);
      dlg.ShowDialog();
      var aktuellesDatum = dlg.StartDatum;
      var ende = dlg.EndDatum;
      aktuellesDatum = aktuellesDatum.AddDays(1);

      while (aktuellesDatum <= ende)
      {
        var termin = new Schultermin
        {
          Beschreibung = this.CurrentTermin.TerminBeschreibung,
          Datum = aktuellesDatum,
          ErsteUnterrichtsstunde = this.CurrentTermin.TerminErsteUnterrichtsstunde.Model,
          LetzteUnterrichtsstunde = this.CurrentTermin.TerminLetzteUnterrichtsstunde.Model,
          Schuljahr = this.CurrentTermin.SchulterminSchuljahr.Model,
          Termintyp = this.CurrentTermin.TerminTermintyp
        };
        //App.UnitOfWork.Context.Termine.Add(termin);

        var vm = new SchulterminViewModel(termin);
        foreach (var betroffeneLerngruppeViewModel in this.CurrentTermin.BetroffeneLerngruppen)
        {
          var betroffeneLerngruppe = new BetroffeneLerngruppe
          {
            Lerngruppe = betroffeneLerngruppeViewModel.BetroffeneLerngruppeLerngruppe.Model,
            Schultermin = termin
          };
          var viemModelBetroffeneKlasse = new BetroffeneLerngruppeViewModel(betroffeneLerngruppe);
          vm.BetroffeneLerngruppen.Add(viemModelBetroffeneKlasse);
          //App.MainViewModel.BetroffeneKlassen.Add(viemModelBetroffeneKlasse);
        }

        termin.Ort = this.CurrentTermin.TerminOrt;

        if (!App.MainViewModel.Schultermine.Contains(vm))
        {
          App.MainViewModel.Schultermine.Add(vm);
          AddToModifiedList(vm, SchulterminUpdateType.Added, null);
        }

        aktuellesDatum = aktuellesDatum.AddDays(1);
      }
    }

    /// <summary>
    /// Handles deletion of the current Termin
    /// </summary>
    private void DeleteCurrentTermin()
    {
      this.DeleteTermin(this.CurrentTermin);
    }
  }
}