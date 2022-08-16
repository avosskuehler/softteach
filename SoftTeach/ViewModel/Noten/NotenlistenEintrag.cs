namespace SoftTeach.ViewModel.Noten
{
  using System;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Datenbank;

  /// <summary>
  /// </summary>
  public class NotenlistenEintrag
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NotenlistenEintrag" /> Klasse.
    /// </summary>
    /// <param name="notenTermintyp">Der zeugnisnotentyp.</param>
    /// <param name="jahresplanViewModel">The jahresplan view model.</param>
    /// <param name="termin">Der Termin des Zeugnisses.</param>
    public NotenlistenEintrag(NotenTermintyp notenTermintyp, SchuljahrViewModel schuljahr, DateTime termin)
    {
      this.Schuljahr = schuljahr;
      this.NotenTermintyp = notenTermintyp;
      this.Termin = termin;
    }

    /// <summary>
    /// Holt oder setzt den Jahresplan.
    /// </summary>
    public SchuljahrViewModel Schuljahr { get; set; }

    /// <summary>
    /// Holt oder setzt den Stundenentwurf
    /// </summary>
    public NotenTermintyp NotenTermintyp { get; set; }

    /// <summary>
    /// Gets or sets the termin.
    /// </summary>
    /// <value>The termin.</value>
    public DateTime Termin { get; set; }

    /// <summary>
    /// Holt den termin as a formatted string
    /// </summary>
    /// <value>The termin.</value>
    public string TerminString
    {
      get
      {
        return this.Termin.ToString("ddd dd.MM.yy");
      }
    }
  }
}
