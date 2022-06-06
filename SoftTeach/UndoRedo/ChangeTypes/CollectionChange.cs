namespace SoftTeach.UndoRedo.ChangeTypes
{
  using System.Collections;

  /// <summary>
  /// Abstrakte Basisklasse für alle Änderungen an Collections.
  /// </summary>
  public abstract class CollectionChange : Change
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="CollectionChange"/> Klasse.
    /// </summary>
    /// <param name="target">Das Target der Aktion.</param>
    ///  <param name="propertyName">Der Name der Property der Collection.</param>
    /// <param name="collection">Die Collection, die verändert wurde.</param>
    /// <param name="changeKey">Ein eindeutiger Schlüssel für die Änderung.</param>
    /// <param name="isDataContextRelevant">True, wenn die Änderung in der Datenbank getrackt werden soll.</param>
    /// <param name="description">Eine Beschreibung der Änderungsaktion für die Undoliste.</param>
    protected CollectionChange(object target, string propertyName, IList collection, object changeKey, bool isDataContextRelevant, string description)
      : base(target, changeKey, isDataContextRelevant, description)
    {
      this.PropertyName = propertyName;
      this.Collection = collection;
    }

    /// <summary>
    /// Holt die veränderte Collection
    /// </summary>
    public IList Collection { get; private set; }

    /// <summary>
    /// Holt den Namen der Property der Collection
    /// </summary>
    public string PropertyName { get; private set; }
  }
}