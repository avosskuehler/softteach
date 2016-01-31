namespace SoftTeach.View.Curricula
{
  using SoftTeach.ViewModel.Curricula;

  /// <summary>
  /// Interaction logic for CurriculumDetailView.xaml
  /// </summary>
  public partial class CurriculumDetailView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="CurriculumDetailView"/> Klasse.
    /// </summary>
    public CurriculumDetailView()
    {
      this.InitializeComponent();
    }

    private void UsedItemsListBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
      var curriculumViewModel = this.DataContext as CurriculumViewModel;
      curriculumViewModel.UpdateFocus();
    }
  }
}
