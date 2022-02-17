
namespace SoftTeach.View.Termine
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for StundeDetailView.xaml
  /// </summary>
  public partial class StundeDetailView : UserControl
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundeDetailView"/> Klasse.
    /// </summary>
    public StundeDetailView()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Handles the OnSelected event of the PhasenGrid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void PhasenGrid_OnCellSelected(object sender, RoutedEventArgs e)
    {
      // Lookup for the source to be DataGridCell
      if (e.OriginalSource.GetType() == typeof(DataGridCell))
      {
        // Starts the Edit on the row;
        var grd = (DataGrid)sender;
        //grd.BeginEdit(e);
      }
    }
  }
}
