namespace SoftTeach.ViewModel.Jahrespläne
{
  using Helper;
  using Resources.Controls;
  using Setting;
  using SoftTeach;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Wochenpläne;
  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;

  /// <summary>
  /// ViewModel eines Jahresplans
  /// </summary>
  public class JahresplanViewModel : ViewModelBase
  {
    /// <summary>
    /// Das aktuell gewählte Datum
    /// </summary>
    private DateTime currentDate;

    private TagViewModel aktuellerTag;

    private LerngruppeViewModel lerngruppe;

    /// <summary>
    /// Diese Event wird aufgerufen, wenn ein Tag geändert wurde.
    /// </summary>
    public event EventHandler<TagGeändertEventArgs> TagGeändert;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahresplanViewModel"/> Klasse. 
    /// </summary>
    public JahresplanViewModel(LerngruppeViewModel lerngruppe)
    {
      this.MonatZurückCommand = new DelegateCommand(this.MonatZurück);
      this.MonatVorCommand = new DelegateCommand(this.MonatVor);

      this.AktuellesDatum = DateTime.Today;

      this.Wochentage = new ObservableCollection<string> { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
      this.MonateHJ1 = new ObservableCollection<string> { "August", "September", "Oktober", "November", "Dezember", "Januar" };
      this.MonateHJ2 = new ObservableCollection<string> { "Februar", "März", "April", "Mai", "Juni", "Juli" };

      this.TageHJ1 = new ObservableCollection<TagViewModel>();
      this.TageHJ1ViewSource = new CollectionViewSource() { Source = this.TageHJ1 };
      using (this.TageHJ1ViewSource.DeferRefresh())
      {
        this.TageHJ1ViewSource.Filter += this.TageViewSource_Filter;
        this.TageHJ1ViewSource.SortDescriptions.Add(new SortDescription("Datum", ListSortDirection.Ascending));
        this.TageHJ1ViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Monat"));
      }

      this.TageHJ2 = new ObservableCollection<TagViewModel>();
      this.TageHJ2ViewSource = new CollectionViewSource() { Source = this.TageHJ2 };
      using (this.TageHJ2ViewSource.DeferRefresh())
      {
        this.TageHJ2ViewSource.Filter += this.TageViewSource_Filter;
        this.TageHJ2ViewSource.SortDescriptions.Add(new SortDescription("Datum", ListSortDirection.Ascending));
        this.TageHJ2ViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Monat"));
      }

      this.currentDate = DateTime.Now;
      this.lerngruppe = lerngruppe;
    }

    /// <summary>
    /// Holt den Befehl den Kalender einen Monat früher anzuzeigen
    /// </summary>
    public DelegateCommand MonatZurückCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl den Kalender einen Monat später anzuzeigen
    /// </summary>
    public DelegateCommand MonatVorCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die Liste mit den Tagen für den Kalender im ersten Halbjahr
    /// </summary>
    public ObservableCollection<TagViewModel> TageHJ1 { get; private set; }

    /// <summary>
    /// Holt oder setzt die Liste mit den Tagen für den Kalender im zweiten Halbjahr
    /// </summary>
    public ObservableCollection<TagViewModel> TageHJ2 { get; private set; }

    /// <summary>
    /// Holt oder setzt die Wochentagsliste
    /// </summary>
    public ObservableCollection<string> Wochentage { get; private set; }

    /// <summary>
    /// Holt oder setzt die Monatsliste für das 1.Halbjahr
    /// </summary>
    public ObservableCollection<string> MonateHJ1 { get; private set; }

    /// <summary>
    /// Holt oder setzt die Monatsliste für das 2.Halbjahr
    /// </summary>
    public ObservableCollection<string> MonateHJ2 { get; private set; }

    /// <summary>
    /// Holt oder setzt die View Source der Tage des ersten Halbjahres
    /// </summary>
    public CollectionViewSource TageHJ1ViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes und gruppiertes View der Tage
    /// </summary>
    public ICollectionView TageHJ1View => this.TageHJ1ViewSource.View;

    /// <summary>
    /// Holt oder setzt die View Source der Tage des zweiten Halbjahres
    /// </summary>
    public CollectionViewSource TageHJ2ViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes und gruppiertes View der Tage
    /// </summary>
    public ICollectionView TageHJ2View => this.TageHJ2ViewSource.View;

    /// <summary>
    /// Holt oder setzt das aktuelle Datum
    /// </summary>
    public DateTime AktuellesDatum
    {
      get => this.currentDate;

      set
      {
        if (value == this.currentDate) return;
        this.currentDate = value;
        this.RaisePropertyChanged("AktuellesDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt den aktuell ausgewählten Tag
    /// </summary>
    public TagViewModel AktuellerTag
    {
      get => this.aktuellerTag;

      set
      {
        if (value == this.aktuellerTag) return;
        this.aktuellerTag = value;
        this.RaisePropertyChanged("AktuellerTag");
      }
    }

    /// <summary>
    /// Holt oder setzt die im Jahresplan dargestellt Lerngruppe
    /// </summary>
    public LerngruppeViewModel Lerngruppe
    {
      get => this.lerngruppe;

      set
      {
        if (value == this.lerngruppe) return;
        this.lerngruppe = value;
        this.RaisePropertyChanged("Lerngruppe");
      }
    }

    /// <summary>
    /// Holt das Fach zum Jahresplan, entspricht dem Fach der Lerngruppe
    /// </summary>
    public FachViewModel Fach
    {
      get
      {
        return this.Lerngruppe.LerngruppeFach;
      }
    }

    /// <summary>
    /// Holt das Schuljahr zum Jahresplan, entspricht dem Schuljahr der Lerngruppe
    /// </summary>
    public SchuljahrViewModel Schuljahr
    {
      get
      {
        return this.Lerngruppe.LerngruppeSchuljahr;
      }
    }

    /// <summary>
    /// Holt den Jahrgang zum Jahresplan, entspricht dem Jahrgang der Lerngruppe
    /// </summary>
    public int Jahrgang
    {
      get
      {
        return this.Lerngruppe.LerngruppeJahrgang;
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung des aktuellen Kalendermonats
    /// </summary>
    [DependsUpon("AktuellesDatum")]
    public string MonatsBezeichnung => this.AktuellesDatum.ToString("MMMM yyyy");

    /// <summary>
    /// Erstellt den Kalender neu und lädt dabei die Lerngruppentermine
    /// </summary>
    public void KalenderNeuLaden()
    {
      if (!this.TageHJ1.Any())
      {
        this.KalenderErstellen();
      }
    }

    private void KalenderErstellen()
    {
      this.TageHJ1.Clear();
      this.TageHJ2.Clear();

      // Berechne den ersten Tag des Monats
      // die Kästchen vorher werden anders dargestellt
      //DateTime d = new DateTime(targetDate.Year, targetDate.Month, 1);
      //int offset = DayOfWeekNumber(d.DayOfWeek) - 1;
      //d = d.AddDays(-offset);
      DateTime d = new DateTime(this.Schuljahr.SchuljahrJahr, 8, 1);

      // Erstelle das ganze Jahr
      for (int box = 1; box <= 365; box++)
      {
        TagViewModel tag = new TagViewModel { Datum = d, Enabled = true, IstWochenende = d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday };

        var lerngruppentermine = this.lerngruppe.Lerngruppentermine.Where(o =>
          o.LerngruppenterminSchuljahr.Jahr == Selection.Instance.Schuljahr.SchuljahrJahr
          && o.LerngruppenterminDatum == d.Date).ToObservableCollection();
        tag.Lerngruppentermine = lerngruppentermine;

        var ferien = App.MainViewModel.Schultermine.Where(o =>
          o.SchulterminSchuljahr.SchuljahrJahr == Selection.Instance.Schuljahr.SchuljahrJahr
          && o.TerminTermintyp == Termintyp.Ferien).Any(o => o.SchulterminDatum.Date == d.Date);
        if (ferien)
        {
          tag.IstFerien = true;
        }

        var feiertag = App.MainViewModel.Schultermine.Where(o =>
          o.SchulterminSchuljahr.SchuljahrJahr == Selection.Instance.Schuljahr.SchuljahrJahr
          && o.TerminTermintyp == Termintyp.Feiertag).Any(o => o.SchulterminDatum.Date == d.Date);
        if (feiertag)
        {
          tag.IstFeiertag = true;
        }

        tag.PropertyChanged += this.Day_Changed;
        tag.IstHeute = d == DateTime.Today;

        if (box <= 183)
        {
          this.TageHJ1.Add(tag);
        }
        else
        {
          this.TageHJ2.Add(tag);
        }
        d = d.AddDays(1);
      }
    }

    private void Day_Changed(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != "Notizen") return;
      if (TagGeändert == null) return;

      TagGeändert(this, new TagGeändertEventArgs(sender as TagViewModel));
    }

    private static int DayOfWeekNumber(DayOfWeek dow)
    {
      return Convert.ToInt32(dow.ToString("D"));
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
      return "Kalender";
    }

    /// <summary>
    /// Erstellt den Kalender neu für einen Monat früher
    /// </summary>
    internal void MonatZurück()
    {
      var neuerMonat = this.AktuellesDatum.Month - 1;
      var neuesJahr = this.AktuellesDatum.Year;
      if (neuerMonat < 1)
      {
        neuerMonat = 12;
        neuesJahr -= 1;
      }
      if (this.AktuellesDatum.Day > 28)
      {
        this.AktuellesDatum = new DateTime(neuesJahr, neuerMonat, 28);
      }
      else
      {
        this.AktuellesDatum = new DateTime(neuesJahr, neuerMonat, this.AktuellesDatum.Day);
      }

      this.KalenderErstellen();
    }

    /// <summary>
    /// Erstellt den Kalender neu für einen Monat später
    /// </summary>
    internal void MonatVor()
    {
      var neuerMonat = this.AktuellesDatum.Month + 1;
      var neuesJahr = this.AktuellesDatum.Year;
      if (neuerMonat > 12)
      {
        neuerMonat = 1;
        neuesJahr = neuesJahr + 1;
      }
      if (this.AktuellesDatum.Day > 28)
      {
        this.AktuellesDatum = new DateTime(neuesJahr, neuerMonat, 28);
      }
      else
      {
        this.AktuellesDatum = new DateTime(neuesJahr, neuerMonat, this.AktuellesDatum.Day);
      }
      this.KalenderErstellen();
    }

    /// <summary>
    /// Filtert die Wahlfachangebote nach Schuljahr
    /// </summary>
    private void TageViewSource_Filter(object sender, FilterEventArgs e)
    {
      if (!(e.Item is TagViewModel tag))
      {
        e.Accepted = false;
        return;
      }

      e.Accepted = true;
    }

  }
}
