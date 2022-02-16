namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

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
    /// The betroffeneKlasse currently selected
    /// </summary>
    private BetroffeneKlasseViewModel currentBetroffeneKlasse;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchulterminViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schultermin">
    /// The underlying termin this ViewModel is to be based on
    /// </param>
    public SchulterminViewModel(SchulterminNeu schultermin)
      : base(schultermin)
    {
      // Build data structures for betroffeneKlassen
      this.BetroffeneKlassen = new ObservableCollection<BetroffeneKlasseViewModel>();
      foreach (var betroffeneKlasse in schultermin.BetroffeneKlassen)
      {
        var vm = new BetroffeneKlasseViewModel(betroffeneKlasse);
        App.MainViewModel.BetroffeneKlassen.Add(vm);
        this.BetroffeneKlassen.Add(vm);
      }

      this.BetroffeneKlassen.CollectionChanged += this.BetroffeneKlassenCollectionChanged;

      this.PropertyChanging += this.TerminViewModelPropertyChanging;

      this.AddBetroffeneKlasseCommand = new DelegateCommand(this.AddBetroffeneKlasse);
      this.DeleteBetroffeneKlasseCommand = new DelegateCommand(this.DeleteCurrentBetroffeneKlasse, () => this.CurrentBetroffeneKlasse != null);
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Email address
    /// </summary>
    public DelegateCommand AddBetroffeneKlasseCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current employee
    /// </summary>
    public DelegateCommand DeleteBetroffeneKlasseCommand { get; private set; }

    ///// <summary>
    ///// Holt den underlying Termin this ViewModel is based on
    ///// </summary>
    //public override Schultermin Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected betroffeneKlasse
    /// </summary>
    public BetroffeneKlasseViewModel CurrentBetroffeneKlasse
    {
      get
      {
        return this.currentBetroffeneKlasse;
      }

      set
      {
        this.currentBetroffeneKlasse = value;
        this.RaisePropertyChanged("CurrentBetroffeneKlasse");
        this.DeleteBetroffeneKlasseCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Datum
    /// </summary>
    public DateTime SchulterminDatum
    {
      get
      {
        return ((SchulterminNeu)this.Model).Datum;
      }

      set
      {
        if (value == ((SchulterminNeu)this.Model).Datum) return;
        this.UndoablePropertyChanging(this, "SchulterminDatum", ((SchulterminNeu)this.Model).Datum, value);
        ((SchulterminNeu)this.Model).Datum = value;
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
        if (((SchulterminNeu)this.Model).Schuljahr == null)
        {
          return null;
        }

        if (this.schuljahr == null || this.schuljahr.Model != ((SchulterminNeu)this.Model).Schuljahr)
        {
          this.schuljahr = App.MainViewModel.Schuljahre.SingleOrDefault(d => d.Model == ((SchulterminNeu)this.Model).Schuljahr);
        }

        return this.schuljahr;
      }

      set
      {
        if (value.SchuljahrBezeichnung == this.schuljahr.SchuljahrBezeichnung) return;
        this.UndoablePropertyChanging(this, "SchulterminSchuljahr", this.schuljahr, value);
        this.schuljahr = value;
        ((SchulterminNeu)this.Model).Schuljahr = value.Model;
        this.RaisePropertyChanged("SchulterminSchuljahr");
      }
    }

    /// <summary>
    /// Holt den betroffeneKlassen on file for this termin
    /// </summary>
    public ObservableCollection<BetroffeneKlasseViewModel> BetroffeneKlassen { get; private set; }

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
        var dlg = new EditSchulterminDialog { DataContext = this };
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
    /// Handles addition a new betroffeneKlasse to this termin
    /// </summary>
    private void AddBetroffeneKlasse()
    {
      var dlg = new BetroffeneKlassenDialog();

      foreach (var betroffeneKlasse in this.BetroffeneKlassen)
      {
        dlg.Klassen.Add(betroffeneKlasse.BetroffeneKlasseLerngruppe);
      }

      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Betroffene Klassen des Termins {0} verändert.", this), false))
        {
          // Remove deselected Klassen
          var klassenToRemove = new List<BetroffeneKlasseViewModel>();
          foreach (var bereitsGewählteKlasse in this.BetroffeneKlassen)
          {
            var found =
              dlg.Klassen.Any(
                o => bereitsGewählteKlasse.BetroffeneKlasseLerngruppe.LerngruppeBezeichnung == o.LerngruppeBezeichnung);

            if (!found)
            {
              // Delete
              klassenToRemove.Add(bereitsGewählteKlasse);
            }
          }

          foreach (var betroffeneKlasseViewModel in klassenToRemove)
          {
            this.BetroffeneKlassen.RemoveTest(betroffeneKlasseViewModel);
          }

          foreach (var klasse in dlg.Klassen)
          {
            // Check for already existing klassen
            var skip =
              this.BetroffeneKlassen.Any(o => o.BetroffeneKlasseLerngruppe.LerngruppeBezeichnung == klasse.LerngruppeBezeichnung);

            if (!skip)
            {
              var betroffeneKlasse = new BetroffeneKlasseNeu();
              betroffeneKlasse.Lerngruppe = klasse.Model;
              betroffeneKlasse.Schultermin = this.Model as SchulterminNeu;

              var vm = new BetroffeneKlasseViewModel(betroffeneKlasse);
              //App.UnitOfWork.Context.BetroffeneKlassen.Add(betroffeneKlasse);
              App.MainViewModel.BetroffeneKlassen.Add(vm);
              this.BetroffeneKlassen.Add(vm);
              this.CurrentBetroffeneKlasse = vm;
            }
          }
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current betroffeneKlasse
    /// </summary>
    private void DeleteCurrentBetroffeneKlasse()
    {
      //App.UnitOfWork.Context.BetroffeneKlassen.Remove(this.CurrentBetroffeneKlasse.Model);
      App.MainViewModel.BetroffeneKlassen.RemoveTest(this.CurrentBetroffeneKlasse);
      this.BetroffeneKlassen.RemoveTest(this.CurrentBetroffeneKlasse);
      this.CurrentBetroffeneKlasse = null;
    }

    /// <summary>
    /// Tritt auf, wenn die BetroffeneKlassenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void BetroffeneKlassenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      SchulterminWorkspaceViewModel.AddToModifiedList(this, SchulterminUpdateType.BetroffeneKlasseChanged, e);
      this.UndoableCollectionChanged(this, "BetroffeneKlassen", this.BetroffeneKlassen, e, "Änderung der BetroffeneKlassen");
    }

    /// <summary>
    /// The termin view model property changed extended.
    /// </summary>
    /// <param name="sender"> The sender. </param> 
    /// <param name="e"> The e. </param>
    private void TerminViewModelPropertyChanging(object sender, PropertyChangingEventArgs e)
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
