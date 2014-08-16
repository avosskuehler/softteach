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
  using Setting;
  using ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for AskForHalbjahresplanToAdaptDialog.xaml
  /// </summary>
  public partial class AskForSchülerlisteToAddDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AskForSchülerlisteToAddDialog"/> Klasse. 
    /// </summary>
    public AskForSchülerlisteToAddDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.Halbjahrtyp = Selection.Instance.Halbjahr;
      this.Jahrtyp = Selection.Instance.Jahrtyp;
      this.Klasse = Selection.Instance.Klasse;
      this.Fach = Selection.Instance.Fach;
      this.NotenWichtung = App.MainViewModel.NotenWichtungen.FirstOrDefault();
    }

    #endregion

    /// <summary>
    /// Holt oder setzt das Schuljahr für die Schülerliste
    /// </summary>
    public JahrtypViewModel Jahrtyp { get; set; }

    /// <summary>
    /// Holt oder setzt das Halbjahr für die Schülerliste
    /// </summary>
    public HalbjahrtypViewModel Halbjahrtyp { get; set; }

    /// <summary>
    /// Holt oder setzt die Klasse für die Schülerliste
    /// </summary>
    public KlasseViewModel Klasse { get; set; }

    /// <summary>
    /// Holt oder setzt das Fach für die Schülerliste
    /// </summary>
    public FachViewModel Fach { get; set; }

    /// <summary>
    /// Holt oder setzt die NotenWichtung für die Schülerliste
    /// </summary>
    public NotenWichtungViewModel NotenWichtung { get; set; }

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