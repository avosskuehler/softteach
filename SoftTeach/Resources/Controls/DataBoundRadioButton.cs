// -----------------------------------------------------------------------
// <copyright file="DataBoundRadioButton.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SoftTeach.Resources.Controls
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class DataBoundRadioButton : RadioButton
  {
    protected override void OnChecked(RoutedEventArgs e)
    {
      // Do nothing. This will prevent IsChecked from being manually set and overwriting the binding.
    }

    protected override void OnToggle()
    {
      // Do nothing. This will prevent IsChecked from being manually set and overwriting the binding.
    }
  }
}
