namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for MediumWorkspaceView.xaml
  /// </summary>
  public partial class MediumWorkspaceView : Window
  {
    public MediumWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
