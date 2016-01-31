// -----------------------------------------------------------------------
// <copyright file="Bepunktungstyp.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// Diese Enumeration beschreibt die drei Möglichkeiten
  /// ein und dieselbe Note zu zeigen.
  /// </summary>
  public enum Bepunktungstyp
  {
    /// <summary>
    /// Ganze Noten der Form 1,2,3,4,5,6
    /// </summary>
    GanzeNote,

    /// <summary>
    /// Noten mit Tendenz der Form 1+,1,1-,2+,2,2- etc.
    /// </summary>
    NoteMitTendenz,

    /// <summary>
    /// Notenpunkte der Oberstufe 15,14,14,12,11,10 etc.
    /// </summary>
    Notenpunkte
  }
}
