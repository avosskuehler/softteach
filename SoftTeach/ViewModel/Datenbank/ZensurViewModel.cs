namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Zensur
  /// </summary>
  public class ZensurViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ZensurViewModel"/> Klasse. 
    /// </summary>
    /// <param name="zensur">
    /// The underlying zensur this ViewModel is to be based on
    /// </param>
    public ZensurViewModel(Zensur zensur)
    {
      this.Model = zensur ?? throw new ArgumentNullException(nameof(zensur));
    }

    /// <summary>
    /// Holt den underlying Zensur this ViewModel is based on
    /// </summary>
    public Zensur Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Notenpunkte
    /// </summary>
    public int ZensurNotenpunkte
    {
      get
      {
        return this.Model.Notenpunkte;
      }

      set
      {
        if (value == this.Model.Notenpunkte) return;
        this.UndoablePropertyChanging(this, nameof(ZensurNotenpunkte), this.Model.Notenpunkte, value);
        this.Model.Notenpunkte = value;
        this.RaisePropertyChanged("ZensurNotenpunkte");
      }
    }

    /// <summary>
    /// Holt oder setzt die GanzeNote
    /// </summary>
    public int ZensurGanzeNote
    {
      get
      {
        return this.Model.GanzeNote;
      }

      set
      {
        if (value == this.Model.GanzeNote) return;
        this.UndoablePropertyChanging(this, nameof(ZensurGanzeNote), this.Model.GanzeNote, value);
        this.Model.GanzeNote = value;
        this.RaisePropertyChanged("ZensurGanzeNote");
      }
    }

    /// <summary>
    /// Holt oder setzt die NoteMitTendenz
    /// </summary>
    public string ZensurNoteMitTendenz
    {
      get
      {
        return this.Model.NoteMitTendenz;
      }

      set
      {
        if (value == this.Model.NoteMitTendenz) return;
        this.UndoablePropertyChanging(this, nameof(ZensurNoteMitTendenz), this.Model.NoteMitTendenz, value);
        this.Model.NoteMitTendenz = value;
        this.RaisePropertyChanged("ZensurNoteMitTendenz");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Zensur: " + this.ZensurNoteMitTendenz;
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
      var compareZensur = viewModel as ZensurViewModel;
      if (compareZensur == null)
      {
        return -1;
      }

      return this.Model.Notenpunkte.CompareTo(compareZensur.ZensurNotenpunkte);
    }
  }
}
