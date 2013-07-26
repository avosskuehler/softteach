namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for ZensurenDBView.xaml
  /// </summary>
  public partial class ZensurenDBView : Window
  {
    public ZensurenDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
