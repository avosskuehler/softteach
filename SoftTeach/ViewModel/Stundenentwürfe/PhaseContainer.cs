namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;

  /// <summary>
  /// Diese Klasse nimmt die serialisierbare Kopie einer PhasenEntity auf
  /// um sie in die Zwischenablage zu exportieren.
  /// </summary>
  [Serializable]
  public class PhaseContainer
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="PhaseContainer"/> Klasse.
    /// </summary>
    /// <param name="inhalt"> Der Inhalt der Phase.</param>
    /// <param name="medium"> Das Medium der Phase.</param>
    /// <param name="sozialform"> Die Sozialform der Phase.</param>
    /// <param name="zeit"> Die Dauer der Phase.</param>
    public PhaseContainer(string inhalt, string medium, string sozialform, int zeit)
    {
      this.Inhalt = inhalt;
      this.Medium = medium;
      this.Sozialform = sozialform;
      this.Zeit = zeit;
    }

    /// <summary>
    /// Holt oder setzt den Inhalt der Phase.
    /// </summary>
    public string Inhalt { get; set; }

    /// <summary>
    /// Holt oder setzt das Medium der Phase.
    /// </summary>
    public string Medium { get; set; }

    /// <summary>
    /// Holt oder setzt die Sozialform der Phase.
    /// </summary>
    public string Sozialform { get; set; }

    /// <summary>
    /// Holt oder setzt die Dauer der Phase.
    /// </summary>
    public int Zeit { get; set; }
  }
}
