namespace SoftTeach.ViewModel.Personen
{
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Personen;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.View.Sitzpläne;

  /// <summary>
  /// ViewModel for managing Lerngruppe
  /// </summary>
  public class LerngruppeWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Lerngruppe currently selected
    /// </summary>
    private LerngruppeViewModel currentLerngruppe;

    /// <summary>
    /// Das Fach, dessen Lerngruppen nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Die Jahrgangsstufe, deren Lerngruppen nur dargestellt werden sollen.
    /// </summary>
    private SchuljahrViewModel schuljahrFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="LerngruppeWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public LerngruppeWorkspaceViewModel()
    {
      this.AddLerngruppeCommand = new DelegateCommand(this.AddLerngruppe);
      this.MoveLerngruppeCommand = new DelegateCommand(this.MoveCurrentLerngruppe, () => this.CurrentLerngruppe != null);
      this.DeleteLerngruppeCommand = new DelegateCommand(this.DeleteCurrentLerngruppe, () => this.CurrentLerngruppe != null);
      this.ResetSchuljahrFilterCommand = new DelegateCommand(() => this.SchuljahrFilter = null, () => this.SchuljahrFilter != null);
      this.ResetFachFilterCommand = new DelegateCommand(() => this.FachFilter = null, () => this.FachFilter != null);

      this.currentLerngruppe = App.MainViewModel.Lerngruppen.Count > 0 ? App.MainViewModel.Lerngruppen[0] : null;

      this.LerngruppenView = CollectionViewSource.GetDefaultView(App.MainViewModel.Lerngruppen);
      this.LerngruppenView.Filter = this.CustomFilter;
      this.LerngruppenView.SortDescriptions.Add(new SortDescription("LerngruppeSchuljahr", ListSortDirection.Ascending));
      this.LerngruppenView.SortDescriptions.Add(new SortDescription("LerngruppeFach", ListSortDirection.Ascending));
      this.LerngruppenView.SortDescriptions.Add(new SortDescription("LerngruppeBezeichnung", ListSortDirection.Ascending));
      this.LerngruppenView.Refresh();

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Lerngruppen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentLerngruppe))
        {
          this.CurrentLerngruppe = null;
        }
      };

      this.SchuljahrFilter = Selection.Instance.Schuljahr;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Lerngruppe
    /// </summary>
    public DelegateCommand AddLerngruppeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um die Lerngruppe ins nächste Halbjahr zu übernehmen
    /// </summary>
    public DelegateCommand MoveLerngruppeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Lerngruppe
    /// </summary>
    public DelegateCommand DeleteLerngruppeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetSchuljahrFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetFachFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die gefilterten Lerngruppen
    /// </summary>
    public ICollectionView LerngruppenView { get; set; }

    /// <summary>
    /// Holt oder setzt die stundenentwurf currently selected in this workspace
    /// </summary>
    public LerngruppeViewModel CurrentLerngruppe
    {
      get
      {
        return this.currentLerngruppe;
      }

      set
      {
        this.currentLerngruppe = value;
        Selection.Instance.Lerngruppe = value;
        this.RaisePropertyChanged("CurrentLerngruppe");
        this.DeleteLerngruppeCommand.RaiseCanExecuteChanged();
        this.MoveLerngruppeCommand.RaiseCanExecuteChanged();
        if (this.currentLerngruppe != null && Configuration.Instance.IsMetroMode)
        {
          switch (Configuration.Instance.NavigateTarget)
          {
            case NavigateTarget.Noten:
              Configuration.Instance.NavigationService.Navigate(new MetroLerngruppePage());
              break;
            case NavigateTarget.Gruppen:
              Configuration.Instance.NavigationService.Navigate(new MetroGruppenPage());
              break;
            case NavigateTarget.Sitzpläne:
              Configuration.Instance.NavigationService.Navigate(new MetroRäumePage());
              break;
            default:
              break;
          }
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
        this.LerngruppenView.Refresh();
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
        this.LerngruppenView.Refresh();
        this.ResetSchuljahrFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Filtert die Terminliste nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool CustomFilter(object item)
    {
      var schülerlisteViewModel = item as LerngruppeViewModel;
      if (schülerlisteViewModel == null)
      {
        return false;
      }

      if (this.schuljahrFilter != null && this.fachFilter != null)
      {
        return schülerlisteViewModel.LerngruppeSchuljahr.SchuljahrBezeichnung == this.schuljahrFilter.SchuljahrBezeichnung
          && schülerlisteViewModel.LerngruppeFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      if (this.schuljahrFilter != null)
      {
        return schülerlisteViewModel.LerngruppeSchuljahr.SchuljahrBezeichnung == this.schuljahrFilter.SchuljahrBezeichnung;
      }

      if (this.fachFilter != null)
      {
        return schülerlisteViewModel.LerngruppeFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      return true;
    }

    /// <summary>
    /// Handles addition a new Stundenentwurf to the workspace and model
    /// </summary>
    private void AddLerngruppe()
    {
      // Check for existing schülerliste
      var dlg = new AskForLerngruppeToAddDialog();
      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      if (
        App.MainViewModel.Lerngruppen.Any(
          o =>
          o.LerngruppeSchuljahr.SchuljahrJahr == dlg.Schuljahr.SchuljahrJahr
          && o.LerngruppeBezeichnung == dlg.Bezeichnung
          && o.LerngruppeJahrgang == dlg.Jahrgang
          && o.LerngruppeFach.FachBezeichnung == dlg.Fach.FachBezeichnung))
      {
        Log.ProcessMessage(
          "Lerngruppe bereits vorhanden",
          "Diese Lerngruppe ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      var neueLerngruppe = new LerngruppeNeu();
      neueLerngruppe.Jahrgang = dlg.Jahrgang;
      neueLerngruppe.Bezeichnung = dlg.Bezeichnung;
      neueLerngruppe.Schuljahr = dlg.Schuljahr.Model;
      neueLerngruppe.Fach = dlg.Fach.Model;
      neueLerngruppe.NotenWichtung = dlg.NotenWichtung.Model;
      neueLerngruppe.Bepunktungstyp = dlg.Bepunktungstyp;
      var vm = new LerngruppeViewModel(neueLerngruppe);
      using (new UndoBatch(App.MainViewModel, string.Format("Neue Lerngruppe {0} angelegt.", vm), false))
      {
        App.MainViewModel.Lerngruppen.Add(vm);
        this.CurrentLerngruppe = vm;
      }
    }

    /// <summary>
    /// Kopiert die Lerngruppe ins nächste Jahr
    /// </summary>
    private void MoveCurrentLerngruppe()
    {
      // Check for existing schülerliste
      var newSchuljahrJahr = this.CurrentLerngruppe.LerngruppeSchuljahr.SchuljahrJahr + 1;
      var newSchuljahr = App.MainViewModel.Schuljahre.FirstOrDefault(o => o.SchuljahrJahr == newSchuljahrJahr);

      // Lerngruppe soll ins nächste Jahr übernommen werden
      if (newSchuljahr == null)
      {
        InformationDialog.Show(
          "Schuljahr fehlt",
          "Bitte legen Sie zuerst das neue Schuljahr an, bevor Sie die Lerngruppen erzeugen",
          false);
        return;
      }

      var oldKlasseString = this.CurrentLerngruppe.LerngruppeBezeichnung;
      var oldKlassenstufe = this.CurrentLerngruppe.LerngruppeJahrgang;

      if (oldKlassenstufe > 9)
      {
        InformationDialog.Show(
          "Klasse nicht möglich",
          "Schüler der 10.Klassen oder der Oberstufe können nicht ins nächste Schuljahr übernommen werden",
          false);
        return;
      }

      var newKlassenstufe = oldKlassenstufe + 1;
      var newKlasseBezeichnung = newKlassenstufe + oldKlasseString.StripRight(1);

      if (
        App.MainViewModel.Lerngruppen.Any(
          o =>
          o.LerngruppeSchuljahr.SchuljahrJahr == newSchuljahrJahr
          && o.LerngruppeBezeichnung == newKlasseBezeichnung
          && o.LerngruppeJahrgang == newKlassenstufe
          && o.LerngruppeFach.FachBezeichnung == this.CurrentLerngruppe.LerngruppeFach.FachBezeichnung))
      {
        Log.ProcessMessage(
          "Lerngruppe bereits vorhanden",
          "Diese Lerngruppe ist bereits in der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Lerngruppe kopiert."), false))
      {
        var newLerngruppe = (LerngruppeViewModel)this.CurrentLerngruppe.Clone();
        newLerngruppe.LerngruppeSchuljahr = newSchuljahr;
        this.CurrentLerngruppe = newLerngruppe;
      }
    }

    /// <summary>
    /// Löscht die aktuelle Lerngruppe
    /// </summary>
    private void DeleteCurrentLerngruppe()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Lerngruppe {0} gelöscht.", this.CurrentLerngruppe), false))
      {
        // Dazugehörige Arbeiten löschen
        var arbeiten =
          App.MainViewModel.Arbeiten.Where(
            o =>
            o.ArbeitLerngruppe == this.CurrentLerngruppe
            && o.ArbeitFach == this.CurrentLerngruppe.LerngruppeFach);
        var list = arbeiten.ToList();
        foreach (var arbeitViewModel in list)
        {
          bool success = App.MainViewModel.Arbeiten.RemoveTest(arbeitViewModel);
        }

        // Jetzt die Lerngruppe löschen
        App.MainViewModel.Lerngruppen.RemoveTest(this.CurrentLerngruppe);
        this.CurrentLerngruppe = null;
      }
    }
  }
}
