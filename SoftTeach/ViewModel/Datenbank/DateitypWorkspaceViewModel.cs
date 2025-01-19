namespace SoftTeach.ViewModel.Datenbank
{
  using System.Linq;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class DateitypWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Dateityp currently selected
    /// </summary>
    private DateitypViewModel currentDateityp;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="DateitypWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public DateitypWorkspaceViewModel()
    {
      this.AddDateitypCommand = new DelegateCommand(this.AddDateityp);
      this.DeleteDateitypCommand = new DelegateCommand(this.DeleteCurrentDateityp, () => this.CurrentDateityp != null);

      this.CurrentDateityp = App.MainViewModel.Dateitypen.Count > 0 ? App.MainViewModel.Dateitypen[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Dateitypen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentDateityp))
        {
          this.CurrentDateityp = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Dateityp
    /// </summary>
    public DelegateCommand AddDateitypCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Dateityp
    /// </summary>
    public DelegateCommand DeleteDateitypCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die dateityp currently selected in this workspace
    /// </summary>
    public DateitypViewModel CurrentDateityp
    {
      get
      {
        return this.currentDateityp;
      }

      set
      {
        this.currentDateityp = value;
        this.RaisePropertyChanged("CurrentDateityp");
        this.DeleteDateitypCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Dateityp to the workspace and model
    /// </summary>
    private void AddDateityp()
    {
      var dateityp = new Dateityp();

      // Check for existing tagesplan
      if (App.MainViewModel.Dateitypen.Any(vorhandenerDateityp => vorhandenerDateityp.DateitypBezeichnung == dateityp.Bezeichnung))
      {
        Log.ProcessMessage("Dateityp bereits vorhanden",
          "Dieser Dateityp ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      var vm = new DateitypViewModel(dateityp);
      App.MainViewModel.Dateitypen.Add(vm);
      this.CurrentDateityp = vm;
    }

    /// <summary>
    /// Handles deletion of the current Dateityp
    /// </summary>
    private void DeleteCurrentDateityp()
    {
      App.MainViewModel.Dateitypen.RemoveTest(this.CurrentDateityp);
      this.CurrentDateityp = null;
    }
  }
}
