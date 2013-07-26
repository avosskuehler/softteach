namespace Liduv.ViewModel.Datenbank
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows.Input;

  using Liduv.ExceptionHandling;
  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Datenbank;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class JahrtypWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Jahrtyp currently selected
    /// </summary>
    private JahrtypViewModel currentJahrtyp;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="JahrtypWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public JahrtypWorkspaceViewModel()
    {
      this.AddJahrtypCommand = new DelegateCommand(this.AddJahrtyp);
      this.DeleteJahrtypCommand = new DelegateCommand(this.DeleteCurrentJahrtyp, () => this.CurrentJahrtyp != null);
      this.AddWeeksCommand = new DelegateCommand(this.AddWeeks, () => this.CurrentJahrtyp != null);

      this.CurrentJahrtyp = App.MainViewModel.Jahrtypen.Count > 0 ? App.MainViewModel.Jahrtypen[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Jahrtypen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentJahrtyp))
        {
          this.CurrentJahrtyp = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahrtyp
    /// </summary>
    public DelegateCommand AddJahrtypCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Jahrtyp
    /// </summary>
    public DelegateCommand DeleteJahrtypCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding the weeks and days to this schuljahr
    /// </summary>
    public DelegateCommand AddWeeksCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die jahrtyp currently selected in this workspace
    /// </summary>
    public JahrtypViewModel CurrentJahrtyp
    {
      get
      {
        return this.currentJahrtyp;
      }

      set
      {
        this.currentJahrtyp = value;
        this.RaisePropertyChanged("CurrentJahrtyp");
        this.DeleteJahrtypCommand.RaiseCanExecuteChanged();
        this.AddWeeksCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Jahrtyp to the workspace and model
    /// </summary>
    private void AddJahrtyp()
    {
      var backup = Selection.Instance.Schuljahr;
      using (new UndoBatch(App.MainViewModel, string.Format("Neues Schuljahr hinzugefügt"), false))
      {
        var dlgVm = new AddJahrtypDialogViewModel();
        var dlg = new AddSchuljahrDialog { DataContext = dlgVm };
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          foreach (var ferienViewModel in dlgVm.Ferien)
          {
            App.MainViewModel.Ferien.Add(ferienViewModel);
          }

          this.AddSchulwochen(dlgVm.Jahrtyp);
        }
        else
        {
          App.MainViewModel.Jahrtypen.RemoveTest(dlgVm.Jahrtyp);
          Selection.Instance.Schuljahr = backup;
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current Jahrtyp
    /// </summary>
    private void DeleteCurrentJahrtyp()
    {
      // App.UnitOfWork.GetRepository<Jahrtyp>().RemoveTest(this.CurrentJahrtyp.Model);
      App.MainViewModel.Jahrtypen.RemoveTest(this.CurrentJahrtyp);
      this.CurrentJahrtyp = null;
    }

    private void AddWeeks()
    {
      if (this.CurrentJahrtyp != null)
      {
        this.AddSchulwochen(this.CurrentJahrtyp);
      }
    }

    private void AddSchulwochen(JahrtypViewModel jahrtypViewModel)
    {
      if (jahrtypViewModel.Schulwochen.Count > 0)
      {
        return;
      }

      var vorhergehendeSommerferien =
        App.MainViewModel.Ferien.First(
          o => o.FerienJahrtyp.JahrtypJahr == jahrtypViewModel.JahrtypJahr - 1 && o.FerienBezeichnung == "Sommerferien");

      var schulmontag = vorhergehendeSommerferien.FerienLetzterFerientag;
      while (schulmontag.DayOfWeek != DayOfWeek.Monday)
      {
        schulmontag = schulmontag.AddDays(1);
      }

      var schultage = new Dictionary<DateTime, SchultagViewModel>();
      for (int i = 1; i < 53; i++)
      {
        var schulwoche = new Schulwoche { Montagsdatum = schulmontag };
        var vm = new SchulwocheViewModel(schulwoche);

        for (int j = 0; j < 5; j++)
        {
          var schultag = new Schultag();
          schultag.Datum = schulmontag.AddDays(j);
          schultag.Termintyp = App.MainViewModel.Termintypen.First(o => o.TermintypBezeichnung == "Unterricht").Model;

          var viewModelTag = new SchultagViewModel(schultag);
          vm.Schultage.Add(viewModelTag);
          App.MainViewModel.Schultage.Add(viewModelTag);
        }

        schulmontag = schulmontag.AddDays(7);

        foreach (var schultagViewModel in vm.Schultage)
        {
          schultage.Add(schultagViewModel.SchultagDatum, schultagViewModel);
        }

        App.MainViewModel.Schulwochen.Add(vm);
        jahrtypViewModel.Schulwochen.Add(vm);
        jahrtypViewModel.CurrentSchulwoche = vm;
      }

      // Add Ferien
      foreach (var ferien in App.MainViewModel.Ferien.Where(
        schuljahr => schuljahr.FerienJahrtyp == jahrtypViewModel))
      {
        var ferientage = (ferien.FerienLetzterFerientag - ferien.FerienErsterFerientag).Days;
        for (int i = 0; i < ferientage; i++)
        {
          var ferientag = ferien.FerienErsterFerientag.AddDays(i);
          if (schultage.ContainsKey(ferientag))
          {
            schultage[ferientag].SchultagTermintyp = App.MainViewModel.Termintypen.First(o => o.TermintypBezeichnung == "Ferien");
          }
        }
      }
    }
  }
}
