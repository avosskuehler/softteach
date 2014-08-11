namespace Liduv.View.Sitzpläne
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for RäumeDBView.xaml
  /// </summary>
  public partial class RäumeDBView : Window
  {
    public RäumeDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
