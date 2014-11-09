namespace Liduv.ViewModel.Stundenpläne
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows.Controls;
  using System.Windows.Input;

  using Liduv.Model.EntityFramework;
  using Liduv.UndoRedo;
  using Liduv.View.Stundenpläne;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual stundenplan
  /// </summary>
  public class StundenplanViewModel : ViewModelBase, IComparable, ICloneable
  {
    /// <summary>
    /// The jahrtyp currently assigned to this stundenplan
    /// </summary>
    private JahrtypViewModel jahrtyp;

    /// <summary>
    /// The halbjahrtyp assigned to this stundenplan
    /// </summary>
    private HalbjahrtypViewModel halbjahrtyp;

    /// <summary>
    /// The stundenplaneintraf currently selected
    /// </summary>
    private StundenplaneintragViewModel currentStundenplaneintrag;

    /// <summary>
    /// The <see cref="ContextMenu"/> for each stundenplan grid.
    /// </summary>
    private ContextMenu stundenplanContextMenu;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="stundenplan">
    /// The underlying stundenplan this ViewModel is to be based on
    /// </param>
    public StundenplanViewModel(Stundenplan stundenplan)
    {
      if (stundenplan == null)
      {
        throw new ArgumentNullException("stundenplan");
      }

      this.Model = stundenplan;
      this.AddStundenplaneintragCommand = new DelegateCommand(this.AddStundenplaneintrag);
      this.DeleteStundenplaneintragCommand = new DelegateCommand(this.DeleteCurrentStundenplaneintrag, () => this.CurrentStundenplaneintrag != null);
      this.ÄnderungsListe = new List<StundenplanÄnderung>();

      // Build data structures for phasen
      this.Stundenplaneinträge = new ObservableCollection<StundenplaneintragViewModel>();
      foreach (var stundenplaneintrag in stundenplan.Stundenplaneinträge)
      {
        var vm = new StundenplaneintragViewModel(this, stundenplaneintrag);
        App.MainViewModel.Stundenplaneinträge.Add(vm);
        this.Stundenplaneinträge.Add(vm);
      }

      this.Stundenplaneinträge.CollectionChanged += this.StundenplaneinträgeCollectionChanged;
      this.CreateContextMenu();
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Stundenplaneintrag
    /// </summary>
    public DelegateCommand AddStundenplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Stundenplaneintrag
    /// </summary>
    public DelegateCommand DeleteStundenplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Stundenplan this ViewModel is based on
    /// </summary>
    public Stundenplan Model { get; private set; }

    /// <summary>
    /// Holt oder setzt a value containing the flags for the view mode
    /// for this stundenplan.
    /// </summary>
    public StundenplanViewMode ViewMode { get; set; }

    /// <summary>
    /// Holt oder setzt die list of changes during edit of this stundenplan
    /// </summary>
    public List<StundenplanÄnderung> ÄnderungsListe { get; set; }

    /// <summary>
    /// Holt das context menu for the entries of this Stundenplan
    /// </summary>
    public ContextMenu StundenplanContextMenu
    {
      get
      {
        return this.stundenplanContextMenu;
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected Stundenplaneintrag
    /// </summary>
    public StundenplaneintragViewModel CurrentStundenplaneintrag
    {
      get
      {
        return this.currentStundenplaneintrag;
      }

      set
      {
        this.currentStundenplaneintrag = value;
        this.RaisePropertyChanged("CurrentStundenplaneintrag");
        this.DeleteStundenplaneintragCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string StundenplanBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "StundenplanBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("StundenplanBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die GültigAb
    /// </summary>
    public DateTime StundenplanGültigAb
    {
      get
      {
        return this.Model.GültigAb;
      }

      set
      {
        if (value == this.Model.GültigAb) return;
        this.UndoablePropertyChanging(this, "StundenplanGültigAb", this.Model.GültigAb, value);
        this.Model.GültigAb = value;
        this.RaisePropertyChanged("StundenplanGültigAb");
      }
    }

    /// <summary>
    /// Holt oder setzt die Jahrtyp currently assigned to this Stundenplan
    /// </summary>
    public JahrtypViewModel StundenplanJahrtyp
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
        this.UndoablePropertyChanging(this, "StundenplanJahrtyp", this.jahrtyp, value);
        this.jahrtyp = value;
        this.Model.Jahrtyp = value.Model;
        this.RaisePropertyChanged("StundenplanJahrtyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die Halbjahrtyp currently assigned to this Stundenplan
    /// </summary>
    public HalbjahrtypViewModel StundenplanHalbjahrtyp
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
        this.UndoablePropertyChanging(this, "StundenplanHalbjahrtyp", this.halbjahrtyp, value);
        this.halbjahrtyp = value;
        this.Model.Halbjahrtyp = value.Model;
        this.RaisePropertyChanged("StundenplanHalbjahrtyp");
      }
    }

    public string Stundenplan1Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[0].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan1Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[0].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan2Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[1].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan2Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[1].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan3Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[2].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan3Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[2].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan4Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[3].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan4Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[3].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan5Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[4].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan5Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[4].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan6Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[5].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan6Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[5].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan7Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[6].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan7Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[6].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan8Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[7].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan8Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[7].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan9Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[8].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan9Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[8].UnterrichtsstundeZeitraum; }
    }

    public string Stundenplan10Bezeichnung
    {
      get { return App.MainViewModel.Unterrichtsstunden[9].UnterrichtsstundeBezeichnung; }
    }

    public string Stundenplan10Zeit
    {
      get { return App.MainViewModel.Unterrichtsstunden[9].UnterrichtsstundeZeitraum; }
    }

    public int StundenplanStundenzahlproTag
    {
      get
      {
        return App.MainViewModel.Unterrichtsstunden.Count();
      }
    }

    #region Montag

    public StundenplaneintragViewModel Stundenplan11
    {
      get { return this.GetStundenplanEintragViewModel(1, 1); }
    }

    public int Stundenplan11Span
    {
      get { return this.Stundenanzahl(1, 1); }
    }

    public StundenplaneintragViewModel Stundenplan12
    {
      get { return this.GetStundenplanEintragViewModel(1, 2); }
    }

    public int Stundenplan12Span
    {
      get { return this.Stundenanzahl(1, 2); }
    }

    public StundenplaneintragViewModel Stundenplan13
    {
      get { return this.GetStundenplanEintragViewModel(1, 3); }
    }

    public int Stundenplan13Span
    {
      get { return this.Stundenanzahl(1, 3); }
    }

    public StundenplaneintragViewModel Stundenplan14
    {
      get { return this.GetStundenplanEintragViewModel(1, 4); }
    }

    public int Stundenplan14Span
    {
      get { return this.Stundenanzahl(1, 4); }
    }

    public StundenplaneintragViewModel Stundenplan15
    {
      get { return this.GetStundenplanEintragViewModel(1, 5); }
    }

    public int Stundenplan15Span
    {
      get { return this.Stundenanzahl(1, 5); }
    }

    public StundenplaneintragViewModel Stundenplan16
    {
      get { return this.GetStundenplanEintragViewModel(1, 6); }
    }

    public int Stundenplan16Span
    {
      get { return this.Stundenanzahl(1, 6); }
    }

    public StundenplaneintragViewModel Stundenplan17
    {
      get { return this.GetStundenplanEintragViewModel(1, 7); }
    }

    public int Stundenplan17Span
    {
      get { return this.Stundenanzahl(1, 7); }
    }

    public StundenplaneintragViewModel Stundenplan18
    {
      get { return this.GetStundenplanEintragViewModel(1, 8); }
    }

    public int Stundenplan18Span
    {
      get { return this.Stundenanzahl(1, 8); }
    }

    public StundenplaneintragViewModel Stundenplan19
    {
      get { return this.GetStundenplanEintragViewModel(1, 9); }
    }

    public int Stundenplan19Span
    {
      get { return this.Stundenanzahl(1, 9); }
    }

    public StundenplaneintragViewModel Stundenplan110
    {
      get { return this.GetStundenplanEintragViewModel(1, 10); }
    }

    public int Stundenplan110Span
    {
      get { return this.Stundenanzahl(1, 10); }
    }

    #endregion

    #region Dienstag

    public StundenplaneintragViewModel Stundenplan21
    {
      get { return this.GetStundenplanEintragViewModel(2, 1); }
    }

    public int Stundenplan21Span
    {
      get { return this.Stundenanzahl(2, 1); }
    }

    public StundenplaneintragViewModel Stundenplan22
    {
      get { return this.GetStundenplanEintragViewModel(2, 2); }
    }

    public int Stundenplan22Span
    {
      get { return this.Stundenanzahl(2, 2); }
    }

    public StundenplaneintragViewModel Stundenplan23
    {
      get { return this.GetStundenplanEintragViewModel(2, 3); }
    }

    public int Stundenplan23Span
    {
      get { return this.Stundenanzahl(2, 3); }
    }

    public StundenplaneintragViewModel Stundenplan24
    {
      get { return this.GetStundenplanEintragViewModel(2, 4); }
    }

    public int Stundenplan24Span
    {
      get { return this.Stundenanzahl(2, 4); }
    }

    public StundenplaneintragViewModel Stundenplan25
    {
      get { return this.GetStundenplanEintragViewModel(2, 5); }
    }

    public int Stundenplan25Span
    {
      get { return this.Stundenanzahl(2, 5); }
    }

    public StundenplaneintragViewModel Stundenplan26
    {
      get { return this.GetStundenplanEintragViewModel(2, 6); }
    }

    public int Stundenplan26Span
    {
      get { return this.Stundenanzahl(2, 6); }
    }

    public StundenplaneintragViewModel Stundenplan27
    {
      get { return this.GetStundenplanEintragViewModel(2, 7); }
    }

    public int Stundenplan27Span
    {
      get { return this.Stundenanzahl(2, 7); }
    }

    public StundenplaneintragViewModel Stundenplan28
    {
      get { return this.GetStundenplanEintragViewModel(2, 8); }
    }

    public int Stundenplan28Span
    {
      get { return this.Stundenanzahl(2, 8); }
    }

    public StundenplaneintragViewModel Stundenplan29
    {
      get { return this.GetStundenplanEintragViewModel(2, 9); }
    }

    public int Stundenplan29Span
    {
      get { return this.Stundenanzahl(2, 9); }
    }

    public StundenplaneintragViewModel Stundenplan210
    {
      get { return this.GetStundenplanEintragViewModel(2, 10); }
    }

    public int Stundenplan210Span
    {
      get { return this.Stundenanzahl(2, 10); }
    }

    #endregion

    #region Mittwoch

    public StundenplaneintragViewModel Stundenplan31
    {
      get { return this.GetStundenplanEintragViewModel(3, 1); }
    }

    public int Stundenplan31Span
    {
      get { return this.Stundenanzahl(3, 1); }
    }

    public StundenplaneintragViewModel Stundenplan32
    {
      get { return this.GetStundenplanEintragViewModel(3, 2); }
    }

    public int Stundenplan32Span
    {
      get { return this.Stundenanzahl(3, 2); }
    }

    public StundenplaneintragViewModel Stundenplan33
    {
      get { return this.GetStundenplanEintragViewModel(3, 3); }
    }

    public int Stundenplan33Span
    {
      get { return this.Stundenanzahl(3, 3); }
    }

    public StundenplaneintragViewModel Stundenplan34
    {
      get { return this.GetStundenplanEintragViewModel(3, 4); }
    }

    public int Stundenplan34Span
    {
      get { return this.Stundenanzahl(3, 4); }
    }

    public StundenplaneintragViewModel Stundenplan35
    {
      get { return this.GetStundenplanEintragViewModel(3, 5); }
    }

    public int Stundenplan35Span
    {
      get { return this.Stundenanzahl(3, 5); }
    }

    public StundenplaneintragViewModel Stundenplan36
    {
      get { return this.GetStundenplanEintragViewModel(3, 6); }
    }

    public int Stundenplan36Span
    {
      get { return this.Stundenanzahl(3, 6); }
    }

    public StundenplaneintragViewModel Stundenplan37
    {
      get { return this.GetStundenplanEintragViewModel(3, 7); }
    }

    public int Stundenplan37Span
    {
      get { return this.Stundenanzahl(3, 7); }
    }

    public StundenplaneintragViewModel Stundenplan38
    {
      get { return this.GetStundenplanEintragViewModel(3, 8); }
    }

    public int Stundenplan38Span
    {
      get { return this.Stundenanzahl(3, 8); }
    }

    public StundenplaneintragViewModel Stundenplan39
    {
      get { return this.GetStundenplanEintragViewModel(3, 9); }
    }

    public int Stundenplan39Span
    {
      get { return this.Stundenanzahl(3, 9); }
    }

    public StundenplaneintragViewModel Stundenplan310
    {
      get { return this.GetStundenplanEintragViewModel(3, 10); }
    }

    public int Stundenplan310Span
    {
      get { return this.Stundenanzahl(3, 10); }
    }

    #endregion

    #region Donnerstag

    public StundenplaneintragViewModel Stundenplan41
    {
      get { return this.GetStundenplanEintragViewModel(4, 1); }
    }

    public int Stundenplan41Span
    {
      get { return this.Stundenanzahl(4, 1); }
    }

    public StundenplaneintragViewModel Stundenplan42
    {
      get { return this.GetStundenplanEintragViewModel(4, 2); }
    }

    public int Stundenplan42Span
    {
      get { return this.Stundenanzahl(4, 2); }
    }

    public StundenplaneintragViewModel Stundenplan43
    {
      get { return this.GetStundenplanEintragViewModel(4, 3); }
    }

    public int Stundenplan43Span
    {
      get { return this.Stundenanzahl(4, 3); }
    }

    public StundenplaneintragViewModel Stundenplan44
    {
      get { return this.GetStundenplanEintragViewModel(4, 4); }
    }

    public int Stundenplan44Span
    {
      get { return this.Stundenanzahl(4, 4); }
    }

    public StundenplaneintragViewModel Stundenplan45
    {
      get { return this.GetStundenplanEintragViewModel(4, 5); }
    }

    public int Stundenplan45Span
    {
      get { return this.Stundenanzahl(4, 5); }
    }

    public StundenplaneintragViewModel Stundenplan46
    {
      get { return this.GetStundenplanEintragViewModel(4, 6); }
    }

    public int Stundenplan46Span
    {
      get { return this.Stundenanzahl(4, 6); }
    }

    public StundenplaneintragViewModel Stundenplan47
    {
      get { return this.GetStundenplanEintragViewModel(4, 7); }
    }

    public int Stundenplan47Span
    {
      get { return this.Stundenanzahl(4, 7); }
    }

    public StundenplaneintragViewModel Stundenplan48
    {
      get { return this.GetStundenplanEintragViewModel(4, 8); }
    }

    public int Stundenplan48Span
    {
      get { return this.Stundenanzahl(4, 8); }
    }

    public StundenplaneintragViewModel Stundenplan49
    {
      get { return this.GetStundenplanEintragViewModel(4, 9); }
    }

    public int Stundenplan49Span
    {
      get { return this.Stundenanzahl(4, 9); }
    }

    public StundenplaneintragViewModel Stundenplan410
    {
      get { return this.GetStundenplanEintragViewModel(4, 10); }
    }

    public int Stundenplan410Span
    {
      get { return this.Stundenanzahl(4, 10); }
    }

    #endregion

    #region Freitag

    public StundenplaneintragViewModel Stundenplan51
    {
      get { return this.GetStundenplanEintragViewModel(5, 1); }
    }

    public int Stundenplan51Span
    {
      get { return this.Stundenanzahl(5, 1); }
    }

    public StundenplaneintragViewModel Stundenplan52
    {
      get { return this.GetStundenplanEintragViewModel(5, 2); }
    }

    public int Stundenplan52Span
    {
      get { return this.Stundenanzahl(5, 2); }
    }

    public StundenplaneintragViewModel Stundenplan53
    {
      get { return this.GetStundenplanEintragViewModel(5, 3); }
    }

    public int Stundenplan53Span
    {
      get { return this.Stundenanzahl(5, 3); }
    }

    public StundenplaneintragViewModel Stundenplan54
    {
      get { return this.GetStundenplanEintragViewModel(5, 4); }
    }

    public int Stundenplan54Span
    {
      get { return this.Stundenanzahl(5, 4); }
    }

    public StundenplaneintragViewModel Stundenplan55
    {
      get { return this.GetStundenplanEintragViewModel(5, 5); }
    }

    public int Stundenplan55Span
    {
      get { return this.Stundenanzahl(5, 5); }
    }

    public StundenplaneintragViewModel Stundenplan56
    {
      get { return this.GetStundenplanEintragViewModel(5, 6); }
    }

    public int Stundenplan56Span
    {
      get { return this.Stundenanzahl(5, 6); }
    }

    public StundenplaneintragViewModel Stundenplan57
    {
      get { return this.GetStundenplanEintragViewModel(5, 7); }
    }

    public int Stundenplan57Span
    {
      get { return this.Stundenanzahl(5, 7); }
    }

    public StundenplaneintragViewModel Stundenplan58
    {
      get { return this.GetStundenplanEintragViewModel(5, 8); }
    }

    public int Stundenplan58Span
    {
      get { return this.Stundenanzahl(5, 8); }
    }

    public StundenplaneintragViewModel Stundenplan59
    {
      get { return this.GetStundenplanEintragViewModel(5, 9); }
    }

    public int Stundenplan59Span
    {
      get { return this.Stundenanzahl(5, 9); }
    }

    public StundenplaneintragViewModel Stundenplan510
    {
      get { return this.GetStundenplanEintragViewModel(5, 10); }
    }

    public int Stundenplan510Span
    {
      get { return this.Stundenanzahl(5, 10); }
    }

    #endregion

    /// <summary>
    /// Holt die StundenplanEinträge for this stundenplan
    /// </summary>
    public ObservableCollection<StundenplaneintragViewModel> Stundenplaneinträge { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Stundenplan: " + this.StundenplanBezeichnung;
    }

    /// <summary>
    /// Handles addition a new Stundenplaneintrag to this stundenplan
    /// without adding it to the changes list.
    /// </summary>
    public void AddStundenplaneintrag(
      int ersteUnterrichtsstundeIndex,
      int letzteUnterrichtsstundeIndex,
      int wochentagIndex,
      Fach fach,
      Klasse klasse,
      Raum raum)
    {
      var stundenplaneintrag = new Stundenplaneintrag();
      stundenplaneintrag.ErsteUnterrichtsstundeIndex = ersteUnterrichtsstundeIndex;
      stundenplaneintrag.LetzteUnterrichtsstundeIndex = letzteUnterrichtsstundeIndex;
      stundenplaneintrag.WochentagIndex = wochentagIndex;
      stundenplaneintrag.Fach = fach;
      stundenplaneintrag.Klasse = klasse;
      stundenplaneintrag.Raum = raum;
      stundenplaneintrag.Stundenplan = this.Model;
      var vm = new StundenplaneintragViewModel(this, stundenplaneintrag);
      using (new UndoBatch(App.MainViewModel, string.Format("Neuer Stundenplaneintrag {0} angelegt.", vm), false))
      {
        App.MainViewModel.Stundenplaneinträge.Add(vm);
        this.Stundenplaneinträge.Add(vm);
        this.CurrentStundenplaneintrag = vm;
        this.UpdateProperties(vm);
      }
    }

    /// <summary>
    /// Sendet PropertyChanged events für den gegebenen stundenplaneintrag.
    /// </summary>
    /// <param name="wochentag"></param>
    /// <param name="ersteStunde"></param>
    /// <param name="stundenzahl"></param>
    public void UpdateProperties(int wochentag, int ersteStunde, int stundenzahl)
    {
      var wochentagIndex = wochentag;
      var ersteUnterrichtsstundeIndex = ersteStunde;
      if (stundenzahl > 1)
      {
        ersteUnterrichtsstundeIndex++;
        string nextPropertyString = "Stundenplan" + wochentagIndex + ersteUnterrichtsstundeIndex.ToString("N0");
        this.RaisePropertyChanged(nextPropertyString);
      }

      string propertyString = "Stundenplan" + wochentagIndex + ersteStunde;
      string propertySpanString = "Stundenplan" + wochentagIndex
                                  + ersteStunde + "Span";
      this.RaisePropertyChanged(propertyString);
      this.RaisePropertyChanged(propertySpanString);
    }

    public void UpdateProperties(StundenplaneintragViewModel vm)
    {
      this.UpdateProperties(vm.Model.WochentagIndex, vm.Model.ErsteUnterrichtsstundeIndex, vm.Stundenanzahl);
    }

    /// <summary>
    /// Handles deletion of the current Stundenplaneintrag
    /// </summary>
    public void DeleteStundenplaneintrag(StundenplaneintragViewModel stundenplaneintragViewModel)
    {
      // Hier wird eine Kopie angelegt, die ermöglicht auch nach der Löschung im ViewModel
      // auch noch die Änderungen in den Jahresplänen vorzunehmen
      // Das findet ja erst nach allen Änderungen statt.
      var eintrag = new Stundenplaneintrag
                      {
                        Klasse = stundenplaneintragViewModel.StundenplaneintragKlasse.Model,
                        Fach = stundenplaneintragViewModel.StundenplaneintragFach.Model,
                        ErsteUnterrichtsstundeIndex =
                          stundenplaneintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex,
                        LetzteUnterrichtsstundeIndex =
                          stundenplaneintragViewModel
                          .StundenplaneintragLetzteUnterrichtsstundeIndex,
                        Raum = stundenplaneintragViewModel.StundenplaneintragRaum.Model,
                        WochentagIndex =
                          stundenplaneintragViewModel.StundenplaneintragWochentagIndex,
                        Stundenplan = this.Model
                      };
      var vm = new StundenplaneintragViewModel(eintrag);
      var änderung = new StundenplanÄnderung(
        StundenplanÄnderungUpdateType.Removed,
        stundenplaneintragViewModel.StundenplaneintragWochentagIndex,
        stundenplaneintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex,
        vm);
      this.ÄnderungsListe.Add(änderung);

      using (new UndoBatch(App.MainViewModel, string.Format("Stundenplaneintrag {0} gelöscht.", stundenplaneintragViewModel), false))
      {
        bool success = App.MainViewModel.Stundenplaneinträge.RemoveTest(stundenplaneintragViewModel);
        this.Stundenplaneinträge.RemoveTest(stundenplaneintragViewModel);
        this.CurrentStundenplaneintrag = null;
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherStundenplanViewModel = obj as StundenplanViewModel;
      if (otherStundenplanViewModel != null)
      {
        return DateTime.Compare(this.StundenplanGültigAb, otherStundenplanViewModel.StundenplanGültigAb);
      }

      throw new ArgumentException("Object is not a StundenplanViewModel");
    }

    /// <summary>
    /// Erstellt eine komplette Kopie des Stundenplans.
    /// </summary>
    /// <returns>Den Stundenplan als Kopie.</returns>
    public object Clone()
    {
      var stundenplan = new Stundenplan();
      stundenplan.Jahrtyp = this.StundenplanJahrtyp.Model;
      stundenplan.Halbjahrtyp = this.StundenplanHalbjahrtyp.Model;
      stundenplan.GültigAb = this.StundenplanGültigAb;
      stundenplan.Bezeichnung = this.StundenplanBezeichnung;
      var stundenplanViewModel = new StundenplanViewModel(stundenplan);
      using (new UndoBatch(App.MainViewModel, string.Format("Stundenplankopie von {0} erstellt.", stundenplanViewModel), false))
      {
        foreach (var stundenplanEintrag in this.Stundenplaneinträge)
        {
          var clone = new Stundenplaneintrag();
          clone.ErsteUnterrichtsstundeIndex = stundenplanEintrag.StundenplaneintragErsteUnterrichtsstundeIndex;
          clone.Fach = stundenplanEintrag.StundenplaneintragFach.Model;
          clone.Klasse = stundenplanEintrag.StundenplaneintragKlasse.Model;
          clone.LetzteUnterrichtsstundeIndex = stundenplanEintrag.StundenplaneintragLetzteUnterrichtsstundeIndex;
          clone.Raum = stundenplanEintrag.StundenplaneintragRaum.Model;
          clone.WochentagIndex = stundenplanEintrag.StundenplaneintragWochentagIndex;
          clone.Stundenplan = stundenplan;
          var stundenplanEintragViewModel = new StundenplaneintragViewModel(stundenplanViewModel, clone);
          App.MainViewModel.Stundenplaneinträge.Add(stundenplanEintragViewModel);
          stundenplanViewModel.Stundenplaneinträge.Add(stundenplanEintragViewModel);
        }

        App.MainViewModel.Stundenpläne.Add(stundenplanViewModel);
      }

      return stundenplanViewModel;
    }

    /// <summary>
    /// Tritt auf, wenn die StundenplaneinträgeCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void StundenplaneinträgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Stundenplaneinträge", this.Stundenplaneinträge, e, false, "Änderung der Stundenplaneinträge");
    }

    private StundenplaneintragViewModel GetStundenplanEintragViewModel(int wochentagIndex, int stundeIndex)
    {
      if (stundeIndex % 2 == 0)
      {
        var previousStundenplaneintragViewModel =
          this.Stundenplaneinträge.FirstOrDefault(
            eintrag => eintrag.StundenplaneintragWochentagIndex == wochentagIndex && eintrag.StundenplaneintragErsteUnterrichtsstundeIndex == stundeIndex - 1);
        if (previousStundenplaneintragViewModel != null && previousStundenplaneintragViewModel.Stundenanzahl > 1)
        {
          // Previous Stundenplaneintrag is spanning two rows
          // TODO: care for spanning more than one row
          return null;
        }
      }

      var stundenplaneintragViewModel =
        this.Stundenplaneinträge.SingleOrDefault(
          eintrag => eintrag.StundenplaneintragWochentagIndex == wochentagIndex && eintrag.StundenplaneintragErsteUnterrichtsstundeIndex == stundeIndex);

      if (stundenplaneintragViewModel == null)
      {
        var emptyStundenplanEintrag = new Stundenplaneintrag
          {
            ErsteUnterrichtsstundeIndex = stundeIndex,
            LetzteUnterrichtsstundeIndex = stundeIndex,
            WochentagIndex = wochentagIndex
          };
        var emptyStundenplanEintragViewModel = new StundenplaneintragViewModel(this, emptyStundenplanEintrag);

        return emptyStundenplanEintragViewModel;
      }

      return stundenplaneintragViewModel;
    }

    private int Stundenanzahl(int wochentagIndex, int stundeIndex)
    {
      var stundenplaneintragViewModel =
        this.Stundenplaneinträge.SingleOrDefault(
          eintrag => eintrag.StundenplaneintragWochentagIndex == wochentagIndex && eintrag.StundenplaneintragErsteUnterrichtsstundeIndex == stundeIndex);
      if (stundenplaneintragViewModel != null)
      {
        return stundenplaneintragViewModel.Stundenanzahl;
      }

      return 1;
    }

    /// <summary>
    /// Creates the <see cref="ContextMenu"/> for each stundenplan grid.
    /// </summary>
    private void CreateContextMenu()
    {
      this.stundenplanContextMenu = new ContextMenu();

      var addStundenplaneintragItem = new MenuItem
      {
        Header = "Stunde anlegen",
        Command = this.AddStundenplaneintragCommand,
        Icon = App.GetImage("Stundenentwurf16.png")
      };

      this.stundenplanContextMenu.Items.Add(addStundenplaneintragItem);
    }

    /// <summary>
    /// Handles addition a new Stundenplaneintrag to this stundenplan
    /// </summary>
    private void AddStundenplaneintrag()
    {
      this.AddStundenplaneintrag(
        StundenplanSelection.Instance.ErsteUnterrichtsstundeIndex,
        StundenplanSelection.Instance.LetzteUnterrichtsstundeIndex,
        StundenplanSelection.Instance.WochentagIndex);
    }

    /// <summary>
    /// Handles addition a new Stundenplaneintrag to this stundenplan
    /// </summary>
    /// <param name="ersteStundeIndex"> The erste Stunde Index. </param>
    /// <param name="letzteStundeIndex"> The letzte Stunde Index. </param>
    /// <param name="wochentagIndex"> The wochentag Index. </param>
    private void AddStundenplaneintrag(int ersteStundeIndex, int letzteStundeIndex, int wochentagIndex)
    {
      var stundenplaneintrag = new Stundenplaneintrag();
      stundenplaneintrag.ErsteUnterrichtsstundeIndex = ersteStundeIndex;
      stundenplaneintrag.LetzteUnterrichtsstundeIndex = letzteStundeIndex;
      stundenplaneintrag.WochentagIndex = wochentagIndex;
      stundenplaneintrag.Stundenplan = this.Model;

      var vm = new StundenplaneintragViewModel(this, stundenplaneintrag);
      var undo = false;
      //using (new UndoBatch(App.MainViewModel, string.Format("Stundenplaneintrag {0} erstellt.", vm), false))
      {
        var dlg = new AddStundenplaneintragDialog(vm);
        if (!(undo = !dlg.ShowDialog().GetValueOrDefault(false)))
        {
          App.MainViewModel.Stundenplaneinträge.Add(vm);
          this.Stundenplaneinträge.Add(vm);
          this.CurrentStundenplaneintrag = vm;
          this.UpdateProperties(vm);

          var änderung = new StundenplanÄnderung(StundenplanÄnderungUpdateType.Added, -1, -1, vm);
          this.ÄnderungsListe.Add(änderung);
        }
      }

      //if (undo)
      //{
      //  App.MainViewModel.ExecuteUndoCommand();
      //}
    }

    /// <summary>
    /// Handles deletion of the current Stundenplaneintrag
    /// </summary>
    private void DeleteCurrentStundenplaneintrag()
    {
      this.DeleteStundenplaneintrag(this.CurrentStundenplaneintrag);
    }
  }
}
