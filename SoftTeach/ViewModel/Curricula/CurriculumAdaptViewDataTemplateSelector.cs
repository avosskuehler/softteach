// -----------------------------------------------------------------------
// <copyright file="CurriculumAdaptViewDataTemplateSelector.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SoftTeach.ViewModel.Curricula
{
  using System.Windows;
  using System.Windows.Controls;

  using SoftTeach.ViewModel.Jahrespläne;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class CurriculumAdaptViewDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var element = container as FrameworkElement;

      if (element != null && item != null)
      {
        if (item is TagesplanViewModel)
        {
          return element.FindResource("TagespläneView") as DataTemplate;
        }
        
        if (item is SequenzViewModel)
        {
          return element.FindResource("SequenzenTagesplanView") as DataTemplate;
        }
      }

      return null;
    }
  }
}
