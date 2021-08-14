namespace SoftTeach.ViewModel.Datenbank
{
  using System.Linq;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class SozialformWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Sozialform currently selected
    /// </summary>
    private SozialformViewModel currentSozialform;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SozialformWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SozialformWorkspaceViewModel()
    {
      this.AddSozialformCommand = new DelegateCommand(this.AddSozialform);
      this.DeleteSozialformCommand = new DelegateCommand(this.DeleteCurrentSozialform, () => this.CurrentSozialform != null);

      this.CurrentSozialform = App.MainViewModel.Sozialformen.Count > 0 ? App.MainViewModel.Sozialformen[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Sozialformen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentSozialform))
        {
          this.CurrentSozialform = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Sozialform
    /// </summary>
    public DelegateCommand AddSozialformCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Sozialform
    /// </summary>
    public DelegateCommand DeleteSozialformCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die sozialform currently selected in this workspace
    /// </summary>
    public SozialformViewModel CurrentSozialform
    {
      get
      {
        return this.currentSozialform;
      }

      set
      {
        this.currentSozialform = value;
        this.RaisePropertyChanged("CurrentSozialform");
        this.DeleteSozialformCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Sozialform to the workspace and model
    /// </summary>
    private void AddSozialform()
    {
      var sozialform = new Sozialform();
 
      // Check for existing tagesplan
      if (App.MainViewModel.Sozialformen.Any(vorhandenerSozialform => vorhandenerSozialform.SozialformBezeichnung == sozialform.Bezeichnung))
      {
        Log.ProcessMessage(
          "Sozialform bereits vorhanden",
          "Dieser Sozialform ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      // App.UnitOfWork.GetRepository<Sozialform>().Add(sozialform);
      var vm = new SozialformViewModel(sozialform);
      App.MainViewModel.Sozialformen.Add(vm);
      this.CurrentSozialform = vm;
    }

    /// <summary>
    /// Handles deletion of the current Sozialform
    /// </summary>
    private void DeleteCurrentSozialform()
    {
      // App.UnitOfWork.GetRepository<Sozialform>().RemoveTest(this.CurrentSozialform.Model);
      App.MainViewModel.Sozialformen.RemoveTest(this.CurrentSozialform);
      this.CurrentSozialform = null;
    }
  }
}
