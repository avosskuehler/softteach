namespace Liduv.ViewModel.Noten
{
  /// <summary>
  /// Diese Enumeration beschreibt die verschiedenen Typen
  /// von Noten.
  /// </summary>
  public enum Notentyp
  {
    /// <summary>
    /// Gesamte mündliche Note
    /// </summary>
    MündlichGesamt,

    /// <summary>
    /// Note für die Qualität mündlicher Beteiligung
    /// </summary>
    MündlichQualität,

    /// <summary>
    /// Note für die Quantität mündlicher Beteiligung
    /// </summary>
    MündlichQuantität,

    /// <summary>
    /// Note für weitere mündliche Noten.
    /// </summary>
    MündlichSonstige,

    /// <summary>
    /// Note für schriftliche Leistungen wie Klausur oder Klassenarbeit
    /// </summary>
    SchriftlichKlassenarbeit,

    /// <summary>
    /// Note für weitere schriftliche Leistungen wie Tests, Projekte
    /// </summary>
    SchriftlichSonstige,

    /// <summary>
    /// Gesamtnote für schriftliche Leistung
    /// </summary>
    SchriftlichGesamt
  }
}
