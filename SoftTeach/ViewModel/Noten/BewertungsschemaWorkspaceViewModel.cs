namespace SoftTeach.ViewModel.Noten
{
  using Helper;

  using SoftTeach.UndoRedo;

  using SoftTeach.Model.EntityFramework;

  /// <summary>
  /// ViewModel for managing Bewertungsschema
  /// </summary>
  public class BewertungsschemaWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Bewertungsschema currently selected
    /// </summary>
    private BewertungsschemaViewModel currentBewertungsschema;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="BewertungsschemaWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public BewertungsschemaWorkspaceViewModel()
    {
      this.AddBewertungsschemaCommand = new DelegateCommand(this.AddBewertungsschema);
      this.DeleteBewertungsschemaCommand = new DelegateCommand(this.DeleteBewertungsschema, () => this.CurrentBewertungsschema != null);
      this.CurrentBewertungsschema = App.MainViewModel.Bewertungsschemata.Count > 0 ? App.MainViewModel.Bewertungsschemata[0] : null;
    }

    /// <summary>
    /// Holt den Befehl, um eine neue Bewertungsschema anzulegen.
    /// </summary>
    public DelegateCommand AddBewertungsschemaCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um eine bestehende Bewertungsschema zu löschen
    /// </summary>
    public DelegateCommand DeleteBewertungsschemaCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bewertungsschema currently selected in this workspace
    /// </summary>
    public BewertungsschemaViewModel CurrentBewertungsschema
    {
      get
      {
        return this.currentBewertungsschema;
      }

      set
      {
        this.currentBewertungsschema = value;
        this.RaisePropertyChanged("CurrentBewertungsschema");
        this.DeleteBewertungsschemaCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Stundenentwurf to the workspace and model
    /// </summary>
    private void AddBewertungsschema()
    {
      var bewertungsschema = new Bewertungsschema();
      bewertungsschema.Bezeichnung = "Neues Bewertungsschema";
      var vm = new BewertungsschemaViewModel(bewertungsschema);
      App.MainViewModel.Bewertungsschemata.Add(vm);
      this.CurrentBewertungsschema = vm;
    }

    /// <summary>
    /// Löscht das aktuelle Bewertungsschema.
    /// </summary>
    private void DeleteBewertungsschema()
    {
      this.DeleteBewertungsschema(this.CurrentBewertungsschema);
    }

    /// <summary>
    /// Löscht die gegebene Bewertungsschema.
    /// </summary>
    /// <param name="bewertungsschema">Das BewertungsschemaViewModel mit dem zu löschenden Bewertungsschema.</param>
    private void DeleteBewertungsschema(BewertungsschemaViewModel bewertungsschema)
    {
      App.MainViewModel.Bewertungsschemata.RemoveTest(bewertungsschema);
      this.CurrentBewertungsschema = null;
    }
  }
}
