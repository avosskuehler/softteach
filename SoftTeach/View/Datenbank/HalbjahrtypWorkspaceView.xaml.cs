namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for HalbjahrWorkspaceView.xaml
  /// </summary>
  public partial class HalbjahrWorkspaceView : Window
  {
    public HalbjahrWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
