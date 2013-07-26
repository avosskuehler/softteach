namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for StundenplanWorkspaceView.xaml
  /// </summary>
  public partial class StundenplanWorkspaceView : Window
  {
    public StundenplanWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
