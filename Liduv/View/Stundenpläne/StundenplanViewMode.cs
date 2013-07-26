// -----------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Liduv.View.Stundenpläne
{
  using System;

  /// <summary>
  /// This enumeration contains the possible view modes for the Stundenplan
  /// </summary>
  [Flags]
  public enum StundenplanViewMode
  {
    /// <summary>
    /// The default view, with disabled edit and disabled drag drop,
    /// just view
    /// </summary>
    Default = 1,

    /// <summary>
    /// This mode allows editing and creation of new stundenplaneinträgen
    /// </summary>
    Edit = 2,

    /// <summary>
    /// This mode allows drag and drop of exisiting stundenplaneinträgen
    /// </summary>
    DragDrop = 4
  }
}
