namespace SoftTeach.ViewModel.Wochenpläne
{
  using System;
  using System.Linq;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// ViewModel of an individual tagesplan
  /// </summary>
  public class TagesplanWorkspaceViewModel : TerminplanWorkspaceViewModel
  {
    /// <summary>
    /// Das Datum des Tagesplans.
    /// </summary>
    private DateTime tagesplanDatum;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TagesplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public TagesplanWorkspaceViewModel()
    {
      this.NextDayCommand = new DelegateCommand(this.NextDay);
      this.PreviousDayCommand = new DelegateCommand(this.PreviousDay);

      // Build data structures
      this.tagesplanDatum = DateTime.Today;

      // On saturday oder sunday show next week (to be prepared :-) )
      var day = (int)DateTime.Today.DayOfWeek;
      if (day > 5)
      {
        this.tagesplanDatum = this.tagesplanDatum.AddDays(day == 6 ? 2 : 1);
      }

      //this.WochenplanMontag = new DateTime(2013, 10, 21);
      this.PopulateTerminplan();
    }

    /// <summary>
    /// Holt den Befehl zur showing the next day in this wochenplan
    /// </summary>
    public DelegateCommand NextDayCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur showing the previous day in this wochenplan
    /// </summary>
    public DelegateCommand PreviousDayCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt das Datum des Tagesplans
    /// </summary>
    public DateTime TagesplanDatum
    {
      get
      {
        return this.tagesplanDatum;
      }

      set
      {
        this.tagesplanDatum = value;
        this.RaisePropertyChanged("TagesplanDatum");
        this.RaisePropertyChanged("TagesplanÜberschrift");
        this.PopulateTerminplan();
      }
    }

    /// <summary>
    /// Holt die Überschrift über den Tagesplan
    /// </summary>
    public string TagesplanÜberschrift
    {
      get
      {
        var überschrift = this.tagesplanDatum.ToString(@"D");

        // check if it is the current day
        if (this.tagesplanDatum == DateTime.Today)
        {
          überschrift += " > Heute";
        }

        return überschrift;
      }
    }

    /// <summary>
    /// Holt die Ganztagstermine
    /// </summary>
    public GanztagstermineViewModel Ganztagstermine
    {
      get { return this.GetGanztagstermine((int)this.tagesplanDatum.DayOfWeek); }
    }

