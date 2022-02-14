namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class SchuljahrWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Schuljahr currently selected
    /// </summary>
    private SchuljahrViewModel currentSchuljahr;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchuljahrWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SchuljahrWorkspaceViewModel()
    {
      this.AddSchuljahrCommand = new DelegateCommand(AddSchuljahr);
      this.DeleteSchuljahrCommand = new DelegateCommand(this.DeleteCurrentSchuljahr, () => this.CurrentSchuljahr != null);
      //this.AddWeeksCommand = new DelegateCommand(this.AddWeeks, () => this.CurrentSchuljahr != null);

      this.CurrentSchuljahr = App.MainViewModel.Schuljahre.Count > 0 ? App.MainViewModel.Schuljahre[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Schuljahre.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentSchuljahr))
        {
          this.CurrentSchuljahr = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Schuljahr
    /// </summary>
    public DelegateCommand AddSchuljahrCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Schuljahr
    /// </summary>
    public DelegateCommand DeleteSchuljahrCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding the weeks and days to this schuljahr
    /// </summary>
    public DelegateCommand AddWeeksCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die schuljahr currently selected in this workspace
    /// </summary>
    public SchuljahrViewModel CurrentSchuljahr
    {
      get
      {
        return this.currentSchuljahr;
      }

      set
      {
        this.currentSchuljahr = value;
        this.RaisePropertyChanged("CurrentSchuljahr");
        this.DeleteSchuljahrCommand.RaiseCanExecuteChanged();
        this.AddWeeksCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Schuljahr to the workspace and model
    /// </summary>
    public static void AddSchuljahr()
    {
      var backup = Selection.Instance.Schuljahr;
      using (new UndoBatch(App.MainViewModel, string.Format("Neues Schuljahr hinzugefügt"), false))
      {
        var dlgVm = new AddSchuljahrDialogViewModel();
        var dlg = new AddSchuljahrDialog { DataContext = dlgVm };
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          foreach (var ferienViewModel in dlgVm.Ferien)
          {
            App.MainViewModel.Ferien.Add(ferienViewModel);
          }
        }
        else
        {
          App.MainViewModel.Schuljahre.RemoveTest(dlgVm.Schuljahr);
          Selection.Instance.Schuljahr = backup;
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current Schuljahr
    /// </summary>
    private void DeleteCurrentSchuljahr()
    {
      // App.UnitOfWork.GetRepository<Schuljahr>().RemoveTest(this.CurrentSchuljahr.Model);
      App.MainViewModel.Schuljahre.RemoveTest(this.CurrentSchuljahr);
      this.CurrentSchuljahr = null;
    }
  }
}
