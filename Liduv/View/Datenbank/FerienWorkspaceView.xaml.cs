namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for FerienWorkspaceView.xaml
  /// </summary>
  public partial class FerienWorkspaceView : Window
  {
    public FerienWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
