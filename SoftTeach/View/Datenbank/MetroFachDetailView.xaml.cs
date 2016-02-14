﻿
namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  using SoftTeach.Model.EntityFramework;

  using SoftTeach.View.Noten;
  using SoftTeach.View.Personen;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Datenbank;

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
      App.MainViewModel.SchülerlisteWorkspace.JahrtypFilter = Selection.Instance.Jahrtyp;
      App.MainViewModel.SchülerlisteWorkspace.FachFilter = Selection.Instance.Fach;
    }

    /// <summary>
    /// Handles the OnLoaded event of the MetroFachDetailView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MetroFachDetailView_OnLoaded(object sender, RoutedEventArgs e)
    {
      var model = this.DataContext as FachViewModel;
      if (model == Selection.Instance.Fach)
      {
        this.FachRadioButton.IsChecked = true;
      }
    }
  }
}