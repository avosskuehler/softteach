namespace SoftTeach.View.Datenbank
{
  using SoftTeach.Model.TeachyModel;
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
      this.DataContext = this;
      this.Dateiverweise = new List<DateiverweisNeu>();
      foreach (var dateiverweis in App.UnitOfWork.Context.Dateiverweise)
      {
        this.Dateiverweise.Add(dateiverweis);
      }
    }

    public List<DateiverweisNeu> Dateiverweise { get; private set; }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
