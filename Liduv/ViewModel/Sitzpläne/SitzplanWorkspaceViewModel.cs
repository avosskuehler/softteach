namespace Liduv.ViewModel.Sitzpläne
{
  using System;
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
  /// ViewModel for managing Sitzplan
  /// </summary>
  public class SitzplanWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Sitzplan currently selected
    /// </summary>
    private SitzplanViewModel currentSitzplan;

    /// <summary>
    /// Der Jahrtyp für den Filter
    /// </summary>
    private JahrtypViewModel jahrtypFilter;

    /// <summary>
    /// Das Fach für den Filter
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SitzplanWorkspaceViewModel()
    {
      this.AddSitzplanCommand = new DelegateCommand(this.AddSitzplan);
      this.DeleteSitzplanCommand = new DelegateCommand(this.DeleteCurrentSitzplan, () => this.CurrentSitzplan != null);
      this.ResetJahrtypFilterCommand = new DelegateCommand(() => this.JahrtypFilter = null, () => this.JahrtypFilter != null);
      this.ResetFachFilterCommand = new DelegateCommand(() => this.FachFilter = null, () => this.FachFilter != null);

      var numberOfSitzpläne = App.MainViewModel.Sitzpläne.Count;
      this.CurrentSitzplan = numberOfSitzpläne > 0 ? App.MainViewModel.Sitzpläne[numberOfSitzpläne - 1] : null;
      this.SitzpläneView = CollectionViewSource.GetDefaultView(App.MainViewModel.Sitzpläne);
      this.SitzpläneView.Filter = this.CustomFilter;
      this.SitzpläneView.SortDescriptions.Add(new SortDescription("SitzplanSchülerliste", ListSortDirection.Ascending));
      this.SitzpläneView.SortDescriptions.Add(new SortDescription("SitzplanRaumplan", ListSortDirection.Ascending));
      this.SitzpläneView.Refresh();

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Sitzpläne.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentSitzplan))
        {
          this.CurrentSitzplan = null;
        }
      };

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Sitzplan
    /// </summary>
    public DelegateCommand AddSitzplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Sitzplan
    /// </summary>
    public DelegateCommand DeleteSitzplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Sitzplan
    /// </summary>
    public DelegateCommand ResetJahrtypFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Sitzplan
    /// </summary>
    public DelegateCommand ResetFachFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Schultermincollection
    /// </summary>
    public ICollectionView SitzpläneView { get; set; }

    /// <summary>
    /// Holt oder setzt die sitzplan currently selected in this workspace
    /// </summary>
    public SitzplanViewModel CurrentSitzplan
    {
      get
      {
        return this.currentSitzplan;
      }

      set
      {
        this.currentSitzplan = value;
        this.RaisePropertyChanged("CurrentSitzplan");
        this.DeleteSitzplanCommand.RaiseCanExecuteChanged();
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
        if (value != null && Selection.Instance.Jahrtyp != value)
        {
          Selection.Instance.Jahrtyp = value;
        }

        this.RaisePropertyChanged("JahrtypFilter");
        this.SitzpläneView.Refresh();
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
        if (value != null && Selection.Instance.Fach != value)
        {
          Selection.Instance.Fach = value;
        }

        this.RaisePropertyChanged("FachFilter");
        this.SitzpläneView.Refresh();
        this.ResetFachFilterCommand.RaiseCanExecuteChanged();
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
        this.JahrtypFilter = Selection.Instance.Jahrtyp;
        this.SitzpläneView.Refresh();
      }
      else if (e.PropertyName == "Fach")
      {
        this.FachFilter = Selection.Instance.Fach;
        this.SitzpläneView.Refresh();
      }
    }

    /// <summary>
    /// Filtert die Sitzplanliste nach Jahrtyp und Fach
    /// </summary>
    /// <param name="item">Das SitzplanViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool CustomFilter(object item)
    {
      var sitzplan = item as SitzplanViewModel;
      if (sitzplan == null)
      {
        return false;
      }

      if (sitzplan.SitzplanSchülerliste == null)
      {
        return true;
      }

      if (this.jahrtypFilter != null && this.fachFilter != null)
      {
        return sitzplan.SitzplanSchülerliste.SchülerlisteJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung
          && sitzplan.SitzplanSchülerliste.SchülerlisteFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      if (this.jahrtypFilter != null)
      {
        return sitzplan.SitzplanSchülerliste.SchülerlisteJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung;
      }

      if (this.fachFilter != null)
      {
        return sitzplan.SitzplanSchülerliste.SchülerlisteFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      return true;
    }

    /// <summary>
    /// Handles addition of a new Sitzplan to the workspace and model.
    /// </summary>
    private void AddSitzplan()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sitzplan neu angelegt"), false))
      {
        var sitzplan = new Sitzplan();
        sitzplan.Bezeichnung = "Neuer Sitzplan";
        sitzplan.GültigAb = DateTime.Now.Date;
        sitzplan.Raumplan = Selection.Instance.Raumplan.Model;
        sitzplan.Schülerliste = Selection.Instance.Schülerliste.Model;

        var vm = new SitzplanViewModel(sitzplan);
        App.MainViewModel.Sitzpläne.Add(vm);
        this.CurrentSitzplan = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current Sitzplan
    /// </summary>
    private void DeleteCurrentSitzplan()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sitzplan {0} gelöscht.", this.CurrentSitzplan.SitzplanBezeichnung), false))
      {
        App.MainViewModel.Sitzpläne.RemoveTest(this.CurrentSitzplan);
        this.CurrentSitzplan = null;
      }
    }
  }
}
