namespace SoftTeach.ViewModel.Curricula
{
  using System.Windows;
  using System.Windows.Controls;

  using SoftTeach.ViewModel.Datenbank;

  /// <summary>
  /// Klasse um im Curriculumsmodul das richtige DataTemplate auszuwählen
  /// </summary>
  public class CurriculumViewDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var element = container as FrameworkElement;

      if (element != null && item != null)
      {
        //if (item is SchulwocheViewModel)
        //{
        //  return element.FindResource("SchulwochenView") as DataTemplate;
        //}
        
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
