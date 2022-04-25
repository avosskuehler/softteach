namespace SoftTeach.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Timers;
  using System.Windows;
  using System.Windows.Data;
  using System.Windows.Media;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Main;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Curricula;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Noten;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Sitzpläne;
  using SoftTeach.ViewModel.Stundenentwürfe;
  using SoftTeach.ViewModel.Stundenpläne;
  using SoftTeach.ViewModel.Termine;
  using SoftTeach.ViewModel.Wochenpläne;

  /// <summary>
  /// ViewModel for accessing all data
  /// </summary>
  public class MainViewModel : ViewModelBase, ISupportsUndo
  {
    /// <summary>
    /// The noten timer
    /// </summary>
    private Timer notenTimer;

    /// <summary>
    /// The error icon
    /// </summary>
    private ImageSource errorIcon;

    /// <summary>
    /// The inactive icon
    /// </summary>
    private ImageSource inactiveIcon;
    private ArbeitWorkspaceViewModel arbeitWorkspace;
    private RaumWorkspaceViewModel raumWorkspace;
    private LerngruppeWorkspaceViewModel lerngruppeWorkspace;
    private JahresplanWorkspaceViewModel jahresplanWorkspace;
    private SitzplanWorkspaceViewModel sitzplanWorkspace;
    private WochenplanWorkspaceViewModel wochenplanWorkspace;
    private TagesplanWorkspaceViewModel tagesplanWorkspace;
    private SucheStundeWorkspaceViewModel stundenentwurfWorkspace;
    //private ObservableCollection<StundeViewModel> stunden;
    private ObservableCollection<CurriculumViewModel> curricula;
    private ObservableCollection<RaumViewModel> räume;
    private ObservableCollection<SitzplanViewModel> sitzpläne;
    private ObservableCollection<ArbeitViewModel> arbeiten;
    private ObservableCollection<Personen.LerngruppeViewModel> lerngruppen;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MainViewModel"/> Klasse. 
    /// </summary>
    public MainViewModel()
    {
      this.notenTimer = new Timer(30000);
      this.notenTimer.Elapsed += this.NotenTimerElapsed;

      this.errorIcon = App.GetImageSource("Error.ico");
      this.inactiveIcon = App.GetImageSource("Inactive.ico");

      // Initialisiert Undo/Redo
      UndoService.Current[this].Clear();

      // Build data structures to populate areas of the application surface
      this.Schuljahre = new ObservableCollection<SchuljahrViewModel>();
      this.Dateitypen = new ObservableCollection<DateitypViewModel>();
      this.Unterrichtsstunden = new ObservableCollection<UnterrichtsstundeViewModel>();
      this.Jahrgänge = new ObservableCollection<int>();
      this.Fächer = new ObservableCollection<FachViewModel>();
      this.Module = new ObservableCollection<ModulViewModel>();
      //this.Reihen = new ObservableCollection<ReiheViewModel>();
      this.Ferien = new ObservableCollection<FerienViewModel>();
      this.Fachstundenanzahl = new ObservableCollection<FachstundenanzahlViewModel>();
      this.Personen = new ObservableCollection<PersonViewModel>();
      this.Zensuren = new ObservableCollection<ZensurViewModel>();
      this.NotenWichtungen = new ObservableCollection<NotenWichtungViewModel>();
      //this.Sequenzen = new ObservableCollection<SequenzViewModel>();
      this.Bewertungsschemata = new ObservableCollection<BewertungsschemaViewModel>();
      this.Prozentbereiche = new ObservableCollection<ProzentbereichViewModel>();

      this.räume = new ObservableCollection<RaumViewModel>();
      //this.Raumpläne = new ObservableCollection<RaumplanViewModel>();
      //this.Sitzplätze = new ObservableCollection<SitzplatzViewModel>();
      this.sitzpläne = new ObservableCollection<SitzplanViewModel>();
      //this.Sitzplaneinträge = new ObservableCollection<SitzplaneintragViewModel>();

      // The creation of the Arbeiten includes the creation of
      // the Aufgaben and Ergebnisse models
      this.arbeiten = new ObservableCollection<ArbeitViewModel>();
      //this.Aufgaben = new ObservableCollection<AufgabeViewModel>();
      //this.Ergebnisse = new ObservableCollection<ErgebnisViewModel>();

      // The creation of the Lerngruppen includes the creation of
      // the schülereintrag, noten, notentendenzen, hausaufgaben models
      this.lerngruppen = new ObservableCollection<Personen.LerngruppeViewModel>();
      //this.Schülereinträge = new ObservableCollection<SchülereintragViewModel>();
      //this.Noten = new ObservableCollection<NoteViewModel>();
      //this.Notentendenzen = new ObservableCollection<NotentendenzViewModel>();
      //this.Hausaufgaben = new ObservableCollection<HausaufgabeViewModel>();

      // The creation of the Schultermine includes the creation of
      // the betroffene klassen models, schultermin, lerngruppentermine, stunden
      this.Schultermine = new ObservableCollection<SchulterminViewModel>();
      //this.Lerngruppentermine = new ObservableCollection<LerngruppenterminViewModel>();
      //this.Stunden = new ObservableCollection<StundeViewModel>();
      //this.BetroffeneKlassen = new ObservableCollection<BetroffeneLerngruppeViewModel>();

      // The creation of the allJahrespläne includes the creation of the 
      // halbjahres/monats/tagesplan/stunde models
      this.Jahrespläne = new ObservableCollection<JahresplanViewModel>();
      //this.Halbjahrespläne = new ObservableCollection<HalbjahresplanViewModel>();
      //this.Monatspläne = new ObservableCollection<MonatsplanViewModel>();
      //this.Tagespläne = new ObservableCollection<TagesplanViewModel>();

      // The creation of the allStundenentwürfe includes the creation of
      // the phase and dateiverweis models
      //this.stunden = new ObservableCollection<StundeViewModel>();
      //this.Stundenentwürfe = new ObservableCollection<StundenentwurfViewModel>();
      //this.Phasen = new ObservableCollection<PhaseViewModel>();
      //this.Dateiverweise = new ObservableCollection<DateiverweisViewModel>();

      // The creation of the Curricula includes the creation of
      // the sequenz models
      this.curricula = new ObservableCollection<CurriculumViewModel>();

      // The creation of the alleStundenpläne includes the creation of
      // the Stundenplaneinträge models
      this.Stundenpläne = new ObservableCollection<StundenplanViewModel>();
      //this.Stundenplaneinträge = new ObservableCollection<StundenplaneintragViewModel>();

      this.SaveCommand = new DelegateCommand(this.SaveChanges);
      this.ShowOptionsCommand = new DelegateCommand(this.ShowOptions);
      this.CreateDirectoryTreeForDocuments();

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    }

    private void SelectionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Schuljahr")
      {
        LoadLerngruppen();
      }
    }

    /// <summary>
    /// Holt diesen Stack mit allen Changesets mit Informationen
    /// über die Schritte die rückgängig gemacht werden können.
    /// </summary>
    public IEnumerable<ChangeSet> UndoStack
    {
      get
      {
        return UndoService.Current[App.MainViewModel].UndoStack;
      }
    }

    /// <summary>
    /// Holt diesen Stack mit allen Changesets mit Informationen
    /// über die Schritte die wiederhergestellt werden können.
    /// </summary>
    public IEnumerable<ChangeSet> RedoStack
    {
      get
      {
        return UndoService.Current[App.MainViewModel].RedoStack;
      }
    }

    #region ModelCollections

    /// <summary>
    /// Holt alle Schuljahre der Datenbank
    /// </summary>
    public ObservableCollection<SchuljahrViewModel> Schuljahre { get; private set; }

    /// <summary>
    /// Holt alle Unterrichtsstunden der Datenbank
    /// </summary>
    public ObservableCollection<UnterrichtsstundeViewModel> Unterrichtsstunden { get; private set; }

    /// <summary>
    /// Holt alle Jahrgänge
    /// </summary>
    public ObservableCollection<int> Jahrgänge { get; private set; }

    /// <summary>
    /// Holt alle Lerngruppen der Datenbank
    /// </summary>
    public ObservableCollection<LerngruppeViewModel> Lerngruppen
    {
      get
      {
        return this.lerngruppen;
      }
    }

    ///// <summary>
    ///// Holt alle Schülereinträge der Datenbank
    ///// </summary>
    //public ObservableCollection<SchülereintragViewModel> Schülereinträge { get; private set; }

    ///// <summary>
    ///// Holt alle Noten der Datenbank
    ///// </summary>
    //public ObservableCollection<NoteViewModel> Noten { get; private set; }

    ///// <summary>
    ///// Holt alle Notentendenzen der Datenbank
    ///// </summary>
    //public ObservableCollection<NotentendenzViewModel> Notentendenzen { get; private set; }

    /// <summary>
    /// Holt alle Fächer der Datenbank
    /// </summary>
    public ObservableCollection<FachViewModel> Fächer { get; private set; }

    /// <summary>
    /// Holt alle Module der Datenbank
    /// </summary>
    public ObservableCollection<ModulViewModel> Module { get; private set; }

    ///// <summary>
    ///// Holt alle Reihen der Datenbank
    ///// </summary>
    //public ObservableCollection<ReiheViewModel> Reihen { get; private set; }

    /// <summary>
    /// Holt alle  der Datenbank
    /// </summary>
    public ObservableCollection<DateitypViewModel> Dateitypen { get; private set; }

    /// <summary>
    /// Holt alle Ferien der Datenbank
    /// </summary>
    public ObservableCollection<FerienViewModel> Ferien { get; private set; }

    /// <summary>
    /// Holt alle Fachstundenanzahlen der Datenbank
    /// </summary>
    public ObservableCollection<FachstundenanzahlViewModel> Fachstundenanzahl { get; private set; }

    /// <summary>
    /// Holt alle Lerngruppentermine der Datenbank
    /// </summary>
    public ObservableCollection<SchulterminViewModel> Schultermine { get; private set; }

    ///// <summary>
    ///// Holt alle BetroffeneKlassen der Datenbank
    ///// </summary>
    //public ObservableCollection<BetroffeneLerngruppeViewModel> BetroffeneKlassen { get; private set; }


    ///// <summary>
    ///// Holt alle Stundenentwürfe der Datenbank
    ///// </summary>
    //public ObservableCollection<StundeViewModel> Stunden
    //{
    //  get
    //  {
    //    return this.stunden;
    //  }
    //}

    ///// <summary>
    ///// Holt alle Phasen der Datenbank
    ///// </summary>
    //public ObservableCollection<PhaseViewModel> Phasen { get; private set; }

    ///// <summary>
    ///// Holt alle Dateiverweise der Datenbank
    ///// </summary>
    //public ObservableCollection<DateiverweisViewModel> Dateiverweise { get; private set; }

    /// <summary>
    /// Holt alle Curricula der Datenbank
    /// </summary>
    public ObservableCollection<CurriculumViewModel> Curricula
    {
      get
      {
        return this.curricula;
      }
    }

    ///// <summary>
    ///// Holt alle Sequenzen der Datenbank
    ///// </summary>
    //public ObservableCollection<SequenzViewModel> Sequenzen { get; private set; }

    /// <summary>
    /// Holt alle Stundenpläne der Datenbank
    /// </summary>
    public ObservableCollection<StundenplanViewModel> Stundenpläne { get; private set; }

    ///// <summary>
    ///// Holt alle Stundenplaneinträge der Datenbank
    ///// </summary>
    //public ObservableCollection<StundenplaneintragViewModel> Stundenplaneinträge { get; private set; }

    /// <summary>
    /// Holt alle Personen der Datenbank
    /// </summary>
    public ObservableCollection<PersonViewModel> Personen { get; private set; }

    /// <summary>
    /// Holt alle Zensuren der Datenbank
    /// </summary>
    public ObservableCollection<ZensurViewModel> Zensuren { get; private set; }

    /// <summary>
    /// Holt alle NotenWichtungen der Datenbank
    /// </summary>
    public ObservableCollection<NotenWichtungViewModel> NotenWichtungen { get; private set; }

    /// <summary>
    /// Holt alle Arbeiten der Datenbank
    /// </summary>
    public ObservableCollection<ArbeitViewModel> Arbeiten
    {
      get
      {
        return this.arbeiten;
      }
    }

    ///// <summary>
    ///// Holt alle Aufgaben der Datenbank
    ///// </summary>
    //public ObservableCollection<AufgabeViewModel> Aufgaben { get; private set; }

    ///// <summary>
    ///// Holt alle Ergebnisse der Datenbank
    ///// </summary>
    //public ObservableCollection<ErgebnisViewModel> Ergebnisse { get; private set; }

    /// <summary>
    /// Holt alle Bewertungsschemata der Datenbank
    /// </summary>
    public ObservableCollection<BewertungsschemaViewModel> Bewertungsschemata { get; private set; }

    /// <summary>
    /// Holt alle Prozentbereiche der Datenbank
    /// </summary>
    public ObservableCollection<ProzentbereichViewModel> Prozentbereiche { get; private set; }

    ///// <summary>
    ///// Holt alle Hausaufgaben der Datenbank
    ///// </summary>
    //public ObservableCollection<HausaufgabeViewModel> Hausaufgaben { get; private set; }

    /// <summary>
    /// Holt alle Räume der Datenbank
    /// </summary>
    public ObservableCollection<RaumViewModel> Räume
    {
      get
      {
        return this.räume;
      }
    }

    /// <summary>
    /// Holt alle Jahrespläne der Datenbank
    /// </summary>
    public ObservableCollection<JahresplanViewModel> Jahrespläne { get; private set; }


    ///// <summary>
    ///// Holt alle Raumpläne der Datenbank
    ///// </summary>
    //public ObservableCollection<RaumplanViewModel> Raumpläne { get; private set; }

    ///// <summary>
    ///// Holt alle Sitzplätze der Datenbank
    ///// </summary>
    //public ObservableCollection<SitzplatzViewModel> Sitzplätze { get; private set; }

    /// <summary>
    /// Holt alle Sitzpläne der Datenbank
    /// </summary>
    public ObservableCollection<SitzplanViewModel> Sitzpläne
    {
      get
      {
        return this.sitzpläne;
      }
    }


    ///// <summary>
    ///// Holt alle Sitzplaneinträge der Datenbank
    ///// </summary>
    //public ObservableCollection<SitzplaneintragViewModel> Sitzplaneinträge { get; private set; }


    #endregion ModelCollections

    #region Commands

    /// <summary>
    /// Holt den Befehl die letzte Datenbankaktion rückgängig zu machen.
    /// </summary>
    public DelegateCommand UndoCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl die letzte Datenbankaktion rückgängig zu machen.
    /// </summary>
    public DelegateCommand RedoCommand { get; private set; }

    /// <summary>
    /// Holt den command to save all changes made in the current sessions UnitOfWork
    /// </summary>
    public DelegateCommand SaveCommand { get; private set; }

    /// <summary>
    /// Holt den command to show options for the program
    /// </summary>
    public DelegateCommand ShowOptionsCommand { get; private set; }

    #endregion //Commands

    #region Workspaces

    /// <summary>
    /// Holt den workspace for managing stundenentwürfe
    /// </summary>
    public SucheStundeWorkspaceViewModel StundenentwurfWorkspace
    {
      get
      {
        if (this.stundenentwurfWorkspace == null)
        {
          this.stundenentwurfWorkspace = new SucheStundeWorkspaceViewModel();
        }

        return this.stundenentwurfWorkspace;
      }
    }

    /// <summary>
    /// Holt den workspace for managing Curricula
    /// </summary>
    public CurriculumWorkspaceViewModel CurriculumWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Wochenpläne
    /// </summary>
    public WochenplanWorkspaceViewModel WochenplanWorkspace
    {
      get
      {
        if (this.wochenplanWorkspace == null)
        {
          this.wochenplanWorkspace = new WochenplanWorkspaceViewModel();
        }

        return this.wochenplanWorkspace;
      }
    }


    /// <summary>
    /// Holt den workspace for managing Tagespläne
    /// </summary>
    public TagesplanWorkspaceViewModel TagesplanWorkspace
    {
      get
      {
        if (this.tagesplanWorkspace == null)
        {
          this.tagesplanWorkspace = new TagesplanWorkspaceViewModel();
        }

        return this.tagesplanWorkspace;
      }
    }

    /// <summary>
    /// Holt den workspace for managing Stundenpläne
    /// </summary>
    public StundenplanWorkspaceViewModel StundenplanWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Termine
    /// </summary>
    public SchulterminWorkspaceViewModel SchulterminWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Schuljahre
    /// </summary>
    public SchuljahrWorkspaceViewModel SchuljahrWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Fächer
    /// </summary>
    public FachWorkspaceViewModel FachWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Dateitypen
    /// </summary>
    public DateitypWorkspaceViewModel DateitypWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Module
    /// </summary>
    public ModulWorkspaceViewModel ModulWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Unterrichtsstunden
    /// </summary>
    public UnterrichtsstundeWorkspaceViewModel UnterrichtsstundeWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Ferien
    /// </summary>
    public FerienWorkspaceViewModel FerienWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Ferien
    /// </summary>
    public FachstundenanzahlWorkspaceViewModel FachstundenanzahlWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Lerngruppen
    /// </summary>
    public LerngruppeWorkspaceViewModel LerngruppeWorkspace
    {
      get
      {
        if (this.lerngruppeWorkspace == null)
        {
          this.lerngruppeWorkspace = new LerngruppeWorkspaceViewModel();
        }

        return this.lerngruppeWorkspace;
      }
    }

    /// <summary>
    /// Holt den workspace for managing Lerngruppen
    /// </summary>
    public JahresplanWorkspaceViewModel JahresplanWorkspace
    {
      get
      {
        if (this.jahresplanWorkspace == null)
        {
          this.jahresplanWorkspace = new JahresplanWorkspaceViewModel();
        }

        return this.jahresplanWorkspace;
      }
    }

    /// <summary>
    /// Holt den workspace for managing Schülereinträge
    /// </summary>
    public SchülereintragWorkspaceViewModel SchülereintragWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing Personen
    /// </summary>
    public PersonenWorkspaceViewModel PersonenWorkspace { get; private set; }

    /// <summary>
    /// Holt den workspace for managing NotenWichtung
    /// </summary>
    public NotenWichtungWorkspaceViewModel NotenWichtungWorkspace { get; private set; }

    /// <summary>
    /// Holt das Modul zur Bearbeitung von Zensuren.
    /// </summary>
    public ZensurWorkspaceViewModel ZensurWorkspace { get; private set; }

    /// <summary>
    /// Holt das Modul zur Bearbeitung von Arbeiten.
    /// </summary>
    public ArbeitWorkspaceViewModel ArbeitWorkspace
    {
      get
      {
        if (this.arbeitWorkspace == null)
        {
          this.arbeitWorkspace = new ArbeitWorkspaceViewModel();
        }

        return this.arbeitWorkspace;
      }
    }

    /// <summary>
    /// Holt das Modul zur Bearbeitung von Bewertungsschemata.
    /// </summary>
    public BewertungsschemaWorkspaceViewModel BewertungsschemaWorkspace { get; private set; }

    /// <summary>
    /// Holt das Modul zur Bearbeitung von Räumen.
    /// </summary>
    public RaumWorkspaceViewModel RaumWorkspace
    {
      get
      {
        if (this.raumWorkspace == null)
        {
          this.raumWorkspace = new RaumWorkspaceViewModel();
        }

        return this.raumWorkspace;
      }
    }

    /// <summary>
    /// Holt das Modul zur Bearbeitung von Sitzplänen.
    /// </summary>
    public SitzplanWorkspaceViewModel SitzplanWorkspace
    {
      get
      {
        // Schüler anlegen, wenn noch nicht geschehen, da für Sitzpläne benötigt
        var schüler = this.LerngruppeWorkspace;

        if (this.sitzplanWorkspace == null)
        {
          this.sitzplanWorkspace = new SitzplanWorkspaceViewModel();
        }

        return this.sitzplanWorkspace;
      }
    }


    #endregion Workspaces

    /// <summary>
    /// Holt eine Liste der ganzen Noten als String.
    /// </summary>
    public List<string> ZensurenList
    {
      get
      {
        var list = new List<string>(this.Zensuren.Count);
        list.AddRange(this.Zensuren.Select(zensurViewModel => zensurViewModel.ZensurGanzeNote.ToString("N0")));
        return list;
      }
    }

    /// <summary>
    /// Führt einen UndoSchritt aus.
    /// </summary>
    public void ExecuteUndoCommand()
    {
      UndoService.Current[this].Undo();
    }

    /// <summary>
    /// Gibt zurück, ob ein Undoschritt verfügbar ist.
    /// </summary>
    /// <returns>True, wenn der UndoStack mindestens ein Element enthält</returns>
    public bool CanExecuteUndoCommand()
    {
      // Are we in a state to do something?
      return UndoService.Current[this].CanUndo;
    }

    /// <summary>
    /// Führt einen Redo Schritt aus.
    /// </summary>
    public void ExecuteRedoCommand()
    {
      UndoService.Current[this].Redo();
    }

    /// <summary>
    /// Gibt zurück, ob ein Redo Schritt verfügbar ist.
    /// </summary>
    /// <returns>True, wenn der Redo Stack mindestens ein Element enthält</returns>
    public bool CanExecuteRedoCommand()
    {
      // Are we in a state to do something?
      return UndoService.Current[this].CanRedo;
    }

    /// <summary>
    /// Gets the "root document" or "root object" that this instance is part of.
    /// </summary>
    /// <returns>This MainViewModel object.</returns>
    public object GetUndoRoot()
    {
      return this;
    }

    /// <summary>
    /// Startet die noteneingabe.
    /// </summary>
    public void StartNoteneingabe()
    {
      var nochZuBenotendeStunden = this.HoleNochZuBenotendeStunden();
      if (!nochZuBenotendeStunden.Any())
      {
        return;
      }

      var viewModel = new StundennotenReminderWorkspaceViewModel(nochZuBenotendeStunden);
      var dlg = new MetroStundennotenReminderWindow { DataContext = viewModel };
      dlg.ShowDialog();
    }

    /// <summary>
    /// Populates this instance of the MainViewModel class.
    /// </summary>
    public void Populate()
    {
      //this.ImportDataFromOldContext();

      var context = App.UnitOfWork.Context;
      context.Configuration.AutoDetectChangesEnabled = false;
      ChangeFactory.Current.IsTracking = false;
      var watch = new Stopwatch();
      watch.Start();

      // Notenerinnerungstimer starten
      //this.notenTimer.Start();

      // TODO: Divide into multiple contexts for performance reasons
      try
      {
        //LoadRäume();
        //Console.WriteLine("Elapsed Räume {0}", watch.ElapsedMilliseconds);
        //watch.Restart();

        foreach (var schuljahr in context.Schuljahre)
        {
          this.Schuljahre.Add(new SchuljahrViewModel(schuljahr));
        }

        // Erstes Öffnen des Contexts dauert ca. 1,5s, hat nichts mit Schuljahren zu tun
        Console.WriteLine("Elapsed Schuljahre {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        foreach (var dateityp in context.Dateitypen.OrderBy(o => o.Bezeichnung))
        {
          this.Dateitypen.Add(new DateitypViewModel(dateityp));
        }
        //this.Dateitypen.BubbleSort();
        Console.WriteLine("Elapsed Dateitypen {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        foreach (var unterrichtsstunde in context.Unterrichtsstunden)
        {
          this.Unterrichtsstunden.Add(new UnterrichtsstundeViewModel(unterrichtsstunde));
        }
        Console.WriteLine("Elapsed Unterrichtsstunden {0}", watch.ElapsedMilliseconds);

        //foreach (var klasse in context.Klassen)
        //{
        //  this.Klassen.Add(new KlasseViewModel(klasse));
        //}
        //this.Klassen.BubbleSort();

        foreach (var fach in context.Fächer.OrderBy(o => o.Bezeichnung))
        {
          this.Fächer.Add(new FachViewModel(fach));
        }
        //this.Fächer.BubbleSort();
        Console.WriteLine("Elapsed Fächer {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        foreach (var modul in context.Module)
        {
          this.Module.Add(new ModulViewModel(modul));
        }
        Console.WriteLine("Elapsed Module {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        //foreach (var reihe in context.Reihen)
        //{
        //  this.Reihen.Add(new ReiheViewModel(reihe));
        //}

        //foreach (var sequenz in context.Sequenzen)
        //{
        //  this.Sequenzen.Add(new SequenzViewModel(sequenz));
        //}

        foreach (var ferien in context.Ferien)
        {
          this.Ferien.Add(new FerienViewModel(ferien));
        }
        Console.WriteLine("Elapsed Ferien {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        foreach (var fachstundenanzahl in context.Fachstundenanzahlen)
        {
          this.Fachstundenanzahl.Add(new FachstundenanzahlViewModel(fachstundenanzahl));
        }
        Console.WriteLine("Elapsed Fachstundenanzahl {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        for (int i = 7; i < 13; i++)
        {
          this.Jahrgänge.Add(i);
        }

        foreach (var zensur in context.Zensuren)
        {
          this.Zensuren.Add(new ZensurViewModel(zensur));
        }
        Console.WriteLine("Elapsed Zensuren {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        foreach (var notenWichtung in context.NotenWichtungen)
        {
          this.NotenWichtungen.Add(new NotenWichtungViewModel(notenWichtung));
        }
        Console.WriteLine("Elapsed NotenWichtungen {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        //LoadArbeiten();
        //Console.WriteLine("Elapsed Arbeiten {0}", watch.ElapsedMilliseconds);
        //watch.Restart();
        Selection.Instance.PopulateFromSettings();

        Console.WriteLine("PopulateFromSettings {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        foreach (SchulterminNeu termin in context.Termine.OfType<SchulterminNeu>().Where(o => o.Schuljahr.Jahr == Selection.Instance.Schuljahr.SchuljahrJahr))
        {
          this.Schultermine.Add(new SchulterminViewModel(termin as SchulterminNeu));
        }
        Console.WriteLine("Elapsed Schultermine {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        //foreach (BetroffeneKlasse betroffeneLerngruppe in context.BetroffeneKlassen)
        //{
        //  this.BetroffeneKlassen.Add(new BetroffeneKlasseViewModel(betroffeneLerngruppe));
        //}
        //Console.WriteLine("Elapsed BetroffeneKlasse {0}", watch.ElapsedMilliseconds);

        //foreach (Person person in context.Personen.Where(o => o.Schülereintrag.Any(a => a.Lerngruppe.Schuljahr.Jahr == Selection.Instance.Schuljahr.SchuljahrJahr)))
        foreach (PersonNeu person in context.Personen)
        {
          this.Personen.Add(new PersonViewModel(person));
        }
        Console.WriteLine("Elapsed Personen {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        //LoadLerngruppe();
        //Console.WriteLine("Elapsed Lerngruppen {0}", watch.ElapsedMilliseconds);
        //watch.Restart();

        //foreach (var schülereintrag in context.Schülereinträge)
        //{
        //  this.Schülereinträge.Add(new SchülereintragViewModel(schülereintrag));
        //}

        //foreach (var hausaufgabe in context.Hausaufgaben)
        //{
        //  this.Hausaufgaben.Add(new HausaufgabeViewModel(hausaufgabe));
        //}

        foreach (var bewertungsschema in context.Bewertungsschemata)
        {
          this.Bewertungsschemata.Add(new BewertungsschemaViewModel(bewertungsschema));
        }
        Console.WriteLine("Elapsed Bewertungsschemata {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        //foreach (var prozentbereich in context.Prozentbereiche)
        //{
        //  this.Prozentbereiche.Add(new ProzentbereichViewModel(prozentbereich));
        //}
        //Console.WriteLine("Elapsed Prozentbereiche {0}", watch.ElapsedMilliseconds);


        //foreach (var stunde in context.Termine.OfType<StundeNeu>().Where(o => o.Lerngruppe.Schuljahr.Jahr == Selection.Instance.Schuljahr.SchuljahrJahr))
        //{
        //  this.Stunden.Add(new StundeViewModel(stunde));
        //}
        //Console.WriteLine("Elapsed Stunden {0}", watch.ElapsedMilliseconds);
        //watch.Restart();

        //foreach (var phase in context.Phasen)
        //{
        //  this.Phasen.Add(new PhaseViewModel(phase));
        //}

        //foreach (var dateiverweis in context.Dateiverweise)
        //{
        //  this.Dateiverweise.Add(new DateiverweisViewModel(dateiverweis));
        //}
        //Console.WriteLine("Elapsed Dateiverweise {0}", watch.ElapsedMilliseconds);

        //foreach (var curriculum in context.Curricula)
        //{
        //  this.Curricula.Add(new CurriculumViewModel(curriculum));
        //}
        //Console.WriteLine("Elapsed Curricula {0}", watch.ElapsedMilliseconds);
        //watch.Restart();

        //LoadJahrespläne();
        //Console.WriteLine("Elapsed Jahrespläne {0}", watch.ElapsedMilliseconds);
        //watch.Restart();

        //foreach (var halbjahresplan in context.Halbjahrespläne)
        //{
        //  this.Halbjahrespläne.Add(new HalbjahresplanViewModel(halbjahresplan));
        //}
        //Console.WriteLine("Elapsed Halbjahrespläne {0}", watch.ElapsedMilliseconds);

        //foreach (var monatsplan in context.Monatspläne)
        //{
        //  this.Monatspläne.Add(new MonatsplanViewModel(monatsplan));
        //}
        //Console.WriteLine("Elapsed Monatspläne {0}", watch.ElapsedMilliseconds);

        //foreach (var tagesplan in context.Tagespläne)
        //{
        //  this.Tagespläne.Add(new TagesplanViewModel(tagesplan));
        //}
        //Console.WriteLine("Elapsed Tagespläne {0}", watch.ElapsedMilliseconds);

        //foreach (var schulwoche in context.Schulwochen)
        //{
        //  this.Schulwochen.Add(new SchulwocheViewModel(schulwoche));
        //}

        //foreach (var schultag in context.Schultage)
        //{
        //  this.Schultage.Add(new SchultagViewModel(schultag));
        //}

        foreach (var stundenplan in context.Stundenpläne)
        {
          this.Stundenpläne.Add(new StundenplanViewModel(stundenplan));
        }
        Console.WriteLine("Elapsed Stundenpläne {0}", watch.ElapsedMilliseconds);
        watch.Restart();

        //foreach (var stundenplaneintrag in context.Stundenplaneinträge)
        //{
        //  this.Stundenplaneinträge.Add(new StundenplaneintragViewModel(stundenplaneintrag));
        //}

        //foreach (var aufgabe in context.Aufgaben)
        //{
        //  this.Aufgaben.Add(new AufgabeViewModel(aufgabe));
        //}

        //foreach (var ergebnis in context.Ergebnisse)
        //{
        //  this.Ergebnisse.Add(new ErgebnisViewModel(ergebnis));
        //}

        //foreach (var note in context.Noten)
        //{
        //  this.Noten.Add(new NoteViewModel(note));
        //}
        //Console.WriteLine("Elapsed Noten {0}", watch.ElapsedMilliseconds);

        //foreach (var notentendenz in context.Notentendenzen)
        //{
        //  this.Notentendenzen.Add(new NotentendenzViewModel(notentendenz));
        //}

        // Weiter hinten, da personen, räume und schülerlisten benötigt werden
        //foreach (var sitzplan in context.Sitzpläne)
        //{
        //  this.Sitzpläne.Add(new SitzplanViewModel(sitzplan));
        //}
        //Console.WriteLine("Elapsed Sitzpläne {0}", watch.ElapsedMilliseconds);
        //watch.Restart();

        //foreach (var sitzplaneintrag in context.Sitzplaneinträge)
        //{
        //  this.Sitzplaneinträge.Add(new SitzplaneintragViewModel(sitzplaneintrag));
        //}


        //this.StundenentwurfWorkspace = new StundenentwurfWorkspaceViewModel();
        this.CurriculumWorkspace = new CurriculumWorkspaceViewModel();
        //this.JahresplanWorkspace = new JahresplanWorkspaceViewModel();
        this.StundenplanWorkspace = new StundenplanWorkspaceViewModel();
        this.SchulterminWorkspace = new SchulterminWorkspaceViewModel();
        //this.WochenplanWorkspace = new WochenplanWorkspaceViewModel();
        //this.TagesplanWorkspace = new TagesplanWorkspaceViewModel();
        this.DateitypWorkspace = new DateitypWorkspaceViewModel();
        this.FachWorkspace = new FachWorkspaceViewModel();
        this.SchuljahrWorkspace = new SchuljahrWorkspaceViewModel();
        this.ModulWorkspace = new ModulWorkspaceViewModel();
        this.UnterrichtsstundeWorkspace = new UnterrichtsstundeWorkspaceViewModel();
        this.FerienWorkspace = new FerienWorkspaceViewModel();
        this.FachstundenanzahlWorkspace = new FachstundenanzahlWorkspaceViewModel();
        //this.LerngruppeWorkspace = new LerngruppeWorkspaceViewModel();
        this.SchülereintragWorkspace = new SchülereintragWorkspaceViewModel();
        this.PersonenWorkspace = new PersonenWorkspaceViewModel();
        this.NotenWichtungWorkspace = new NotenWichtungWorkspaceViewModel();
        this.ZensurWorkspace = new ZensurWorkspaceViewModel();
        //this.ArbeitWorkspace = new ArbeitWorkspaceViewModel();
        this.BewertungsschemaWorkspace = new BewertungsschemaWorkspaceViewModel();
        //this.RaumWorkspace = new RaumWorkspaceViewModel();
        //this.SitzplanWorkspace = new SitzplanWorkspaceViewModel();

        this.RedoCommand = new DelegateCommand(this.ExecuteRedoCommand, this.CanExecuteRedoCommand);
        this.UndoCommand = new DelegateCommand(this.ExecuteUndoCommand, this.CanExecuteUndoCommand);

        // The undo / redo stack collections are not "Observable", so we 
        // need to manually refresh the UI when they change.
        var root = UndoService.Current[this];
        root.UndoStackChanged += this.RefreshUndoStackList;
        root.RedoStackChanged += this.RefreshUndoStackList;

        // Register collection changed events,
        // so dass undo/redo stack aktualisiert wird
        // wenn sich die collections ändern
        this.Schuljahre.CollectionChanged += this.SchuljahreCollectionChanged;
        this.Dateitypen.CollectionChanged += this.DateitypenCollectionChanged;
        this.Unterrichtsstunden.CollectionChanged += this.UnterrichtsstundenCollectionChanged;
        //this.Lerngruppen.CollectionChanged += this.LerngruppenCollectionChanged;
        this.Fächer.CollectionChanged += this.FächerCollectionChanged;
        this.Module.CollectionChanged += this.ModuleCollectionChanged;
        //this.Reihen.CollectionChanged += this.ReihenCollectionChanged;
        //this.Sequenzen.CollectionChanged += this.SequenzenCollectionChanged;
        this.Ferien.CollectionChanged += this.FerienCollectionChanged;
        this.Fachstundenanzahl.CollectionChanged += this.FachstundenanzahlCollectionChanged;
        this.Zensuren.CollectionChanged += this.ZensurenCollectionChanged;
        this.NotenWichtungen.CollectionChanged += this.NotenWichtungenCollectionChanged;
        //this.arbeiten.CollectionChanged += this.ArbeitenCollectionChanged;
        //this.Aufgaben.CollectionChanged += this.AufgabenCollectionChanged;
        //this.Ergebnisse.CollectionChanged += this.ErgebnisseCollectionChanged;
        this.Schultermine.CollectionChanged += this.SchultermineCollectionChanged;
        //this.Stunden.CollectionChanged += this.StundenCollectionChanged;
        //this.Lerngruppentermine.CollectionChanged += this.LerngruppentermineCollectionChanged;
        //this.BetroffeneKlassen.CollectionChanged += this.BetroffeneKlassenCollectionChanged;
        this.Personen.CollectionChanged += this.PersonenCollectionChanged;
        //this.schülerlisten.CollectionChanged += this.LerngruppenCollectionChanged;
        //this.Schülereinträge.CollectionChanged += this.SchülereinträgeCollectionChanged;
        //this.Noten.CollectionChanged += this.NotenCollectionChanged;
        //this.Notentendenzen.CollectionChanged += this.NotentendenzenCollectionChanged;
        //this.Hausaufgaben.CollectionChanged += this.HausaufgabenCollectionChanged;
        this.Bewertungsschemata.CollectionChanged += this.BewertungsschemataCollectionChanged;
        this.Prozentbereiche.CollectionChanged += this.ProzentbereicheCollectionChanged;

        //this.räume.CollectionChanged += this.RäumeCollectionChanged;
        //this.Raumpläne.CollectionChanged += this.RaumpläneCollectionChanged;
        //this.Sitzplätze.CollectionChanged += this.SitzplätzeCollectionChanged;
        //this.sitzpläne.CollectionChanged += this.SitzpläneCollectionChanged;
        //this.Sitzplaneinträge.CollectionChanged += this.SitzplaneinträgeCollectionChanged;

        //this.Stunden.CollectionChanged += this.StundenentwürfeCollectionChanged;
        //this.Phasen.CollectionChanged += this.PhasenCollectionChanged;
        //this.Dateiverweise.CollectionChanged += this.DateiverweiseCollectionChanged;
        this.Curricula.CollectionChanged += this.CurriculaCollectionChanged;
        //this.jahrespläne.CollectionChanged += this.JahrespläneCollectionChanged;
        //this.Halbjahrespläne.CollectionChanged += this.HalbjahrespläneCollectionChanged;
        //this.Monatspläne.CollectionChanged += this.MonatspläneCollectionChanged;
        //this.Tagespläne.CollectionChanged += this.TagespläneCollectionChanged;
        this.Stundenpläne.CollectionChanged += this.StundenpläneCollectionChanged;
        //this.Stundenplaneinträge.CollectionChanged += this.StundenplaneinträgeCollectionChanged;

        context.Configuration.AutoDetectChangesEnabled = true;
        ChangeFactory.Current.IsTracking = true;
        Console.WriteLine("Elapsed All {0}", watch.ElapsedMilliseconds);

      }
      catch (Exception ex)
      {
        Log.HandleException(ex);
      }
    }

    private void ImportDataFromOldContext()
    {
      var newContext = App.UnitOfWork.Context;
      var oldContext = App.UnitOfWork.OldContext;
      newContext.Configuration.AutoDetectChangesEnabled = false;
      using (var transaction = newContext.Database.BeginTransaction())
      {
        foreach (var notenWichtung in oldContext.NotenWichtungen)
        {
          var newNotenWichtung = new NotenWichtungNeu();
          newNotenWichtung.Bezeichnung = notenWichtung.Bezeichnung;
          newNotenWichtung.Id = notenWichtung.Id;
          newNotenWichtung.MündlichGesamt = notenWichtung.MündlichGesamt;
          newNotenWichtung.MündlichQualität = notenWichtung.MündlichQualität;
          newNotenWichtung.MündlichQuantität = notenWichtung.MündlichQuantität;
          newNotenWichtung.MündlichSonstige = notenWichtung.MündlichSonstige;
          newNotenWichtung.SchriftlichGesamt = notenWichtung.SchriftlichGesamt;
          newNotenWichtung.SchriftlichKlassenarbeit = notenWichtung.SchriftlichKlassenarbeit;
          newNotenWichtung.SchriftlichSonstige = notenWichtung.SchriftlichSonstige;
          newContext.NotenWichtungen.Add(newNotenWichtung);
        }
        var newDefaultOhneWichtung = new NotenWichtungNeu();
        newDefaultOhneWichtung.Bezeichnung = "Ohne Bewertung";
        newDefaultOhneWichtung.Id = 6;
        newDefaultOhneWichtung.MündlichGesamt = 0.5f;
        newDefaultOhneWichtung.MündlichQualität = 0.5f;
        newDefaultOhneWichtung.MündlichQuantität = 0.5f;
        newDefaultOhneWichtung.MündlichSonstige = 0f;
        newDefaultOhneWichtung.SchriftlichGesamt = 0.5f;
        newDefaultOhneWichtung.SchriftlichKlassenarbeit = 0.5f;
        newDefaultOhneWichtung.SchriftlichSonstige = 0.5f;
        newContext.NotenWichtungen.Add(newDefaultOhneWichtung);

        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Notenwichtungen] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Notenwichtungen] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {
        foreach (var jahrtyp in oldContext.Jahrtypen)
        {
          var newSchuljahr = new SchuljahrNeu();
          newSchuljahr.Bezeichnung = jahrtyp.Bezeichnung;
          newSchuljahr.Id = jahrtyp.Id;
          newSchuljahr.Jahr = jahrtyp.Jahr;
          newContext.Schuljahre.Add(newSchuljahr);
        }

        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Schuljahre] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Schuljahre] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {
        foreach (var fach in oldContext.Fächer)
        {
          var newFach = new FachNeu();
          newFach.Bezeichnung = fach.Bezeichnung;
          newFach.Farbe = fach.Farbe;
          newFach.Id = fach.Id;
          newContext.Fächer.Add(newFach);
        }

        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Fächer] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Fächer] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {
        foreach (var jahresplan in oldContext.Jahrespläne)
        {
          var newLerngruppe = new LerngruppeNeu();
          newLerngruppe.Bepunktungstyp = (Bepunktungstyp)Enum.Parse(typeof(Bepunktungstyp), jahresplan.Klasse.Klassenstufe.Jahrgangsstufe.Bepunktungstyp);
          newLerngruppe.Bezeichnung = jahresplan.Klasse.Bezeichnung;
          newLerngruppe.FachId = jahresplan.FachId;
          switch (jahresplan.Klasse.Klassenstufe.Bezeichnung)
          {
            case "7":
              newLerngruppe.Jahrgang = 7;
              break;
            case "8":
              newLerngruppe.Jahrgang = 8;
              break;
            case "9":
              newLerngruppe.Jahrgang = 9;
              break;
            case "10":
              newLerngruppe.Jahrgang = 10;
              break;
            case "GK-Q1/2":
              newLerngruppe.Jahrgang = 11;
              break;
            case "GK-Q3/4":
              newLerngruppe.Jahrgang = 12;
              break;
            case "LK-Q1/2":
              newLerngruppe.Jahrgang = 11;
              break;
            case "LK-Q3/4":
              newLerngruppe.Jahrgang = 12;
              break;
            default:
              newLerngruppe.Jahrgang = 0;
              break;
          }
          var schülerliste = oldContext.Schülerlisten.FirstOrDefault(o => o.JahrtypId == jahresplan.JahrtypId && o.FachId == jahresplan.FachId && o.KlasseId == jahresplan.Klasse.Id);
          if (schülerliste != null)
          {
            newLerngruppe.NotenWichtungId = schülerliste.NotenWichtungId;
          }
          else
          {
            newLerngruppe.NotenWichtungId = 6;
          }
          newLerngruppe.SchuljahrId = jahresplan.JahrtypId;
          newContext.Lerngruppen.Add(newLerngruppe);
        }
        //newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Lerngruppen] ON");
        newContext.SaveChanges();
        //newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Lerngruppen] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var bewertungsschemata in oldContext.Bewertungsschemata)
        {
          var newBewertungsschema = new BewertungsschemaNeu();
          newBewertungsschema.Bezeichnung = bewertungsschemata.Bezeichnung;
          newBewertungsschema.Id = bewertungsschemata.Id;
          newContext.Bewertungsschemata.Add(newBewertungsschema);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Bewertungsschemata] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Bewertungsschemata] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var zensur in oldContext.Zensuren)
        {
          var newZensur = new ZensurNeu();
          newZensur.GanzeNote = zensur.GanzeNote;
          newZensur.Id = zensur.Id;
          newZensur.NoteMitTendenz = zensur.NoteMitTendenz;
          newZensur.Notenpunkte = zensur.Notenpunkte;
          newContext.Zensuren.Add(newZensur);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Zensuren] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Zensuren] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var prozentbereich in oldContext.Prozentbereiche)
        {
          var newProzentbereich = new ProzentbereichNeu();
          newProzentbereich.BewertungsschemaId = prozentbereich.BewertungsschemaId;
          newProzentbereich.ZensurId = prozentbereich.ZensurId;
          newProzentbereich.BisProzent = prozentbereich.BisProzent;
          newProzentbereich.VonProzent = prozentbereich.VonProzent;
          newProzentbereich.Id = prozentbereich.Id;
          newContext.Prozentbereiche.Add(newProzentbereich);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Prozentbereiche] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Prozentbereiche] OFF");

        transaction.Commit();
      }

      var nichtImportierteArbeitenIDs = new List<int>();

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var arbeit in oldContext.Arbeiten)
        {
          var newArbeit = new ArbeitNeu();
          newArbeit.Bepunktungstyp = (Bepunktungstyp)Enum.Parse(typeof(Bepunktungstyp), arbeit.Bepunktungstyp);
          newArbeit.BewertungsschemaId = arbeit.BewertungsschemaId;
          newArbeit.Bezeichnung = arbeit.Bezeichnung;
          newArbeit.Datum = arbeit.Datum;
          newArbeit.FachId = arbeit.FachId;
          newArbeit.Id = arbeit.Id;
          newArbeit.IstKlausur = arbeit.IstKlausur;
          var lerngruppe = newContext.Lerngruppen.FirstOrDefault(o =>
             o.SchuljahrId == arbeit.JahrtypId
             && o.FachId == arbeit.FachId
             && o.Bezeichnung == arbeit.Klasse.Bezeichnung);
          if (lerngruppe == null)
          {
            nichtImportierteArbeitenIDs.Add(arbeit.Id);
            InformationDialog.Show("Lerngruppe nicht gefunden", string.Format("Lerngruppe {0} {1} {2} nicht gefunden", arbeit.Jahrtyp.Bezeichnung, arbeit.Fach.Bezeichnung, arbeit.Klasse.Bezeichnung), false);
            continue;
          }
          newArbeit.Lerngruppe = lerngruppe;
          newArbeit.LfdNr = arbeit.LfdNr;
          newContext.Arbeiten.Add(newArbeit);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Arbeiten] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Arbeiten] OFF");

        transaction.Commit();
      }

      var nichtImportierteAufgabenIDs = new List<int>();
      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var aufgabe in oldContext.Aufgaben)
        {
          if (nichtImportierteArbeitenIDs.Contains(aufgabe.ArbeitId))
          {
            nichtImportierteAufgabenIDs.Add(aufgabe.Id);
            continue;
          }

          var newAufgabe = new AufgabeNeu();
          newAufgabe.ArbeitId = aufgabe.ArbeitId;
          newAufgabe.Bezeichnung = aufgabe.Bezeichnung;
          newAufgabe.Id = aufgabe.Id;
          newAufgabe.LfdNr = aufgabe.LfdNr;
          newAufgabe.MaxPunkte = aufgabe.MaxPunkte;
          newContext.Aufgaben.Add(newAufgabe);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Aufgaben] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Aufgaben] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var person in oldContext.Personen)
        {
          var newPerson = new PersonNeu();
          newPerson.EMail = person.EMail;
          newPerson.Fax = person.Fax;
          newPerson.Foto = person.Foto;
          newPerson.Geburtstag = person.Geburtstag;
          newPerson.Geschlecht = person.Geschlecht ? Geschlecht.w : Geschlecht.m;
          newPerson.Handy = person.Handy;
          newPerson.Hausnummer = person.Hausnummer;
          newPerson.Id = person.Id;
          newPerson.IstLehrer = person.IstLehrer;
          newPerson.Nachname = person.Nachname;
          newPerson.Ort = person.Ort;
          newPerson.PLZ = person.PLZ;
          newPerson.Straße = person.Straße;
          newPerson.Telefon = person.Telefon;
          newPerson.Titel = person.Titel;
          newPerson.Vorname = person.Vorname;
          newContext.Personen.Add(newPerson);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Personen] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Personen] OFF");

        transaction.Commit();
      }

      var nichtImportierteSchülereintragIDs = new List<int>();
      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var schülereintrag in oldContext.Schülereinträge)
        {
          var newSchülereintrag = new SchülereintragNeu();
          newSchülereintrag.Id = schülereintrag.Id;
          var lerngruppe = newContext.Lerngruppen.FirstOrDefault(o =>
            o.SchuljahrId == schülereintrag.Schülerliste.JahrtypId
            && o.FachId == schülereintrag.Schülerliste.FachId
            && o.Bezeichnung == schülereintrag.Schülerliste.Klasse.Bezeichnung);
          if (lerngruppe == null)
          {
            nichtImportierteSchülereintragIDs.Add(schülereintrag.Id);
            continue;
          }

          newSchülereintrag.Lerngruppe = lerngruppe;
          newSchülereintrag.PersonId = schülereintrag.PersonId;
          newContext.Schülereinträge.Add(newSchülereintrag);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Schülereinträge] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Schülereinträge] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var ergebnis in oldContext.Ergebnisse)
        {
          if (nichtImportierteAufgabenIDs.Contains(ergebnis.AufgabeId))
          {
            continue;
          }
          var newErgebnis = new ErgebnisNeu();
          newErgebnis.AufgabeId = ergebnis.AufgabeId;
          newErgebnis.Id = ergebnis.Id;
          newErgebnis.Punktzahl = ergebnis.Punktzahl;
          newErgebnis.SchülereintragId = ergebnis.SchülereintragId;
          newContext.Ergebnisse.Add(newErgebnis);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Ergebnisse] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Ergebnisse] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var note in oldContext.Noten)
        {
          if (note.ArbeitId.HasValue && nichtImportierteArbeitenIDs.Contains(note.ArbeitId.Value))
          {
            continue;
          }
          if (nichtImportierteSchülereintragIDs.Contains(note.SchülereintragId))
          {
            continue;
          }

          var newNote = new NoteNeu();
          newNote.ArbeitId = note.ArbeitId;
          newNote.Bezeichnung = note.Bezeichnung;
          newNote.Datum = note.Datum;
          newNote.Id = note.Id;
          newNote.IstSchriftlich = note.IstSchriftlich;
          if (NotenTermintyp.TryParse(note.NotenTermintyp, out NotenTermintyp termintyp))
          {
            newNote.NotenTermintyp = termintyp;
          }
          newNote.Notentyp = (Notentyp)Enum.Parse(typeof(Notentyp), note.Notentyp);
          newNote.SchülereintragId = note.SchülereintragId;
          newNote.Wichtung = note.Wichtung;
          newNote.ZensurId = note.ZensurId;
          newContext.Noten.Add(newNote);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Noten] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Noten] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var unterrichtsstunde in oldContext.Unterrichtsstunden)
        {
          var newUnterrichtsstunde = new UnterrichtsstundeNeu();
          newUnterrichtsstunde.Beginn = unterrichtsstunde.Beginn;
          newUnterrichtsstunde.Bezeichnung = unterrichtsstunde.Bezeichnung;
          newUnterrichtsstunde.Ende = unterrichtsstunde.Ende;
          newUnterrichtsstunde.Id = unterrichtsstunde.Id;
          newUnterrichtsstunde.Stundenindex = unterrichtsstunde.Stundenindex;
          newContext.Unterrichtsstunden.Add(newUnterrichtsstunde);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Unterrichtsstunden] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Unterrichtsstunden] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {
        foreach (var modul in oldContext.Module)
        {
          var newModul = new ModulNeu();
          newModul.Bausteine = modul.Bausteine;
          newModul.Bezeichnung = modul.Bezeichnung;
          newModul.Id = modul.Id;
          newModul.FachId = modul.FachId;
          newModul.Stundenbedarf = modul.Stundenbedarf;
          switch (modul.Jahrgangsstufe.Bezeichnung)
          {
            case "7/8":
              newModul.Jahrgang = 7;
              break;
            case "9/10":
              newModul.Jahrgang = 8;
              break;
            case "Grundkurse":
            case "Leistungskurse":
              newModul.Jahrgang = 9;
              break;
            default:
              newModul.Jahrgang = 0;
              break;
          }
          newContext.Module.Add(newModul);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Module] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Module] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {
        foreach (var termin in oldContext.Termine.OfType<Schultermin>())
        {
          var newTermin = new SchulterminNeu();
          ((SchulterminNeu)newTermin).Datum = termin.Datum;
          ((SchulterminNeu)newTermin).SchuljahrId = termin.JahrtypId;
          newTermin.Beschreibung = termin.Beschreibung;
          newTermin.ErsteUnterrichtsstundeId = termin.ErsteUnterrichtsstundeId;
          newTermin.Id = termin.Id;
          newTermin.IstGeprüft = termin.IstGeprüft;
          newTermin.LetzteUnterrichtsstundeId = termin.LetzteUnterrichtsstundeId;
          newTermin.Ort = termin.Ort;
          if (Model.TeachyModel.Termintyp.TryParse(termin.Termintyp.Bezeichnung, out Model.TeachyModel.Termintyp typ))
          {
            newTermin.Termintyp = typ;
          }
          else
          {
            if (termin.Termintyp.Bezeichnung.StartsWith("Tag"))
            {
              newTermin.Termintyp = Model.TeachyModel.Termintyp.TagDerOffenenTür;
            }
            else
            {
              bool stop = true;
            }
          }
          newContext.Termine.Add(newTermin);
        }

        foreach (var stunde in oldContext.Termine.OfType<Stunde>())
        {
          var newTermin = new StundeNeu();

          if (stunde.Stundenentwurf == null)
          {
            continue;
          }

          newTermin.Ansagen = stunde.Stundenentwurf.Ansagen;
          newTermin.Beschreibung = stunde.Stundenentwurf.Stundenthema;
          newTermin.Computer = stunde.Stundenentwurf.Computer;
          newTermin.Datum = stunde.Tagesplan.Datum;
          newTermin.FachId = stunde.Stundenentwurf.FachId;
          if (stunde.Tagesplan.Monatsplan.Halbjahresplan.Halbjahrtyp.HalbjahrIndex == 1)
          {
            newTermin.Halbjahr = Halbjahr.Winter;
          }
          else
          {
            newTermin.Halbjahr = Halbjahr.Sommer;
          }
          newTermin.Hausaufgaben = stunde.Stundenentwurf.Hausaufgaben;
          newTermin.Id = stunde.Id;
          newTermin.IstBenotet = stunde.IstBenotet;
          switch (stunde.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Klassenstufe.Bezeichnung)
          {
            case "7":
              newTermin.Jahrgang = 7;
              break;
            case "8":
              newTermin.Jahrgang = 8;
              break;
            case "9":
              newTermin.Jahrgang = 9;
              break;
            case "10":
              newTermin.Jahrgang = 10;
              break;
            case "GK-Q1/2":
              newTermin.Jahrgang = 11;
              break;
            case "GK-Q3/4":
              newTermin.Jahrgang = 12;
              break;
            case "LK-Q1/2":
              newTermin.Jahrgang = 11;
              break;
            case "LK-Q3/4":
              newTermin.Jahrgang = 12;
              break;
            default:
              newTermin.Jahrgang = 0;
              break;
          }
          newTermin.Kopieren = stunde.Stundenentwurf.Kopieren;
          var lerngruppe = newContext.Lerngruppen.FirstOrDefault(o =>
            o.SchuljahrId == stunde.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.JahrtypId
            && o.FachId == stunde.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.FachId
            && o.Bezeichnung == stunde.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Bezeichnung);
          newTermin.Lerngruppe = lerngruppe;
          newTermin.ModulId = stunde.Stundenentwurf.ModulId;
          newTermin.Beschreibung = stunde.Beschreibung;
          newTermin.ErsteUnterrichtsstundeId = stunde.ErsteUnterrichtsstundeId;
          newTermin.Id = stunde.Id;
          newTermin.IstGeprüft = stunde.IstGeprüft;
          newTermin.LetzteUnterrichtsstundeId = stunde.LetzteUnterrichtsstundeId;
          newTermin.Ort = stunde.Ort;
          if (Model.TeachyModel.Termintyp.TryParse(stunde.Termintyp.Bezeichnung, out Model.TeachyModel.Termintyp typ))
          {
            newTermin.Termintyp = typ;
          }
          else
          {
            if (stunde.Termintyp.Bezeichnung.StartsWith("Tag"))
            {
              newTermin.Termintyp = Model.TeachyModel.Termintyp.TagDerOffenenTür;
            }
            else
            {
              bool stop = true;
            }
          }
          newContext.Termine.Add(newTermin);
        }

        foreach (var termin in oldContext.Termine.OfType<Lerngruppentermin>())
        {
          if (termin is Stunde)
          {
            continue;
          }

          var newTermin = new LerngruppenterminNeu();
          {
            newTermin.Datum = termin.Tagesplan.Datum;
            if (termin.Tagesplan.Monatsplan.Halbjahresplan.Halbjahrtyp.HalbjahrIndex == 1)
            {
              newTermin.Halbjahr = Halbjahr.Winter;
            }
            else
            {
              newTermin.Halbjahr = Halbjahr.Sommer;
            }
            var lerngruppe = newContext.Lerngruppen.FirstOrDefault(o =>
             o.SchuljahrId == termin.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.JahrtypId
             && o.FachId == termin.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.FachId
             && o.Bezeichnung == termin.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Bezeichnung);
            newTermin.Lerngruppe = lerngruppe;

            newTermin.Beschreibung = termin.Beschreibung;
            newTermin.ErsteUnterrichtsstundeId = termin.ErsteUnterrichtsstundeId;
            newTermin.Id = termin.Id;
            newTermin.IstGeprüft = termin.IstGeprüft;
            newTermin.LetzteUnterrichtsstundeId = termin.LetzteUnterrichtsstundeId;
            newTermin.Ort = termin.Ort;
            if (Model.TeachyModel.Termintyp.TryParse(termin.Termintyp.Bezeichnung, out Model.TeachyModel.Termintyp typ))
            {
              newTermin.Termintyp = typ;
            }
            else
            {
              if (termin.Termintyp.Bezeichnung.StartsWith("Tag"))
              {
                newTermin.Termintyp = Model.TeachyModel.Termintyp.TagDerOffenenTür;
              }
            }
            newContext.Termine.Add(newTermin);
          }
        }

        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Termine] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Termine] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var betroffeneLerngruppe in oldContext.BetroffeneKlassen)
        {
          var lerngruppen = newContext.Lerngruppen.Where(o =>
             o.SchuljahrId == betroffeneLerngruppe.Termin.JahrtypId
             && o.Bezeichnung == betroffeneLerngruppe.Klasse.Bezeichnung);

          foreach (var lerngruppe in lerngruppen)
          {
            var newBetroffeneLerngruppe = new BetroffeneLerngruppeNeu();
            //newBetroffeneKlasse.Id = betroffeneLerngruppe.Id;
            newBetroffeneLerngruppe.LerngruppeId = lerngruppe.Id;
            newBetroffeneLerngruppe.TerminId = betroffeneLerngruppe.TerminId;
            newContext.BetroffeneLerngruppen.Add(newBetroffeneLerngruppe);
          }
        }
        //newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[BetroffeneKlassen] ON");
        newContext.SaveChanges();
        //newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[BetroffeneKlassen] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var curriculum in oldContext.Curricula)
        {
          var newCurriculum = new CurriculumNeu();
          newCurriculum.Bezeichnung = curriculum.Bezeichnung;
          newCurriculum.FachId = curriculum.FachId;
          newCurriculum.Halbjahr = curriculum.Halbjahrtyp.HalbjahrIndex == 1 ? Halbjahr.Winter : Halbjahr.Sommer;
          newCurriculum.Id = curriculum.Id;
          switch (curriculum.Klassenstufe.Bezeichnung)
          {
            case "7":
              newCurriculum.Jahrgang = 7;
              break;
            case "8":
              newCurriculum.Jahrgang = 8;
              break;
            case "9":
              newCurriculum.Jahrgang = 9;
              break;
            case "10":
              newCurriculum.Jahrgang = 10;
              break;
            case "GK-Q1/2":
              newCurriculum.Jahrgang = 11;
              break;
            case "GK-Q3/4":
              newCurriculum.Jahrgang = 12;
              break;
            case "LK-Q1/2":
              newCurriculum.Jahrgang = 11;
              break;
            case "LK-Q3/4":
              newCurriculum.Jahrgang = 12;
              break;
            default:
              newCurriculum.Jahrgang = 0;
              break;
          }
          newCurriculum.SchuljahrId = curriculum.JahrtypId;
          newContext.Curricula.Add(newCurriculum);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Curricula] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Curricula] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var reihe in oldContext.Reihen)
        {
          var newReihe = new ReiheNeu();
          newReihe.CurriculumId = reihe.CurriculumId;
          newReihe.Id = reihe.Id;
          newReihe.ModulId = reihe.ModulId;
          newReihe.Reihenfolge = reihe.AbfolgeIndex;
          newReihe.Stundenbedarf = reihe.Stundenbedarf;
          newReihe.Thema = reihe.Thema;
          newContext.Reihen.Add(newReihe);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Reihen] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Reihen] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var sequenz in oldContext.Sequenzen)
        {
          var newSequenz = new SequenzNeu();
          newSequenz.Id = sequenz.Id;
          newSequenz.ReiheId = sequenz.ReiheId;
          newSequenz.Reihenfolge = sequenz.AbfolgeIndex;
          newSequenz.Stundenbedarf = sequenz.Stundenbedarf;
          newSequenz.Thema = sequenz.Thema;
          newContext.Sequenzen.Add(newSequenz);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Sequenzen] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Sequenzen] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var dateityp in oldContext.Dateitypen)
        {
          var newDateityp = new DateitypNeu();
          newDateityp.Bezeichnung = dateityp.Bezeichnung;
          newDateityp.Id = dateityp.Id;
          newDateityp.Kürzel = dateityp.Kürzel;
          newContext.Dateitypen.Add(newDateityp);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Dateitypen] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Dateitypen] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var dateiverweis in oldContext.Dateiverweise)
        {
          foreach (var stunde in dateiverweis.Stundenentwurf.Stunden)
          {
            var newDateiverweis = new DateiverweisNeu();
            newDateiverweis.Dateiname = dateiverweis.Dateiname;
            newDateiverweis.DateitypId = dateiverweis.DateitypId;
            //newDateiverweis.Id = dateiverweis.Id;
            newDateiverweis.StundeId = stunde.Id;
            newContext.Dateiverweise.Add(newDateiverweis);
          }
        }
        //newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Dateiverweise] ON");
        newContext.SaveChanges();
        //newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Dateiverweise] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var fachstundenanzahl in oldContext.Fachstundenanzahlen)
        {
          var newFachstundenanzahl = new FachstundenanzahlNeu();
          newFachstundenanzahl.FachId = fachstundenanzahl.FachId;
          newFachstundenanzahl.Id = fachstundenanzahl.Id;
          switch (fachstundenanzahl.Klassenstufe.Bezeichnung)
          {
            case "7":
              newFachstundenanzahl.Jahrgang = 7;
              break;
            case "8":
              newFachstundenanzahl.Jahrgang = 8;
              break;
            case "9":
              newFachstundenanzahl.Jahrgang = 9;
              break;
            case "10":
              newFachstundenanzahl.Jahrgang = 10;
              break;
            case "GK-Q1/2":
              newFachstundenanzahl.Jahrgang = 11;
              break;
            case "GK-Q3/4":
              newFachstundenanzahl.Jahrgang = 12;
              break;
            case "LK-Q1/2":
              newFachstundenanzahl.Jahrgang = 11;
              break;
            case "LK-Q3/4":
              newFachstundenanzahl.Jahrgang = 12;
              break;
            default:
              newFachstundenanzahl.Jahrgang = 0;
              break;
          }

          newFachstundenanzahl.Stundenzahl = fachstundenanzahl.Stundenzahl;
          newFachstundenanzahl.Teilungsstundenzahl = fachstundenanzahl.Teilungsstundenzahl;
          newContext.Fachstundenanzahlen.Add(newFachstundenanzahl);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Fachstundenanzahlen] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Fachstundenanzahlen] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var ferien in oldContext.Ferien)
        {
          var newFerien = new FerienNeu();
          newFerien.Bezeichnung = ferien.Bezeichnung;
          newFerien.Id = ferien.Id;
          newFerien.ErsterFerientag = ferien.ErsterFerientag;
          newFerien.LetzterFerientag = ferien.LetzterFerientag;
          newFerien.SchuljahrId = ferien.JahrtypId;
          newContext.Ferien.Add(newFerien);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Ferien] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Ferien] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var hausaufgabe in oldContext.Hausaufgaben)
        {
          var newHausaufgabe = new HausaufgabeNeu();
          newHausaufgabe.Bezeichnung = hausaufgabe.Bezeichnung;
          newHausaufgabe.Id = hausaufgabe.Id;
          newHausaufgabe.Datum = hausaufgabe.Datum;
          newHausaufgabe.IstNachgereicht = hausaufgabe.IstNachgereicht;
          newHausaufgabe.SchülereintragId = hausaufgabe.SchülereintragId;
          newContext.Hausaufgaben.Add(newHausaufgabe);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Hausaufgaben] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Hausaufgaben] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var phase in oldContext.Phasen)
        {
          foreach (var stunde in phase.Stundenentwurf.Stunden)
          {
            var newPhase = new PhaseNeu();
            newPhase.Inhalt = phase.Inhalt;
            if (Model.TeachyModel.Medium.TryParse(phase.Medium.Bezeichnung, out Model.TeachyModel.Medium medium))
            {
              newPhase.Medium = medium;
            }
            newPhase.Reihenfolge = phase.AbfolgeIndex;
            if (Model.TeachyModel.Sozialform.TryParse(phase.Sozialform.Bezeichnung, out Model.TeachyModel.Sozialform sozialform))
            {
              newPhase.Sozialform = sozialform;
            }
            newPhase.StundeId = stunde.Id;
            newPhase.Zeit = phase.Zeit;
            newContext.Phasen.Add(newPhase);
          }
        }
        //newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Phasen] ON");
        newContext.SaveChanges();
        //newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Phasen] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var raum in oldContext.Räume)
        {
          var newRaum = new RaumNeu();
          newRaum.Bezeichnung = raum.Bezeichnung;
          newRaum.Id = raum.Id;
          newContext.Räume.Add(newRaum);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Räume] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Räume] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var raumplan in oldContext.Raumpläne)
        {
          var newRaumplan = new RaumplanNeu();
          newRaumplan.Bezeichnung = raumplan.Bezeichnung;
          newRaumplan.Grundriss = raumplan.Grundriss;
          newRaumplan.Id = raumplan.Id;
          newRaumplan.RaumId = raumplan.RaumId;
          newContext.Raumpläne.Add(newRaumplan);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Raumpläne] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Raumpläne] OFF");

        transaction.Commit();
      }


      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var sitzplan in oldContext.Sitzpläne)
        {
          var newSitzplan = new SitzplanNeu();
          newSitzplan.Id = sitzplan.Id;
          newSitzplan.Bezeichnung = sitzplan.Bezeichnung;
          newSitzplan.GültigAb = sitzplan.GültigAb;
          newSitzplan.Lerngruppe = newContext.Lerngruppen.First(o =>
            o.SchuljahrId == sitzplan.Schülerliste.JahrtypId
            && o.FachId == sitzplan.Schülerliste.FachId
            && o.Bezeichnung == sitzplan.Schülerliste.Klasse.Bezeichnung);
          newSitzplan.RaumplanId = sitzplan.RaumplanId;
          newContext.Sitzpläne.Add(newSitzplan);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Sitzpläne] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Sitzpläne] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var sitzplatz in oldContext.Sitzplätze)
        {
          var newSitzplatz = new SitzplatzNeu();
          newSitzplatz.Id = sitzplatz.Id;
          newSitzplatz.Breite = sitzplatz.Breite;
          newSitzplatz.Drehwinkel = sitzplatz.Drehwinkel;
          newSitzplatz.Höhe = sitzplatz.Höhe;
          newSitzplatz.LinksObenX = sitzplatz.LinksObenX;
          newSitzplatz.LinksObenY = sitzplatz.LinksObenY;
          newSitzplatz.RaumplanId = sitzplatz.RaumplanId;
          newContext.Sitzplätze.Add(newSitzplatz);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Sitzplätze] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Sitzplätze] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var sitzplaneintrag in oldContext.Sitzplaneinträge)
        {
          var newSitzplaneintrag = new SitzplaneintragNeu();
          newSitzplaneintrag.Id = sitzplaneintrag.Id;
          newSitzplaneintrag.SchülereintragId = sitzplaneintrag.SchülereintragId;
          newSitzplaneintrag.SitzplanId = sitzplaneintrag.SitzplanId;
          newSitzplaneintrag.SitzplatzId = sitzplaneintrag.SitzplatzId;
          newContext.Sitzplaneinträge.Add(newSitzplaneintrag);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Sitzplaneinträge] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Sitzplaneinträge] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var stundenplan in oldContext.Stundenpläne)
        {
          var newStundenplan = new StundenplanNeu();
          newStundenplan.Id = stundenplan.Id;
          newStundenplan.Bezeichnung = stundenplan.Bezeichnung;
          newStundenplan.GültigAb = stundenplan.GültigAb;
          newStundenplan.Halbjahr = stundenplan.Halbjahrtyp.HalbjahrIndex == 1 ? Halbjahr.Winter : Halbjahr.Sommer;
          newStundenplan.SchuljahrID = stundenplan.JahrtypId;
          newContext.Stundenpläne.Add(newStundenplan);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Stundenpläne] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Stundenpläne] OFF");

        transaction.Commit();
      }

      using (var transaction = newContext.Database.BeginTransaction())
      {

        foreach (var stundenplaneintrag in oldContext.Stundenplaneinträge)
        {
          var newStundenplaneintrag = new StundenplaneintragNeu();
          newStundenplaneintrag.Id = stundenplaneintrag.Id;
          newStundenplaneintrag.ErsteUnterrichtsstundeIndex = stundenplaneintrag.ErsteUnterrichtsstundeIndex;
          newStundenplaneintrag.Lerngruppe = newContext.Lerngruppen.First(o =>
            o.SchuljahrId == stundenplaneintrag.Stundenplan.JahrtypId
            && o.FachId == stundenplaneintrag.FachId
            && o.Bezeichnung == stundenplaneintrag.Klasse.Bezeichnung);
          newStundenplaneintrag.LetzteUnterrichtsstundeIndex = stundenplaneintrag.LetzteUnterrichtsstundeIndex;
          newStundenplaneintrag.RaumId = stundenplaneintrag.RaumId;
          newStundenplaneintrag.StundenplanId = stundenplaneintrag.StundenplanId;
          newStundenplaneintrag.WochentagIndex = stundenplaneintrag.WochentagIndex;
          newContext.Stundenplaneinträge.Add(newStundenplaneintrag);
        }
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Stundenplaneinträge] ON");
        newContext.SaveChanges();
        newContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Stundenplaneinträge] OFF");

        transaction.Commit();
      }
      newContext.Configuration.AutoDetectChangesEnabled = false;

    }

    public void LoadLerngruppen()
    {
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
      var collection = this.lerngruppen;
      this.lerngruppen.CollectionChanged -= this.LerngruppenCollectionChanged;

      foreach (var schülerliste in App.UnitOfWork.Context.Lerngruppen.Where(o => o.Schuljahr.Jahr == Selection.Instance.Schuljahr.SchuljahrJahr))
      {
        if (!collection.Any(o => o.Model.Id == schülerliste.Id))
        {
          var vm = new LerngruppeViewModel(schülerliste);
          collection.Add(vm);
        }
      }

      this.lerngruppen.CollectionChanged += this.LerngruppenCollectionChanged;

      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
    }

    public LerngruppeViewModel LoadLerngruppe(LerngruppeNeu lg)
    {
      var collection = this.lerngruppen;

      LerngruppeViewModel vm = collection.FirstOrDefault(o => o.Model.Id == lg.Id);

      if (vm == null)
      {
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
        this.lerngruppen.CollectionChanged -= this.LerngruppenCollectionChanged;
        vm = new LerngruppeViewModel(lg);
        collection.Add(vm);
        this.lerngruppen.CollectionChanged += this.LerngruppenCollectionChanged;
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
      }


      return vm;
    }

    public void LoadArbeiten()
    {
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
      var collection = this.arbeiten;
      this.arbeiten.CollectionChanged -= this.ArbeitenCollectionChanged;
      foreach (var arbeit in App.UnitOfWork.Context.Arbeiten.Where(o => o.Lerngruppe.Schuljahr.Jahr == Selection.Instance.Schuljahr.SchuljahrJahr))
      {
        if (!collection.Any(o => o.Model.Id == arbeit.Id))
        {
          collection.Add(new ArbeitViewModel(arbeit));
        }
      }
      this.arbeiten.CollectionChanged += this.ArbeitenCollectionChanged;
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
    }

    /// <summary>
    /// Lädt alle Räume ins View Model
    /// </summary>
    public void LoadRäume()
    {
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
      var collection = this.räume;
      this.räume.CollectionChanged -= this.RäumeCollectionChanged;
      foreach (var raum in App.UnitOfWork.Context.Räume.OrderBy(o => o.Bezeichnung))
      {
        if (!collection.Any(o => o.Model.Bezeichnung == raum.Bezeichnung))
        {
          collection.Add(new RaumViewModel(raum));
        }
      }
      this.räume.CollectionChanged += this.RäumeCollectionChanged;
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
    }

    /// <summary>
    /// Lädt alle Curricula ins View Model
    /// </summary>
    public void LoadCurricula()
    {
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
      var collection = this.curricula;
      this.curricula.CollectionChanged -= this.CurriculaCollectionChanged;
      foreach (var curriculum in App.UnitOfWork.Context.Curricula)
      {
        if (!collection.Any(o => o.Model.Id == curriculum.Id))
        {
          collection.Add(new CurriculumViewModel(curriculum));
        }
      }
      this.curricula.CollectionChanged += this.CurriculaCollectionChanged;
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
    }

    ///// <summary>
    ///// Lädt die Stunden ins View Model
    ///// </summary>
    //public void LoadStunden(IEnumerable<StundeNeu> stundenZumHinzufügen)
    //{
    //  App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
    //  var collection = this.stunden;
    //  this.stunden.CollectionChanged -= this.StundenCollectionChanged;
    //  foreach (var stunde in stundenZumHinzufügen)
    //  {
    //    if (!collection.Any(o => o.Model.Id == stunde.Id))
    //    {
    //      collection.Add(new StundeViewModel(stunde));
    //    }
    //  }
    //  this.stunden.CollectionChanged += this.StundenCollectionChanged;
    //  App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
    //}

    public void LoadSitzpläne()
    {
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
      var collection = this.sitzpläne;
      this.sitzpläne.CollectionChanged -= this.SitzpläneCollectionChanged;

      foreach (var sitzplan in App.UnitOfWork.Context.Sitzpläne.Where(o => o.Lerngruppe.Schuljahr.Jahr == Selection.Instance.Schuljahr.SchuljahrJahr))
      {
        if (!collection.Any(o => o.Model.Id == sitzplan.Id))
        {
          collection.Add(new SitzplanViewModel(sitzplan));
        }
      }
      this.sitzpläne.CollectionChanged += this.SitzpläneCollectionChanged;

      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
    }

    /// <summary>
    /// Diese Methode aktualisiert den Datenbankkontext und speichert die Änderungen
    /// in der Datenbank.
    /// </summary>
    private void SaveChanges()
    {
      App.UnitOfWork.SaveChanges();
    }

    /// <summary>
    /// Refresh the UI when the undo / redo stacks change.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="arguments">Empty event args</param>
    private void RefreshUndoStackList(object sender, EventArgs arguments)
    {
      // Calling refresh on the CollectionView will tell the UI to rebind the list.
      // If the list were an ObservableCollection, or implemented INotifyCollectionChanged, this would not be needed.
      var cv = CollectionViewSource.GetDefaultView(this.UndoStack);
      cv.Refresh();

      cv = CollectionViewSource.GetDefaultView(this.RedoStack);
      cv.Refresh();

      this.UndoCommand.RaiseCanExecuteChanged();
      this.RedoCommand.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Display a new options dialog.
    /// </summary>
    private void ShowOptions()
    {
      new OptionsDialog().ShowDialog();
    }

    /// <summary>
    /// This method creates a directory tree in the my documents folder
    /// with classes and moduls.
    /// </summary>
    private void CreateDirectoryTreeForDocuments()
    {
      var basePath = Configuration.GetMyDocumentsPath();

      foreach (var modulViewModel in this.Module)
      {
        var fachPath = Path.Combine(basePath, modulViewModel.ModulFach.FachBezeichnung);
        if (!Directory.Exists(fachPath))
        {
          Directory.CreateDirectory(fachPath);
        }

        var jahrgangsstufe = modulViewModel.ModulJahrgang;
        var jahrgangsPath = Path.Combine(fachPath, jahrgangsstufe.ToString());
        if (!Directory.Exists(jahrgangsPath))
        {
          Directory.CreateDirectory(jahrgangsPath);
        }

        var modulPath = Path.Combine(jahrgangsPath, modulViewModel.ModulBezeichnung);
        if (!Directory.Exists(modulPath))
        {
          Directory.CreateDirectory(modulPath);
        }
      }
    }

    /// <summary>
    /// Wird aufgerufen, wenn der noten timer abgelaufen ist.
    /// Checkt, ob Noten eingegeben werden müssen.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
    private void NotenTimerElapsed(object source, ElapsedEventArgs e)
    {
      var nochZuBenotendeStunden = this.HoleNochZuBenotendeStunden();
      var anzahlNichtbenoteterStunden = nochZuBenotendeStunden.Count;
      if (anzahlNichtbenoteterStunden > 0)
      {
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
          App.NotenErinnerungsIcon.ToolTipText = anzahlNichtbenoteterStunden + " Stunden noch nicht benotet";
          App.NotenErinnerungsIcon.IconSource = this.errorIcon;
          App.NotenErinnerungsIcon.Visibility = Visibility.Visible;
        }));
      }
      else
      {
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
          {
            App.NotenErinnerungsIcon.ToolTipText = "Keine offenen Bewertungen.";
            App.NotenErinnerungsIcon.IconSource = this.inactiveIcon;
            App.NotenErinnerungsIcon.Visibility = Visibility.Collapsed;
          }));
      }
    }

    /// <summary>
    /// Gibt eine Collection mit den noch zu benotenden Stunden aus.
    /// </summary>
    /// <returns>ObservableCollection&lt;StundeViewModel&gt;.</returns>
    private ObservableCollection<StundeNeu> HoleNochZuBenotendeStunden()
    {
      var von = DateTime.Now.AddDays(-14);
      var bis = DateTime.Now;
      var nichtBenoteteStundenderLetzten14Tage =
        App.UnitOfWork.Context.Termine.OfType<StundeNeu>().Where(
          o =>
          o.Datum > von && o.Datum <= bis && !o.IstBenotet
          && (o.Fach.Bezeichnung == "Mathematik" || o.Fach.Bezeichnung == "Physik"));

      var nochZuBenotendeStunden = new ObservableCollection<StundeNeu>();


      foreach (var stundeViewModel in nichtBenoteteStundenderLetzten14Tage)
      {
        if (stundeViewModel.Datum.Date == bis.Date)
        {
          if (stundeViewModel.LetzteUnterrichtsstunde.Beginn > bis.TimeOfDay)
          {
            continue;
          }
        }

        nochZuBenotendeStunden.Add(stundeViewModel);
      }
      return nochZuBenotendeStunden;
    }

    #region CollectionChangedEventHandler

    /// <summary>
    /// Tritt auf, wenn die SchuljahreCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SchuljahreCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Schuljahre", this.Schuljahre, e, "Änderung der Schuljahre");
    }

    /// <summary>
    /// Tritt auf, wenn die DateitypenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void DateitypenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Dateitypen", this.Dateitypen, e, "Änderung der Dateitypen");
    }

    /// <summary>
    /// Tritt auf, wenn die UnterrichtsstundenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void UnterrichtsstundenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Unterrichtsstunden", this.Unterrichtsstunden, e, "Änderung der Unterrichtsstunden");
    }

    /// <summary>
    /// Tritt auf, wenn die FächerCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void FächerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Fächer", this.Fächer, e, "Änderung der Fächer");
    }

    /// <summary>
    /// Tritt auf, wenn die ModuleCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void ModuleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Module", this.Module, e, "Änderung der Module");
    }

    ///// <summary>
    ///// Tritt auf, wenn die ReihenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void ReihenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Reihen", this.Reihen, e, "Änderung der Reihen");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die SequenzenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void SequenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Sequenzen", this.Sequenzen, e, "Änderung der Sequenzen");
    //}

    /// <summary>
    /// Tritt auf, wenn die FerienCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void FerienCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Ferien", this.Ferien, e, "Änderung der Ferien");
    }

    /// <summary>
    /// Tritt auf, wenn die FachstundenanzahlCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void FachstundenanzahlCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Fachstundenanzahl", this.Fachstundenanzahl, e, "Änderung der Fachstundenanzahl");
    }

    /// <summary>
    /// Tritt auf, wenn die ZensurenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void ZensurenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Zensuren", this.Zensuren, e, "Änderung der Zensuren");
    }

    /// <summary>
    /// Tritt auf, wenn die NotenWichtungenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void NotenWichtungenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "NotenWichtungen", this.NotenWichtungen, e, "Änderung der NotenWichtungen");
    }

    /// <summary>
    /// Tritt auf, wenn die ArbeitenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void ArbeitenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Arbeiten", this.arbeiten, e, "Änderung der Arbeiten");
    }

    ///// <summary>
    ///// Tritt auf, wenn die AufgabenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void AufgabenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Aufgaben", this.Aufgaben, e, "Änderung der Aufgaben");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die ErgebnisseCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void ErgebnisseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Ergebnisse", this.Ergebnisse, e, "Änderung der Ergebnisse");
    //}

    /// <summary>
    /// Tritt auf, wenn die SchultermineCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SchultermineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Schultermine", this.Schultermine, e, "Änderung der Schultermine");
    }

    ///// <summary>
    ///// Tritt auf, wenn die StundenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void StundenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Stunden", this.Stunden, e, "Änderung der Stunden");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die LerngruppentermineCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void LerngruppentermineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  //this.UndoableCollectionChanged(this, "Lerngruppentermine", this.Lerngruppentermine, e, "Änderung der Lerngruppentermine");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die BetroffeneKlassenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void BetroffeneKlassenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "BetroffeneKlassen", this.BetroffeneKlassen, e, "Änderung der BetroffeneKlassen");
    //}

    /// <summary>
    /// Tritt auf, wenn die PersonenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void PersonenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Personen", this.Personen, e, "Änderung der Personen");
    }

    /// <summary>
    /// Tritt auf, wenn die LerngruppenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void LerngruppenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Lerngruppen", this.Lerngruppen, e, "Änderung der Lerngruppen");
    }

    ///// <summary>
    ///// Tritt auf, wenn die SchülereinträgeCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void SchülereinträgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Schülereinträge", this.Schülereinträge, e, "Änderung der Schülereinträge");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die NotenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void NotenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Noten", this.Noten, e, "Änderung der Noten");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die NotentendenzenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void NotentendenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Notentendenzen", this.Notentendenzen, e, "Änderung der Notentendenzen");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die HausaufgabenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void HausaufgabenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Hausaufgaben", this.Hausaufgaben, e, "Änderung der Hausaufgaben");
    //}

    /// <summary>
    /// Tritt auf, wenn die BewertungsschemataCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void BewertungsschemataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Bewertungsschemata", this.Bewertungsschemata, e, "Änderung der Bewertungsschemata");
    }

    /// <summary>
    /// Tritt auf, wenn die ProzentbereicheCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void ProzentbereicheCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Prozentbereiche", this.Prozentbereiche, e, "Änderung der Prozentbereiche");
    }

    /// <summary>
    /// Tritt auf, wenn die RäumeCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void RäumeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Räume", this.räume, e, "Änderung der Räume");
    }

    ///// <summary>
    ///// Tritt auf, wenn die RaumpläneCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void RaumpläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Raumpläne", this.Raumpläne, e, "Änderung der Raumpläne");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die SitzplätzeCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void SitzplätzeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Sitzplätze", this.Sitzplätze, e, "Änderung der Sitzplätze");
    //}

    /// <summary>
    /// Tritt auf, wenn die SitzpläneCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SitzpläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Sitzpläne", this.sitzpläne, e, "Änderung der Sitzpläne");
    }

    ///// <summary>
    ///// Tritt auf, wenn die SitzplaneinträgeCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void SitzplaneinträgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Sitzplaneinträge", this.Sitzplaneinträge, e, "Änderung der Sitzplaneinträge");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die StundenentwürfeCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void StundenentwürfeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Stundenentwürfe", this.stunden, e, "Änderung der Stundenentwürfe");
    //}

    /// <summary>
    /// Tritt auf, wenn die CurriculaCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void CurriculaCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Curricula", this.curricula, e, "Änderung der Curricula");
    }

    ///// <summary>
    ///// Tritt auf, wenn die HalbjahrespläneCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name = "sender" > Die auslösende Collection</param>
    ///// <param name = "e" > Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void HalbjahrespläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Halbjahrespläne", this.Halbjahrespläne, e, "Änderung der Halbjahrespläne");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die MonatspläneCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name = "sender" > Die auslösende Collection</param>
    ///// <param name = "e" > Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void MonatspläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Monatspläne", this.Monatspläne, e, "Änderung der Monatspläne");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die TagespläneCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name = "sender" > Die auslösende Collection</param>
    ///// <param name = "e" > Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void TagespläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Tagespläne", this.Tagespläne, e, "Änderung der Tagespläne");
    //}

    /// <summary>
    /// Tritt auf, wenn die StundenpläneCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void StundenpläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Stundenpläne", this.Stundenpläne, e, "Änderung der Stundenpläne");
    }

    ///// <summary>
    ///// Tritt auf, wenn die StundenplaneinträgeCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void StundenplaneinträgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "Stundenplaneinträge", this.Stundenplaneinträge, e, "Änderung der Stundenplaneinträge");
    //}

    #endregion
  }
}