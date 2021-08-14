namespace SoftTeach.ViewModel.Datenbank
{
  using System.Linq;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class MediumWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Medium currently selected
    /// </summary>
    private MediumViewModel currentMedium;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MediumWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public MediumWorkspaceViewModel()
    {
      this.AddMediumCommand = new DelegateCommand(this.AddMedium);
      this.DeleteMediumCommand = new DelegateCommand(this.DeleteCurrentMedium, () => this.CurrentMedium != null);

      this.CurrentMedium = App.MainViewModel.Medien.Count > 0 ? App.MainViewModel.Medien[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Medien.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentMedium))
        {
          this.CurrentMedium = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Medium
    /// </summary>
    public DelegateCommand AddMediumCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Medium
    /// </summary>
    public DelegateCommand DeleteMediumCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die bepunktungstyp currently selected in this workspace
    /// </summary>
    public MediumViewModel CurrentMedium
    {
      get
      {
        return this.currentMedium;
      }

      set
      {
        this.currentMedium = value;
        this.RaisePropertyChanged("CurrentMedium");
        this.DeleteMediumCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Medium to the workspace and model
    /// </summary>
    private void AddMedium()
    {
      var medium = new Medium();

      // Check for existing tagesplan
      if (App.MainViewModel.Medien.Any(vorhandenerMedium => vorhandenerMedium.MediumBezeichnung == medium.Bezeichnung))
      {
        Log.ProcessMessage("Medium bereits vorhanden",
          "Dieses Medium ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      // App.UnitOfWork.GetRepository<Medium>().Add(medium);

      var vm = new MediumViewModel(medium);

      App.MainViewModel.Medien.Add(vm);
      this.CurrentMedium = vm;
    }

    /// <summary>
    /// Handles deletion of the current Medium
    /// </summary>
    private void DeleteCurrentMedium()
    {
      // App.UnitOfWork.GetRepository<Medium>().RemoveTest(this.CurrentMedium.Model);
      App.MainViewModel.Medien.RemoveTest(this.CurrentMedium);
      this.CurrentMedium = null;
    }
  }
}
