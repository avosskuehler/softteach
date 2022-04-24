namespace SoftTeach.View.Noten
{
  using System.Text.RegularExpressions;
  using System.Windows;

  /// <summary>
  /// Interaction logic for ArbeitWorkspace.xaml
  /// </summary>
  public partial class ArbeitWorkspace : Window
  {
    public ArbeitWorkspace()
    {
      this.InitializeComponent();
      App.MainViewModel.LoadArbeiten();
      this.DataContext = App.MainViewModel.ArbeitWorkspace;
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

    private void TextboxInputValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      Regex regex = new Regex("[^0-9-/]+");
      e.Handled = regex.IsMatch(e.Text);
    }
  }
}
