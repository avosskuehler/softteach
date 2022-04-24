namespace SoftTeach.ViewModel.Noten
{
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using Helper;

  using Setting;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Datenbank;

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
    /// Das Fach, dessen Lerngruppen nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Die Jahrgangsstufe, deren Lerngruppen nur dargestellt werden sollen.
    /// </summary>
    private SchuljahrViewModel schuljahrFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ArbeitWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public ArbeitWorkspaceViewModel()
    {
      this.AddArbeitCommand = new DelegateCommand(this.AddArbeit);
      this.DeleteArbeitCommand = new DelegateCommand(this.DeleteArbeit, () => this.CurrentArbeit != null);

      this.ResetSchuljahrFilterCommand = new DelegateCommand(() => this.SchuljahrFilter = null, () => this.SchuljahrFilter != null);
      this.ResetFachFilterCommand = new DelegateCommand(() => this.FachFilter = null, () => this.FachFilter != null);

      this.ArbeitenViewSource = new CollectionViewSource() { Source = App.MainViewModel.Arbeiten };
      using (this.ArbeitenViewSource.DeferRefresh())
      {
        this.ArbeitenViewSource.Filter += this.ArbeitenViewSource_Filter;
        this.ArbeitenViewSource.SortDescriptions.Add(new SortDescription("ArbeitFach.FachBezeichnung", ListSortDirection.Ascending));
        this.ArbeitenViewSource.SortDescriptions.Add(new SortDescription("ArbeitLerngruppe.LerngruppeSchuljahr.SchuljahrJahr", ListSortDirection.Ascending));
        this.ArbeitenViewSource.SortDescriptions.Add(new SortDescription("ArbeitLerngruppe.LerngruppeJahrgang", ListSortDirection.Ascending));
      }

      this.SchuljahrFilter = Selection.Instance.Schuljahr;
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
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetSchuljahrFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetFachFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die JahrespläneViewSource
    /// </summary>
    public CollectionViewSource ArbeitenViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Arbeiten
    /// </summary>
    public ICollectionView ArbeitenView => this.ArbeitenViewSource.View;

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
    /// Holt oder setzt die fach filter for the stundenentwurf list.
    /// </summary>
    public FachViewModel FachFilter
    {
      get
      {
        return this.fachFilter;
      }

      set
      {
        this.fachFilter = value;
        this.RaisePropertyChanged("FachFilter");
        this.ArbeitenView.Refresh();
        this.ResetFachFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die jahrgangsstufe filter for the stundenentwurf list.
    /// </summary>
    public SchuljahrViewModel SchuljahrFilter
    {
      get
      {
        return this.schuljahrFilter;
      }

      set
      {
        this.schuljahrFilter = value;
        Selection.Instance.Schuljahr = value;
        this.RaisePropertyChanged("SchuljahrFilter");
        this.ArbeitenView.Refresh();
        this.ResetSchuljahrFilterCommand.RaiseCanExecuteChanged();
      }
    }
    /// <summary>
    /// Handles addition a new Stundenentwurf to the workspace and model
    /// </summary>
    private void AddArbeit()
    {
      var dlg = new AddArbeitDialog();
      var workspace = new AddArbeitWorkspaceViewModel();
      dlg.DataContext = workspace;
      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }
      using (new UndoBatch(App.MainViewModel, string.Format("Arbeit angelegt"), false))
      {

        var arbeit = new ArbeitNeu();
        arbeit.Lerngruppe = workspace.Lerngruppe.Model;
        arbeit.Fach = workspace.Lerngruppe.LerngruppeFach.Model;
        arbeit.Bepunktungstyp = workspace.Bepunktungstyp;
        arbeit.Bewertungsschema = workspace.Bewertungsschema.Model;
        arbeit.Bezeichnung = workspace.Bezeichnung;
        arbeit.Datum = workspace.Datum;
        arbeit.IstKlausur = workspace.IstKlausur;

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

    /// <summary>
    /// Filtert die Lerngruppen nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Die Lerngruppe, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private void ArbeitenViewSource_Filter(object sender, FilterEventArgs e)
    {
      var arbeitViewModel = e.Item as ArbeitViewModel;
      if (arbeitViewModel == null)
      {
        e.Accepted = false;
        return;
      }

      if (this.fachFilter != null)
      {
        if (arbeitViewModel.ArbeitFach.FachBezeichnung != this.fachFilter.FachBezeichnung) e.Accepted = false;
        return;
      }


      if (this.schuljahrFilter != null)
      {
        if (arbeitViewModel.ArbeitLerngruppe.LerngruppeSchuljahr.SchuljahrJahr != this.schuljahrFilter.SchuljahrJahr) e.Accepted = false;
        return;
      }

      e.Accepted = true;
      return;
    }

  }
}
