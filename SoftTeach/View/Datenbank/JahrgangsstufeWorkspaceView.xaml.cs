namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for JahrgangsstufeWorkspaceView.xaml
  /// </summary>
  public partial class JahrgangsstufeWorkspaceView : Window
  {
    public JahrgangsstufeWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
