
namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Datenbank;

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

    /// <summary>
    /// Handles the OnLoaded event of the MetroHalbjahrDetailView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MetroHalbjahrDetailView_OnLoaded(object sender, RoutedEventArgs e)
    {
      var model = this.DataContext as HalbjahrtypViewModel;
      if (model == Selection.Instance.Halbjahr)
      {
        this.HalbjahrRadioButton.IsChecked = true;
      }
    }
  }
}
