// <copyright file="MetroHausaufgabeDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System;
  using System.Windows;

  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Setting;

  /// <summary>
  /// The add note dialog.
  /// </summary>
  public partial class MetroAddHausaufgabeDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="MetroAddHausaufgabeDialog" /> Klasse.
    /// </summary>
    /// <param name="datum">The datum.</param>
    public MetroAddHausaufgabeDialog(DateTime datum)
    {
      this.DataContext = this;
      this.Datum = datum;
      this.InitializeComponent();
    }

    #endregion

    /// <summary>
    /// Holt oder setzt das Datum für die Hausaufgabe
    /// </summary>
    public DateTime Datum { get; set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung der Hausaufgabe
    /// </summary>
    public string Bezeichnung { get; set; }

    /// <summary>
    /// Handles the OnClick event of the Fertig button control.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private async void FertigButtonOnClick(object sender, RoutedEventArgs e)
    {
      var metroWindow = Configuration.Instance.MetroWindow;
      await metroWindow.HideMetroDialogAsync(this);
      Selection.Instance.HausaufgabeDatum = this.Datum;
      Selection.Instance.HausaufgabeBezeichnung = this.Bezeichnung;

      // Reset currently selected hausaufgaben
      foreach (var schülereintragViewModel in Selection.Instance.Lerngruppe.Schülereinträge)
      {
        schülereintragViewModel.CurrentHausaufgabe = null;
      }

      var dlg = new MetroAddHausaufgabenDialog();
      await metroWindow.ShowMetroDialogAsync(dlg);
    }
  }
}