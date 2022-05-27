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
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// ViewModel for managing Jahresplan
  /// </summary>
  public class JahresplanWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// Die ausgewählte Lerngruppe für den Jahresplan
    /// </summary>
    private LerngruppeNeu currentLerngruppe;

    /// <summary>
    /// Der Jahresplan der ausgewählten Lerngruppe
    /// </summary>
    private JahresplanViewModel currentJahresplan;

    /// <summary>
    /// Das Fach, für das die Jahrespläne nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Das Schuljahr, dessen Jahrespläne nur dargestellt werden sollen.
    /// </summary>
    private SchuljahrViewModel schuljahrFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahresplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public JahresplanWorkspaceViewModel()
    {
      this.AddJahresplanCommand = new DelegateCommand(this.AddJahresplan);
      //this.CopyJahresplanCommand = new DelegateCommand(this.CopyCurrentJahresplan, () => this.CurrentJahresplan != null);
      this.DeleteJahresplanCommand = new DelegateCommand(this.DeleteCurrentJahresplan, () => this.CurrentLerngruppe != null);
      this.ResetSchuljahrFilterCommand = new DelegateCommand(() => this.SchuljahrFilter = null, () => this.SchuljahrFilter != null);
      this.ResetFachFilterCommand = new DelegateCommand(() => this.FachFilter = null, () => this.FachFilter != null);

      this.CurrentLerngruppe = App.UnitOfWork.Context.Lerngruppen.Count() > 0 ? App.UnitOfWork.Context.Lerngruppen.First() : null;
      var lerngruppenImContext = new List<LerngruppeNeu>();
      foreach (var lerngruppe in App.UnitOfWork.Context.Lerngruppen)
      {
        lerngruppenImContext.Add(lerngruppe);
      }

      this.LerngruppenViewSource = new CollectionViewSource() { Source = lerngruppenImContext };
      using (this.LerngruppenViewSource.DeferRefresh())
      {
        this.LerngruppenViewSource.Filter += this.LerngruppenViewSource_Filter;
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("Fach.Bezeichnung", ListSortDirection.Ascending));
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("Schuljahr.Jahr", ListSortDirection.Ascending));
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("Jahrgang", ListSortDirection.Ascending));
      }

      this.SelectedJahrespläne = new List<JahresplanViewModel>();

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
      this.SchuljahrFilter = Selection.Instance.Schuljahr;
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
    public CollectionViewSource LerngruppenViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Lerngruppen
    /// </summary>
    public ICollectionView LerngruppenView => this.LerngruppenViewSource.View;

    /// <summary>
    /// Holt die markierten Jahrespläne im Workspace
    /// </summary>
    public IList SelectedJahrespläne { get; set; }

    /// <summary>
    /// Holt oder setzt die jahresplan currently selected in this workspace
    /// </summary>
    public LerngruppeNeu CurrentLerngruppe
    {
      get
      {
        return this.currentLerngruppe;
      }

      set
      {
        this.currentLerngruppe = value;
        if (this.currentLerngruppe != null)
        {
          // FIND
          var lerngruppe = App.UnitOfWork.Context.Lerngruppen.FirstOrDefault(o => o.SchuljahrId == this.currentLerngruppe.SchuljahrId && o.FachId == this.currentLerngruppe.FachId && o.Jahrgang == this.currentLerngruppe.Jahrgang);
          if (lerngruppe == null)
          {
            InformationDialog.Show("Fehler", "Lerngruppe nicht gefunden", false);
            return;
          }

          var vm = App.MainViewModel.LoadLerngruppe(lerngruppe);

          Selection.Instance.Lerngruppe = vm;
          Selection.Instance.Fach = vm.LerngruppeFach;

          var jahresplan = App.MainViewModel.Jahrespläne.FirstOrDefault(o => o.Lerngruppe.Model.Id == vm.Model.Id);
          if (jahresplan == null)
          {
            jahresplan = new JahresplanViewModel(vm);
            App.MainViewModel.Jahrespläne.Add(jahresplan);
          }

          this.CurrentJahresplan = jahresplan;
        }

        this.RaisePropertyChanged("CurrentLerngruppe");
        this.DeleteJahresplanCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt aktuell darzustellenden Jahresplan
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
        this.RaisePropertyChanged("CurrentJahresplan");
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
        this.RaisePropertyChanged("SchuljahrFilter");
        this.LerngruppenView.Refresh();
        this.ResetSchuljahrFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Filtert die Lerngruppen nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Die Lerngruppe, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private void LerngruppenViewSource_Filter(object sender, FilterEventArgs e)
    {
      var lerngruppeViewModel = e.Item as LerngruppeNeu;
      if (lerngruppeViewModel == null)
      {
        e.Accepted = false;
        return;
      }

      if (this.fachFilter != null)
      {
        if (lerngruppeViewModel.Fach.Bezeichnung != this.fachFilter.FachBezeichnung) e.Accepted = false;
        return;
      }


      if (this.schuljahrFilter != null)
      {
        if (lerngruppeViewModel.Schuljahr == null)
        {
          e.Accepted = true;
          return;
        }

        if (lerngruppeViewModel.Schuljahr.Bezeichnung != this.schuljahrFilter.SchuljahrBezeichnung) e.Accepted = false;
        return;
      }

      e.Accepted = true;
      return;
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
        this.LerngruppenView.Refresh();
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
