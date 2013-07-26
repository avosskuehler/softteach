namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for DateiverweisWorkspaceView.xaml
  /// </summary>
  public partial class DateiverweisWorkspaceView : Window
  {
    public DateiverweisWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
