namespace SoftTeach.View.Sitzpläne
{
  /// <summary>
  /// Interaction logic for MetroRäumePage.xaml
  /// </summary>
  public partial class MetroRäumePage
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroRäumePage"/> Klasse.
    /// </summary>
    public MetroRäumePage()
    {
      // Lädt die Räume, falls noch nicht geschehen
      var räume = App.MainViewModel.RaumWorkspace;
      this.DataContext = App.MainViewModel.SitzplanWorkspace;
      this.InitializeComponent();
    }
  }
}
