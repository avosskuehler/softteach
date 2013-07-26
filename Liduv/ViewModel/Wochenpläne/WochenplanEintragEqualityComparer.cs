// <copyright file="WochenplanEintragEqualityComparer.cs" company="Paul Natorp Gymnasium, Berlin">        
// LEUDA - Lehrerunterrichtsdatenbank
// Copyright (C) 2013 Dr. Adrian Voßkühler
// -----------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published  
// by the Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version. This program is distributed in the 
// hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// ***********************************************************************
// </copyright>
// <author>Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>

namespace Liduv.ViewModel.Wochenpläne
{
  using System.Collections.Generic;

  /// <summary>
  /// Custom comparer for the WochenplanEintrag class
  /// </summary>
  internal class WochenplanEintragEqualityComparer : IEqualityComparer<WochenplanEintrag>
  {
    /// <summary>
    /// WochenplanEintrag are equal if their StundenplaneintragWochentagIndex, StundenplaneintragErsteUnterrichtsstundeIndex,
    /// Stundenanzahl, Thema are equal.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(WochenplanEintrag x, WochenplanEintrag y)
    {
      //Check whether the compared objects reference the same data.
      if (ReferenceEquals(x, y)) return true;

      //Check whether any of the compared objects is null.
      if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

      //Check whether the WochenplanEinträge are equal.
      var returnvalue = x.WochentagIndex == y.WochentagIndex
        && x.ErsteUnterrichtsstundeIndex == y.ErsteUnterrichtsstundeIndex
        && x.Stundenanzahl == y.Stundenanzahl
        && x.WochenplaneintragThema.GetHashCode() == y.WochenplaneintragThema.GetHashCode();

      return returnvalue;
    }

    /// <summary>
    /// If Equals() returns true for a pair of objects 
    /// then GetHashCode() must return the same value for these objects.
    /// </summary>
    /// <param name="wochenplanEintrag"></param>
    /// <returns></returns>
    public int GetHashCode(WochenplanEintrag wochenplanEintrag)
    {
      //Check whether the object is null
      if (ReferenceEquals(wochenplanEintrag, null)) return 0;

      //Get hash code for the WochenplaneintragThema field if it is not null.
      int hashWochenplaneintragThema = wochenplanEintrag.WochenplaneintragThema == null ? 0 : wochenplanEintrag.WochenplaneintragThema.GetHashCode();

      //Get hash code for the StundenplaneintragWochentagIndex field.
      int hashWochentagIndex = wochenplanEintrag.WochentagIndex.GetHashCode();

      //Get hash code for the StundenplaneintragErsteUnterrichtsstundeIndex field.
      int hashErsteUnterrichtsstundeIndex = wochenplanEintrag.ErsteUnterrichtsstundeIndex.GetHashCode();

      //Get hash code for the StundenplaneintragErsteUnterrichtsstundeIndex field.
      int hashStundenanzahl = wochenplanEintrag.Stundenanzahl.GetHashCode();

      //Calculate the hash code for the WochenplanEintrag.
      //return hashWochenplaneintragThema ^ hashWochentagIndex ^ hashErsteUnterrichtsstundeIndex ^ hashStundenanzahl;
      return hashWochentagIndex ^ hashErsteUnterrichtsstundeIndex ^ hashStundenanzahl ^ hashWochenplaneintragThema;
    }
  }
}