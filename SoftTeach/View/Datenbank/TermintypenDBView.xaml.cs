namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for TermintypenDBView.xaml
  /// </summary>
  public partial class TermintypenDBView : Window
  {
    public TermintypenDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
