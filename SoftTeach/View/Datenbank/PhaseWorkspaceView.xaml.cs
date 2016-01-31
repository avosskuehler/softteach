namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for PhaseWorkspaceView.xaml
  /// </summary>
  public partial class PhaseWorkspaceView : Window
  {
    public PhaseWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
