namespace SoftTeach.Resources.Controls
{
  using System.Windows;
  using System.Windows.Controls;

  public class SDataGrid : DataGrid
  {
    static SDataGrid()
    {
      ItemsControl.ItemsSourceProperty.OverrideMetadata(typeof(SDataGrid), new FrameworkPropertyMetadata((PropertyChangedCallback)null, (CoerceValueCallback)null));
    }
  }
}
