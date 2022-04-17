using SoftTeach;
using SoftTeach.ExceptionHandling;
using SoftTeach.Model.TeachyModel;
using SoftTeach.UndoRedo;
using SoftTeach.View.Termine;
using SoftTeach.ViewModel;
using SoftTeach.ViewModel.Helper;
using SoftTeach.ViewModel.Jahrespläne;
using SoftTeach.ViewModel.Personen;
using SoftTeach.ViewModel.Termine;
using SoftTeach.ViewModel.Wochenpläne;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SoftTeach.ViewModel.Jahrespläne
{
  public class TagViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Die momentan ausgewählte Lerngruppentermin des Tages
    /// </summary>
    private LerngruppenterminViewModel currentLerngruppentermin;
    private LerngruppeViewModel lerngruppe;
    private Halbjahr halbjahr;

    private DateTime datum;
    private string notizen;
    private bool enabled;
    private bool istImAktuellenMonat;
    private bool istHeute;
    private bool istWochenende;
    private bool istFerien;
    private bool istFeiertag;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahresplanViewModel"/> Klasse. 
    /// </summary>
    public TagViewModel(LerngruppeViewModel lerngruppe)
    {
      this.lerngruppe = lerngruppe;
      this.EditLerngruppenterminCommand = new DelegateCommand(this.EditLerngruppentermin);
      this.LöscheLerngruppenterminCommand = new DelegateCommand(this.LöscheLerngruppentermin);
    }

    /// <summary>
    /// Holt den Befehl aus Notizen einen neuen Termin zu machen
    /// </summary>
    public DelegateCommand EditLerngruppenterminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl aus Notizen einen neuen Termin zu machen
    /// </summary>
    public DelegateCommand LöscheLerngruppenterminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl einen Termin zu löschen
    /// </summary>
    public DelegateCommand LöscheTerminCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die Lerngruppenterminn des Tages
    /// </summary>
    public ObservableCollection<LerngruppenterminViewModel> Lerngruppentermine { get; set; }

    /// <summary>
    /// Holt oder setzt die Lerngruppentermin, die vom Tag ausgewählt ist
    /// </summary>
    public LerngruppenterminViewModel CurrentLerngruppentermin
    {
      get => this.currentLerngruppentermin;

      set
      {
        this.currentLerngruppentermin = value;
        this.RaisePropertyChanged("CurrentLerngruppentermin");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Tag mit DateTime.Now übereinstimmt.
    /// </summary>
    public bool IstHeute
    {
      get => this.istHeute;
      set
      {
        this.istHeute = value;
        this.RaisePropertyChanged("IstHeute");
      }
    }

    /// <summary>
    /// Holt oder setzt, ob dieser Tag zum aktuell ausgewählten Monat gehört.
    /// </summary>
    public bool IstImAktuellenMonat
    {
      get => this.istImAktuellenMonat;
      set
      {
        this.istImAktuellenMonat = value;
        this.RaisePropertyChanged("IstImAktuellenMonat");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Tag zum Wochenende gehört
    /// </summary>
    public bool IstWochenende
    {
      get => this.istWochenende;
      set
      {
        this.istWochenende = value;
        this.RaisePropertyChanged("IstWochenende");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Tag ein Ferientag ist
    /// </summary>
    public bool IstFerien
    {
      get => this.istFerien;
      set
      {
        this.istFerien = value;
        this.RaisePropertyChanged("IstFerien");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Tag ein Feiertag ist
    /// </summary>
    public bool IstFeiertag
    {
      get => this.istFeiertag;
      set
      {
        this.istFeiertag = value;
        this.RaisePropertyChanged("IstFeiertag");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob an diesem Tag neue Eintragungen stattfinden dürfen oder nicht.
    /// </summary>
    public bool Enabled
    {
      get => this.enabled;
      set
      {
        this.enabled = value;
        this.RaisePropertyChanged("Enabled");
      }
    }

    /// <summary>
    /// Holt oder setzt den Text, der in den Tag hineingeschrieben wurde.
    /// </summary>
    public string Notizen
    {
      get => this.notizen;
      set
      {
        this.notizen = value;
        this.RaisePropertyChanged("Notizen");
      }
    }

    /// <summary>
    /// Holt oder setzt das Datum des Tages
    /// </summary>
    public DateTime Datum
    {
      get => this.datum;
      set
      {
        this.datum = value;
        this.RaisePropertyChanged("Datum");
      }
    }

    /// <summary>
    /// Holt den Monat zum Tag
    /// </summary>
    [DependsUpon("Datum")]
    public int Monat
    {
      get
      {
        return this.Datum.Month;
      }
    }

    /// <summary>
    /// Holt oder setzt das Halbjahr zum Tag
    /// </summary>
    public Halbjahr Halbjahr
    {
      get => this.halbjahr;
      set
      {
        this.halbjahr = value;
        this.RaisePropertyChanged("Halbjahr");
      }
    }

    /// <summary>
    /// Holt den ersten Lerngruppentermin des Tages
    /// </summary>
    public LerngruppenterminViewModel ErsterLerngruppentermin
    {
      get
      {
        return this.Lerngruppentermine.FirstOrDefault();
      }
    }

    public ContextMenu TagContextMenu
    {
      get
      {
        return null;
      }
    }

    public SolidColorBrush TagKalenderfarbe
    {
      get
      {
        if (this.Lerngruppentermine.Any())
        {
          return this.Lerngruppentermine.First().LerngruppenterminFarbe;
        }

        return Brushes.Transparent;
      }
    }

    public string TagBeschreibung
    {
      get
      {
        if (this.Lerngruppentermine.Any())
        {
          return this.Lerngruppentermine.First().TerminBeschreibung;
        }

        return string.Empty;
      }
    }

    public bool KeineLerngruppentermine
    {
      get
      {
        if (this.Lerngruppentermine.Any())
        {
          return false;
        }

        return true;
      }
    }

    /// <summary>
    /// Erstellt eine neue Stunde für diesen Tag und die Lerngruppe
    /// </summary>
    public void AddStunde()
    {
      var stunde = new StundeNeu();
      stunde.ErsteUnterrichtsstunde =
        App.MainViewModel.Unterrichtsstunden.First(unterrichtsstunde => unterrichtsstunde.UnterrichtsstundeBezeichnung == "1").Model;
      stunde.LetzteUnterrichtsstunde =
        App.MainViewModel.Unterrichtsstunden.First(unterrichtsstunde => unterrichtsstunde.UnterrichtsstundeBezeichnung == "2").Model;
      stunde.Datum = this.Datum;
      stunde.Termintyp = Termintyp.Unterricht;
      stunde.Hausaufgaben = string.Empty;
      stunde.Ansagen = string.Empty;
      stunde.Lerngruppe = lerngruppe.Model;
      stunde.Halbjahr = this.Halbjahr;
      stunde.Jahrgang = lerngruppe.LerngruppeJahrgang;

      stunde.Fach = lerngruppe.LerngruppeFach.Model;

      var vm = new StundeViewModel(stunde);
      var stundeDlg = new EditStundeDialog(vm);
      if (stundeDlg.ShowDialog().GetValueOrDefault(false))
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Stunde {0} angelegt.", vm), false))
        {
          this.Lerngruppentermine.Add(vm);
          lerngruppe.Lerngruppentermine.Add(vm);
          this.CurrentLerngruppentermin = vm;
          this.UpdateView();
        }
      }
    }

    /// <summary>
    /// Erstellt einen neuen Lerngruppentermin für diesen Tag und die Lerngruppe
    /// </summary>
    public void AddLerngruppentermin()
    {
      var dlg = new AddLerngruppenterminDialog();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Lernguppentermin {0} angelegt.", dlg.Terminbezeichnung), false))
        {
          var ersteStunde = dlg.TerminErsteUnterrichtsstunde;
          var letzteStunde = dlg.TerminLetzteUnterrichtsstunde;

          var lerngruppentermin = new LerngruppenterminNeu();
          lerngruppentermin.Beschreibung = dlg.Terminbezeichnung;
          lerngruppentermin.Termintyp = dlg.TerminTermintyp;
          lerngruppentermin.ErsteUnterrichtsstunde = ersteStunde.Model;
          lerngruppentermin.LetzteUnterrichtsstunde = letzteStunde.Model;
          lerngruppentermin.Lerngruppe = this.lerngruppe.Model;
          lerngruppentermin.Datum = this.Datum;

          var vm = new LerngruppenterminViewModel(lerngruppentermin);

          this.Lerngruppentermine.Add(vm);
          lerngruppe.Lerngruppentermine.Add(vm);
          this.CurrentLerngruppentermin = vm;
          this.UpdateView();
        }
      }
    }

    private void EditLerngruppentermin()
    {
      if (this.currentLerngruppentermin != null)
      {
        this.currentLerngruppentermin.EditLerngruppenterminCommand.Execute(null);
      }
    }

    /// <summary>
    /// Entfernt die gegebene Lerngruppentermin aus der Liste und der Datenbank
    /// </summary>
    /// <param name="aufgabeViewModel">Die Lerngruppentermin die gelöscht werden soll.</param>
    private void LöscheLerngruppentermin()
    {
      if (this.currentLerngruppentermin != null)
      {
        var result = InformationDialog.Show("Stunde löschen?", "Soll die Stunde wirklich gelöscht werden?", true);
        if (result.GetValueOrDefault(false))
        {
          this.lerngruppe.Lerngruppentermine.RemoveTest(this.currentLerngruppentermin);
          this.Lerngruppentermine.Remove(this.currentLerngruppentermin);
          this.currentLerngruppentermin = null;
          this.UpdateView();
        }
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      if (obj is TagViewModel otherTagViewModel)
      {
        return this.Datum.CompareTo(otherTagViewModel.Datum);
      }

      throw new ArgumentException("Object is not a TagViewModel");
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
      return this.datum.ToShortDateString();
    }

    public void UpdateView()
    {
      this.RaisePropertyChanged("TagBeschreibung");
      this.RaisePropertyChanged("TagKalenderfarbe");
    }
  }
}
