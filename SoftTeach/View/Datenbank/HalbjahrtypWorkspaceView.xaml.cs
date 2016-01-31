namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for HalbjahrtypWorkspaceView.xaml
  /// </summary>
  public partial class HalbjahrtypWorkspaceView : Window
  {
    public HalbjahrtypWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
