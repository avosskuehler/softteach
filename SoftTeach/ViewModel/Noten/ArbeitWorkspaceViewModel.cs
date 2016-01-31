namespace SoftTeach.ViewModel.Noten
{
  using System.Linq;

  using Helper;

  using Setting;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.View.Noten;

  /// <summary>
  /// ViewModel for managing Arbeit
  /// </summary>
  public class ArbeitWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Arbeit currently selected
    /// </summary>
    private ArbeitViewModel currentArbeit;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ArbeitWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public ArbeitWorkspaceViewModel()
    {
      this.AddArbeitCommand = new DelegateCommand(this.AddArbeit);
      this.DeleteArbeitCommand = new DelegateCommand(this.DeleteArbeit, () => this.CurrentArbeit != null);
      this.CurrentArbeit = App.MainViewModel.Arbeiten.Count > 0 ? App.MainViewModel.Arbeiten[0] : null;
    }

    /// <summary>
    /// Holt den Befehl, um eine neue Arbeit anzulegen.
    /// </summary>
    public DelegateCommand AddArbeitCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um eine bestehende Arbeit zu löschen
    /// </summary>
    public DelegateCommand DeleteArbeitCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die Arbeit currently selected in this workspace
    /// </summary>
    public ArbeitViewModel CurrentArbeit
    {
      get
      {
        return this.currentArbeit;
      }

      set
      {
        this.currentArbeit = value;
        Selection.Instance.Arbeit = value;
        this.RaisePropertyChanged("CurrentArbeit");
        this.DeleteArbeitCommand.RaiseCanExecuteChanged();
        if (this.currentArbeit != null)
        {
          this.currentArbeit.BerechneNotenspiegel();
        }
      }
    }

    /// <summary>
    /// Handles addition a new Stundenentwurf to the workspace and model
    /// </summary>
    private void AddArbeit()
    {
      var dlg = new AddArbeitDialog();
      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      var arbeit = new Arbeit();
      arbeit.Klasse = dlg.Klasse.Model;
      arbeit.Jahrtyp = dlg.Jahrtyp.Model;
      arbeit.Halbjahrtyp = dlg.Halbjahrtyp.Model;
      arbeit.Fach = dlg.Fach.Model;
      arbeit.Bepunktungstyp = dlg.Bepunktungstyp.ToString();
      arbeit.Bewertungsschema = dlg.Bewertungsschema.Model;
      arbeit.Bezeichnung = dlg.Bezeichnung;
      arbeit.Datum = dlg.Datum;
      arbeit.IstKlausur = dlg.IstKlausur;

      var vorhandeneArbeiten =
        App.MainViewModel.Arbeiten.Count(
          o =>
          o.ArbeitJahrtyp.JahrtypBezeichnung == Selection.Instance.Jahrtyp.JahrtypBezeichnung
          && o.ArbeitHalbjahrtyp.HalbjahrtypIndex == Selection.Instance.Halbjahr.HalbjahrtypIndex
          && o.ArbeitKlasse.KlasseBezeichnung == Selection.Instance.Klasse.KlasseBezeichnung
          && o.ArbeitFach.FachBezeichnung == Selection.Instance.Fach.FachBezeichnung);
      arbeit.LfdNr = vorhandeneArbeiten + 1;

      // App.UnitOfWork.GetRepository<Arbeit>().Add(arbeit);
      var vm = new ArbeitViewModel(arbeit);
      App.MainViewModel.Arbeiten.Add(vm);
      this.CurrentArbeit = vm;
    }

    /// <summary>
    /// Löscht die aktuelle Arbeit.
    /// </summary>
    private void DeleteArbeit()
    {
      this.DeleteArbeit(this.CurrentArbeit);
    }

    /// <summary>
    /// Löscht die gegebene Arbeit.
    /// </summary>
    /// <param name="arbeit">Das ArbeitViewModel mit der zu löschenden Arbeit.</param>
    private void DeleteArbeit(ArbeitViewModel arbeit)
    {
      // App.UnitOfWork.GetRepository<Arbeit>().RemoveTest(arbeit.Model);
      App.MainViewModel.Arbeiten.RemoveTest(arbeit);
      this.CurrentArbeit = null;
    }
  }
}
