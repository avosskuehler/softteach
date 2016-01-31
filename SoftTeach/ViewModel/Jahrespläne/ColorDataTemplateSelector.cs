// -----------------------------------------------------------------------
// <copyright file="ColorDataTemplateSelector.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SoftTeach.ViewModel.Jahrespläne
{
  using System;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class ColorDataTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// </summary>
    /// <param name="item"> The item. </param>
    /// <param name="container"> The container. </param>
    /// <returns> </returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var element = container as FrameworkElement;

      if (element != null && item != null && item is TagesplanViewModel)
      {
        var tagesplanViewModel = item as TagesplanViewModel;
        if (tagesplanViewModel.TagesplanFerientag)
        {
          return element.FindResource("WeekendTemplate") as DataTemplate;
        }

        switch (tagesplanViewModel.TagesplanDatum.DayOfWeek)
        {
          case DayOfWeek.Saturday:
          case DayOfWeek.Sunday:
            return element.FindResource("WeekendTemplate") as DataTemplate;
          case DayOfWeek.Monday:
          case DayOfWeek.Tuesday:
          case DayOfWeek.Wednesday:
          case DayOfWeek.Thursday:
          case DayOfWeek.Friday:
            return element.FindResource("TagesplanTemplate") as DataTemplate;
        }
      }

      return null;
    }
  }
}
