namespace Liduv.UndoRedo.ChangeTypes
{
  using System.Collections;

  /// <summary>
  /// Abstrakte Basisklasse für alle Add/Remove Änderungen an Collections
  /// </summary>
  public abstract class CollectionAddRemoveChangeBase : CollectionChange
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="CollectionAddRemoveChangeBase"/> Klasse.
    /// </summary>
    /// <param name="target"> The target. </param>
    /// <param name="propertyName"> The property name. </param>
    /// <param name="collection"> The collection. </param>
    /// <param name="index"> The index. </param>
    /// <param name="element"> The element. </param>
    /// <param name="isDataContextRelevant"> The is Data Context Relevant. </param>
    /// <param name="description"> The description. </param>
    protected CollectionAddRemoveChangeBase(
      object target, string propertyName, IList collection, int index, object element, bool isDataContextRelevant, string description)
      : base(
        target,
        propertyName,
        collection,
        new ChangeKey<object, string, object>(target, propertyName, element),
        isDataContextRelevant,
        description)
    {
      this.Element = element;
      this.Index = index;
      this.RedoElement = element;
      this.RedoIndex = index;
    }

    /// <summary>
    /// Holt das Element, dass hinzugefügt oder entfernt wurde.
    /// </summary>
    public object Element { get; private set; }

    /// <summary>
    /// Holt den Index, an dem das Element hinzugefügt oder entfernt wurde.
    /// </summary> 
    public int Index { get; private set; }

    /// <summary>
    /// Holt das Element, dass wiederhergestellt werden soll
    /// </summary>
    protected object RedoElement { get; private set; }

    /// <summary>
    /// Holt den Index, an dem das Element wiederhergestellt werden soll.
    /// </summary>
    protected int RedoIndex { get; private set; }

    /// <summary>
    /// Holt einen Text, der diese Änderung im Debuggerfenster zusammenfasst.
    /// </summary>
    protected string DebuggerDisplay
    {
      get
      {
        return string.Format(
          "{4}(Property={0}, Target={{{1}}}, Index={2}, Element={{{3}}})",
          this.PropertyName,
          this.Target,
          this.Index,
          this.Element,
          this.GetType().Name);
      }
    }

    /// <summary>
    /// Does this method is correct merge?
    /// </summary>
    /// <param name="latestChange"> The latest Change. </param>
    public override void MergeWith(Change latestChange)
    {
      var other = latestChange as CollectionAddRemoveChangeBase;

      if (null != other)
      {
        this.RedoElement = other.RedoElement;
        this.RedoIndex = other.RedoIndex;
      }
    }
  }
}