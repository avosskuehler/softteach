namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for TendenztypenDBView.xaml
  /// </summary>
  public partial class TendenztypenDBView : Window
  {
    public TendenztypenDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
