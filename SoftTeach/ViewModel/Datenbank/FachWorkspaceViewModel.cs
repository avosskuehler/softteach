namespace SoftTeach.ViewModel.Datenbank
{
  using System.Linq;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class FachWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Fach currently selected
    /// </summary>
    private FachViewModel currentFach;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="FachWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public FachWorkspaceViewModel()
    {
      this.AddFachCommand = new DelegateCommand(this.AddFach);
      this.DeleteFachCommand = new DelegateCommand(this.DeleteCurrentFach, () => this.CurrentFach != null);

      this.CurrentFach = App.MainViewModel.Fächer.Count > 0 ? App.MainViewModel.Fächer[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Fächer.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentFach))
        {
          this.CurrentFach = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Fach
    /// </summary>
    public DelegateCommand AddFachCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Fach
    /// </summary>
    public DelegateCommand DeleteFachCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die fach currently selected in this workspace
    /// </summary>
    public FachViewModel CurrentFach
    {
      get
      {
        return this.currentFach;
      }

      set
      {
        this.currentFach = value;
        this.RaisePropertyChanged("CurrentFach");
        this.DeleteFachCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Fach to the workspace and model
    /// </summary>
    private void AddFach()
    {
      var fach = new Fach();

      if (App.MainViewModel.Fächer.Any(vorhandenesFach => vorhandenesFach.FachBezeichnung == fach.Bezeichnung))
      {
        Log.ProcessMessage("Fach bereits vorhanden",
          "Dieses Fach ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      var vm = new FachViewModel(fach);

      App.MainViewModel.Fächer.Add(vm);
      this.CurrentFach = vm;
    }

    /// <summary>
    /// Handles deletion of the current Fach
    /// </summary>
    private void DeleteCurrentFach()
    {
      App.MainViewModel.Fächer.RemoveTest(this.CurrentFach);
      this.CurrentFach = null;
    }
  }
}
