namespace Liduv.ViewModel.Datenbank
{
  using System;
  using System.Windows.Media;

  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Termintyp
  /// </summary>
  public class TermintypViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TermintypViewModel"/> Klasse. 
    /// </summary>
    /// <param name="termintyp">
    /// The underlying Termintyp this ViewModel is to be based on
    /// </param>
    public TermintypViewModel(Termintyp termintyp)
    {
      if (termintyp == null)
      {
        throw new ArgumentNullException("termintyp");
      }

      this.Model = termintyp;
    }

    /// <summary>
    /// Holt den underlying Termintyp this ViewModel is based on
    /// </summary>
    public Termintyp Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string TermintypBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "TermintypBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("TermintypBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die bezeichnung of this Termintyp
    /// </summary>
    public Color TermintypKalenderfarbe
    {
      get
      {
        if (this.Model.Kalenderfarbe != null)
        {
          return (Color)ColorConverter.ConvertFromString(this.Model.Kalenderfarbe);
        }

        return Colors.Black;
      }

      set
      {
        if (value.ToString() == this.Model.Kalenderfarbe) return;
        this.UndoablePropertyChanging(this, "TermintypKalenderfarbe", this.Model.Kalenderfarbe, value.ToString());
        this.Model.Kalenderfarbe = value.ToString();
        this.RaisePropertyChanged("TermintypKalenderfarbe");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Termintyp: " + this.TermintypBezeichnung;
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
      var compareTermintyp = viewModel as TermintypViewModel;
      if (compareTermintyp == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareTermintyp.TermintypBezeichnung, StringComparison.Ordinal);
    }

  }
}
