namespace Liduv.View.Curricula
{
  /// <summary>
  /// Interaction logic for CurriculumZuweisenDialog.xaml
  /// </summary>
  public partial class CurriculumZuweisenDialog
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="CurriculumZuweisenDialog"/> Klasse.
    /// </summary>
    public CurriculumZuweisenDialog()
    {
      this.InitializeComponent();
    }

    private void OkClick(object sender, System.Windows.RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }
  }
}
