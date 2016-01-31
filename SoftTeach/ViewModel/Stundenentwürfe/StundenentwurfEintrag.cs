namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;

  using SoftTeach.ViewModel.Jahrespläne;

  /// <summary>
  /// </summary>
  public class StundenentwurfEintrag
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenentwurfEintrag"/> Klasse.
    /// </summary>
    /// <param name="inhalt"> Der Jahresplan des Stundenentwurfs.</param>
    /// <param name="medium"> Der Stundenentwurf des Stundenentwurf.</param>
    /// <param name="termin"> Der Termin des Stundenentwurfs.</param>
    public StundenentwurfEintrag(JahresplanViewModel jahresplanViewModel, StundenentwurfViewModel stundenentwurfViewModel, DateTime termin)
    {
      this.Jahresplan = jahresplanViewModel;
      this.Stundenentwurf = stundenentwurfViewModel;
      this.Termin = termin;
    }

    /// <summary>
    /// Holt oder setzt den Jahresplan.
    /// </summary>
    public JahresplanViewModel Jahresplan { get; set; }

    /// <summary>
    /// Holt oder setzt den Stundenentwurf
    /// </summary>
    public StundenentwurfViewModel Stundenentwurf { get; set; }

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
