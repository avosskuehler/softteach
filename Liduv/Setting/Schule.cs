// <copyright file="Schule.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace Liduv.Setting
{
  using System.Runtime.Serialization;

  /// <summary>
  /// TODO: Update summary.
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