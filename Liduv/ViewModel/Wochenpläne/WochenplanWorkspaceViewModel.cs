namespace Liduv.ViewModel.Wochenpläne
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Globalization;
  using System.Linq;
  using System.Windows.Controls;

  using Liduv.ExceptionHandling;
  using Liduv.Model.EntityFramework;
  using Liduv.UndoRedo;
  using Liduv.View.Termine;
  using Liduv.View.Wochenpläne;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Jahrespläne;
  using Liduv.ViewModel.Termine;

  /// <summary>
  /// ViewModel of an individual wochenplan
  /// </summary>
  public class WochenplanWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// Das Datum des Montags für diesen Wochenplan.
    /// </summary>
    private DateTime wochenplanMontag;

    /// <summary>
    /// The wochenplaneintraf currently selected
    /// </summary>
    private LerngruppenterminViewModel currentWochenplaneintrag;

    /// <summary>
    /// The <see cref="ContextMenu"/> for each wochenplan grid.
    /// </summary>
    private ContextMenu wochenplanContextMenu;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="WochenplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public WochenplanWorkspaceViewModel()
    {
      this.AddVertretungsstundeCommand = new DelegateCommand(this.AddVertretungsstunde);
      this.AddBesprechungCommand = new DelegateCommand(this.AddBesprechung);
      this.AddSonderterminCommand = new DelegateCommand(this.AddSondertermin);
      this.NextWeekCommand = new DelegateCommand(this.NextWeek);
      this.PreviousWeekCommand = new DelegateCommand(this.PreviousWeek);

      // Build data structures
      this.Wochenplaneinträge = new ObservableCollection<WochenplanEintrag>();
      this.wochenplanMontag = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

      // On saturday oder sunday show next week (to be prepared :-) )
      if ((int)DateTime.Today.DayOfWeek > 5)
      {
        this.wochenplanMontag = this.wochenplanMontag.AddDays(7);
      }

      this.PopulateWochenplanWithStundenFromCurrentWeek();
      this.Wochenplaneinträge.CollectionChanged += this.WochenplaneinträgeCollectionChanged;

      this.CreateContextMenu();
    }

    /// <summary>
    /// Holt den Befehl zur showing the next week in this wochenplan
    /// </summary>
    public DelegateCommand NextWeekCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur showing the previous week in this wochenplan
    /// </summary>
    public DelegateCommand PreviousWeekCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Stundenentwurf
    /// </summary>
    public DelegateCommand AddVertretungsstundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Stundenentwurf
    /// </summary>
    public DelegateCommand AddBesprechungCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Stundenentwurf
    /// </summary>
    public DelegateCommand AddSonderterminCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die wochenplan montag.
    /// </summary>
    public DateTime WochenplanMontag
    {
      get
      {
        return this.wochenplanMontag;
      }

      set
      {
        this.wochenplanMontag = value;
        this.RaisePropertyChanged("WochenplanMontag");
        this.RaisePropertyChanged("WochenplanÜberschrift");
        this.PopulateWochenplanWithStundenFromCurrentWeek();
      }
    }

    /// <summary>
    /// Holt das context menu for the entries of this Wochenplan
    /// </summary>
    public ContextMenu WochenplanContextMenu
    {
      get
      {
        return this.wochenplanContextMenu;
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected Wochenplaneintrag
    /// </summary>
    public LerngruppenterminViewModel CurrentWochenplaneintrag
    {
      get
      {
        return this.currentWochenplaneintrag;
      }

      set
      {
        this.currentWochenplaneintrag = value;
        this.RaisePropertyChanged("CurrentWochenplaneintrag");
      }
    }

    /// <summary>
    /// Holt die Überschrift über den Wochenplan
    /// </summary>
    public string WochenplanÜberschrift
    {
      get
      {
        // Holt den Calendar instance associated with a CultureInfo.
        var deCulture = new CultureInfo("de-DE");
        var deCalendar = deCulture.Calendar;

        // Holt den DTFI properties required by GetWeekOfYear.
        var weekRule = deCulture.DateTimeFormat.CalendarWeekRule;
        var firstDayOfWeek = deCulture.DateTimeFormat.FirstDayOfWeek;

        // Displays the number of the current week relative to the beginning of the year.
        var kalenderwoche = deCalendar.GetWeekOfYear(this.wochenplanMontag, weekRule, firstDayOfWeek);
        var überschrift = kalenderwoche + ". Woche, " + this.wochenplanMontag.ToString(@"dd.MM") + " - "
                          + this.wochenplanMontag.AddDays(5).ToString(@"dd.MM");

        // check if it is the current week
        var aktuelleKalenderwoche = deCalendar.GetWeekOfYear(DateTime.Today, weekRule, firstDayOfWeek);
        if (aktuelleKalenderwoche == kalenderwoche)
        {
          überschrift += "   > Aktuelle Woche";
        }

        return überschrift;
      }
    }

    /// <summary>
    /// Holt die Bezeichnung der ersten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan1Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[0].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der ersten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan1Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[0].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der zweiten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan2Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[1].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der zweiten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan2Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[1].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der dritten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan3Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[2].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der dritten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan3Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[2].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der vierten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan4Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[3].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der vierten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan4Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[3].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der fünften Unterrichtsstunde.
    /// </summary>
    public string Wochenplan5Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[4].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der fünften Unterrichtsstunde.
    /// </summary>
    public string Wochenplan5Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[4].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der sechsten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan6Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[5].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der sechsten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan6Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[5].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der siebten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan7Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[6].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der siebten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan7Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[6].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der achten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan8Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[7].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der achten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan8Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[7].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der neunten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan9Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[8].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der neunten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan9Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[8].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der zehnten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan10Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[9].UnterrichtsstundeBezeichnung; }
    }

    /// <summary>
    /// Holt den Zeitraum der zehnten Unterrichtsstunde.
    /// </summary>
    public string Wochenplan10Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[9].UnterrichtsstundeZeitraum; }
    }

    #region Ganztagstermine

    /// <summary>
    /// Holt die Ganztagstermine für Montag.
    /// </summary>
    public GanztagstermineViewModel Wochenplan10
    {
      get { return this.GetGanztagstermine(1); }
    }

    /// <summary>
    /// Holt die Ganztagstermine für Dienstag.
    /// </summary>
    public GanztagstermineViewModel Wochenplan20
    {
      get { return this.GetGanztagstermine(2); }
    }

    /// <summary>
    /// Holt die Ganztagstermine für MIttwoch.
    /// </summary>
    public GanztagstermineViewModel Wochenplan30
    {
      get { return this.GetGanztagstermine(3); }
    }

    /// <summary>
    /// Holt die Ganztagstermine für Donnerstag.
    /// </summary>
    public GanztagstermineViewModel Wochenplan40
    {
      get { return this.GetGanztagstermine(4); }
    }

    /// <summary>
    /// Holt die Ganztagstermine für Freitag.
    /// </summary>
    public GanztagstermineViewModel Wochenplan50
    {
      get { return this.GetGanztagstermine(5); }
    }

    #endregion

    #region Montag

    public WochenplanEintragCollection Wochenplan11
    {
      get { return this.GetWochenplanEintragCollection(1, 1); }
    }

    public int Wochenplan11Span
    {
      get { return this.Stundenanzahl(1, 1); }
    }

    public WochenplanEintragCollection Wochenplan12
    {
      get { return this.GetWochenplanEintragCollection(1, 2); }
    }

    public int Wochenplan12Span
    {
      get { return this.Stundenanzahl(1, 2); }
    }

    public WochenplanEintragCollection Wochenplan13
    {
      get { return this.GetWochenplanEintragCollection(1, 3); }
    }

    public int Wochenplan13Span
    {
      get { return this.Stundenanzahl(1, 3); }
    }

    public WochenplanEintragCollection Wochenplan14
    {
      get { return this.GetWochenplanEintragCollection(1, 4); }
    }

    public int Wochenplan14Span
    {
      get { return this.Stundenanzahl(1, 4); }
    }

    public WochenplanEintragCollection Wochenplan15
    {
      get { return this.GetWochenplanEintragCollection(1, 5); }
    }

    public int Wochenplan15Span
    {
      get { return this.Stundenanzahl(1, 5); }
    }

    public WochenplanEintragCollection Wochenplan16
    {
      get { return this.GetWochenplanEintragCollection(1, 6); }
    }

    public int Wochenplan16Span
    {
      get { return this.Stundenanzahl(1, 6); }
    }

    public WochenplanEintragCollection Wochenplan17
    {
      get { return this.GetWochenplanEintragCollection(1, 7); }
    }

    public int Wochenplan17Span
    {
      get { return this.Stundenanzahl(1, 7); }
    }

    public WochenplanEintragCollection Wochenplan18
    {
      get { return this.GetWochenplanEintragCollection(1, 8); }
    }

    public int Wochenplan18Span
    {
      get { return this.Stundenanzahl(1, 8); }
    }

    public WochenplanEintragCollection Wochenplan19
    {
      get { return this.GetWochenplanEintragCollection(1, 9); }
    }

    public int Wochenplan19Span
    {
      get { return this.Stundenanzahl(1, 9); }
    }

    public WochenplanEintragCollection Wochenplan110
    {
      get { return this.GetWochenplanEintragCollection(1, 10); }
    }

    public int Wochenplan110Span
    {
      get { return this.Stundenanzahl(1, 10); }
    }

    #endregion

    #region Dienstag

    public WochenplanEintragCollection Wochenplan21
    {
      get { return this.GetWochenplanEintragCollection(2, 1); }
    }

    public int Wochenplan21Span
    {
      get { return this.Stundenanzahl(2, 1); }
    }

    public WochenplanEintragCollection Wochenplan22
    {
      get { return this.GetWochenplanEintragCollection(2, 2); }
    }

    public int Wochenplan22Span
    {
      get { return this.Stundenanzahl(2, 2); }
    }

    public WochenplanEintragCollection Wochenplan23
    {
      get { return this.GetWochenplanEintragCollection(2, 3); }
    }

    public int Wochenplan23Span
    {
      get { return this.Stundenanzahl(2, 3); }
    }

    public WochenplanEintragCollection Wochenplan24
    {
      get { return this.GetWochenplanEintragCollection(2, 4); }
    }

    public int Wochenplan24Span
    {
      get { return this.Stundenanzahl(2, 4); }
    }

    public WochenplanEintragCollection Wochenplan25
    {
      get { return this.GetWochenplanEintragCollection(2, 5); }
    }

    public int Wochenplan25Span
    {
      get { return this.Stundenanzahl(2, 5); }
    }

    public WochenplanEintragCollection Wochenplan26
    {
      get { return this.GetWochenplanEintragCollection(2, 6); }
    }

    public int Wochenplan26Span
    {
      get { return this.Stundenanzahl(2, 6); }
    }

    public WochenplanEintragCollection Wochenplan27
    {
      get { return this.GetWochenplanEintragCollection(2, 7); }
    }

    public int Wochenplan27Span
    {
      get { return this.Stundenanzahl(2, 7); }
    }

    public WochenplanEintragCollection Wochenplan28
    {
      get { return this.GetWochenplanEintragCollection(2, 8); }
    }

    public int Wochenplan28Span
    {
      get { return this.Stundenanzahl(2, 8); }
    }

    public WochenplanEintragCollection Wochenplan29
    {
      get { return this.GetWochenplanEintragCollection(2, 9); }
    }

    public int Wochenplan29Span
    {
      get { return this.Stundenanzahl(2, 9); }
    }

    public WochenplanEintragCollection Wochenplan210
    {
      get { return this.GetWochenplanEintragCollection(2, 10); }
    }

    public int Wochenplan210Span
    {
      get { return this.Stundenanzahl(2, 10); }
    }

    #endregion

    #region Mittwoch

    public WochenplanEintragCollection Wochenplan31
    {
      get { return this.GetWochenplanEintragCollection(3, 1); }
    }

    public int Wochenplan31Span
    {
      get { return this.Stundenanzahl(3, 1); }
    }

    public WochenplanEintragCollection Wochenplan32
    {
      get { return this.GetWochenplanEintragCollection(3, 2); }
    }

    public int Wochenplan32Span
    {
      get { return this.Stundenanzahl(3, 2); }
    }

    public WochenplanEintragCollection Wochenplan33
    {
      get { return this.GetWochenplanEintragCollection(3, 3); }
    }

    public int Wochenplan33Span
    {
      get { return this.Stundenanzahl(3, 3); }
    }

    public WochenplanEintragCollection Wochenplan34
    {
      get { return this.GetWochenplanEintragCollection(3, 4); }
    }

    public int Wochenplan34Span
    {
      get { return this.Stundenanzahl(3, 4); }
    }

    public WochenplanEintragCollection Wochenplan35
    {
      get { return this.GetWochenplanEintragCollection(3, 5); }
    }

    public int Wochenplan35Span
    {
      get { return this.Stundenanzahl(3, 5); }
    }

    public WochenplanEintragCollection Wochenplan36
    {
      get { return this.GetWochenplanEintragCollection(3, 6); }
    }

    public int Wochenplan36Span
    {
      get { return this.Stundenanzahl(3, 6); }
    }

    public WochenplanEintragCollection Wochenplan37
    {
      get { return this.GetWochenplanEintragCollection(3, 7); }
    }

    public int Wochenplan37Span
    {
      get { return this.Stundenanzahl(3, 7); }
    }

    public WochenplanEintragCollection Wochenplan38
    {
      get { return this.GetWochenplanEintragCollection(3, 8); }
    }

    public int Wochenplan38Span
    {
      get { return this.Stundenanzahl(3, 8); }
    }

    public WochenplanEintragCollection Wochenplan39
    {
      get { return this.GetWochenplanEintragCollection(3, 9); }
    }

    public int Wochenplan39Span
    {
      get { return this.Stundenanzahl(3, 9); }
    }

    public WochenplanEintragCollection Wochenplan310
    {
      get { return this.GetWochenplanEintragCollection(3, 10); }
    }

    public int Wochenplan310Span
    {
      get { return this.Stundenanzahl(3, 10); }
    }

    #endregion

    #region Donnerstag

    public WochenplanEintragCollection Wochenplan41
    {
      get { return this.GetWochenplanEintragCollection(4, 1); }
    }

    public int Wochenplan41Span
    {
      get { return this.Stundenanzahl(4, 1); }
    }

    public WochenplanEintragCollection Wochenplan42
    {
      get { return this.GetWochenplanEintragCollection(4, 2); }
    }

    public int Wochenplan42Span
    {
      get { return this.Stundenanzahl(4, 2); }
    }

    public WochenplanEintragCollection Wochenplan43
    {
      get { return this.GetWochenplanEintragCollection(4, 3); }
    }

    public int Wochenplan43Span
    {
      get { return this.Stundenanzahl(4, 3); }
    }

    public WochenplanEintragCollection Wochenplan44
    {
      get { return this.GetWochenplanEintragCollection(4, 4); }
    }

    public int Wochenplan44Span
    {
      get { return this.Stundenanzahl(4, 4); }
    }

    public WochenplanEintragCollection Wochenplan45
    {
      get { return this.GetWochenplanEintragCollection(4, 5); }
    }

    public int Wochenplan45Span
    {
      get { return this.Stundenanzahl(4, 5); }
    }

    public WochenplanEintragCollection Wochenplan46
    {
      get { return this.GetWochenplanEintragCollection(4, 6); }
    }

    public int Wochenplan46Span
    {
      get { return this.Stundenanzahl(4, 6); }
    }

    public WochenplanEintragCollection Wochenplan47
    {
      get { return this.GetWochenplanEintragCollection(4, 7); }
    }

    public int Wochenplan47Span
    {
      get { return this.Stundenanzahl(4, 7); }
    }

    public WochenplanEintragCollection Wochenplan48
    {
      get { return this.GetWochenplanEintragCollection(4, 8); }
    }

    public int Wochenplan48Span
    {
      get { return this.Stundenanzahl(4, 8); }
    }

    public WochenplanEintragCollection Wochenplan49
    {
      get { return this.GetWochenplanEintragCollection(4, 9); }
    }

    public int Wochenplan49Span
    {
      get { return this.Stundenanzahl(4, 9); }
    }

    public WochenplanEintragCollection Wochenplan410
    {
      get { return this.GetWochenplanEintragCollection(4, 10); }
    }

    public int Wochenplan410Span
    {
      get { return this.Stundenanzahl(4, 10); }
    }

    #endregion

    #region Freitag

    public WochenplanEintragCollection Wochenplan51
    {
      get { return this.GetWochenplanEintragCollection(5, 1); }
    }

    public int Wochenplan51Span
    {
      get { return this.Stundenanzahl(5, 1); }
    }

    public WochenplanEintragCollection Wochenplan52
    {
      get { return this.GetWochenplanEintragCollection(5, 2); }
    }

    public int Wochenplan52Span
    {
      get { return this.Stundenanzahl(5, 2); }
    }

    public WochenplanEintragCollection Wochenplan53
    {
      get { return this.GetWochenplanEintragCollection(5, 3); }
    }

    public int Wochenplan53Span
    {
      get { return this.Stundenanzahl(5, 3); }
    }

    public WochenplanEintragCollection Wochenplan54
    {
      get { return this.GetWochenplanEintragCollection(5, 4); }
    }

    public int Wochenplan54Span
    {
      get { return this.Stundenanzahl(5, 4); }
    }

    public WochenplanEintragCollection Wochenplan55
    {
      get { return this.GetWochenplanEintragCollection(5, 5); }
    }

    public int Wochenplan55Span
    {
      get { return this.Stundenanzahl(5, 5); }
    }

    public WochenplanEintragCollection Wochenplan56
    {
      get { return this.GetWochenplanEintragCollection(5, 6); }
    }

    public int Wochenplan56Span
    {
      get { return this.Stundenanzahl(5, 6); }
    }

    public WochenplanEintragCollection Wochenplan57
    {
      get { return this.GetWochenplanEintragCollection(5, 7); }
    }

    public int Wochenplan57Span
    {
      get { return this.Stundenanzahl(5, 7); }
    }

    public WochenplanEintragCollection Wochenplan58
    {
      get { return this.GetWochenplanEintragCollection(5, 8); }
    }

    public int Wochenplan58Span
    {
      get { return this.Stundenanzahl(5, 8); }
    }

    public WochenplanEintragCollection Wochenplan59
    {
      get { return this.GetWochenplanEintragCollection(5, 9); }
    }

    public int Wochenplan59Span
    {
      get { return this.Stundenanzahl(5, 9); }
    }

    public WochenplanEintragCollection Wochenplan510
    {
      get { return this.GetWochenplanEintragCollection(5, 10); }
    }

    public int Wochenplan510Span
    {
      get { return this.Stundenanzahl(5, 10); }
    }

    #endregion

    /// <summary>
    /// Holt die WochenplanEinträge for this wochenplan
    /// </summary>
    public ObservableCollection<WochenplanEintrag> Wochenplaneinträge { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "WochenplanWorkspace: " + this.WochenplanÜberschrift;
    }

    /// <summary>
    /// Löscht den gegebenen Wochenplaneintrag
    /// </summary>
    /// <param name="wochenplanEintrag">Der Wochenplaneintrag zum Löschen</param>
    public void RemoveWochenplaneintrag(WochenplanEintrag wochenplanEintrag)
    {
      this.Wochenplaneinträge.RemoveTest(wochenplanEintrag);
    }

    /// <summary>
    /// Tritt auf, wenn die WochenplaneinträgeCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void WochenplaneinträgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      //this.UndoableCollectionChanged(this, "Wochenplaneinträge", this.Wochenplaneinträge, e, false, "Änderung der Wochenplaneinträge");
    }

    /// <summary>
    /// Shows next week.
    /// </summary>
    private void NextWeek()
    {
      this.WochenplanMontag = this.wochenplanMontag.AddDays(7);
    }

    /// <summary>
    /// Shows previous week.
    /// </summary>
    private void PreviousWeek()
    {
      this.WochenplanMontag = this.wochenplanMontag.AddDays(-7);
    }

    /// <summary>
    /// Fügt eine Vertretungssstunde hinzu
    /// </summary>
    private void AddVertretungsstunde()
    {
      var tagesplanToAdd = this.GetTagesplanToAddInVertretungsjahresplan();

      var stunde = new Stunde();
      stunde.ErsteUnterrichtsstunde =
        App.MainViewModel.Unterrichtsstunden.First(
          o => o.UnterrichtsstundeIndex == WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex).Model;
      stunde.LetzteUnterrichtsstunde =
        App.MainViewModel.Unterrichtsstunden.First(
          o => o.UnterrichtsstundeIndex == WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex).Model;
      stunde.Tagesplan = tagesplanToAdd.Model;
      stunde.Termintyp = App.MainViewModel.Termintypen.First(
          termintyp => termintyp.TermintypBezeichnung == "Vertretung").Model;

      var vm = new StundeViewModel(tagesplanToAdd, stunde);

      var stundeDlg = new AddStundeDialog(vm);
      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Vertretungsstunde {0} angelegt.", vm), false))
      {
        if (!(undo = !stundeDlg.ShowDialog().GetValueOrDefault(false)))
        {
          App.MainViewModel.Stunden.Add(vm);
          tagesplanToAdd.Lerngruppentermine.Add(vm);
          tagesplanToAdd.CurrentLerngruppentermin = vm;

          var wochenplanEintrag = new WochenplanEintrag(this, vm);
          if (!this.Wochenplaneinträge.Contains(wochenplanEintrag, new WochenplanEintragEqualityComparer()))
          {
            this.Wochenplaneinträge.Add(wochenplanEintrag);
          }

          this.UpdateProperties(
            vm.LerngruppenterminWochentagIndex,
            vm.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex,
            vm.TerminStundenanzahl);
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Fügt eine Besprechung hinzu
    /// </summary>
    private void AddBesprechung()
    {
      this.AddSpecialLerngruppentermin("Besprechung");
    }

    /// <summary>
    /// Fügt einen Sondertermin hinzu
    /// </summary>
    private void AddSondertermin()
    {
      this.AddSpecialLerngruppentermin("Sondertermin");
    }

    /// <summary>
    /// Fügt einen Sondertermin oder eine Besprechung ein (Lerngruppentermine)
    /// </summary>
    /// <param name="termintypBezeichnung">Die Bezeichnung des Termins.</param>
    private void AddSpecialLerngruppentermin(string termintypBezeichnung)
    {
      var dlg = new AddSonderterminDialog();
      dlg.TerminTermintyp = App.MainViewModel.Termintypen.First(o => o.TermintypBezeichnung == termintypBezeichnung);

      bool undo;
      if (!(undo = !dlg.ShowDialog().GetValueOrDefault(false)))
      {
        var tagesplanToAdd = this.GetTagesplanToAddInVertretungsjahresplan();

        var lerngruppentermin = new Lerngruppentermin();
        lerngruppentermin.Beschreibung = dlg.TerminBeschreibung;
        lerngruppentermin.ErsteUnterrichtsstunde =
          App.MainViewModel.Unterrichtsstunden.First(
            o => o.UnterrichtsstundeIndex == WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex).Model;
        lerngruppentermin.LetzteUnterrichtsstunde =
          App.MainViewModel.Unterrichtsstunden.First(
            o => o.UnterrichtsstundeIndex == WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex).Model;
        lerngruppentermin.Tagesplan = tagesplanToAdd.Model;
        lerngruppentermin.Termintyp =
          App.MainViewModel.Termintypen.First(termintyp => termintyp.TermintypBezeichnung == termintypBezeichnung)
             .Model;
        lerngruppentermin.Ort = dlg.TerminOrt;

        var vm = new LerngruppenterminViewModel(tagesplanToAdd, lerngruppentermin);
        using (new UndoBatch(App.MainViewModel, string.Format("Sondertermin {0} angelegt.", vm), false))
        {
          App.MainViewModel.Lerngruppentermine.Add(vm);
          tagesplanToAdd.Lerngruppentermine.Add(vm);
          tagesplanToAdd.CurrentLerngruppentermin = vm;

          var wochenplanEintrag = new WochenplanEintrag(this, vm);
          if (!this.Wochenplaneinträge.Contains(wochenplanEintrag, new WochenplanEintragEqualityComparer()))
          {
            this.Wochenplaneinträge.Add(wochenplanEintrag);
          }

          this.UpdateProperties(
            vm.LerngruppenterminWochentagIndex,
            vm.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex,
            vm.TerminStundenanzahl);
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Holt den Tagesplan aus dem Vertretungsjahresplan für die Sondertermine.
    /// </summary>
    /// <returns>Ein <see cref="TagesplanViewModel"/> mit dem Tagesplan.</returns>
    private TagesplanViewModel GetTagesplanToAddInVertretungsjahresplan()
    {
      // The day to add the stunde to
      var date = this.wochenplanMontag.AddDays(WochenplanSelection.Instance.WochentagIndex - 1);

      bool sommerHalbjahr;
      int jahresplanJahr;
      this.GetJahrAndHalbjahr(out sommerHalbjahr, out jahresplanJahr);

      var vertretungsjahresplan =
        App.MainViewModel.Jahrespläne.Single(
          o => o.JahresplanFach.FachBezeichnung == "Vertretungsstunden" && o.JahresplanJahrtyp.JahrtypJahr == jahresplanJahr);

      // Get correct Halbjahresplan
      var halbjahresplanViewModel = sommerHalbjahr
                              ? vertretungsjahresplan.CurrentJahresplanSommerhalbjahr
                              : vertretungsjahresplan.CurrentJahresplanWinterhalbjahr;

      // Wenn der Halbjahresplan noch fehlt, dann anlegen
      if (halbjahresplanViewModel == null)
      {
        if (sommerHalbjahr)
        {
          vertretungsjahresplan.AddSommerHalbjahresplan();
          halbjahresplanViewModel = vertretungsjahresplan.CurrentJahresplanSommerhalbjahr;
        }
        else
        {
          vertretungsjahresplan.AddWinterHalbjahresplan();
          halbjahresplanViewModel = vertretungsjahresplan.CurrentJahresplanWinterhalbjahr;
        }
      }

      // Get correct month
      var month = halbjahresplanViewModel.Monatspläne.Single(o => o.MonatsplanMonatindex == date.Month);

      // Get correct day
      var tagesplanToAdd = month.Tagespläne.Single(o => o.TagesplanDatum == date);
      return tagesplanToAdd;
    }

    /// <summary>
    /// Holt die Ganztagstermine für den gegebenen Wochentag.
    /// </summary>
    /// <param name="wochentagIndex">Der Index des Wochentags</param>
    /// <returns>Ein <see cref="GanztagstermineViewModel"/> mit allen Ganztagsterminen.</returns>
    private GanztagstermineViewModel GetGanztagstermine(int wochentagIndex)
    {
      // Get all entries which include the whole day
      var ganztagViewModels =
        this.Wochenplaneinträge.Where(eintrag => eintrag.WochentagIndex == wochentagIndex
            && eintrag.Stundenanzahl >= 7);

      // Create empty entry, if there is no stunde in one of the jahrespläne
      if (!ganztagViewModels.Any())
      {
        var emptyGanztagstermineViewModel = new GanztagstermineViewModel(null)
        {
          WochentagIndex = wochentagIndex
        };

        return emptyGanztagstermineViewModel;
      }

      var newGanztagsViewModel = new GanztagstermineViewModel(ganztagViewModels.ToList());
      return newGanztagsViewModel;
    }

    private WochenplanEintragCollection GetWochenplanEintragCollection(int wochentagIndex, int stundeIndex)
    {
      try
      {
        if (stundeIndex > 1)
        {
          var previousLerngruppenterminViewModel =
            this.Wochenplaneinträge.Where(
              eintrag => eintrag.WochentagIndex == wochentagIndex
                && eintrag.ErsteUnterrichtsstundeIndex == stundeIndex - 1
                && eintrag.Stundenanzahl < 7);

          if (previousLerngruppenterminViewModel.Any(o => o.Stundenanzahl > 1))
          {
            // Previous Wochenplaneintrag is spanning two rows
            // TODO: care for spanning more than one row
            return null;
          }
        }

        // Get all entries
        var wochenplanEinträge = this.Wochenplaneinträge.Where(
            eintrag =>
              eintrag.WochentagIndex == wochentagIndex
              && eintrag.ErsteUnterrichtsstundeIndex == stundeIndex && eintrag.Stundenanzahl < 7).ToList();

        if (!wochenplanEinträge.Any())
        {
          var emptyEintrag = new WochenplanEintrag(this, null)
                        {
                          ErsteUnterrichtsstundeIndex = stundeIndex,
                          LetzteUnterrichtsstundeIndex = stundeIndex,
                          WochentagIndex = wochentagIndex
                        };
          var emptyList = new List<WochenplanEintrag> { emptyEintrag };
          var emptyCollection = new WochenplanEintragCollection(wochentagIndex, emptyList);
          return emptyCollection;
        }

        for (var i = 0; i < wochenplanEinträge.Count(); i++)
        {
          wochenplanEinträge[i].ColumnIndex = i;
        }

        return new WochenplanEintragCollection(wochentagIndex, wochenplanEinträge.ToList());
      }
      catch (Exception)
      {
        InformationDialog.Show("Mehrere Wochenplaneinträge", "Mehrere parallele Wochenplaneinträge gefunden!", false);
      }

      return null;
    }

    private int Stundenanzahl(int wochentagIndex, int stundeIndex)
    {
      var wochenplaneintragViewModel =
        this.Wochenplaneinträge.Where(
          eintrag => eintrag.WochentagIndex == wochentagIndex
            && eintrag.ErsteUnterrichtsstundeIndex == stundeIndex
            && eintrag.Stundenanzahl < 7);

      var maxStundenzahl = 1;

      foreach (var wochenplanEintrag in wochenplaneintragViewModel)
      {
        if (wochenplanEintrag.Stundenanzahl > maxStundenzahl)
        {
          maxStundenzahl = wochenplanEintrag.Stundenanzahl;
        }
      }

      return maxStundenzahl;
    }

    /// <summary>
    /// Creates the <see cref="ContextMenu"/> for each wochenplan grid,
    /// </summary>
    private void CreateContextMenu()
    {
      this.wochenplanContextMenu = new ContextMenu();

      var addVertretungsstundeItem = new MenuItem
      {
        Header = "Vertretungsstunde anlegen",
        Command = this.AddVertretungsstundeCommand,
        Icon = App.GetImage("Vertretung16.png")
      };
      this.wochenplanContextMenu.Items.Add(addVertretungsstundeItem);

      var addBesprechungItem = new MenuItem
      {
        Header = "Besprechung anlegen",
        Command = this.AddBesprechungCommand,
        Icon = App.GetImage("Besprechung16.png")
      };
      this.wochenplanContextMenu.Items.Add(addBesprechungItem);

      var addSonderterminItem = new MenuItem
      {
        Header = "Sondertermin anlegen",
        Command = this.AddSonderterminCommand,
        Icon = App.GetImage("Sondertermin16.png")
      };
      this.wochenplanContextMenu.Items.Add(addSonderterminItem);
    }

    public void UpdateProperties(int wochentag, int ersteStunde, int stundenzahl)
    {
      var wochentagIndex = wochentag;
      var ersteUnterrichtsstundeIndex = ersteStunde;
      if (stundenzahl > 1)
      {
        ersteUnterrichtsstundeIndex++;
        string nextPropertyString = "Wochenplan" + wochentagIndex + ersteUnterrichtsstundeIndex.ToString("N0");
        this.RaisePropertyChanged(nextPropertyString);
      }

      string propertyString = "Wochenplan" + wochentagIndex + ersteStunde;
      string propertySpanString = "Wochenplan" + wochentagIndex
                                  + ersteStunde + "Span";
      this.RaisePropertyChanged(propertyString);
      this.RaisePropertyChanged(propertySpanString);
    }

    public void UpdateProperties(LerngruppenterminViewModel vm)
    {
      this.UpdateProperties(
        (int)vm.LerngruppenterminDatum.DayOfWeek,
        vm.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex,
        vm.TerminStundenanzahl);
    }

    /// <summary>
    /// Erstellt den Wochenplan aus den Stunden der Datenbank.
    /// </summary>
    private void PopulateWochenplanWithStundenFromCurrentWeek()
    {
      this.Wochenplaneinträge.Clear();
      //// use this instead of clear to enable undo
      //var count = this.Wochenplaneinträge.Count;
      //for (int i = 0; i < count; i++)
      //{
      //  this.Wochenplaneinträge.RemoveAt(this.Wochenplaneinträge.Count - 1);
      //}

      var comparer = new WochenplanEintragEqualityComparer();


      bool sommerHalbjahr;
      int jahresplanJahr;
      this.GetJahrAndHalbjahr(out sommerHalbjahr, out jahresplanJahr);

      // Get Einträge from Terminliste
      // Die Schultermine müssen zuerst eingelesen werden, da
      // sie sonst von den gleichen Lerngruppenterminen nicht zugelassen wurden
      var schultermine = App.MainViewModel.Schultermine.Where(o => o is SchulterminViewModel);
      var termineInWeek = schultermine.Where(o =>
        o.SchulterminDatum >= this.wochenplanMontag &&
        o.SchulterminDatum < this.wochenplanMontag.AddDays(6));

      foreach (var schulterminViewModel in termineInWeek)
      {
        var wochenplanEintrag = new WochenplanEintrag(this, schulterminViewModel);
        if (!this.Wochenplaneinträge.Contains(wochenplanEintrag, comparer))
        {
          this.Wochenplaneinträge.Add(wochenplanEintrag);
        }
      }

      // Get all jahrespläne for the selected week
      var jahrespläne =
        App.MainViewModel.Jahrespläne.Where(
          o => o.JahresplanJahrtyp.JahrtypJahr == jahresplanJahr);

      foreach (var jahresplanViewModel in jahrespläne)
      {
        // Get correct Halbjahresplan
        var halbjahrespläne = sommerHalbjahr
                                ? jahresplanViewModel.CurrentJahresplanSommerhalbjahr
                                : jahresplanViewModel.CurrentJahresplanWinterhalbjahr;

        // Get correct month
        if (halbjahrespläne == null)
        {
          continue;
        }

        var month = halbjahrespläne.Monatspläne.Single(o => o.MonatsplanMonatindex == this.wochenplanMontag.Month);

        // Get correct days
        var daysInWeek =
          month.Tagespläne.Where(
            o => o.TagesplanDatum >= this.wochenplanMontag && o.TagesplanDatum < this.wochenplanMontag.AddDays(6));

        foreach (var tagesplanViewModel in daysInWeek)
        {
          foreach (var lerngruppenterminViewModel in tagesplanViewModel.Lerngruppentermine)
          {
            var wochenplanEintrag = new WochenplanEintrag(this, lerngruppenterminViewModel);
            if (!this.Wochenplaneinträge.Contains(wochenplanEintrag, comparer))
            {
              this.Wochenplaneinträge.Add(wochenplanEintrag);
            }
          }
        }

        // check if this week is at the end of month
        // and add missing days if it is so
        if (this.wochenplanMontag.Day + 6 > DateTime.DaysInMonth(this.wochenplanMontag.Year, this.wochenplanMontag.Month))
        {
          if (halbjahrespläne.Monatspläne.All(o => o.MonatsplanMonatindex != this.wochenplanMontag.Month + 1))
          {
            continue;
          }

          // Get correct month
          var nextMonth = halbjahrespläne.Monatspläne.Single(o => o.MonatsplanMonatindex == this.wochenplanMontag.Month + 1);

          // Get correct days
          var missingDaysInWeek =
            nextMonth.Tagespläne.Where(o => o.TagesplanDatum < this.wochenplanMontag.AddDays(6));

          foreach (var tagesplanViewModel in missingDaysInWeek)
          {
            foreach (var lerngruppenterminViewModel in tagesplanViewModel.Lerngruppentermine)
            {
              var wochenplanEintrag = new WochenplanEintrag(this, lerngruppenterminViewModel);

              if (!this.Wochenplaneinträge.Contains(wochenplanEintrag, comparer))
              {
                this.Wochenplaneinträge.Add(wochenplanEintrag);
              }
            }
          }
        }
      }

      // Check for Ferien
      foreach (var ferien in App.MainViewModel.Ferien.Where(
        schuljahr => schuljahr.FerienJahrtyp.Model.Jahr == jahresplanJahr))
      {
        for (int i = 0; i < 5; i++)
        {
          var tag = this.wochenplanMontag.AddDays(i);
          if (tag >= ferien.FerienErsterFerientag && tag <= ferien.FerienLetzterFerientag)
          {
            var termin = new Schultermin();
            termin.Beschreibung = ferien.FerienBezeichnung;
            termin.ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[0].Model;
            termin.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[8].Model;
            termin.IstGeprüft = true;
            termin.Termintyp = App.MainViewModel.Termintypen.First(o => o.TermintypBezeichnung == "Ferien").Model;
            termin.Datum = tag;
            termin.Jahrtyp = App.MainViewModel.Jahrtypen.First(o => o.JahrtypJahr == jahresplanJahr).Model;

            var ferienTerminViewModel = new SchulterminViewModel(termin);
            var ferientagEintrag = new WochenplanEintrag(this, ferienTerminViewModel);

            if (!this.Wochenplaneinträge.Contains(ferientagEintrag, comparer))
            {
              this.Wochenplaneinträge.Add(ferientagEintrag);
            }
          }
        }
      }

      // Suche Geburtstage für Schüler der aktuellen Klassen
      var personenMitGeburtstag = App.MainViewModel.Personen.Where(
        o => o.PersonGeburtstag.HasValue
        && o.PersonGeburtstag.Value.DayOfYear >= this.wochenplanMontag.DayOfYear
        && o.PersonGeburtstag.Value.DayOfYear < this.wochenplanMontag.AddDays(6).DayOfYear);

      foreach (var person in personenMitGeburtstag)
      {
        var termin = new Schultermin();
        termin.ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[0].Model;
        termin.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[8].Model;
        termin.IstGeprüft = true;
        termin.Termintyp = App.MainViewModel.Termintypen.First(o => o.TermintypBezeichnung == "Geburtstag").Model;
        var geburstagInDerWoche = new DateTime(
          this.WochenplanMontag.Year,
          person.PersonGeburtstag.Value.Month,
          person.PersonGeburtstag.Value.Day);
        termin.Datum = geburstagInDerWoche;
        var alter = person.PersonAlter(this.wochenplanMontag.AddDays((int)geburstagInDerWoche.DayOfWeek));
        termin.Beschreibung = string.Format("{0} {1} ({2})", person.PersonVorname, person.PersonNachname, alter);
        termin.Jahrtyp = App.MainViewModel.Jahrtypen.First(o => o.JahrtypJahr == jahresplanJahr).Model;

        var geburtstagTerminViewModel = new SchulterminViewModel(termin);
        var geburtstagEintrag = new WochenplanEintrag(this, geburtstagTerminViewModel);

        if (!this.Wochenplaneinträge.Contains(geburtstagEintrag, comparer))
        {
          this.Wochenplaneinträge.Add(geburtstagEintrag);
        }
      }

      this.NotifyWochenplanChanges();
    }

    private void GetJahrAndHalbjahr(out bool sommerHalbjahr, out int jahresplanJahr)
    {
      // Get StundenFrom Jahresplänen
      if (this.wochenplanMontag.Month >= 8)
      {
        jahresplanJahr = this.wochenplanMontag.Year;
        sommerHalbjahr = false;
      }
      else
      {
        jahresplanJahr = this.wochenplanMontag.Year - 1;
        if (this.wochenplanMontag.Month == 1)
        {
          sommerHalbjahr = false;
        }
        else
        {
          sommerHalbjahr = true;
        }
      }
    }

    private void NotifyWochenplanChanges()
    {
      for (int i = 1; i < 6; i++)
      {
        string propertyString = "Wochenplan" + i + "0";
        this.RaisePropertyChanged(propertyString);
      }
      for (int i = 1; i < 6; i++)
      {
        for (int j = 1; j < 11; j++)
        {
          string propertyString = "Wochenplan" + i + j;
          string propertySpanString = "Wochenplan" + i + j + "Span";
          this.RaisePropertyChanged(propertyString);
          this.RaisePropertyChanged(propertySpanString);
        }
      }
    }
  }
}
