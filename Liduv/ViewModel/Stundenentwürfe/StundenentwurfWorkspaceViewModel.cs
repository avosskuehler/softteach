namespace Liduv.ViewModel.Stundenentwürfe
{
  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;

  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Stundenentwurf
  /// </summary>
  public class StundenentwurfWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// Das Fach, dessen Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Die Jahrgangsstufe, deren Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private JahrgangsstufeViewModel jahrgangsstufeFilter;

    /// <summary>
    /// Das Modul, dessen Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private ModulViewModel modulFilter;

    /// <summary>
    /// The Stundenentwurf currently selected
    /// </summary>
    private StundenentwurfViewModel currentStundenentwurf;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenentwurfWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public StundenentwurfWorkspaceViewModel()
    {
      this.AddStundenentwurfCommand = new DelegateCommand(this.AddStundenentwurf);
      this.DeleteStundenentwurfCommand = new DelegateCommand(this.DeleteCurrentStundenentwurf, () => this.CurrentStundenentwurf != null);
      this.RemoveFilterCommand = new DelegateCommand(this.RemoveFilter);

      // Init display of subset of modules
      this.FilteredStundenentwürfe = CollectionViewSource.GetDefaultView(App.MainViewModel.Stundenentwürfe);
      this.FilteredStundenentwürfe.Filter = this.FilterStundenentwürfe;

      this.FilteredModule = CollectionViewSource.GetDefaultView(App.MainViewModel.Module);
      this.FilteredModule.Filter = this.FilterModules;

      this.CurrentStundenentwurf = App.MainViewModel.Stundenentwürfe.Count > 0 ? App.MainViewModel.Stundenentwürfe[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Stundenentwürfe.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentStundenentwurf))
        {
          this.CurrentStundenentwurf = null;
        }
      };

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    }

    /// <summary>
    /// Holt oder setzt die gefilterted Stundenentwürfe
    /// </summary>
    public ICollectionView FilteredStundenentwürfe { get; set; }

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
        this.FilteredStundenentwürfe.Refresh();
        this.FilteredModule.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt die jahrgangsstufe filter for the stundenentwurf list.
    /// </summary>
    public JahrgangsstufeViewModel JahrgangsstufeFilter
    {
      get
      {
        return this.jahrgangsstufeFilter;
      }

      set
      {
        this.jahrgangsstufeFilter = value;
        this.RaisePropertyChanged("JahrgangsstufeFilter");
        this.FilteredStundenentwürfe.Refresh();
        this.FilteredModule.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt die modul filter for the stundenentwurf list.
    /// </summary>
    public ModulViewModel ModulFilter
    {
      get
      {
        return this.modulFilter;
      }

      set
      {
        this.modulFilter = value;
        this.RaisePropertyChanged("ModulFilter");
        this.FilteredStundenentwürfe.Refresh();
      }
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Stundenentwurf
    /// </summary>
    public DelegateCommand AddStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Stundenentwurf
    /// </summary>
    public DelegateCommand DeleteStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur resetting the current modul filter
    /// </summary>
    public DelegateCommand RemoveFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die stundenentwurf currently selected in this workspace
    /// </summary>
    public StundenentwurfViewModel CurrentStundenentwurf
    {
      get
      {
        return this.currentStundenentwurf;
      }

      set
      {
        this.currentStundenentwurf = value;
        Selection.Instance.Stundenentwurf = value;
        this.DeleteStundenentwurfCommand.RaiseCanExecuteChanged();
        this.RaisePropertyChanged("CurrentStundenentwurf");
      }
    }

    /// <summary>
    /// Dieser Property changed event handler für die Selection, aktualisiert den
    /// FachFilter.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An empty <see cref="PropertyChangedEventArgs"/></param>
    private void SelectionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Fach")
      {
        this.FachFilter = Selection.Instance.Fach;
      }
    }

    /// <summary>
    /// Handles addition a new Stundenentwurf to the workspace and model
    /// </summary>
    private void AddStundenentwurf()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Stundenentwurf neu erstellt."), false))
      {
        var entwurf = new Stundenentwurf();
        var vm = new StundenentwurfViewModel(entwurf);
        App.MainViewModel.Stundenentwürfe.Add(vm);
        this.CurrentStundenentwurf = vm;
      }
    }

    /// <summary>
    /// Handles addition a new Stundenentwurf to the workspace and model
    /// </summary>
    /// <param name="date">Das Datum des Stundenentwurfs.</param>
    /// <param name="fach">Das Fach des Stundenentwurfs.</param>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe des Stundenentwurfs.</param>
    private void AddStundenentwurf(
      DateTime date,
      Fach fach,
      Jahrgangsstufe jahrgangsstufe)
    {
      var entwurf = new Stundenentwurf();
      entwurf.Datum = date;
      entwurf.Fach = fach;
      entwurf.Jahrgangsstufe = jahrgangsstufe;
      var vm = new StundenentwurfViewModel(entwurf);

      using (new UndoBatch(App.MainViewModel, string.Format("Stundenentwurf {0} erstellt.", vm), false))
      {
        App.MainViewModel.Stundenentwürfe.Add(vm);
        this.CurrentStundenentwurf = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current Stundenentwurf
    /// </summary>
    private void DeleteCurrentStundenentwurf()
    {
      if (this.CurrentStundenentwurf == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Stundenentwurf {0} gelöscht.", this.CurrentStundenentwurf), false))
      {
        App.MainViewModel.Stundenentwürfe.RemoveTest(this.CurrentStundenentwurf);
        this.CurrentStundenentwurf = null;
      }
    }

    /// <summary>
    /// Removes all filter.
    /// </summary>
    private void RemoveFilter()
    {
      this.FachFilter = null;
      this.JahrgangsstufeFilter = null;
      this.ModulFilter = null;
    }

    /// <summary>
    /// The filter predicate that filters the person table view only showing schüler
    /// </summary>
    /// <param name="de">The <see cref="PersonViewModel"/> that should be filtered</param>
    /// <returns>True if the given object should remain in the list.</returns>
    private bool FilterStundenentwürfe(object de)
    {
      var stundenentwurf = de as StundenentwurfViewModel;
      if (stundenentwurf == null)
      {
        return false;
      }

      if (stundenentwurf.StundenentwurfPhasenKurzform == string.Empty)
      {
        return false;
      }

      if (this.FachFilter != null && this.JahrgangsstufeFilter != null && this.ModulFilter != null)
      {
        if (stundenentwurf.StundenentwurfFach == this.FachFilter
          && stundenentwurf.StundenentwurfJahrgangsstufe == this.JahrgangsstufeFilter
          && stundenentwurf.StundenentwurfModul == this.ModulFilter)
        {
          return true;
        }

        return false;
      }

      if (this.FachFilter != null && this.JahrgangsstufeFilter != null)
      {
        if (stundenentwurf.StundenentwurfFach == this.FachFilter
          && stundenentwurf.StundenentwurfJahrgangsstufe == this.JahrgangsstufeFilter)
        {
          return true;
        }

        return false;
      }

      if (this.JahrgangsstufeFilter != null && this.ModulFilter != null)
      {
        if (stundenentwurf.StundenentwurfJahrgangsstufe == this.JahrgangsstufeFilter
          && stundenentwurf.StundenentwurfModul == this.ModulFilter)
        {
          return true;
        }

        return false;
      }

      if (this.FachFilter != null && this.ModulFilter != null)
      {
        if (stundenentwurf.StundenentwurfFach == this.FachFilter
          && stundenentwurf.StundenentwurfModul == this.ModulFilter)
        {
          return true;
        }

        return false;
      }

      if (this.FachFilter != null)
      {
        if (stundenentwurf.StundenentwurfFach == this.FachFilter)
        {
          return true;
        }

        return false;
      }

      if (this.JahrgangsstufeFilter != null)
      {
        if (stundenentwurf.StundenentwurfJahrgangsstufe == this.JahrgangsstufeFilter)
        {
          return true;
        }

        return false;
      }

      if (this.ModulFilter != null)
      {
        if (stundenentwurf.StundenentwurfModul == this.ModulFilter)
        {
          return true;
        }

        return false;
      }

      return true;
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

      if (this.FachFilter != null && this.JahrgangsstufeFilter != null)
      {
        if (modulViewModel.ModulFach == this.FachFilter
          && modulViewModel.ModulJahrgangsstufe == this.JahrgangsstufeFilter)
        {
          return true;
        }

        return false;
      }

      if (this.FachFilter != null)
      {
        if (modulViewModel.ModulFach == this.FachFilter)
        {
          return true;
        }

        return false;
      }

      if (this.JahrgangsstufeFilter != null)
      {
        if (modulViewModel.ModulJahrgangsstufe == this.JahrgangsstufeFilter)
        {
          return true;
        }

        return false;
      }

      return true;
    }
  }
}
