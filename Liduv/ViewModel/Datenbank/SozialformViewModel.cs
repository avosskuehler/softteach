namespace Liduv.ViewModel.Datenbank
{
  using System;

  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual sozialform
  /// </summary>
  public class SozialformViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SozialformViewModel"/> Klasse. 
    /// </summary>
    /// <param name="sozialform"> The underlying sozialform this ViewModel is to be based on </param>
    public SozialformViewModel(Sozialform sozialform)
    {
      if (sozialform == null)
      {
        throw new ArgumentNullException("sozialform");
      }

      this.Model = sozialform;
    }

    /// <summary>
    /// Holt den underlying Sozialform this ViewModel is based on
    /// </summary>
    public Sozialform Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string SozialformBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "SozialformBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("SozialformBezeichnung");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Sozialform: " + this.SozialformBezeichnung;
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
      var compareSozialform = viewModel as SozialformViewModel;
      if (compareSozialform == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareSozialform.SozialformBezeichnung, StringComparison.Ordinal);
    }

  }
}
