// -----------------------------------------------------------------------
// <copyright file="CurriculumAdaptViewDataTemplateSelector.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SoftTeach.ViewModel.Jahrespläne
{
  using System.Windows;
  using System.Windows.Controls;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class AdaptPlanDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var element = container as FrameworkElement;

      if (element != null && item != null)
      {
        if (item is StundeViewModel)
        {
          return element.FindResource("StundenViewModelTagesplanView") as DataTemplate;
        }

        if (item is LerngruppenterminViewModel)
        {
          return element.FindResource("FerienViewModelTagesplanView") as DataTemplate;
        }

        //if (item is StundeNeu)
        //{
        //  return element.FindResource("StundenTagesplanView") as DataTemplate;
        //}

        //if (item is LerngruppenterminNeu)
        //{
        //  return element.FindResource("FerienTagesplanView") as DataTemplate;
        //}

      }

      return null;
    }
  }
}
