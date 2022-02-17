namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for SchuljahrWorkspaceView.xaml
  /// </summary>
  public partial class SchuljahrWorkspaceView : Window
  {
    public SchuljahrWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
