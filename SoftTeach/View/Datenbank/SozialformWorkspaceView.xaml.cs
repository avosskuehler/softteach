namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for SozialformWorkspaceView.xaml
  /// </summary>
  public partial class SozialformWorkspaceView : Window
  {
    public SozialformWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