    public TerminplanEintragCollection Wochenplan1
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 1); }
    }

    public int Wochenplan1Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 1); }
    }

    public TerminplanEintragCollection Wochenplan2
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 2); }
    }

    public int Wochenplan2Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 2); }
    }

    public TerminplanEintragCollection Wochenplan3
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 3); }
    }

    public int Wochenplan3Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 3); }
    }

    public TerminplanEintragCollection Wochenplan4
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 4); }
    }

    public int Wochenplan4Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 4); }
    }

    public TerminplanEintragCollection Wochenplan5
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 5); }
    }

    public int Wochenplan5Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 5); }
    }

    public TerminplanEintragCollection Wochenplan6
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 6); }
    }

    public int Wochenplan6Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 6); }
    }

    public TerminplanEintragCollection Wochenplan7
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 7); }
    }

    public int Wochenplan7Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 7); }
    }

    public TerminplanEintragCollection Wochenplan8
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 8); }
    }

    public int Wochenplan8Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 8); }
    }

    public TerminplanEintragCollection Wochenplan9
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 9); }
    }

    public int Wochenplan9Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 9); }
    }

    public TerminplanEintragCollection Wochenplan10
    {
      get { return this.GetWochenplanEintragCollection((int)this.tagesplanDatum.DayOfWeek, 10); }
    }

    public int Wochenplan10Span
    {
      get { return this.Stundenanzahl((int)this.tagesplanDatum.DayOfWeek, 10); }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "TagesplanWorkspace: " + this.TagesplanÜberschrift;
    }

    /// <summary>
    /// Shows next day.
    /// </summary>
    private void NextDay()
    {
      this.TagesplanDatum = this.tagesplanDatum.AddDays(1);
    }

    /// <summary>
    /// Shows previous day.
    /// </summary>
    private void PreviousDay()
    {
      this.TagesplanDatum = this.tagesplanDatum.AddDays(-1);
    }

    protected override void PopulateTerminplan()
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

      var jahrtyp = App.MainViewModel.Jahrtypen.FirstOrDefault(o => o.JahrtypJahr == jahresplanJahr);
      if (jahrtyp == null)
      {
        //var result = InformationDialog.Show("Schuljahr fehlt", string.Format("Das Schuljahr {0} ist noch nicht angelegt, soll das jetzt gemacht werden?", jahresplanJahr), true);
        //if (result.HasValue && result.Value)
        //{
        //  JahrtypWorkspaceViewModel.AddJahrtyp();
        //}
        //else
        //{
        return;
        //}
      }

      // Get Einträge from Terminliste
      // Die Schultermine müssen zuerst eingelesen werden, da
      // sie sonst von den gleichen Lerngruppenterminen nicht zugelassen wurden
      var schultermine = App.MainViewModel.Schultermine.Where(o => o is SchulterminViewModel);
      var termineAmTag = schultermine.Where(o => o.SchulterminDatum == this.tagesplanDatum);

      foreach (var schulterminViewModel in termineAmTag)
      {
        var terminplanEintrag = new TerminplanEintrag(this, schulterminViewModel);
        if (!this.Terminplaneinträge.Contains(terminplanEintrag, comparer))
        {
          this.Terminplaneinträge.Add(terminplanEintrag);
        }
      }

      // Get all jahrespläne for the selected week
      var jahrespläne = App.MainViewModel.Jahrespläne.Where(o => o.JahresplanJahrtyp.JahrtypJahr == jahresplanJahr);

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


        var month = halbjahrespläne.Monatspläne.SingleOrDefault(o => o.MonatsplanMonatindex == this.tagesplanDatum.Month);
        if (month == null)
        {
          continue;
        }

        // Get correct days
        var day = month.Tagespläne.Where(o => o.TagesplanDatum == this.tagesplanDatum);

        foreach (var tagesplanViewModel in day)
        {
          foreach (var lerngruppenterminViewModel in tagesplanViewModel.Lerngruppentermine)
          {
            var terminplanEintrag = new TerminplanEintrag(this, lerngruppenterminViewModel);
            if (!this.Terminplaneinträge.Contains(terminplanEintrag, comparer))
            {
              this.Terminplaneinträge.Add(terminplanEintrag);
            }
          }
        }
      }

      // Check for Ferien
      foreach (var ferien in App.MainViewModel.Ferien.Where(
        schuljahr => schuljahr.FerienJahrtyp.Model.Jahr == jahresplanJahr))
      {
        var tag = this.tagesplanDatum;
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
          var ferientagEintrag = new TerminplanEintrag(this, ferienTerminViewModel);

          if (!this.Terminplaneinträge.Contains(ferientagEintrag, comparer))
          {
            this.Terminplaneinträge.Add(ferientagEintrag);
          }
        }
      }


      // Suche Geburtstage für Schüler der aktuellen Klassen
      var personenMitGeburtstag = App.MainViewModel.Personen.Where(
        o => o.PersonGeburtstag.HasValue
        && new DateTime(this.tagesplanDatum.Year, o.PersonGeburtstag.Value.Month, o.PersonGeburtstag.Value.Day) == this.tagesplanDatum);

      foreach (var person in personenMitGeburtstag)
      {
        var termin = new Schultermin();
        termin.ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[0].Model;
        termin.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[8].Model;
        termin.IstGeprüft = true;
        termin.Termintyp = App.MainViewModel.Termintypen.First(o => o.TermintypBezeichnung == "Geburtstag").Model;
        var geburstagInDerWoche = new DateTime(
          this.TagesplanDatum.Year,
          person.PersonGeburtstag.Value.Month,
          person.PersonGeburtstag.Value.Day);
        termin.Datum = geburstagInDerWoche;
        var alter = person.PersonAlter(this.tagesplanDatum);
        if (alter > 18)
        {
          continue;
        }

        termin.Beschreibung = string.Format("{0} {1} ({2})", person.PersonVorname, person.PersonNachname, alter);
        termin.Jahrtyp = App.MainViewModel.Jahrtypen.First(o => o.JahrtypJahr == jahresplanJahr).Model;

        var geburtstagTerminViewModel = new SchulterminViewModel(termin);
        var geburtstagEintrag = new TerminplanEintrag(this, geburtstagTerminViewModel);

        if (!this.Terminplaneinträge.Contains(geburtstagEintrag, comparer))
        {
          this.Terminplaneinträge.Add(geburtstagEintrag);
        }
      }

      this.NotifyTagesplanChanges();
    }

    /// <summary>
    /// Notifies the tagesplan changes.
    /// </summary>
    private void NotifyTagesplanChanges()
    {
      for (int i = 1; i < 11; i++)
      {
        string propertyString = "Wochenplan" + i;
        string propertySpanString = "Wochenplan" + i + "Span";
        this.RaisePropertyChanged(propertyString);
        this.RaisePropertyChanged(propertySpanString);
        this.RaisePropertyChanged("Ganztagstermine");
      }
    }
  }
}
