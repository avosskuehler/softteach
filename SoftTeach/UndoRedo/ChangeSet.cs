namespace SoftTeach.UndoRedo
{
  using System.Collections.Generic;
  using System.Linq;

  using SoftTeach.UndoRedo.ChangeTypes;

  /// <summary>
  /// A set of changes that represent a single "unit of change".
  /// </summary>
  public class ChangeSet
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="ChangeSet"/> Klasse. 
    /// Create a ChangeSet for the specified UndoRoot.
    /// </summary>
    /// <param name="undoRoot"> The UndoRoot that this ChangeSet belongs to. </param>
    /// <param name="description"> A description of the change. </param>
    /// <param name="change"> The Change instance that can perform the undo / redo as needed. </param>
    public ChangeSet(UndoRoot undoRoot, string description, Change change)
    {
      this.Undone = false;
      this.UndoRoot = undoRoot;
      this.Changes = new List<Change>();
      this.Description = description;

      if (null != change)
        this.AddChange(change);
    }

    /// <summary>
    /// Holt The associated UndoRoot.
    /// </summary>
    public UndoRoot UndoRoot { get; private set; }

    /// <summary>
    /// Holt A description of this set of changes.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Holt einen Wert, der angibt, ob this ChangeSet been undone.
    /// </summary>
    public bool Undone { get; private set; }

    /// <summary>
    /// Holt The changes that are part of this ChangeSet
    /// </summary>
    public IList<Change> Changes { get; private set; }

    /// <summary>
    /// Add a change to this ChangeSet.
    /// </summary>
    /// <param name="change">Die Änderung die hinzugefügt werden soll.</param>
    internal void AddChange(Change change)
    {
      if (this.UndoRoot.ConsolidateChangesForSameInstance)
      {
        var dupe = this.Changes.FirstOrDefault(c => null != c.ChangeKey && c.ChangeKey.Equals(change.ChangeKey));
        if (null != dupe)
        {
          dupe.MergeWith(change);
        }
        else
        {
          this.Changes.Add(change);
        }
      }
      else
      {
        this.Changes.Add(change);
      }
    }

    /// <summary>
    /// Undo all Changes in this ChangeSet.
    /// </summary>
    internal void Undo()
    {
      foreach (var change in this.Changes.Reverse()) change.Undo();
      this.Undone = true;
    }

    /// <summary>
    /// Redo all Changes in this ChangeSet.
    /// </summary>
    internal void Redo()
    {
      foreach (var change in this.Changes) change.Redo();
      this.Undone = false;
    }
  }
}
