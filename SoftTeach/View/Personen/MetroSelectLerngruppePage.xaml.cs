namespace SoftTeach.View.Noten
{
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Personen;
  using System.Windows.Controls;

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

    private void LerngruppenListBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var textbox = sender as TextBlock;
      var lerngruppe = textbox.DataContext as LerngruppeViewModel;
      if (lerngruppe != null)
      {
        App.MainViewModel.LerngruppeWorkspace.CurrentLerngruppe = lerngruppe;
      }
    }
  }
}
