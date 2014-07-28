// <copyright file="Selection.cs" company="Paul Natorp Gymnasium, Berlin">        
// LEUDA - Lehrerunterrichtsdatenbank
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

namespace Liduv.Setting
{
  using System;
  using System.ComponentModel;
  using System.Linq;

  using Liduv.ViewModel.Termine;

  using Properties;
  using ViewModel.Datenbank;
  using ViewModel.Noten;
  using ViewModel.Personen;
  using ViewModel.Stundenentwürfe;

  /// <summary>
  /// The selection.
  /// </summary>
  public class Selection : INotifyPropertyChanged
  {
    private FachViewModel fach;
    private HalbjahrtypViewModel halbjahr;
    private SchülerlisteViewModel schülerliste;
    private JahrtypViewModel schuljahr;
    private KlasseViewModel klasse;
    private ModulViewModel modul;
    private StundeViewModel stunde;
    private StundenentwurfViewModel stundenentwurf;
    private SchülereintragViewModel schülereintrag;
    private ArbeitViewModel arbeit;
    private BewertungsschemaViewModel bewertungsschema;
    private DateTime hausaufgabeDatum;
    private string hausaufgabeBezeichnung;

    /// <summary>
    /// The instance.
    /// </summary>
    private static Selection instance;

    /// <summary>
    /// Verhindert, dass eine Standardinstanz der <see cref="Selection"/> Klasse erstellt wird. 
    /// </summary>
    private Selection()
    {
    }

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///   Holt den <see cref = "Selection" /> singleton.
    ///   If the underlying instance is null, a instance will be created.
    /// </summary>
    public static Selection Instance
    {
      get
      {
        // check again, if the underlying instance is null
        // return the existing/new instance
        return instance ?? (instance = new Selection());
      }
    }

    /// <summary>
    /// Holt oder setzt das Fach.
    /// </summary>
    public FachViewModel Fach
    {
      get
      {
        return this.fach;
      }

      set
      {
        this.fach = value;
        this.OnPropertyChanged("Fach");
      }
    }

    /// <summary>
    /// Holt oder setzt das Halbjahr.
    /// </summary>
    public HalbjahrtypViewModel Halbjahr
    {
      get
      {
        return this.halbjahr;
      }

      set
      {
        this.halbjahr = value;
        this.OnPropertyChanged("Halbjahr");
      }
    }

    /// <summary>
    /// Holt oder setzt die Klasse.
    /// </summary>
    public KlasseViewModel Klasse
    {
      get
      {
        return this.klasse;
      }

      set
      {
        this.klasse = value;
        this.OnPropertyChanged("Klasse");
      }
    }

    /// <summary>
    /// Holt oder setzt das Schuljahr.
    /// </summary>
    public JahrtypViewModel Schuljahr
    {
      get
      {
        return this.schuljahr;
      }

      set
      {
        this.schuljahr = value;
        this.OnPropertyChanged("Schuljahr");
      }
    }

