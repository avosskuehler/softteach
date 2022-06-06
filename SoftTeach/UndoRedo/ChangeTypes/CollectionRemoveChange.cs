namespace SoftTeach.UndoRedo.ChangeTypes
{
  using System.Collections;
  using System.Diagnostics;

  /// <summary>
  /// Diese Changes sind für das Entfernen von Elementen zu Collections.
  /// </summary>
  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class CollectionRemoveChange : CollectionAddRemoveChangeBase
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="CollectionRemoveChange"/> Klasse.
    /// </summary>
    /// <param name="target"> The target. </param>
    /// <param name="propertyName"> The property name. </param>
    /// <param name="collection"> The collection. </param>
    /// <param name="index"> The index. </param>
    /// <param name="element"> The element. </param>
    ///  <param name="isDataContextRelevant"> The is Data Context Relevant. </param>
    /// <param name="description"> The description. </param>
    public CollectionRemoveChange(
      object target,
      string propertyName,
      IList collection,
      int index,
      object element,
      bool isDataContextRelevant,
      string description)
      : base(target, propertyName, collection, index, element, isDataContextRelevant, description)
    {
    }

    /// <summary>
    /// Macht das Entfernen eines Elements zur Collection rückgängig.
    /// </summary>
    protected override void PerformUndo()
    {
      this.Collection.Insert(this.Index, this.Element);
    }

    /// <summary>
    /// Entfernt das Element wieder, nachdem es durch undo hinzugefügt wurde.
    /// </summary>
    protected override void PerformRedo()
    {
      this.Collection.Remove(this.RedoElement);
    }
  }
}