namespace Liduv.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for ModulWorkspaceView.xaml
  /// </summary>
  public partial class ModulWorkspaceView : Window
  {
    public ModulWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
