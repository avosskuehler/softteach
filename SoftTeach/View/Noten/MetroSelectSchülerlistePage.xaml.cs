namespace SoftTeach.View.Noten
{
  using SoftTeach.Setting;

  /// <summary>
  /// Interaction logic for MetroNotenLandingPage.xaml
  /// </summary>
  public partial class MetroSelectLerngruppePage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MetroSelectLerngruppePage"/> class.
    /// </summary>
    public MetroSelectLerngruppePage()
    {
      this.DataContext = App.MainViewModel.LerngruppeWorkspace;
      App.MainViewModel.LerngruppeWorkspace.SchuljahrFilter = Selection.Instance.Schuljahr;
      App.MainViewModel.LerngruppeWorkspace.FachFilter = Selection.Instance.Fach;
      this.InitializeComponent();
    }
  }
}
