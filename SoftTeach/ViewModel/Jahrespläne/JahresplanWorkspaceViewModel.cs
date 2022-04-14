namespace SoftTeach.ViewModel.Jahrespläne
{
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Jahrespläne;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Jahresplan
  /// </summary>
  public class JahresplanWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Jahresplan currently selected
    /// </summary>
    private JahresplanViewModel currentJahresplan;

    /// <summary>
    /// Das Fach, dessen Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Die Jahrgangsstufe, deren Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private SchuljahrViewModel schuljahrFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahresplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public JahresplanWorkspaceViewModel()
    {
      this.AddJahresplanCommand = new DelegateCommand(this.AddJahresplan);
      //this.CopyJahresplanCommand = new DelegateCommand(this.CopyCurrentJahresplan, () => this.CurrentJahresplan != null);
      this.DeleteJahresplanCommand = new DelegateCommand(this.DeleteCurrentJahresplan, () => this.CurrentJahresplan != null);
      this.ResetSchuljahrFilterCommand = new DelegateCommand(() => this.SchuljahrFilter = null, () => this.SchuljahrFilter != null);
      this.ResetFachFilterCommand = new DelegateCommand(() => this.FachFilter = null, () => this.FachFilter != null);

      this.CurrentJahresplan = App.MainViewModel.Jahrespläne.Count > 0 ? App.MainViewModel.Jahrespläne[0] : null;
      this.JahrespläneViewSource = new CollectionViewSource() { Source = App.MainViewModel.Jahrespläne };
      using (this.JahrespläneViewSource.DeferRefresh())
      {
        this.JahrespläneViewSource.Filter += this.JahrespläneViewSource_Filter;
        this.JahrespläneViewSource.SortDescriptions.Add(new SortDescription("Fach", ListSortDirection.Ascending));
        this.JahrespläneViewSource.SortDescriptions.Add(new SortDescription("Schuljahr", ListSortDirection.Ascending));
        this.JahrespläneViewSource.SortDescriptions.Add(new SortDescription("Jahrgang", ListSortDirection.Ascending));
      }

      this.SelectedJahrespläne = new List<JahresplanViewModel>();

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    }


    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand AddJahresplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Jahresplan
    /// </summary>
    public DelegateCommand DeleteJahresplanCommand { get; private set; }

    ///// <summary>
    ///// Holt den Befehl zur copy the current Jahresplan
    ///// </summary>
    //public DelegateCommand CopyJahresplanCommand { get; private set; }

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
    public CollectionViewSource JahrespläneViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Jahrespläne
    /// </summary>
    public ICollectionView JahrespläneView => this.JahrespläneViewSource.View;

    /// <summary>
    /// Holt die markierten Jahrespläne im Workspace
    /// </summary>
    public IList SelectedJahrespläne { get; set; }

    /// <summary>
    /// Holt oder setzt die jahresplan currently selected in this workspace
    /// </summary>
    public JahresplanViewModel CurrentJahresplan
    {
      get
      {
        return this.currentJahresplan;
      }

      set
      {
        this.currentJahresplan = value;
        if (this.currentJahresplan != null)
        {
          // FIND
          Selection.Instance.Fach = this.currentJahresplan.Fach;
          var lerngruppe = App.UnitOfWork.Context.Lerngruppen.FirstOrDefault(o => o.SchuljahrId == this.currentJahresplan.Schuljahr.Model.Id && o.FachId == this.currentJahresplan.Fach.Model.Id && o.Jahrgang == this.currentJahresplan.Jahrgang);
          if (lerngruppe == null)
          {
            InformationDialog.Show("Fehler", "Lerngruppe nicht gefunden", false);
            return;
          }

          if (!App.MainViewModel.Lerngruppen.Any(o => o.Model.Id == lerngruppe.Id))
          {
            App.MainViewModel.Lerngruppen.Add(new Personen.LerngruppeViewModel(lerngruppe));
          }

          Selection.Instance.Lerngruppe = App.MainViewModel.Lerngruppen.First(o => o.Model.Id == lerngruppe.Id);
          this.currentJahresplan.KalenderNeuLaden();
        }

        this.RaisePropertyChanged("CurrentJahresplan");
        this.DeleteJahresplanCommand.RaiseCanExecuteChanged();
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
        this.JahrespläneView.Refresh();
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
        this.RaisePropertyChanged("SchuljahrFilter");
        this.JahrespläneView.Refresh();
        this.ResetSchuljahrFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Filtert die Terminliste nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private void JahrespläneViewSource_Filter(object sender, FilterEventArgs e)
    {
      var jahresplanViewModel = e.Item as JahresplanViewModel;
      if (jahresplanViewModel == null)
      {
        e.Accepted = false;
        return;
      }

      if (this.schuljahrFilter != null && this.fachFilter != null)
      {
        e.Accepted = jahresplanViewModel.Schuljahr.SchuljahrBezeichnung == this.schuljahrFilter.SchuljahrBezeichnung
          && jahresplanViewModel.Fach.FachBezeichnung == this.fachFilter.FachBezeichnung;
        return;
      }

      if (this.schuljahrFilter != null)
      {
        e.Accepted = jahresplanViewModel.Schuljahr.SchuljahrBezeichnung == this.schuljahrFilter.SchuljahrBezeichnung;
        return;
      }

      if (this.fachFilter != null)
      {
        e.Accepted = jahresplanViewModel.Fach.FachBezeichnung == this.fachFilter.FachBezeichnung;
        return;
      }
    }

    /// <summary>
    /// Updates the Schuljahrfilter, if a schuljahr is set.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">A <see cref="PropertyChangedEventArgs"/> with the event data.</param>
    private void SelectionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Schuljahr")
      {
        this.SchuljahrFilter = Selection.Instance.Schuljahr;
        this.JahrespläneView.Refresh();
      }
    }

    /// <summary>
    /// Handles addition a new Jahresplan to the workspace and model
    /// </summary>
    private void AddJahresplan()
    {
      //using (new UndoBatch(App.MainViewModel, string.Format("Neues Jahresplan erstellt"), false))
      //{
      //  var dlg = new AskForJahrFachStufeDialog();
      //  if (dlg.ShowDialog().GetValueOrDefault(false))
      //  {
      //    var jahresplan = new JahresplanNeu();
      //    jahresplan.Bezeichnung = dlg.Bezeichnung;
      //    jahresplan.Fach = dlg.Fach.Model;
      //    jahresplan.Jahrgang = dlg.Jahrgang;
      //    jahresplan.Schuljahr = dlg.Schuljahr.Model;
      //    jahresplan.Halbjahr = dlg.Halbjahr;
      //    //App.UnitOfWork.Context.Jahrespläne.Add(jahresplan);
      //    var vm = new JahresplanViewModel(jahresplan);
      //    App.MainViewModel.Jahrespläne.Add(vm);
      //    this.CurrentJahresplan = vm;
      //  }
      //}
    }

    /// <summary>
    /// Handles deletion of the current Jahresplan
    /// </summary>
    private void DeleteCurrentJahresplan()
    {
      //using (new UndoBatch(App.MainViewModel, string.Format("Jahresplan gelöscht"), false))
      //{
      //  var jahrespläneToDelete = new List<JahresplanViewModel>();
      //  foreach (var jahresplan in this.SelectedJahrespläne)
      //  {
      //    jahrespläneToDelete.Add(jahresplan as JahresplanViewModel);
      //  }

      //  foreach (var jahresplan in jahrespläneToDelete)
      //  {
      //    //App.UnitOfWork.Context.Jahrespläne.Remove(CurrentJahresplan.Model);
      //    App.MainViewModel.Jahrespläne.RemoveTest(jahresplan);
      //  }

      //  this.CurrentJahresplan = null;
      //  this.JahrespläneView.Refresh();
      //}
    }
  }
}
