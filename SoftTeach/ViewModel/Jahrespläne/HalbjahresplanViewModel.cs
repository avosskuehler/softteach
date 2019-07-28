namespace SoftTeach.ViewModel.Jahrespläne
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// ViewModel of an individual halbjahresplan
  /// </summary>
  public class HalbjahresplanViewModel : ViewModelBase
  {
    /// <summary>
    /// The halbjahrtyp currently assigned to this halbjahresplan
    /// </summary>
    private HalbjahrtypViewModel halbjahrtyp;

    /// <summary>
    /// The monatsplan currently selected
    /// </summary>
    private MonatsplanViewModel currentMonatsplan;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="HalbjahresplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="halbjahresplan">
    /// The underlying halbjahresplan this ViewModel is to be based on
    /// </param>
    public HalbjahresplanViewModel(Halbjahresplan halbjahresplan)
    {
      if (halbjahresplan == null)
      {
        throw new ArgumentNullException("halbjahresplan");
      }

      this.Model = halbjahresplan;

      // Build data structures for phasen
      this.Monatspläne = new ObservableCollection<MonatsplanViewModel>();
      foreach (var monatsplan in halbjahresplan.Monatspläne)
      {
        var vm = new MonatsplanViewModel(monatsplan);
        //App.MainViewModel.Monatspläne.Add(vm);
        this.Monatspläne.Add(vm);
      }

      this.Monatspläne.CollectionChanged += this.MonatspläneCollectionChanged;
    }

    /// <summary>
    /// Holt den underlying Halbjahresplan this ViewModel is based on
    /// </summary>
    public Halbjahresplan Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected Monatsplan
    /// </summary>
    public MonatsplanViewModel CurrentMonatsplan
    {
      get
      {
        return this.currentMonatsplan;
      }

      set
      {
        this.currentMonatsplan = value;
        this.RaisePropertyChanged("CurrentMonatsplan");
      }
    }

    /// <summary>
    /// Holt den ersten monat
    /// </summary>
    public MonatsplanViewModel Monat1
    {
      get
      {
        if (this.Monatspläne.Count > 0)
        {
          return this.Monatspläne[0];
        }

        return null;
      }
    }

    /// <summary>
    /// Holt den zweiten monat
    /// </summary>
    public MonatsplanViewModel Monat2
    {
      get
      {
        if (this.Monatspläne.Count > 1)
        {
          return this.Monatspläne[1];
        }

        return null;
      }
    }

    /// <summary>
    /// Holt den dritten monat
    /// </summary>
    public MonatsplanViewModel Monat3
    {
      get
      {
        if (this.Monatspläne.Count > 2)
        {
          return this.Monatspläne[2];
        }

        return null;
      }
    }

    /// <summary>
    /// Holt den vierten monat
    /// </summary>
    public MonatsplanViewModel Monat4
    {
      get
      {
        if (this.Monatspläne.Count > 3)
        {
          return this.Monatspläne[3];
        }

        return null;
      }
    }

    /// <summary>
    /// Holt den fünften monat
    /// </summary>
    public MonatsplanViewModel Monat5
    {
      get
      {
        if (this.Monatspläne.Count > 4)
        {
          return this.Monatspläne[4];
        }

        return null;
      }
    }

    /// <summary>
    /// Holt den sechsten monat
    /// </summary>
    public MonatsplanViewModel Monat6
    {
      get
      {
        if (this.Monatspläne.Count > 5)
        {
          return this.Monatspläne[5];
        }

        return null;
      }
    }

    /// <summary>
    /// Holt eine Bezeichnung für den Halbjahresplan.
    /// </summary>
    [DependsUpon("HalbjahresplanHalbjahrtyp")]
    public string HalbjahresplanKurbezeichnung
    {
      get
      {
        return
          string.Format(
            this.HalbjahresplanHalbjahrtyp != null
              ? this.HalbjahresplanHalbjahrtyp.HalbjahrtypBezeichnung
              : string.Empty);
      }
    }

    /// <summary>
    /// Holt eine Bezeichnung für den Halbjahresplan.
    /// </summary>
    [DependsUpon("HalbjahresplanHalbjahrtyp")]
    public string HalbjahresplanBezeichnung
    {
      get
      {
        return string.Format(
          "Halbjahresplan {0}, {1} {2}, für Lerngruppe {3}",
          this.HalbjahresplanFach != null ? this.HalbjahresplanFach.FachBezeichnung : string.Empty,
          this.HalbjahresplanHalbjahrtyp != null ? this.HalbjahresplanHalbjahrtyp.HalbjahrtypBezeichnung : string.Empty,
          this.HalbjahresplanJahrtyp != null ? this.HalbjahresplanJahrtyp.JahrtypBezeichnung : string.Empty,
          this.HalbjahresplanKlasse != null ? this.HalbjahresplanKlasse.KlasseBezeichnung : "?");
      }
    }

    /// <summary>
    /// Holt eine Überschrift für den Halbjahresplan.
    /// </summary>
    public string HalbjahresplanHeader
    {
      get
      {
        var jahr = this.Model.Jahresplan.Jahrtyp.Jahr;
        if (this.Model.Halbjahrtyp.Bezeichnung == "Sommer")
        {
          jahr++;
        }

        return this.Model.Halbjahrtyp.Bezeichnung + "\n" + jahr;
      }
    }

    /// <summary>
    /// Holt oder setzt die Halbjahrtyp currently assigned to this Halbjahresplan
    /// </summary>
    public HalbjahrtypViewModel HalbjahresplanHalbjahrtyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Halbjahrtyp == null)
        {
          return null;
        }

        if (this.halbjahrtyp == null || this.halbjahrtyp.Model != this.Model.Halbjahrtyp)
        {
          this.halbjahrtyp = App.MainViewModel.Halbjahrtypen.SingleOrDefault(d => d.Model == this.Model.Halbjahrtyp);
        }

        return this.halbjahrtyp;
      }

      set
      {
        if (value.HalbjahrtypBezeichnung == this.halbjahrtyp.HalbjahrtypBezeichnung) return;
        this.UndoablePropertyChanging(this, "HalbjahresplanHalbjahrtyp", this.halbjahrtyp, value);
        this.halbjahrtyp = value;
        this.Model.Halbjahrtyp = value.Model;
        this.RaisePropertyChanged("HalbjahresplanHalbjahrtyp");
      }
    }

    /// <summary>
    /// Holt den currently selected Halbjahresplan Jahrtyp
    /// </summary>
    public JahrtypViewModel HalbjahresplanJahrtyp
    {
      get
      {
        return App.MainViewModel.Jahrtypen.First(o => o.Model == this.Model.Jahresplan.Jahrtyp);
      }
    }

    /// <summary>
    /// Holt den currently selected HalbjahresplanFach
    /// </summary>
    public FachViewModel HalbjahresplanFach
    {
      get
      {
        return App.MainViewModel.Fächer.First(o => o.Model == this.Model.Jahresplan.Fach);
      }
    }

    /// <summary>
    /// Holt den currently selected Halbjahresplan Klasse
    /// </summary>
    public KlasseViewModel HalbjahresplanKlasse
    {
      get
      {
        return App.MainViewModel.Klassen.First(o => o.Model == this.Model.Jahresplan.Klasse);
      }
    }

    public IEnumerable<StundeViewModel> Stunden
    {
      get
      {
        return App.MainViewModel.Stunden.Where(
          o =>
             o.LerngruppenterminSchuljahr == this.HalbjahresplanJahrtyp.JahrtypBezeichnung
          && o.LerngruppenterminHalbjahr == this.HalbjahresplanHalbjahrtyp.HalbjahrtypBezeichnung
          && o.LerngruppenterminKlasse == this.HalbjahresplanKlasse.KlasseBezeichnung
          && o.LerngruppenterminFach == this.HalbjahresplanFach.FachBezeichnung);
      }
    }

    /// <summary>
    /// Holt die Monatspläne für diesen Halbjahresplan
    /// </summary>
    public ObservableCollection<MonatsplanViewModel> Monatspläne { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.HalbjahresplanBezeichnung;
    }

    /// <summary>
    /// Erstellt vordefinierte Stunden an den Tagen, an denen laut Stundenplan
    /// Unterricht stattfindet.
    /// </summary>
    public void PullStunden()
    {
      var stundenpläne = App.MainViewModel.Stundenpläne.Where(
        o => o.StundenplanJahrtyp.Model == this.HalbjahresplanJahrtyp.Model
        && o.StundenplanHalbjahrtyp == this.HalbjahresplanHalbjahrtyp).ToList();

      if (!stundenpläne.Any())
      {
        var dlg = new InformationDialog(
          "Stundenplan fehlt", "Ein Stundenplan ist für das aktuelle" + " Halbjahr noch nicht angelegt", false);
        dlg.ShowDialog();
        return;
      }

      // sort by gültigab date
      stundenpläne.Sort();

      using (new UndoBatch(App.MainViewModel, string.Format("Stunden im Halbjahresplan {0} angelegt.", this.HalbjahresplanBezeichnung), false))
      {
        for (int i = 0; i < stundenpläne.Count; i++)
        {
          var stundenplanViewModel = stundenpläne[i];
          var stundenplaneinträge =
            stundenplanViewModel.Stundenplaneinträge.Where(
              o =>
                 o.StundenplaneintragKlasse == this.HalbjahresplanKlasse
              && o.StundenplaneintragFach == this.HalbjahresplanFach);

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

          foreach (var monatsplanViewModel in this.Monatspläne)
          {
            foreach (var tagesplanViewModel in monatsplanViewModel.Tagespläne)
            {
              foreach (var stundenplaneintragViewModel in stundenplaneinträge)
              {
                if (tagesplanViewModel.TagesplanDatum >= gültigAb && tagesplanViewModel.TagesplanDatum <= gültigBis)
                {
                  if ((int)tagesplanViewModel.TagesplanDatum.DayOfWeek
                      == stundenplaneintragViewModel.StundenplaneintragWochentagIndex)
                  {
                    // skip if we have no unterricht at all
                    if (tagesplanViewModel.TagesplanFerientag)
                    {
                      continue;
                    }

                    // skip if we have already an stunde at this timescale
                    if (
                      tagesplanViewModel.Lerngruppentermine.Any(
                        o =>
                        o.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex
                        <= stundenplaneintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex
                        && o.TerminLetzteUnterrichtsstunde.UnterrichtsstundeIndex
                        >= stundenplaneintragViewModel.StundenplaneintragLetzteUnterrichtsstundeIndex))
                    {
                      continue;
                    }

                    // Now we found the day on which a stunde is found in the stundenplan
                    // and no other stunden already placed
                    // So create a stunde
                    var stunde = new Stunde();
                    stunde.ErsteUnterrichtsstunde =
                      App.MainViewModel.Unterrichtsstunden[
                        stundenplaneintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex - 1].Model;
                    stunde.LetzteUnterrichtsstunde =
                      App.MainViewModel.Unterrichtsstunden[
                        stundenplaneintragViewModel.StundenplaneintragLetzteUnterrichtsstundeIndex - 1].Model;
                    stunde.Tagesplan = tagesplanViewModel.Model;
                    stunde.Termintyp =
                      App.MainViewModel.Termintypen.First(termintyp => termintyp.TermintypBezeichnung == "Unterricht")
                         .Model;
                    stunde.Ort = stundenplaneintragViewModel.StundenplaneintragRaum.RaumBezeichnung;
                    
                    var vm = new StundeViewModel(tagesplanViewModel, stunde);
                    App.MainViewModel.Stunden.Add(vm);
                    tagesplanViewModel.Lerngruppentermine.Add(vm);
                    tagesplanViewModel.UpdateBeschreibung();
                  }
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Tritt auf, wenn die MonatspläneCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void MonatspläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Monatspläne", this.Monatspläne, e, false, "Änderung der Monatspläne");
    }

  }
}
