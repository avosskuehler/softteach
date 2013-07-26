namespace Liduv.ViewModel.Datenbank
{
  using System.Linq;
  using System.Windows.Input;

  using Liduv.ExceptionHandling;
  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class UnterrichtsstundeWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Unterrichtsstunde currently selected
    /// </summary>
    private UnterrichtsstundeViewModel currentUnterrichtsstunde;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="UnterrichtsstundeWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public UnterrichtsstundeWorkspaceViewModel()
    {
      this.AddUnterrichtsstundeCommand = new DelegateCommand(this.AddUnterrichtsstunde);
      this.DeleteUnterrichtsstundeCommand = new DelegateCommand(this.DeleteCurrentUnterrichtsstunde, () => this.CurrentUnterrichtsstunde != null);

      this.CurrentUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.Count > 0 ? App.MainViewModel.Unterrichtsstunden[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Unterrichtsstunden.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentUnterrichtsstunde))
        {
          this.CurrentUnterrichtsstunde = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Unterrichtsstunde
    /// </summary>
    public DelegateCommand AddUnterrichtsstundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Unterrichtsstunde
    /// </summary>
    public DelegateCommand DeleteUnterrichtsstundeCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die unterrichtsstunde currently selected in this workspace
    /// </summary>
    public UnterrichtsstundeViewModel CurrentUnterrichtsstunde
    {
      get
      {
        return this.currentUnterrichtsstunde;
      }

      set
      {
        this.currentUnterrichtsstunde = value;
        this.RaisePropertyChanged("CurrentUnterrichtsstunde");
        this.DeleteUnterrichtsstundeCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Unterrichtsstunde to the workspace and model
    /// </summary>
    private void AddUnterrichtsstunde()
    {
      var unterrichtsstunde = new Unterrichtsstunde();

      // Check for existing jahresplan
      if (App.MainViewModel.Unterrichtsstunden.Any(vorhandenesUnterrichtsstunde => vorhandenesUnterrichtsstunde.UnterrichtsstundeBezeichnung == unterrichtsstunde.Bezeichnung))
      {
        Log.ProcessMessage("Unterrichtsstunde bereits vorhanden",
          "Dieses Unterrichtsstunde ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      // App.UnitOfWork.GetRepository<Unterrichtsstunde>().Add(unterrichtsstunde);

      var vm = new UnterrichtsstundeViewModel(unterrichtsstunde);

      App.MainViewModel.Unterrichtsstunden.Add(vm);
      this.CurrentUnterrichtsstunde = vm;
    }

    /// <summary>
    /// Handles deletion of the current Unterrichtsstunde
    /// </summary>
    private void DeleteCurrentUnterrichtsstunde()
    {
      // App.UnitOfWork.GetRepository<Unterrichtsstunde>().RemoveTest(this.CurrentUnterrichtsstunde.Model);
      App.MainViewModel.Unterrichtsstunden.RemoveTest(this.CurrentUnterrichtsstunde);
      this.CurrentUnterrichtsstunde = null;
    }
  }
}
