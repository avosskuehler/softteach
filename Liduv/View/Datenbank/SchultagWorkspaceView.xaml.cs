namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for SchultagWorkspaceView.xaml
  /// </summary>
  public partial class SchultagWorkspaceView : Window
  {
    public SchultagWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
