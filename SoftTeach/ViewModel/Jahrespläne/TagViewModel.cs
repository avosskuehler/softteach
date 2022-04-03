using SoftTeach;
using SoftTeach.ViewModel;
using SoftTeach.ViewModel.Helper;
using SoftTeach.ViewModel.Jahrespläne;
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
    public TagViewModel()
    {
      this.AddLerngruppenterminCommand = new DelegateCommand(this.AddLerngruppentermin);
      this.EditLerngruppenterminCommand = new DelegateCommand(this.EditLerngruppentermin);
      this.LöscheLerngruppenterminCommand = new DelegateCommand(this.LöscheLerngruppentermin);
      this.AddStundeCommand = new DelegateCommand(this.AddStunde);
    }

    /// <summary>
    /// Holt den Befehl aus Notizen einen neuen Termin zu machen
    /// </summary>
    public DelegateCommand AddLerngruppenterminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl aus Notizen einen neuen Termin zu machen
    /// </summary>
    public DelegateCommand EditLerngruppenterminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl aus Notizen einen neuen Termin zu machen
    /// </summary>
    public DelegateCommand AddStundeCommand { get; private set; }

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
    private void AddStunde()
    {
      var firstLerngruppenTermin = this.Lerngruppentermine.FirstOrDefault();
    }

    /// <summary>
    /// Erstellt einen neuen Lerngruppentermin für diesen Tag und die Lerngruppe
    /// </summary>
    private void AddLerngruppentermin()
    {
      //var newTermin = new Termin
      //{
      //  Schuljahr = Selection.Instance.Schuljahr.Model,
      //  Datum = this.Datum
      //};
      //var ggfUhrzeit = this.Notizen.Split(' ').FirstOrDefault();
      //if (int.TryParse(ggfUhrzeit, out int stunde))
      //{
      //  if (stunde > 24 || stunde < 0)
      //  {
      //    newTermin.Titel = this.Notizen;
      //  }
      //  else
      //  {
      //    newTermin.Zeit = new TimeSpan(stunde, 0, 0);
      //    newTermin.Titel = this.Notizen.Replace(ggfUhrzeit, string.Empty);
      //  }
      //}
      //else if (TimeSpan.TryParse(ggfUhrzeit, out TimeSpan uhrzeit))
      //{
      //  newTermin.Zeit = uhrzeit;
      //  newTermin.Titel = this.Notizen.Replace(ggfUhrzeit, string.Empty);
      //}
      //else
      //{
      //  newTermin.Titel = this.Notizen;
      //}
      //newTermin.Benutzer = App.MainViewModel.CurrentBenutzer.Model;
      //newTermin.Termintyp = App.MainViewModel.TermintypenCollection.First(o => o.Bezeichnung == "Sonstiges").Model;

      //App.UnitOfWork.Context.Termine.Add(newTermin);
      //var vm = new TerminViewModel(newTermin);
      //App.MainViewModel.TermineCollection.Add(vm);
      //this.Termine.Add(vm);
      //this.RaisePropertyChanged("Termine");
      //App.UnitOfWork.SaveChanges();
      //this.Notizen = string.Empty;
    }
    private void EditLerngruppentermin()
    {
      var firstLerngruppenTermin = this.Lerngruppentermine.FirstOrDefault();
      if (firstLerngruppenTermin != null)
      {
        firstLerngruppenTermin.EditLerngruppenterminCommand.Execute(null);
      }
    }

    /// <summary>
    /// Entfernt die gegebene Lerngruppentermin aus der Liste und der Datenbank
    /// </summary>
    /// <param name="aufgabeViewModel">Die Lerngruppentermin die gelöscht werden soll.</param>
    private void LöscheLerngruppentermin()
    {
      //if (this.CurrentLerngruppentermin != null)
      //{
      var firstLerngruppenTermin = this.Lerngruppentermine.FirstOrDefault();
      if (firstLerngruppenTermin != null)
      {
        this.LöscheLerngruppentermin(this.CurrentLerngruppentermin);
      }
      //}
    }

    /// <summary>
    /// Entfernt die gegebene Lerngruppentermin aus der Liste und der Datenbank
    /// </summary>
    /// <param name="lerngruppentermin">Die Lerngruppentermin die gelöscht werden soll.</param>
    public void LöscheLerngruppentermin(LerngruppenterminViewModel lerngruppentermin)
    {
      var success = App.UnitOfWork.Context.Termine.Remove(lerngruppentermin.Model);
      var result = this.Lerngruppentermine.Remove(lerngruppentermin);
      this.RaisePropertyChanged("Lerngruppentermine");
      App.UnitOfWork.SaveChanges();
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
      return "Tag";
    }

  }
}
