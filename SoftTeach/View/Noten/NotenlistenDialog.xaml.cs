// <copyright file="NotenlistenDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
// SoftTeach - Lehrerunterrichtsdatenbank
// Copyright (C) 2013 Dr. Adrian Voßkühler
// -----------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published  
// by the Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version. This program is distributed in the 
// hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// ***********************************************************************
// </copyright>
// <author>Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>

namespace SoftTeach.View.Noten
{
  using System.Windows;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.ViewModel.Noten;

  /// <summary>
  /// Ein Dialog um Noten für das Zeugnis oder einen Zwischenstand zu machen
  /// </summary>
  public partial class NotenlistenDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NotenlistenDialog"/> Klasse.
    /// </summary>
    public NotenlistenDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    /// <summary>
    /// Der Dialog wurde abgebrochen.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    /// <summary>
    /// Zwischenstand ausgewählt. UI anpassen.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void ZwischenstandClick(object sender, RoutedEventArgs e)
    {
      //this.ZwischenstandRadio.IsChecked = true;
      this.Button.Visibility = Visibility.Visible;
      this.ÄndernButton.Visibility = Visibility.Collapsed;
      this.VorhandeneNotenlistenControl.Visibility = Visibility.Collapsed;
      this.DatePicker.IsEnabled = true;
      var workspace = this.DataContext as NotenlistenWorkspaceViewModel;
      if (workspace != null && workspace.CurrentLerngruppe == null)
      {
        workspace.ResetLerngruppe();
      }
    }

    /// <summary>
    /// Halbjahresnote ausgewählt. UI anpassen.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void HalbjahrClick(object sender, RoutedEventArgs e)
    {
      //this.HalbjahrRadio.IsChecked = true;
      this.Button.Visibility = Visibility.Visible;
      this.ÄndernButton.Visibility = Visibility.Collapsed;
      this.VorhandeneNotenlistenControl.Visibility = Visibility.Collapsed;
      this.DatePicker.IsEnabled = true;
    }

    /// <summary>
    /// Ganzjahresnote ausgewählt. UI anpassen.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void GanzjahrClick(object sender, RoutedEventArgs e)
    {
      //this.GanzjahrRadio.IsChecked = true;
      this.Button.Visibility = Visibility.Visible;
      this.ÄndernButton.Visibility = Visibility.Collapsed;
      this.VorhandeneNotenlistenControl.Visibility = Visibility.Collapsed;
      this.DatePicker.IsEnabled = true;
    }

    /// <summary>
    /// Ändern ausgewählt. UI anpassen.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void ModifyClick(object sender, RoutedEventArgs e)
    {
      //this.ZwischenstandRadio.IsChecked = true;
      this.Button.Visibility = Visibility.Collapsed;
      this.ÄndernButton.Visibility = Visibility.Visible;
      this.VorhandeneNotenlistenControl.Visibility = Visibility.Visible;
      this.DatePicker.IsEnabled = false;
      var workspace = this.DataContext as NotenlistenWorkspaceViewModel;
      if (workspace != null && workspace.NotenlistenEinträge.Count == 0)
      {
        workspace.CurrentLerngruppe = null;
      }
    }

    private void Click(object sender, RoutedEventArgs e)
    {
      if (this.DatePicker.SelectedDate == null)
      {
        var dlg = new InformationDialog("Datum angeben", "Bitte ein Datum angeben.", false);
        dlg.ShowDialog();
        return;
      }

      var workspace = this.DataContext as NotenlistenWorkspaceViewModel;
      if (workspace != null && workspace.CheckIfZeugnisExists())
      {
        var dlg = new InformationDialog("Existiert bereits", "Zu diesem Datum existiert schon eine Notenliste.", false);
        dlg.ShowDialog();
        return;
      }

      workspace.ZeugnisnotenAnlegen();

      this.DialogResult = true;
      this.Close();
    }

    private void ÄndernClick(object sender, RoutedEventArgs e)
    {
      var workspace = this.DataContext as NotenlistenWorkspaceViewModel;
      workspace.ZeugnisnotenAnlegen();
      this.DialogResult = true;
      //this.Close();
    }
  }
}