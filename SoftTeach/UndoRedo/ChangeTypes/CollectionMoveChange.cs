namespace SoftTeach.UndoRedo.ChangeTypes
{
  using System.Collections;
  using System.Diagnostics;

  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class CollectionMoveChange : CollectionChange
  {
    #region Member Variables

    private int redoNewIndex;
    private int redoOldIndex;

    #endregion

    #region Constructors

    public CollectionMoveChange(
      object target,
      string propertyName,
      IList collection,
      int newIndex,
      int oldIndex,
      bool isDataContextRelevant,
      string description)
      : base(
        target,
        propertyName,
        collection,
        new ChangeKey<object, string, object>(target, propertyName, new ChangeKey<int, int>(oldIndex, newIndex)),
        isDataContextRelevant,
        description)
    {
      this.NewIndex = newIndex;
      this.OldIndex = oldIndex;

      this.redoNewIndex = newIndex;
      this.redoOldIndex = oldIndex;
    }

    #endregion

    #region Public Properties

    public int NewIndex { get; private set; }

    public int OldIndex { get; private set; }

    #endregion

    #region Public Methods

    public override void MergeWith(Change latestChange)
    {
      var other = latestChange as CollectionMoveChange;

      if (null != other)
      {
        this.redoOldIndex = other.redoOldIndex;
        this.redoNewIndex = other.redoNewIndex;
      }
      // FIXME should only affect undo
    }

    #endregion

    #region Internal

    protected override void PerformUndo()
    {
      this.Collection.GetType().GetMethod("Move").Invoke(this.Collection, new object[] { this.NewIndex, this.OldIndex });
    }

    protected override void PerformRedo()
    {
      this.Collection.GetType().GetMethod("Move").Invoke(this.Collection, new object[] { this.redoOldIndex, this.redoNewIndex });
    }

    private string DebuggerDisplay
    {
      get
      {
        return string.Format(
          "CollectionMoveChange(Property={0}, Target={{{1}}}, OldIndex={2}, NewIndex={{{3}}})",
          this.PropertyName,
          this.Target,
          this.OldIndex,
          this.NewIndex);
      }
    }

    #endregion
  }
}