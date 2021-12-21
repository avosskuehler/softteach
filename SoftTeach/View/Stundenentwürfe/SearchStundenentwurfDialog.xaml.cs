namespace SoftTeach.View.Stundenentwürfe
{
  using System.Linq;
  using System.Windows;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Stundenentwürfe;

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

    /// <summary>
    /// Es wurde auf Kopie geklickt, so dass nun von den ausgewählten Stundenentwürfen eine Kopie angelegt wird.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void KopieClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      var entwuerfe = this.StundenentwurfWorkspaceViewModel.SelectedStundenentwurfEinträge;
      if (entwuerfe.Count == 0)
      {
        var dlg = new InformationDialog("Kein Entwurf ausgewählt", "Bitte einen Stundenentwurf durch Anklicken auswählen", false);
        dlg.ShowDialog();
        return;
      }

      if (entwuerfe.Count == 1)
      {
        var entwurf = entwuerfe[0] as StundenentwurfEintrag;
        if (entwurf != null)
        {
          var entwurfsKopie = (StundenentwurfViewModel)entwurf.Stundenentwurf.Clone();
          this.StundenentwurfWorkspaceViewModel.CurrentStundenentwurf = entwurfsKopie;
        }
      }
      else
      {
        // Mehrere Entwürfe sind ausgewählt, die zu einem zusammengefasst werden sollen.
        var entwurf = entwuerfe[0] as StundenentwurfEintrag;
        if (entwurf != null)
        {
          var newEntwurf = (StundenentwurfViewModel)entwurf.Stundenentwurf.Clone();
          for (int i = 1; i < entwuerfe.Count; i++)
          {
            var stundenentwurfEintrag = entwuerfe[i] as StundenentwurfEintrag;
            if (stundenentwurfEintrag == null || stundenentwurfEintrag.Stundenentwurf == null)
            {
              continue;
            }

            foreach (var phaseViewModel in stundenentwurfEintrag.Stundenentwurf.Phasen)
            {
              newEntwurf.AddPhase((PhaseViewModel)phaseViewModel.Clone());
            }
          }

          this.StundenentwurfWorkspaceViewModel.CurrentStundenentwurf = newEntwurf;
        }
      }

      this.Close();
    }

    private void LinkClick(object sender, RoutedEventArgs e)
    {
      var entwuerfe = this.StundenentwurfWorkspaceViewModel.SelectedStundenentwurfEinträge;
      if (entwuerfe.Count != 1)
      {
        var dlg = new InformationDialog("Mehrer Entwürfe ausgewählt.", "Bitte für den Verweis nur einen Stundenentwurf durch Anklicken auswählen. Wenn mehrere Entwürfe zusammengefasst werden sollen bitte auf Kopie klicken.", false);
        dlg.ShowDialog();
        return;
      }

      this.DialogResult = true;
      this.Close();
    }

    /// <summary>
    /// Benutzer hat abbruch geklickt.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}
