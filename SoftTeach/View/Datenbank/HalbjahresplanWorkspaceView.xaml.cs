namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for HalbjahresplanWorkspaceView.xaml
  /// </summary>
  public partial class HalbjahresplanWorkspaceView : Window
  {
    public HalbjahresplanWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
