namespace Liduv.ViewModel.Datenbank
{
  using System.Windows.Input;

  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class FachstundenanzahlWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Fachstundenanzahl currently selected
    /// </summary>
    private FachstundenanzahlViewModel currentFachstundenanzahl;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="FachstundenanzahlWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public FachstundenanzahlWorkspaceViewModel()
    {
      this.AddFachstundenanzahlCommand = new DelegateCommand(this.AddFachstundenanzahl);
      this.DeleteFachstundenanzahlCommand = new DelegateCommand(this.DeleteCurrentFachstundenanzahl, () => this.CurrentFachstundenanzahl != null);
      
      this.CurrentFachstundenanzahl = App.MainViewModel.Fachstundenanzahl.Count > 0 ? App.MainViewModel.Fachstundenanzahl[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Fachstundenanzahl.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentFachstundenanzahl))
        {
          this.CurrentFachstundenanzahl = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Fachstundenanzahl
    /// </summary>
    public DelegateCommand AddFachstundenanzahlCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Fachstundenanzahl
    /// </summary>
    public DelegateCommand DeleteFachstundenanzahlCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die ferien currently selected in this workspace
    /// </summary>
    public FachstundenanzahlViewModel CurrentFachstundenanzahl
    {
      get
      {
        return this.currentFachstundenanzahl;
      }

      set
      {
        this.currentFachstundenanzahl = value;
        this.RaisePropertyChanged("CurrentFachstundenanzahl");
        this.DeleteFachstundenanzahlCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Fachstundenanzahl to the workspace and model
    /// </summary>
    private void AddFachstundenanzahl()
    {
      var fachstundenanzahl = new Fachstundenanzahl();

      // App.UnitOfWork.GetRepository<Fachstundenanzahl>().Add(fachstundenanzahl);
      fachstundenanzahl.Fach = Selection.Instance.Fach.Model;
      fachstundenanzahl.Klassenstufe = App.MainViewModel.Klassenstufen[0].Model;
      fachstundenanzahl.Stundenzahl = 4;
      fachstundenanzahl.Teilungsstundenzahl = 0;

      var vm = new FachstundenanzahlViewModel(fachstundenanzahl);

      App.MainViewModel.Fachstundenanzahl.Add(vm);
      this.CurrentFachstundenanzahl = vm;
    }

    /// <summary>
    /// Handles deletion of the current Fachstundenanzahl
    /// </summary>
    private void DeleteCurrentFachstundenanzahl()
    {
      // App.UnitOfWork.GetRepository<Fachstundenanzahl>().RemoveTest(this.CurrentFachstundenanzahl.Model);
      App.MainViewModel.Fachstundenanzahl.RemoveTest(this.CurrentFachstundenanzahl);
      this.CurrentFachstundenanzahl = null;
    }
  }
}
