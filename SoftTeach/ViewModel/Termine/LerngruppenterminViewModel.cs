﻿namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Linq;
  using System.Windows.Media;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;

  /// <summary>
  /// ViewModel of an individual lerngruppentermin
  /// </summary>
  public class LerngruppenterminViewModel : TerminViewModel
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="LerngruppenterminViewModel"/> Klasse. 
    /// </summary>
    /// <param name="lerngruppentermin">
    /// The underlying lerngruppentermin this ViewModel is to be based on
    /// </param>
    public LerngruppenterminViewModel(Lerngruppentermin lerngruppentermin)
      : base(lerngruppentermin)
    {
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="LerngruppenterminViewModel"/> Klasse. 
    /// </summary>
    /// <param name="parentTagesplan">
    /// The tagesplan this ViewModel belongs to
    /// </param>
    /// <param name="lerngruppentermin">
    /// The underlying lerngruppentermin this ViewModel is to be based on
    /// </param>
    public LerngruppenterminViewModel(TagesplanViewModel parentTagesplan, Lerngruppentermin lerngruppentermin)
      : base(lerngruppentermin)
    {
      if (parentTagesplan == null)
      {
        throw new ArgumentNullException("parentTagesplan");
      }

      this.ParentTagesplan = parentTagesplan;
      this.ViewLerngruppenterminCommand = new DelegateCommand(this.ViewLerngruppentermin);
      this.EditLerngruppenterminCommand = new DelegateCommand(this.EditLerngruppentermin);
      this.RemoveLerngruppenterminCommand = new DelegateCommand(this.DeleteTermin);
    }

    /// <summary>
    /// Holt den tagesplan this lerngruppentermin belongs to.
    /// </summary>
    public TagesplanViewModel ParentTagesplan { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Ansicht des aktuellen Lerngruppentermins
    /// </summary>
    public DelegateCommand ViewLerngruppenterminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur editing the current Lerngruppentermin
    /// </summary>
    public DelegateCommand EditLerngruppenterminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur removing the current Lerngruppentermin
    /// </summary>
    public DelegateCommand RemoveLerngruppenterminCommand { get; private set; }

    /// <summary>
    /// Holt den Kalenderfarbe of this Tagesplan
    /// </summary>
    [ViewModelBase.DependsUponAttribute("TerminTermintyp")]
    public SolidColorBrush LerngruppenterminFarbe
    {
      get
      {
        if (this.Model.Termintyp == null)
        {
          return Brushes.Black;
        }

        var convertFromString = ColorConverter.ConvertFromString(this.Model.Termintyp.Kalenderfarbe);
        return convertFromString != null ? new SolidColorBrush((Color)convertFromString) : Brushes.Transparent;
      }
    }

    /// <summary>
    /// Holt a <see cref="DateTime"/> with the date this lerngruppentermin belongs to
    /// </summary>
    public DateTime LerngruppenterminDatum
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Tagesplan.Datum;
      }
    }

    /// <summary>
    /// Holt a <see cref="DateTime"/> with the date this lerngruppentermin belongs to
    /// </summary>
    [ViewModelBase.DependsUponAttribute("LerngruppenterminDatum")]
    public string LerngruppenterminDatumKurz
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Tagesplan.Datum.ToString("ddd dd.MM");
      }
    }

    /// <summary>
    /// Holt a <see cref="Int32"/> with the week day index this lerngruppentermin belongs to
    /// </summary>
    public int LerngruppenterminWochentagIndex
    {
      get
      {
        return (int)((Lerngruppentermin)this.Model).Tagesplan.Datum.DayOfWeek;
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the schuljahr this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminSchuljahr
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Jahrtyp.Bezeichnung;
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the halbjahr this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminHalbjahr
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Halbjahrtyp.Bezeichnung;
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the fach this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminFach
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Fach.Bezeichnung;
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the klasse this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminKlasse
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Bezeichnung;
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Lerngruppentermin: " + this.TerminBeschreibung;
    }

    /// <summary>
    /// Handles deletion of the current termin
    /// </summary>
    protected override void DeleteTermin()
    {
      this.ParentTagesplan.DeleteLerngruppentermin(this);
    }

    /// <summary>
    /// Zeigt den aktuellen Lerngruppentermin
    /// </summary>
    protected virtual void ViewLerngruppentermin()
    {
      var dlg = new AddLerngruppenterminDialog
      {
        Terminbezeichnung = this.TerminBeschreibung,
        TerminTermintyp = this.TerminTermintyp,
        TerminErsteUnterrichtsstunde = this.TerminErsteUnterrichtsstunde,
        TerminLetzteUnterrichtsstunde = this.TerminLetzteUnterrichtsstunde
      };

      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        this.TerminBeschreibung = dlg.Terminbezeichnung;
        this.TerminTermintyp = dlg.TerminTermintyp;
        this.TerminErsteUnterrichtsstunde = dlg.TerminErsteUnterrichtsstunde;
        this.TerminLetzteUnterrichtsstunde = dlg.TerminLetzteUnterrichtsstunde;
        this.ParentTagesplan.UpdateBeschreibung();
      }
    }

    /// <summary>
    /// Handles deletion of the current Lerngruppentermin
    /// </summary>
    protected virtual void EditLerngruppentermin()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Lerngruppentermin {0} editieren", this), false))
      {
        var dlg = new AddLerngruppenterminDialog
                    {
                      Terminbezeichnung = this.TerminBeschreibung,
                      TerminTermintyp = this.TerminTermintyp,
                      TerminErsteUnterrichtsstunde = this.TerminErsteUnterrichtsstunde,
                      TerminLetzteUnterrichtsstunde = this.TerminLetzteUnterrichtsstunde
                    };

        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          this.TerminBeschreibung = dlg.Terminbezeichnung;
          this.TerminTermintyp = dlg.TerminTermintyp;
          this.TerminErsteUnterrichtsstunde = dlg.TerminErsteUnterrichtsstunde;
          this.TerminLetzteUnterrichtsstunde = dlg.TerminLetzteUnterrichtsstunde;
          this.ParentTagesplan.UpdateBeschreibung();
        }
      }
    }

    /// <summary>
    /// Aktualisiert die Schülerliste in der singleton Selection Klasse
    /// </summary>
    protected void UpdateSchülerlisteInSelection()
    {
      var schülerliste =
        App.MainViewModel.Schülerlisten.FirstOrDefault(
          o =>
          o.SchülerlisteFach.FachBezeichnung == this.LerngruppenterminFach
          && o.SchülerlisteJahrtyp.JahrtypBezeichnung == this.LerngruppenterminSchuljahr
          && o.SchülerlisteKlasse.KlasseBezeichnung == this.LerngruppenterminKlasse);
      Selection.Instance.Schülerliste = schülerliste;
    }
  }
}
