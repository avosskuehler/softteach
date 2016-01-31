
namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Sitzpläne;

  /// <summary>
  /// Interaction logic for MetroRaumDetailView.xaml
  /// </summary>
  public partial class MetroRaumDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroRaumDetailView"/> Klasse.
    /// </summary>
    public MetroRaumDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Schuljahr button on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void RaumButtonOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.Raum = this.DataContext as RaumViewModel;
    }

    /// <summary>
    /// Handles the OnLoaded event of the MetroRaumDetailView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MetroRaumDetailView_OnLoaded(object sender, RoutedEventArgs e)
    {
      var model = this.DataContext as RaumViewModel;
      if (model == Selection.Instance.Raum)
      {
        this.RaumRadioButton.IsChecked = true;
      }
    }
  }
}
