
namespace Liduv.View.Termine
{
  /// <summary>
  /// Interaction logic for SchulterminDetailView.xaml
  /// </summary>
  public partial class SchulterminDetailView
  {
    public SchulterminDetailView()
    {
      this.InitializeComponent();
    }

    private void GanztagChecked(object sender, System.Windows.RoutedEventArgs e)
    {
      this.ErsteStunde.SelectedItem = App.MainViewModel.Unterrichtsstunden[0];
      this.LetzteStunde.SelectedItem =
        App.MainViewModel.Unterrichtsstunden[App.MainViewModel.Unterrichtsstunden.Count - 1];
    }
  }
}
