
namespace Liduv.View.Datenbank
{
  using System.Windows;

  using Liduv.Setting;
  using Liduv.View.Sitzpläne;
  using Liduv.ViewModel.Sitzpläne;

  /// <summary>
  /// Interaction logic for MetroSitzplanDetailView.xaml
  /// </summary>
  public partial class MetroSitzplanDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroSitzplanDetailView"/> Klasse.
    /// </summary>
    public MetroSitzplanDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Sitzplan button on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void SitzplanButtonOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.Sitzplan = this.DataContext as SitzplanViewModel;
      Configuration.Instance.NavigationService.Navigate(new MetroSitzplanPage());
    }
  }
}
