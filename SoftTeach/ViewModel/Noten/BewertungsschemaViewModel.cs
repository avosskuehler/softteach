namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual bewertungsschema
  /// </summary>
  public class BewertungsschemaViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Die momentan ausgewählte Prozentbereich
    /// </summary>
    private ProzentbereichViewModel currentProzentbereich;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="BewertungsschemaViewModel"/> Klasse. 
    /// </summary>
    /// <param name="bewertungsschema">
    /// The underlying bewertungsschema this ViewModel is to be based on
    /// </param>
    public BewertungsschemaViewModel(Bewertungsschema bewertungsschema)
    {
      this.Model = bewertungsschema ?? throw new ArgumentNullException(nameof(bewertungsschema));

      this.AddProzentbereichCommand = new DelegateCommand(this.AddProzentbereich);
      this.AddProzentbereicheCommand = new DelegateCommand(this.AddProzentbereiche);
      this.DeleteProzentbereichCommand = new DelegateCommand(this.DeleteCurrentProzentbereich, () => this.CurrentProzentbereich != null);

      // Build data structures for prozentbereiche
      this.Prozentbereiche = new ObservableCollection<ProzentbereichViewModel>();
      foreach (var phase in bewertungsschema.Prozentbereiche)
      {
        var vm = new ProzentbereichViewModel(phase);
        App.MainViewModel.Prozentbereiche.Add(vm);
        this.Prozentbereiche.Add(vm);
      }

      this.Prozentbereiche.CollectionChanged += this.ProzentbereicheCollectionChanged;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Email address
    /// </summary>
    public DelegateCommand AddProzentbereichCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Email address
    /// </summary>
    public DelegateCommand AddProzentbereicheCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current employee
    /// </summary>
    public DelegateCommand DeleteProzentbereichCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Bewertungsschema this ViewModel is based on
    /// </summary>
    public Bewertungsschema Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected phase
    /// </summary>
    public ProzentbereichViewModel CurrentProzentbereich
    {
      get
      {
        return this.currentProzentbereich;
      }

      set
      {
        this.currentProzentbereich = value;
        this.RaisePropertyChanged("CurrentProzentbereich");
        this.DeleteProzentbereichCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string BewertungsschemaBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(BewertungsschemaBezeichnung), this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("BewertungsschemaBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die Prozentbereiche für das Bewertungsschema
    /// </summary>
    public ObservableCollection<ProzentbereichViewModel> Prozentbereiche { get; set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Bewertungsschema: " + this.BewertungsschemaBezeichnung;
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="viewModel">The object to be compared with this instance</param>
    /// <returns>Less than zero if This object is less than the other parameter. 
    /// Zero if This object is equal to other. Greater than zero if This object is greater than other.
    /// </returns>
    public int CompareTo(object viewModel)
    {
      var compareBewertungsschema = viewModel as BewertungsschemaViewModel;
      if (compareBewertungsschema == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareBewertungsschema.BewertungsschemaBezeichnung, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tritt auf, wenn die ProzentbereicheCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void ProzentbereicheCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(Prozentbereiche), this.Prozentbereiche, e, true, "Änderung der Prozentbereiche");
    }

    /// <summary>
    /// Hier wird für ein Prozentbereich angelegt.
    /// </summary>
    private void AddProzentbereich()
    {
      var prozentbereich = new Prozentbereich
      {
        BisProzent = 0.05f,
        VonProzent = 0f,
        Zensur = App.MainViewModel.Zensuren[0].Model,
        Bewertungsschema = this.Model
      };
      var vm = new ProzentbereichViewModel(prozentbereich);

      using (new UndoBatch(App.MainViewModel, string.Format("Neuer Prozentbereich {0} erstellt.", vm), false))
      {
        App.MainViewModel.Prozentbereiche.Add(vm);
        this.Prozentbereiche.Add(vm);
        this.CurrentProzentbereich = vm;
      }
    }

    /// <summary>
    /// Hier wird für jede verfügbare Zensur ein vordefinierter Prozentbereich angelegt.
    /// </summary>
    private void AddProzentbereiche()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Für alle Zensuren Prozentbereich erstellt."), false))
      {
        var zensuren = App.MainViewModel.Zensuren;
        var startProzent = 0f;
        foreach (var zensurViewModel in zensuren)
        {
          var prozentbereich = new Prozentbereich
          {
            BisProzent = startProzent + 0.05f,
            VonProzent = startProzent,
            Zensur = zensurViewModel.Model,
            Bewertungsschema = this.Model
          };
          startProzent += 0.05f;

          var vm = new ProzentbereichViewModel(prozentbereich);
          App.MainViewModel.Prozentbereiche.Add(vm);
          this.Prozentbereiche.Add(vm);
          this.CurrentProzentbereich = vm;
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current Prozentbereich
    /// </summary>
    private void DeleteCurrentProzentbereich()
    {
      this.DeleteProzentbereich(this.CurrentProzentbereich);
    }

    /// <summary>
    /// Handles deletion of the given Prozentbereich
    /// </summary>
    /// <param name="prozentbereichViewModel"> The phase View Model. </param>
    private void DeleteProzentbereich(ProzentbereichViewModel prozentbereichViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Prozentbereich {0} gelöscht", prozentbereichViewModel), false))
      {
        App.MainViewModel.Prozentbereiche.RemoveTest(prozentbereichViewModel);
        var result = this.Prozentbereiche.RemoveTest(prozentbereichViewModel);
      }
    }
  }
}
