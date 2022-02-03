namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for StundenentwurfWorkspaceDBView.xaml
  /// </summary>
  public partial class StundenentwurfWorkspaceDBView : Window
  {
    public StundenentwurfWorkspaceDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
