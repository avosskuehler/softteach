namespace Liduv.View.Stundenentwürfe
{
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;

  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.ViewModel.Stundenentwürfe;

  /// <summary>
  /// Interaction logic for SearchStundenentwurfDialog.xaml
  /// </summary>
  public partial class SearchStundenentwurfDialog
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SearchStundenentwurfDialog"/> Klasse.
    /// </summary>
    /// <param name="stundenentwurfWorkspaceViewModel">
    /// The stundenentwurf workspace view model.
    /// </param>
    public SearchStundenentwurfDialog(StundenentwurfWorkspaceViewModel stundenentwurfWorkspaceViewModel)
    {
      this.StundenentwurfWorkspaceViewModel = stundenentwurfWorkspaceViewModel;
      if (stundenentwurfWorkspaceViewModel.CurrentStundenentwurf != null)
      {
        this.StundenentwurfWorkspaceViewModel.FachFilter =
          stundenentwurfWorkspaceViewModel.CurrentStundenentwurf.StundenentwurfFach;
        this.StundenentwurfWorkspaceViewModel.ModulFilter =
          stundenentwurfWorkspaceViewModel.CurrentStundenentwurf.StundenentwurfModul;
        this.StundenentwurfWorkspaceViewModel.JahrgangsstufeFilter =
          stundenentwurfWorkspaceViewModel.CurrentStundenentwurf.StundenentwurfJahrgangsstufe;
      }
      else
      {
        this.StundenentwurfWorkspaceViewModel.FachFilter = Selection.Instance.Fach;
        this.StundenentwurfWorkspaceViewModel.ModulFilter = Selection.Instance.Modul;
        this.StundenentwurfWorkspaceViewModel.JahrgangsstufeFilter = App.MainViewModel.Jahrgangsstufen.First(o => o.JahrgangsstufeBezeichnung ==
          Selection.Instance.Klasse.Model.Klassenstufe.Jahrgangsstufe.Bezeichnung);
      }

      this.InitializeComponent();
      this.DataContext = stundenentwurfWorkspaceViewModel;
    }

    /// <summary>
    /// Gets StundeViewModel.
    /// </summary>
    public StundenentwurfWorkspaceViewModel StundenentwurfWorkspaceViewModel { get; private set; }

    /// <summary>
    /// Gets StundeViewModel.
    /// </summary>
    public StundenentwurfViewModel SelectedStundenentwurfViewModel
    {
      get
      {
        return this.StundenentwurfWorkspaceViewModel.CurrentStundenentwurf;
      }
    }

    private void KopieClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      var entwurf = this.StundenentwurfWorkspaceViewModel.CurrentStundenentwurf;
      this.StundenentwurfWorkspaceViewModel.CurrentStundenentwurf = (StundenentwurfViewModel)entwurf.Clone();

      this.Close();
    }

    private void LinkClick(object sender, RoutedEventArgs e)
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
