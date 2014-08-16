namespace Liduv.View.Noten
{
  using System.Windows;

  using Liduv.Setting;
  using Liduv.View.Main;
  using Liduv.View.Wochenpläne;

  /// <summary>
  /// Interaction logic for MetroNotenLandingPage.xaml
  /// </summary>
  public partial class MetroSelectSchülerlistePage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MetroSelectSchülerlistePage"/> class.
    /// </summary>
    public MetroSelectSchülerlistePage()
    {
      this.InitializeComponent();
      App.MainViewModel.SchülerlisteWorkspace.JahrtypFilter = Selection.Instance.Jahrtyp;
      App.MainViewModel.SchülerlisteWorkspace.HalbjahrtypFilter = Selection.Instance.Halbjahr;
      App.MainViewModel.SchülerlisteWorkspace.FachFilter = Selection.Instance.Fach;
    }
  }
}
