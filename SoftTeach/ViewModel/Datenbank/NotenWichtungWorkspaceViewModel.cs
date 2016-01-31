namespace SoftTeach.ViewModel.Datenbank
{
  using System.Linq;
  using System.Windows.Input;

  using SoftTeach.Model;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class NotenWichtungWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The NotenWichtung currently selected
    /// </summary>
    private NotenWichtungViewModel currentNotenWichtung;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NotenWichtungWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public NotenWichtungWorkspaceViewModel()
    {
      this.AddNotenWichtungCommand = new DelegateCommand(this.AddNotenWichtung);
      this.DeleteNotenWichtungCommand = new DelegateCommand(this.DeleteCurrentNotenWichtung, () => this.CurrentNotenWichtung != null);
      
      this.CurrentNotenWichtung = App.MainViewModel.NotenWichtungen.Count > 0 ? App.MainViewModel.NotenWichtungen[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.NotenWichtungen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentNotenWichtung))
        {
          this.CurrentNotenWichtung = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new NotenWichtung
    /// </summary>
    public DelegateCommand AddNotenWichtungCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current NotenWichtung
    /// </summary>
    public DelegateCommand DeleteNotenWichtungCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die bepunktungstyp currently selected in this workspace
    /// </summary>
    public NotenWichtungViewModel CurrentNotenWichtung
    {
      get
      {
        return this.currentNotenWichtung;
      }

      set
      {
        this.currentNotenWichtung = value;
        this.RaisePropertyChanged("CurrentNotenWichtung");
        this.DeleteNotenWichtungCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new NotenWichtung to the workspace and model
    /// </summary>
    private void AddNotenWichtung()
    {
      var bepunktungstyp = new NotenWichtung();
 
      // Check for existing tagesplan
      if (App.MainViewModel.NotenWichtungen.Any(o => o.NotenWichtungBezeichnung == bepunktungstyp.Bezeichnung))
      {
        Log.ProcessMessage(
          "NotenWichtung bereits vorhanden",
          "Dieses NotenWichtung ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      // // // App.UnitOfWork.GetRepository<NotenWichtung>().Add(bepunktungstyp);

      var vm = new NotenWichtungViewModel(bepunktungstyp);

      App.MainViewModel.NotenWichtungen.Add(vm);
      this.CurrentNotenWichtung = vm;
    }

    /// <summary>
    /// Handles deletion of the current NotenWichtung
    /// </summary>
    private void DeleteCurrentNotenWichtung()
    {
      // // App.UnitOfWork.GetRepository<NotenWichtung>().RemoveTest(this.CurrentNotenWichtung.Model);
      App.MainViewModel.NotenWichtungen.RemoveTest(this.CurrentNotenWichtung);
      this.CurrentNotenWichtung = null;
    }
  }
}
