namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for ReiheWorkspaceView.xaml
  /// </summary>
  public partial class ReiheWorkspaceView : Window
  {
    public ReiheWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
