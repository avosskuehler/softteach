
namespace Liduv.View.Datenbank
{
  using System.Windows;

  using Liduv.Setting;
  using Liduv.View.Noten;
  using Liduv.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for MetroHalbjahrDetailView.xaml
  /// </summary>
  public partial class MetroHalbjahrDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroHalbjahrDetailView"/> Klasse.
    /// </summary>
    public MetroHalbjahrDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Schuljahr button on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void HalbjahrButtonOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.Halbjahr = this.DataContext as HalbjahrtypViewModel;
    }
  }
}
