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
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;

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
    private JahrtypViewModel jahrtypFilter;

    /// <summary>
    /// Der Termintyp, dessen Termine nur dargestellt werden sollen.
    /// </summary>
    private TermintypViewModel termintypFilter;


    /// <summary>
    /// Speichert das zuletzt verwendete Datum
    /// </summary>
    public static DateTime ZuletztVerwendetesDatum;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchulterminWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SchulterminWorkspaceViewModel()
    {
      this.AddTerminCommand = new DelegateCommand(this.AddTermin);
      this.ResetJahrtypFilterCommand = new DelegateCommand(() => this.JahrtypFilter = null, () => this.JahrtypFilter != null);
      this.ResetTermintypFilterCommand = new DelegateCommand(() => this.TermintypFilter = null, () => this.TermintypFilter != null);
      this.AddMultipleDayTerminCommand = new DelegateCommand(this.AddMultipleDayTermin, () => this.CurrentTermin != null);
      this.DeleteTerminCommand = new DelegateCommand(this.DeleteCurrentTermin, () => this.CurrentTermin != null);
      ModifiedTermine = new List<ModifiedTermin>();
      this.TermineView = CollectionViewSource.GetDefaultView(App.MainViewModel.Schultermine);
      this.TermineView.Filter = this.CustomFilter;
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
    public DelegateCommand ResetJahrtypFilterCommand { get; private set; }

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
    /// Holt oder setzt ein gefiltertes View der Schultermincollection
    /// </summary>
    public ICollectionView TermineView { get; set; }

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
        this.currentTermin = value;
        this.RaisePropertyChanged("CurrentTermin");
        this.DeleteTerminCommand.RaiseCanExecuteChanged();
        this.AddMultipleDayTerminCommand.RaiseCanExecuteChanged();
        ZuletztVerwendetesDatum = value.SchulterminDatum;
      }
    }

    /// <summary>
    /// Holt oder setzt die fach filter for the module list.
    /// </summary>
    public JahrtypViewModel JahrtypFilter
    {
      get
      {
        return this.jahrtypFilter;
      }

      set
      {
        this.jahrtypFilter = value;
        this.RaisePropertyChanged("JahrtypFilter");
        this.TermineView.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt den Termintyp Filter
    /// </summary>
    public TermintypViewModel TermintypFilter
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
      var aktualisiertePläne = new List<JahresplanViewModel>();

      foreach (var termin in ModifiedTermine)
      {
        var terminViewModel = termin.SchulterminViewModel;
        var jahrespläne = App.MainViewModel.Jahrespläne;

        TagesplanViewModel tagesplan;
        IEnumerable<JahresplanViewModel> betroffeneJahrespläne;
        if (termin.SchulterminUpdateType == SchulterminUpdateType.BetroffeneKlasseChanged)
        {
          var eventArgs = termin.Parameter as NotifyCollectionChangedEventArgs;

          switch (eventArgs.Action)
          {
            case NotifyCollectionChangedAction.Add:
              betroffeneJahrespläne = jahrespläne.Where(
               plan => (plan.JahresplanJahrtyp == terminViewModel.SchulterminJahrtyp
                 && plan.JahresplanKlasse == ((BetroffeneKlasseViewModel)eventArgs.NewItems[0]).BetroffeneKlasseKlasse));

              if (eventArgs.NewItems.Count > 1)
              {
                throw new ArgumentOutOfRangeException("More than one new item.");
              }

              // Skip if there is nothing to do
              if (!betroffeneJahrespläne.Any())
              {
                continue;
              }

              foreach (var jahresplanViewModel in betroffeneJahrespläne)
              {
                if (!aktualisiertePläne.Contains(jahresplanViewModel)) aktualisiertePläne.Add(jahresplanViewModel);

                tagesplan = jahresplanViewModel.GetTagesplanByDate(terminViewModel.SchulterminDatum);
                // Skip if there is nothing to do
                if (tagesplan == null)
                {
                  continue;
                }

                if (tagesplan.Lerngruppentermine.Count == 0)
                {
                  AddTerminToTagesplan(tagesplan, terminViewModel);
                }
                else
                {
                  UpdateTerminInTagesplan(terminViewModel, tagesplan, terminViewModel.TerminBeschreibung);
                }
              }

              break;
            case NotifyCollectionChangedAction.Remove:
              var betroffeneKlasse = eventArgs.OldItems[0] as BetroffeneKlasseViewModel;
              betroffeneJahrespläne = jahrespläne.Where(
               plan => (plan.JahresplanJahrtyp == terminViewModel.SchulterminJahrtyp
                 && plan.JahresplanKlasse == betroffeneKlasse.BetroffeneKlasseKlasse));

              if (eventArgs.OldItems.Count > 1)
              {
                throw new ArgumentOutOfRangeException("More than one new item.");
              }

              // Skip if there is nothing to do
              if (!betroffeneJahrespläne.Any())
              {
                continue;
              }

              foreach (var jahresplanViewModel in betroffeneJahrespläne)
              {
                if (!aktualisiertePläne.Contains(jahresplanViewModel)) aktualisiertePläne.Add(jahresplanViewModel);
                tagesplan = jahresplanViewModel.GetTagesplanByDate(terminViewModel.SchulterminDatum);
                RemoveTerminFromTagesplan(terminViewModel, tagesplan);
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
          // Get only jahrespläne of correct year and with a klasse that is in the
          // betroffene Klassen list of the termin
          // Filter by betroffene Klasse
          betroffeneJahrespläne = jahrespläne.Where(
           plan => (plan.JahresplanJahrtyp == terminViewModel.SchulterminJahrtyp
             && terminViewModel.BetroffeneKlassen.Any(betroffeneKlasse => betroffeneKlasse.Model.Klasse == plan.JahresplanKlasse.Model)));

          // Skip if there is nothing to do
          if (!betroffeneJahrespläne.Any())
          {
            continue;
          }

          foreach (var jahresplanViewModel in betroffeneJahrespläne)
          {
            if (!aktualisiertePläne.Contains(jahresplanViewModel)) aktualisiertePläne.Add(jahresplanViewModel);
            switch (termin.SchulterminUpdateType)
            {
              case SchulterminUpdateType.Added:
                tagesplan = jahresplanViewModel.GetTagesplanByDate(terminViewModel.SchulterminDatum);
                if (tagesplan == null)
                {
                  continue;
                }
                AddTerminToTagesplan(tagesplan, terminViewModel);
                break;
              case SchulterminUpdateType.Removed:
                tagesplan = jahresplanViewModel.GetTagesplanByDate(terminViewModel.SchulterminDatum);
                if (tagesplan == null)
                {
                  continue;
                }
                RemoveTerminFromTagesplan(terminViewModel, tagesplan);
                break;
              case SchulterminUpdateType.Changed:
                tagesplan = jahresplanViewModel.GetTagesplanByDate(terminViewModel.SchulterminDatum);
                if (tagesplan == null)
                {
                  continue;
                }
                if (tagesplan.Lerngruppentermine.Count == 0)
                {
                  AddTerminToTagesplan(tagesplan, terminViewModel);
                }
                else
                {
                  UpdateTerminInTagesplan(terminViewModel, tagesplan, terminViewModel.TerminBeschreibung);
                }
                break;
              case SchulterminUpdateType.ChangedBeschreibung:
                tagesplan = jahresplanViewModel.GetTagesplanByDate(terminViewModel.SchulterminDatum);
                if (tagesplan == null)
                {
                  continue;
                }

                if (
                  tagesplan.Lerngruppentermine.Any(
                    o => o.TerminBeschreibung == terminViewModel.TerminBeschreibung))
                {
                  UpdateTerminInTagesplan(terminViewModel, tagesplan, (string)termin.Parameter);
                }
                else
                {
                  AddTerminToTagesplan(tagesplan, terminViewModel);
                }

                break;
              case SchulterminUpdateType.ChangedWithNewDay:
                var oldtagesplan = jahresplanViewModel.GetTagesplanByDate((DateTime)termin.Parameter);
                if (oldtagesplan == null)
                {
                  continue;
                }
                RemoveTerminFromTagesplan(terminViewModel, oldtagesplan);
                tagesplan = jahresplanViewModel.GetTagesplanByDate(terminViewModel.SchulterminDatum);
                if (tagesplan == null)
                {
                  continue;
                }
                AddTerminToTagesplan(tagesplan, terminViewModel);
                break;
            }
          }
        }
      }

      // Empty list cause we have updated all termine
      ModifiedTermine.Clear();
      App.SetCursor(null);

      var message = "Die folgenden Jahrespläne wurden angepasst:\n";
      foreach (var jahresplanViewModel in aktualisiertePläne)
      {
        message += jahresplanViewModel + "\n";
      }

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

    private static void UpdateTerminInTagesplan(SchulterminViewModel terminViewModel, TagesplanViewModel tagesplan, string alteBeschreibung)
    {
      // if there is no lerngruppentermin with the new bezeichung try to use
      // the old one
      var searchText = terminViewModel.TerminBeschreibung;
      if (tagesplan.Lerngruppentermine.All(vm => vm.TerminBeschreibung != searchText))
      {
        searchText = alteBeschreibung;
      }

      var lerngruppenTerminViewModel = tagesplan.Lerngruppentermine.SingleOrDefault(
        vm => vm.TerminBeschreibung == searchText);
      if (lerngruppenTerminViewModel != null)
      {
        lerngruppenTerminViewModel.TerminBeschreibung = terminViewModel.TerminBeschreibung;
        lerngruppenTerminViewModel.TerminErsteUnterrichtsstunde = terminViewModel.TerminErsteUnterrichtsstunde;
        lerngruppenTerminViewModel.TerminLetzteUnterrichtsstunde = terminViewModel.TerminLetzteUnterrichtsstunde;
        lerngruppenTerminViewModel.TerminTermintyp = terminViewModel.TerminTermintyp;
        lerngruppenTerminViewModel.TerminOrt = terminViewModel.TerminOrt;
      }
      tagesplan.UpdateBeschreibung();
    }

    private static void RemoveTerminFromTagesplan(SchulterminViewModel terminViewModel, TagesplanViewModel tagesplan)
    {
      if (tagesplan == null) return;
      if (tagesplan.Lerngruppentermine.All(vm => vm.TerminBeschreibung != terminViewModel.TerminBeschreibung))
      {
        // nothing to do
        return;
      }

      var lerngruppenTerminViewModel =
        tagesplan.Lerngruppentermine.Single(
        vm => vm.TerminBeschreibung == terminViewModel.TerminBeschreibung);

      if (tagesplan.Lerngruppentermine.Contains(lerngruppenTerminViewModel))
      {
        tagesplan.DeleteLerngruppentermin(lerngruppenTerminViewModel);
      }

      tagesplan.UpdateBeschreibung();
    }

    private static void AddTerminToTagesplan(TagesplanViewModel tagesplan, SchulterminViewModel terminViewModel)
    {
      if (tagesplan == null)
      {
        return;
      }

      if (tagesplan.Lerngruppentermine.Any(o => o.TerminBeschreibung == terminViewModel.TerminBeschreibung))
      {
        // already added by another method
        return;
      }

      var lerngruppenTermin = new Lerngruppentermin();
      lerngruppenTermin.Beschreibung = terminViewModel.TerminBeschreibung;
      lerngruppenTermin.ErsteUnterrichtsstunde = terminViewModel.TerminErsteUnterrichtsstunde.Model;
      lerngruppenTermin.LetzteUnterrichtsstunde = terminViewModel.TerminLetzteUnterrichtsstunde.Model;
      lerngruppenTermin.Termintyp = terminViewModel.TerminTermintyp.Model;
      lerngruppenTermin.Ort = terminViewModel.TerminOrt;
      lerngruppenTermin.Tagesplan = tagesplan.Model;

      var lerngruppenTerminViewModel = new LerngruppenterminViewModel(tagesplan, lerngruppenTermin);

      if (!tagesplan.Lerngruppentermine.Contains(lerngruppenTerminViewModel))
      {
        //App.UnitOfWork.Context.Termine.Add(lerngruppenTermin);
        //App.MainViewModel.Lerngruppentermine.Add(lerngruppenTerminViewModel);
        tagesplan.Lerngruppentermine.Add(lerngruppenTerminViewModel);
      }

      tagesplan.UpdateBeschreibung();
    }

    /// <summary>
    /// Filtert die Terminliste nach Jahrtyp und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool CustomFilter(object item)
    {
      var schultermin = item as SchulterminViewModel;
      if (this.jahrtypFilter != null && this.termintypFilter != null)
      {
        return schultermin.SchulterminJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung
          && schultermin.TerminTermintyp.TermintypBezeichnung == this.termintypFilter.TermintypBezeichnung;
      }

      if (this.jahrtypFilter != null)
      {
        return schultermin.SchulterminJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung;
      }

      if (this.termintypFilter != null)
      {
        return schultermin.TerminTermintyp.TermintypBezeichnung == this.termintypFilter.TermintypBezeichnung;
      }

      return true;
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
        this.JahrtypFilter = Selection.Instance.Jahrtyp;
      }
    }

    /// <summary>
    /// Handles addition a new Termin to the workspace and model
    /// </summary>
    private void AddTermin()
    {
      var termin = new Schultermin();
      termin.Beschreibung = "Neuer Termin";
      termin.Datum = ZuletztVerwendetesDatum;
      termin.ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[0].Model;
      var letzte = Math.Min(App.MainViewModel.Unterrichtsstunden.Count - 1, 9);
      termin.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[letzte].Model;
      if (Selection.Instance.Jahrtyp != null)
      {
        termin.Jahrtyp = Selection.Instance.Jahrtyp.Model;
      }

      termin.Termintyp = App.MainViewModel.Termintypen[0].Model;
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
        var termin = new Schultermin();
        termin.Beschreibung = this.CurrentTermin.TerminBeschreibung;
        termin.Datum = aktuellesDatum;
        termin.ErsteUnterrichtsstunde = this.CurrentTermin.TerminErsteUnterrichtsstunde.Model;
        termin.LetzteUnterrichtsstunde = this.CurrentTermin.TerminLetzteUnterrichtsstunde.Model;
        termin.Jahrtyp = this.CurrentTermin.SchulterminJahrtyp.Model;
        termin.Termintyp = this.CurrentTermin.TerminTermintyp.Model;
        //App.UnitOfWork.Context.Termine.Add(termin);

        var vm = new SchulterminViewModel(termin);
        foreach (var betroffeneKlasseViewModel in this.CurrentTermin.BetroffeneKlassen)
        {
          var betroffeneKlasse = new BetroffeneKlasse();
          betroffeneKlasse.Klasse = betroffeneKlasseViewModel.BetroffeneKlasseKlasse.Model;
          betroffeneKlasse.Termin = termin;
          var viemModelBetroffeneKlasse = new BetroffeneKlasseViewModel(betroffeneKlasse);
          vm.BetroffeneKlassen.Add(viemModelBetroffeneKlasse);
          App.MainViewModel.BetroffeneKlassen.Add(viemModelBetroffeneKlasse);
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