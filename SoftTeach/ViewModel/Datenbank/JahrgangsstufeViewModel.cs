namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Noten;

  /// <summary>
  /// ViewModel of an individual jahrgangsstufe
  /// </summary>
  public class JahrgangsstufeViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The klassenstufe currently selected
    /// </summary>
    private KlassenstufeViewModel currentKlassenstufe;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahrgangsstufeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="jahrgangsstufe">
    /// The underlying jahrgangsstufe this ViewModel is to be based on
    /// </param>
    public JahrgangsstufeViewModel(Jahrgangsstufe jahrgangsstufe)
    {
      if (jahrgangsstufe == null)
      {
        throw new ArgumentNullException("jahrgangsstufe");
      }

      this.Model = jahrgangsstufe;

      this.AddKlassenstufeCommand = new DelegateCommand(this.AddKlassenstufe);
      this.DeleteKlassenstufeCommand = new DelegateCommand(this.DeleteCurrentKlassenstufe, () => this.CurrentKlassenstufe != null);

      // Build data structures for klassen
      this.Klassenstufen = new ObservableCollection<KlassenstufeViewModel>();
      foreach (var klassenstufe in jahrgangsstufe.Klassenstufen)
      {
        var vm = new KlassenstufeViewModel(klassenstufe);
        App.MainViewModel.Klassenstufen.Add(vm);
        this.Klassenstufen.Add(vm);
      }

      this.Klassenstufen.CollectionChanged += this.KlassenstufenCollectionChanged;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Klassenstufe
    /// </summary>
    public DelegateCommand AddKlassenstufeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Klassenstufe
    /// </summary>
    public DelegateCommand DeleteKlassenstufeCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Jahrgangsstufe this ViewModel is based on
    /// </summary>
    public Jahrgangsstufe Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected klassenstufe
    /// </summary>
    public KlassenstufeViewModel CurrentKlassenstufe
    {
      get
      {
        return this.currentKlassenstufe;
      }

      set
      {
        this.currentKlassenstufe = value;
        this.RaisePropertyChanged("CurrentKlassenstufe");
        this.DeleteKlassenstufeCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string JahrgangsstufeBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "JahrgangsstufeBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("JahrgangsstufeBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die Bepunktungstyp currently assigned to this Jahrgangsstufe
    /// </summary>
    public Bepunktungstyp JahrgangsstufeBepunktungstyp
    {
      get
      {
        return (Bepunktungstyp)Enum.Parse(typeof(Bepunktungstyp), this.Model.Bepunktungstyp);
      }

      set
      {
        if (value.ToString() == this.Model.Bepunktungstyp) return;
        this.UndoablePropertyChanging(this, "JahrgangsstufeBepunktungstyp", this.Model.Bepunktungstyp, value.ToString());
        this.Model.Bepunktungstyp = value.ToString();
        this.RaisePropertyChanged("JahrgangsstufeBepunktungstyp");
      }
    }

    /// <summary>
    /// Holt den klassenstufen on file for this jahrgangsstufe
    /// </summary>
    public ObservableCollection<KlassenstufeViewModel> Klassenstufen { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Jahrgangsstufe: " + this.JahrgangsstufeBezeichnung;
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
      var compareJahrgangsstufe = viewModel as JahrgangsstufeViewModel;
      if (compareJahrgangsstufe == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareJahrgangsstufe.JahrgangsstufeBezeichnung, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tritt auf, wenn die KlassenstufenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void KlassenstufenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Klassenstufen", this.Klassenstufen, e, true, "Änderung der Klassenstufen");
    }

    /// <summary>
    /// Handles addition a new klassenstufe to this jahrgangsstufe
    /// </summary>
    private void AddKlassenstufe()
    {
      var klassenstufe = new Klassenstufe { Bezeichnung = "Neue Klassenstufe" };
      var vm = new KlassenstufeViewModel(klassenstufe);

      using (new UndoBatch(App.MainViewModel, string.Format("{0} neu erstellt", vm), false))
      {
        this.Klassenstufen.Add(vm);
        App.MainViewModel.Klassenstufen.Add(vm);
        this.CurrentKlassenstufe = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current klassenstufe
    /// </summary>
    private void DeleteCurrentKlassenstufe()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("{0} gelöscht", this.CurrentKlassenstufe), false))
      {
        App.MainViewModel.Klassenstufen.RemoveTest(this.CurrentKlassenstufe);
        this.Klassenstufen.RemoveTest(this.CurrentKlassenstufe);
        this.CurrentKlassenstufe = null;
      }
    }
  }
}
