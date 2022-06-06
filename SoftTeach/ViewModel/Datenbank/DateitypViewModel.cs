namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Dateityp
  /// </summary>
  public class DateitypViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="DateitypViewModel"/> Klasse. 
    /// </summary>
    /// <param name="dateityp">
    /// The underlying dateityp this ViewModel is to be based on
    /// </param>
    public DateitypViewModel(Dateityp dateityp)
    {
      this.Model = dateityp ?? throw new ArgumentNullException(nameof(dateityp));
    }

    /// <summary>
    /// Holt den underlying Dateityp this ViewModel is based on
    /// </summary>
    public Dateityp Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die bezeichnung of this Dateityp
    /// </summary>
    public string DateitypBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(DateitypBezeichnung), this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("DateitypBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die Kürzel of this Dateityp
    /// </summary>
    public string DateitypKürzel
    {
      get
      {
        return this.Model.Kürzel;
      }

      set
      {
        if (value == this.Model.Kürzel)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(DateitypKürzel), this.Model.Kürzel, value);
        this.Model.Kürzel = value;
        this.RaisePropertyChanged("DateitypKürzel");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Dateityp: " + this.DateitypBezeichnung;
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
      var compareDateityp = viewModel as DateitypViewModel;
      if (compareDateityp == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareDateityp.DateitypBezeichnung, StringComparison.Ordinal);
    }
  }
}
