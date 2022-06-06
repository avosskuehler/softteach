namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Linq;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// ViewModel of an individual betroffeneLerngruppe
  /// </summary>
  public class BetroffeneLerngruppeViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The klasse currently assigned to this betroffeneLerngruppe
    /// </summary>
    private LerngruppeViewModel lerngruppe;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="BetroffeneLerngruppeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="betroffeneLerngruppe">
    /// The underlying betroffeneLerngruppe this ViewModel is to be based on
    /// </param>
    public BetroffeneLerngruppeViewModel(BetroffeneLerngruppe betroffeneLerngruppe)
    {
      this.Model = betroffeneLerngruppe ?? throw new ArgumentNullException(nameof(betroffeneLerngruppe));
     }

    /// <summary>
    /// Holt den underlying BetroffeneKlasse this ViewModel is based on
    /// </summary>
    public BetroffeneLerngruppe Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public LerngruppeViewModel BetroffeneLerngruppeLerngruppe
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Lerngruppe == null && this.lerngruppe != null)
        {
          return this.lerngruppe;
        }

        if (this.lerngruppe == null || this.lerngruppe.Model != this.Model.Lerngruppe)
        {
          this.lerngruppe = App.MainViewModel.Lerngruppen.SingleOrDefault(d => d.Model == this.Model.Lerngruppe);
        }

        return this.lerngruppe;
      }

      set
      {
        if (value.LerngruppeBezeichnung == this.lerngruppe.LerngruppeBezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(BetroffeneLerngruppeLerngruppe), this.lerngruppe, value);
        this.lerngruppe = value;
        this.Model.Lerngruppe = value.Model;
        this.RaisePropertyChanged("BetroffeneLerngruppeLerngruppe");
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherViewModel = obj as BetroffeneLerngruppeViewModel;
      if (otherViewModel != null)
      {
        return StringLogicalComparer.Compare(this.Model.Lerngruppe.Bezeichnung, otherViewModel.Model.Lerngruppe.Bezeichnung);
      }

      throw new ArgumentException("Object is not a BetroffeneLerngruppeViewModel");
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "BetroffeneLerngruppe: " + this.BetroffeneLerngruppeLerngruppe.LerngruppeBezeichnung;
    }
  }
}
