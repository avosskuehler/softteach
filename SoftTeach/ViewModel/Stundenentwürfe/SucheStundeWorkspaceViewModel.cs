namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// ViewModel for managing Stundenentwurf
  /// </summary>
  public class SucheStundeWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// Das Fach, dessen Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Die Jahrgangsstufe, deren Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private int? jahrgangFilter;

    /// <summary>
    /// Das Modul, dessen Stundenentwürfe nur dargestellt werden sollen.
    /// </summary>
    private ModulViewModel modulFilter;

    /// <summary>
    /// The item currently selected
    /// </summary>
    private Stunde currentStunde;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="SucheStundeWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SucheStundeWorkspaceViewModel()
    {
      this.DeleteStundeCommand = new DelegateCommand(this.DeleteSelectedStunde, () => this.CurrentStunde != null);
      this.RemoveFilterCommand = new DelegateCommand(this.RemoveFilter);

      this.StundenFürFachUndJahrgang = new List<Stunde>();
      this.SelectedStunden = new List<Stunde>();

      this.PopulateStunden();

      this.FilteredStunden = CollectionViewSource.GetDefaultView(this.StundenFürFachUndJahrgang);
      this.FilteredStunden.GroupDescriptions.Add(new PropertyGroupDescription("Lerngruppe.Schuljahr.Jahr"));
      this.FilteredStunden.Filter = this.FilterStunden;

      // Init display of subset of modules
      this.FilteredModule = CollectionViewSource.GetDefaultView(App.MainViewModel.Module);
      this.FilteredModule.Filter = this.FilterModules;

      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    }

    private void PopulateStunden()
    {
      UiServices.SetBusyState();
      this.StundenFürFachUndJahrgang.Clear();

      if (this.FachFilter == null)
      {
        return;
      }

      if (this.JahrgangFilter == null)
      {
        return;
      }

      var stundenNachFilter = App.UnitOfWork.Context.Termine.OfType<Stunde>().Where(o => o.FachId == this.fachFilter.Model.Id && o.Jahrgang == this.jahrgangFilter);

      foreach (var stunde in stundenNachFilter)
      {
        this.StundenFürFachUndJahrgang.Add(stunde);
      }

      this.FilteredStunden.Refresh();
    }

    /// <summary>
    /// Holt oder setzt die gefilterten und sortierten Stundenentwürfe
    /// </summary>
    public List<Stunde> StundenFürFachUndJahrgang { get; set; }

    /// <summary>
    /// Holt oder setzt die gefilterten Module
    /// </summary>
    public ICollectionView FilteredModule { get; set; }

    /// <summary>
    /// Holt oder setzt die gefilterten Module
    /// </summary>
    public ICollectionView FilteredStunden { get; set; }

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
        this.PopulateStunden();
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
        this.PopulateStunden();
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
        this.FilteredStunden.Refresh();
      }
    }

    /// <summary>
    /// Holt den Befehl um den aktuellen Stundenwurfeintrag zu löschen
    /// </summary>
    public DelegateCommand DeleteStundeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur resetting the current modul filter
    /// </summary>
    public DelegateCommand RemoveFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt den ausgewählten Stunde
    /// </summary>
    public Stunde CurrentStunde
    {
      get
      {
        return this.currentStunde;
      }

      set
      {
        this.currentStunde = value;
        this.RaisePropertyChanged("CurrentStunde");
      }
    }

    /// <summary>
    /// Holt die markierten Stundenentwürfe
    /// </summary>
    public IList SelectedStunden { get; set; }

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
    /// Löscht alle markierten Stundenentwurfeinträge
    /// </summary>
    private void DeleteSelectedStunde()
    {
      if (this.SelectedStunden.Count == 0)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("{0} Stundenentwürfe gelöscht.", this.SelectedStunden.Count), false))
      {
        //foreach (StundenentwurfEintrag stundenentwurfEintrag in this.SelectedStunden)
        //{
        //  foreach (var stundenentwurfEintrag2 in this.FilteredAndSortedStunden.Where(o => o.Stundenentwurf == this.CurrentStunde.Stundenentwurf).ToList())
        //  {
        //    this.FilteredAndSortedStunden.RemoveTest(stundenentwurfEintrag2);
        //  }
        //  App.MainViewModel.Stunden.RemoveTest(this.CurrentStunde.Stundenentwurf);
        //  App.UnitOfWork.Context.Stundenentwürfe.Remove(this.CurrentStunde.Stundenentwurf.Model);
        //  this.CurrentStunde = null;
        //  this.FilteredAndSortedStunden = this.FilteredAndSortedStunden.OrderBy(o => o.Termin).ToList();
        //  this.RaisePropertyChanged("FilteredAndSortedStundenentwurfEinträge");
        //}
      }
    }

    /// <summary>
    /// Removes all filter.
    /// </summary>
    private void RemoveFilter()
    {
      this.FachFilter = null;
      this.JahrgangFilter = null;
      this.ModulFilter = null;
    }

    /// <summary>
    /// The filter predicate that filters the person table view only showing schüler
    /// </summary>
    /// <param name="de">The <see cref="PersonViewModel"/> that should be filtered</param>
    /// <returns>True if the given object should remain in the list.</returns>
    private bool FilterStunden(object de)
    {
      var stundeViewModel = de as Stunde;
      if (stundeViewModel == null)
      {
        return false;
      }

      if (this.ModulFilter != null)
      {
        if (stundeViewModel.ModulId == this.ModulFilter.Model.Id)
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

      if (this.FachFilter != null && this.JahrgangFilter != null)
      {
        if (modulViewModel.ModulFach == this.FachFilter
          && modulViewModel.ModulJahrgang == this.JahrgangFilter)
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

      if (this.JahrgangFilter != null)
      {
        if (modulViewModel.ModulJahrgang == this.JahrgangFilter)
        {
          return true;
        }

        return false;
      }

      return true;
    }
  }
}
