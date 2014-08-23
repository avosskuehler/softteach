
namespace Liduv.ViewModel.Jahrespläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Globalization;
  using System.Linq;
  using System.Runtime.Remoting.Contexts;
  using System.Windows.Controls;
  using System.Windows.Media;

  using Liduv.Model.EntityFramework;
  using Liduv.UndoRedo;
  using Liduv.View.Jahrespläne;
  using Liduv.View.Termine;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Termine;

  /// <summary>
  /// ViewModel of an individual tagesplan
  /// </summary>
  public class TagesplanViewModel : ViewModelBase
  {
    /// <summary>
    /// The Lerngruppentermin currently selected
    /// </summary>
    private LerngruppenterminViewModel currentLerngruppentermin;

    /// <summary>
    /// The <see cref="ContextMenu"/> for each tagesplan grid.
    /// </summary>
    private ContextMenu tagesplanContextMenu;

    private static Image entwurfIcon;
    private static Image terminIcon;

    static TagesplanViewModel()
    {
      entwurfIcon = App.GetImage("Stundenentwurf16.png");
      terminIcon = App.GetImage("Lerngruppentermin16.png");
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TagesplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="tagesplan">
    /// The underlying tagesplan this ViewModel is to be based on
    /// </param>
    public TagesplanViewModel(Tagesplan tagesplan)
    {
      if (tagesplan == null)
      {
        throw new ArgumentNullException("tagesplan");
      }

      this.Model = tagesplan;

      this.AddStundeCommand = new DelegateCommand(this.AddStunde);
      this.DeleteStundeCommand = new DelegateCommand(this.DeleteCurrentLerngruppentermin, () => this.CurrentLerngruppentermin != null);
      this.AddLerngruppenterminCommand = new DelegateCommand(this.AddLerngruppentermin);
      this.AddSchulterminCommand = new DelegateCommand(this.AddSchultermin);

      // Build data structures for Lerngruppentermine
      this.Lerngruppentermine = new ObservableCollection<LerngruppenterminViewModel>();
      foreach (var lerngruppentermin in tagesplan.Lerngruppentermine)
      {
        if (lerngruppentermin is Stunde)
        {
          var vm = new StundeViewModel(this, lerngruppentermin as Stunde);
          App.MainViewModel.Stunden.Add(vm);
          this.Lerngruppentermine.Add(vm);
        }
        else
        {
          var vm = new LerngruppenterminViewModel(this, lerngruppentermin);
          App.MainViewModel.Lerngruppentermine.Add(vm);
          this.Lerngruppentermine.Add(vm);
        }
      }

      this.Lerngruppentermine.CollectionChanged += this.LerngruppentermineCollectionChanged;

      if (!this.KeineLerngruppentermine)
      {
        this.UpdateBeschreibung();
      }
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Tagesplan
    /// </summary>
    public DelegateCommand AddStundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Tagesplan
    /// </summary>
    public DelegateCommand DeleteStundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Lerngruppentermin
    /// </summary>
    public DelegateCommand AddLerngruppenterminCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Schultermin
    /// </summary>
    public DelegateCommand AddSchulterminCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Tagesplan this ViewModel is based on
    /// </summary>
    public Tagesplan Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected stunde
    /// </summary>
    public LerngruppenterminViewModel CurrentLerngruppentermin
    {
      get
      {
        return this.currentLerngruppentermin;
      }

      set
      {
        this.currentLerngruppentermin = value;
        this.RaisePropertyChanged("CurrentLerngruppentermin");
        this.DeleteStundeCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Datum
    /// </summary>
    public DateTime TagesplanDatum
    {
      get
      {
        return this.Model.Datum;
      }

      set
      {
        if (value == this.Model.Datum) return;
        this.UndoablePropertyChanging(this, "TagesplanDatum", this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("TagesplanDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Tag in den Ferien liegt
    /// </summary>
    public bool TagesplanFerientag
    {
      get
      {
        return this.Model.Ferientag;
      }

      set
      {
        if (value == this.Model.Ferientag) return;
        this.UndoablePropertyChanging(this, "TagesplanFerientag", this.Model.Ferientag, value);
        this.Model.Ferientag = value;
        this.RaisePropertyChanged("TagesplanFerientag");
      }
    }

    /// <summary>
    /// Holt oder setzt die Beschreibung
    /// </summary>
    public string TagesplanBeschreibung
    {
      get
      {
        return this.TagesplanFerientag ? this.Model.Beschreibung : this.CreateBeschreibung();
      }

      set
      {
        if (value == this.Model.Beschreibung) return;
        this.UndoablePropertyChanging(this, "TagesplanBeschreibung", this.Model.Beschreibung, value);
        this.Model.Beschreibung = value;
        this.RaisePropertyChanged("TagesplanBeschreibung");
      }
    }

    /// <summary>
    /// Holt den Wochentag of this Tagesplan
    /// </summary>
    [DependsUpon("TagesplanDatum")]
    public string TagesplanTooltipTitle
    {
      get
      {
        return this.Model.Datum.ToString("ddd dd. MMMM yyyy", new CultureInfo("de-DE"));
      }
    }

    /// <summary>
    /// Holt den Wochentag of this Tagesplan
    /// </summary>
    [DependsUpon("TagesplanDatum")]
    public string TagesplanWochentag
    {
      get
      {
        return this.Model.Datum.ToString("ddd", new CultureInfo("de-DE"));
      }
    }

    /// <summary>
    /// Holt den Wochentag of this Tagesplan
    /// </summary>
    [DependsUpon("TagesplanDatum")]
    public string TagesplanMonat
    {
      get
      {
        return this.Model.Datum.ToString("MMMM", new CultureInfo("de-DE"));
      }
    }

    /// <summary>
    /// Holt den index for the wochentag of this Tagesplan
    /// </summary>
    [DependsUpon("TagesplanDatum")]
    public int TagesplanWochentagIndex
    {
      get
      {
        return (int)this.Model.Datum.DayOfWeek;
      }
    }

    /// <summary>
    /// Holt den TagDesMonats of this Tagesplan
    /// </summary>
    [DependsUpon("TagesplanDatum")]
    public int TagesplanTagDesMonats
    {
      get
      {
        return this.Model.Datum.Day;
      }
    }

    /// <summary>
    /// Holt den Kalenderfarbe of this Tagesplan
    /// </summary>
    [DependsUpon("TagesplanDatum")]
    [DependsUpon("TagesplanFerientag")]
    [DependsUpon("TagesplanBeschreibung")]
    public SolidColorBrush TagesplanKalenderfarbe
    {
      get
      {
        if (this.TagesplanFerientag)
        {
          return Properties.Settings.Default.FerienColor;
        }

        if (this.TagesplanDatum.DayOfWeek == DayOfWeek.Saturday || this.TagesplanDatum.DayOfWeek == DayOfWeek.Sunday)
        {
          return Properties.Settings.Default.WeekendColor;
        }

        // return transparent if this tagesplan has no entries
        if (this.Lerngruppentermine.Count == 0)
        {
          return Brushes.Transparent;
        }

        // return the color of the lerngruppentermin if we have only one
        if (this.Lerngruppentermine.Count == 1)
        {
          return new SolidColorBrush(this.Lerngruppentermine[0].TerminTermintyp.TermintypKalenderfarbe);
        }

        // return gray if we have more than one lerngruppentermin
        return Brushes.Gray;
      }
    }

    /// <summary>
    /// Holt den <see cref="ContextMenu"/> for each tagesplans grid.
    /// </summary>
    public ContextMenu TagesplanContextMenu
    {
      get
      {
        return this.tagesplanContextMenu;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob an diesem Tag Lerngruppentermine vorhanden sind.
    /// </summary>
    [DependsUpon("TagesplanBeschreibung")]
    public bool KeineLerngruppentermine
    {
      get
      {
        return this.Lerngruppentermine.Count == 0;
      }
    }

    /// <summary>
    /// Holt die Stundenanzahl der Fachstunden dieses Tages
    /// </summary>
    public int TagesplanStundenanzahl
    {
      get
      {
        var stunden = this.Lerngruppentermine.Where(o => o is StundeViewModel);
        if (!stunden.Any())
        {
          return 0;
        }

        var stundenzahl = 0;

        foreach (var stundenViewModel in stunden)
        {
          stundenzahl += stundenViewModel.TerminStundenanzahl;
        }

        return stundenzahl;
      }
    }

    /// <summary>
    /// Holt den Tagesplan als breite
    /// </summary>
    [DependsUpon("TagesplanFerientag")]
    [DependsUpon("TagesplanDatum")]
    public int TagesplanBreite
    {
      get
      {
        if (this.TagesplanFerientag)
        {
          return 0;
        }

        if (this.TagesplanDatum.DayOfWeek == DayOfWeek.Saturday || this.TagesplanDatum.DayOfWeek == DayOfWeek.Sunday)
        {
          return 0;
        }

        // return 0 if this tagesplan has no entries
        if (this.Lerngruppentermine.Count == 0)
        {
          return 0;
        }

        return this.TagesplanStundenanzahl * Properties.Settings.Default.Stundenbreite;
      }
    }

    /// <summary>
    /// Holt die Lerngruppentermine für diesen tagesplan
    /// </summary>
    public ObservableCollection<LerngruppenterminViewModel> Lerngruppentermine { get; private set; }

    /// <summary>
    /// Handles deletion of the given Lerngruppentermin
    /// </summary>
    /// <param name="lerngruppenterminViewModel">The lerngruppentermin View Model to be removed
    /// from the tagesplan.</param>
    public void DeleteLerngruppentermin(LerngruppenterminViewModel lerngruppenterminViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Lerngruppentermin {0} löschen", lerngruppenterminViewModel.TerminBeschreibung), false))
      {
        if (lerngruppenterminViewModel is StundeViewModel)
        {
          var success = App.MainViewModel.Stunden.RemoveTest(lerngruppenterminViewModel as StundeViewModel);
        }
        else
        {
          var success = App.MainViewModel.Lerngruppentermine.RemoveTest(lerngruppenterminViewModel);
        }

        var result = this.Lerngruppentermine.RemoveTest(lerngruppenterminViewModel);
        this.CurrentLerngruppentermin = null;
        this.UpdateBeschreibung();
      }
    }

    /// <summary>
    /// Aktualisiert die Beschreibung des Tagesplans.
    /// </summary>
    public void UpdateBeschreibung()
    {
      this.RaisePropertyChanged("TagesplanBeschreibung");
    }

    /// <summary>
    /// Handles addition a new stunde to this tagesplan
    /// </summary>
    public void AddStunde()
    {
      var stunde = new Stunde();
      stunde.ErsteUnterrichtsstunde =
        App.MainViewModel.Unterrichtsstunden.First(
          unterrichtsstunde => unterrichtsstunde.UnterrichtsstundeBezeichnung == "1").Model;
      stunde.LetzteUnterrichtsstunde =
        App.MainViewModel.Unterrichtsstunden.First(
          unterrichtsstunde => unterrichtsstunde.UnterrichtsstundeBezeichnung == "2").Model;
      stunde.Tagesplan = this.Model;
      stunde.Termintyp = App.MainViewModel.Termintypen.First(
          termintyp => termintyp.TermintypBezeichnung == "Unterricht").Model;
      stunde.Tagesplan = this.Model;

      var vm = new StundeViewModel(this, stunde);
      var stundeDlg = new AddStundeDialog(vm);
      if (stundeDlg.ShowDialog().GetValueOrDefault(false))
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Stunde {0} angelegt.", vm), false))
        {
          this.Lerngruppentermine.Add(vm);
          App.MainViewModel.Stunden.Add(vm);
          this.CurrentLerngruppentermin = vm;
        }
      }

      this.UpdateBeschreibung();
    }

    /// <summary>
    /// Kopiert die gegebene Stunde in den Tagesplan hinein
    /// </summary>
    /// <param name="stundeViewModel">Die zu kopierende Stunde</param>
    public void AddStunde(StundeViewModel stundeViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Stunde {0} angelegt.", stundeViewModel), false))
      {
        var stunde = new Stunde();
        stunde.Beschreibung = stundeViewModel.TerminBeschreibung;
        stunde.Termintyp = stundeViewModel.TerminTermintyp.Model;
        stunde.ErsteUnterrichtsstunde = stundeViewModel.TerminErsteUnterrichtsstunde.Model;
        stunde.LetzteUnterrichtsstunde = stundeViewModel.TerminLetzteUnterrichtsstunde.Model;
        stunde.IstGeprüft = stundeViewModel.TerminIstGeprüft;
        stunde.Ort = stundeViewModel.TerminOrt;
        stunde.Tagesplan = this.Model;
        if (stundeViewModel.StundeStundenentwurf != null)
        {
          stunde.Stundenentwurf = stundeViewModel.StundeStundenentwurf.Model;
        }

        var vm = new StundeViewModel(this, stunde);
        App.MainViewModel.Stunden.Add(vm);
        this.Lerngruppentermine.Add(vm);
        this.CurrentLerngruppentermin = vm;

        this.UpdateBeschreibung();
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Tagesplan: " + this.TagesplanDatum.ToShortDateString();
    }

    /// <summary>
    /// Updates the description content of this tagesplan according to lerngruppentermin entries
    /// </summary>
    /// <returns>Eine sinnvolle Beschriftung des Tagesplans</returns>
    private string CreateBeschreibung()
    {
      var beschreibung = string.Empty;

      if (this.Lerngruppentermine.Count == 1)
      {
        var lerngruppenTermin = this.Lerngruppentermine[0];
        var needCut = lerngruppenTermin.TerminBeschreibung.Length > 22;
        beschreibung = needCut ? 
          lerngruppenTermin.TerminBeschreibung.Substring(0, Math.Min(lerngruppenTermin.TerminBeschreibung.Length, 22)) + "..." 
          : lerngruppenTermin.TerminBeschreibung;
      }
      else if (this.Lerngruppentermine.Count > 1)
      {
        beschreibung = "...";
      }

      return beschreibung;
    }

    /// <summary>
    /// Tritt auf, wenn die LerngruppentermineCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void LerngruppentermineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Lerngruppentermine", this.Lerngruppentermine, e, false, "Änderung der Lerngruppentermine");
      this.UpdateBeschreibung();
    }

    /// <summary>
    /// Handles deletion of the current Lerngruppentermin
    /// </summary>
    private void DeleteCurrentLerngruppentermin()
    {
      this.DeleteLerngruppentermin(this.CurrentLerngruppentermin);
    }

    /// <summary>
    /// Handles addition of a new lerngruppentermin to this tagesplan
    /// </summary>
    private void AddLerngruppentermin()
    {
      var dlg = new AddLerngruppenterminDialog();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Lernguppentermin {0} angelegt.", dlg.Terminbezeichnung), false))
        {
          var ersteStunde = dlg.TerminErsteUnterrichtsstunde;
          var letzteStunde = dlg.TerminLetzteUnterrichtsstunde;

          var lerngruppentermin = new Lerngruppentermin();
          lerngruppentermin.Beschreibung = dlg.Terminbezeichnung;
          lerngruppentermin.Termintyp = dlg.TerminTermintyp.Model;
          lerngruppentermin.ErsteUnterrichtsstunde = ersteStunde.Model;
          lerngruppentermin.LetzteUnterrichtsstunde = letzteStunde.Model;
          lerngruppentermin.Tagesplan = this.Model;

          var vm = new LerngruppenterminViewModel(this, lerngruppentermin);
          App.MainViewModel.Lerngruppentermine.Add(vm);
          this.Lerngruppentermine.Add(vm);
          this.CurrentLerngruppentermin = vm;
          this.UpdateBeschreibung();
        }
      }
    }

    private void AddSchultermin()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Schultermine angelegt."), false))
      {
        var dlg = new AddSchulterminDialog();
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          this.UpdateBeschreibung();
        }
      }
    }
  }
}