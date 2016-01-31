namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for SchulwocheWorkspaceView.xaml
  /// </summary>
  public partial class SchulwocheWorkspaceView : Window
  {
    public SchulwocheWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
