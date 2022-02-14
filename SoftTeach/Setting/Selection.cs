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

namespace SoftTeach.Setting
{
  using System;
  using System.ComponentModel;
  using System.Linq;

  using Properties;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Sitzpläne;

  using ViewModel.Datenbank;
  using ViewModel.Noten;
  using ViewModel.Personen;
  using ViewModel.Stundenentwürfe;

  /// <summary>
  /// The selection.
  /// </summary>
  public class Selection : INotifyPropertyChanged
  {
    private SitzplanViewModel sitzplan;
    private RaumViewModel raum;
    private RaumplanViewModel raumplan;
    private FachViewModel fach;
    private Halbjahr halbjahr;
    private int jahrgang;
    private LerngruppeViewModel lerngruppe;
    private SchuljahrViewModel schuljahr;
    private ModulViewModel modul;
    private StundeNeu stunde;
    private StundenentwurfViewModel stundenentwurf;
    private SchülereintragViewModel schülereintrag;
    private ArbeitViewModel arbeit;
    private BewertungsschemaViewModel bewertungsschema;
    private DateTime hausaufgabeDatum;
    private string hausaufgabeBezeichnung;
    private DateTime sonstigeNoteDatum;
    private string sonstigeNoteBezeichnung;
    private Notentyp sonstigeNoteNotentyp;
    private int sonstigeNoteWichtung;

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
    public Halbjahr Halbjahr
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
    /// Holt oder setzt das Schuljahr.
    /// </summary>
    public SchuljahrViewModel Schuljahr
    {
      get
      {
        if (this.schuljahr == null)
        {
          this.schuljahr = App.MainViewModel.Schuljahre.FirstOrDefault(o => o.SchuljahrBezeichnung == "2021/2022");
        }

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
    public StundeNeu Stunde
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
    /// Holt oder setzt den Jahrgang
    /// </summary>
    public int Jahrgang
    {
      get
      {
        return this.jahrgang;
      }

      set
      {
        this.jahrgang = value;
        this.OnPropertyChanged("Jahrgang");
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
    /// Holt oder setzt die Lerngruppe.
    /// </summary>
    public LerngruppeViewModel Lerngruppe
    {
      get
      {
        return this.lerngruppe;
      }

      set
      {
        this.lerngruppe = value;
        this.OnPropertyChanged("Lerngruppe");
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
    /// Holt oder setzt den Raum.
    /// </summary>
    public RaumViewModel Raum
    {
      get
      {
        return this.raum;
      }

      set
      {
        this.raum = value;
        this.OnPropertyChanged("Raum");
      }
    }

    /// <summary>
    /// Holt oder setzt den Raumplan.
    /// </summary>
    public RaumplanViewModel Raumplan
    {
      get
      {
        return this.raumplan;
      }

      set
      {
        this.raumplan = value;
        this.OnPropertyChanged("Raumplan");
      }
    }

    /// <summary>
    /// Holt oder setzt den Sitzplan.
    /// </summary>
    public SitzplanViewModel Sitzplan
    {
      get
      {
        return this.sitzplan;
      }

      set
      {
        this.sitzplan = value;
        this.OnPropertyChanged("Sitzplan");
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
    /// Holt oder setzt das Datum der sonstigen Note
    /// </summary>
    public DateTime SonstigeNoteDatum
    {
      get
      {
        return this.sonstigeNoteDatum;
      }

      set
      {
        this.sonstigeNoteDatum = value;
        this.OnPropertyChanged("SonstigeNoteDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung für die SonstigeNote
    /// </summary>
    public string SonstigeNoteBezeichnung
    {
      get
      {
        return this.sonstigeNoteBezeichnung;
      }

      set
      {
        this.sonstigeNoteBezeichnung = value;
        this.OnPropertyChanged("SonstigeNoteBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt den Typ für die SonstigeNote
    /// </summary>
    public Notentyp SonstigeNoteNotentyp
    {
      get
      {
        return this.sonstigeNoteNotentyp;
      }

      set
      {
        this.sonstigeNoteNotentyp = value;
        this.OnPropertyChanged("SonstigeNoteNotentyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die Wichtung für die SonstigeNote
    /// </summary>
    public int SonstigeNoteWichtung
    {
      get
      {
        return this.sonstigeNoteWichtung;
      }

      set
      {
        this.sonstigeNoteWichtung = value;
        this.OnPropertyChanged("SonstigeNoteWichtung");
      }
    }


    /// <summary>
    /// Holt das Datum der SonstigeNote in der form LongDateString
    /// </summary>
    public string SonstigeNoteDatumString
    {
      get
      {
        return this.sonstigeNoteDatum.ToLongDateString();
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
      this.SetSelectionSilent(Settings.Default.Schuljahr, Settings.Default.Halbjahr, Settings.Default.Fach, Settings.Default.Lerngruppe, Settings.Default.Modul);
      if (this.Schuljahr == null)
      {
        if (App.MainViewModel.Schuljahre.Count == 0)
        {

        }
        this.Schuljahr = App.MainViewModel.Schuljahre.Last();
      }
      else
      {
        this.OnPropertyChanged("Schuljahr");
      }

      if (this.Halbjahr == null)
      {
        if (DateTime.Now.Month > 7 || DateTime.Now.Month == 1)
        {
          this.Halbjahr = App.MainViewModel.Halbjahre[0];
        }
        else
        {
          this.Halbjahr = App.MainViewModel.Halbjahre[1];
        }
      }

      if (this.Fach == null)
      {
        this.Fach = App.MainViewModel.Fächer.First();
      }

      if (this.Lerngruppe == null)
      {
        this.Lerngruppe = App.MainViewModel.Lerngruppen.First();
      }
    }

    /// <summary>
    /// The update user settings.
    /// </summary>
    public void UpdateUserSettings()
    {
      Settings.Default.Fach = this.Fach != null ? this.Fach.FachBezeichnung : string.Empty;
      Settings.Default.Lerngruppe = this.Lerngruppe != null ? this.Lerngruppe.LerngruppeBezeichnung : string.Empty;
      Settings.Default.Schuljahr = this.Schuljahr != null ? this.Schuljahr.SchuljahrBezeichnung : string.Empty;
      Settings.Default.Halbjahr = this.Halbjahr != null ? this.Halbjahr.ToString() : string.Empty;
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
    /// <param name="newLerngruppe"> The klasse. </param>
    /// <param name="newModul"> The modul. </param>    
    private void SetSelection(
      string newSchuljahr,
      string newHalbjahr,
      string newFach,
      string newLerngruppe,
      string newModul)
    {
      this.Schuljahr = App.MainViewModel.Schuljahre.FirstOrDefault(o => o.SchuljahrBezeichnung == newSchuljahr);
      this.Halbjahr = (Halbjahr)Enum.Parse(typeof(Halbjahr), newHalbjahr);
      this.Fach = App.MainViewModel.Fächer.FirstOrDefault(o => o.FachBezeichnung == newFach);
      this.Lerngruppe = App.MainViewModel.Lerngruppen.FirstOrDefault(o => o.LerngruppeBezeichnung == newLerngruppe);
      this.Modul = App.MainViewModel.Module.FirstOrDefault(o => o.ModulBezeichnung == newModul);
    }

    /// <summary>
    /// The set selection.
    /// </summary>
    /// <param name="newSchuljahr"> The schuljahr. </param>
    /// <param name="newHalbjahr"> The halbjahr. </param>
    /// <param name="newFach">The fach. </param>
    /// <param name="newKlasse"> The klasse. </param>
    /// <param name="newModul"> The modul. </param>    
    private void SetSelectionSilent(
      string newSchuljahr,
      string newHalbjahr,
      string newFach,
      string newLerngruppe,
      string newModul)
    {
      this.schuljahr = App.MainViewModel.Schuljahre.FirstOrDefault(o => o.SchuljahrBezeichnung == newSchuljahr);
      this.halbjahr = (Halbjahr)Enum.Parse(typeof(Halbjahr), newHalbjahr);
      this.fach = App.MainViewModel.Fächer.FirstOrDefault(o => o.FachBezeichnung == newFach);
      this.lerngruppe = App.MainViewModel.Lerngruppen.FirstOrDefault(o => o.LerngruppeBezeichnung == newLerngruppe);
      this.modul = App.MainViewModel.Module.FirstOrDefault(o => o.ModulBezeichnung == newModul);
    }

  }
}