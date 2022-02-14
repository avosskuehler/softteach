namespace SoftTeach.ViewModel.Wochenpläne
{
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Termine;
  using System;
  using System.Globalization;
  using System.Linq;

  /// <summary>
  /// ViewModel of an individual wochenplan
  /// </summary>
  public class WochenplanWorkspaceViewModel : TerminplanWorkspaceViewModel
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="WochenplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public WochenplanWorkspaceViewModel()
    {
      this.NextWeekCommand = new DelegateCommand(this.NextWeek);
      this.PreviousWeekCommand = new DelegateCommand(this.PreviousWeek);
      this.PopulateTerminplan();
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
        var kalenderwoche = deCalendar.GetWeekOfYear(this.WochenplanMontag, weekRule, firstDayOfWeek);
        var überschrift = kalenderwoche + ". Woche, " + this.WochenplanMontag.ToString(@"dd.MM") + " - "
                          + this.WochenplanMontag.AddDays(4).ToString(@"dd.MM");

        // check if it is the current week
        var aktuelleKalenderwoche = deCalendar.GetWeekOfYear(DateTime.Today, weekRule, firstDayOfWeek);
        if (aktuelleKalenderwoche == kalenderwoche)
        {
          überschrift += "   > Aktuelle Woche";
        }

        return überschrift;
      }
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

    public TerminplanEintragCollection Wochenplan11
    {
      get { return this.GetWochenplanEintragCollection(1, 1); }
    }

    public int Wochenplan11Span
    {
      get { return this.Stundenanzahl(1, 1); }
    }

    public TerminplanEintragCollection Wochenplan12
    {
      get { return this.GetWochenplanEintragCollection(1, 2); }
    }

    public int Wochenplan12Span
    {
      get { return this.Stundenanzahl(1, 2); }
    }

    public TerminplanEintragCollection Wochenplan13
    {
      get { return this.GetWochenplanEintragCollection(1, 3); }
    }

    public int Wochenplan13Span
    {
      get { return this.Stundenanzahl(1, 3); }
    }

    public TerminplanEintragCollection Wochenplan14
    {
      get { return this.GetWochenplanEintragCollection(1, 4); }
    }

    public int Wochenplan14Span
    {
      get { return this.Stundenanzahl(1, 4); }
    }

    public TerminplanEintragCollection Wochenplan15
    {
      get { return this.GetWochenplanEintragCollection(1, 5); }
    }

    public int Wochenplan15Span
    {
      get { return this.Stundenanzahl(1, 5); }
    }

    public TerminplanEintragCollection Wochenplan16
    {
      get { return this.GetWochenplanEintragCollection(1, 6); }
    }

    public int Wochenplan16Span
    {
      get { return this.Stundenanzahl(1, 6); }
    }

    public TerminplanEintragCollection Wochenplan17
    {
      get { return this.GetWochenplanEintragCollection(1, 7); }
    }

    public int Wochenplan17Span
    {
      get { return this.Stundenanzahl(1, 7); }
    }

    public TerminplanEintragCollection Wochenplan18
    {
      get { return this.GetWochenplanEintragCollection(1, 8); }
    }

    public int Wochenplan18Span
    {
      get { return this.Stundenanzahl(1, 8); }
    }

    public TerminplanEintragCollection Wochenplan19
    {
      get { return this.GetWochenplanEintragCollection(1, 9); }
    }

    public int Wochenplan19Span
    {
      get { return this.Stundenanzahl(1, 9); }
    }

    public TerminplanEintragCollection Wochenplan110
    {
      get { return this.GetWochenplanEintragCollection(1, 10); }
    }

    public int Wochenplan110Span
    {
      get { return this.Stundenanzahl(1, 10); }
    }

    #endregion

    #region Dienstag

    public TerminplanEintragCollection Wochenplan21
    {
      get { return this.GetWochenplanEintragCollection(2, 1); }
    }

    public int Wochenplan21Span
    {
      get { return this.Stundenanzahl(2, 1); }
    }

    public TerminplanEintragCollection Wochenplan22
    {
      get { return this.GetWochenplanEintragCollection(2, 2); }
    }

    public int Wochenplan22Span
    {
      get { return this.Stundenanzahl(2, 2); }
    }

    public TerminplanEintragCollection Wochenplan23
    {
      get { return this.GetWochenplanEintragCollection(2, 3); }
    }

    public int Wochenplan23Span
    {
      get { return this.Stundenanzahl(2, 3); }
    }

    public TerminplanEintragCollection Wochenplan24
    {
      get { return this.GetWochenplanEintragCollection(2, 4); }
    }

    public int Wochenplan24Span
    {
      get { return this.Stundenanzahl(2, 4); }
    }

    public TerminplanEintragCollection Wochenplan25
    {
      get { return this.GetWochenplanEintragCollection(2, 5); }
    }

    public int Wochenplan25Span
    {
      get { return this.Stundenanzahl(2, 5); }
    }

    public TerminplanEintragCollection Wochenplan26
    {
      get { return this.GetWochenplanEintragCollection(2, 6); }
    }

    public int Wochenplan26Span
    {
      get { return this.Stundenanzahl(2, 6); }
    }

    public TerminplanEintragCollection Wochenplan27
    {
      get { return this.GetWochenplanEintragCollection(2, 7); }
    }

    public int Wochenplan27Span
    {
      get { return this.Stundenanzahl(2, 7); }
    }

    public TerminplanEintragCollection Wochenplan28
    {
      get { return this.GetWochenplanEintragCollection(2, 8); }
    }

    public int Wochenplan28Span
    {
      get { return this.Stundenanzahl(2, 8); }
    }

    public TerminplanEintragCollection Wochenplan29
    {
      get { return this.GetWochenplanEintragCollection(2, 9); }
    }

    public int Wochenplan29Span
    {
      get { return this.Stundenanzahl(2, 9); }
    }

    public TerminplanEintragCollection Wochenplan210
    {
      get { return this.GetWochenplanEintragCollection(2, 10); }
    }

    public int Wochenplan210Span
    {
      get { return this.Stundenanzahl(2, 10); }
    }

    #endregion

    #region Mittwoch

    public TerminplanEintragCollection Wochenplan31
    {
      get { return this.GetWochenplanEintragCollection(3, 1); }
    }

    public int Wochenplan31Span
    {
      get { return this.Stundenanzahl(3, 1); }
    }

    public TerminplanEintragCollection Wochenplan32
    {
      get { return this.GetWochenplanEintragCollection(3, 2); }
    }

    public int Wochenplan32Span
    {
      get { return this.Stundenanzahl(3, 2); }
    }

    public TerminplanEintragCollection Wochenplan33
    {
      get { return this.GetWochenplanEintragCollection(3, 3); }
    }

    public int Wochenplan33Span
    {
      get { return this.Stundenanzahl(3, 3); }
    }

    public TerminplanEintragCollection Wochenplan34
    {
      get { return this.GetWochenplanEintragCollection(3, 4); }
    }

    public int Wochenplan34Span
    {
      get { return this.Stundenanzahl(3, 4); }
    }

    public TerminplanEintragCollection Wochenplan35
    {
      get { return this.GetWochenplanEintragCollection(3, 5); }
    }

    public int Wochenplan35Span
    {
      get { return this.Stundenanzahl(3, 5); }
    }

    public TerminplanEintragCollection Wochenplan36
    {
      get { return this.GetWochenplanEintragCollection(3, 6); }
    }

    public int Wochenplan36Span
    {
      get { return this.Stundenanzahl(3, 6); }
    }

    public TerminplanEintragCollection Wochenplan37
    {
      get { return this.GetWochenplanEintragCollection(3, 7); }
    }

    public int Wochenplan37Span
    {
      get { return this.Stundenanzahl(3, 7); }
    }

    public TerminplanEintragCollection Wochenplan38
    {
      get { return this.GetWochenplanEintragCollection(3, 8); }
    }

    public int Wochenplan38Span
    {
      get { return this.Stundenanzahl(3, 8); }
    }

    public TerminplanEintragCollection Wochenplan39
    {
      get { return this.GetWochenplanEintragCollection(3, 9); }
    }

    public int Wochenplan39Span
    {
      get { return this.Stundenanzahl(3, 9); }
    }

    public TerminplanEintragCollection Wochenplan310
    {
      get { return this.GetWochenplanEintragCollection(3, 10); }
    }

    public int Wochenplan310Span
    {
      get { return this.Stundenanzahl(3, 10); }
    }

    #endregion

    #region Donnerstag

    public TerminplanEintragCollection Wochenplan41
    {
      get { return this.GetWochenplanEintragCollection(4, 1); }
    }

    public int Wochenplan41Span
    {
      get { return this.Stundenanzahl(4, 1); }
    }

    public TerminplanEintragCollection Wochenplan42
    {
      get { return this.GetWochenplanEintragCollection(4, 2); }
    }

    public int Wochenplan42Span
    {
      get { return this.Stundenanzahl(4, 2); }
    }

    public TerminplanEintragCollection Wochenplan43
    {
      get { return this.GetWochenplanEintragCollection(4, 3); }
    }

    public int Wochenplan43Span
    {
      get { return this.Stundenanzahl(4, 3); }
    }

    public TerminplanEintragCollection Wochenplan44
    {
      get { return this.GetWochenplanEintragCollection(4, 4); }
    }

    public int Wochenplan44Span
    {
      get { return this.Stundenanzahl(4, 4); }
    }

    public TerminplanEintragCollection Wochenplan45
    {
      get { return this.GetWochenplanEintragCollection(4, 5); }
    }

    public int Wochenplan45Span
    {
      get { return this.Stundenanzahl(4, 5); }
    }

    public TerminplanEintragCollection Wochenplan46
    {
      get { return this.GetWochenplanEintragCollection(4, 6); }
    }

    public int Wochenplan46Span
    {
      get { return this.Stundenanzahl(4, 6); }
    }

    public TerminplanEintragCollection Wochenplan47
    {
      get { return this.GetWochenplanEintragCollection(4, 7); }
    }

    public int Wochenplan47Span
    {
      get { return this.Stundenanzahl(4, 7); }
    }

    public TerminplanEintragCollection Wochenplan48
    {
      get { return this.GetWochenplanEintragCollection(4, 8); }
    }

    public int Wochenplan48Span
    {
      get { return this.Stundenanzahl(4, 8); }
    }

    public TerminplanEintragCollection Wochenplan49
    {
      get { return this.GetWochenplanEintragCollection(4, 9); }
    }

    public int Wochenplan49Span
    {
      get { return this.Stundenanzahl(4, 9); }
    }

    public TerminplanEintragCollection Wochenplan410
    {
      get { return this.GetWochenplanEintragCollection(4, 10); }
    }

    public int Wochenplan410Span
    {
      get { return this.Stundenanzahl(4, 10); }
    }

    #endregion

    #region Freitag

    public TerminplanEintragCollection Wochenplan51
    {
      get { return this.GetWochenplanEintragCollection(5, 1); }
    }

    public int Wochenplan51Span
    {
      get { return this.Stundenanzahl(5, 1); }
    }

    public TerminplanEintragCollection Wochenplan52
    {
      get { return this.GetWochenplanEintragCollection(5, 2); }
    }

    public int Wochenplan52Span
    {
      get { return this.Stundenanzahl(5, 2); }
    }

    public TerminplanEintragCollection Wochenplan53
    {
      get { return this.GetWochenplanEintragCollection(5, 3); }
    }

    public int Wochenplan53Span
    {
      get { return this.Stundenanzahl(5, 3); }
    }

    public TerminplanEintragCollection Wochenplan54
    {
      get { return this.GetWochenplanEintragCollection(5, 4); }
    }

    public int Wochenplan54Span
    {
      get { return this.Stundenanzahl(5, 4); }
    }

    public TerminplanEintragCollection Wochenplan55
    {
      get { return this.GetWochenplanEintragCollection(5, 5); }
    }

    public int Wochenplan55Span
    {
      get { return this.Stundenanzahl(5, 5); }
    }

    public TerminplanEintragCollection Wochenplan56
    {
      get { return this.GetWochenplanEintragCollection(5, 6); }
    }

    public int Wochenplan56Span
    {
      get { return this.Stundenanzahl(5, 6); }
    }

    public TerminplanEintragCollection Wochenplan57
    {
      get { return this.GetWochenplanEintragCollection(5, 7); }
    }

    public int Wochenplan57Span
    {
      get { return this.Stundenanzahl(5, 7); }
    }

    public TerminplanEintragCollection Wochenplan58
    {
      get { return this.GetWochenplanEintragCollection(5, 8); }
    }

    public int Wochenplan58Span
    {
      get { return this.Stundenanzahl(5, 8); }
    }

    public TerminplanEintragCollection Wochenplan59
    {
      get { return this.GetWochenplanEintragCollection(5, 9); }
    }

    public int Wochenplan59Span
    {
      get { return this.Stundenanzahl(5, 9); }
    }

    public TerminplanEintragCollection Wochenplan510
    {
      get { return this.GetWochenplanEintragCollection(5, 10); }
    }

    public int Wochenplan510Span
    {
      get { return this.Stundenanzahl(5, 10); }
    }

    #endregion

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "WochenplanWorkspace: " + this.WochenplanÜberschrift;
    }

    /// <summary>
    /// Populates the terminplan.
    /// </summary>
    protected override sealed void PopulateTerminplan()
    {
      this.Terminplaneinträge.Clear();
      //// use this instead of clear to enable undo
      //var count = this.Wochenplaneinträge.Count;
      //for (int i = 0; i < count; i++)
      //{
      //  this.Wochenplaneinträge.RemoveAt(this.Wochenplaneinträge.Count - 1);
      //}

      var comparer = new TerminplanEintragEqualityComparer();


      bool sommerHalbjahr;
      int jahresplanJahr;
      this.GetJahrAndHalbjahr(out sommerHalbjahr, out jahresplanJahr);

      // Get Einträge from Terminliste
      // Die Schultermine müssen zuerst eingelesen werden, da
      // sie sonst von den gleichen Lerngruppenterminen nicht zugelassen wurden
      var schultermine = App.MainViewModel.Schultermine.Where(o => o is SchulterminViewModel);
      var termineInWeek = schultermine.Where(o =>
        o.SchulterminDatum >= this.WochenplanMontag &&
        o.SchulterminDatum < this.WochenplanMontag.AddDays(6));

      foreach (var schulterminViewModel in termineInWeek)
      {
        var wochenplanEintrag = new TerminplanEintrag(this, schulterminViewModel);
        if (!this.Terminplaneinträge.Contains(wochenplanEintrag, comparer))
        {
          this.Terminplaneinträge.Add(wochenplanEintrag);
        }
      }

      // Get all jahrespläne for the selected week
      var jahrespläne =
        App.MainViewModel.Jahrespläne.Where(
          o => o.JahresplanSchuljahr.SchuljahrJahr == jahresplanJahr);

      foreach (var jahresplanViewModel in jahrespläne)
      {
        //foreach (var halbjahrsplan in jahresplanViewModel.Halbjahrespläne)
        //{

        // Get correct Halbjahresplan
        var halbjahrsplan = sommerHalbjahr
                                ? jahresplanViewModel.CurrentJahresplanSommerhalbjahr
                                : jahresplanViewModel.CurrentJahresplanWinterhalbjahr;

        if (halbjahrsplan == null)
        {
          continue;
        }

        // Get correct month
        var month = halbjahrsplan.Monatspläne.SingleOrDefault(o => o.MonatsplanMonatindex == this.WochenplanMontag.Month);
        if (month == null)
        {
          continue;
        }

        // Get correct days
        var daysInWeek =
          month.Tagespläne.Where(
            o => o.TagesplanDatum >= this.WochenplanMontag && o.TagesplanDatum < this.WochenplanMontag.AddDays(6));

        foreach (var tagesplanViewModel in daysInWeek)
        {
          foreach (var lerngruppenterminViewModel in tagesplanViewModel.Lerngruppentermine)
          {
            var wochenplanEintrag = new TerminplanEintrag(this, lerngruppenterminViewModel);
            if (!this.Terminplaneinträge.Contains(wochenplanEintrag, comparer))
            {
              this.Terminplaneinträge.Add(wochenplanEintrag);
            }
          }
        }

        // check if this week is at the end of month
        // and add missing days if it is so
        if (this.WochenplanMontag.Day + 6 > DateTime.DaysInMonth(this.WochenplanMontag.Year, this.WochenplanMontag.Month))
        {
          if (halbjahrsplan.Monatspläne.All(o => o.MonatsplanMonatindex != this.WochenplanMontag.Month + 1))
          {
            halbjahrsplan = sommerHalbjahr
                                  ? jahresplanViewModel.CurrentJahresplanWinterhalbjahr
                                  : jahresplanViewModel.CurrentJahresplanSommerhalbjahr;
          }

          if (halbjahrsplan == null)
          {
            continue;
          }

          // Get correct month
          var nextMonth = halbjahrsplan.Monatspläne.SingleOrDefault(o => o.MonatsplanMonatindex == this.WochenplanMontag.Month + 1);
          if (nextMonth == null)
          {
            // TODO
            continue;
          }

          // Get correct days
          var missingDaysInWeek =
            nextMonth.Tagespläne.Where(o => o.TagesplanDatum < this.WochenplanMontag.AddDays(6));

          foreach (var tagesplanViewModel in missingDaysInWeek)
          {
            foreach (var lerngruppenterminViewModel in tagesplanViewModel.Lerngruppentermine)
            {
              var wochenplanEintrag = new TerminplanEintrag(this, lerngruppenterminViewModel);

              if (!this.Terminplaneinträge.Contains(wochenplanEintrag, comparer))
              {
                this.Terminplaneinträge.Add(wochenplanEintrag);
              }
            }
          }
        }
      }
      //}

      // Check for Ferien
      foreach (var ferien in App.MainViewModel.Ferien.Where(
        schuljahr => schuljahr.FerienSchuljahr.Model.Jahr == jahresplanJahr))
      {
        for (int i = 0; i < 5; i++)
        {
          var tag = this.WochenplanMontag.AddDays(i);
          if (tag >= ferien.FerienErsterFerientag && tag <= ferien.FerienLetzterFerientag)
          {
            var termin = new Schultermin();
            termin.Beschreibung = ferien.FerienBezeichnung;
            termin.ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[0].Model;
            termin.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[8].Model;
            termin.IstGeprüft = true;
            termin.Termintyp = App.MainViewModel.Termintypen.First(o => o.TermintypBezeichnung == "Ferien").Model;
            termin.Datum = tag;
            termin.Schuljahr = App.MainViewModel.Schuljahre.First(o => o.SchuljahrJahr == jahresplanJahr).Model;

            var ferienTerminViewModel = new SchulterminViewModel(termin);
            var ferientagEintrag = new TerminplanEintrag(this, ferienTerminViewModel);

            if (!this.Terminplaneinträge.Contains(ferientagEintrag, comparer))
            {
              this.Terminplaneinträge.Add(ferientagEintrag);
            }
          }
        }
      }

      // Suche Geburtstage für Schüler der aktuellen Klassen
      var personenMitGeburtstag = App.MainViewModel.Personen.Where(
        o => o.PersonGeburtstag.HasValue
        && new DateTime(this.WochenplanMontag.Year, o.PersonGeburtstag.Value.Month, o.PersonGeburtstag.Value.Day) >= this.WochenplanMontag
        && new DateTime(this.WochenplanMontag.Year, o.PersonGeburtstag.Value.Month, o.PersonGeburtstag.Value.Day) < this.WochenplanMontag.AddDays(6));

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
        var alter = person.PersonAlter(this.WochenplanMontag.AddDays((int)geburstagInDerWoche.DayOfWeek));
        if (alter>18)
        {
          continue;
        }
        termin.Beschreibung = string.Format("{0} {1} ({2})", person.Vorname, person.Nachname, alter);
        var schuljahrViewModel = App.MainViewModel.Schuljahre.FirstOrDefault(o => o.SchuljahrJahr == jahresplanJahr);
        if (schuljahrViewModel != null)
        {
          termin.Schuljahr = schuljahrViewModel.Model;
        }

        var geburtstagTerminViewModel = new SchulterminViewModel(termin);
        var geburtstagEintrag = new TerminplanEintrag(this, geburtstagTerminViewModel);

        if (!this.Terminplaneinträge.Contains(geburtstagEintrag, comparer))
        {
          this.Terminplaneinträge.Add(geburtstagEintrag);
        }
      }

      this.NotifyWochenplanChanges();
    }

    /// <summary>
    /// Shows next week.
    /// </summary>
    private void NextWeek()
    {
      this.WochenplanMontag = this.WochenplanMontag.AddDays(7);
    }

    /// <summary>
    /// Shows previous week.
    /// </summary>
    private void PreviousWeek()
    {
      this.WochenplanMontag = this.WochenplanMontag.AddDays(-7);
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
