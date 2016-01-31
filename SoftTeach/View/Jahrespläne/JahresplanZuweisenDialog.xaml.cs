namespace SoftTeach.View.Jahrespläne
{
  /// <summary>
  /// Interaction logic for JahresplanZuweisenDialog.xaml
  /// </summary>
  public partial class JahresplanZuweisenDialog
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahresplanZuweisenDialog"/> Klasse.
    /// </summary>
    public JahresplanZuweisenDialog()
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
