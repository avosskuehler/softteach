namespace SoftTeach.ViewModel.Jahrespläne
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using Microsoft.Win32;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Curricula;
  using SoftTeach.View.Jahrespläne;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Helper.ODSSupport;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// ViewModel of an individual jahresplan
  /// </summary>
  public class JahresplanViewModel : ViewModelBase
  {
    /// <summary>
    /// The jahrtyp currently assigned to this jahresplan
    /// </summary>
    private JahrtypViewModel jahrtyp;

    /// <summary>
    /// The fach currently assigned to this jahresplan
    /// </summary>
    private FachViewModel fach;

    /// <summary>
    /// The klasse currently assigned to this jahresplan
    /// </summary>
    private KlasseViewModel klasse;

    /// <summary>
    /// The halbjahresplan currently selected
    /// </summary>
    private HalbjahresplanViewModel currentHalbjahresplan;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahresplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="jahresplan">
    /// The underlying jahresplan this ViewModel is to be based on
    /// </param>
    public JahresplanViewModel(Jahresplan jahresplan)
    {
      if (jahresplan == null)
      {
        throw new ArgumentNullException("jahresplan");
      }

      this.Model = jahresplan;
      this.PullStundenCommand = new DelegateCommand(
        () => { if (this.CurrentHalbjahresplan != null) this.CurrentHalbjahresplan.PullStunden(); },
        () => this.CurrentHalbjahresplan != null);
      this.RemoveStundenCommand = new DelegateCommand(this.RemoveStunden, () => this.CurrentHalbjahresplan != null);
      this.AddWinterHalbjahresplanCommand = new DelegateCommand(this.AddWinterHalbjahresplan, () => this.CurrentJahresplanWinterhalbjahr == null);
      this.AddSommerHalbjahresplanCommand = new DelegateCommand(this.AddSommerHalbjahresplan, () => this.CurrentJahresplanSommerhalbjahr == null);
      this.DeleteHalbjahresplanCommand = new DelegateCommand(this.DeleteCurrentHalbjahresplan, () => this.CurrentHalbjahresplan != null);
      this.GetStundenFromOtherHalbjahresplanCommand = new DelegateCommand(this.GetStundenFromOtherHalbjahresplan, () => this.CurrentHalbjahresplan != null);
      this.StundenAlsOdsExportierenCommand = new DelegateCommand(this.StundenAlsOdsExportieren);

      // Build data structures for phasen
      this.Halbjahrespläne = new ObservableCollection<HalbjahresplanViewModel>();
      foreach (var halbjahresplan in jahresplan.Halbjahrespläne)
      {
        var vm = new HalbjahresplanViewModel(halbjahresplan);
        //App.MainViewModel.Halbjahrespläne.Add(vm);
        this.Halbjahrespläne.Add(vm);
      }

      this.Halbjahrespläne.CollectionChanged += this.HalbjahrespläneCollectionChanged;

      if (this.Halbjahrespläne.Count > 0)
      {
        this.CurrentHalbjahresplan = this.Halbjahrespläne[0];
      }
    }

    /// <summary>
    /// Holt den Befehl zur getting the stunden for this jahresplan from a stundenplan
    /// </summary>
    public DelegateCommand PullStundenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl alle Stunden aus dem Halbjahresplan zu löschen
    /// </summary>
    public DelegateCommand RemoveStundenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new winter Halbjahresplan
    /// </summary>
    public DelegateCommand AddWinterHalbjahresplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new sommer Halbjahresplan
    /// </summary>
    public DelegateCommand AddSommerHalbjahresplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Halbjahresplan
    /// </summary>
    public DelegateCommand DeleteHalbjahresplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um die Stunden aus einem alten Halbjahresplan zu holen
    /// </summary>
    public DelegateCommand GetStundenFromOtherHalbjahresplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um die Stunden als ODS zu exportieren
    /// </summary>
    public DelegateCommand StundenAlsOdsExportierenCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Jahresplan this ViewModel is based on
    /// </summary>
    public Jahresplan Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected halbjahresplan
    /// </summary>
    public HalbjahresplanViewModel CurrentHalbjahresplan
    {
      get
      {
        return this.currentHalbjahresplan;
      }

      set
      {
        this.currentHalbjahresplan = value;
        this.RaisePropertyChanged("CurrentHalbjahresplan");
        this.DeleteHalbjahresplanCommand.RaiseCanExecuteChanged();
        this.PullStundenCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt den currently selected halbjahresplan
    /// </summary>
    public HalbjahresplanViewModel CurrentJahresplanWinterhalbjahr
    {
      get
      {
        return this.Halbjahrespläne.FirstOrDefault(plan => plan.HalbjahresplanHalbjahrtyp.HalbjahrtypBezeichnung.StartsWith("Winter"));
      }
    }

    /// <summary>
    /// Holt den currently selected halbjahresplan
    /// </summary>
    public HalbjahresplanViewModel CurrentJahresplanSommerhalbjahr
    {
      get
      {
        return this.Halbjahrespläne.FirstOrDefault(plan => plan.HalbjahresplanHalbjahrtyp.HalbjahrtypBezeichnung.StartsWith("Sommer"));
      }
    }

    ///// <summary>
    ///// Holt oder setzt die jahrgangsstufe filter for the module list.
    ///// </summary>
    //public string JahresplanHeader
    //{
    //  get { return (string)this.GetValue(JahresplanHeaderProperty); }
    //  set { this.SetValue(JahresplanHeaderProperty, value); }
    //}

    /// <summary>
    /// Holt die Bezeichnung des Jahresplans
    /// </summary>
    public string JahresplanBezeichnung
    {
      get
      {
        return string.Format(
          "Jahresplan {0}, {1}, für Lerngruppe {2}",
          this.JahresplanFach != null ? this.JahresplanFach.FachBezeichnung : string.Empty,
          this.JahresplanJahrtyp != null ? this.JahresplanJahrtyp.JahrtypBezeichnung : string.Empty,
          this.JahresplanKlasse != null ? this.JahresplanKlasse.KlasseBezeichnung : "?");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Jahresplan
    /// </summary>
    public FachViewModel JahresplanFach
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Fach == null)
        {
          return null;
        }

        if (this.fach == null || this.fach.Model != this.Model.Fach)
        {
          this.fach = App.MainViewModel.Fächer.SingleOrDefault(d => d.Model == this.Model.Fach);
        }

        return this.fach;
      }

      set
      {
        if (value.FachBezeichnung == this.fach.FachBezeichnung) return;
        this.UndoablePropertyChanging(this, "JahresplanFach", this.fach, value);
        this.fach = value;
        this.Model.Fach = value.Model;
        this.RaisePropertyChanged("JahresplanFach");
      }
    }

    /// <summary>
    /// Holt oder setzt die Klasse currently assigned to this Jahresplan
    /// </summary>
    public KlasseViewModel JahresplanKlasse
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Klasse == null)
        {
          return null;
        }

        if (this.klasse == null || this.klasse.Model != this.Model.Klasse)
        {
          this.klasse = App.MainViewModel.Klassen.SingleOrDefault(d => d.Model == this.Model.Klasse);
        }

        return this.klasse;
      }

      set
      {
        if (value.KlasseBezeichnung == this.klasse.KlasseBezeichnung) return;
        this.UndoablePropertyChanging(this, "JahresplanKlasse", this.klasse, value);
        this.klasse = value;
        this.Model.Klasse = value.Model;
        this.RaisePropertyChanged("JahresplanKlasse");
      }
    }

    /// <summary>
    /// Holt oder setzt die Jahrgangsstufe currently assigned to this Jahresplan
    /// </summary>
    public JahrgangsstufeViewModel JahresplanJahrgangsstufe
    {
      get
      {
        return
          App.MainViewModel.Jahrgangsstufen.SingleOrDefault(
            d => d.Model == this.JahresplanKlasse.Model.Klassenstufe.Jahrgangsstufe);
      }
    }

    /// <summary>
    /// Holt oder setzt die Jahrtyp currently assigned to this Jahresplan
    /// </summary>
    public JahrtypViewModel JahresplanJahrtyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Jahrtyp == null)
        {
          return null;
        }

        if (this.jahrtyp == null || this.jahrtyp.Model != this.Model.Jahrtyp)
        {
          this.jahrtyp = App.MainViewModel.Jahrtypen.SingleOrDefault(d => d.Model == this.Model.Jahrtyp);
        }

        return this.jahrtyp;
      }

      set
      {
        if (value.JahrtypBezeichnung == this.jahrtyp.JahrtypBezeichnung) return;
        this.UndoablePropertyChanging(this, "JahresplanJahrtyp", this.jahrtyp, value);
        this.jahrtyp = value;
        this.Model.Jahrtyp = value.Model;
        this.RaisePropertyChanged("JahresplanJahrtyp");
      }
    }

    /// <summary>
    /// Holt den halbjahrespläne for this jahresplan
    /// </summary>
    public ObservableCollection<HalbjahresplanViewModel> Halbjahrespläne { get; private set; }

    /// <summary>
    /// Handles addition a new Winterhalbjahresplan to this jahresplan
    /// </summary>
    public void AddWinterHalbjahresplan()
    {
      // Check for existing Halbjahresplan
      if (
        this.Halbjahrespläne.Any(
          vorhandenerHalbjahresplan =>
          vorhandenerHalbjahresplan.HalbjahresplanHalbjahrtyp.HalbjahrtypBezeichnung.StartsWith("Winter")))
      {
        Log.ProcessMessage("Winterhalbjahr vorhanden", "Das Winterhalbjahr ist bereits im " + "aktuellen Jahr angelegt");
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Winterhalbjahr für Jahresplan {0} erstellt", this.JahresplanBezeichnung), false))
      {
        var halbjahresplan = new Halbjahresplan();

        // Set to winter
        halbjahresplan.Halbjahrtyp = App.MainViewModel.Halbjahrtypen[0].Model;
        halbjahresplan.Jahresplan = this.Model;
        var vm = new HalbjahresplanViewModel(halbjahresplan);
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
        this.AddMonatspläne(vm);
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
        //App.MainViewModel.Halbjahrespläne.Add(vm);
        this.Halbjahrespläne.Add(vm);
        this.CurrentHalbjahresplan = vm;
      }
    }

    /// <summary>
    /// Handles addition a new Sommerhalbjahresplan to this jahresplan
    /// </summary>
    public void AddSommerHalbjahresplan()
    {
      // Check for existing Halbjahresplan
      if (this.Halbjahrespläne.Any(vorhandenerHalbjahresplan => vorhandenerHalbjahresplan.HalbjahresplanHalbjahrtyp.HalbjahrtypBezeichnung.StartsWith("Sommer")))
      {
        Log.ProcessMessage("Sommerhalbjahr vorhanden", "Das Sommerhalbjahr ist bereits im " + "aktuellen Jahr angelegt");
        return;
      }

      using (
        new UndoBatch(
          App.MainViewModel,
          string.Format("Sommerhalbjahr für Jahresplan {0} erstellt", this.JahresplanBezeichnung),
          false))
      {
        var halbjahresplan = new Halbjahresplan();

        // Set to Sommer
        halbjahresplan.Halbjahrtyp = App.MainViewModel.Halbjahrtypen[1].Model;
        halbjahresplan.Jahresplan = this.Model;
        var vm = new HalbjahresplanViewModel(halbjahresplan);
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
        this.AddMonatspläne(vm);
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
        //App.MainViewModel.Halbjahrespläne.Add(vm);
        this.Halbjahrespläne.Add(vm);
        this.CurrentHalbjahresplan = vm;
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.JahresplanBezeichnung;
    }

    /// <summary>
    /// Diese Methode gibt für den aktuellen Jahresplan den Tagesplan des gegebenen Datums zurück.
    /// </summary>
    /// <param name="date">Das Datum, für das der Tagesplan gesucht wird.</param>
    /// <returns>Ein Tagesplan des gegebenen Datums aus dem aktuellen Jahresplan</returns>
    public TagesplanViewModel GetTagesplanByDate(DateTime date)
    {
      Dictionary<string, MonatsplanViewModel> monatsPläne;
      ObservableCollection<TagesplanViewModel> tage = null;
      IEnumerable<TagesplanViewModel> gefundenerTagesplan = null;

      var halbJahresPläne =
        this.Halbjahrespläne.ToDictionary(halbjahrViewModel => halbjahrViewModel.HalbjahresplanKurbezeichnung);

      if (halbJahresPläne.ContainsKey("Sommer"))
      {
        var sommerPlan = halbJahresPläne["Sommer"];
        switch (date.Month)
        {
          case 2:
            monatsPläne = sommerPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["Februar"].Tagespläne;
            break;
          case 3:
            monatsPläne = sommerPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["März"].Tagespläne;
            break;
          case 4:
            monatsPläne = sommerPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["April"].Tagespläne;
            break;
          case 5:
            monatsPläne = sommerPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["Mai"].Tagespläne;
            break;
          case 6:
            monatsPläne = sommerPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["Juni"].Tagespläne;
            break;
          case 7:
            monatsPläne = sommerPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["Juli"].Tagespläne;
            break;
        }
      }

      if (halbJahresPläne.ContainsKey("Winter"))
      {
        var winterPlan = halbJahresPläne["Winter"];
        switch (date.Month)
        {
          case 1:
            monatsPläne = winterPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["Januar"].Tagespläne;
            break;
          case 8:
            monatsPläne = winterPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["August"].Tagespläne;
            break;
          case 9:
            monatsPläne = winterPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["September"].Tagespläne;
            break;
          case 10:
            monatsPläne = winterPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["Oktober"].Tagespläne;
            break;
          case 11:
            monatsPläne = winterPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["November"].Tagespläne;
            break;
          case 12:
            monatsPläne = winterPlan.Monatspläne.ToDictionary(monatsViewModel => monatsViewModel.MonatsplanMonatstyp.MonatstypBezeichnung);
            tage = monatsPläne["Dezember"].Tagespläne;
            break;
        }
      }

      if (tage != null)
      {
        gefundenerTagesplan = tage.Where(tagesplan => tagesplan.TagesplanDatum.Date == date.Date);
      }

      if (gefundenerTagesplan == null)
      {
        return null;
      }

      if (gefundenerTagesplan.Any())
      {
        return gefundenerTagesplan.Single();
      }

      return null;
    }

    /// <summary>
    /// Tritt auf, wenn die HalbjahrespläneCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void HalbjahrespläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Halbjahrespläne", this.Halbjahrespläne, e, false, "Änderung der Halbjahrespläne");
    }

    /// <summary>
    /// Handles deletion of the current Halbjahresplan
    /// </summary>
    private void DeleteCurrentHalbjahresplan()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Halbjahresplan {0} gelöscht", this.CurrentHalbjahresplan), false))
      {
        //App.MainViewModel.Halbjahrespläne.RemoveTest(this.CurrentHalbjahresplan);
        App.UnitOfWork.Context.Halbjahrespläne.Remove(this.CurrentHalbjahresplan.Model);
        this.Halbjahrespläne.RemoveTest(this.CurrentHalbjahresplan);
        if (this.Halbjahrespläne.Count > 0)
        {
          this.CurrentHalbjahresplan = this.Halbjahrespläne[0];
        }
      }
    }

    /// <summary>
    /// Handles addition a new Halbjahresplan to this jahresplan
    /// </summary>
    /// <param name="halbjahresplan"> The halbjahresplan.</param>
    private void AddMonatspläne(HalbjahresplanViewModel halbjahresplan)
    {
      int monatsstartIndex = 0;
      if (halbjahresplan.HalbjahresplanHalbjahrtyp.HalbjahrtypBezeichnung == "Sommer")
      {
        monatsstartIndex = 6;
      }

      for (int i = 0; i < 6; i++)
      {
        var monatsplan = new Monatsplan();
        monatsplan.Monatstyp = App.MainViewModel.Monatstypen[monatsstartIndex + i].Model;
        monatsplan.Halbjahresplan = halbjahresplan.Model;
        App.UnitOfWork.Context.Monatspläne.Add(monatsplan);

        var vm = new MonatsplanViewModel(monatsplan);
        this.AddTagespläne(vm);
        //App.MainViewModel.Monatspläne.Add(vm);
        halbjahresplan.Monatspläne.Add(vm);
      }
    }

    /// <summary>
    /// Fügt alle Tagespläne dem gegebenen Monatsplan hinzu.
    /// </summary>
    /// <param name="monatsplan"> The monatsplan. </param>
    private void AddTagespläne(MonatsplanViewModel monatsplan)
    {
      var year = monatsplan.MonatsplanJahr;
      var month = monatsplan.MonatsplanMonatindex;
      if (month <= 7)
      {
        year++;
      }

      var monatsTage = DateTime.DaysInMonth(year, month);
      var aktuelleFerien = string.Empty;

      for (int i = 1; i <= monatsTage; i++)
      {
        var tagesplan = new Tagesplan();
        var datum = new DateTime(year, month, i);
        tagesplan.Datum = datum;
        tagesplan.Monatsplan = monatsplan.Model;
        App.UnitOfWork.Context.Tagespläne.Add(tagesplan);

        // Check for Ferien
        foreach (var ferien in App.MainViewModel.Ferien.Where(
          schuljahr => schuljahr.FerienJahrtyp.Model == tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Jahrtyp))
        {
          if (datum >= ferien.FerienErsterFerientag && datum <= ferien.FerienLetzterFerientag)
          {
            tagesplan.Ferientag = true;
            if (aktuelleFerien != ferien.FerienBezeichnung)
            {
              aktuelleFerien = ferien.FerienBezeichnung;
              tagesplan.Beschreibung = ferien.FerienBezeichnung;
            }
          }
        }

        // Check for Termine des Jahres und Tages and if the klasse of this jahresplan is
        // a betroffene klasse of this termin
        var schultermine = App.MainViewModel.Schultermine.Where(o => o is SchulterminViewModel);
        var termineJahrTag = schultermine.Where(
            o =>
            o.SchulterminJahrtyp.Model == tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Jahrtyp
             && o.SchulterminDatum == datum.Date
             && o.BetroffeneKlassen.Any(betroffeneKlasse => betroffeneKlasse.Model.Klasse == monatsplan.Model.Halbjahresplan.Jahresplan.Klasse));

        if (termineJahrTag.Count() == 1)
        {
          var termin = termineJahrTag.First();
          var termintyp = termin.TerminTermintyp;
        }
        else if (termineJahrTag.Count() > 1)
        {
        }

        var vm = new TagesplanViewModel(tagesplan);
        foreach (var terminViewModel in termineJahrTag)
        {
          var lerngruppentermin = new Lerngruppentermin();
          lerngruppentermin.Beschreibung = terminViewModel.TerminBeschreibung;
          lerngruppentermin.ErsteUnterrichtsstunde = terminViewModel.TerminErsteUnterrichtsstunde.Model;
          lerngruppentermin.LetzteUnterrichtsstunde = terminViewModel.TerminLetzteUnterrichtsstunde.Model;
          lerngruppentermin.Termintyp = terminViewModel.TerminTermintyp.Model;
          lerngruppentermin.Ort = terminViewModel.TerminOrt;
          lerngruppentermin.Tagesplan = tagesplan;
          App.UnitOfWork.Context.Termine.Add(lerngruppentermin);

          var viewModelLerngruppentermin = new LerngruppenterminViewModel(lerngruppentermin);
          //App.MainViewModel.Lerngruppentermine.Add(viewModelLerngruppentermin);
          vm.Lerngruppentermine.Add(viewModelLerngruppentermin);
        }

        //App.MainViewModel.Tagespläne.Add(vm);
        monatsplan.Tagespläne.Add(vm);
      }
    }

    /// <summary>
    /// Löscht alle Stunden aus dem Jahresplan
    /// </summary>
    private void RemoveStunden()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Stunden im Jahresplan {0} gelöscht.", this.JahresplanBezeichnung), false))
      {
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
        foreach (var halbjahr in this.Halbjahrespläne.Where(o => o.HalbjahresplanHalbjahrtyp == this.CurrentHalbjahresplan.HalbjahresplanHalbjahrtyp))
        {
          foreach (var monat in halbjahr.Monatspläne)
          {
            foreach (var tag in monat.Tagespläne)
            {
              var stunden = tag.Lerngruppentermine.Where(o => o is StundeViewModel).ToList();
              foreach (var lerngruppenterminViewModel in stunden)
              {
                var stunde = lerngruppenterminViewModel as StundeViewModel;
                if (stunde.StundeStundenentwurf != null
                    && stunde.StundeStundenentwurf.StundenentwurfPhasenKurzform == string.Empty)
                {
                  App.MainViewModel.Stundenentwürfe.RemoveTest(stunde.StundeStundenentwurf);
                }

                App.UnitOfWork.Context.Termine.Remove(stunde.Model);
                tag.Lerngruppentermine.RemoveTest(lerngruppenterminViewModel);
                //App.MainViewModel.Stunden.RemoveTest(lerngruppenterminViewModel as StundeViewModel);
                tag.UpdateBeschreibung();
              }
            }
          }
        }
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
      }
    }

    /// <summary>
    /// Holt die Stundenentwürfe aus einem anderen Halbjahresplan.
    /// </summary>
    private void GetStundenFromOtherHalbjahresplan()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Stunden im Halbjahresplan {0} importiert.", this.JahresplanBezeichnung), false))
      {
        var dlg = new AskForHalbjahresplanToAdaptDialog(
          this.JahresplanFach,
          App.MainViewModel.Klassenstufen.First(
            o => o.KlassenstufeBezeichnung == this.JahresplanKlasse.Model.Klassenstufe.Bezeichnung),
          this.CurrentHalbjahresplan.HalbjahresplanHalbjahrtyp);
        dlg.Title = "Aus welchem Halbjahresplan sollen die Stunden übertragen werden?";
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          var halbjahresplanZuweisenWorkspace = new HalbjahresplanZuweisenWorkspaceViewModel(
            dlg.Halbjahresplan, this.CurrentHalbjahresplan);
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
        new OdsReaderWriter().WriteOdsFile(this, fileDialog.FileName);
      }

    }
  }
}
