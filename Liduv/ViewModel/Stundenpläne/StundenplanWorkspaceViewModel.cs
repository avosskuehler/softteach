namespace Liduv.ViewModel.Stundenpläne
{
  using System;
  using System.Linq;
  using System.Windows.Input;

  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Stundenpläne;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;

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
      this.EditStundenplanCommand = new DelegateCommand(this.AddStundenplanÄnderung, () => this.CurrentStundenplan != null);

      this.CurrentStundenplan = App.MainViewModel.Stundenpläne.Count > 0 ? App.MainViewModel.Stundenpläne[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Stundenpläne.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentStundenplan))
        {
          this.CurrentStundenplan = null;
        }
      };
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
          this.currentStundenplan.ViewMode = StundenplanViewMode.Edit;
        }

        this.RaisePropertyChanged("CurrentStundenplan");
        this.DeleteStundenplanCommand.RaiseCanExecuteChanged();
        this.EditStundenplanCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Stundenplan to the workspace and model
    /// </summary>
    /// <param name="jahrtyp"> The jahrtyp. </param>
    /// <param name="halbjahrtyp"> The halbjahrtyp. </param>
    /// <param name="gültigAb"> The gültig Ab. </param>
    public void AddStundenplan(JahrtypViewModel jahrtyp, HalbjahrtypViewModel halbjahrtyp, DateTime gültigAb)
    {
      var stundenplan = new Stundenplan();
      stundenplan.Jahrtyp = jahrtyp.Model;
      stundenplan.Halbjahrtyp = halbjahrtyp.Model;
      stundenplan.GültigAb = gültigAb;
      stundenplan.Bezeichnung = string.Format("Stundenplan für {0} {1}", Configuration.Instance.Lehrer.Titel, Configuration.Instance.Lehrer.Nachname);
      var vm = new StundenplanViewModel(stundenplan);
      App.MainViewModel.Stundenpläne.Add(vm);
      this.CurrentStundenplan = vm;
    }

    /// <summary>
    /// Handles deletion of the current Stundenplan
    /// </summary>
    public void DeleteCurrentStundenplan()
    {
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

        var dlg = new AddStundenplanÄnderungDialog(stundenplan);
        if (!(undo = !dlg.ShowDialog().GetValueOrDefault(false)))
        {
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
    public StundenplanViewModel GetAktuellenStundenplan()
    {
      var sortedByDate = App.MainViewModel.Stundenpläne.OrderBy(o => o.StundenplanGültigAb);
      return sortedByDate.Last();
    }

    /// <summary>
    /// Fügt einen neuen Stundenplan hinzu mit den aktuellen Parametern.
    /// </summary>
    private void AddStundenplan()
    {
      this.AddStundenplan(Selection.Instance.Schuljahr, Selection.Instance.Halbjahr, DateTime.Now);
    }

    /// <summary>
    /// Fügt eine Stundenplanänderung hinzu.
    /// </summary>
    private void AddStundenplanÄnderung()
    {
      this.AddStundenplanÄnderung(this.CurrentStundenplan);
    }
  }
}
