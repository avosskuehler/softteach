
namespace SoftTeach.View.Noten
{
  using System.Windows.Input;

  using SoftTeach.ViewModel.Noten;

  /// <summary>
  /// Interaction logic for MetroNoteEintragenDialog.xaml
  /// </summary>
  public partial class MetroNoteEintragenFlyout
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroNoteEintragenFlyout"/> Klasse.
    /// </summary>
    public MetroNoteEintragenFlyout()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Handles the OnMouseLeave event of the MetroNoteEintragenFlyout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void MetroNoteEintragenFlyout_OnMouseLeave(object sender, MouseEventArgs e)
    {
      this.IsOpen = false;
      this.Qualität1Radio.IsChecked = false;
      this.Qualität2Radio.IsChecked = false;
      this.Qualität3Radio.IsChecked = false;
      this.Qualität4Radio.IsChecked = false;
      this.Qualität5Radio.IsChecked = false;
      this.Qualität6Radio.IsChecked = false;
      this.Quantität1Radio.IsChecked = false;
      this.Quantität2Radio.IsChecked = false;
      this.Quantität3Radio.IsChecked = false;
      this.Quantität4Radio.IsChecked = false;
      this.Quantität5Radio.IsChecked = false;
      this.Quantität6Radio.IsChecked = false;

      var s = this.DataContext as SchülereintragViewModel;
      if (s != null)
      {
        s.IstZufälligAusgewählt = false;
      }
    }
  }
}
