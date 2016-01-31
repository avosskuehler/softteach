
namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  using SoftTeach.Setting;
  using SoftTeach.View.Sitzpläne;
  using SoftTeach.ViewModel.Sitzpläne;

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
