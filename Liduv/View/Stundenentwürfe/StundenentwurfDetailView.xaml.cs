namespace Liduv.View.Stundenentwürfe
{
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for StundenentwurfDetailView.xaml
  /// </summary>
  public partial class StundenentwurfDetailView : UserControl
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenentwurfDetailView"/> Klasse.
    /// </summary>
    public StundenentwurfDetailView()
    {
      this.InitializeComponent();
    }

    private void PhasenGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var viewModel = this.DataContext;

    }
  }
}
