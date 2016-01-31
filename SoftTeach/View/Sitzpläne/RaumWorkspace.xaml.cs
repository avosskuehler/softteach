namespace SoftTeach.View.Sitzpläne
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for RaumWorkspace.xaml
  /// </summary>
  public partial class RaumWorkspace : Window
  {
    public RaumWorkspace()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
