namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for FachstundenanzahlWorkspaceView.xaml
  /// </summary>
  public partial class FachstundenanzahlWorkspaceView : Window
  {
    public FachstundenanzahlWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
