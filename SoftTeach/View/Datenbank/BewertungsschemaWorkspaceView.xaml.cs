namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for BewertungsschemaWorkspaceView.xaml
  /// </summary>
  public partial class BewertungsschemaWorkspaceView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="BewertungsschemaWorkspaceView"/> Klasse.
    /// </summary>
    public BewertungsschemaWorkspaceView()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
