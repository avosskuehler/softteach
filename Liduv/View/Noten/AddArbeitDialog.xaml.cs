// <copyright file="AddArbeitDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System.Linq;
  using System.Windows;

  using Liduv.ExceptionHandling;
  using Liduv.Setting;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Noten;

  /// <summary>
  /// Ein Dialog um nicht gemachte Arbeiten einzutragen.
  /// </summary>
  public partial class AddArbeitDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AddArbeitDialog"/> Klasse. 
    /// </summary>
    public AddArbeitDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.Klasse = Selection.Instance.Klasse;
      this.Jahrtyp = Selection.Instance.Jahrtyp;
      this.Halbjahrtyp = Selection.Instance.Halbjahr;
      this.Fach = Selection.Instance.Fach;
      this.Bepunktungstyp = Bepunktungstyp.NoteMitTendenz;
      this.Bewertungsschema = App.MainViewModel.Bewertungsschemata[0];
      this.Bezeichnung = "Neue Arbeit";
      this.Datum = DateTime.Now;
      this.IstKlausur = true;
    }

    #endregion
    /// <summary>
    /// Holt oder setzt die Klasse für die Arbeit
    /// </summary>
    public KlasseViewModel Klasse { get; set; }

    /// <summary>
    /// Holt oder setzt den Jahrtyp für die Arbeit
    /// </summary>
    public JahrtypViewModel Jahrtyp { get; set; }

    /// <summary>
    /// Holt oder setzt den Halbjahrtyp für die Arbeit
    /// </summary>
    public HalbjahrtypViewModel Halbjahrtyp { get; set; }

    /// <summary>
    /// Holt oder setzt das Fach für die Arbeit
    /// </summary>
    public FachViewModel Fach { get; set; }

    /// <summary>
    /// Holt oder setzt den Bepunktungstyp für die Arbeit
    /// </summary>
    public Bepunktungstyp Bepunktungstyp { get; set; }

    /// <summary>
    /// Holt oder setzt das Bewertungsschema für die Arbeit
    /// </summary>
    public BewertungsschemaViewModel Bewertungsschema { get; set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung der Arbeit
    /// </summary>
    public string Bezeichnung { get; set; }

    /// <summary>
    /// Holt oder setzt das Datum für die Arbeit
    /// </summary>
    public DateTime Datum { get; set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob die Arbeit eine Klausur ist.
    /// </summary>
    public bool IstKlausur { get; set; }

    /// <summary> The ok click. </summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="e"> The e. </param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      // Doppelte Arbeiten zum selben Termin vermeiden.
      if (App.MainViewModel.Arbeiten.Any(
          o =>
          o.ArbeitJahrtyp.JahrtypBezeichnung == this.Jahrtyp.JahrtypBezeichnung
          && o.ArbeitHalbjahrtyp.HalbjahrtypIndex == this.Halbjahrtyp.HalbjahrtypIndex
          && o.ArbeitKlasse.KlasseBezeichnung == this.Klasse.KlasseBezeichnung
          && o.ArbeitFach.FachBezeichnung == this.Fach.FachBezeichnung && o.ArbeitDatum.Date == this.Datum.Date))
      {
        var message = "Eine Arbeit der Klasse " + this.Klasse.KlasseBezeichnung + " im Fach "
                      + this.Fach.FachBezeichnung + " ist am " + this.Datum.Date.ToShortDateString() + " bereits in der Datenbank angelegt.";

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