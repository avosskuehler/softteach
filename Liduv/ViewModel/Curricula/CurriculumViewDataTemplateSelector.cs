// -----------------------------------------------------------------------
// <copyright file="CurriculumViewDataTemplateSelector.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Liduv.ViewModel.Curricula
{
  using System.Windows;
  using System.Windows.Controls;

  using Liduv.ViewModel.Datenbank;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class CurriculumViewDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var element = container as FrameworkElement;

      if (element != null && item != null)
      {
        if (item is SchulwocheViewModel)
        {
          return element.FindResource("SchulwochenView") as DataTemplate;
        }
        
        if (item is ReiheViewModel)
        {
          return element.FindResource("ReihenNarrowView") as DataTemplate;
        }

        if (item is SequenzViewModel)
        {
          return element.FindResource("SequenzenNarrowView") as DataTemplate;
        }
      }

      return null;
    }
  }
}