    /// <summary>
    /// Holt oder setzt das Thema.
    /// </summary>
    public ModulViewModel Modul
    {
      get
      {
        return this.modul;
      }

      set
      {
        this.modul = value;
        this.OnPropertyChanged("Modul");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stunde
    /// </summary>
    public StundeViewModel Stunde
    {
      get
      {
        return this.stunde;
      }

      set
      {
        this.stunde = value;
        this.OnPropertyChanged("Stunde");
      }
    }

    /// <summary>
    /// Holt oder setzt den Stundenentwurf.
    /// </summary>
    public StundenentwurfViewModel Stundenentwurf
    {
      get
      {
        return this.stundenentwurf;
      }

      set
      {
        this.stundenentwurf = value;
        this.OnPropertyChanged("Stundenentwurf");
      }
    }

    /// <summary>
    /// Holt oder setzt die Schülerliste.
    /// </summary>
    public SchülerlisteViewModel Schülerliste
    {
      get
      {
        return this.schülerliste;
      }

      set
      {
        this.schülerliste = value;
        this.OnPropertyChanged("Schülerliste");
      }
    }

    /// <summary>
    /// Holt oder setzt den Schülereintrag.
    /// </summary>
    public SchülereintragViewModel Schülereintrag
    {
      get
      {
        return this.schülereintrag;
      }

      set
      {
        this.schülereintrag = value;
        this.OnPropertyChanged("Schülereintrag");
      }
    }

    /// <summary>
    /// Holt oder setzt die Arbeit.
    /// </summary>
    public ArbeitViewModel Arbeit
    {
      get
      {
        return this.arbeit;
      }

      set
      {
        this.arbeit = value;
        this.OnPropertyChanged("Arbeit");
      }
    }

    /// <summary>
    /// Holt oder setzt das Bewertungsschema.
    /// </summary>
    public BewertungsschemaViewModel Bewertungsschema
    {
      get
      {
        return this.bewertungsschema;
      }

      set
      {
        this.bewertungsschema = value;
        this.OnPropertyChanged("Bewertungsschema");
      }
    }

    /// <summary>
    /// Holt oder setzt das Datum der Hausaufgabe
    /// </summary>
    public DateTime HausaufgabeDatum
    {
      get
      {
        return this.hausaufgabeDatum;
      }

      set
      {
        this.hausaufgabeDatum = value;
        this.OnPropertyChanged("HausaufgabeDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung für die Hausaufgabe
    /// </summary>
    public string HausaufgabeBezeichnung
    {
      get
      {
        return this.hausaufgabeBezeichnung;
      }

      set
      {
        this.hausaufgabeBezeichnung = value;
        this.OnPropertyChanged("HausaufgabeBezeichnung");
      }
    }

    /// <summary>
    /// Holt das Datum der Hausaufgabe in der form LongDateString
    /// </summary>
    public string HausaufgabeDatumString
    {
      get
      {
        return this.hausaufgabeDatum.ToLongDateString();
      }
    }

    /// <summary>
    /// Sendet OnPropertyChanged Schuljahr
    /// </summary>
    public void ReNotifySchuljahr()
    {
      this.OnPropertyChanged("Schuljahr");
    }

    /// <summary>
    /// Populates the selection values from the application settings.
    /// </summary>
    public void PopulateFromSettings()
    {
      this.SetSelection(Settings.Default.Schuljahr, Settings.Default.Halbjahr, Settings.Default.Fach, Settings.Default.Klasse, Settings.Default.Modul);
    }

    /// <summary>
    /// The update user settings.
    /// </summary>
    public void UpdateUserSettings()
    {
      Settings.Default.Fach = this.Fach != null ? this.Fach.FachBezeichnung : string.Empty;
      Settings.Default.Klasse = this.Klasse != null ? this.Klasse.KlasseBezeichnung : string.Empty;
      Settings.Default.Schuljahr = this.Schuljahr != null ? this.Schuljahr.JahrtypBezeichnung : string.Empty;
      Settings.Default.Halbjahr = this.Halbjahr != null ? this.Halbjahr.HalbjahrtypBezeichnung : string.Empty;
      Settings.Default.Modul = this.Modul != null ? this.Modul.ModulBezeichnung : string.Empty;
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name.
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// The set selection.
    /// </summary>
    /// <param name="newSchuljahr"> The schuljahr. </param>
    /// <param name="newHalbjahr"> The halbjahr. </param>
    /// <param name="newFach">The fach. </param>
    /// <param name="newKlasse"> The klasse. </param>
    /// <param name="newModul"> The modul. </param>    
    private void SetSelection(
      string newSchuljahr,
      string newHalbjahr,
      string newFach,
      string newKlasse,
      string newModul)
    {
      this.Schuljahr = App.MainViewModel.Jahrtypen.FirstOrDefault(o => o.JahrtypBezeichnung == newSchuljahr);
      this.Halbjahr = App.MainViewModel.Halbjahrtypen.FirstOrDefault(o => o.HalbjahrtypBezeichnung == newHalbjahr);
      this.Fach = App.MainViewModel.Fächer.FirstOrDefault(o => o.FachBezeichnung == newFach);
      this.Klasse = App.MainViewModel.Klassen.FirstOrDefault(o => o.KlasseBezeichnung == newKlasse);
      this.Modul = App.MainViewModel.Module.FirstOrDefault(o => o.ModulBezeichnung == newModul);
    }

  }
}