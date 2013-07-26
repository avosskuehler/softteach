namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for MonatsplanWorkspaceView.xaml
  /// </summary>
  public partial class MonatsplanWorkspaceView : Window
  {
    public MonatsplanWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
