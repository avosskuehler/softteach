namespace SoftTeach.View.Datenbank
{
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Datenbank;
  using System.Collections.Generic;
  using System.Windows;

  /// <summary>
  /// Interaction logic for DateiverweisWorkspaceView.xaml
  /// </summary>
  public partial class DateiverweisWorkspaceView : Window
  {
    public DateiverweisWorkspaceView()
    {
      this.InitializeComponent();
      this.DataContext = new DateiverweisWorkspaceViewModel();
    }


    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
