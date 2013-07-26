namespace Liduv.ViewModel.Jahrespläne
{
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;

  using Liduv.ExceptionHandling;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;

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
    /// Der Jahrtyp für den Filter
    /// </summary>
    private JahrtypViewModel jahrtypFilter;

    /// <summary>
    /// Das Fach für den Filter
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahresplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public JahresplanWorkspaceViewModel()
    {
      this.AddJahresplanCommand = new DelegateCommand(this.AddJahresplan);
      this.DeleteJahresplanCommand = new DelegateCommand(this.DeleteCurrentJahresplan, () => this.CurrentJahresplan != null);
      this.ResetJahrtypFilterCommand = new DelegateCommand(() => this.JahrtypFilter = null, () => this.JahrtypFilter != null);
      this.ResetFachFilterCommand = new DelegateCommand(() => this.FachFilter = null, () => this.FachFilter != null);

      this.CurrentJahresplan = App.MainViewModel.Jahrespläne.Count > 0 ? App.MainViewModel.Jahrespläne[0] : null;
      this.JahrespläneView = CollectionViewSource.GetDefaultView(App.MainViewModel.Jahrespläne);
      this.JahrespläneView.Filter = this.CustomFilter;
      this.JahrespläneView.SortDescriptions.Add(new SortDescription("JahresplanJahrtyp", ListSortDirection.Ascending));
      this.JahrespläneView.SortDescriptions.Add(new SortDescription("JahresplanFach", ListSortDirection.Ascending));
      this.JahrespläneView.SortDescriptions.Add(new SortDescription("JahresplanKlasse", ListSortDirection.Ascending));
      this.JahrespläneView.Refresh();

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Jahrespläne.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentJahresplan))
        {
          this.CurrentJahresplan = null;
        }
      };

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

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetJahrtypFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetFachFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Schultermincollection
    /// </summary>
    public ICollectionView JahrespläneView { get; set; }

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
        this.RaisePropertyChanged("CurrentJahresplan");
        this.DeleteJahresplanCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt den Jahrtypfilter
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
        this.JahrespläneView.Refresh();
        this.ResetJahrtypFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt den Fachfilter
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
    /// Handles addition of a new Jahresplan to the workspace and model
    /// </summary>
    /// <param name="jahrtypViewModel"> The jahrtyp View Model. </param>
    /// <param name="fachViewModel"> The fach View Model. </param>
    /// <param name="klasseViewModel">The klasse View Model. </param>
    /// <param name="sommerHalbjahr"> The sommer Halbjahr. </param>
    public void AddJahresplan(
      JahrtypViewModel jahrtypViewModel,
      FachViewModel fachViewModel,
      KlasseViewModel klasseViewModel,
      bool? sommerHalbjahr)
    {
      var kurzBezeichnung = jahrtypViewModel.JahrtypBezeichnung + " " + fachViewModel.FachBezeichnung + " "
                            + klasseViewModel.KlasseBezeichnung;

      // Check for existing jahresplan
      if (App.MainViewModel.Jahrespläne.Any(o => o.JahresplanJahrtyp == jahrtypViewModel
        && o.JahresplanFach == fachViewModel
        && o.JahresplanKlasse == klasseViewModel))
      {
        // Only show message if we are not in automatic creation mode
        // or creating the vertretungsstunden jahresplan
        if (sommerHalbjahr == null && fachViewModel.FachBezeichnung == "Vertretungsstunden")
        {
          Log.ProcessMessage(
            "Jahresplan vorhanden ...",
            string.Format("Der Jahresplan {0} ist bereits in der Datenbank vorhanden und kann nicht doppelt angelegt werden.", kurzBezeichnung));
        }

        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Jahresplan {0} neu angelegt", kurzBezeichnung), false))
      {
        var jahresplan = new Jahresplan();
        jahresplan.Jahrtyp = jahrtypViewModel.Model;
        jahresplan.Fach = fachViewModel.Model;
        jahresplan.Klasse = klasseViewModel.Model;

        var vm = new JahresplanViewModel(jahresplan);
        App.MainViewModel.Jahrespläne.Add(vm);
        this.CurrentJahresplan = vm;

        // If we should also add complete halbjahr
        if (sommerHalbjahr.HasValue)
        {
          if (sommerHalbjahr.Value)
          {
            vm.AddSommerHalbjahresplan();
          }
          else
          {
            vm.AddWinterHalbjahresplan();
          }
        }
      }
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
        this.JahrespläneView.Refresh();
      }
    }

    /// <summary>
    /// Filtert die Terminliste nach Jahrtyp und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool CustomFilter(object item)
    {
      var jahresplan = item as JahresplanViewModel;
      if (jahresplan == null)
      {
        return false;
      }

      if (this.jahrtypFilter != null && this.fachFilter != null)
      {
        return jahresplan.JahresplanJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung
          && jahresplan.JahresplanFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      if (this.jahrtypFilter != null)
      {
        return jahresplan.JahresplanJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung;
      }

      if (this.fachFilter != null)
      {
        return jahresplan.JahresplanFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      return true;
    }

    /// <summary>
    /// Handles addition of a new Jahresplan to the workspace and model using the
    /// current selection.
    /// </summary>
    private void AddJahresplan()
    {
      this.AddJahresplan(Selection.Instance.Schuljahr, Selection.Instance.Fach, Selection.Instance.Klasse, null);
    }

    /// <summary>
    /// Handles deletion of the current Jahresplan
    /// </summary>
    private void DeleteCurrentJahresplan()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Jahresplan {0} gelöscht.", this.CurrentJahresplan.JahresplanBezeichnung), false))
      {
        App.MainViewModel.Jahrespläne.RemoveTest(this.CurrentJahresplan);
        this.CurrentJahresplan = null;
      }
    }
  }
}
