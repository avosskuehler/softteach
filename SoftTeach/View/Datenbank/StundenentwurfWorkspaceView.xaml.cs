namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for StundenentwurfWorkspaceView.xaml
  /// </summary>
  public partial class StundenentwurfWorkspaceView : Window
  {
    public StundenentwurfWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
