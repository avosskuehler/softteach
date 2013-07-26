// -----------------------------------------------------------------------
// <copyright file="CurriculumAdaptViewDataTemplateSelector.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Liduv.ViewModel.Jahrespläne
{
  using System.Windows;
  using System.Windows.Controls;

  using Liduv.ViewModel.Termine;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class HalbjahresplanAdaptViewDataTemplateSelector : DataTemplateSelector
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
        
        if (item is StundeViewModel)
        {
          return element.FindResource("StundenTagesplanView") as DataTemplate;
        }
      }

      return null;
    }
  }
}
