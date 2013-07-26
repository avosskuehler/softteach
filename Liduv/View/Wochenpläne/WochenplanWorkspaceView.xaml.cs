
namespace Liduv.View.Wochenpläne
{
  /// <summary>
  /// Interaction logic for WochenplanWorkspaceView.xaml
  /// </summary>
  public partial class WochenplanWorkspaceView
  {
    public WochenplanWorkspaceView()
    {
      this.InitializeComponent();
      WochenplanSelection.Instance.Plangrid = this.PlanGrid;
    }
  }
}
