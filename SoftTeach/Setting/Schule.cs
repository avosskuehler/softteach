namespace SoftTeach.Setting
{
  using System.Runtime.Serialization;

  /// <summary>
  /// Klasse für Schuleigenschaften
  /// </summary>
  [DataContract(Name = "Schule", Namespace = "http://www.vosskuehler.name")]
  public class Schule
  {
    #region Public Properties

    /// <summary>
    /// Gets or sets Hausnummer.
    /// </summary>
    [DataMember]
    public string Hausnummer { get; set; }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets Postleitzahl.
    /// </summary>
    [DataMember]
    public string Postleitzahl { get; set; }

    /// <summary>
    /// Gets or sets Stadt.
    /// </summary>
    [DataMember]
    public string Stadt { get; set; }

    /// <summary>
    /// Gets or sets Strasse.
    /// </summary>
    [DataMember]
    public string Strasse { get; set; }

    #endregion
  }
}