
using System.Collections;

namespace SoftTeach.View.Personen
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for SelectSchülerView.xaml
  /// </summary>
  public partial class SelectSchülerDialog
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SelectSchülerDialog"/> Klasse.
    /// </summary>
    public SelectSchülerDialog()
    {
      this.InitializeComponent();
      this.DataContext = App.MainViewModel.PersonenWorkspace;
    }

    /// <summary>
    /// Holt alle ausgewählten Schüler
    /// </summary>
    public IList SelectedSchüler
    {
      get { return this.SchülerGrid.SelectedItems; }
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}
