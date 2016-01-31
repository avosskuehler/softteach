namespace SoftTeach.ViewModel.Datenbank
{
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Noten;

  /// <summary>
  /// ViewModel for managing Jahrgangsstufe
  /// </summary>
  public class JahrgangsstufeWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Jahrgangsstufe currently selected
    /// </summary>
    private JahrgangsstufeViewModel currentJahrgangsstufe;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahrgangsstufeWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public JahrgangsstufeWorkspaceViewModel()
    {
      this.AddJahrgangsstufeCommand = new DelegateCommand(this.AddJahrgangsstufe);
      this.DeleteJahrgangsstufeCommand = new DelegateCommand(
        this.DeleteCurrentJahrgangsstufe, () => this.CurrentJahrgangsstufe != null);

      this.CurrentJahrgangsstufe = App.MainViewModel.Jahrgangsstufen.Count > 0
                                     ? App.MainViewModel.Jahrgangsstufen[0]
                                     : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Jahrgangsstufen.CollectionChanged += (sender, e) =>
        {
          if (e.OldItems != null && e.OldItems.Contains(this.CurrentJahrgangsstufe))
          {
            this.CurrentJahrgangsstufe = null;
          }
        };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahrgangsstufe
    /// </summary>
    public DelegateCommand AddJahrgangsstufeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Jahrgangsstufe
    /// </summary>
    public DelegateCommand DeleteJahrgangsstufeCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die jahrgangsstufe currently selected in this workspace
    /// </summary>
    public JahrgangsstufeViewModel CurrentJahrgangsstufe
    {
      get
      {
        return this.currentJahrgangsstufe;
      }

      set
      {
        this.currentJahrgangsstufe = value;
        this.RaisePropertyChanged("CurrentJahrgangsstufe");
        this.DeleteJahrgangsstufeCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Jahrgangsstufe to the workspace and model
    /// </summary>
    private void AddJahrgangsstufe()
    {
      var jahrgangsstufe = new Jahrgangsstufe();
      jahrgangsstufe.Bezeichnung = "Neue Jahrgangsstufe";
      jahrgangsstufe.Bepunktungstyp = Bepunktungstyp.Notenpunkte.ToString();
      var vm = new JahrgangsstufeViewModel(jahrgangsstufe);
      App.MainViewModel.Jahrgangsstufen.Add(vm);
      this.CurrentJahrgangsstufe = vm;
    }

    /// <summary>
    /// Handles deletion of the current Jahrgangsstufe
    /// </summary>
    private void DeleteCurrentJahrgangsstufe()
    {
      App.MainViewModel.Jahrgangsstufen.RemoveTest(this.CurrentJahrgangsstufe);
      this.CurrentJahrgangsstufe = null;
    }
  }
}
