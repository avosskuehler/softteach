namespace SoftTeach.ViewModel.Sitzpläne
{
  using System;
  using System.ComponentModel;
  using System.Windows.Data;
  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.View.Sitzpläne;

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
    /// Das Raum für den Filter
    /// </summary>
    private RaumViewModel raumFilter;

    /// <summary>
    /// Der Raumplan für den Filter
    /// </summary>
    private RaumplanViewModel raumplanFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SitzplanWorkspaceViewModel()
    {
      this.AddSitzplanCommand = new DelegateCommand(this.AddSitzplan);
      this.DeleteSitzplanCommand = new DelegateCommand(this.DeleteCurrentSitzplan, () => this.CurrentSitzplan != null);
      this.SitzplanBearbeitenCommand = new DelegateCommand(this.SitzplanBearbeiten, () => this.CurrentSitzplan != null);

      App.MainViewModel.LoadSitzpläne();
      var numberOfSitzpläne = App.MainViewModel.Sitzpläne.Count;
      //this.CurrentSitzplan = numberOfSitzpläne > 0 ? App.MainViewModel.Sitzpläne[numberOfSitzpläne - 1] : null;
      this.SitzpläneViewSource = new CollectionViewSource() { Source = App.MainViewModel.Sitzpläne };
      using (this.SitzpläneView.DeferRefresh())
      {
        this.SitzpläneView.Filter = this.CustomFilter;
        this.SitzpläneView.SortDescriptions.Add(new SortDescription("SitzplanLerngruppe", ListSortDirection.Ascending));
        this.SitzpläneView.SortDescriptions.Add(new SortDescription("SitzplanRaumplan", ListSortDirection.Ascending));
      }

      //// Re-act to any changes from outside this ViewModel
      //App.MainViewModel.Sitzpläne.CollectionChanged += (sender, e) =>
      //{
      //  if (e.OldItems != null && e.OldItems.Contains(this.CurrentSitzplan))
      //  {
      //    this.CurrentSitzplan = null;
      //  }
      //};

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
    /// Holt den Befehl den Sitzplan zu bearbeiten
    /// </summary>
    public DelegateCommand SitzplanBearbeitenCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die ViewSource der Sitzpläne
    /// </summary>
    public CollectionViewSource SitzpläneViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Sitzpläne
    /// </summary>
    public ICollectionView SitzpläneView => this.SitzpläneViewSource.View;

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
        //this.SitzpläneView.Refresh();
        this.RaisePropertyChanged("CurrentSitzplan");
        this.DeleteSitzplanCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt den Raumfilter
    /// </summary>
    public RaumViewModel RaumFilter
    {
      get
      {
        return this.raumFilter;
      }

      set
      {
        this.raumFilter = value;
        //if (value != null && Selection.Instance.Raum != value)
        //{
        //  Selection.Instance.Raum = value;
        //}

        this.RaisePropertyChanged("RaumFilter");
        this.SitzpläneView.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt den Raumfilter
    /// </summary>
    public RaumplanViewModel RaumplanFilter
    {
      get
      {
        return this.raumplanFilter;
      }

      set
      {
        this.raumplanFilter = value;
        //if (value != null && Selection.Instance.Raum != value)
        //{
        //  Selection.Instance.Raum = value;
        //}

        this.RaisePropertyChanged("RaumplanFilter");
        this.SitzpläneView.Refresh();
      }
    }

    /// <summary>
    /// Aktualisiert den Sitzplanfilter wenn der Raum oder die Lerngruppe geändert wird.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">A <see cref="PropertyChangedEventArgs"/> with the event data.</param>
    private void SelectionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Raum")
      {
        this.RaumFilter = Selection.Instance.Raum;
        this.SitzpläneView.Refresh();
      }
      else
      if (e.PropertyName == "Lerngruppe")
      {
        this.SitzpläneView.Refresh();
      }
    }

    /// <summary>
    /// Filtert die Sitzplanliste nach Raum und Raumplan
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

      if (Selection.Instance.Lerngruppe != null && this.raumFilter != null)
      {
        if (sitzplan.SitzplanLerngruppe == null)
        {
          return true;
        }

        if (sitzplan.SitzplanRaumplan.RaumplanRaum == null)
        {
          return true;
        }

        return sitzplan.SitzplanLerngruppe.LerngruppeÜberschrift == Selection.Instance.Lerngruppe.LerngruppeÜberschrift
          && sitzplan.SitzplanRaumplan.RaumplanRaum.RaumBezeichnung == this.raumFilter.RaumBezeichnung;
      }

      return false;
    }

    /// <summary>
    /// Bearbeitet den ausgewählten Sitzplan
    /// </summary>
    private void SitzplanBearbeiten()
    {
      if (this.CurrentSitzplan == null)
      {
        return;
      }

      Selection.Instance.Sitzplan = this.CurrentSitzplan;
      Configuration.Instance.NavigationService.Navigate(new MetroSitzplanPage());
    }

    /// <summary>
    /// Handles addition of a new Sitzplan to the workspace and model.
    /// </summary>
    private void AddSitzplan()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sitzplan neu angelegt"), false))
      {
        try
        {
          App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
          var sitzplan = new SitzplanNeu { Bezeichnung = "Neuer Sitzplan", GültigAb = DateTime.Now.Date };

          if (this.raumplanFilter != null)
          {
            sitzplan.Raumplan = this.raumplanFilter.Model;
          }
          else
          {
            Configuration.Instance.MetroWindow.ShowMessageAsync(
              "Bitte beachten",
              "Bitte klicken Sie erst den Raumplan an, für den der Sitzplan erstellt werden soll");
            return;
          }

          if (Selection.Instance.Lerngruppe != null)
          {
            sitzplan.Lerngruppe = Selection.Instance.Lerngruppe.Model;
          }

          //App.UnitOfWork.Context.Sitzpläne.Add(sitzplan);
          var vm = new SitzplanViewModel(sitzplan);
          App.MainViewModel.Sitzpläne.Add(vm);
          this.CurrentSitzplan = vm;
        }
        catch (Exception)
        {
          throw;
        }
        finally
        {
          App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current Sitzplan
    /// </summary>
    private void DeleteCurrentSitzplan()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sitzplan {0} gelöscht.", this.CurrentSitzplan.SitzplanBezeichnung), false))
      {
        //App.UnitOfWork.Context.Sitzpläne.Remove(this.CurrentSitzplan.Model);
        App.MainViewModel.Sitzpläne.RemoveTest(this.CurrentSitzplan);
        this.CurrentSitzplan = null;
      }
    }
  }
}
