// <copyright file="AskForLerngruppeToAddDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Personen
{
  using System.Linq;
  using System.Windows;
  using Setting;
  using SoftTeach.Model.TeachyModel;
  using ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for AskForHalbjahresplanToAdaptDialog.xaml
  /// </summary>
  public partial class AskForLerngruppeToAddDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AskForLerngruppeToAddDialog"/> Klasse. 
    /// </summary>
    public AskForLerngruppeToAddDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.Halbjahr = Selection.Instance.Halbjahr;
      this.Schuljahr = Selection.Instance.Schuljahr;
      this.Jahrgang = Selection.Instance.Jahrgang;
      this.Fach = Selection.Instance.Fach;
      this.NotenWichtung = App.MainViewModel.NotenWichtungen.FirstOrDefault();
    }

    #endregion

    /// <summary>
    /// Holt oder setzt das Schuljahr für die Lerngruppe
    /// </summary>
    public SchuljahrViewModel Schuljahr { get; set; }

    /// <summary>
    /// Holt oder setzt das Halbjahr für die Lerngruppe
    /// </summary>
    public Halbjahr Halbjahr { get; set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung für die Lerngruppe
    /// </summary>
    public string Bezeichnung { get; set; }

    /// <summary>
    /// Holt oder setzt die Klasse für die Lerngruppe
    /// </summary>
    public int Jahrgang { get; set; }

    /// <summary>
    /// Holt oder setzt das Fach für die Lerngruppe
    /// </summary>
    public FachViewModel Fach { get; set; }

    /// <summary>
    /// Holt oder setzt die NotenWichtung für die Lerngruppe
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