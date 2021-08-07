namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Personen;

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
    /// The item currently selected
    /// </summary>
    private StundenentwurfEintrag currentStundenentwurfEintrag;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenentwurfWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public StundenentwurfWorkspaceViewModel()
    {
      this.AddStundenentwurfCommand = new DelegateCommand(this.AddStundenentwurf);
      this.DeleteStundenentwurfCommand = new DelegateCommand(this.DeleteCurrentStundenentwurf, () => this.CurrentStundenentwurf != null);
      this.DeleteStundenentwurfEintragCommand = new DelegateCommand(this.DeleteCurrentStundenentwurfEintrag, () => this.CurrentStundenentwurfEintrag != null);
      this.RemoveFilterCommand = new DelegateCommand(this.RemoveFilter);

      this.FilteredAndSortedStundenentwürfe = new List<StundenentwurfEintrag>();
      this.SelectedStundenentwurfEinträge = new List<StundenentwurfEintrag>();

      this.PopulateStundenentwürfe();

      // Init display of subset of modules
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

    private void PopulateStundenentwürfe()
    {
      this.FilteredAndSortedStundenentwürfe.Clear();
      var pläne = App.MainViewModel.Jahrespläne.Where(
        o =>
        o.JahresplanFach == this.FachFilter
        && o.JahresplanJahrgangsstufe == this.JahrgangsstufeFilter);

      var lastJahrtyp = App.MainViewModel.Jahrtypen[App.MainViewModel.Jahrtypen.Count - 1];
      if (DateTime.Now.Month > 7 || DateTime.Now.Month == 1)
      {
        Selection.Instance.Halbjahr = App.MainViewModel.Halbjahrtypen[0];
      }
      else
      {
        Selection.Instance.Halbjahr = App.MainViewModel.Halbjahrtypen[1];
      }

      foreach (var jahresplanViewModel in pläne)
      {
        foreach (var halbjahresplanViewModel in jahresplanViewModel.Halbjahrespläne)
        {
          if (halbjahresplanViewModel.HalbjahresplanHalbjahrtyp == Selection.Instance.Halbjahr && jahresplanViewModel.JahresplanJahrtyp == lastJahrtyp)
          {
            continue;
          }

          foreach (var monatsplanViewModel in halbjahresplanViewModel.Monatspläne)
          {
            foreach (var tagesplanViewModel in monatsplanViewModel.Tagespläne)
            {
              foreach (var lerngruppenterminViewModel in tagesplanViewModel.Lerngruppentermine)
              {
                if (lerngruppenterminViewModel is Termine.StundeViewModel)
                {
                  var stunde = lerngruppenterminViewModel as Termine.StundeViewModel;
                  if (stunde.StundeStundenentwurf != null)
                  {
                    if (stunde.StundeModul == this.ModulFilter)
                    {
                      this.FilteredAndSortedStundenentwürfe.Add(new StundenentwurfEintrag(jahresplanViewModel, stunde.LerngruppenterminKlasse, stunde.StundeStundenentwurf, stunde.LerngruppenterminDatum));
                    }
                  }
                }
              }
            }
          }
        }
      }

      var unbenutzteEntwürfe = App.MainViewModel.Stundenentwürfe.Where(o => !o.StundenentwurfStundenCollection.Any()
        && o.StundenentwurfFach == this.FachFilter
        && o.StundenentwurfJahrgangsstufe == this.JahrgangsstufeFilter
        && o.StundenentwurfModul == this.ModulFilter);

      foreach (var stundenentwurfViewModel in unbenutzteEntwürfe)
      {
        this.FilteredAndSortedStundenentwürfe.Add(new StundenentwurfEintrag(App.MainViewModel.Jahrespläne[0], "Nicht zugeordnet", stundenentwurfViewModel, null));
      }

      this.FilteredAndSortedStundenentwürfe = this.FilteredAndSortedStundenentwürfe.OrderBy(o => o.Termin).ToList();
      this.RaisePropertyChanged("FilteredAndSortedStundenentwürfe");
    }

    /// <summary>
    /// Holt oder setzt die gefilterten und sortierten Stundenentwürfe
    /// </summary>
    public List<StundenentwurfEintrag> FilteredAndSortedStundenentwürfe { get; set; }

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
        this.PopulateStundenentwürfe();
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
        this.FilteredModule.Refresh();
        this.PopulateStundenentwürfe();
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
        this.PopulateStundenentwürfe();
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
    /// Holt den Befehl um den aktuellen Stundenwurfeintrag zu löschen
    /// </summary>
    public DelegateCommand DeleteStundenentwurfEintragCommand { get; private set; }

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
    /// Holt oder setzt den ausgewählten Stundenentwurfseintrag
    /// </summary>
    public StundenentwurfEintrag CurrentStundenentwurfEintrag
    {
      get
      {
        return this.currentStundenentwurfEintrag;
      }

      set
      {
        this.currentStundenentwurfEintrag = value;
        this.RaisePropertyChanged("CurrentStundenentwurfEintrag");
      }
    }

    /// <summary>
    /// Holt die markierten Stundenentwürfe
    /// </summary>
    public IList SelectedStundenentwurfEinträge { get; set; }

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
        App.UnitOfWork.Context.Stundenentwürfe.Add(entwurf);
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
        App.UnitOfWork.Context.Stundenentwürfe.Add(entwurf);
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
        App.UnitOfWork.Context.Stundenentwürfe.Remove(this.CurrentStundenentwurf.Model);
        App.MainViewModel.Stundenentwürfe.RemoveTest(this.CurrentStundenentwurf);
        this.CurrentStundenentwurf = null;
      }
    }

    /// <summary>
    /// Löscht den aktuellen Stundenentwurfeintrag im 
    /// </summary>
    private void DeleteCurrentStundenentwurfEintrag()
    {
      if (this.CurrentStundenentwurfEintrag == null)
      {
        return;
      }

      if (this.CurrentStundenentwurfEintrag.Stundenentwurf.StundenentwurfStundenCollection.Any())
      {
        var stunde = this.CurrentStundenentwurf.StundenentwurfStundenCollection.First();
        InformationDialog.Show(
          "Noch verwendet.",
          string.Format("Dieser Stundenentwurf wird noch in der Stunde {0} vom {1} in Klasse {2} verwendet und daher nicht gelöscht", stunde.Beschreibung, stunde.Tagesplan.Datum.ToString("dd.MM.yyyy"), stunde.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Bezeichnung),
          false);

        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Stundenentwurf {0} gelöscht.", this.CurrentStundenentwurfEintrag.Stundenentwurf), false))
      {
        this.FilteredAndSortedStundenentwürfe.Remove(
          this.FilteredAndSortedStundenentwürfe.First(o => o.Stundenentwurf == this.CurrentStundenentwurfEintrag.Stundenentwurf));
        App.MainViewModel.Stundenentwürfe.RemoveTest(this.CurrentStundenentwurfEintrag.Stundenentwurf);
        this.CurrentStundenentwurfEintrag = null;
        this.FilteredAndSortedStundenentwürfe = this.FilteredAndSortedStundenentwürfe.OrderBy(o => o.Termin).ToList();
        this.RaisePropertyChanged("FilteredAndSortedStundenentwürfe");
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
