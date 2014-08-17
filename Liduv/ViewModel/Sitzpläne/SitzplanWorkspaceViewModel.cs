namespace Liduv.ViewModel.Sitzpläne
{
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Runtime.Remoting.Messaging;
  using System.Windows.Data;

  using Liduv.ExceptionHandling;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Personen;

  using MahApps.Metro.Controls;
  using MahApps.Metro.Controls.Dialogs;

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
    /// Die Schülerliste für den Filter
    /// </summary>
    private SchülerlisteViewModel schülerlisteFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SitzplanWorkspaceViewModel()
    {
      this.AddSitzplanCommand = new DelegateCommand(this.AddSitzplan);
      this.DeleteSitzplanCommand = new DelegateCommand(this.DeleteCurrentSitzplan, () => this.CurrentSitzplan != null);
      this.ResetSchülerlisteFilterCommand = new DelegateCommand(() => this.SchülerlisteFilter = null, () => this.SchülerlisteFilter != null);
      this.ResetRaumFilterCommand = new DelegateCommand(() => this.RaumFilter = null, () => this.RaumFilter != null);

      var numberOfSitzpläne = App.MainViewModel.Sitzpläne.Count;
      //this.CurrentSitzplan = numberOfSitzpläne > 0 ? App.MainViewModel.Sitzpläne[numberOfSitzpläne - 1] : null;
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
    public DelegateCommand ResetSchülerlisteFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Sitzplan
    /// </summary>
    public DelegateCommand ResetRaumFilterCommand { get; private set; }

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
        if (value != null && Selection.Instance.Raum != value)
        {
          Selection.Instance.Raum = value;
        }

        this.RaisePropertyChanged("RaumFilter");
        this.SitzpläneView.Refresh();
        this.ResetRaumFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt den Schülerlistefilter
    /// </summary>
    public SchülerlisteViewModel SchülerlisteFilter
    {
      get
      {
        return this.schülerlisteFilter;
      }

      set
      {
        this.schülerlisteFilter = value;
        if (value != null && Selection.Instance.Schülerliste != value)
        {
          Selection.Instance.Schülerliste = value;
        }

        this.RaisePropertyChanged("SchülerlisteFilter");
        this.SitzpläneView.Refresh();
        this.ResetSchülerlisteFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Aktualisiert den Sitzplanfilter wenn der Raum oder die Schülerliste geändert wird.
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
      else if (e.PropertyName == "Schülerliste")
      {
        this.SchülerlisteFilter = Selection.Instance.Schülerliste;
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

      if (this.schülerlisteFilter != null && this.raumFilter != null)
      {
        if (sitzplan.SitzplanSchülerliste == null)
        {
          return true;
        }

        return sitzplan.SitzplanSchülerliste.SchülerlisteÜberschrift == this.schülerlisteFilter.SchülerlisteÜberschrift
          && sitzplan.SitzplanRaumplan.RaumplanRaum.RaumBezeichnung == this.raumFilter.RaumBezeichnung;
      }

      return false;
    }

    /// <summary>
    /// Handles addition of a new Sitzplan to the workspace and model.
    /// </summary>
    private void AddSitzplan()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sitzplan neu angelegt"), false))
      {
        var sitzplan = new Sitzplan { Bezeichnung = "Neuer Sitzplan", GültigAb = DateTime.Now.Date };

        if (Selection.Instance.Raumplan != null)
        {
          sitzplan.Raumplan = Selection.Instance.Raumplan.Model;
        }
        else
        {
          Configuration.Instance.MetroWindow.ShowMessageAsync(
            "Bitte beachten",
            "Bitte klicken Sie erst den Raumplan an, für den der Sitzplan erstellt werden soll");
          return;
        }
        
        if (Selection.Instance.Schülerliste != null)
        {
          sitzplan.Schülerliste = Selection.Instance.Schülerliste.Model;
        }

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
