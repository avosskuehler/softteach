namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for SchülerlisteWorkspace.xaml
  /// </summary>
  public partial class SchülerlisteWorkspace : Window
  {
    public SchülerlisteWorkspace()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
