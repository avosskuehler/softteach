namespace SoftTeach.ViewModel.Datenbank
{
  using System.Linq;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class TendenzWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Tendenz currently selected
    /// </summary>
    private TendenzViewModel currentTendenz;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TendenzWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public TendenzWorkspaceViewModel()
    {
      this.AddTendenzCommand = new DelegateCommand(this.AddTendenz);
      this.DeleteTendenzCommand = new DelegateCommand(this.DeleteCurrentTendenz, () => this.CurrentTendenz != null);
      
      this.CurrentTendenz = App.MainViewModel.Tendenzen.Count > 0 ? App.MainViewModel.Tendenzen[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Tendenzen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentTendenz))
        {
          this.CurrentTendenz = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Tendenz
    /// </summary>
    public DelegateCommand AddTendenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Tendenz
    /// </summary>
    public DelegateCommand DeleteTendenzCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die tendenz currently selected in this workspace
    /// </summary>
    public TendenzViewModel CurrentTendenz
    {
      get
      {
        return this.currentTendenz;
      }

      set
      {
        this.currentTendenz = value;
        this.RaisePropertyChanged("CurrentTendenz");
        this.DeleteTendenzCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Tendenz to the workspace and model
    /// </summary>
    private void AddTendenz()
    {
      var tendenz = new Tendenz();
 
      // Check for existing tagesplan
      if (App.MainViewModel.Tendenzen.Any(vorhandenerTendenz => vorhandenerTendenz.TendenzBezeichnung == tendenz.Bezeichnung))
      {
        Log.ProcessMessage(
          "Tendenz bereits vorhanden",
          "Dieses Tendenz ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      // App.UnitOfWork.GetRepository<Tendenz>().Add(tendenz);
      var vm = new TendenzViewModel(tendenz);
      App.MainViewModel.Tendenzen.Add(vm);
      this.CurrentTendenz = vm;
    }

    /// <summary>
    /// Handles deletion of the current Tendenz
    /// </summary>
    private void DeleteCurrentTendenz()
    {
      // App.UnitOfWork.GetRepository<Tendenz>().RemoveTest(this.CurrentTendenz.Model);
      App.MainViewModel.Tendenzen.RemoveTest(this.CurrentTendenz);
      this.CurrentTendenz = null;
    }
  }
}
