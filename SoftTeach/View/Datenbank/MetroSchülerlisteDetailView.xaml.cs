
namespace SoftTeach.View.Datenbank
{
  using System;
  using System.Windows;

  using SoftTeach.Setting;
  using SoftTeach.View.Personen;
  using SoftTeach.View.Sitzpläne;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// Interaction logic for MetroSchülerlisteDetailView.xaml
  /// </summary>
  public partial class MetroSchülerlisteDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroSchülerlisteDetailView"/> Klasse.
    /// </summary>
    public MetroSchülerlisteDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Schülerliste button on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void SchülerlisteButtonOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.Schülerliste = this.DataContext as SchülerlisteViewModel;
      if (Selection.Instance.Schülerliste == null)
      {
        return;
      }

      switch (Configuration.Instance.NavigateTarget)
      {
        case NavigateTarget.Gruppen:
          Selection.Instance.Schülerliste.ResetKrankenstand();
          Configuration.Instance.NavigationService.Navigate(new MetroGruppenPage());
          break;
        case NavigateTarget.Noten:
          Selection.Instance.Schülerliste.NotenDatum = DateTime.Today;
          Configuration.Instance.NavigationService.Navigate(new MetroSchülerlistePage());
          break;
        case NavigateTarget.Sitzpläne:
          Configuration.Instance.NavigationService.Navigate(new MetroRäumePage());
          break;
      }
    }
  }
}
