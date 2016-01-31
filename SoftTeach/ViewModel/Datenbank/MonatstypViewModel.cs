namespace SoftTeach.ViewModel.Datenbank
{
  using System;

  using SoftTeach.Model;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Monatstyp
  /// </summary>
  public class MonatstypViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MonatstypViewModel"/> Klasse. 
    /// </summary>
    /// <param name="monatstyp">
    /// The underlying Monatstyp this ViewModel is to be based on
    /// </param>
    public MonatstypViewModel(Monatstyp monatstyp)
    {
      if (monatstyp == null)
      {
        throw new ArgumentNullException("monatstyp");
      }

      this.Model = monatstyp;
    }

    /// <summary>
    /// Holt den underlying Monatstyp this ViewModel is based on
    /// </summary>
    public Monatstyp Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string MonatstypBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "MonatstypBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("MonatstypBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die Index
    /// </summary>
    public int MonatstypIndex
    {
      get
      {
        return this.Model.MonatIndex;
      }

      set
      {
        if (value == this.Model.MonatIndex) return;
        this.UndoablePropertyChanging(this, "MonatstypIndex", this.Model.MonatIndex, value);
        this.Model.MonatIndex = value;
        this.RaisePropertyChanged("MonatstypIndex");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Monatstyp: " + this.MonatstypBezeichnung;
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
      var compareMonatstyp = viewModel as MonatstypViewModel;
      if (compareMonatstyp == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareMonatstyp.MonatstypBezeichnung, StringComparison.Ordinal);
    }

  }
}
