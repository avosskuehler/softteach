namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for StundenplaneintragWorkspaceView.xaml
  /// </summary>
  public partial class StundenplaneintragWorkspaceView : Window
  {
    public StundenplaneintragWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
