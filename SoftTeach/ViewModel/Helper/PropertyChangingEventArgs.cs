namespace SoftTeach.ViewModel.Helper
{
  using System;

  /// <summary>
  /// Provides data for the INotifyPropertyChangedExtended.PropertyChanging event.
  /// </summary>
  public class PropertyChangingEventArgs : EventArgs
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="PropertyChangingEventArgs"/> Klasse. 
    /// </summary>
    /// <param name="propertyName"> The name of the property that changed. </param>
    /// <param name="propertyOldValue"> The property Old Value. </param>
    /// <param name="propertyNewValue"> The property New Value.  </param>
    public PropertyChangingEventArgs(string propertyName, object propertyOldValue, object propertyNewValue)
    {
      this.PropertyName = propertyName;
      this.PropertyNewValue = propertyNewValue;
      this.PropertyOldValue = propertyOldValue;
    }

    /// <summary>
    /// Holt den name of the property that changed.
    /// </summary>
    public virtual string PropertyName { get; private set; }

    /// <summary>
    /// Holt den old value of the property that changed.
    /// </summary>
    public virtual object PropertyOldValue { get; private set; }

    /// <summary>
    /// Holt den new value of the property that changed.
    /// </summary>
    public object PropertyNewValue { get; private set; }
  }
}
