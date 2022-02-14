namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Linq;
  using System.Windows.Media;
  using SoftTeach.Model.TeachyModel;
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
    public LerngruppenterminViewModel(LerngruppenterminNeu lerngruppentermin)
      : base(lerngruppentermin)
    {
      this.ViewLerngruppenterminCommand = new DelegateCommand(this.ViewLerngruppentermin);
      this.EditLerngruppenterminCommand = new DelegateCommand(this.EditLerngruppentermin);
      this.RemoveLerngruppenterminCommand = new DelegateCommand(this.DeleteTermin);
    }

    /// <summary>
    /// Holt den underlying Termin this ViewModel is based on
    /// </summary>
    public new LerngruppenterminNeu Model { get; private set; }

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
    [DependsUpon("TerminTermintyp")]
    public SolidColorBrush LerngruppenterminFarbe
    {
      get
      {
        if (this.Model.Termintyp == null)
        {
          return Brushes.Black;
        }

        switch (this.Model.Termintyp)
        {
          case Termintyp.Klausur:
            return Brushes.Yellow;
          case Termintyp.TagDerOffenenTür:
            return Brushes.Blue;
          case Termintyp.Wandertag:
            return Brushes.Magenta;
          case Termintyp.Abitur:
            return Brushes.Red;
          case Termintyp.MSA:
            return Brushes.Red;
          case Termintyp.Unterricht:
            return Brushes.LightBlue;
          case Termintyp.Vertretung:
            return Brushes.LightGray;
          case Termintyp.Besprechung:
            return Brushes.Orange;
          case Termintyp.Sondertermin:
            return Brushes.Orange;
          case Termintyp.Ferien:
            return Brushes.Green;
          case Termintyp.Kursfahrt:
            return Brushes.Fuchsia;
          case Termintyp.Klassenfahrt:
            return Brushes.Fuchsia;
          case Termintyp.Projekttag:
            return Brushes.Orange;
          case Termintyp.Praktikum:
            return Brushes.Orange;
          case Termintyp.Geburtstag:
            return Brushes.SteelBlue;
          case Termintyp.Veranstaltung:
            return Brushes.Maroon;
          case Termintyp.PSE:
            return Brushes.Fuchsia;
          default:
            return Brushes.Transparent;
        }
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung dieser Lerngruppe.
    /// </summary>
    public DateTime LerngruppenterminDatum
    {
      get
      {
        return this.Model.Datum;
      }

      set
      {
        if (value == this.Model.Datum) return;
        this.UndoablePropertyChanging(this, "LerngruppenterminDatum", this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("LerngruppenterminDatum");
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
        return this.LerngruppenterminDatum.ToString("ddd dd.MM");
      }
    }

    /// <summary>
    /// Holt a <see cref="Int32"/> with the week day index this lerngruppentermin belongs to
    /// </summary>
    public int LerngruppenterminWochentagIndex
    {
      get
      {
        return (int)this.LerngruppenterminDatum.DayOfWeek;
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the schuljahr this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminSchuljahr
    {
      get
      {
        return this.Model.Lerngruppe.Schuljahr.Bezeichnung;
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the halbjahr this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminHalbjahr
    {
      get
      {
        return this.Model.Lerngruppe.Halbjahr.ToString();
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the fach this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminFach
    {
      get
      {
        return this.Model.Lerngruppe.Fach.Bezeichnung;
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the klasse this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminLerngruppe
    {
      get
      {
        return this.Model.Lerngruppe.Bezeichnung;
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
      //TODO
      //this.ParentTagesplan.DeleteLerngruppentermin(this);
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
        //this.ParentTagesplan.UpdateBeschreibung();
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
          //this.ParentTagesplan.UpdateBeschreibung();
        }
      }
    }

    /// <summary>
    /// Aktualisiert die Lerngruppe in der singleton Selection Klasse
    /// </summary>
    protected void UpdateLerngruppeInSelection()
    {
      var schülerliste =
        App.MainViewModel.Lerngruppen.FirstOrDefault(
          o =>
          o.LerngruppeFach.FachBezeichnung == this.LerngruppenterminFach
          && o.LerngruppeSchuljahr.SchuljahrBezeichnung == this.LerngruppenterminSchuljahr
          && o.LerngruppeBezeichnung == this.LerngruppenterminLerngruppe);
      Selection.Instance.Lerngruppe = schülerliste;
    }
  }
}
