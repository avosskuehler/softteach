namespace SoftTeach.ViewModel.Stundenpläne
{
  /// <summary>
  /// This enumeration list the possibilities for termin updates
  /// </summary>
  public enum StundenplanÄnderungUpdateType
  {
    /// <summary>
    /// The stundenplaneintrag was newly added.
    /// </summary>
    Added,

    /// <summary>
    /// The stundenplaneintrag was deleted.
    /// </summary>
    Removed,

    /// <summary>
    /// The stundenplaneintrags time slot
    /// </summary>
    ChangedTimeSlot,

    /// <summary>
    /// The stundenplaneintrags Klasse property changed
    /// </summary>
    ChangedKlasse,

    /// <summary>
    /// The stundenplaneintrags room property changed
    /// </summary>
    ChangedRoom,
  }
}
