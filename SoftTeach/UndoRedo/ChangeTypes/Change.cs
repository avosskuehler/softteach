namespace SoftTeach.UndoRedo.ChangeTypes
{
  /// <summary>
  /// Represents an individual change, with the commands to undo / redo the change as needed.
  /// </summary>
  public abstract class Change
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="Change"/> Klasse. 
    /// </summary>
    /// <param name="target">
    /// The object that this change affects.
    /// </param>
    /// <param name="changeKey">
    /// An object, that will be used to detect changes that affect the same "field". 
    /// This object should implement or override object.Equals() and return true if the changes are for the same field.
    /// This is used when the undo UndoRoot has started a batch, or when the UndoRoot.ConsolidateChangesForSameInstance is true.
    /// A string will work, but should be sufficiently unique within the scope of changes that affect this Target instance.
    /// Another good option is to use the Tuple class to uniquely identify the change. The Tuple could contain
    /// the object, and a string representing the property name. For a collection change, you might include the 
    /// instance, the property name, and the item added/removed from the collection.
    /// </param>
    /// <param name="isDataContextRelevant">
    /// The is Data Context Relevant.
    /// </param>
    /// <param name="description">
    /// The description for this change which appears in the Stacklist
    /// </param>
    protected Change(object target, object changeKey, bool isDataContextRelevant, string description)
    {
      this.Target = target; // new WeakReference(target);
      this.ChangeKey = changeKey;
      this.IsDataContextRelevant = isDataContextRelevant;
      this.Description = description;
    }

    /// <summary>
    /// Holt a reference to the object that this change is for.
    /// </summary>
    public object Target { get; private set; }

    /// <summary>
    /// Holt the change "key" that uniquely identifies this instance. (see commends on the constructor.)
    /// </summary>
    public object ChangeKey { get; private set; }

    /// <summary>
    /// Holt einen Wert, der angibt, ob diese Änderung rückgängig gemacht wurde.
    /// </summary>
    public bool Undone { get; private set; }

    /// <summary>
    /// Holt einen Wert, der angibt, ob die Änderung in den Datenbankkontext übernommen werden soll.
    /// </summary>
    public bool IsDataContextRelevant { get; private set; }

    /// <summary>
    /// Holt die Beschreibung der Änderung.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// When consolidating events, we want to keep the original (first) "Undo"
    /// but use the most recent Redo. This will pull the Redo from the 
    /// specified Change and apply it to this instance.
    /// </summary>
    /// <param name="latestChange">Die letzte Änderung.
    /// </param>
    public abstract void MergeWith(Change latestChange);

    /// <summary>
    /// Apply the undo logic from this instance, and raise the ISupportsUndoNotification.UndoHappened event.
    /// </summary>
    internal void Undo()
    {
      this.PerformUndo();

      this.Undone = true;

      var notify = this.Target as ISupportUndoNotification;
      if (null != notify)
        notify.UndoHappened(this);
    }

    /// <summary>
    /// Apply the redo logic from this instance, and raise the ISupportsUndoNotification.RedoHappened event.
    /// </summary>
    internal void Redo()
    {
      this.PerformRedo();

      this.Undone = false;

      var notify = this.Target as ISupportUndoNotification;
      if (null != notify)
        notify.RedoHappened(this);
    }

    /// <summary>
    /// Overridden in derived classes to contain the actual Undo logic.
    /// </summary>
    protected abstract void PerformUndo();

    /// <summary>
    /// Overridden in derived classes to contain the actual Redo logic.
    /// </summary>
    protected abstract void PerformRedo();
  }
}
