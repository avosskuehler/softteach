namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Windows.Media;

  using SoftTeach.Model;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual fach
  /// </summary>
  public class FachViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="FachViewModel"/> Klasse. 
    /// </summary>
    /// <param name="fach">
    /// The underlying fach this ViewModel is to be based on
    /// </param>
    public FachViewModel(Fach fach)
    {
      if (fach == null)
      {
        throw new ArgumentNullException("fach");
      }

      this.Model = fach;
    }

    /// <summary>
    /// Holt den underlying Fach this ViewModel is based on
    /// </summary>
    public Fach Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string FachBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "FachBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("FachBezeichnung");
      }
    }

    /// <summary>
    /// Holt eine zweibuchstabige Kurzbezeichnung des Fachs
    /// </summary>
    [DependsUpon("FachBezeichnung")]
    public string FachKurzbezeichnung
    {
      get
      {
        return this.FachBezeichnung.Substring(0, 2);
      }
    }

    /// <summary>
    /// Holt oder setzt die fabre of this fach
    /// </summary>
    public Color FachFarbe
    {
      get
      {
        if (this.Model.Farbe != null)
        {
          return (Color)ColorConverter.ConvertFromString(this.Model.Farbe);
        }

        return Colors.Black;
      }

      set
      {
        if (value.ToString() == this.Model.Farbe) return;
        this.UndoablePropertyChanging(this, "FachFarbe", this.Model.Farbe, value);
        this.Model.Farbe = value.ToString();
        this.RaisePropertyChanged("FachFarbe");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Fach: " + this.FachBezeichnung;
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
      var compareFach = viewModel as FachViewModel;
      if (compareFach == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareFach.FachBezeichnung, StringComparison.Ordinal);
    }
  }
}
