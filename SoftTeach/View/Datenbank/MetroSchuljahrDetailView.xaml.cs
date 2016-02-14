﻿
namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  using SoftTeach.View.Noten;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for MetroSchuljahrDetailView.xaml
  /// </summary>
  public partial class MetroSchuljahrDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroSchuljahrDetailView"/> Klasse.
    /// </summary>
    public MetroSchuljahrDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Schuljahr button on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void SchuljahrButtonOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.Jahrtyp = this.DataContext as JahrtypViewModel;
    }

    /// <summary>
    /// Handles the OnLoaded event of the MetroSchuljahrDetailView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MetroSchuljahrDetailView_OnLoaded(object sender, RoutedEventArgs e)
    {
      var model = this.DataContext as JahrtypViewModel;
      if (model == Selection.Instance.Jahrtyp)
      {
        this.SchuljahrRadioButton.IsChecked = true;
      }
    }
  }
}