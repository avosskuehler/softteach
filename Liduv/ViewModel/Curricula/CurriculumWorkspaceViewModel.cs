namespace Liduv.ViewModel.Curricula
{
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using System.Windows.Input;

  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Curricula;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Curriculum
  /// </summary>
  public class CurriculumWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Curriculum currently selected
    /// </summary>
    private CurriculumViewModel currentCurriculum;

    /// <summary>
    /// Das Fach, dessen Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Die Jahrgangsstufe, deren Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private JahrtypViewModel jahrtypFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="CurriculumWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public CurriculumWorkspaceViewModel()
    {
      this.AddCurriculumCommand = new DelegateCommand(this.AddCurriculum);
      this.DeleteCurriculumCommand = new DelegateCommand(this.DeleteCurrentCurriculum, () => this.CurrentCurriculum != null);
      this.ResetJahrtypFilterCommand = new DelegateCommand(() => this.JahrtypFilter = null, () => this.JahrtypFilter != null);
      this.ResetFachFilterCommand = new DelegateCommand(() => this.FachFilter = null, () => this.FachFilter != null);

      this.CurrentCurriculum = App.MainViewModel.Curricula.Count > 0 ? App.MainViewModel.Curricula[0] : null;
      this.CurriculaView = CollectionViewSource.GetDefaultView(App.MainViewModel.Curricula);
      this.CurriculaView.Filter = this.CustomFilter;
      this.CurriculaView.SortDescriptions.Add(new SortDescription("CurriculumFach", ListSortDirection.Ascending));
      this.CurriculaView.SortDescriptions.Add(new SortDescription("CurriculumJahrtyp", ListSortDirection.Ascending));
      this.CurriculaView.SortDescriptions.Add(new SortDescription("CurriculumKlassenstufe", ListSortDirection.Ascending));
      this.CurriculaView.Refresh();

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Curricula.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentCurriculum))
        {
          this.CurrentCurriculum = null;
        }
      };

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Curriculum
    /// </summary>
    public DelegateCommand AddCurriculumCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Curriculum
    /// </summary>
    public DelegateCommand DeleteCurriculumCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetJahrtypFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetFachFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die gefilterten Curricula
    /// </summary>
    public ICollectionView CurriculaView { get; set; }

    /// <summary>
    /// Holt oder setzt die curriculum currently selected in this workspace
    /// </summary>
    public CurriculumViewModel CurrentCurriculum
    {
      get
      {
        return this.currentCurriculum;
      }

      set
      {
        this.currentCurriculum = value;
        if (this.currentCurriculum != null)
        {
          Selection.Instance.Fach = this.currentCurriculum.CurriculumFach;
          Selection.Instance.Klasse = App.MainViewModel.Klassen.First(o => o.Model.Klassenstufe == this.currentCurriculum.CurriculumKlassenstufe.Model);
        }

        this.RaisePropertyChanged("CurrentCurriculum");
        this.DeleteCurriculumCommand.RaiseCanExecuteChanged();
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
        this.CurriculaView.Refresh();
        this.ResetFachFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die jahrgangsstufe filter for the stundenentwurf list.
    /// </summary>
    public JahrtypViewModel JahrtypFilter
    {
      get
      {
        return this.jahrtypFilter;
      }

      set
      {
        this.jahrtypFilter = value;
        this.RaisePropertyChanged("JahrtypFilter");
        this.CurriculaView.Refresh();
        this.ResetJahrtypFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Filtert die Terminliste nach Jahrtyp und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool CustomFilter(object item)
    {
      var curriculumViewModel = item as CurriculumViewModel;
      if (curriculumViewModel == null)
      {
        return false;
      }

      if (this.jahrtypFilter != null && this.fachFilter != null)
      {
        return curriculumViewModel.CurriculumJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung
          && curriculumViewModel.CurriculumFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      if (this.jahrtypFilter != null)
      {
        return curriculumViewModel.CurriculumJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung;
      }

      if (this.fachFilter != null)
      {
        return curriculumViewModel.CurriculumFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      return true;
    }

    /// <summary>
    /// Updates the Jahrtypfilter, if a schuljahr is set.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">A <see cref="PropertyChangedEventArgs"/> with the event data.</param>
    private void SelectionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Schuljahr")
      {
        this.JahrtypFilter = Selection.Instance.Schuljahr;
        this.CurriculaView.Refresh();
      }
    }

    /// <summary>
    /// Handles addition a new Curriculum to the workspace and model
    /// </summary>
    private void AddCurriculum()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Neues Curriculum erstellt"), false))
      {
        var dlg = new AskForJahrFachStufeDialog();
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          var curriculum = new Curriculum();
          curriculum.Bezeichnung = dlg.Bezeichnung;
          curriculum.Fach = dlg.Fach.Model;
          curriculum.Klassenstufe = dlg.Klassenstufe.Model;
          curriculum.Jahrtyp = dlg.Jahrtyp.Model;
          curriculum.Halbjahrtyp = dlg.Halbjahrtyp.Model;
          var vm = new CurriculumViewModel(curriculum);
          App.MainViewModel.Curricula.Add(vm);
          this.CurrentCurriculum = vm;
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current Curriculum
    /// </summary>
    private void DeleteCurrentCurriculum()
    {
      App.MainViewModel.Curricula.RemoveTest(this.CurrentCurriculum);
      this.CurrentCurriculum = null;
    }
  }
}
