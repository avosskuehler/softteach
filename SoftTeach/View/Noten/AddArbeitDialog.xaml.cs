// <copyright file="AddArbeitDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Data;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Noten;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// Ein Dialog um nicht gemachte Arbeiten einzutragen.
  /// </summary>
  public partial class AddArbeitDialog
  {
    //private SchuljahrViewModel schuljahr;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AddArbeitDialog"/> Klasse. 
    /// </summary>
    public AddArbeitDialog()
    {
      this.InitializeComponent();
    }

    /// <summary> The ok click. </summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="e"> The e. </param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      var workspace = this.DataContext as AddArbeitWorkspaceViewModel;

      // Doppelte Arbeiten zum selben Termin vermeiden.
      if (App.MainViewModel.Arbeiten.Any(
          o =>
          o.ArbeitLerngruppe.LerngruppeSchuljahr.SchuljahrBezeichnung == workspace.Lerngruppe.LerngruppeSchuljahr.SchuljahrBezeichnung
          && o.ArbeitLerngruppe.LerngruppeBezeichnung == workspace.Lerngruppe.LerngruppeBezeichnung
          && o.ArbeitFach.FachBezeichnung == workspace.Lerngruppe.LerngruppeFach.FachBezeichnung && o.ArbeitDatum.Date == workspace.Datum.Date))
      {
        var message = "Eine Arbeit der Lerngruppe " + workspace.Lerngruppe.LerngruppeKurzbezeichnung + " ist am " + workspace.Datum.Date.ToShortDateString() + " bereits in der Datenbank angelegt.";

        var dlg = new InformationDialog("Arbeit schon angelegt.", message, false);
        dlg.ShowDialog();
        return;
      }

      this.DialogResult = true;
      this.Close();
    }

    /// <summary> The cancel click. </summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="e"> The e. </param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}