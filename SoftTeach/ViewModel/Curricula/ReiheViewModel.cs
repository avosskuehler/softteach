namespace SoftTeach.ViewModel.Curricula
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Diagnostics;
  using System.Linq;
  using System.Windows.Media;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Curricula;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual reihe
  /// </summary>
  public class ReiheViewModel : SequencedViewModel
  {
    /// <summary>
    /// The modul currently assigned to this reihe
    /// </summary>
    private ModulViewModel modul;

    /// <summary>
    /// The sequenz currently selected
    /// </summary>
    private SequenzViewModel currentSequenz;

    private bool istZuerst;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ReiheViewModel"/> Klasse.
    /// </summary>
    /// <param name="reihe">
    /// The underlying reihe this ViewModel is to be based on
    /// </param>
    public ReiheViewModel(Reihe reihe)
    {
      this.Model = reihe ?? throw new ArgumentNullException(nameof(reihe));

      // Build data structures for sequenzen
      this.Sequenzen = new ObservableCollection<SequenzViewModel>();
      //this.AvailableSequenzen = new ObservableCollection<SequenzViewModel>();

      // Sort Sequenzen by Reihenfolge
      foreach (var sequenz in reihe.Sequenzen.OrderBy(o => o.Reihenfolge).ToList())
      {
        var vm = new SequenzViewModel(this, sequenz);
        //App.MainViewModel.Sequenzen.Add(vm);
        //if (sequenz.Reihenfolge == -1)
        //{
        //  this.AvailableSequenzen.Add(vm);
        //}
        //else
        //{
        this.Sequenzen.Add(vm);
        //}
      }

      // Listen for changes
      this.Sequenzen.CollectionChanged += this.SequenzenCollectionChanged;
      //this.AvailableSequenzen.CollectionChanged += this.AvailableSequenzenCollectionChanged;

      this.AddSequenzCommand = new DelegateCommand(this.AddSequenz);
      this.EditReiheCommand = new DelegateCommand(this.EditReihe);
      this.DeleteSequenzCommand = new DelegateCommand(this.DeleteCurrentSequenz, () => this.CurrentSequenz != null);
      this.LengthenReiheCommand = new DelegateCommand(this.LengthenReihe);
      this.ShortenReiheCommand = new DelegateCommand(this.ShortenReihe);
    }

    /// <summary>
    /// Holt den underlying Reihe this ViewModel is based on
    /// </summary>
    public Reihe Model { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new sequenz
    /// </summary>
    public DelegateCommand AddSequenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Bearbeitung der Sequenz
    /// </summary>
    public DelegateCommand EditReiheCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current sequenz
    /// </summary>
    public DelegateCommand DeleteSequenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur lengthen this reihe for 1 hour
    /// </summary>
    public DelegateCommand LengthenReiheCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur shorten this reihe with 1 hour
    /// </summary>
    public DelegateCommand ShortenReiheCommand { get; private set; }

    /// <summary>
    /// Holt den used sequenzen of this curriculum
    /// </summary>
    public ObservableCollection<SequenzViewModel> Sequenzen { get; private set; }


    /// <summary>
    /// Holt oder setzt die currently selected sequenz
    /// </summary>
    public SequenzViewModel CurrentSequenz
    {
      get
      {
        return this.currentSequenz;
      }

      set
      {
        this.currentSequenz = value;
        this.RaisePropertyChanged("CurrentSequenz");
        this.DeleteSequenzCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public ModulViewModel ReiheModul
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Modul == null)
        {
          return null;
        }

        if (this.modul == null || this.modul.Model != this.Model.Modul)
        {
          this.modul = App.MainViewModel.Module.SingleOrDefault(d => d.Model == this.Model.Modul);
        }

        return this.modul;
      }

      set
      {
        if (value.ModulBezeichnung == this.modul.ModulBezeichnung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(ReiheModul), this.modul, value);
        this.modul = value;
        this.Model.Modul = value.Model;
        this.RaisePropertyChanged("ReiheModul");
      }
    }

    /// <summary>
    /// Holt oder setzt die Thema of this reihe
    /// </summary>
    public string Thema
    {
      get
      {
        return this.Model.Thema;
      }

      set
      {
        if (value == this.Model.Thema)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(Thema), this.Model.Thema, value);
        this.Model.Thema = value;
        this.RaisePropertyChanged("Thema");
      }
    }

    /// <summary>
    /// Holt einen Wert der angibt, ob diese Reihe im Curriculum verwendet wird.
    /// Das ist der Fall wenn die Reihenreihenfolge ungleich -1 ist.
    /// </summary>
    public bool ReiheWirdinCurriculumBenutzt
    {
      get
      {
        return this.Reihenfolge != -1;
      }
    }

    /// <summary>
    /// Holt die bausteine for the modul of this reihe
    /// </summary>
    [DependsUpon("ReiheModul")]
    public string ReiheBausteine
    {
      get
      {
        return this.Model.Modul.Bausteine;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob dies eine PseudoReihe ist.
    /// </summary>
    [DependsUpon("Thema")]
    public bool ReiheIsDummy
    {
      get
      {
        return this.Thema == "NN";
      }
    }

    /// <summary>
    /// Holt die passende Hintergrundfarbe
    /// </summary>
    public SolidColorBrush BackgroundBrush
    {
      get
      {
        return App.Current.FindResource("ReiheBackgroundBrush") as SolidColorBrush;
      }
    }

    /// <summary>
    /// Dummy, um Binding-Fehlermeldungen beim Zuweisen von Curricula zu Stunden zu vermeiden
    /// </summary>
    public static string LerngruppenterminMonat
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Dummy, um Binding-Fehlermeldungen beim Zuweisen von Curricula zu Stunden zu vermeiden
    /// </summary>
    public static DateTime LerngruppenterminDatum
    {
      get
      {
        return new DateTime(2020, 1, 1);
      }
    }


    /// <summary>
    /// Holt oder setzt die Reihenfolge der Reihe
    /// </summary>
    public override int Reihenfolge
    {
      get
      {
        return this.Model.Reihenfolge;
      }

      set
      {
        if (value == this.Model.Reihenfolge)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(Reihenfolge), this.Model.Reihenfolge, value);
        this.Model.Reihenfolge = value;
        this.RaisePropertyChanged("Reihenfolge");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob die Reihenfolge Vorrang vor allen
    /// anderer Reihenfolgen der gleichen Zahl hat.
    /// </summary>
    public override bool IstZuerst
    {
      get
      {
        return this.istZuerst;
      }

      set
      {
        if (value == this.istZuerst)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(IstZuerst), this.istZuerst, value);
        this.istZuerst = value;
        this.RaisePropertyChanged("IstZuerst");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stundenbedarf of this reihe
    /// </summary>
    public int ReiheStundenbedarf
    {
      get
      {
        return this.Model.Stundenbedarf;
      }

      set
      {
        if (value == this.Model.Stundenbedarf)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(ReiheStundenbedarf), this.Model.Stundenbedarf, value);
        this.Model.Stundenbedarf = value;
        this.RaisePropertyChanged("ReiheStundenbedarf");
        var vm = App.MainViewModel.Curricula.FirstOrDefault(o => o.Model == this.Model.Curriculum);
        if (vm != null)
        {
          vm.UpdateUsedStunden();
        }
      }
    }

    /// <summary>
    /// Holt den Stundenbedarf as a string
    /// </summary>
    [DependsUpon("ReiheStundenbedarf")]
    public string StundenbedarfString
    {
      get
      {
        return this.Model.Stundenbedarf + "h";
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    [DependsUpon("ReiheStundenbedarf")]
    [DependsUpon("ReiheModul")]
    public int Breite
    {
      get
      {
        var fachBezeichnung = this.Model.Curriculum.Fach.Bezeichnung;
        var jahrgang = this.Model.Curriculum.Jahrgang;

        var fachstundenanzahl = App.MainViewModel.Fachstundenanzahl.FirstOrDefault(
          o =>
          o.FachstundenanzahlFach.FachBezeichnung == fachBezeichnung// Selection.Instance.Fach.FachBezeichnung
          && o.FachstundenanzahlJahrgang == jahrgang);//Selection.Instance.Lerngruppe.LerngruppeJahrgang);
        if (fachstundenanzahl == null)
        {
          Debug.WriteLine("Keine Fachstundenanzahl gefunden für {0} {1}", fachBezeichnung, jahrgang);
          return 40;
        }

        var wochenstunden = fachstundenanzahl.FachstundenanzahlStundenzahl
                            + fachstundenanzahl.FachstundenanzahlTeilungsstundenzahl;

        return (int)(this.Model.Stundenbedarf / (float)wochenstunden * Properties.Settings.Default.Wochenbreite);
      }
    }

    /// <summary>
    /// Holt die Breite der Reihe für die Curriculumsanpassung
    /// </summary>
    [DependsUpon("ReiheStundenbedarf")]
    public int ReiheStundenbreite
    {
      get
      {
        return this.ReiheStundenbedarf * Properties.Settings.Default.Stundenbreite;
      }
    }

    ///// <summary>
    ///// Holt den available sequenzen of this curriculum
    ///// </summary>
    //public ObservableCollection<SequenzViewModel> AvailableSequenzen { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.Thema;
    }

    /// <summary>
    /// Handles addition a new sequenz to this curriculum
    /// </summary>
    private void AddSequenz()
    {
      var sequenz = new Sequenz
      {
        Reihenfolge = -1,
        Stundenbedarf = 10,
        Thema = "Neues Thema",
        Reihe = this.Model
      };

      var vm = new SequenzViewModel(this, sequenz);
      using (new UndoBatch(App.MainViewModel, string.Format("Sequenz {0} hinzugefügt", vm), false))
      {
        var dlg = new SequenzDialog { Sequenz = vm };
        dlg.ShowDialog();

        this.Sequenzen.Add(vm);
        //App.MainViewModel.Sequenzen.Add(vm);
        this.CurrentSequenz = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current sequenz
    /// </summary>
    private void DeleteCurrentSequenz()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sequenz {0} entfernt", this.CurrentSequenz), false))
      {
        //App.MainViewModel.Sequenzen.RemoveTest(this.CurrentSequenz);
        //if (this.AvailableSequenzen.Contains(this.CurrentSequenz))
        //{
        //  this.AvailableSequenzen.RemoveTest(this.CurrentSequenz);
        //}

        if (this.Sequenzen.Contains(this.CurrentSequenz))
        {
          this.Sequenzen.RemoveTest(this.CurrentSequenz);
        }
      }
    }

    /// <summary>
    /// This is the method that is called when the user selects to increase
    /// the number of stunden this sequenz should last during adaption
    /// of the curriculum
    /// </summary>
    private void LengthenReihe()
    {
      this.ReiheStundenbedarf++;
    }

    /// <summary>
    /// This is the method that is called when the user selects to decrease
    /// the number of stunden this sequenz should last during adaption
    /// of the curriculum
    /// </summary>
    private void ShortenReihe()
    {
      if (this.ReiheStundenbedarf > 1)
      {
        this.ReiheStundenbedarf--;
      }
    }

    /// <summary>
    /// Mit dieser Methode wird die aktuelle Reihe bearbeitet.
    /// </summary>
    private void EditReihe()
    {
      var dlg = new ReiheDialog { Reihe = this };
      dlg.ShowDialog();
    }

    /// <summary>
    /// Tritt auf, wenn die UsedSequenzenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SequenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(Sequenzen), this.Sequenzen, e, true, "Änderung der UsedSequenzen");
    }

    ///// <summary>
    ///// Tritt auf, wenn die AvailableSequenzenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void AvailableSequenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  UndoableCollectionChanged(this, nameof(AvailableSequenzen), this.AvailableSequenzen, e, true, "Änderung der AvailableSequenzen");
    //}
  }
}
