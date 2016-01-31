namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Jahrtyp
  /// </summary>
  public class JahrtypViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The Schulwoche currently selected
    /// </summary>
    private SchulwocheViewModel currentSchulwoche;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahrtypViewModel"/> Klasse. 
    /// </summary>
    /// <param name="jahrtyp">
    /// The underlying jahrtyp this ViewModel is to be based on
    /// </param>
    public JahrtypViewModel(Jahrtyp jahrtyp)
    {
      if (jahrtyp == null)
      {
        throw new ArgumentNullException("jahrtyp");
      }

      this.Model = jahrtyp;

      // Build data structures for schulwochen
      this.Schulwochen = new ObservableCollection<SchulwocheViewModel>();
      foreach (var schulwoche in jahrtyp.Schulwochen)
      {
        var vm = new SchulwocheViewModel(schulwoche);
        App.MainViewModel.Schulwochen.Add(vm);
        this.Schulwochen.Add(vm);
      }

      this.Schulwochen.CollectionChanged += this.SchulwochenCollectionChanged;
    }

    /// <summary>
    /// Holt den underlying Jahrtyp this ViewModel is based on
    /// </summary>
    public Jahrtyp Model { get; private set; }

    /// <summary>
    /// Holt die Schulwochen für den Jahrtyp.
    /// </summary>
    public ObservableCollection<SchulwocheViewModel> Schulwochen { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected Schulwoche
    /// </summary>
    public SchulwocheViewModel CurrentSchulwoche
    {
      get
      {
        return this.currentSchulwoche;
      }

      set
      {
        this.currentSchulwoche = value;
        this.RaisePropertyChanged("CurrentSchulwoche");
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string JahrtypBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "JahrtypBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("JahrtypBezeichnung");
      }
    }

    /// <summary>
    /// Holt eine Kurzbezeichnung des Schuljahrs
    /// </summary>
    [DependsUpon("JahrtypBezeichnung")]
    public string JahrtypKurzbezeichnung
    {
      get
      {
        return string.Format("{0}/{1}", this.JahrtypJahr - 2000, this.JahrtypJahr - 2000 + 1);
      }
    }

    /// <summary>
    /// Holt oder setzt die Jahr
    /// </summary>
    public int JahrtypJahr
    {
      get
      {
        return this.Model.Jahr;
      }

      set
      {
        if (value == this.Model.Jahr) return;
        this.UndoablePropertyChanging(this, "JahrtypJahr", this.Model.Jahr, value);
        this.Model.Jahr = value;
        this.RaisePropertyChanged("JahrtypJahr");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Jahrtyp: " + this.JahrtypBezeichnung;
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
      var compareJahrtyp = viewModel as JahrtypViewModel;
      if (compareJahrtyp == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareJahrtyp.JahrtypBezeichnung, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tritt auf, wenn die SchulwochenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SchulwochenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Schulwochen", this.Schulwochen, e, "Änderung der Schulwochen");
    }
  }
}
