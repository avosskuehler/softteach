namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for DateitypWorkspaceView.xaml
  /// </summary>
  public partial class DateitypWorkspaceView : Window
  {
    public DateitypWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
