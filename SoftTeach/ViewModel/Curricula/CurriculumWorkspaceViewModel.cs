namespace SoftTeach.ViewModel.Curricula
{
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using System.Windows.Input;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Curricula;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

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
      this.CopyCurriculumCommand = new DelegateCommand(this.CopyCurrentCurriculum, () => this.CurrentCurriculum != null);
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
    /// Holt den Befehl zur copy the current Curriculum
    /// </summary>
    public DelegateCommand CopyCurriculumCommand { get; private set; }

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
        this.JahrtypFilter = Selection.Instance.Jahrtyp;
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
          //App.UnitOfWork.Context.Curricula.Add(curriculum);
          var vm = new CurriculumViewModel(curriculum);
          App.MainViewModel.Curricula.Add(vm);
          this.CurrentCurriculum = vm;
        }
      }
    }

    /// <summary>
    /// Handles addition of a new Curriculum to the workspace and model
    /// </summary>
    private void CopyCurrentCurriculum()
    {
      if (this.CurrentCurriculum == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Curriculum kopiert"), false))
      {
        var dlg = new AskForJahrFachStufeDialog();
        dlg.Fach = this.CurrentCurriculum.CurriculumFach;
        dlg.Klassenstufe = this.CurrentCurriculum.CurriculumKlassenstufe;
        dlg.Halbjahrtyp = this.CurrentCurriculum.CurriculumHalbjahrtyp;
        dlg.Jahrtyp = Selection.Instance.Jahrtyp;
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          // Create a clone of this curriculum for the adaption dialog
          var curriculumClone = new Curriculum();
          curriculumClone.Bezeichnung = dlg.Bezeichnung;
          curriculumClone.Fach = dlg.Fach.Model;
          curriculumClone.Klassenstufe = dlg.Klassenstufe.Model;
          curriculumClone.Jahrtyp = dlg.Jahrtyp.Model;
          curriculumClone.Halbjahrtyp = dlg.Halbjahrtyp.Model;

          foreach (var reihe in this.CurrentCurriculum.Model.Reihen)
          {
            var reiheClone = new Reihe();
            reiheClone.AbfolgeIndex = reihe.AbfolgeIndex;
            reiheClone.Modul = reihe.Modul;
            reiheClone.Stundenbedarf = reihe.Stundenbedarf;
            reiheClone.Thema = reihe.Thema;
            reiheClone.Curriculum = curriculumClone;
            //App.UnitOfWork.Context.Reihen.Add(reiheClone);

            foreach (var sequenz in reihe.Sequenzen)
            {
              var sequenzClone = new Sequenz();
              sequenzClone.AbfolgeIndex = sequenz.AbfolgeIndex;
              sequenzClone.Stundenbedarf = sequenz.Stundenbedarf;
              sequenzClone.Thema = sequenz.Thema;
              sequenzClone.Reihe = reiheClone;
              //App.UnitOfWork.Context.Sequenzen.Add(sequenzClone);
              reiheClone.Sequenzen.Add(sequenzClone);
            }

            curriculumClone.Reihen.Add(reiheClone);
          }

          //App.UnitOfWork.Context.Curricula.Add(curriculumClone);

          var curriculumCloneViewModel = new CurriculumViewModel(curriculumClone, false);
          App.MainViewModel.Curricula.Add(curriculumCloneViewModel);
          this.CurrentCurriculum = curriculumCloneViewModel;
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current Curriculum
    /// </summary>
    private void DeleteCurrentCurriculum()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Curriculum gelöscht"), false))
      {
        //App.UnitOfWork.Context.Curricula.Remove(CurrentCurriculum.Model);
        App.MainViewModel.Curricula.RemoveTest(this.CurrentCurriculum);
        this.CurrentCurriculum = null;
      }
    }
  }
}
