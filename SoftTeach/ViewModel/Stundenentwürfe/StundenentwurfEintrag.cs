namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;

  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Jahrespläne;

  /// <summary>
  /// </summary>
  public class StundenentwurfEintrag
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenentwurfEintrag" /> Klasse.
    /// </summary>
    /// <param name="jahresplanViewModel">The jahresplan view model.</param>
    /// <param name="klasse">The klasse.</param>
    /// <param name="stundenentwurfViewModel">The stundenentwurf view model.</param>
    /// <param name="termin">Der Termin des Stundenentwurfs.</param>
    public StundenentwurfEintrag(JahresplanViewModel jahresplanViewModel, string klasse, StundenentwurfViewModel stundenentwurfViewModel, DateTime? termin)
    {
      this.Jahresplan = jahresplanViewModel;
      this.Klasse = klasse;
      this.Stundenentwurf = stundenentwurfViewModel;
      this.Termin = termin;
    }

    /// <summary>
    /// Holt oder setzt den Jahresplan.
    /// </summary>
    public JahresplanViewModel Jahresplan { get; set; }

    /// <summary>
    /// Holt oder setzt die Klasse.
    /// </summary>
    public string Klasse { get; set; }

    /// <summary>
    /// Holt oder setzt den Stundenentwurf
    /// </summary>
    public StundenentwurfViewModel Stundenentwurf { get; set; }

    /// <summary>
    /// Gets or sets the termin.
    /// </summary>
    /// <value>The termin.</value>
    public DateTime? Termin { get; set; }

    /// <summary>
    /// Holt den termin as a formatted string
    /// </summary>
    /// <value>The termin.</value>
    public string TerminString
    {
      get
      {
        if (this.Termin.HasValue)
        {
          return this.Termin.Value.ToString("ddd dd.MM.yy");
        }

        return string.Empty;
      }
    }
  }
}
