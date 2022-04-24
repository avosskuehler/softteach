using SoftTeach.ViewModel.Termine;
using System.Windows;

namespace SoftTeach.View.Termine
{
  /// <summary>
  /// Interaction logic for SchulterminDialog.xaml
  /// </summary>
  public partial class SchulterminDialog
  {
    public SchulterminDialog()
    {
      this.InitializeComponent();
      this.DataContext = App.MainViewModel.SchulterminWorkspace;
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void UpdateClick(object sender, RoutedEventArgs e)
    {
      SchulterminWorkspaceViewModel.UpdateJahrespläne();
    }

    private void CancelClick(object sender, System.Windows.RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}
