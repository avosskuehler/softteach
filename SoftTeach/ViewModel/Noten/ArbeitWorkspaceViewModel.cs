namespace SoftTeach.ViewModel.Noten
{
  using System.Linq;

  using Helper;

  using Setting;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
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
      using (new UndoBatch(App.MainViewModel, string.Format("Arbeit angelegt"), false))
      {

        var arbeit = new ArbeitNeu();
        arbeit.Lerngruppe = dlg.Lerngruppe.Model;
        arbeit.Fach = dlg.Fach.Model;
        arbeit.Bepunktungstyp = dlg.Bepunktungstyp;
        arbeit.Bewertungsschema = dlg.Bewertungsschema.Model;
        arbeit.Bezeichnung = dlg.Bezeichnung;
        arbeit.Datum = dlg.Datum;
        arbeit.IstKlausur = dlg.IstKlausur;

        var vorhandeneArbeiten = arbeit.Lerngruppe.Arbeiten.Count();
        arbeit.LfdNr = vorhandeneArbeiten + 1;

        var vm = new ArbeitViewModel(arbeit);
        App.MainViewModel.Arbeiten.Add(vm);
        this.CurrentArbeit = vm;
      }
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
      using (new UndoBatch(App.MainViewModel, string.Format("Arbeit gelöscht"), false))
      {

        //App.UnitOfWork.Context.Arbeiten.Remove(arbeit.Model);
        App.MainViewModel.Arbeiten.RemoveTest(arbeit);
        this.CurrentArbeit = null;
      }
    }
  }
}
