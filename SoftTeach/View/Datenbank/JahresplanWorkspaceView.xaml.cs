namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for JahresplanWorkspaceView.xaml
  /// </summary>
  public partial class JahresplanWorkspaceView : Window
  {
    public JahresplanWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
