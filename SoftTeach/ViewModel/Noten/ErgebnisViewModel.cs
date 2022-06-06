namespace SoftTeach.ViewModel.Noten
{
  using System;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual ergebnis
  /// </summary>
  public class ErgebnisViewModel : ViewModelBase
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="ErgebnisViewModel"/> Klasse. 
    /// </summary>
    /// <param name="ergebnis">
    /// The underlying ergebnis this ViewModel is to be based on
    /// </param>
    public ErgebnisViewModel(Ergebnis ergebnis)
    {
      this.Model = ergebnis ?? throw new ArgumentNullException(nameof(ergebnis));
    }

    /// <summary>
    /// Holt the underlying Ergebnis this ViewModel is based on
    /// </summary>
    public Ergebnis Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Punktzahl
    /// </summary>
    public double? ErgebnisPunktzahl
    {
      get
      {
        return this.Model.Punktzahl;
      }

      set
      {
        if (value == this.Model.Punktzahl) return;
        this.UndoablePropertyChanging(this, nameof(ErgebnisPunktzahl), this.Model.Punktzahl, value);
        this.Model.Punktzahl = value;
        this.RaisePropertyChanged("ErgebnisPunktzahl");
        Selection.Instance.Schülereintrag.UpdateNoten();
        Selection.Instance.Arbeit.BerechneNotenspiegel();
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Ergebnis: " + this.ErgebnisPunktzahl;
    }
  }
}
