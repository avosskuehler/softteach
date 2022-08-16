namespace SoftTeach.ViewModel.Datenbank
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Data;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class ModulWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// Das Fach, dessen Module nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Der Jahrgang, dessen Module nur dargestellt werden sollen.
    /// </summary>
    private int? jahrgangFilter;

    /// <summary>
    /// The Modul currently selected
    /// </summary>
    private ModulViewModel currentModul;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ModulWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public ModulWorkspaceViewModel()
    {
      this.AddModulCommand = new DelegateCommand(this.AddModul);
      this.DeleteModulCommand = new DelegateCommand(this.DeleteCurrentModul, () => this.CurrentModul != null);
      this.RemoveFilterCommand = new DelegateCommand(this.RemoveFilter);

      // Init display of subset of modules
      this.FilteredModule = CollectionViewSource.GetDefaultView(App.MainViewModel.Module);
      this.FilteredModule.SortDescriptions.Add(new SortDescription("ModulFach", ListSortDirection.Ascending));
      this.FilteredModule.SortDescriptions.Add(new SortDescription("ModulJahrgang", ListSortDirection.Ascending));
      this.FilteredModule.SortDescriptions.Add(new SortDescription("ModulBezeichnung", ListSortDirection.Ascending));
      this.FilteredModule.Filter = this.FilterModules;
    }

    /// <summary>
    /// Holt oder setzt die gefilterten Module
    /// </summary>
    public ICollectionView FilteredModule { get; set; }

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
        this.FilteredModule.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt die jahrgangsstufe filter for the stundenentwurf list.
    /// </summary>
    public int? JahrgangFilter
    {
      get
      {
        return this.jahrgangFilter;
      }

      set
      {
        this.jahrgangFilter = value;
        this.RaisePropertyChanged("JahrgangFilter");
        this.FilteredModule.Refresh();
      }
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Modul
    /// </summary>
    public DelegateCommand AddModulCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Modul
    /// </summary>
    public DelegateCommand DeleteModulCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur resetting the current modul filter
    /// </summary>
    public DelegateCommand RemoveFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die modul currently selected in this workspace
    /// </summary>
    public ModulViewModel CurrentModul
    {
      get
      {
        return this.currentModul;
      }

      set
      {
        this.currentModul = value;
        this.DeleteModulCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Modul to the workspace and model
    /// </summary>
    private void AddModul()
    {
      var modul = new Modul();

      // Check for existing modul
      if (App.MainViewModel.Module.Any(vorhandenerModul => vorhandenerModul.ModulBezeichnung == modul.Bezeichnung))
      {
        Log.ProcessMessage("Modul bereits vorhanden",
          "Dieser Modul ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      modul.Fach = this.FachFilter != null ? this.FachFilter.Model : Selection.Instance.Fach.Model;
      modul.Jahrgang = this.JahrgangFilter != null ? this.JahrgangFilter.Value : Selection.Instance.Lerngruppe.LerngruppeJahrgang;

      var vm = new ModulViewModel(modul);

      App.MainViewModel.Module.Add(vm);
      this.CurrentModul = vm;
    }

    /// <summary>
    /// Handles deletion of the current Modul
    /// </summary>
    private void DeleteCurrentModul()
    {
      var toDelete = this.CurrentModul;
      App.MainViewModel.Module.RemoveTest(toDelete);
      this.CurrentModul = null;
    }

    /// <summary>
    /// Removes the fach and jahrgangsstufe filter.
    /// </summary>
    private void RemoveFilter()
    {
      this.FachFilter = null;
      this.JahrgangFilter = null;
      this.FilteredModule.Refresh();
    }

    /// <summary>
    /// The filter predicate that filters the person table view only showing schüler
    /// </summary>
    /// <param name="de">The <see cref="PersonViewModel"/> that should be filtered</param>
    /// <returns>True if the given object should remain in the list.</returns>
    private bool FilterModules(object de)
    {
      var modulViewModel = de as ModulViewModel;
      if (modulViewModel == null)
      {
        return false;
      }

      if (this.FachFilter != null)
      {
        if (modulViewModel.ModulFach != this.FachFilter)
        {
          return false;
        }
      }

      if (this.JahrgangFilter != null)
      {
        if (modulViewModel.ModulJahrgang != this.JahrgangFilter)
        {
          return false;
        }
      }

      return true;
    }
  }
}
