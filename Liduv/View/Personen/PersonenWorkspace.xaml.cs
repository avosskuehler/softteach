namespace Liduv.View.Personen
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for PersonenWorkspace.xaml
  /// </summary>
  public partial class PersonenWorkspace : Window
  {
    public PersonenWorkspace()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }
  }
}
