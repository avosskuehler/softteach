namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Tendenz
  /// </summary>
  public class TendenzViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TendenzViewModel"/> Klasse. 
    /// </summary>
    /// <param name="tendenz">
    /// The underlying tendenz this ViewModel is to be based on
    /// </param>
    public TendenzViewModel(Tendenz tendenz)
    {
      if (tendenz == null)
      {
        throw new ArgumentNullException("tendenz");
      }

      this.Model = tendenz;
    }

    /// <summary>
    /// Holt den underlying Tendenz this ViewModel is based on
    /// </summary>
    public Tendenz Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string TendenzBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "TendenzBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("TendenzBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die Wichtung
    /// </summary>
    public int TendenzWichtung
    {
      get
      {
        return this.Model.Wichtung;
      }

      set
      {
        if (value == this.Model.Wichtung) return;
        this.UndoablePropertyChanging(this, "TendenzWichtung", this.Model.Wichtung, value);
        this.Model.Wichtung = value;
        this.RaisePropertyChanged("TendenzWichtung");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Tendenz: " + this.TendenzBezeichnung;
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
      var compareTendenz = viewModel as TendenzViewModel;
      if (compareTendenz == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareTendenz.TendenzBezeichnung, StringComparison.Ordinal);
    }

  }
}
