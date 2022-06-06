namespace SoftTeach.View.Stundenentwürfe
{
  using System.Linq;
  using System.Windows;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Stundenentwürfe;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// Interaction logic for SearchStundenentwurfDialog.xaml
  /// </summary>
  public partial class SearchStundeDialog
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="SearchStundeDialog"/> Klasse.
    /// </summary>
    /// <param name="stundenWorkspaceViewModel">
    /// The stundenentwurf workspace view model.
    /// </param>
    public SearchStundeDialog(SucheStundeWorkspaceViewModel stundenWorkspaceViewModel)
    {
      this.StundenWorkspaceViewModel = stundenWorkspaceViewModel;
      this.StundenWorkspaceViewModel.FachFilter = Selection.Instance.Fach;
      this.StundenWorkspaceViewModel.ModulFilter = Selection.Instance.Modul;
      this.StundenWorkspaceViewModel.JahrgangFilter = Selection.Instance.Lerngruppe.LerngruppeJahrgang;

      this.InitializeComponent();
      this.DataContext = stundenWorkspaceViewModel;
    }

    /// <summary>
    /// Gets StundeViewModel.
    /// </summary>
    public SucheStundeWorkspaceViewModel StundenWorkspaceViewModel { get; private set; }

    /// <summary>
    /// Gets StundeViewModel.
    /// </summary>
    public Stunde SelectedStundeViewModel
    {
      get
      {
        return this.StundenWorkspaceViewModel.CurrentStunde;
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
      var entwuerfe = this.StundenWorkspaceViewModel.SelectedStunden;
      if (entwuerfe.Count == 0)
      {
        var dlg = new InformationDialog("Kein Entwurf ausgewählt", "Bitte einen Stundenentwurf durch Anklicken auswählen", false);
        dlg.ShowDialog();
        return;
      }

      //if (entwuerfe.Count == 1)
      //{
      //  var entwurf = entwuerfe[0] as StundeViewModel;
      //  if (entwurf != null)
      //  {
      //    var entwurfsKopie = (StundenentwurfViewModel)entwurf.Stundenentwurf.Clone();
      //    this.StundenWorkspaceViewModel.CurrentStunde.Stundenentwurf = entwurfsKopie;
      //  }
      //}
      //else
      //{
      //  // Mehrere Entwürfe sind ausgewählt, die zu einem zusammengefasst werden sollen.
      //  var entwurf = entwuerfe[0] as StundenentwurfEintrag;
      //  if (entwurf != null)
      //  {
      //    var newEntwurf = (StundenentwurfViewModel)entwurf.Stundenentwurf.Clone();
      //    for (int i = 1; i < entwuerfe.Count; i++)
      //    {
      //      var stundenentwurfEintrag = entwuerfe[i] as StundenentwurfEintrag;
      //      if (stundenentwurfEintrag == null || stundenentwurfEintrag.Stundenentwurf == null)
      //      {
      //        continue;
      //      }

      //      foreach (var phaseViewModel in stundenentwurfEintrag.Stundenentwurf.Phasen)
      //      {
      //        newEntwurf.AddPhase((PhaseViewModel)phaseViewModel.Clone());
      //      }
      //    }

      //    this.StundenWorkspaceViewModel.CurrentStunde.Stundenentwurf = newEntwurf;
      //  }
      //}

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
