namespace SoftTeach.View.Personen
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for LerngruppeWorkspace.xaml
  /// </summary>
  public partial class LerngruppeWorkspace : Window
  {
    public LerngruppeWorkspace()
    {
      this.InitializeComponent();
      this.DataContext = App.MainViewModel.LerngruppeWorkspace;
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
