namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for TagesplanWorkspaceView.xaml
  /// </summary>
  public partial class TagesplanWorkspaceView : Window
  {
    public TagesplanWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
