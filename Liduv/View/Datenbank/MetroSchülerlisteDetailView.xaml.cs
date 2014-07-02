﻿
namespace Liduv.View.Datenbank
{
  using System.Windows;

  using Liduv.Setting;
  using Liduv.ViewModel.Personen;

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
      //Configuration.Instance.NavigationService.Navigate(new MetroSchülerlistePage());
    }
  }
}
