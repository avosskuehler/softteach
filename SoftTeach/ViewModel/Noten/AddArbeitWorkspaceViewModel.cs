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

namespace SoftTeach.ViewModel.Noten
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
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Noten;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// Ein Dialog um nicht gemachte Arbeiten einzutragen.
  /// </summary>
  public class AddArbeitWorkspaceViewModel
  {
    private SchuljahrViewModel schuljahr;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AddArbeitDialog"/> Klasse. 
    /// </summary>
    public AddArbeitWorkspaceViewModel()
    {
      this.LerngruppenViewSource = new CollectionViewSource() { Source = App.MainViewModel.Lerngruppen };
      using (this.LerngruppenViewSource.DeferRefresh())
      {
        this.LerngruppenViewSource.Filter += this.LerngruppenViewSource_Filter;
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("LerngruppeSchuljahr.SchuljahrJahr", ListSortDirection.Ascending));
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("LerngruppeFach.FachBezeichnung", ListSortDirection.Ascending));
      }

      this.Schuljahr = Selection.Instance.Schuljahr;
      this.Bepunktungstyp = Bepunktungstyp.NoteMitTendenz;
      this.Bewertungsschema = App.MainViewModel.Bewertungsschemata[0];
      this.Bezeichnung = "neue Arbeit";
      this.Datum = DateTime.Now;
      this.IstKlausur = true;
    }

    /// <summary>
    /// Holt oder setzt die Lerngruppe für die Arbeit
    /// </summary>
    public LerngruppeViewModel Lerngruppe { get; set; }

    /// <summary>
    /// Holt oder setzt die LerngruppenViewSource
    /// </summary>
    public CollectionViewSource LerngruppenViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Lerngruppen
    /// </summary>
    public ICollectionView LerngruppenView => this.LerngruppenViewSource.View;

    /// <summary>
    /// Holt oder setzt das Schuljahr für die Arbeit
    /// </summary>
    public SchuljahrViewModel Schuljahr
    {
      get => this.schuljahr;
      set
      {
        UiServices.SetBusyState();
        this.schuljahr = value;
        Selection.Instance.Schuljahr = value;
        this.LerngruppenView.Refresh();
      }
    }

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

    /// <summary>
    /// Filtert die Lerngruppen nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Die Lerngruppe, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private void LerngruppenViewSource_Filter(object sender, FilterEventArgs e)
    {
      var lerngruppeViewModel = e.Item as LerngruppeViewModel;
      if (lerngruppeViewModel == null)
      {
        e.Accepted = false;
        return;
      }

      if (this.Schuljahr != null)
      {
        if (lerngruppeViewModel.LerngruppeSchuljahr.SchuljahrJahr != this.Schuljahr.SchuljahrJahr) e.Accepted = false;
        return;
      }

      e.Accepted = true;
      return;
    }

  }
}