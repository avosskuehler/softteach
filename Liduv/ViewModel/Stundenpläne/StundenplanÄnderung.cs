namespace Liduv.ViewModel.Stundenpläne
{
  using Liduv.Model.EntityFramework;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class StundenplanÄnderung
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenplanÄnderung"/> Klasse. 
    /// </summary>
    /// <param name="updateType">
    /// The <see cref="StundenplanÄnderungUpdateType"/> that describes the 
    /// modification of the stundenplaneintrag.
    /// </param>
    /// <param name="oldWochentagIndex">
    /// The old Wochentagindex for the änderung.
    /// </param>
    /// <param name="oldErsteStundeIndex">
    /// The old erste Stunde index for the änderung.
    /// </param>
    /// <param name="modifiedEntry">
    /// The new <sdee cref="Stundenplaneintrag"/> for this Stundenplan.
    /// </param>
    public StundenplanÄnderung(
      StundenplanÄnderungUpdateType updateType,
      int oldWochentagIndex,
      int oldErsteStundeIndex,
      StundenplaneintragViewModel modifiedEntry)
    {
      this.UpdateType = updateType;
      this.OldWochentagIndex = oldWochentagIndex;
      this.OldErsteStundeIndex = oldErsteStundeIndex;
      this.ModifiedEntry = modifiedEntry;
    }

    /// <summary>
    /// Holt den <see cref="StundenplanÄnderungUpdateType"/> für diese
    /// Stundenplanänderung.
    /// </summary>
    public StundenplanÄnderungUpdateType UpdateType { get; private set; }

    /// <summary>
    /// Holt den Wochentagindex des ursprünglichen
    /// zu verändernen Stundenplaneintrags.
    /// </summary>
    public int OldWochentagIndex { get; private set; }

    /// <summary>
    /// Holt den Index der ersten Stunde des ursprünglichen
    /// zu verändernen Stundenplaneintrags.
    /// </summary>
    public int OldErsteStundeIndex { get; private set; }

    /// <summary>
    /// Holt den Wochentagindex des ursprünglichen
    /// zu verändernen Stundenplaneintrags.
    /// </summary>
    public StundenplaneintragViewModel ModifiedEntry { get; private set; }
  }
}
