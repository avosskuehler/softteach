namespace Liduv.ViewModel.Datenbank
{
  using System;

  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Tendenztyp
  /// </summary>
  public class TendenztypViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TendenztypViewModel"/> Klasse. 
    /// </summary>
    /// <param name="tendenztyp">
    /// The underlying tendenztyp this ViewModel is to be based on
    /// </param>
    public TendenztypViewModel(Tendenztyp tendenztyp)
    {
      if (tendenztyp == null)
      {
        throw new ArgumentNullException("tendenztyp");
      }

      this.Model = tendenztyp;
    }

    /// <summary>
    /// Holt den underlying Tendenztyp this ViewModel is based on
    /// </summary>
    public Tendenztyp Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string TendenztypBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "TendenztypBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("TendenztypBezeichnung");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Tendenztyp: " + this.TendenztypBezeichnung;
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
      var compareTendenztyp = viewModel as TendenztypViewModel;
      if (compareTendenztyp == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareTendenztyp.TendenztypBezeichnung, StringComparison.Ordinal);
    }

  }
}
