﻿
namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Markup;
  using System.Windows.Media;
  using GongSolutions.Wpf.DragDrop;
  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;
  using SoftTeach.View.Stundenentwürfe;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Noten;
  using SoftTeach.ViewModel.Stundenentwürfe;

  /// <summary>
  /// ViewModel of an individual stunde
  /// </summary>
  public class StundeViewModel : LerngruppenterminViewModel, ISequencedObject, ICloneable, IDropTarget
  {
    /// <summary>
    /// The fach currently assigned to this stundenentwurf
    /// </summary>
    private FachViewModel fach;

    /// <summary>
    /// The schuljahr currently assigned to this stundenentwurf
    /// </summary>
    private ModulViewModel modul;

    /// <summary>
    /// The phase currently selected
    /// </summary>
    private PhaseViewModel currentPhase;

    /// <summary>
    /// The dateiverweis currently selected
    /// </summary>
    private DateiverweisViewModel currentDateiverweis;

    /// <summary>
    /// In diese Liste werden alle Phasen geschrieben, die in die 
    /// Zwischenablage kopiert werden
    /// </summary>
    private List<PhaseContainer> copyToClipboardList;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="parentTagesplan">
    /// The tagesplan this ViewModel belongs to
    /// </param>
    /// <param name="stunde">
    /// The underlying stunde this ViewModel is to be based on
    /// </param>
    public StundeViewModel(Stunde stunde)
      : base(stunde)
    {
      this.Model = stunde;

      this.AddStundennotenCommand = new DelegateCommand(this.AddStundennoten);
      this.SearchStundeCommand = new DelegateCommand(this.SearchStunde);
      this.PreviewStundeCommand = new DelegateCommand(this.PreviewStunde);
      this.PrintStundeCommand = new DelegateCommand(this.PrintStunde);
      this.PrintAllStundeCommand = new DelegateCommand(this.PrintAllStunde);
      this.AddHausaufgabenCommand = new DelegateCommand(this.AddHausaufgaben);
      this.EditStundeCommand = new DelegateCommand(this.EditStunde);

      this.AddPhaseCommand = new DelegateCommand(this.AddPhase);
      this.MovePhaseForwardCommand = new DelegateCommand(this.MovePhaseForward, () => this.CurrentPhase != null);
      this.MovePhaseBackwardCommand = new DelegateCommand(this.MovePhaseBackward, () => this.CurrentPhase != null);
      this.DeletePhaseCommand = new DelegateCommand(this.DeleteCurrentPhase, () => this.CurrentPhase != null);
      this.AddDateiverweisCommand = new DelegateCommand(this.AddDateiverweis);
      this.DeleteDateiverweisCommand = new DelegateCommand(this.DeleteCurrentDateiverweis, () => this.CurrentDateiverweis != null);
      this.CopyCommand = new DelegateCommand(this.Copy);
      this.PasteCommand = new DelegateCommand(this.Paste);

      // Build data structures for phasen
      this.Phasen = new ObservableCollection<PhaseViewModel>();
      foreach (var phase in stunde.Phasen.OrderBy(o => o.Reihenfolge))
      {
        var vm = new PhaseViewModel(phase);
        this.Phasen.Add(vm);
      }

      this.Phasen.CollectionChanged += this.PhasenCollectionChanged;
      this.copyToClipboardList = new List<PhaseContainer>();
      this.SelectedPhasen = new List<PhaseViewModel>();

      // Build data structures for dateiverweise
      this.Dateiverweise = new ObservableCollection<DateiverweisViewModel>();
      foreach (var dateiverweis in stunde.Dateiverweise)
      {
        var vm = new DateiverweisViewModel(dateiverweis);
        this.Dateiverweise.Add(vm);
      }

      this.Dateiverweise.CollectionChanged += this.DateiverweiseCollectionChanged;

      this.ModulView = new ListCollectionView(App.MainViewModel.Module)
      {
        Filter = this.ModulFilter
      };
      this.ModulView.SortDescriptions.Add(new SortDescription("ModulBezeichnung", ListSortDirection.Ascending));
      this.ModulView.Refresh();
    }

    /// <summary>
    /// Holt den Befehl, um vergessen Hausaufgaben anzulegen.
    /// </summary>
    public DelegateCommand EditStundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um vergessen Hausaufgaben anzulegen.
    /// </summary>
    public DelegateCommand AddHausaufgabenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um Noten für die Stunde zu geben
    /// </summary>
    public DelegateCommand AddStundennotenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new stundenentwurf
    /// </summary>
    public DelegateCommand AddStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur removing a new stundenentwurf
    /// </summary>
    public DelegateCommand RemoveStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur searching an existing stundenentwurf
    /// </summary>
    public DelegateCommand SearchStundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new stundenentwurf
    /// </summary>
    public DelegateCommand PreviewStundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur printing the current stundenentwurf
    /// </summary>
    public DelegateCommand PrintStundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur printing the current stundenentwurf including
    /// attached files
    /// </summary>
    public DelegateCommand PrintAllStundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl ein phase zu erstellen
    /// </summary>
    public DelegateCommand AddPhaseCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl die phasen in die nächste Stunde zu verschieben.
    /// </summary>
    public DelegateCommand MovePhaseForwardCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl die phasen in die vorangegangene Stunde zu verschieben.
    /// </summary>
    public DelegateCommand MovePhaseBackwardCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current phase
    /// </summary>
    public DelegateCommand DeletePhaseCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Dateiverweis
    /// </summary>
    public DelegateCommand AddDateiverweisCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the Dateiverweis
    /// </summary>
    public DelegateCommand DeleteDateiverweisCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zum Einfügen
    /// </summary>
    public DelegateCommand PasteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zum Einfügen
    /// </summary>
    public DelegateCommand CopyCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die gefilterten Module
    /// </summary>
    public ICollectionView ModulView { get; set; }

    /// <summary>
    /// Holt die Phasen for this stundenentwurf
    /// </summary>
    public ObservableCollection<PhaseViewModel> Phasen { get; private set; }

    /// <summary>
    /// Holt die markierten Phasen im Stundenentwurf
    /// </summary>
    public IList SelectedPhasen { get; set; }

    /// <summary>
    /// Holt den Dateiverweise for this stundenentwurf
    /// </summary>
    public ObservableCollection<DateiverweisViewModel> Dateiverweise { get; private set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob diese Stunde schon benotet wurde.
    /// </summary>
    public bool StundeIstBenotet
    {
      get
      {
        return ((Stunde)this.Model).IstBenotet;
      }

      set
      {
        if (value == ((Stunde)this.Model).IstBenotet)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(StundeIstBenotet), ((Stunde)this.Model).IstBenotet, value);
        ((Stunde)this.Model).IstBenotet = value;
        this.RaisePropertyChanged("StundeIstBenotet");
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected phase
    /// </summary>
    public PhaseViewModel CurrentPhase
    {
      get
      {
        return this.currentPhase;
      }

      set
      {
        this.currentPhase = value;
        this.RaisePropertyChanged("CurrentPhase");
        this.DeletePhaseCommand.RaiseCanExecuteChanged();
        this.MovePhaseForwardCommand.RaiseCanExecuteChanged();
        this.MovePhaseBackwardCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected Dateiverweis
    /// </summary>
    public DateiverweisViewModel CurrentDateiverweis
    {
      get
      {
        return this.currentDateiverweis;
      }

      set
      {
        this.currentDateiverweis = value;
        this.RaisePropertyChanged("CurrentDateiverweis");
        this.DeleteDateiverweisCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Ansagen
    /// </summary>
    public string StundeAnsagen
    {
      get
      {
        return ((Stunde)this.Model).Ansagen;
      }

      set
      {
        if (value == ((Stunde)this.Model).Ansagen) return;
        this.UndoablePropertyChanging(this, nameof(StundeAnsagen), ((Stunde)this.Model).Ansagen, value);
        ((Stunde)this.Model).Ansagen = value;
        this.RaisePropertyChanged("StundeAnsagen");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob der Computer benötigt wird
    /// </summary>
    public bool StundeComputer
    {
      get
      {
        return ((Stunde)this.Model).Computer;
      }

      set
      {
        if (value == ((Stunde)this.Model).Computer) return;
        this.UndoablePropertyChanging(this, nameof(StundeComputer), ((Stunde)this.Model).Computer, value);
        ((Stunde)this.Model).Computer = value;
        this.RaisePropertyChanged("StundeComputer");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public FachViewModel StundeFach
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (((Stunde)this.Model).Fach == null)
        {
          return null;
        }

        if (this.fach == null || this.fach.Model != ((Stunde)this.Model).Fach)
        {
          this.fach = App.MainViewModel.Fächer.SingleOrDefault(d => d.Model == ((Stunde)this.Model).Fach);
        }

        return this.fach;
      }

      set
      {
        if (value.FachBezeichnung == this.fach.FachBezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(StundeFach), this.fach, value);
        this.fach = value;
        ((Stunde)this.Model).Fach = value.Model;
        this.RaisePropertyChanged("StundeFach");
        this.ModulView.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt die Hausaufgaben
    /// </summary>
    public string StundeHausaufgaben
    {
      get
      {
        return ((Stunde)this.Model).Hausaufgaben;
      }

      set
      {
        if (value == ((Stunde)this.Model).Hausaufgaben) return;
        this.UndoablePropertyChanging(this, nameof(StundeHausaufgaben), ((Stunde)this.Model).Hausaufgaben, value);
        ((Stunde)this.Model).Hausaufgaben = value;
        this.RaisePropertyChanged("StundeHausaufgaben");
      }
    }

    /// <summary>
    /// Holt oder setzt die Jahrgangsstufe currently assigned to this Stundenentwurf
    /// </summary>
    public int StundeJahrgang
    {
      get
      {
        return ((Stunde)this.Model).Jahrgang;
      }

      set
      {
        if (value == ((Stunde)this.Model).Jahrgang) return;
        this.UndoablePropertyChanging(this, nameof(StundeJahrgang), ((Stunde)this.Model).Jahrgang, value);
        ((Stunde)this.Model).Jahrgang = value;
        this.RaisePropertyChanged("StundeJahrgang");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob kopiert werden muss
    /// </summary>
    public bool StundeKopieren
    {
      get
      {
        return ((Stunde)this.Model).Kopieren;
      }

      set
      {
        if (value == ((Stunde)this.Model).Kopieren) return;
        this.UndoablePropertyChanging(this, nameof(StundeKopieren), ((Stunde)this.Model).Kopieren, value);
        ((Stunde)this.Model).Kopieren = value;
        this.RaisePropertyChanged("StundeKopieren");
      }
    }

    /// <summary>
    /// Holt oder setzt die Schuljahr currently assigned to this Stundenentwurf
    /// </summary>
    public ModulViewModel StundeModul
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (((Stunde)this.Model).Modul == null)
        {
          return null;
        }

        if (this.modul == null || this.modul.Model != ((Stunde)this.Model).Modul)
        {
          this.modul = App.MainViewModel.Module.SingleOrDefault(d => d.Model == ((Stunde)this.Model).Modul);
        }

        return this.modul;
      }

      set
      {
        if (value == null) return;
        if (value == this.StundeModul) return;
        this.UndoablePropertyChanging(this, nameof(StundeModul), this.modul, value);
        this.modul = value;
        ((Stunde)this.Model).Modul = value.Model;
        this.RaisePropertyChanged("StundeModul");
      }
    }

    /// <summary>
    /// Holt die Stundenzahl
    /// </summary>
    [DependsUpon("TerminErsteUnterrichtsstunde")]
    [DependsUpon("TerminLetzteUnterrichtsstunde")]
    public int StundeStundenzahl
    {
      get
      {
        return this.TerminLetzteUnterrichtsstunde.UnterrichtsstundeIndex - this.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex + 1;
      }
    }

    /// <summary>
    /// Holt a linebreaked string with a short form string for the phasen
    /// of this stundenentwurf
    /// </summary>
    public string StundePhasenKurzform
    {
      get
      {
        var kurzform = string.Empty;
        foreach (var phaseViewModel in this.Phasen)
        {
          var inhalt = phaseViewModel.PhaseInhalt;
          kurzform += phaseViewModel.PhaseZeit + "' ";
          kurzform += phaseViewModel.PhaseSozialform + ": ";
          kurzform += inhalt.Length > 0 ? inhalt.Substring(0, Math.Min(100, inhalt.Length)) : string.Empty;
          if (inhalt.Length > 100)
          {
            kurzform += " ... ";
          }

          kurzform += Environment.NewLine;
        }

        return kurzform;
      }
    }

    /// <summary>
    /// Holt a linebreaked string with a short form string for the phasen
    /// of this stundenentwurf
    /// </summary>
    public string StundePhasenLangform
    {
      get
      {
        var langform = new StringBuilder();

        foreach (var phaseViewModel in this.Phasen)
        {
          //          var inhalt = phaseViewModel.PhaseInhalt;
          langform.Append(phaseViewModel.PhaseZeit);
          langform.Append("' ");
          langform.Append(phaseViewModel.PhaseSozialform);
          langform.Append(": ");
          langform.AppendLine(phaseViewModel.PhaseInhalt);
        }

        return langform.ToString();
      }
    }

    /// <summary>
    /// Holt a linebreaked string with a short form string for the phasen
    /// of this stundenentwurf
    /// </summary>
    public string StundeDateiliste
    {
      get
      {
        var dateiliste = new StringBuilder();

        foreach (var dateiverweis in this.Dateiverweise)
        {
          dateiliste.AppendLine(dateiverweis.DateiverweisDateinameOhnePfad);
        }

        return dateiliste.ToString();
      }
    }

    /// <summary>
    /// Holt den total minute amount that are used in the phasen collection
    /// without counting phases quoted as "Pause"
    /// </summary>
    public int StundeVerplanteMinuten
    {
      get
      {
        return this.Phasen.Where(
          o => o.PhaseMedium != Medium.Pause
          && o.PhaseInhalt != "Pause").Sum(phaseViewModel => phaseViewModel.PhaseZeit);
      }
    }

    /// <summary>
    /// Holt den total minute amount that are used in the phasen collection
    /// without counting phases quoted as "Pause" in relation to the available minutes as string
    /// </summary>
    [DependsUpon("StundeVerplanteMinuten")]
    public string StundeVerplanteMinutenVonAllen
    {
      get
      {
        return string.Format("{0} von {1}", this.StundeVerplanteMinuten, this.StundeStundenzahl * 45);
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob für den Stundenentwurf zuviel Zeit verplant wurde.
    /// </summary>
    [DependsUpon("StundeVerplanteMinuten")]
    public bool StundenentwurfIstZuvielZeitVerplant
    {
      get
      {
        return this.StundeVerplanteMinuten > this.StundeStundenzahl * 45;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob für den Stundenentwurf zuwenig Zeit verplant wurde.
    /// </summary>
    [DependsUpon("StundeVerplanteMinuten")]
    public bool StundeIstZuwenigZeitVerplant
    {
      get
      {
        return this.StundeVerplanteMinuten < this.StundeStundenzahl * 45;
      }
    }

    /// <summary>
    /// Holt die Farbe für den Minutencheck.
    /// </summary>
    [DependsUpon("StundeVerplanteMinuten")]
    public SolidColorBrush StundeMinutenCheckBrush
    {
      get
      {
        if (this.StundenentwurfIstZuvielZeitVerplant)
        {
          return Brushes.Red;
        }

        if (this.StundeIstZuwenigZeitVerplant)
        {
          return Brushes.Orange;
        }

        return Brushes.Green;
      }
    }

    /// <summary>
    /// Holt die Modulkurzbezeichnung für die Stunde, wenn ein Stundenentwurf existiert,
    /// ansonsten leer.
    /// </summary>
    [DependsUpon("StundeModul")]
    public string StundeModulKurzbezeichnung
    {
      get
      {
        if (this.StundeModul != null)
        {
          return this.StundeModul.ModulKurzbezeichnung;
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Holt die Stundennummer dieser Stunde.
    /// Das ist die laufende Nummer im aktuellen Halbjahresplan.
    /// </summary>
    public int StundeLaufendeStundennummer
    {
      get
      {
        var stundenindex = 0;
        var vorhergehendeTermine = ((Stunde)this.Model).Lerngruppe.Lerngruppentermine.OfType<Stunde>().Where(o => o.Halbjahr == ((Stunde)this.Model).Halbjahr && o.Datum < this.LerngruppenterminDatum);
        foreach (var termin in vorhergehendeTermine)
        {
          var letzteStunde = termin.LetzteUnterrichtsstunde.Stundenindex;
          var ersteStunde = termin.ErsteUnterrichtsstunde.Stundenindex;
          stundenindex += letzteStunde - ersteStunde + 1;
        }

        return stundenindex + 1;
      }
    }

    /// <summary>
    /// Holt oder setzt den Reihenfolge dieser Stunde im laufenden Jahresplan.
    /// </summary>
    public int Reihenfolge { get; set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob die Reihenfolge Vorrang vor allen
    /// anderer Reihenfolgen der gleichen Zahl hat.
    /// </summary>
    public bool IstZuerst { get; set; }

    /// <summary>
    /// Holt einen String mit dem Stundenbereich der laufenden Nummer.
    /// </summary>
    [DependsUpon("StundeLaufendeStundennummer")]
    [DependsUpon("TerminStundenanzahl")]
    public string StundeLaufendeNummerStundebereich
    {
      get
      {
        if (this.TerminStundenanzahl == 1)
        {
          return "Stunde " + this.StundeLaufendeStundennummer;
        }

        var letzteStundeNummer = this.StundeLaufendeStundennummer + this.TerminStundenanzahl - 1;
        return "Stunden " + this.StundeLaufendeStundennummer + "-" + letzteStundeNummer;
      }
    }

    /// <summary>
    /// Holt oder setzt die Beschreibung of this Lerngruppentermin
    /// </summary>
    public override string TerminBeschreibung
    {
      get
      {
        return string.IsNullOrEmpty(base.TerminBeschreibung) ? this.StundeLaufendeNummerStundebereich : base.TerminBeschreibung;
      }

      set
      {
        if (value == this.Model.Beschreibung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(TerminBeschreibung), this.Model.Beschreibung, value);
        this.Model.Beschreibung = value;
        this.RaisePropertyChanged("TerminBeschreibung");
      }
    }

    /// <summary>
    /// Holt den Stundenthema of the Stundenentwurf assigned to this stunde
    /// </summary>
    [DependsUpon("LerngruppenterminLerngruppe")]
    [DependsUpon("LerngruppenterminFach")]
    [DependsUpon("LerngruppenterminDatum")]
    public string StundeMetroTitel
    {
      get
      {
        return this.LerngruppenterminLerngruppe + " " + this.LerngruppenterminFach + " " + this.LerngruppenterminDatum.ToLongDateString();
      }
    }

    /// <summary>
    /// Holt den date of this stunde as as string
    /// </summary>
    [DependsUpon("LerngruppenterminDatum")]
    public string StundeDatum
    {
      get
      {
        return this.LerngruppenterminDatum.ToString("dd.MM.yy", new CultureInfo("de-DE"));
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    [DependsUpon("TerminStundenanzahl")]
    public int StundeBreite
    {
      get
      {
        return this.TerminStundenanzahl * Properties.Settings.Default.Stundenbreite;
      }
    }

    /// <summary>
    /// Holt den Stundenbedarf as a string
    /// </summary>
    [DependsUpon("TerminStundenanzahl")]
    public string StundeStundenbedarfString
    {
      get
      {
        return this.TerminStundenanzahl + "h";
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    [DependsUpon("StundeBreite")]
    public int StundeDetailBreite
    {
      get
      {
        return this.StundeBreite * 4;
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Stunde: " + this.TerminStundenbereich;
    }

    private void UpdateStundenentwurfPhasenzeitraum()
    {
      // Update phasen with correct zeitraum
      var phasen = this.Phasen;

      // Get begin time for this stunde
      var startzeit = this.TerminErsteUnterrichtsstunde.UnterrichtsstundeBeginn;
      var phasenstartzeit = startzeit;
      for (int x = 0; x < phasen.Count; x++)
      {
        var phase = phasen[x];

        var phasenendzeit = phasenstartzeit + new TimeSpan(0, phase.PhaseZeit, 0);
        var phasenzeitraum = phasenstartzeit.ToString(@"hh\:mm") + "-" + phasenendzeit.ToString(@"hh\:mm");
        phasenstartzeit = phasenendzeit;
        phase.PhaseZeitraum = phasenzeitraum;
      }
    }

    /// <summary>
    /// Zeigt den aktuellen Lerngruppentermin
    /// </summary>
    protected override void ViewLerngruppentermin()
    {
      this.UpdateLerngruppeInSelection();
      if (Configuration.Instance.IsMetroMode)
      {
        var stundePage = new MetroStundenentwurfPage();

        // Set correct times
        this.UpdateStundenentwurfPhasenzeitraum();
        stundePage.DataContext = this;
        Configuration.Instance.NavigationService.Navigate(stundePage);
      }
    }

    /// <summary>
    /// Handles deletion of the current Lerngruppentermin
    /// </summary>
    protected override void EditLerngruppentermin()
    {
      this.UpdateLerngruppeInSelection();
      bool undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Stunde {0} editieren", this), false))
      {
        var dlg = new EditStundeDialog(this);
        Selection.Instance.Stunde = this;
        if (!(undo = !dlg.ShowDialog().GetValueOrDefault(false)))
        {
          this.TerminBeschreibung = dlg.StundeViewModel.TerminBeschreibung;
          this.TerminTermintyp = dlg.StundeViewModel.TerminTermintyp;
          this.TerminErsteUnterrichtsstunde = dlg.StundeViewModel.TerminErsteUnterrichtsstunde;
          this.TerminLetzteUnterrichtsstunde = dlg.StundeViewModel.TerminLetzteUnterrichtsstunde;
          this.TerminTermintyp = dlg.StundeViewModel.TerminTermintyp;
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    ///// <summary>
    ///// Handles addition a new Stundenentwurf to the workspace and model
    ///// </summary>
    //private void AddStundenentwurf()
    //{
    //  var entwurf = new Stundenentwurf();
    //  entwurf.Datum = DateTime.Now;
    //  entwurf.Fach = ((Stunde)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Fach;
    //  entwurf.Jahrgangsstufe = ((Stunde)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Klassenstufe.Jahrgangsstufe;
    //  entwurf.Stundenzahl = this.TerminStundenanzahl;
    //  entwurf.Ansagen = string.Empty;
    //  entwurf.Computer = false;
    //  entwurf.Hausaufgaben = string.Empty;
    //  entwurf.Kopieren = false;
    //  entwurf.Stundenthema = "Neues Thema";
    //  var availableModule =
    //    App.MainViewModel.Module.Where(
    //      o => o.ModulJahrgang.Model == entwurf.Jahrgangsstufe && o.ModulFach.Model == entwurf.Fach);
    //  entwurf.Modul = availableModule.First().Model;

    //  var vm = new StundenentwurfViewModel(entwurf);
    //  App.MainViewModel.Stundenentwürfe.Add(vm);
    //  this.StundeStundenentwurf = vm;
    //}

    ///// <summary>
    ///// Removes the current assigned Stundenentwurf from this stunde
    ///// </summary>
    //private void RemoveStundenentwurf()
    //{
    //  if (this.StundeStundenentwurf != null &&
    //    this.StundeStundenentwurf.StundenentwurfPhasenKurzform == string.Empty) App.MainViewModel.Stundenentwürfe.RemoveTest(this.StundeStundenentwurf);
    //  this.StundeStundenentwurf = null;
    //}

    /// <summary>
    /// Searches a Stundenentwurf for this stunde.
    /// </summary>
    private void SearchStunde()
    {
      var dlg = new SearchStundeDialog(App.MainViewModel.StundenentwurfWorkspace);
      App.MainViewModel.StundenentwurfWorkspace.ModulFilter = this.StundeModul;

      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        //// Das ausgewählte Modul verschwindet wenn man den Stundenentwurf  setzt
        //// vermutlich durch das Combo
        //// Hack: zwischenspeichern und nachher wieder eintragen...
        var eStunde = dlg.SelectedStundeViewModel;
        //var modulBackup = eStunde.StundeModul;
        this.TerminBeschreibung = eStunde.Beschreibung;
        this.StundeAnsagen = eStunde.Ansagen;
        this.StundeComputer = eStunde.Computer;
        this.StundeFach = App.MainViewModel.Fächer.First(o => o.Model.Id == eStunde.FachId);
        this.StundeHausaufgaben = eStunde.Hausaufgaben;
        this.StundeKopieren = eStunde.Kopieren;
        this.StundeModul = App.MainViewModel.Module.First(o => o.Model.Id == eStunde.ModulId);
        this.Phasen.Clear();
        foreach (var phase in this.Phasen.ToList())
        {
          this.Phasen.RemoveTest(phase);
        }

        foreach (var phase in eStunde.Phasen)
        {
          var ePhase = new Phase
          {
            Inhalt = phase.Inhalt,
            Medium = phase.Medium,
            Reihenfolge = phase.Reihenfolge,
            Sozialform = phase.Sozialform,
            Stunde = ((Stunde)this.Model),
            Zeit = phase.Zeit
          };
          var vm = new PhaseViewModel(ePhase);
          this.Phasen.Add(vm);
        }
        //SequencingService.SetCollectionSequence(this.Phasen);

        this.Dateiverweise.Clear();
        foreach (var dateiverweis in eStunde.Dateiverweise)
        {
          var erDateiverweis = new Dateiverweis
          {
            Dateiname = dateiverweis.Dateiname,
            Dateityp = dateiverweis.Dateityp,
            Stunde = ((Stunde)this.Model)
          };
          this.Dateiverweise.Add(new DateiverweisViewModel(erDateiverweis));
        }
      }
    }

    /// <summary>
    /// Zeigt eine Druckvorschau des Stundenentwurfs für den Ausdruck.
    /// </summary>
    private void PreviewStunde()
    {
      // select printer and get printer settings
      var pd = new PrintDialog();

      // create a document
      var document = new FixedDocument { Name = "StundenAusdruck" };
      document.DocumentPaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

      // create a page
      var page1 = new FixedPage
      {
        Width = document.DocumentPaginator.PageSize.Width,
        Height = document.DocumentPaginator.PageSize.Height
      };

      // Set correct times
      this.UpdateStundenentwurfPhasenzeitraum();

      // create the print output usercontrol
      var content = new StundenentwurfPrintView
      {
        DataContext = this,
        Width = page1.Width,
        Height = page1.Height
      };

      page1.Children.Add(content);

      // add the page to the document
      var page1Content = new PageContent();
      ((IAddChild)page1Content).AddChild(page1);
      document.Pages.Add(page1Content);

      // create Preview
      var preview = new StundenentwurfFixedDocument
      {
        Owner = Application.Current.MainWindow,
        Document = document
      };

      preview.ShowDialog();
    }

    /// <summary>
    /// Druckt den Stundenentwurf ohne Nachfrage aus.
    /// </summary>
    private void PrintStunde()
    {
      // select printer and get printer settings
      var pd = new PrintDialog();
      //if (pd.ShowDialog() != true) return;

      // create a document
      var document = new FixedDocument { Name = "StundeAusdruck" };
      document.DocumentPaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

      // create a page
      var fixedPage = new FixedPage
      {
        Width = document.DocumentPaginator.PageSize.Width,
        Height = document.DocumentPaginator.PageSize.Height
      };

      // Set correct times
      this.UpdateStundenentwurfPhasenzeitraum();

      // create the print output usercontrol
      var content = new StundenentwurfPrintView
      {
        DataContext = this,
        Width = fixedPage.Width,
        Height = fixedPage.Height
      };

      fixedPage.Children.Add(content);

      // Update the layout of our FixedPage
      var size = document.DocumentPaginator.PageSize;
      fixedPage.Measure(size);
      fixedPage.Arrange(new Rect(new Point(), size));
      fixedPage.UpdateLayout();

      // print it out
      pd.PrintVisual(fixedPage, "Stundenentwurf-Ausdruck");
    }

    /// <summary>
    /// Druckt den Stundenentwurf und alle dazugehörigen Dateien aus.
    /// </summary>
    private void PrintAllStunde()
    {
      this.PrintStunde();
      foreach (var dateiverweisViewModel in this.Dateiverweise)
      {
        App.PrintFile(dateiverweisViewModel.DateiverweisDateiname);
      }
    }

    /// <summary>
    /// Ruft den Dialog zur Eingabe von Noten für die momentane Stunde auf
    /// </summary>
    private void AddStundennoten()
    {
      //bool undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Noten hinzugefügt"), false))
      {
        var stunden = new ObservableCollection<Stunde>
        {
          this.Model as Stunde
        };
        var viewModel = new StundennotenReminderWorkspaceViewModel(stunden);
        var dlg = new MetroStundennotenReminderWindow { DataContext = viewModel };
        dlg.ShowDialog();
      }
    }

    /// <summary>
    /// Hier wird der Dialog zur Hausaufgabenkontrolle aufgerufen
    /// </summary>
    private async void AddHausaufgaben()
    {
      this.UpdateLerngruppeInSelection();
      //var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgaben hinzugefügt."), false))
      {
        var addDlg = new MetroAddHausaufgabeDialog(this.LerngruppenterminDatum);
        var metroWindow = Configuration.Instance.MetroWindow;
        await metroWindow.ShowMetroDialogAsync(addDlg);
      }
    }

    private void EditStunde()
    {
      var dlg = new AskForStundeneigenschaftenDialog
      {
        Stundenzahl = this.TerminStundenanzahl,
        Stundenthema = this.TerminBeschreibung
      };
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        this.TerminBeschreibung = dlg.Stundenthema;
        this.TerminLetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.FirstOrDefault(o => o.UnterrichtsstundeIndex == (this.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex + dlg.Stundenzahl - 1));
      }
    }

    /// <summary>
    /// Updates the date.
    /// </summary>
    public void UpdateDate()
    {
      this.RaisePropertyChanged("StundeDatum");
    }

    /// <summary>
    /// Returns a deep copy of this StundenentwurfViewModel
    /// </summary>
    /// <returns>A cloned <see cref="StundenentwurfViewModel"/></returns>
    public object Clone()
    {
      App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
      var entwurfClone = new Stunde();
      using (new UndoBatch(App.MainViewModel, string.Format("Stunde geklont"), false))
      {
        entwurfClone.Ansagen = ((Stunde)this.Model).Ansagen;
        entwurfClone.Computer = ((Stunde)this.Model).Computer;
        foreach (var dateiverweis in ((Stunde)this.Model).Dateiverweise.ToList())
        {
          var dateiverweisClone = new Dateiverweis
          {
            Dateiname = dateiverweis.Dateiname,
            Dateityp = dateiverweis.Dateityp,
            Stunde = entwurfClone
          };
          //App.UnitOfWork.Context.Dateiverweise.Add(dateiverweisClone);
          entwurfClone.Dateiverweise.Add(dateiverweis);
        }

        entwurfClone.Datum = ((Stunde)this.Model).Datum;
        entwurfClone.Fach = ((Stunde)this.Model).Fach;
        entwurfClone.Hausaufgaben = ((Stunde)this.Model).Hausaufgaben;
        entwurfClone.Jahrgang = ((Stunde)this.Model).Jahrgang;
        entwurfClone.Kopieren = ((Stunde)this.Model).Kopieren;
        entwurfClone.Modul = ((Stunde)this.Model).Modul;
        entwurfClone.Beschreibung = ((Stunde)this.Model).Beschreibung;
        foreach (var phase in ((Stunde)this.Model).Phasen.ToList())
        {
          var phaseClone = new Phase
          {
            Reihenfolge = phase.Reihenfolge,
            Inhalt = phase.Inhalt,
            Medium = phase.Medium,
            Sozialform = phase.Sozialform,
            Zeit = phase.Zeit,
            Stunde = entwurfClone
          };
          //App.UnitOfWork.Context.Phasen.Add(phaseClone);
          entwurfClone.Phasen.Add(phaseClone);
        }
        //App.UnitOfWork.Context.Stundenentwürfe.Add(entwurfClone);
      }

      var vm = new StundeViewModel(entwurfClone);
      //App.MainViewModel.Stunden.Add(vm);
      App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;

      return vm;
    }

    /// <summary>
    /// Aktualisiert die verplanten Minuten und die abhängigen Eigenschaften
    /// </summary>
    public void NotifyPhaseZeitChanged()
    {
      this.RaisePropertyChanged("StundeVerplanteMinuten");
    }

    /// <summary>
    /// DragOver wird aufgerufen, wenn ein Element über eines der ListViews
    /// gezogen wird. Hier wird festgelegt, ob die Operation erlaubt wird oder nicht.
    /// </summary>
    /// <param name="dropInfo">Ein <see cref="DropInfo"/> mit dem Element was gezogen wird
    /// und dem Element auf das gezogen wurde.</param>
    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data as PhaseViewModel;
      var sourceItemCollection = dropInfo.Data as List<PhaseViewModel>;
      if (sourceItem == null && sourceItemCollection == null)
      {
        dropInfo.Effects = DragDropEffects.None;
        return;
      }

      if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Copy;
      }
      else
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Move;
      }
    }

    /// <summary>
    /// Führt die Darg and Drop Operation aus.
    /// </summary>
    /// <param name="dropInfo">Ein <see cref="DropInfo"/> mit dem Element was gezogen wird
    /// und dem Element auf das gezogen wurde.</param>
    public void Drop(IDropInfo dropInfo)
    {
      if (dropInfo.VisualTarget is not DataGrid)
      {
        return;
      }

      var targetGrid = dropInfo.VisualTarget as DataGrid;
      if (targetGrid.Name != "PhasenGrid")
      {
        return;
      }


      if (dropInfo.Data is PhaseViewModel)
      {
        this.PerformDropWithSinglePhase(dropInfo);
      }
      else if (dropInfo.Data is List<PhaseViewModel>)
      {
        this.PerformDropWithPhasencollection(dropInfo);
      }

      SequencingService.NeuAnordnen(this.Phasen);

      // Editing stoppen, da sonst kein Refresh funktioniert
      targetGrid.CommitEdit();
      targetGrid.CancelEdit();
    }

    private void PerformDropWithSinglePhase(IDropInfo dropInfo)
    {
      var phaseViewModel = (PhaseViewModel)dropInfo.Data;

      var newIndex = dropInfo.InsertIndex;
      //if (newIndex < 0)
      //{
      //  newIndex = this.Phasen.Count;
      //}

      //var oldIndex = this.Phasen.IndexOf(phaseViewModel);
      //if (newIndex > oldIndex)
      //{
      //  newIndex--;
      //}

      // Wenn Control gedrückt ist
      if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      {
        //if (newIndex > this.Phasen.Count)
        //{
        //  newIndex = this.Phasen.Count;
        //}
        var kopie = (PhaseViewModel)phaseViewModel.Clone();
        kopie.Reihenfolge = newIndex;
        kopie.IstZuerst = true;
        this.Phasen.Insert(newIndex, kopie);
      }
      else
      {
        //if (newIndex >= this.Phasen.Count)
        //{
        //  newIndex = this.Phasen.Count - 1;
        //}

        //if (newIndex != oldIndex)
        //{
        //  this.Phasen.Move(oldIndex, newIndex);
        //}
        phaseViewModel.Reihenfolge= newIndex;
        phaseViewModel.IstZuerst = true;
      }
    }

    private void PerformDropWithPhasencollection(IDropInfo dropInfo)
    {
      var phaseViewModels = (List<PhaseViewModel>)dropInfo.Data;

      var newIndex = dropInfo.InsertIndex;
      if (newIndex < 0)
      {
        newIndex = this.Phasen.Count;
      }

      foreach (var phaseViewModel in phaseViewModels.OrderByDescending(o => o.Reihenfolge))
      {
        var oldIndex = this.Phasen.IndexOf(phaseViewModel);
        if (newIndex > oldIndex)
        {
          newIndex--;
        }

        // Wenn Control gedrückt ist
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
          if (newIndex > this.Phasen.Count)
          {
            newIndex = this.Phasen.Count;
          }

          this.Phasen.Insert(newIndex, (PhaseViewModel)phaseViewModel.Clone());
        }
        else
        {
          if (newIndex >= this.Phasen.Count)
          {
            newIndex = this.Phasen.Count - 1;
          }

          if (newIndex != oldIndex)
          {
            this.Phasen.Move(oldIndex, newIndex);
          }
        }
      }
    }

    /// <summary>
    /// Tritt auf, wenn die PhasenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void PhasenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(Phasen), this.Phasen, e, true, "Änderung der Phasen");

      this.RaisePropertyChanged("StundenentwurfPhasenKurzform");
      this.NotifyPhaseZeitChanged();
    }

    /// <summary>
    /// Tritt auf, wenn die PhasenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void DateiverweiseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(Dateiverweise), this.Dateiverweise, e, true, "Änderung der Dateiverweise");
    }

    /// <summary>
    /// Handles addition a new phase to this stundenentwurf
    /// </summary>
    private void AddPhase()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("neue Phase angelegt"), false))
      {
        var phase = new Phase
        {
          Reihenfolge = this.Phasen.Count,
          Zeit = 10,
          Inhalt = string.Empty,
          Medium = Medium.Tafel,
          Sozialform = Sozialform.UG,
          Stunde = ((Stunde)this.Model)
        };
        //App.UnitOfWork.Context.Phasen.Add(phase);
        var vm = new PhaseViewModel(phase);

        this.Phasen.Add(vm);
        this.CurrentPhase = vm;
        this.NotifyPhaseZeitChanged();
      }
    }

    /// <summary>
    /// Fügt eine gegebene Phase diesem Stundenentwurf hinzu.
    /// An erster Stelle und unter zurücksetzung des
    /// Reihenfolgees.
    /// </summary>
    /// <param name="vm"> Das ViewModel der hinzuzufügenden Phase </param>
    /// <param name="index"> The index. </param>
    private void AddPhase(PhaseViewModel vm, int index)
    {
      vm.Model.Stunde = ((Stunde)this.Model);
      this.Phasen.Insert(index, vm);
      SequencingService.SetCollectionSequence(this.Phasen);
      this.CurrentPhase = vm;
      this.NotifyPhaseZeitChanged();
    }

    /// <summary>
    /// Fügt eine gegebene Phase diesem Stundenentwurf ans Ende hinzu.
    /// </summary>
    /// <param name="vm"> Das ViewModel der hinzuzufügenden Phase </param>
    public void AddPhase(PhaseViewModel vm)
    {
      vm.Model.Stunde = ((Stunde)this.Model);
      this.Phasen.Add(vm);
      SequencingService.SetCollectionSequence(this.Phasen);
      this.CurrentPhase = vm;
      this.NotifyPhaseZeitChanged();
    }

    /// <summary>
    /// Schiebt die gewählte(n) Phase(n) in die nächste Stunde.
    /// </summary>
    private void MovePhaseForward()
    {
      if (this.SelectedPhasen == null)
      {
        return;
      }

      var lg = App.MainViewModel.Lerngruppen.FirstOrDefault(o => o.Model.Id == ((Stunde)this.Model).Lerngruppe.Id);
      var nächsteStunde = lg.Lerngruppentermine.OfType<StundeViewModel>()
        .OrderBy(o => o.LerngruppenterminDatum)
        .FirstOrDefault(o => o.LerngruppenterminDatum > this.LerngruppenterminDatum);

      using (new UndoBatch(App.MainViewModel, string.Format("Phase verschoben"), false))
      {
        if (nächsteStunde != null)
        {
          var moveItems = new List<PhaseViewModel>(this.SelectedPhasen.Cast<PhaseViewModel>());
          foreach (var phaseViewModel in moveItems)
          {
            var phaseClone = (PhaseViewModel)phaseViewModel.Clone();
            nächsteStunde.AddPhase(phaseClone, 0);
            this.DeletePhase(phaseViewModel);
          }
        }
      }
    }

    /// <summary>
    /// Schiebt die gewählte(n) Phase(n) in die vorangegangene Stunde.
    /// </summary>
    private void MovePhaseBackward()
    {
      if (this.SelectedPhasen == null)
      {
        return;
      }

      var lg = App.MainViewModel.Lerngruppen.FirstOrDefault(o => o.Model.Id == ((Stunde)this.Model).Lerngruppe.Id);
      var nächsteStunde = lg.Lerngruppentermine.OfType<StundeViewModel>()
        .OrderBy(o => o.LerngruppenterminDatum)
        .LastOrDefault(o => o.LerngruppenterminDatum < this.LerngruppenterminDatum);

      using (new UndoBatch(App.MainViewModel, string.Format("Phase verschoben"), false))
      {
        if (nächsteStunde != null)
        {
          var moveItems = new List<PhaseViewModel>(this.SelectedPhasen.Cast<PhaseViewModel>());
          foreach (var phaseViewModel in moveItems)
          {
            var phaseClone = (PhaseViewModel)phaseViewModel.Clone();
            nächsteStunde.AddPhase(phaseClone, 0);
            this.DeletePhase(phaseViewModel);
          }
        }
      }
    }


    ///// <summary>
    ///// Event handler für die PropertyChanged event der Phase.
    ///// Aktualisiert die Zeiten des Stundenentwurfs.
    ///// </summary>
    ///// <param name="sender">Source of the event</param>
    ///// <param name="e">An <see cref="PropertyChangedEventArgs"/> with the property</param>
    //private void PhasePropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //  if (e.PropertyName == "PhaseZeit")
    //  {
    //    this.NotifyPhaseZeitChanged();
    //  }
    //}

    /// <summary>
    /// Handles deletion of the current phase
    /// </summary>
    private void DeleteCurrentPhase()
    {
      this.DeletePhase(this.CurrentPhase);
    }

    /// <summary>
    /// Handles deletion of the given phase
    /// </summary>
    /// <param name="phaseViewModel"> The phase View Model. </param>
    private void DeletePhase(PhaseViewModel phaseViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Phase {0} gelöscht.", phaseViewModel), false))
      {
        var result = this.Phasen.RemoveTest(phaseViewModel);
      }
    }

    /// <summary>
    /// Handles addition a new Dateiverweis to this stundenentwurf
    /// </summary>
    private void AddDateiverweis()
    {
      if (this.StundeFach == null)
      {
        return;
      }

      var dlg = new AskForDateiDialog();
      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      var dateiverweis = new Dateiverweis
      {
        Dateiname = dlg.DateinameMitPfad,
        Dateityp = dlg.Dateityp.Model,
        Stunde = ((Stunde)this.Model)
      };

      var vm = new DateiverweisViewModel(dateiverweis);
      using (new UndoBatch(App.MainViewModel, string.Format("Dateiverweis {0} erstellt.", vm), false))
      {
        this.Dateiverweise.Add(vm);
        this.CurrentDateiverweis = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current Dateiverweis
    /// </summary>
    private void DeleteCurrentDateiverweis()
    {
      if (this.CurrentDateiverweis == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Dateiverweis {0} gelöscht.", this.CurrentDateiverweis), false))
      {
        this.Dateiverweise.RemoveTest(this.CurrentDateiverweis);
        this.CurrentDateiverweis = null;
      }
    }

    /// <summary>
    /// Kopiert die ausgewählten Phasen in die Zwischenablage
    /// </summary>
    private void Copy()
    {
      Clipboard.Clear();
      this.copyToClipboardList.Clear();

      if (this.SelectedPhasen == null)
      {
        throw new ArgumentNullException("SelectedPhasen sind null!");
      }

      foreach (var row in this.SelectedPhasen)
      {
        if (row is not PhaseViewModel)
        {
          continue;
        }

        var phaseViewModel = row as PhaseViewModel;
        this.copyToClipboardList.Add(new PhaseContainer(
          phaseViewModel.PhaseInhalt,
          phaseViewModel.PhaseMedium,
          phaseViewModel.PhaseSozialform,
          phaseViewModel.PhaseZeit));
      }

      var newObject = new DataObject("PhasenCollection", this.copyToClipboardList);
      Clipboard.SetDataObject(newObject);
    }

    /// <summary>
    /// Fügt Phasen aus der Zwischenablage ein
    /// </summary>
    private void Paste()
    {
      var data = Clipboard.GetDataObject();
      if (data == null || !data.GetDataPresent("PhasenCollection"))
      {
        return;
      }

      var phasenCollection = data.GetData("PhasenCollection") as List<PhaseContainer>;
      var insertIndex = this.Phasen.Count;
      if (phasenCollection == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Phasen eingefügt"), false))
      {
        phasenCollection.Reverse();
        foreach (var phaseContainer in phasenCollection)
        {
          var phase = new Phase
          {
            Reihenfolge = insertIndex,
            Zeit = phaseContainer.Zeit,
            Inhalt = phaseContainer.Inhalt,
            Medium = phaseContainer.Medium,
            Sozialform = phaseContainer.Sozialform,
            Stunde = ((Stunde)this.Model)
          };
          var vm = new PhaseViewModel(phase);
          this.Phasen.Add(vm);
          SequencingService.SetCollectionSequence(this.Phasen);
          this.CurrentPhase = vm;
          this.NotifyPhaseZeitChanged();
        }
      }
      //this.PhasenView.Refresh();
    }

    /// <summary>
    /// Filtert die Terminliste nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool ModulFilter(object item)
    {
      var modulViewModel = item as ModulViewModel;
      if (modulViewModel == null)
      {
        return false;
      }

      if (this.StundeFach == null)
      {
        return false;
      }

      if (modulViewModel.ModulFach.FachBezeichnung != this.StundeFach.FachBezeichnung
        || modulViewModel.ModulJahrgang != this.StundeJahrgang)
      {
        return false;
      }

      return true;
    }

  }
}
