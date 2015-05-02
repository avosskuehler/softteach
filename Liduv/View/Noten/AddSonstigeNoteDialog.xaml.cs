// <copyright file="AddSonstigeNoteDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System;
  using System.Windows;

  using Liduv.Model;
  using Liduv.ViewModel.Noten;

  /// <summary>
  /// Ein Dialog um sonstige Noten einzutragen.
  /// </summary>
  public partial class AddSonstigeNoteDialog
  {

    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AddSonstigeNoteDialog"/> Klasse.
    /// </summary>
    public AddSonstigeNoteDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.IsMündlich = true;
    }

    #endregion

    /// <summary>
    /// Holt oder setzt das Datum für die sonstige Note
    /// </summary>
    public DateTime Datum { get; set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung der sonstigen Note
    /// </summary>
    public string Bezeichnung { get; set; }

    /// <summary>
    /// Holt oder setzt die Wichtung der sonstigen Note
    /// </summary>
    public int Wichtung { get; set; }

    /// <summary>
    /// Holt oder setzt einen Wert der angibt, ob die Note Schriftlich oder Mündlich ist.
    /// </summary>
    public bool IsMündlich { get; set; }

    /// <summary>
    /// Holt oder setzt den Notentyp der sonstigen Note
    /// </summary>
    public Notentyp Notentyp
    {
      get
      {
        return this.IsMündlich ? Notentyp.MündlichSonstige : Notentyp.SchriftlichSonstige;
      }
    }


    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}