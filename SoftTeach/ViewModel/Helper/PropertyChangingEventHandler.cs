namespace SoftTeach.ViewModel.Helper
{
  /// <summary>
  /// Represents the method that will handle the INotifyPropertyChanged.PropertyChanging
  /// event raised when a property is changed on a component.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">A <see cref="PropertyChangingEventArgs"/> that contains the event
  /// data.</param>
  public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
}
