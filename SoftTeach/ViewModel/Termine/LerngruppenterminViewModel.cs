namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Globalization;
  using System.Linq;
  using System.Windows.Media;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// ViewModel of an individual lerngruppentermin
  /// </summary>
  public class LerngruppenterminViewModel : TerminViewModel
  {
    /// <summary>
    /// Die Lerngruppe des Termins
    /// </summary>
    private LerngruppeViewModel lerngruppe;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="LerngruppenterminViewModel"/> Klasse. 
    /// </summary>
    /// <param name="lerngruppentermin">
    /// The underlying lerngruppentermin this ViewModel is to be based on
    /// </param>
    public LerngruppenterminViewModel(Lerngruppentermin lerngruppentermin)
      : base(lerngruppentermin)
    {
      this.ViewLerngruppenterminCommand = new DelegateCommand(this.ViewLerngruppentermin);
      this.EditLerngruppenterminCommand = new DelegateCommand(this.EditLerngruppentermin);
      this.RemoveLerngruppenterminCommand = new DelegateCommand(this.DeleteTermin);
      this.Model = lerngruppentermin;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="LerngruppenterminViewModel"/> Klasse. 
    /// </summary>
    /// <param name="lerngruppentermin">
    /// The underlying lerngruppentermin this ViewModel is to be based on
    /// </param>
    public LerngruppenterminViewModel(Stunde stunde)
      : base(stunde)
    {
      this.ViewLerngruppenterminCommand = new DelegateCommand(this.ViewLerngruppentermin);
      this.EditLerngruppenterminCommand = new DelegateCommand(this.EditLerngruppentermin);
      this.RemoveLerngruppenterminCommand = new DelegateCommand(this.DeleteTermin);
      this.Model = stunde;
    }

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
        return ((Lerngruppentermin)this.Model).Datum;
      }

      set
      {
        if (value == ((Lerngruppentermin)this.Model).Datum) return;
        this.UndoablePropertyChanging(this, nameof(LerngruppenterminDatum), ((Lerngruppentermin)this.Model).Datum, value);
        ((Lerngruppentermin)this.Model).Datum = value;
        this.RaisePropertyChanged("LerngruppenterminDatum");
      }
    }

    /// <summary>
    /// Holt das Datum des Monats des Lerngruppentermins
    /// </summary>
    [DependsUpon("LerngruppenterminDatum")]
    public string LerngruppenterminMonat
    {
      get
      {
        return this.LerngruppenterminDatum.ToString("MMM", new CultureInfo("de-DE"));
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
    public Schuljahr LerngruppenterminSchuljahr
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Lerngruppe.Schuljahr;
      }
    }

    /// <summary>
    /// Holt das Halbjahr des Lerngruppentermins
    /// </summary>
    public Halbjahr LerngruppenterminHalbjahr
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Halbjahr;
      }
    }

    /// <summary>
    /// Holt a <see cref="string"/> with the fach this lerngruppentermin belongs to
    /// </summary>
    public string LerngruppenterminFach
    {
      get
      {
        return ((Lerngruppentermin)this.Model).Lerngruppe.Fach.Bezeichnung;
      }
    }

    /// <summary>
    /// Holt oder setzt die ErsteUnterrichtsstunde currently assigned to this Termin
    /// </summary>
    public LerngruppeViewModel LerngruppenterminLerngruppe
    {
      get
      {
        var lg = ((Lerngruppentermin)this.Model).Lerngruppe;
        if (lg == null)
        {
          InformationDialog.Show("Lerngruppe fehlt", "Zu diesem Termin fehlt die Lerngruppe.", false);
          return null;
        }

        if (this.lerngruppe == null || this.lerngruppe.Model != lg)
        {
          var vm = App.MainViewModel.LoadLerngruppe(lg);
          this.lerngruppe = vm;
        }

        return this.lerngruppe;
      }

      set
      {
        if (value == null)
        {
          return;
        }
        this.UndoablePropertyChanging(this, nameof(LerngruppenterminLerngruppe), this.LerngruppenterminLerngruppe, value);
        this.lerngruppe = value;
        ((Lerngruppentermin)this.Model).Lerngruppe = value.Model;
        this.RaisePropertyChanged("LerngruppenterminLerngruppe");
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
      if (this is LerngruppenterminViewModel)
      {
        var lgt = this.Model as Lerngruppentermin;
        var lg = App.MainViewModel.Lerngruppen.FirstOrDefault(o => o.Model.Id == lgt.Lerngruppe.Id);
        if (lg != null)
        {
          lg.Lerngruppentermine.RemoveTest(this);
        }
      }
    }

    /// <summary>
    /// Zeigt den aktuellen Lerngruppentermin
    /// </summary>
    protected virtual void ViewLerngruppentermin()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Lerngruppentermin {0} editieren", this), false))
      {
        var dlg = new AddLerngruppenterminDialog
        {
          Terminbezeichnung = this.TerminBeschreibung,
          TerminTermintyp = this.TerminTermintyp,
          TerminOrt = this.TerminOrt,
          TerminErsteUnterrichtsstunde = this.TerminErsteUnterrichtsstunde,
          TerminLetzteUnterrichtsstunde = this.TerminLetzteUnterrichtsstunde
        };

        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          this.TerminBeschreibung = dlg.Terminbezeichnung;
          this.TerminTermintyp = dlg.TerminTermintyp;
          this.TerminErsteUnterrichtsstunde = dlg.TerminErsteUnterrichtsstunde;
          this.TerminLetzteUnterrichtsstunde = dlg.TerminLetzteUnterrichtsstunde;
          this.TerminOrt = dlg.TerminOrt;
          //this.ParentTagesplan.UpdateBeschreibung();
        }
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
          TerminOrt = this.TerminOrt,
          TerminErsteUnterrichtsstunde = this.TerminErsteUnterrichtsstunde,
          TerminLetzteUnterrichtsstunde = this.TerminLetzteUnterrichtsstunde
        };

        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          this.TerminBeschreibung = dlg.Terminbezeichnung;
          this.TerminTermintyp = dlg.TerminTermintyp;
          this.TerminErsteUnterrichtsstunde = dlg.TerminErsteUnterrichtsstunde;
          this.TerminLetzteUnterrichtsstunde = dlg.TerminLetzteUnterrichtsstunde;
          this.TerminOrt = dlg.TerminOrt;
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
          && o.LerngruppeSchuljahr.SchuljahrJahr == this.LerngruppenterminSchuljahr.Jahr
          && o.LerngruppeBezeichnung == this.LerngruppenterminLerngruppe.LerngruppeBezeichnung);
      Selection.Instance.Lerngruppe = schülerliste;
    }
  }
}
