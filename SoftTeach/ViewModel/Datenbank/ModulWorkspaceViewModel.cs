namespace SoftTeach.ViewModel.Datenbank
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class ModulWorkspaceViewModel : DependencyObject
  {
    /// <summary>
    /// The <see cref="DependencyProperty"/> for the <see cref="ShownModule"/>.
    /// Is needed to get updates on the tables once the filter combos are selected.
    /// </summary>
    public static readonly DependencyProperty ShownModuleProperty = DependencyProperty.Register(
      "ShownModule",
      typeof(ObservableCollection<ModulViewModel>),
      typeof(ModulWorkspaceViewModel),
      new UIPropertyMetadata(null));

    /// <summary>
    /// The <see cref="DependencyProperty"/> for the <see cref="FachFilter"/>.
    /// Is needed to get updates on the filter combos once the filters are reset.
    /// </summary>
    public static readonly DependencyProperty FachFilterProperty = DependencyProperty.Register(
      "FachFilter",
      typeof(FachViewModel),
      typeof(ModulWorkspaceViewModel),
      new UIPropertyMetadata(OnPropertyChanged));

    /// <summary>
    /// The <see cref="DependencyProperty"/> for the <see cref="JahrgangsstufeFilter"/>.
    /// Is needed to get updates on the filter combos once the filters are reset.
    /// </summary>
    public static readonly DependencyProperty JahrgangsstufeFilterProperty = DependencyProperty.Register(
      "JahrgangsstufeFilter",
      typeof(int?),
      typeof(ModulWorkspaceViewModel),
      new UIPropertyMetadata(OnPropertyChanged));

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
      this.ShownModule = new ObservableCollection<ModulViewModel>();
      this.FilterModules();
      this.CurrentModul = App.MainViewModel.Module.Count > 0 ? App.MainViewModel.Module[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Module.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentModul))
        {
          this.CurrentModul = null;
        }
      };

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    }


    /// <summary>
    /// Holt oder setzt die Shown module dependency property which is a subset of
    /// AllModule to display filtered views of the long list of modules
    /// </summary>
    public ObservableCollection<ModulViewModel> ShownModule
    {
      get { return (ObservableCollection<ModulViewModel>)this.GetValue(ShownModuleProperty); }
      set { this.SetValue(ShownModuleProperty, value); }
    }

    /// <summary>
    /// Holt oder setzt die fach filter for the module list.
    /// </summary>
    public FachViewModel FachFilter
    {
      get { return (FachViewModel)this.GetValue(FachFilterProperty); }
      set { this.SetValue(FachFilterProperty, value); }
    }

    /// <summary>
    /// Holt oder setzt die jahrgangsstufe filter for the module list.
    /// </summary>
    public int? JahrgangsstufeFilter
    {
      get { return (int)this.GetValue(JahrgangsstufeFilterProperty); }
      set { this.SetValue(JahrgangsstufeFilterProperty, value); }
    }

    /// <summary>
    /// Raises the <see cref="ViewModelBase.PropertyChanged"/> event.
    /// </summary>
    /// <param name="obj">The source of the event. This.</param>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> with 
    /// the event data.</param>
    private static void OnPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      (obj as ModulWorkspaceViewModel).OnPropertyChanged(args);
    }

    /// <summary>
    /// Raises the <see cref="ViewModelBase.PropertyChanged"/> event.
    /// </summary>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> with 
    /// the event data.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (args.Property.Name == "FachFilter" || args.Property.Name == "JahrgangsstufeFilter")
      {
        this.FilterModules();
      }
    }

     /// <summary>
    /// Updates the filter combos when selection has changed.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event arguments.</param>
    private void SelectionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.FachFilter = Selection.Instance.Fach;
      //this.JahrgangsstufeFilter = Selection.Instance.Jahrgangsstufe;
      this.FilterModules();
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
      var modul = new ModulNeu();

      // Check for existing modul
      if (App.MainViewModel.Module.Any(vorhandenerModul => vorhandenerModul.ModulBezeichnung == modul.Bezeichnung))
      {
        Log.ProcessMessage("Modul bereits vorhanden",
          "Dieser Modul ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      modul.Fach = this.FachFilter != null ? this.FachFilter.Model : Selection.Instance.Fach.Model;
      modul.Jahrgang = this.JahrgangsstufeFilter != null ? this.JahrgangsstufeFilter.Value : Selection.Instance.Lerngruppe.LerngruppeJahrgang;

      // App.UnitOfWork.GetRepository<Modul>().Add(modul);
      var vm = new ModulViewModel(modul);

      App.MainViewModel.Module.Add(vm);
      this.ShownModule.Add(vm);
      this.CurrentModul = vm;
    }

    /// <summary>
    /// Handles deletion of the current Modul
    /// </summary>
    private void DeleteCurrentModul()
    {
      // App.UnitOfWork.GetRepository<Modul>().RemoveTest(this.CurrentModul.Model);
      var toDelete = this.CurrentModul;
      this.ShownModule.RemoveTest(this.CurrentModul);
      App.MainViewModel.Module.RemoveTest(toDelete);
      this.CurrentModul = null;
    }

    /// <summary>
    /// Removes the fach and jahrgangsstufe filter.
    /// </summary>
    private void RemoveFilter()
    {
      this.FachFilter = null;
      this.JahrgangsstufeFilter = null;
      this.FilterModules();
    }

    /// <summary>
    /// This method filters all modules by given fach and jahrgangsstufe
    /// given by the comboboxes.
    /// </summary>
    private void FilterModules()
    {
      // Clear sublist of modules
      this.ShownModule.Clear();

      IEnumerable<ModulViewModel> filteredModules = null;
      if (this.FachFilter != null && this.JahrgangsstufeFilter != null)
      {
        // Filter Modules of complete list
        filteredModules = App.MainViewModel.Module.Where(
          modul => modul.ModulFach == this.FachFilter &&
            modul.ModulJahrgang == this.JahrgangsstufeFilter);
      }
      else if (this.FachFilter != null)
      {
        // Filter Modules of complete list
        filteredModules = App.MainViewModel.Module.Where(modul => modul.ModulFach == this.FachFilter);
      }
      else if (this.JahrgangsstufeFilter != null)
      {
        // Filter Modules of complete list
        filteredModules = App.MainViewModel.Module.Where(modul => modul.ModulJahrgang == this.JahrgangsstufeFilter);
      }

      // Add Module
      foreach (var module in App.MainViewModel.Module)
      {
        if (filteredModules == null || filteredModules.Contains(module))
        {
          this.ShownModule.Add(module);
        }
      }
    }
  }
}
