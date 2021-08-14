namespace SoftTeach.UndoRedo.ChangeTypes
{
  using System.Diagnostics;

  /// <summary>
  /// Diese Änderung wird für die Änderung von Properties genutzt.
  /// </summary>
  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class PropertyChange : Change
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="PropertyChange"/> Klasse.
    /// </summary>
    /// <param name="instance"> The instance.  </param>
    /// <param name="propertyName"> The property name.  </param>
    /// <param name="oldValue"> The old value.  </param>
    /// <param name="newValue"> The new value.  </param>
    /// <param name="isDataContextRelevant"> The is Data Context Relevant. </param>
    /// <param name="description"> The description.  </param>
    public PropertyChange(object instance, string propertyName, object oldValue, object newValue, bool isDataContextRelevant, string description)
      : base(instance, new ChangeKey<object, string>(instance, propertyName), isDataContextRelevant, description)
    {
      this.PropertyName = propertyName;
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    /// <summary>
    /// Holt den Namen der geänderten Property.
    /// </summary>
    public string PropertyName { get; private set; }

    /// <summary>
    /// Holt den alten Wert der Property
    /// </summary>
    public object OldValue { get; private set; }

    /// <summary>
    /// Holt den neuen Wert der Property
    /// </summary>
    public object NewValue { get; private set; }

    /// <summary>
    /// Holt den Debugger string
    /// </summary>
    private string DebuggerDisplay
    {
      get
      {
        return string.Format(
          "PropertyChange(Property={0}, Target={{{1}}}, NewValue={{{2}}}, OldValue={{{3}}})",
          this.PropertyName,
          this.Target,
          this.NewValue,
          this.OldValue);
      }
    }

    /// <summary>
    /// When consolidating events, we want to keep the original "Undo"
    /// but use the most recent Redo. This will pull the Redo from the 
    /// specified Change and apply it to this instance.
    /// </summary>
    /// <param name="latestChange"> The latest Change. </param>
    public override void MergeWith(Change latestChange)
    {
      var other = latestChange as PropertyChange;

      if (null != other)
        this.NewValue = other.NewValue;
    }

    /// <summary>
    /// Macht die Propertyänderung rückgängig.
    /// </summary>
    protected override void PerformUndo()
    {
      this.Target.GetType().GetProperty(this.PropertyName).SetValue(this.Target, this.OldValue, null);
    }

    /// <summary>
    /// Stellt die Propertyänderung wieder her.
    /// </summary>
    protected override void PerformRedo()
    {
      this.Target.GetType().GetProperty(this.PropertyName).SetValue(this.Target, this.NewValue, null);
    }
  }
}