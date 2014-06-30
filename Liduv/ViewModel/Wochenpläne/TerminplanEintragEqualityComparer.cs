namespace Liduv.ViewModel.Wochenpläne
{
  using System.Collections.Generic;

  /// <summary>
  /// Custom comparer for the WochenplanEintrag class
  /// </summary>
  internal class TerminplanEintragEqualityComparer : IEqualityComparer<TerminplanEintrag>
  {
    /// <summary>
    /// WochenplanEintrag are equal if their StundenplaneintragWochentagIndex, StundenplaneintragErsteUnterrichtsstundeIndex,
    /// Stundenanzahl, Thema are equal.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(TerminplanEintrag x, TerminplanEintrag y)
    {
      //Check whether the compared objects reference the same data.
      if (ReferenceEquals(x, y)) return true;

      //Check whether any of the compared objects is null.
      if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

      //Check whether the WochenplanEinträge are equal.
      var returnvalue = x.WochentagIndex == y.WochentagIndex
        && x.ErsteUnterrichtsstundeIndex == y.ErsteUnterrichtsstundeIndex
        && x.Stundenanzahl == y.Stundenanzahl
        && x.TerminplaneintragThema.GetHashCode() == y.TerminplaneintragThema.GetHashCode();

      return returnvalue;
    }

    /// <summary>
    /// If Equals() returns true for a pair of objects 
    /// then GetHashCode() must return the same value for these objects.
    /// </summary>
    /// <param name="terminplanEintrag"></param>
    /// <returns></returns>
    public int GetHashCode(TerminplanEintrag terminplanEintrag)
    {
      //Check whether the object is null
      if (ReferenceEquals(terminplanEintrag, null)) return 0;

      //Get hash code for the WochenplaneintragThema field if it is not null.
      int hashWochenplaneintragThema = terminplanEintrag.TerminplaneintragThema == null ? 0 : terminplanEintrag.TerminplaneintragThema.GetHashCode();

      //Get hash code for the StundenplaneintragWochentagIndex field.
      int hashWochentagIndex = terminplanEintrag.WochentagIndex.GetHashCode();

      //Get hash code for the StundenplaneintragErsteUnterrichtsstundeIndex field.
      int hashErsteUnterrichtsstundeIndex = terminplanEintrag.ErsteUnterrichtsstundeIndex.GetHashCode();

      //Get hash code for the StundenplaneintragErsteUnterrichtsstundeIndex field.
      int hashStundenanzahl = terminplanEintrag.Stundenanzahl.GetHashCode();

      //Calculate the hash code for the WochenplanEintrag.
      //return hashWochenplaneintragThema ^ hashWochentagIndex ^ hashErsteUnterrichtsstundeIndex ^ hashStundenanzahl;
      return hashWochentagIndex ^ hashErsteUnterrichtsstundeIndex ^ hashStundenanzahl ^ hashWochenplaneintragThema;
    }
  }
}