namespace SoftTeach.ViewModel.Datenbank
{
  using System.Linq;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class TendenztypWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Tendenztyp currently selected
    /// </summary>
    private TendenztypViewModel currentTendenztyp;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TendenztypWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public TendenztypWorkspaceViewModel()
    {
      this.AddTendenztypCommand = new DelegateCommand(this.AddTendenztyp);
      this.DeleteTendenztypCommand = new DelegateCommand(this.DeleteCurrentTendenztyp, () => this.CurrentTendenztyp != null);
      
      this.CurrentTendenztyp = App.MainViewModel.Tendenztypen.Count > 0 ? App.MainViewModel.Tendenztypen[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Tendenztypen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentTendenztyp))
        {
          this.CurrentTendenztyp = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Tendenztyp
    /// </summary>
    public DelegateCommand AddTendenztypCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Tendenztyp
    /// </summary>
    public DelegateCommand DeleteTendenztypCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die tendenztyp currently selected in this workspace
    /// </summary>
    public TendenztypViewModel CurrentTendenztyp
    {
      get
      {
        return this.currentTendenztyp;
      }

      set
      {
        this.currentTendenztyp = value;
        this.RaisePropertyChanged("CurrentTendenztyp");
        this.DeleteTendenztypCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Tendenztyp to the workspace and model
    /// </summary>
    private void AddTendenztyp()
    {
      var tendenztyp = new Tendenztyp();
 
      // Check for existing tagesplan
      if (App.MainViewModel.Tendenztypen.Any(vorhandenerTendenztyp => vorhandenerTendenztyp.TendenztypBezeichnung == tendenztyp.Bezeichnung))
      {
        Log.ProcessMessage("Tendenztyp bereits vorhanden",
          "Dieses Tendenztyp ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      // App.UnitOfWork.GetRepository<Tendenztyp>().Add(tendenztyp);

      var vm = new TendenztypViewModel(tendenztyp);

      App.MainViewModel.Tendenztypen.Add(vm);
      this.CurrentTendenztyp = vm;
    }

    /// <summary>
    /// Handles deletion of the current Tendenztyp
    /// </summary>
    private void DeleteCurrentTendenztyp()
    {
      // App.UnitOfWork.GetRepository<Tendenztyp>().RemoveTest(this.CurrentTendenztyp.Model);
      App.MainViewModel.Tendenztypen.RemoveTest(this.CurrentTendenztyp);
      this.CurrentTendenztyp = null;
    }
  }
}
