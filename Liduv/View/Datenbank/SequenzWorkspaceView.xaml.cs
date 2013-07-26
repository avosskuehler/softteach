namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for SequenzWorkspaceView.xaml
  /// </summary>
  public partial class SequenzWorkspaceView : Window
  {
    public SequenzWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
