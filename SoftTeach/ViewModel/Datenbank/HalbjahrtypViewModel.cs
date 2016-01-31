namespace SoftTeach.ViewModel.Datenbank
{
  using System;

  using SoftTeach.Model;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Halbjahrtyp
  /// </summary>
  public class HalbjahrtypViewModel : ViewModelBase
  {
    /// <summary>
    /// Initializes a new instance of the HalbjahrtypViewModel class.
    /// </summary>
    /// <param name="halbjahrtyp">The underlying halbjahrtyp this ViewModel is to be based on</param>
    public HalbjahrtypViewModel(Halbjahrtyp halbjahrtyp)
    {
      if (halbjahrtyp == null)
      {
        throw new ArgumentNullException("halbjahrtyp");
      }

      this.Model = halbjahrtyp;
    }

    /// <summary>
    /// Holt den underlying Halbjahrtyp this ViewModel is based on
    /// </summary>
    public Halbjahrtyp Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string HalbjahrtypBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "HalbjahrtypBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("HalbjahrtypBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt den Halbjahrindex
    /// </summary>
    public int HalbjahrtypIndex
    {
      get
      {
        return this.Model.HalbjahrIndex;
      }

      set
      {
        if (value == this.Model.HalbjahrIndex) return;
        this.UndoablePropertyChanging(this, "HalbjahrtypIndex", this.Model.HalbjahrIndex, value);
        this.Model.HalbjahrIndex = value;
        this.RaisePropertyChanged("HalbjahrtypIndex");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Halbjahrtyp: " + this.HalbjahrtypBezeichnung;
    }
  }
}
