
namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Sitzpläne;

  /// <summary>
  /// Interaction logic for MetroRaumplanDetailView.xaml
  /// </summary>
  public partial class MetroRaumplanDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroRaumplanDetailView"/> Klasse.
    /// </summary>
    public MetroRaumplanDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Schuljahr button on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void RaumplanButtonOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.Raumplan = this.DataContext as RaumplanViewModel;
    }

    /// <summary>
    /// Handles the OnLoaded event of the MetroRaumplanDetailView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MetroRaumplanDetailView_OnLoaded(object sender, RoutedEventArgs e)
    {
      var model = this.DataContext as RaumplanViewModel;
      if (model == Selection.Instance.Raumplan)
      {
        this.RaumplanRadioButton.IsChecked = true;
      }
    }
  }
}
