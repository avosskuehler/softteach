// <copyright file="MetroHausaufgabeDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
// Liduv - Lehrerunterrichtsdatenbank
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

namespace Liduv.View.Noten
{
  using System.Windows;

  using Liduv.Setting;
  using Liduv.ViewModel.Noten;

  using MahApps.Metro.Controls.Dialogs;

  /// <summary>
  /// The add note dialog.
  /// </summary>
  public partial class MetroHausaufgabeDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroHausaufgabeDialog"/> Klasse.
    /// </summary>
    /// <param name="hausaufgabeViewModel"> Das ViewModel der Hausaufgabe.</param>
    public MetroHausaufgabeDialog(HausaufgabeViewModel hausaufgabeViewModel)
    {
      this.DataContext = hausaufgabeViewModel;
      this.InitializeComponent();

      if (hausaufgabeViewModel == null)
      {
        return;
      }

      if (hausaufgabeViewModel.HausaufgabeIstNachgereicht)
      {
        this.NachgereichtButton.IsChecked = true;
      }
      else
      {
        this.NichtgemachtButton.IsChecked = true;
      }
    }

    #endregion

    /// <summary>
    /// Handles the OnClick event of the Fertig button control.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private async void FertigButtonOnClick(object sender, RoutedEventArgs e)
    {
      var metroWindow = Configuration.Instance.MetroWindow;
      await metroWindow.HideMetroDialogAsync(this);
      Selection.Instance.Schülereintrag.UpdateNoten();
    }

    /// <summary>
    /// Handles the OnChecked event of the NichtgemachtButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void NichtgemachtButton_OnChecked(object sender, RoutedEventArgs e)
    {
      var hausaufgabeViewModel = this.DataContext as HausaufgabeViewModel;
      if (hausaufgabeViewModel != null)
      {
        hausaufgabeViewModel.HausaufgabeIstNachgereicht = false;
      }
    }

    /// <summary>
    /// Handles the OnChecked event of the NachgereichtButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void NachgereichtButton_OnChecked(object sender, RoutedEventArgs e)
    {
      var hausaufgabeViewModel = this.DataContext as HausaufgabeViewModel;
      if (hausaufgabeViewModel != null)
      {
        hausaufgabeViewModel.HausaufgabeIstNachgereicht = true;
      }
    }
  }
}