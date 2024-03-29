﻿namespace SoftTeach.ViewModel.Sitzpläne
{
  using System.Linq;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class RaumWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Raum currently selected
    /// </summary>
    private RaumViewModel currentRaum;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="RaumWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public RaumWorkspaceViewModel()
    {
      this.AddRaumCommand = new DelegateCommand(this.AddRaum);
      this.DeleteRaumCommand = new DelegateCommand(this.DeleteCurrentRaum, () => this.CurrentRaum != null);
      this.CopyRaumCommand = new DelegateCommand(this.CopyCurrentRaum, () => this.CurrentRaum != null);
      this.CurrentRaum = App.MainViewModel.Räume.Count > 0 ? App.MainViewModel.Räume[0] : null;

      App.MainViewModel.LoadRäume();

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Räume.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentRaum))
        {
          this.CurrentRaum = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Raum
    /// </summary>
    public DelegateCommand AddRaumCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Raum
    /// </summary>
    public DelegateCommand DeleteRaumCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Raum
    /// </summary>
    public DelegateCommand CopyRaumCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die raum currently selected in this workspace
    /// </summary>
    public RaumViewModel CurrentRaum
    {
      get
      {
        return this.currentRaum;
      }

      set
      {
        this.currentRaum = value;
        this.RaisePropertyChanged("CurrentRaum");
        this.DeleteRaumCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Raum to the workspace and model
    /// </summary>
    private void AddRaum()
    {
      var raum = new Raum();
 
      // Check for existing raum
      if (App.MainViewModel.Räume.Any(vorhandenerRaum => vorhandenerRaum.RaumBezeichnung == raum.Bezeichnung))
      {
        Log.ProcessMessage(
          "Raum bereits vorhanden",
          "Dieser Raum ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      var vm = new RaumViewModel(raum);
      App.MainViewModel.Räume.Add(vm);
      this.CurrentRaum = vm;
    }

    /// <summary>
    /// Handles deletion of the current Raum
    /// </summary>
    private void DeleteCurrentRaum()
    {
      App.MainViewModel.Räume.RemoveTest(this.CurrentRaum);
      this.CurrentRaum = null;
    }

    /// <summary>
    /// Kopiert die Sitzpläne des aktuellen Raums in einen anderen Raum.
    /// </summary>
    private void CopyCurrentRaum()
    {
      App.MainViewModel.Räume.RemoveTest(this.CurrentRaum);
      this.CurrentRaum = null;
    }
  }
}
