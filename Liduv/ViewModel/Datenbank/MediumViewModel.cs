namespace Liduv.ViewModel.Datenbank
{
  using System;

  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Medium
  /// </summary>
  public class MediumViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initializes a new instance of the MediumViewModel class.
    /// </summary>
    /// <param name="medium">The underlying medium this ViewModel is to be based on</param>
    public MediumViewModel(Medium medium)
    {
      if (medium == null)
      {
        throw new ArgumentNullException("medium");
      }

      this.Model = medium;
    }

    /// <summary>
    /// Holt den underlying Medium this ViewModel is based on
    /// </summary>
    public Medium Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string MediumBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "MediumBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("MediumBezeichnung");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Medium: " + this.MediumBezeichnung;
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
      var compareMedium = viewModel as MediumViewModel;
      if (compareMedium == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareMedium.MediumBezeichnung, StringComparison.Ordinal);
    }

  }
}
