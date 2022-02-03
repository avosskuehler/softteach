namespace SoftTeach.View.Datenbank
{
  using SoftTeach.Model.EntityFramework;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;

  /// <summary>
  /// Interaction logic for JahresplanWorkspaceDBView.xaml
  /// </summary>
  public partial class JahresplanWorkspaceDBView : Window
  {
    public JahresplanWorkspaceDBView()
    {
      this.Jahrespläne = new List<Jahresplan>();
      foreach (var jahresplan in App.UnitOfWork.Context.Jahrespläne.OrderBy(o => o.Jahrtyp.Jahr).ThenBy(o => o.Fach.Bezeichnung).ThenBy(o => o.Klasse.Klassenstufe))
      {
        this.Jahrespläne.Add(jahresplan);
      }
      this.DataContext = this;
      this.InitializeComponent();
    }

    public List<Jahresplan> Jahrespläne { get; private set; }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
