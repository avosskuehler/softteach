namespace SoftTeach.UndoRedo.ChangeTypes
{
  using System;

  /// <summary>
  /// Represents an individual change, with the commands to undo / redo the change as needed.
  /// </summary>
  public class DelegateChange : Change
  {
    /// <summary>
    /// The undo action.
    /// </summary>
    private readonly Action undoAction;

    /// <summary>
    /// The redo action.
    /// </summary>
    private Action redoAction;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="DelegateChange"/> Klasse. 
    /// </summary>
    /// <param name="target"> The object that this change affects. </param>
    /// <param name="undoAction"> The delegate that will do the Undo logic </param>
    /// <param name="redoAction"> The delegate that will do the Redo logic </param>
    /// <param name="changeKey">
    /// An object, that will be used to detect changes that affect the same "field". 
    /// This object should implement or override object.Equals() and return true if the changes are for the same field.
    /// This is used when the undo UndoRoot has started a batch, or when the UndoRoot.ConsolidateChangesForSameInstance is true.
    /// A string will work, but should be sufficiently unique within the scope of changes that affect this Target instance.
    /// Another good option is to use the Tuple class to uniquely identify the change. The Tuple could contain
    /// the object, and a string representing the property name. For a collection change, you might include the 
    /// instance, the property name, and the item added/removed from the collection.
    /// </param>
    /// <param name="isDataContextRelevant"> The is Data Context Relevant. </param>
    /// <param name="description"> The description. </param>
    public DelegateChange(object target, Action undoAction, Action redoAction, object changeKey, bool isDataContextRelevant, string description)
      : base(target, changeKey, isDataContextRelevant, description)
    {
      this.undoAction = undoAction; // new WeakReference(undoAction);
      this.redoAction = redoAction; // new WeakReference(redoAction);
    }

    /// <summary>
    /// When consolidating events, we want to keep the original "Undo"
    /// but use the most recent Redo. This will pull the Redo from the 
    /// specified Change and apply it to this instance.
    /// </summary>
    /// <param name="latestChange">The latest Change.</param>
    public override void MergeWith(Change latestChange)
    {
      var other = latestChange as DelegateChange;

      if (null != other)
        this.redoAction = other.redoAction;
    }

    /// <summary>
    /// Performs the actual Undo logic.
    /// </summary>
    protected override void PerformUndo()
    {
      var action = this.undoAction;
      if (null != action) action();
    }

    /// <summary>
    /// Performs the actual redo logic.
    /// </summary>
    protected override void PerformRedo()
    {
      var action = this.redoAction;
      if (null != action) action();
    }
  }
}