namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for NotenWichtungenDBView.xaml
  /// </summary>
  public partial class NotenWichtungenDBView : Window
  {
    public NotenWichtungenDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
