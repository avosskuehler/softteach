namespace Liduv.View.Noten
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for ArbeitWorkspace.xaml
  /// </summary>
  public partial class ArbeitWorkspace : Window
  {
    public ArbeitWorkspace()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}
