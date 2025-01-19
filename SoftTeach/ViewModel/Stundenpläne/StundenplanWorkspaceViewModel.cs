namespace SoftTeach.ViewModel.Stundenpläne
{
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Stundenpläne;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using System;
  using System.Linq;

  /// <summary>
  /// ViewModel for managing Stundenplan
  /// </summary>
  public class StundenplanWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Stundenplan currently selected
    /// </summary>
    private StundenplanViewModel currentStundenplan;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenplanWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public StundenplanWorkspaceViewModel()
    {
      this.AddStundenplanCommand = new DelegateCommand(this.AddStundenplan);
      this.DeleteStundenplanCommand = new DelegateCommand(this.DeleteCurrentStundenplan, () => this.CurrentStundenplan != null);
      this.EditStundenplanCommand = new DelegateCommand(this.EditStundenplan, () => this.CurrentStundenplan != null);

      this.CurrentStundenplan = App.MainViewModel.Stundenpläne.FirstOrDefault(o => o.StundenplanSchuljahr.SchuljahrJahr == Selection.Instance.Schuljahr.SchuljahrJahr);

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Stundenpläne.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentStundenplan))
        {
          this.CurrentStundenplan = null;
        }
      };

      App.MainViewModel.LoadRäume();
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Stundenplan
    /// </summary>
    public DelegateCommand AddStundenplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Stundenplan
    /// </summary>
    public DelegateCommand DeleteStundenplanCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur editing the current Stundenplan
    /// </summary>
    public DelegateCommand EditStundenplanCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die stundenplan currently selected in this workspace
    /// </summary>
    public StundenplanViewModel CurrentStundenplan
    {
      get
      {
        return this.currentStundenplan;
      }

      set
      {
        this.currentStundenplan = value;
        if (value != null)
        {
          UiServices.SetBusyState();
          this.currentStundenplan.ViewMode = StundenplanViewMode.Default;
          Selection.Instance.Schuljahr = this.currentStundenplan.StundenplanSchuljahr;
        }
        this.RaisePropertyChanged("CurrentStundenplan");
        this.DeleteStundenplanCommand.RaiseCanExecuteChanged();
        this.EditStundenplanCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Stundenplan to the workspace and model
    /// </summary>
    /// <param name="schuljahr"> The schuljahr. </param>
    /// <param name="halbschuljahr"> The halbschuljahr. </param>
    /// <param name="gültigAb"> The gültig Ab. </param>
    public void AddStundenplan(SchuljahrViewModel schuljahr, Halbjahr halbschuljahr, DateTime gültigAb)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Stundenplan ergänzt"), false))
      {

        var stundenplan = new Stundenplan
        {
          Schuljahr = schuljahr.Model,
          Halbjahr = halbschuljahr,
          GültigAb = gültigAb,
          Bezeichnung = string.Format("Stundenplan für {0} {1}", Configuration.Instance.Lehrer.Titel, Configuration.Instance.Lehrer.Nachname)
        };
        //App.UnitOfWork.Context.Stundenpläne.Add(stundenplan);
        var vm = new StundenplanViewModel(stundenplan);
        App.MainViewModel.Stundenpläne.Add(vm);
        this.CurrentStundenplan = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current Stundenplan
    /// </summary>
    public void DeleteCurrentStundenplan()
    {
      //var result = App.UnitOfWork.Context.Stundenpläne.Remove(this.CurrentStundenplan.Model);
      App.MainViewModel.Stundenpläne.RemoveTest(this.CurrentStundenplan);
      this.CurrentStundenplan = null;
    }

    /// <summary>
    /// Handles edit / new creation with new date of the current Stundenplan
    /// </summary>
    /// <param name="stundenplanViewModel"> The stundenplan View Model. </param>
    public void AddStundenplanÄnderung(StundenplanViewModel stundenplanViewModel)
    {
      // Get new gültigkeits date
      var dateDialog = new AskForGültigAb();
      if (!dateDialog.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Stundenplan {0} verändert.", stundenplanViewModel), false))
      {
        // Empty old changes
        stundenplanViewModel.ÄnderungsListe.Clear();

        // create a clone of the current stundenplanviewmodel
        var stundenplan = (StundenplanViewModel)stundenplanViewModel.Clone();
        stundenplan.StundenplanGültigAb = dateDialog.GültigAb;

        var dlg = new EditStundenplanDialog(stundenplan);
        if (!(undo = !dlg.ShowDialog().GetValueOrDefault(false)))
        {
          //App.UnitOfWork.Context.Stundenpläne.Add(stundenplan.Model);
          App.MainViewModel.Stundenpläne.Add(stundenplan);
          this.CurrentStundenplan = stundenplan;
        }

        stundenplanViewModel.ViewMode = StundenplanViewMode.Edit;
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Liefert den letzten Stundenplan, abhängig vom GültigAb Datum
    /// </summary>
    /// <returns>Der aktuelle Stundenplan.</returns>
    public static StundenplanViewModel GetAktuellenStundenplan()
    {
      var sortedByDate = App.MainViewModel.Stundenpläne.OrderBy(o => o.StundenplanGültigAb);
      return sortedByDate.Last();
    }

    /// <summary>
    /// Fügt einen en Stundenplan hinzu mit den aktuellen Parametern.
    /// </summary>
    private void AddStundenplan()
    {
      var dlg = new AskForSchuljahr();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        bool undo;
        using (new UndoBatch(App.MainViewModel, string.Format("Stundenplan angelegt."), false))
        {
          // en Stundenplan anlegen
          App.MainViewModel.StundenplanWorkspace.AddStundenplan(dlg.Schuljahr, dlg.Halbjahr, dlg.GültigAb);
          var stundenplanView = new EditStundenplanDialog(App.MainViewModel.StundenplanWorkspace.CurrentStundenplan);
          undo = !stundenplanView.ShowDialog().GetValueOrDefault(false);
        }

        if (undo)
        {
          App.MainViewModel.ExecuteUndoCommand();
        }
      }
    }

    /// <summary>
    /// Fügt eine Stundenplanänderung hinzu.
    /// </summary>
    private void EditStundenplan()
    {
      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Stundenplan {0} verändert.", this.CurrentStundenplan), false))
      {
        // Empty old changes
        this.CurrentStundenplan.ÄnderungsListe.Clear();

        var dlg = new EditStundenplanDialog(this.CurrentStundenplan);
        if (!(undo = !dlg.ShowDialog().GetValueOrDefault(false)))
        {
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }
  }
}
