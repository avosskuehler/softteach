﻿
namespace Liduv.View.Datenbank
{
  using System.Windows;

  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.View.Noten;
  using Liduv.View.Personen;
  using Liduv.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for MetroFachDetailView.xaml
  /// </summary>
  public partial class MetroFachDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroFachDetailView"/> Klasse.
    /// </summary>
    public MetroFachDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Schuljahr button on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void FachButtonOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.Fach = this.DataContext as FachViewModel;
      App.MainViewModel.SchülerlisteWorkspace.JahrtypFilter = Selection.Instance.Schuljahr;
      App.MainViewModel.SchülerlisteWorkspace.HalbjahrtypFilter = Selection.Instance.Halbjahr;
      App.MainViewModel.SchülerlisteWorkspace.FachFilter = Selection.Instance.Fach;
      Configuration.Instance.NavigationService.Navigate(new MetroSchülerlistenPage());
    }
  }
}
