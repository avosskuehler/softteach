namespace SoftTeach.ViewModel.Jahrespläne
{
  using Helper;
  using Microsoft.Win32;
  using Resources.Controls;
  using Setting;
  using SoftTeach;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Curricula;
  using SoftTeach.View.Jahrespläne;
  using SoftTeach.ViewModel;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper.ODSSupport;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Termine;
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

    private int halbjahrIndex;

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
      this.PullStundenCommand = new DelegateCommand(this.PullStunden);
      this.RemoveStundenCommand = new DelegateCommand(this.RemoveStunden);
      this.GetStundenFromOtherJahresplanCommand = new DelegateCommand(this.GetStundenFromOtherJahresplan);
      this.StundenAlsOdsExportierenCommand = new DelegateCommand(this.StundenAlsOdsExportieren);

      this.AktuellesDatum = DateTime.Today;

      this.Wochentage = new ObservableCollection<string> { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
      this.MonateHJ1 = new ObservableCollection<string> { "August", "September", "Oktober", "November", "Dezember", "Januar" };
      this.MonateHJ2 = new ObservableCollection<string> { "Februar", "März", "April", "Mai", "Juni", "Juli" };

      this.TageHJ1 = new ObservableCollection<TagViewModel>();
      this.TageHJ1ViewSource = new CollectionViewSource() { Source = this.TageHJ1 };
      using (this.TageHJ1ViewSource.DeferRefresh())
      {
        this.TageHJ1ViewSource.SortDescriptions.Add(new SortDescription("Datum", ListSortDirection.Ascending));
        this.TageHJ1ViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Monat"));
      }

      this.TageHJ2 = new ObservableCollection<TagViewModel>();
      this.TageHJ2ViewSource = new CollectionViewSource() { Source = this.TageHJ2 };
      using (this.TageHJ2ViewSource.DeferRefresh())
      {
        this.TageHJ2ViewSource.SortDescriptions.Add(new SortDescription("Datum", ListSortDirection.Ascending));
        this.TageHJ2ViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Monat"));
      }

      this.currentDate = DateTime.Now;
      this.lerngruppe = lerngruppe;

      this.KalenderErstellen();
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
    /// Holt den Befehl zur getting the stunden for this jahresplan from a stundenplan
    /// </summary>
    public DelegateCommand PullStundenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl alle Stunden aus dem Halbjahresplan zu löschen
    /// </summary>
    public DelegateCommand RemoveStundenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um die Stunden aus einem alten Halbjahresplan zu holen
    /// </summary>
    public DelegateCommand GetStundenFromOtherJahresplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um die Stunden als ODS zu exportieren
    /// </summary>
    public DelegateCommand StundenAlsOdsExportierenCommand { get; private set; }

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
    /// Holt oder setzt den Halbjahrindex
    /// </summary>
    public int HalbjahrIndex
    {
      get => this.halbjahrIndex;

      set
      {
        if (value == this.halbjahrIndex) return;
        this.halbjahrIndex = value;
        this.RaisePropertyChanged("HalbjahrIndex");
      }
    }

    /// <summary>
    /// Holt das ausgewählte Halbjahr des Jahresplans
    /// </summary>
    [DependsUpon("HalbjahrIndex")]
    public Halbjahr Halbjahr
    {
      get
      {
        if (this.HalbjahrIndex == 0)
        {
          return Halbjahr.Winter;
        }

        return Halbjahr.Sommer;
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
    /// Holt oder setzt die im Jahresplan dargestellt Lerngruppe
    /// </summary>
    public string Bezeichnung
    {
      get => string.Format("Jahresplan für {0} {1} im Schuljahr {2}", this.Lerngruppe.LerngruppeBezeichnung, this.Fach.FachKurzbezeichnung, this.Schuljahr.SchuljahrBezeichnung);
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung des aktuellen Kalendermonats
    /// </summary>
    [DependsUpon("AktuellesDatum")]
    public string MonatsBezeichnung => this.AktuellesDatum.ToString("MMMM yyyy");

    ///// <summary>
    ///// Erstellt den Kalender neu und lädt dabei die Lerngruppentermine
    ///// </summary>
    //public void KalenderNeuLaden()
    //{
    //  if (!this.TageHJ1.Any())
    //  {
    //    this.KalenderErstellen();
    //  }
    //}

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
        TagViewModel tag = new TagViewModel(this.lerngruppe) { Datum = d, Enabled = true, IstWochenende = d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday };

        var lerngruppentermine = this.lerngruppe.Lerngruppentermine.Where(o =>
          o.LerngruppenterminSchuljahr.Jahr == this.Schuljahr.SchuljahrJahr
          && o.LerngruppenterminDatum == d.Date).ToObservableCollection();
        tag.Lerngruppentermine = lerngruppentermine;

        var ferien = App.MainViewModel.Schultermine.Where(o =>
          o.SchulterminSchuljahr.SchuljahrJahr == this.Schuljahr.SchuljahrJahr
          && o.TerminTermintyp == Termintyp.Ferien).Any(o => o.SchulterminDatum.Date == d.Date);
        if (ferien)
        {
          tag.IstFerien = true;
        }

        var ferien2 = App.MainViewModel.Ferien.Where(o =>
          o.Model.Schuljahr.Jahr == this.Schuljahr.SchuljahrJahr).Any(o => o.FerienErsterFerientag <= d.Date && o.FerienLetzterFerientag >= d.Date);
        if (ferien2)
        {
          tag.IstFerien = true;
        }

        var feiertag = App.MainViewModel.Schultermine.Where(o =>
          o.SchulterminSchuljahr.SchuljahrJahr == this.Schuljahr.SchuljahrJahr
          && o.TerminTermintyp == Termintyp.Feiertag).Any(o => o.SchulterminDatum.Date == d.Date);
        if (feiertag)
        {
          tag.IstFeiertag = true;
        }

        tag.IstHeute = d == DateTime.Today;

        if (box <= 184)
        {
          tag.Halbjahr = Halbjahr.Winter;
          this.TageHJ1.Add(tag);
        }
        else
        {
          tag.Halbjahr = Halbjahr.Sommer;
          this.TageHJ2.Add(tag);
        }

        d = d.AddDays(1);
      }
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
      return "Jahresplan für" + this.Lerngruppe.LerngruppeKurzbezeichnung;
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
    /// Erstellt vordefinierte Stunden an den Tagen, an denen laut Stundenplan
    /// Unterricht stattfindet.
    /// </summary>
    public void PullStunden()
    {
      var stundenpläne = App.MainViewModel.Stundenpläne.Where(
        o => o.StundenplanSchuljahr.Model == this.Schuljahr.Model
        && o.StundenplanHalbjahr == this.Halbjahr).ToList();

      if (!stundenpläne.Any())
      {
        var dlg = new InformationDialog(
          "Stundenplan fehlt", "Ein Stundenplan ist für das aktuelle" + " Halbjahr noch nicht angelegt", false);
        dlg.ShowDialog();
        return;
      }

      // sort by gültigab date
      stundenpläne.Sort();

      using (new UndoBatch(App.MainViewModel, string.Format("Stunden im Jahresplan {0} angelegt.", this.Bezeichnung), false))
      {
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
        for (int i = 0; i < stundenpläne.Count; i++)
        {
          var stundenplanViewModel = stundenpläne[i];
          var stundenplaneinträge =
            stundenplanViewModel.Stundenplaneinträge.Where(o => o.StundenplaneintragLerngruppe == this.Lerngruppe && o.StundenplaneintragFach == this.Fach);

          if (!stundenplaneinträge.Any())
          {
            var dlg = new InformationDialog(
              "Im Stundenplan nicht gefunden",
              string.Format(
                "Im Stundenplan gültig ab {0} wurde für diese Klasse und das Fach kein Eintrag gefunden.",
                stundenplanViewModel.StundenplanGültigAb.ToShortDateString()),
              false);
            dlg.ShowDialog();
            continue;
          }

          var gültigAb = stundenplanViewModel.StundenplanGültigAb;
          var gültigBis = stundenpläne.Count > i + 1 ? stundenpläne[i + 1].StundenplanGültigAb : gültigAb.AddYears(1);

          ObservableCollection<TagViewModel> tagedesHalbjahres = this.HalbjahrIndex == 0 ? this.TageHJ1 : this.TageHJ2;

          foreach (var tagesplanViewModel in tagedesHalbjahres)
          {
            foreach (var stundenplaneintragViewModel in stundenplaneinträge)
            {
              if (tagesplanViewModel.Datum >= gültigAb && tagesplanViewModel.Datum <= gültigBis)
              {
                if ((int)tagesplanViewModel.Datum.DayOfWeek == stundenplaneintragViewModel.StundenplaneintragWochentagIndex)
                {
                  // Ferien, Feiertage und Wochenende aussparen
                  if (tagesplanViewModel.IstFerien || tagesplanViewModel.IstFeiertag || tagesplanViewModel.IstWochenende)
                  {
                    continue;
                  }

                  // Wenn schon eine passende Stunde existiert, ignorieren
                  if (tagesplanViewModel.Lerngruppentermine.Any(o =>
                    o.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex <= stundenplaneintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex &&
                    o.TerminLetzteUnterrichtsstunde.UnterrichtsstundeIndex >= stundenplaneintragViewModel.StundenplaneintragLetzteUnterrichtsstundeIndex))
                  {
                    continue;
                  }

                  // Now we found the day on which a stunde is found in the stundenplan
                  // and no other stunden already placed
                  // So create a stunde
                  var stunde = new StundeNeu();
                  stunde.ErsteUnterrichtsstunde =
                    App.MainViewModel.Unterrichtsstunden[
                      stundenplaneintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex - 1].Model;
                  stunde.LetzteUnterrichtsstunde =
                    App.MainViewModel.Unterrichtsstunden[
                      stundenplaneintragViewModel.StundenplaneintragLetzteUnterrichtsstundeIndex - 1].Model;
                  stunde.Datum = tagesplanViewModel.Datum;
                  stunde.Termintyp = Model.TeachyModel.Termintyp.Unterricht;
                  stunde.Lerngruppe = this.Lerngruppe.Model;
                  stunde.Hausaufgaben = string.Empty;
                  stunde.Ansagen = string.Empty;
                  stunde.Jahrgang = this.Jahrgang;
                  stunde.Fach = this.Fach.Model;
                  stunde.Halbjahr = this.Halbjahr;
                  if (stundenplaneintragViewModel.StundenplaneintragRaum != null)
                  {
                    stunde.Ort = stundenplaneintragViewModel.StundenplaneintragRaum.RaumBezeichnung;
                  }

                  var vm = new StundeViewModel(stunde);
                  tagesplanViewModel.Lerngruppentermine.Add(vm);
                  lerngruppe.Lerngruppentermine.Add(vm);
                }
              }
            }

            tagesplanViewModel.UpdateView();
          }
        }

        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
      }
    }

    /// <summary>
    /// Löscht alle leeren Stunden aus dem Jahresplan
    /// </summary>
    private void RemoveStunden()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Stunden im Jahresplan {0} gelöscht.", this.Bezeichnung), false))
      {
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;

        ObservableCollection<TagViewModel> tagedesHalbjahres = this.HalbjahrIndex == 0 ? this.TageHJ1 : this.TageHJ2;

        foreach (var tagesplanViewModel in tagedesHalbjahres)
        {
          var stunden = tagesplanViewModel.Lerngruppentermine.Where(o => o is StundeViewModel).ToList();
          foreach (var lerngruppenterminViewModel in stunden)
          {
            var stunde = lerngruppenterminViewModel as StundeViewModel;
            if (stunde.StundePhasenKurzform == string.Empty)
            {
              lerngruppe.Lerngruppentermine.RemoveTest(lerngruppenterminViewModel);
              tagesplanViewModel.Lerngruppentermine.RemoveTest(lerngruppenterminViewModel);
            }
          }

          tagesplanViewModel.UpdateView();
        }

        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
      }
    }

    /// <summary>
    /// Holt die Stundenentwürfe aus einem anderen Jahresplan.
    /// </summary>
    private void GetStundenFromOtherJahresplan()
    {
      var dlg = new AskForLerngruppeToAdaptDialog(this.Fach, this.Jahrgang);
      dlg.Title = "Aus welcher Lerngruppe sollen die Stunden übertragen werden?";
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Stunden in Jahresplan {0} importiert.", this.Bezeichnung), false))
        {
          var vm = App.MainViewModel.LoadLerngruppe(dlg.SelectedLerngruppe);

          var halbjahresplanZuweisenWorkspace = new JahresplanZuweisenWorkspaceViewModel(vm, this.lerngruppe, this.Halbjahr);
          var dlgZuweisen = new JahresplanZuweisenDialog { DataContext = halbjahresplanZuweisenWorkspace };
          dlgZuweisen.ShowDialog();
        }
      }
    }

    private void StundenAlsOdsExportieren()
    {
      var fileDialog = new SaveFileDialog
      {
        Filter = "ODS files (*.ods)|*.ods|All files (*.*)|*.*",
      };
      if (fileDialog.ShowDialog() == true)
      {
        new OdsReaderWriter().WriteOdsFile(this.Lerngruppe, fileDialog.FileName);
      }
    }
  }
}
