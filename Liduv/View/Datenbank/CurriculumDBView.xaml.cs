namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for CurriculumDBView.xaml
  /// </summary>
  public partial class CurriculumDBView : Window
  {
    public CurriculumDBView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
