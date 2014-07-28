﻿
namespace Liduv.View.Personen
{
  using System.Windows;

  using Liduv.Setting;
  using Liduv.View.Noten;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Noten;

  /// <summary>
  /// Interaction logic for MetroPersonDetailView.xaml
  /// </summary>
  public partial class MetroPersonDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroPersonDetailView"/> Klasse.
    /// </summary>
    public MetroPersonDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Schuljahr button on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void PersonButtonOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.Schülereintrag = this.DataContext as SchülereintragViewModel;
      Configuration.Instance.NavigationService.Navigate(new MetroSchülereintragNotenPage());
    }
  }
}