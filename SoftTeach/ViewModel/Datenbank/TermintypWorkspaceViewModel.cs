namespace SoftTeach.ViewModel.Datenbank
{
  using System.Linq;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class TermintypWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Termintyp currently selected
    /// </summary>
    private TermintypViewModel currentTermintyp;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TermintypWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public TermintypWorkspaceViewModel()
    {
      this.AddTermintypCommand = new DelegateCommand(this.AddTermintyp);
      this.DeleteTermintypCommand = new DelegateCommand(this.DeleteCurrentTermintyp, () => this.CurrentTermintyp != null);

      this.CurrentTermintyp = App.MainViewModel.Termintypen.Count > 0 ? App.MainViewModel.Termintypen[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Termintypen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentTermintyp))
        {
          this.CurrentTermintyp = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Termintyp
    /// </summary>
    public DelegateCommand AddTermintypCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Termintyp
    /// </summary>
    public DelegateCommand DeleteTermintypCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die termintyp currently selected in this workspace
    /// </summary>
    public TermintypViewModel CurrentTermintyp
    {
      get
      {
        return this.currentTermintyp;
      }

      set
      {
        this.currentTermintyp = value;
        this.RaisePropertyChanged("CurrentTermintyp");
        this.DeleteTermintypCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Termintyp to the workspace and model
    /// </summary>
    private void AddTermintyp()
    {
      var termintyp = new Termintyp();
 
      // Check for existing tagesplan
      if (App.MainViewModel.Termintypen.Any(vorhandenerTermintyp => vorhandenerTermintyp.TermintypBezeichnung == termintyp.Bezeichnung))
      {
        Log.ProcessMessage(
          "Termintyp bereits vorhanden",
          "Dieser Termintyp ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      // App.UnitOfWork.GetRepository<Termintyp>().Add(termintyp);

      var vm = new TermintypViewModel(termintyp);

      App.MainViewModel.Termintypen.Add(vm);
      this.CurrentTermintyp = vm;
    }

    /// <summary>
    /// Handles deletion of the current Termintyp
    /// </summary>
    private void DeleteCurrentTermintyp()
    {
      // App.UnitOfWork.GetRepository<Termintyp>().RemoveTest(this.CurrentTermintyp.Model);
      App.MainViewModel.Termintypen.RemoveTest(this.CurrentTermintyp);
      this.CurrentTermintyp = null;
    }
  }
}
