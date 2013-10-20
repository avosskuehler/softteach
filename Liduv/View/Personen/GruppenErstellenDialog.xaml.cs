// <copyright file="AskForSchülerlisteToAddDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace Liduv.View.Personen
{
  using System.Linq;
  using System.Windows;

  using Liduv.ViewModel.Personen;

  using Setting;
  using ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for GruppenErstellenDialog.xaml
  /// </summary>
  public partial class GruppenErstellenDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="GruppenErstellenDialog"/> Klasse. 
    /// </summary>
    public GruppenErstellenDialog(SchülerlisteViewModel schülerliste)
    {
      this.InitializeComponent();
      this.Schülerliste = schülerliste;
      this.DataContext = this.Schülerliste;
    }

    #endregion

    /// <summary>
    /// Holt oder setzt das Schuljahr für die Schülerliste
    /// </summary>
    public SchülerlisteViewModel Schülerliste { get; set; }

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