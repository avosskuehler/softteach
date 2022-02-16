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

  /// <summary>
  /// ViewModel for managing Stundenentwurf
  /// </summary>
  public class StundenentwurfWorkspaceViewModel : ViewModelBase
  {
    ///// <summary>
    ///// Das Fach, dessen Stundenentwürfe nur dargestellt werden sollen.
    ///// </summary>
    //private FachViewModel fachFilter;

    ///// <summary>
    ///// Die Jahrgangsstufe, deren Stundenentwürfe nur dargestellt werden sollen.
    ///// </summary>
    //private JahrgangsstufeViewModel jahrgangsstufeFilter;

    ///// <summary>
    ///// Das Modul, dessen Stundenentwürfe nur dargestellt werden sollen.
    ///// </summary>
    //private ModulViewModel modulFilter;

    ///// <summary>
    ///// The item currently selected
    ///// </summary>
    //private StundenentwurfEintrag currentStundenentwurfEintrag;

    ///// <summary>
    ///// Initialisiert eine neue Instanz der <see cref="StundenentwurfWorkspaceViewModel"/> Klasse. 
    ///// </summary>
    //public StundenentwurfWorkspaceViewModel()
    //{
    //  this.DeleteStundenentwurfEintragCommand = new DelegateCommand(this.DeleteSelectedStundenentwurfEinträge, () => this.CurrentStundenentwurfEintrag != null);
    //  this.RemoveFilterCommand = new DelegateCommand(this.RemoveFilter);

    //  this.FilteredAndSortedStundenentwurfEinträge = new List<StundenentwurfEintrag>();
    //  this.SelectedStundenentwurfEinträge = new List<StundenentwurfEintrag>();

    //  this.PopulateStundenentwurfEinträge();

    //  // Init display of subset of modules
    //  this.FilteredModule = CollectionViewSource.GetDefaultView(App.MainViewModel.Module);
    //  this.FilteredModule.Filter = this.FilterModules;

    //  Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    //}

    //private void PopulateStundenentwurfEinträge()
    //{
    //  UiServices.SetBusyState();
    //  this.FilteredAndSortedStundenentwurfEinträge.Clear();

    //  if (this.FachFilter == null)
    //  {
    //    return;
    //  }

    //  if (this.JahrgangsstufeFilter == null)
    //  {
    //    return;
    //  }

    //  foreach (var jahresplan in App.UnitOfWork.Context.Jahrespläne.Where(o => o.FachId == this.FachFilter.Model.Id && o.Klasse.Klassenstufe.JahrgangsstufeId == this.JahrgangsstufeFilter.Model.Id))
    //  {
    //    if (!App.MainViewModel.Jahrespläne.Any(o => o.Model.Id == jahresplan.Id))
    //    {
    //      App.MainViewModel.LoadJahresplan(jahresplan);
    //    }
    //  }

    //  var pläne = App.MainViewModel.Jahrespläne.Where(
    //    o =>
    //    o.JahresplanFach == this.FachFilter
    //    && o.JahresplanJahrgangsstufe == this.JahrgangsstufeFilter);

    //  var lastSchuljahr = App.MainViewModel.Schuljahre[App.MainViewModel.Schuljahre.Count - 1];
    //  if (DateTime.Now.Month > 7 || DateTime.Now.Month == 1)
    //  {
    //    Selection.Instance.Halbjahr = App.MainViewModel.Halbjahre[0];
    //  }
    //  else
    //  {
    //    Selection.Instance.Halbjahr = App.MainViewModel.Halbjahre[1];
    //  }

    //  foreach (var stundenentwurf in App.UnitOfWork.Context.Stundenentwürfe.Where(o => o.FachId == this.FachFilter.Model.Id && o.JahrgangsstufeId == this.JahrgangsstufeFilter.Model.Id))
    //  {
    //    if (!App.MainViewModel.Stunden.Any(o => o.Model.Id == stundenentwurf.Id))
    //    {
    //      App.MainViewModel.LoadStundenentwurf(stundenentwurf);
    //    }
    //  }

    //  foreach (var jahresplanViewModel in pläne)
    //  {
    //    foreach (var halbjahresplanViewModel in jahresplanViewModel.Halbjahrespläne)
    //    {
    //      //if (halbjahresplanViewModel.HalbjahresplanHalbjahr == Selection.Instance.Halbjahr && jahresplanViewModel.JahresplanSchuljahr == lastSchuljahr)
    //      //{
    //      //  continue;
    //      //}

    //      foreach (var monatsplanViewModel in halbjahresplanViewModel.Monatspläne)
    //      {
    //        foreach (var tagesplanViewModel in monatsplanViewModel.Tagespläne)
    //        {
    //          foreach (var lerngruppenterminViewModel in tagesplanViewModel.Lerngruppentermine)
    //          {
    //            if (lerngruppenterminViewModel is Termine.StundeViewModel)
    //            {
    //              var stunde = lerngruppenterminViewModel as Termine.StundeViewModel;
    //              if (stunde.StundeStundenentwurf != null)
    //              {
    //                if (this.ModulFilter == null || stunde.StundeModul == this.ModulFilter)
    //                {
    //                  this.FilteredAndSortedStundenentwurfEinträge.Add(new StundenentwurfEintrag(jahresplanViewModel, stunde.LerngruppenterminLerngruppe, stunde.StundeStundenentwurf, stunde.LerngruppenterminDatum));
    //                }
    //              }
    //            }
    //          }
    //        }
    //      }
    //    }
    //  }


    //  var unbenutzteEntwürfe = App.MainViewModel.Stunden.Where(o => !o.StundenentwurfStundenCollection.Any()
    //    && o.StundenentwurfFach == this.FachFilter
    //    && o.StundenentwurfJahrgangsstufe == this.JahrgangsstufeFilter
    //    && o.StundenentwurfModul == this.ModulFilter);

    //  if (this.modulFilter == null)
    //  {
    //    unbenutzteEntwürfe = App.MainViewModel.Stunden.Where(o => !o.StundenentwurfStundenCollection.Any()
    //      && o.StundenentwurfFach == this.FachFilter
    //      && o.StundenentwurfJahrgangsstufe == this.JahrgangsstufeFilter);
    //  }

    //  foreach (var stundenentwurfViewModel in unbenutzteEntwürfe)
    //  {
    //    this.FilteredAndSortedStundenentwurfEinträge.Add(new StundenentwurfEintrag(App.MainViewModel.Jahrespläne[0], "Nicht zugeordnet", stundenentwurfViewModel, null));
    //  }

    //  this.FilteredAndSortedStundenentwurfEinträge = this.FilteredAndSortedStundenentwurfEinträge.OrderBy(o => o.Termin).ToList();
    //  this.RaisePropertyChanged("FilteredAndSortedStundenentwurfEinträge");
    //}

    ///// <summary>
    ///// Holt oder setzt die gefilterten und sortierten Stundenentwürfe
    ///// </summary>
    //public List<StundenentwurfEintrag> FilteredAndSortedStundenentwurfEinträge { get; set; }

    ///// <summary>
    ///// Holt oder setzt die gefilterten Module
    ///// </summary>
    //public ICollectionView FilteredModule { get; set; }

    ///// <summary>
    ///// Holt oder setzt die fach filter for the stundenentwurf list.
    ///// </summary>
    //public FachViewModel FachFilter
    //{
    //  get
    //  {
    //    return this.fachFilter;
    //  }

    //  set
    //  {
    //    this.fachFilter = value;
    //    this.RaisePropertyChanged("FachFilter");
    //    this.FilteredModule.Refresh();
    //    this.PopulateStundenentwurfEinträge();
    //  }
    //}

    ///// <summary>
    ///// Holt oder setzt die jahrgangsstufe filter for the stundenentwurf list.
    ///// </summary>
    //public JahrgangsstufeViewModel JahrgangsstufeFilter
    //{
    //  get
    //  {
    //    return this.jahrgangsstufeFilter;
    //  }

    //  set
    //  {
    //    this.jahrgangsstufeFilter = value;
    //    this.RaisePropertyChanged("JahrgangsstufeFilter");
    //    this.FilteredModule.Refresh();
    //    this.PopulateStundenentwurfEinträge();
    //  }
    //}

    ///// <summary>
    ///// Holt oder setzt die modul filter for the stundenentwurf list.
    ///// </summary>
    //public ModulViewModel ModulFilter
    //{
    //  get
    //  {
    //    return this.modulFilter;
    //  }

    //  set
    //  {
    //    this.modulFilter = value;
    //    this.RaisePropertyChanged("ModulFilter");
    //    this.PopulateStundenentwurfEinträge();
    //  }
    //}

    ///// <summary>
    ///// Holt den Befehl um den aktuellen Stundenwurfeintrag zu löschen
    ///// </summary>
    //public DelegateCommand DeleteStundenentwurfEintragCommand { get; private set; }

    ///// <summary>
    ///// Holt den Befehl zur resetting the current modul filter
    ///// </summary>
    //public DelegateCommand RemoveFilterCommand { get; private set; }

    ///// <summary>
    ///// Holt oder setzt den ausgewählten Stundenentwurfseintrag
    ///// </summary>
    //public StundenentwurfEintrag CurrentStundenentwurfEintrag
    //{
    //  get
    //  {
    //    return this.currentStundenentwurfEintrag;
    //  }

    //  set
    //  {
    //    this.currentStundenentwurfEintrag = value;
    //    this.RaisePropertyChanged("CurrentStundenentwurfEintrag");
    //  }
    //}

    ///// <summary>
    ///// Holt die markierten Stundenentwürfe
    ///// </summary>
    //public IList SelectedStundenentwurfEinträge { get; set; }

    ///// <summary>
    ///// Dieser Property changed event handler für die Selection, aktualisiert den
    ///// FachFilter.
    ///// </summary>
    ///// <param name="sender">Source of the event</param>
    ///// <param name="e">An empty <see cref="PropertyChangedEventArgs"/></param>
    //private void SelectionPropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //  if (e.PropertyName == "Fach")
    //  {
    //    this.FachFilter = Selection.Instance.Fach;
    //  }
    //}

    ///// <summary>
    ///// Löscht alle markierten Stundenentwurfeinträge
    ///// </summary>
    //private void DeleteSelectedStundenentwurfEinträge()
    //{
    //  if (this.SelectedStundenentwurfEinträge.Count == 0)
    //  {
    //    return;
    //  }

    //  using (new UndoBatch(App.MainViewModel, string.Format("{0} Stundenentwürfe gelöscht.", this.SelectedStundenentwurfEinträge.Count), false))
    //  {
    //    foreach (StundenentwurfEintrag stundenentwurfEintrag in this.SelectedStundenentwurfEinträge)
    //    {
    //      if (stundenentwurfEintrag.Stundenentwurf.StundenentwurfStundenCollection.Any())
    //      {
    //        var stunde = this.CurrentStundenentwurfEintrag.Stundenentwurf.StundenentwurfStundenCollection.First();
    //        InformationDialog.Show(
    //          "Noch verwendet.",
    //          string.Format("Ein Stundenentwurf wird noch in der Stunde {0} vom {1} in Klasse {2} verwendet und daher nicht gelöscht", stunde.Beschreibung, stunde.Tagesplan.Datum.ToString("dd.MM.yyyy"), stunde.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Bezeichnung),
    //          false);

    //        return;
    //      }

    //      foreach (var stundenentwurfEintrag2 in this.FilteredAndSortedStundenentwurfEinträge.Where(o => o.Stundenentwurf == this.CurrentStundenentwurfEintrag.Stundenentwurf).ToList())
    //      {
    //        this.FilteredAndSortedStundenentwurfEinträge.RemoveTest(stundenentwurfEintrag2);
    //      }
    //      App.MainViewModel.Stunden.RemoveTest(this.CurrentStundenentwurfEintrag.Stundenentwurf);
    //      App.UnitOfWork.Context.Stundenentwürfe.Remove(this.CurrentStundenentwurfEintrag.Stundenentwurf.Model);
    //      this.CurrentStundenentwurfEintrag = null;
    //      this.FilteredAndSortedStundenentwurfEinträge = this.FilteredAndSortedStundenentwurfEinträge.OrderBy(o => o.Termin).ToList();
    //      this.RaisePropertyChanged("FilteredAndSortedStundenentwurfEinträge");
    //    }
    //  }
    //}

    ///// <summary>
    ///// Removes all filter.
    ///// </summary>
    //private void RemoveFilter()
    //{
    //  this.FachFilter = null;
    //  this.JahrgangsstufeFilter = null;
    //  this.ModulFilter = null;
    //}

    ///// <summary>
    ///// The filter predicate that filters the person table view only showing schüler
    ///// </summary>
    ///// <param name="de">The <see cref="PersonViewModel"/> that should be filtered</param>
    ///// <returns>True if the given object should remain in the list.</returns>
    //private bool FilterModules(object de)
    //{
    //  var modulViewModel = de as ModulViewModel;
    //  if (modulViewModel == null)
    //  {
    //    return false;
    //  }

    //  if (this.FachFilter != null && this.JahrgangsstufeFilter != null)
    //  {
    //    if (modulViewModel.ModulFach == this.FachFilter
    //      && modulViewModel.ModulJahrgang == this.JahrgangsstufeFilter)
    //    {
    //      return true;
    //    }

    //    return false;
    //  }

    //  if (this.FachFilter != null)
    //  {
    //    if (modulViewModel.ModulFach == this.FachFilter)
    //    {
    //      return true;
    //    }

    //    return false;
    //  }

    //  if (this.JahrgangsstufeFilter != null)
    //  {
    //    if (modulViewModel.ModulJahrgang == this.JahrgangsstufeFilter)
    //    {
    //      return true;
    //    }

    //    return false;
    //  }

    //  return true;
    //}
  }
}
