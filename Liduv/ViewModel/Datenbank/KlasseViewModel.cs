namespace Liduv.ViewModel.Datenbank
{
  using System;

  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Klasse
  /// </summary>
  public class KlasseViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="KlasseViewModel"/> Klasse. 
    /// </summary>
    /// <param name="klasse">
    /// The underlying klasse this ViewModel is to be based on
    /// </param>
    public KlasseViewModel(Klasse klasse)
    {
      if (klasse == null)
      {
        throw new ArgumentNullException("klasse");
      }

      this.Model = klasse;
    }

    /// <summary>
    /// Holt den underlying Klasse this ViewModel is based on
    /// </summary>
    public Klasse Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string KlasseBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "KlasseBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("KlasseBezeichnung");
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherKlasseViewModel = obj as KlasseViewModel;
      if (otherKlasseViewModel != null)
      {
        return StringLogicalComparer.Compare(this.KlasseBezeichnung, otherKlasseViewModel.KlasseBezeichnung);
      }

      throw new ArgumentException("Object is not a KlasseViewModel");
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Klasse: " + this.KlasseBezeichnung;
    }
  }
}