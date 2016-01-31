
namespace SoftTeach.View.Stundenpläne
{
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for StundenplanDetailView.xaml
  /// </summary>
  public partial class StundenplanDetailView : UserControl
  {
    public StundenplanDetailView()
    {
      this.InitializeComponent();
      StundenplanSelection.Instance.Plangrid = this.PlanGrid;
    }
  }
}
