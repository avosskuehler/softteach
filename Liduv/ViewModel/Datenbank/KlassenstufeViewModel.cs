namespace Liduv.ViewModel.Datenbank
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows.Input;

  using Liduv.Model.EntityFramework;
  using Liduv.UndoRedo;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual klassenstufe
  /// </summary>
  public class KlassenstufeViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The klasse currently selected
    /// </summary>
    private KlasseViewModel currentKlasse;

    /// <summary>
    /// The jahrgangsstufe currently assigned to this KlassenstufeViewModel
    /// </summary>
    private JahrgangsstufeViewModel jahrgangsstufe;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="KlassenstufeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="klassenstufe">
    /// The underlying klassenstufe this ViewModel is to be based on
    /// </param>
    public KlassenstufeViewModel(Klassenstufe klassenstufe)
    {
      if (klassenstufe == null)
      {
        throw new ArgumentNullException("klassenstufe");
      }

      this.Model = klassenstufe;

      this.AddKlasseCommand = new DelegateCommand(this.AddKlasse);
      this.DeleteKlasseCommand = new DelegateCommand(this.DeleteCurrentKlasse, () => this.CurrentKlasse != null);

      // Build data structures for klassen
      this.Klassen = new ObservableCollection<KlasseViewModel>();
      foreach (var klasse in klassenstufe.Klassen)
      {
        var vm = new KlasseViewModel(klasse);
        App.MainViewModel.Klassen.Add(vm);
        this.Klassen.Add(vm);
      }

      this.Klassen.CollectionChanged += this.KlassenCollectionChanged;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Email address
    /// </summary>
    public DelegateCommand AddKlasseCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current employee
    /// </summary>
    public DelegateCommand DeleteKlasseCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Klassenstufe this ViewModel is based on
    /// </summary>
    public Klassenstufe Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected klasse
    /// </summary>
    public KlasseViewModel CurrentKlasse
    {
      get
      {
        return this.currentKlasse;
      }

      set
      {
        this.currentKlasse = value;
        this.RaisePropertyChanged("CurrentKlasse");
        this.DeleteKlasseCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string KlassenstufeBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "KlassenstufeBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("KlassenstufeBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public JahrgangsstufeViewModel KlassenstufeJahrgangsstufe
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Jahrgangsstufe == null)
        {
          return null;
        }

        if (this.jahrgangsstufe == null || this.jahrgangsstufe.Model != this.Model.Jahrgangsstufe)
        {
          this.jahrgangsstufe = App.MainViewModel.Jahrgangsstufen.SingleOrDefault(d => d.Model == this.Model.Jahrgangsstufe);
        }

        return this.jahrgangsstufe;
      }

      set
      {
        if (value.JahrgangsstufeBezeichnung == this.jahrgangsstufe.JahrgangsstufeBezeichnung) return;
        this.UndoablePropertyChanging(this, "KlassenstufeJahrgangsstufe", this.jahrgangsstufe, value);
        this.jahrgangsstufe = value;
        this.Model.Jahrgangsstufe = value.Model;
        this.RaisePropertyChanged("KlassenstufeJahrgangsstufe");
      }
    }

    /// <summary>
    /// Holt den klassen on file for this jahrgangsstufe
    /// </summary>
    public ObservableCollection<KlasseViewModel> Klassen { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Klassenstufe: " + this.KlassenstufeBezeichnung;
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
      var compareKlassenstufe = viewModel as KlassenstufeViewModel;
      if (compareKlassenstufe == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareKlassenstufe.KlassenstufeBezeichnung, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tritt auf, wenn die KlassenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void KlassenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Klassen", this.Klassen, e, false, "Änderung der Klassen");
    }

    /// <summary>
    /// Handles addition a new klasse to this Klassenstufe
    /// </summary>
    private void AddKlasse()
    {
      var klasse = new Klasse { Bezeichnung = "Neue Lerngruppe", Klassenstufe = this.Model };
      var vm = new KlasseViewModel(klasse);

      using (new UndoBatch(App.MainViewModel, string.Format("{0} neu erstellt", vm), false))
      {
        this.Klassen.Add(vm);
        App.MainViewModel.Klassen.Add(vm);
        this.CurrentKlasse = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current klasse
    /// </summary>
    private void DeleteCurrentKlasse()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("{0} gelöscht", this.CurrentKlasse), false))
      {
        var success = App.MainViewModel.Klassen.RemoveTest(this.CurrentKlasse);
        this.Klassen.RemoveTest(this.CurrentKlasse);
        this.CurrentKlasse = null;
      }
    }
  }
}
