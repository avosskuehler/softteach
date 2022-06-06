namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// ViewModel of an individual schultermin
  /// </summary>
  public class SchulterminViewModel : TerminViewModel
  {
    /// <summary>
    /// The schuljahr currently assigned to this termin
    /// </summary>
    private SchuljahrViewModel schuljahr;

    /// <summary>
    /// The betroffeneLerngruppe currently selected
    /// </summary>
    private BetroffeneLerngruppeViewModel currentBetroffeneLerngruppe;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="SchulterminViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schultermin">
    /// The underlying termin this ViewModel is to be based on
    /// </param>
    public SchulterminViewModel(Schultermin schultermin)
      : base(schultermin)
    {
      this.AddBetroffeneLerngruppeCommand = new DelegateCommand(this.AddBetroffeneLerngruppe);
      this.DeleteBetroffeneLerngruppeCommand = new DelegateCommand(this.DeleteCurrentBetroffeneLerngruppe, () => this.CurrentBetroffeneLerngruppe != null);

      // Build data structures for betroffeneLerngruppen
      this.BetroffeneLerngruppen = new ObservableCollection<BetroffeneLerngruppeViewModel>();
      foreach (var betroffeneLerngruppe in schultermin.BetroffeneLerngruppen)
      {
        var vm = new BetroffeneLerngruppeViewModel(betroffeneLerngruppe);
        //App.MainViewModel.BetroffeneKlassen.Add(vm);
        this.BetroffeneLerngruppen.Add(vm);
      }


      this.BetroffeneLerngruppen.CollectionChanged += this.BetroffeneLerngruppenCollectionChanged;

      this.PropertyChanging += this.TerminViewModelPropertyChanging;

    }

    /// <summary>
    /// Holt den Befehl zur adding a new Email address
    /// </summary>
    public DelegateCommand AddBetroffeneLerngruppeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current employee
    /// </summary>
    public DelegateCommand DeleteBetroffeneLerngruppeCommand { get; private set; }

    ///// <summary>
    ///// Holt den underlying Termin this ViewModel is based on
    ///// </summary>
    //public override Schultermin Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected betroffeneLerngruppe
    /// </summary>
    public BetroffeneLerngruppeViewModel CurrentBetroffeneLerngruppe
    {
      get
      {
        return this.currentBetroffeneLerngruppe;
      }

      set
      {
        this.currentBetroffeneLerngruppe = value;
        this.RaisePropertyChanged("CurrentBetroffeneLerngruppe");
        this.DeleteBetroffeneLerngruppeCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Datum
    /// </summary>
    public DateTime SchulterminDatum
    {
      get
      {
        return ((Schultermin)this.Model).Datum;
      }

      set
      {
        if (value == ((Schultermin)this.Model).Datum) return;
        this.UndoablePropertyChanging(this, nameof(SchulterminDatum), ((Schultermin)this.Model).Datum, value);
        ((Schultermin)this.Model).Datum = value;
        SchulterminWorkspaceViewModel.ZuletztVerwendetesDatum = value;
        this.RaisePropertyChanged("SchulterminDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public SchuljahrViewModel SchulterminSchuljahr
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (((Schultermin)this.Model).Schuljahr == null)
        {
          return null;
        }

        if (this.schuljahr == null || this.schuljahr.Model != ((Schultermin)this.Model).Schuljahr)
        {
          this.schuljahr = App.MainViewModel.Schuljahre.SingleOrDefault(d => d.Model == ((Schultermin)this.Model).Schuljahr);
        }

        return this.schuljahr;
      }

      set
      {
        if (value.SchuljahrBezeichnung == this.schuljahr.SchuljahrBezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(SchulterminSchuljahr), this.schuljahr, value);
        this.schuljahr = value;
        ((Schultermin)this.Model).Schuljahr = value.Model;
        this.RaisePropertyChanged("SchulterminSchuljahr");
      }
    }

    /// <summary>
    /// Holt den betroffeneLerngruppen on file for this termin
    /// </summary>
    public ObservableCollection<BetroffeneLerngruppeViewModel> BetroffeneLerngruppen { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Schultermin: " + this.TerminBeschreibung;
    }

    /// <summary>
    /// Handles deletion of the current Lerngruppentermin
    /// </summary>
    public void EditSchultermin()
    {
      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Lerngruppentermin {0} editieren", this), false))
      {
        var dlg = new SchulterminDialog { DataContext = this };
        undo = !dlg.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Handles deletion of the current termin
    /// </summary>
    protected override void DeleteTermin()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Lösche Termin"), false))
      {
        SchulterminWorkspaceViewModel.AddToModifiedList(this, SchulterminUpdateType.Removed, null);
        //App.UnitOfWork.Context.Termine.Remove(this.Model);
        var result = App.MainViewModel.Schultermine.RemoveTest(this);
      }
    }

    /// <summary>
    /// Handles addition a new betroffeneLerngruppe to this termin
    /// </summary>
    private void AddBetroffeneLerngruppe()
    {
      var dlg = new BetroffeneKlassenDialog(this.schuljahr);

      foreach (var bl in this.BetroffeneLerngruppen)
      {
        var lg = dlg.Lerngruppen.Where(o => o.LerngruppeSchuljahr.SchuljahrJahr == this.SchulterminSchuljahr.SchuljahrJahr).FirstOrDefault(o => o.Model == bl.BetroffeneLerngruppeLerngruppe.Model);
        if (lg != null)
        {
          lg.IstBetroffen = true;
        }
      }

      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Betroffene Klassen des Termins {0} verändert.", this), false))
        {
          // Remove deselected Lerngruppen
          var lerngruppenToRemove = new List<BetroffeneLerngruppeViewModel>();
          foreach (var bereitsGewählteKlasse in this.BetroffeneLerngruppen)
          {
            var found = dlg.Lerngruppen.Where(o => o.LerngruppeSchuljahr.SchuljahrJahr == this.SchulterminSchuljahr.SchuljahrJahr).Any(o => bereitsGewählteKlasse.BetroffeneLerngruppeLerngruppe.Model == o.Model && o.IstBetroffen);

            if (!found)
            {
              // Delete
              lerngruppenToRemove.Add(bereitsGewählteKlasse);
            }
          }

          foreach (var betroffeneLerngruppeViewModel in lerngruppenToRemove)
          {
            this.BetroffeneLerngruppen.RemoveTest(betroffeneLerngruppeViewModel);
          }
        }

        foreach (var lerngruppe in dlg.Lerngruppen.Where(o => o.LerngruppeSchuljahr.SchuljahrJahr == this.SchulterminSchuljahr.SchuljahrJahr && o.IstBetroffen))
        {
          // Check for already existing klassen
          var skip = this.BetroffeneLerngruppen.Any(o => o.BetroffeneLerngruppeLerngruppe.Model == lerngruppe.Model);

          if (!skip)
          {
            var betroffeneLerngruppe = new BetroffeneLerngruppe
            {
              Lerngruppe = lerngruppe.Model,
              Schultermin = this.Model as Schultermin
            };

            var vm = new BetroffeneLerngruppeViewModel(betroffeneLerngruppe);
            //App.UnitOfWork.Context.BetroffeneKlassen.Add(betroffeneLerngruppe);
            //App.MainViewModel.BetroffeneKlassen.Add(vm);
            this.BetroffeneLerngruppen.Add(vm);
            //this.CurrentBetroffeneLerngruppe = vm;
          }
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current betroffeneLerngruppe
    /// </summary>
    private void DeleteCurrentBetroffeneLerngruppe()
    {
      //App.UnitOfWork.Context.BetroffeneKlassen.Remove(this.CurrentBetroffeneKlasse.Model);
      //App.MainViewModel.BetroffeneKlassen.RemoveTest(this.CurrentBetroffeneLerngruppe);
      this.BetroffeneLerngruppen.RemoveTest(this.CurrentBetroffeneLerngruppe);
      this.CurrentBetroffeneLerngruppe = null;
    }

    /// <summary>
    /// Tritt auf, wenn die BetroffeneKlassenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void BetroffeneLerngruppenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      SchulterminWorkspaceViewModel.AddToModifiedList(this, SchulterminUpdateType.BetroffeneKlasseChanged, e);
      UndoableCollectionChanged(this, nameof(BetroffeneLerngruppen), this.BetroffeneLerngruppen, e, true, "Änderung der BetroffeneLerngruppen");
    }

    /// <summary>
    /// The termin view model property changed extended.
    /// </summary>
    /// <param name="sender"> The sender. </param> 
    /// <param name="e"> The e. </param>
    private void TerminViewModelPropertyChanging(object sender, Helper.PropertyChangingEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "SchulterminDatum":
          SchulterminWorkspaceViewModel.AddToModifiedList(this, SchulterminUpdateType.ChangedWithNewDay, (DateTime)e.PropertyOldValue);
          break;
        case "TerminBeschreibung":
          SchulterminWorkspaceViewModel.AddToModifiedList(this, SchulterminUpdateType.ChangedBeschreibung, (string)e.PropertyOldValue);
          break;
        default:
          SchulterminWorkspaceViewModel.AddToModifiedList(this, SchulterminUpdateType.Changed, null);
          break;
      }
    }


  }
}
