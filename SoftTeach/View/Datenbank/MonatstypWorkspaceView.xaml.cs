namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for MonatstypWorkspaceView.xaml
  /// </summary>
  public partial class MonatstypWorkspaceView : Window
  {
    public MonatstypWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
