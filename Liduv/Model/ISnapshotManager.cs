// -----------------------------------------------------------------------
// <copyright file="ISnapshotManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Liduv.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public interface ISnapshotManager
  {
    void TakeSnapshot();
    void Undo();
    void Redo();
    bool CanUndo();
    bool CanRedo();
  }
}
