namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for BetroffeneKlassenDBView.xaml
  /// </summary>
  public partial class BetroffeneKlassenDBView : Window
  {
    public BetroffeneKlassenDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
