namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Linq;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual betroffeneKlasse
  /// </summary>
  public class BetroffeneKlasseViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The klasse currently assigned to this betroffeneKlasse
    /// </summary>
    private KlasseViewModel klasse;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="BetroffeneKlasseViewModel"/> Klasse. 
    /// </summary>
    /// <param name="betroffeneKlasse">
    /// The underlying betroffeneKlasse this ViewModel is to be based on
    /// </param>
    public BetroffeneKlasseViewModel(BetroffeneKlasse betroffeneKlasse)
    {
      if (betroffeneKlasse == null)
      {
        throw new ArgumentNullException("betroffeneKlasse");
      }

      this.Model = betroffeneKlasse;
     }

    /// <summary>
    /// Holt den underlying BetroffeneKlasse this ViewModel is based on
    /// </summary>
    public BetroffeneKlasse Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public KlasseViewModel BetroffeneKlasseKlasse
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Klasse == null && this.klasse != null)
        {
          return this.klasse;
        }

        if (this.klasse == null || this.klasse.Model != this.Model.Klasse)
        {
          this.klasse = App.MainViewModel.Klassen.SingleOrDefault(d => d.Model == this.Model.Klasse);
        }

        return this.klasse;
      }

      set
      {
        if (value.KlasseBezeichnung == this.klasse.KlasseBezeichnung) return;
        this.UndoablePropertyChanging(this, "BetroffeneKlasseKlasse", this.klasse, value);
        this.klasse = value;
        this.Model.Klasse = value.Model;
        this.RaisePropertyChanged("BetroffeneKlasseKlasse");
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherViewModel = obj as BetroffeneKlasseViewModel;
      if (otherViewModel != null)
      {
        return StringLogicalComparer.Compare(this.Model.Klasse.Bezeichnung, otherViewModel.Model.Klasse.Bezeichnung);
      }

      throw new ArgumentException("Object is not a BetroffeneKlasseViewModel");
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "BetroffeneKlasse: " + this.BetroffeneKlasseKlasse.KlasseBezeichnung;
    }
  }
}
