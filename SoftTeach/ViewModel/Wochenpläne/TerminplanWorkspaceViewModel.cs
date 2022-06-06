namespace SoftTeach.ViewModel.Wochenpläne
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows.Controls;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Termine;
  using SoftTeach.View.Wochenpläne;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// ViewModel of an individual wochenplan
  /// </summary>
  public class TerminplanWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// Das Datum des Montags für diesen Wochenplan.
    /// </summary>
    private DateTime wochenplanMontag;

    /// <summary>
    /// The wochenplaneintraf currently selected
    /// </summary>
    private LerngruppenterminViewModel currentTerminplaneintrag;

    /// <summary>
    /// The <see cref="ContextMenu"/> for each wochenplan grid.
    /// </summary>
    private ContextMenu terminplanContextMenu;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="WochenplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public TerminplanWorkspaceViewModel()
    {
      this.AddVertretungsstundeCommand = new DelegateCommand(this.AddVertretungsstunde);
      this.AddBesprechungCommand = new DelegateCommand(this.AddBesprechung);
      this.AddSonderterminCommand = new DelegateCommand(this.AddSondertermin);

      // Build data structures
      this.Terminplaneinträge = new ObservableCollection<TerminplanEintrag>();
      this.wochenplanMontag = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

      // On saturday oder sunday show next week (to be prepared :-) )
      if ((int)DateTime.Today.DayOfWeek > 5)
      {
        this.wochenplanMontag = this.wochenplanMontag.AddDays(7);
      }

      this.CreateContextMenu();
    }

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
    /// Holt das context menu for the entries of this Wochenplan
    /// </summary>
    public ContextMenu TerminplanContextMenu
    {
      get
      {
        return this.terminplanContextMenu;
      }
    }

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
        this.PopulateTerminplan();
      }
    }

    protected virtual void PopulateTerminplan()
    {

    }

    /// <summary>
    /// Holt oder setzt die currently selected Wochenplaneintrag
    /// </summary>
    public LerngruppenterminViewModel CurrentTerminplaneintrag
    {
      get
      {
        return this.currentTerminplaneintrag;
      }

      set
      {
        this.currentTerminplaneintrag = value;
        this.RaisePropertyChanged("CurrentWochenplaneintrag");
      }
    }

    /// <summary>
    /// Holt die Bezeichnung der ersten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan1Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[0].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der ersten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan1Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[0].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der zweiten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan2Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[1].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der zweiten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan2Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[1].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der dritten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan3Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[2].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der dritten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan3Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[2].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der vierten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan4Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[3].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der vierten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan4Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[3].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der fünften Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan5Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[4].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der fünften Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan5Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[4].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der sechsten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan6Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[5].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der sechsten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan6Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[5].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der siebten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan7Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[6].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der siebten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan7Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[6].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der achten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan8Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[7].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der achten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan8Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[7].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der nten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan9Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[8].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der nten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan9Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[8].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die Bezeichnung der zehnten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan10Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[9].UnterrichtsstundeBezeichnung + ". Stunde"; }
    }

    /// <summary>
    /// Holt den Zeitraum der zehnten Unterrichtsstunde.
    /// </summary>
    public static string Wochenplan10Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[9].UnterrichtsstundeZeitraum; }
    }

    /// <summary>
    /// Holt die WochenplanEinträge for this wochenplan
    /// </summary>
    public ObservableCollection<TerminplanEintrag> Terminplaneinträge { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "TerminplanWorkspace";
    }

    /// <summary>
    /// Löscht den gegebenen TerminplanEintrag
    /// </summary>
    /// <param name="terminplanEintrag">Der TerminplanEintrag zum Löschen</param>
    public void RemoveTerminplaneintrag(TerminplanEintrag terminplanEintrag)
    {
      this.Terminplaneinträge.RemoveTest(terminplanEintrag);
    }

    /// <summary>
    /// Fügt eine Vertretungssstunde hinzu
    /// </summary>
    private void AddVertretungsstunde()
    {
      var vertretungsLerngruppe = this.GetVertretungsLerngruppe();
      if (vertretungsLerngruppe == null)
      {
        return;
      }

      var stunde = new Stunde();
      var ersteStunde = App.MainViewModel.Unterrichtsstunden.FirstOrDefault(o => o.UnterrichtsstundeIndex == WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex).Model;
      if (ersteStunde == null)
      {
        return;
      }
      stunde.ErsteUnterrichtsstunde = ersteStunde;
      var letzteStunde = App.MainViewModel.Unterrichtsstunden.FirstOrDefault(o => o.UnterrichtsstundeIndex == WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex).Model;
      if (letzteStunde == null)
      {
        return;
      }
      stunde.LetzteUnterrichtsstunde = letzteStunde;

      stunde.Lerngruppe = vertretungsLerngruppe.Model;
      stunde.Termintyp = Termintyp.Vertretung;
      stunde.Datum = this.wochenplanMontag.AddDays(WochenplanSelection.Instance.WochentagIndex - 1);

      var vm = new StundeViewModel(stunde);

      var stundeDlg = new EditStundeDialog(vm);
      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Vertretungsstunde {0} angelegt.", vm), false))
      {
        if (!(undo = !stundeDlg.ShowDialog().GetValueOrDefault(false)))
        {
          //App.MainViewModel.Stunden.Add(vm);
          //App.UnitOfWork.Context.Termine.Add(stunde);
          vertretungsLerngruppe.Lerngruppentermine.Add(vm);

          var wochenplanEintrag = new TerminplanEintrag(this, vm);
          if (!this.Terminplaneinträge.Contains(wochenplanEintrag, new TerminplanEintragEqualityComparer()))
          {
            this.Terminplaneinträge.Add(wochenplanEintrag);
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
      this.AddSpecialLerngruppentermin(Termintyp.Besprechung);
    }

    /// <summary>
    /// Fügt einen Sondertermin hinzu
    /// </summary>
    private void AddSondertermin()
    {
      this.AddSpecialLerngruppentermin(Termintyp.Sondertermin);
    }

    /// <summary>
    /// Fügt einen Sondertermin oder eine Besprechung ein (Lerngruppentermine)
    /// </summary>
    /// <param name="termintyp">Die Bezeichnung des Termins.</param>
    private void AddSpecialLerngruppentermin(Termintyp termintyp)
    {
      var dlg = new AddSonderterminDialog
      {
        TerminTermintyp = termintyp
      };

      bool undo;
      if (!(undo = !dlg.ShowDialog().GetValueOrDefault(false)))
      {
        var vertretungslerngruppe = this.GetVertretungsLerngruppe();
        if (vertretungslerngruppe == null)
        {
          return;
        }

        var lerngruppentermin = new Lerngruppentermin
        {
          Beschreibung = dlg.TerminBeschreibung,
          ErsteUnterrichtsstunde =
          App.MainViewModel.Unterrichtsstunden.First(
            o => o.UnterrichtsstundeIndex == WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex).Model,
          LetzteUnterrichtsstunde =
          App.MainViewModel.Unterrichtsstunden.First(
            o => o.UnterrichtsstundeIndex == WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex).Model,
          Lerngruppe = vertretungslerngruppe.Model,
          Termintyp = termintyp,
          Ort = dlg.TerminOrt,
          Datum = this.wochenplanMontag.AddDays(WochenplanSelection.Instance.WochentagIndex - 1)
        };

        var vm = new LerngruppenterminViewModel(lerngruppentermin);
        using (new UndoBatch(App.MainViewModel, string.Format("Sondertermin {0} angelegt.", vm), false))
        {
          //App.UnitOfWork.Context.Termine.Add(lerngruppentermin);
          //App.MainViewModel.Lerngruppentermine.Add(vm);
          vertretungslerngruppe.Lerngruppentermine.Add(vm);

          var wochenplanEintrag = new TerminplanEintrag(this, vm);
          if (!this.Terminplaneinträge.Contains(wochenplanEintrag, new TerminplanEintragEqualityComparer()))
          {
            this.Terminplaneinträge.Add(wochenplanEintrag);
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
    private LerngruppeViewModel GetVertretungsLerngruppe()
    {
      // The day to add the stunde to
      var date = this.wochenplanMontag.AddDays(WochenplanSelection.Instance.WochentagIndex - 1);

      Halbjahr halbjahr;
      int jahresplanJahr;
      this.GetJahrAndHalbjahr(out halbjahr, out jahresplanJahr);

      var vertretungsjahresplan =
        App.MainViewModel.Lerngruppen.SingleOrDefault(
          o => o.LerngruppeFach.FachBezeichnung == "Vertretungsstunden" && o.LerngruppeSchuljahr.SchuljahrJahr == jahresplanJahr);

      if (vertretungsjahresplan == null)
      {
        InformationDialog.Show("Jahresplan fehlt", "Ein Jahresplan für das Fach Vertretungsstunden fehlt, daher können keine Vertretungsstunden, Sondertermine, etc. angelegt werden. Bitte zuerst den Jahresplan anlegen.", false);
        return null;
      }

      return vertretungsjahresplan;
    }

    /// <summary>
    /// Holt die Ganztagstermine für den gegebenen Wochentag.
    /// </summary>
    /// <param name="wochentagIndex">Der Index des Wochentags</param>
    /// <returns>Ein <see cref="GanztagstermineViewModel"/> mit allen Ganztagsterminen.</returns>
    protected GanztagstermineViewModel GetGanztagstermine(int wochentagIndex)
    {
      // Get all entries which include the whole day
      var ganztagViewModels =
        this.Terminplaneinträge.Where(eintrag => eintrag.WochentagIndex == wochentagIndex
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

    protected TerminplanEintragCollection GetWochenplanEintragCollection(int wochentagIndex, int stundeIndex)
    {
      try
      {
        if (stundeIndex > 1)
        {
          var previousLerngruppenterminViewModel =
            this.Terminplaneinträge.Where(
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
        var wochenplanEinträge = this.Terminplaneinträge.Where(
            eintrag =>
              eintrag.WochentagIndex == wochentagIndex
              && eintrag.ErsteUnterrichtsstundeIndex == stundeIndex && eintrag.Stundenanzahl < 7).ToList();

        if (!wochenplanEinträge.Any())
        {
          var emptyEintrag = new TerminplanEintrag(this, null)
          {
            ErsteUnterrichtsstundeIndex = stundeIndex,
            LetzteUnterrichtsstundeIndex = stundeIndex,
            WochentagIndex = wochentagIndex
          };
          var emptyList = new List<TerminplanEintrag> { emptyEintrag };
          var emptyCollection = new TerminplanEintragCollection(wochentagIndex, emptyList);
          return emptyCollection;
        }

        for (var i = 0; i < wochenplanEinträge.Count(); i++)
        {
          wochenplanEinträge[i].ColumnIndex = i;
        }

        return new TerminplanEintragCollection(wochentagIndex, wochenplanEinträge.ToList());
      }
      catch (Exception)
      {
        InformationDialog.Show("Mehrere Wochenplaneinträge", "Mehrere parallele Wochenplaneinträge gefunden!", false);
      }

      return null;
    }

    protected int Stundenanzahl(int wochentagIndex, int stundeIndex)
    {
      var wochenplaneintragViewModel =
        this.Terminplaneinträge.Where(
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
      this.terminplanContextMenu = new ContextMenu();

      var addVertretungsstundeItem = new MenuItem
      {
        Header = "Vertretungsstunde anlegen",
        Command = this.AddVertretungsstundeCommand,
        Icon = App.GetIcon("Vertretung16")
      };
      this.terminplanContextMenu.Items.Add(addVertretungsstundeItem);

      var addBesprechungItem = new MenuItem
      {
        Header = "Besprechung anlegen",
        Command = this.AddBesprechungCommand,
        Icon = App.GetIcon("Besprechung16")
      };
      this.terminplanContextMenu.Items.Add(addBesprechungItem);

      var addSonderterminItem = new MenuItem
      {
        Header = "Sondertermin anlegen",
        Command = this.AddSonderterminCommand,
        Icon = App.GetIcon("Sondertermin16")
      };
      this.terminplanContextMenu.Items.Add(addSonderterminItem);
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

    protected void GetJahrAndHalbjahr(out Halbjahr halbjahr, out int jahresplanJahr)
    {
      // Get StundenFrom Jahresplänen
      if (this.wochenplanMontag.Month >= 8)
      {
        jahresplanJahr = this.wochenplanMontag.Year;
        halbjahr = Halbjahr.Winter;
      }
      else
      {
        jahresplanJahr = this.wochenplanMontag.Year - 1;
        if (this.wochenplanMontag.Month == 1)
        {
          var date = this.wochenplanMontag.AddDays(WochenplanSelection.Instance.WochentagIndex - 1);

          if (date.Month > 1)
          {
            halbjahr = Halbjahr.Sommer;
          }
          else
          {
            halbjahr = Halbjahr.Winter;
          }
        }
        else
        {
          halbjahr = Halbjahr.Sommer;
        }
      }
    }
  }
}
