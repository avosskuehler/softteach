namespace SoftTeach.ViewModel.Curricula
{
  using System.Windows;
  using System.Windows.Controls;

  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// Ein <see cref="DataTemplateSelector"/> der im Curriculum Zuweisen Dialog sicherstellt,
  /// dass Stunden und Sequenzen das richtige Template zugewiesen bekommen.
  /// </summary>
  public class CurriculumAdaptViewDataTemplateSelector : DataTemplateSelector
  {
    private DataTemplate stundeTemplate;
    private DataTemplate sequenzTemplate;
    private DataTemplate reiheTemplate;
    private DataTemplate ferienTemplate;

    public CurriculumAdaptViewDataTemplateSelector()
    {
      this.stundeTemplate = App.Current.FindResource("StundenViewModelTagesplanView") as DataTemplate;
      this.sequenzTemplate = App.Current.FindResource("SequenzenCurriculumView") as DataTemplate;
      this.reiheTemplate = App.Current.FindResource("ReihenCurriculumView") as DataTemplate;
      this.ferienTemplate = App.Current.FindResource("FerienViewModelTagesplanView") as DataTemplate;
    }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var element = container as FrameworkElement;

      if (element != null && item != null)
      {
        if (item is StundeViewModel)
        {
          var stunde = item as StundeViewModel;
          if (stunde.TerminTermintyp == Model.TeachyModel.Termintyp.Ferien)
          {
            return this.ferienTemplate;
          }

          return this.stundeTemplate;
        }

        if (item is ReiheViewModel)
        {
          return this.reiheTemplate;
        }

        if (item is SequenzViewModel)
        {
          return this.sequenzTemplate;
        }
      }

      return null;
    }
  }
}
