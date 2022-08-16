namespace SoftTeach.View.Wochenpläne
{
  /// <summary>
  /// Interaction logic for WochenplanPage.xaml
  /// </summary>
  public partial class MetroWochenplanPage
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroWochenplanPage"/> Klasse.
    /// </summary>
    public MetroWochenplanPage()
    {
      this.InitializeComponent();
      this.DataContext = App.MainViewModel.WochenplanWorkspace;
      WochenplanSelection.Instance.Plangrid = this.PlanGrid;
    }
  }
}
