namespace SoftTeach.UndoRedo.ChangeTypes
{
  using System.Collections;
  using System.Diagnostics;

  /// <summary>
  /// Diese Changes sind für das Hinzufügen von Elementen zu Collections.
  /// </summary>
  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class CollectionAddChange : CollectionAddRemoveChangeBase
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="CollectionAddChange"/> Klasse.
    /// </summary>
    /// <param name="target"> The target. </param>
    /// <param name="propertyName"> The property name. </param>
    /// <param name="collection"> The collection. </param>
    /// <param name="index"> The index. </param>
    /// <param name="element"> The element. </param>
    /// <param name="isDataContextRelevant"> The is data context relevant. </param>
    /// <param name="description"> The description. </param>
    public CollectionAddChange(
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
    /// Macht das Hinzufügen eines Elements zur Collection rückgängig.
    /// </summary>
    protected override void PerformUndo()
    {
      this.Collection.Remove(this.Element);
    }

    /// <summary>
    /// Stellt das Element wieder her, nachdem es durch undo entfernt wurde.
    /// </summary>
    protected override void PerformRedo()
    {
      this.Collection.Insert(this.RedoIndex, this.RedoElement);
    }
  }
}