namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for UnterrichtsstundenDBView.xaml
  /// </summary>
  public partial class UnterrichtsstundenDBView : Window
  {
    public UnterrichtsstundenDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
