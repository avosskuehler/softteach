namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for JahrtypWorkspaceView.xaml
  /// </summary>
  public partial class JahrtypWorkspaceView : Window
  {
    public JahrtypWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
