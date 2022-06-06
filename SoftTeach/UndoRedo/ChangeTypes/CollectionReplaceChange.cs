namespace SoftTeach.UndoRedo.ChangeTypes
{
  using System.Collections;
  using System.Diagnostics;

  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class CollectionReplaceChange : CollectionChange
  {
    /// <summary>
    /// The redo index.
    /// </summary>
    private int redoIndex;

    /// <summary>
    /// The redo new item.
    /// </summary>
    private object redoNewItem;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="CollectionReplaceChange"/> Klasse.
    /// </summary>
    /// <param name="target"> The target. </param>
    /// <param name="propertyName"> The property name. </param>
    /// <param name="collection"> The collection. </param>
    /// <param name="index"> The index. </param>
    /// <param name="oldItem"> The old item. </param>
    /// <param name="newItem"> The new item. </param>
    /// <param name="isDataContextRelevant"> The is data context relevant. </param>
    /// <param name="description"> The description. </param>
    public CollectionReplaceChange(
      object target,
      string propertyName,
      IList collection,
      int index,
      object oldItem,
      object newItem,
      bool isDataContextRelevant,
      string description)
      : base(
        target,
        propertyName,
        collection,
        new ChangeKey<object, string, object>(target, propertyName, new ChangeKey<object, object>(oldItem, newItem)),
        isDataContextRelevant,
        description)
    {
      this.Index = index;
      this.OldItem = oldItem;
      this.NewItem = newItem;

      this.redoIndex = index;
      this.redoNewItem = newItem;
    }

    /// <summary>
    /// Holt den index.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// Holt the old item.
    /// </summary>
    public object OldItem { get; private set; }

    /// <summary>
    /// Holt the new item.
    /// </summary>
    public object NewItem { get; private set; }

    /// <summary>
    /// Holt the debugger display.
    /// </summary>
    private string DebuggerDisplay
    {
      get
      {
        return string.Format(
          "CollectionReplaceChange(Property={0}, Target={{{1}}}, Index={2}, NewItem={{{3}}}, OldItem={{{4}}})",
          this.PropertyName,
          this.Target,
          this.Index,
          this.NewItem,
          this.OldItem);
      }
    }

    /// <summary>
    /// Führt zwei Änderungen zusammen.
    /// </summary>
    /// <param name="latestChange"> The latest change. </param>
    public override void MergeWith(Change latestChange)
    {
      var other = latestChange as CollectionReplaceChange;

      if (null != other)
      {
        this.redoIndex = other.redoIndex;
        this.redoNewItem = other.redoNewItem;
      }
    }

    protected override void PerformUndo()
    {
      this.Collection[this.Index] = this.OldItem;
    }

    protected override void PerformRedo()
    {
      this.Collection[this.redoIndex] = this.redoNewItem;
    }
  }
}