namespace Liduv.View.Wochenpläne
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;

  /// <summary>
  /// Interaction logic for MetroTagesplanPage.xaml
  /// </summary>
  public partial class MetroTagesplanPage : Page
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MetroTagesplanPage" /> class.
    /// </summary>
    public MetroTagesplanPage()
    {
      this.InitializeComponent();
      WochenplanSelection.Instance.Plangrid = this.PlanGrid;
    }

    protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
    {
      base.OnManipulationDelta(e);
      if (e.CumulativeManipulation.Translation.X > 100)
      {
        App.MainViewModel.TagesplanWorkspace.PreviousDayCommand.Execute(null);
        e.Complete();
      }
      else if (e.CumulativeManipulation.Translation.X < -100)
      {
        App.MainViewModel.TagesplanWorkspace.NextDayCommand.Execute(null);
        e.Complete();
      }
    }
  }
}
