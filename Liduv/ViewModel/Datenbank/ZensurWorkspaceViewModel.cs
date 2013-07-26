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
  public class ZensurWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Zensur currently selected
    /// </summary>
    private ZensurViewModel currentZensur;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ZensurWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public ZensurWorkspaceViewModel()
    {
      this.AddZensurCommand = new DelegateCommand(this.AddZensur);
      this.DeleteZensurCommand = new DelegateCommand(this.DeleteCurrentZensur, () => this.CurrentZensur != null);

      this.CurrentZensur = App.MainViewModel.Zensuren.Count > 0 ? App.MainViewModel.Zensuren[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Zensuren.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentZensur))
        {
          this.CurrentZensur = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Zensur
    /// </summary>
    public DelegateCommand AddZensurCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Zensur
    /// </summary>
    public DelegateCommand DeleteZensurCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die zensur currently selected in this workspace
    /// </summary>
    public ZensurViewModel CurrentZensur
    {
      get
      {
        return this.currentZensur;
      }

      set
      {
        this.currentZensur = value;
        this.RaisePropertyChanged("CurrentZensur");
        this.DeleteZensurCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Zensur to the workspace and model
    /// </summary>
    private void AddZensur()
    {
      var zensur = new Zensur();
 
      // Check for existing zensur
      if (App.MainViewModel.Zensuren.Any(vorhandenerZensur => vorhandenerZensur.ZensurNotenpunkte == zensur.Notenpunkte))
      {
        Log.ProcessMessage(
          "Zensur bereits vorhanden",
          "Dieses Zensur ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      // App.UnitOfWork.GetRepository<Zensur>().Add(zensur);
      var vm = new ZensurViewModel(zensur);
      App.MainViewModel.Zensuren.Add(vm);
      this.CurrentZensur = vm;
    }

    /// <summary>
    /// Handles deletion of the current Zensur
    /// </summary>
    private void DeleteCurrentZensur()
    {
      // App.UnitOfWork.GetRepository<Zensur>().RemoveTest(this.CurrentZensur.Model);
      App.MainViewModel.Zensuren.RemoveTest(this.CurrentZensur);
      this.CurrentZensur = null;
    }
  }
}
