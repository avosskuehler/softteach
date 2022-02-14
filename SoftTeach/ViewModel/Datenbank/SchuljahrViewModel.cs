namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Schuljahr
  /// </summary>
  public class SchuljahrViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchuljahrViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schuljahr">
    /// The underlying schuljahr this ViewModel is to be based on
    /// </param>
    public SchuljahrViewModel(SchuljahrNeu schuljahr)
    {
      if (schuljahr == null)
      {
        throw new ArgumentNullException("schuljahr");
      }

      this.Model = schuljahr;
    }

    /// <summary>
    /// Holt den underlying Schuljahr this ViewModel is based on
    /// </summary>
    public SchuljahrNeu Model { get; private set; }

     /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string SchuljahrBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "SchuljahrBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("SchuljahrBezeichnung");
      }
    }

    /// <summary>
    /// Holt eine Kurzbezeichnung des Schuljahrs
    /// </summary>
    [DependsUpon("SchuljahrBezeichnung")]
    public string SchuljahrKurzbezeichnung
    {
      get
      {
        return string.Format("{0}/{1}", this.SchuljahrJahr - 2000, this.SchuljahrJahr - 2000 + 1);
      }
    }

    /// <summary>
    /// Holt oder setzt die Jahr
    /// </summary>
    public int SchuljahrJahr
    {
      get
      {
        return this.Model.Jahr;
      }

      set
      {
        if (value == this.Model.Jahr) return;
        this.UndoablePropertyChanging(this, "SchuljahrJahr", this.Model.Jahr, value);
        this.Model.Jahr = value;
        this.RaisePropertyChanged("SchuljahrJahr");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Schuljahr: " + this.SchuljahrBezeichnung;
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
      var compareSchuljahr = viewModel as SchuljahrViewModel;
      if (compareSchuljahr == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareSchuljahr.SchuljahrBezeichnung, StringComparison.Ordinal);
    }
  }
}
