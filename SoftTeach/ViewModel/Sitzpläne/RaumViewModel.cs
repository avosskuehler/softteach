namespace SoftTeach.ViewModel.Sitzpläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Sitzpläne;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual raum
  /// </summary>
  public class RaumViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Die momentan ausgewählte Raumplan
    /// </summary>
    private RaumplanViewModel currentRaumplan;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="RaumViewModel"/> Klasse. 
    /// </summary>
    /// <param name="raum">
    /// The underlying raum this ViewModel is to be based on
    /// </param>
    public RaumViewModel(RaumNeu raum)
    {
      if (raum == null)
      {
        throw new ArgumentNullException("raum");
      }

      this.Model = raum;

      this.AddRaumplanCommand = new DelegateCommand(this.AddRaumplan);
      this.EditRaumplanCommand = new DelegateCommand(this.EditRaumplan, () => this.CurrentRaumplan != null);
      this.DeleteRaumplanCommand = new DelegateCommand(this.DeleteCurrentRaumplan, () => this.CurrentRaumplan != null);

      this.Raumpläne = new ObservableCollection<RaumplanViewModel>();
      foreach (var raumplan in raum.Raumpläne)
      {
        var vm = new RaumplanViewModel(raumplan);
        //App.MainViewModel.Raumpläne.Add(vm);
        this.Raumpläne.Add(vm);
      }

      this.Raumpläne.CollectionChanged += this.RaumpläneCollectionChanged;
    }

    /// <summary>
    /// Holt den Befehl zur Erstellung eines neuen Raumplanes
    /// </summary>
    public DelegateCommand AddRaumplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Bearbeitung eines Raumplanes
    /// </summary>
    public DelegateCommand EditRaumplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Löschung eines Raumplanes
    /// </summary>
    public DelegateCommand DeleteRaumplanCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Raum this ViewModel is based on
    /// </summary>
    public RaumNeu Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Raumpläne dieser Raumplan
    /// </summary>
    public ObservableCollection<RaumplanViewModel> Raumpläne { get; set; }

    /// <summary>
    /// Holt oder setzt die currently selected phase
    /// </summary>
    public RaumplanViewModel CurrentRaumplan
    {
      get
      {
        return this.currentRaumplan;
      }

      set
      {
        this.currentRaumplan = value;
        this.RaisePropertyChanged("Raumplan");
        this.DeleteRaumplanCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string RaumBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "RaumBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("RaumBezeichnung");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Raum: " + this.RaumBezeichnung;
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="viewModel">The object to be compared with this instance</param>
    /// <returns>Less than zero if This object is less than the other parameter. 
    /// Zero if This object is equal to other. Greater than zero if This object is greater than other.
    /// </returns>
    public int CompareTo(object viewModel)
    {
      var compareRaum = viewModel as RaumViewModel;
      if (compareRaum == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareRaum.RaumBezeichnung, StringComparison.Ordinal);
    }

    /// <summary>
    /// Handles addition a new phase to this raumplan
    /// </summary>
    private void AddRaumplan()
    {
      var raumplan = new RaumplanNeu();
      raumplan.Raum = this.Model;
      var raumplanViewModel = new RaumplanViewModel(raumplan);
      bool undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Neuer Raumplan {0} erstellt.", raumplanViewModel), false))
      {
        var dlg = new EditRaumplanDialog(raumplanViewModel);
        if (!dlg.ShowDialog().GetValueOrDefault(false))
        {
          undo = true;
        }

        if (!undo)
        {
          //App.UnitOfWork.Context.Raumpläne.Add(raumplanViewModel.Model);
          //App.MainViewModel.Raumpläne.Add(raumplanViewModel);
          this.Raumpläne.Add(raumplanViewModel);
          this.CurrentRaumplan = raumplanViewModel;
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Ruft den Raumpläne Dialog zur Beraumplanung auf
    /// </summary>
    private void EditRaumplan()
    {
      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Raumplan {0} geändert.", this.CurrentRaumplan), false))
      {
        var dlg = new EditRaumplanDialog(this.CurrentRaumplan);
        undo = !dlg.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }


    /// <summary>
    /// Handles deletion of the current phase
    /// </summary>
    private void DeleteCurrentRaumplan()
    {
      this.DeleteRaumplan(this.CurrentRaumplan);
    }

    /// <summary>
    /// Handles deletion of the given phase
    /// </summary>
    /// <param name="raumplanViewModel"> The raumplan View Model. </param>
    private void DeleteRaumplan(RaumplanViewModel raumplanViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Raumplan {0} gelöscht.", raumplanViewModel), false))
      {
        //App.MainViewModel.Raumpläne.RemoveTest(raumplanViewModel);
        //App.UnitOfWork.Context.Raumpläne.Remove(raumplanViewModel.Model);
        var result = this.Raumpläne.RemoveTest(raumplanViewModel);
      }
    }

    /// <summary>
    /// Tritt auf, wenn die ModelCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void RaumpläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Raumpläne", this.Raumpläne, e, true, "Änderung der Raumpläne");
    }
  }
}
