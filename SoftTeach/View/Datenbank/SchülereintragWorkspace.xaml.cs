namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for SchülereintragWorkspace.xaml
  /// </summary>
  public partial class SchülereintragWorkspace : Window
  {
    public SchülereintragWorkspace()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
