namespace SoftTeach.ViewModel.Jahrespläne
{
  using System.Windows;
  using System.Windows.Controls;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// DataTemplateSelector für Tagesplan
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
      }

      return null;
    }
  }
}
