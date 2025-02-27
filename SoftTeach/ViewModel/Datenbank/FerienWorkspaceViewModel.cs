﻿namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class FerienWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Ferien currently selected
    /// </summary>
    private FerienViewModel currentFerien;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="FerienWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public FerienWorkspaceViewModel()
    {
      this.AddFerienCommand = new DelegateCommand(this.AddFerien);
      this.DeleteFerienCommand = new DelegateCommand(this.DeleteCurrentFerien, () => this.CurrentFerien != null);
      
      this.CurrentFerien = App.MainViewModel.Ferien.Count > 0 ? App.MainViewModel.Ferien[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Ferien.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentFerien))
        {
          this.CurrentFerien = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Ferien
    /// </summary>
    public DelegateCommand AddFerienCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Ferien
    /// </summary>
    public DelegateCommand DeleteFerienCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die ferien currently selected in this workspace
    /// </summary>
    public FerienViewModel CurrentFerien
    {
      get
      {
        return this.currentFerien;
      }

      set
      {
        this.currentFerien = value;
        this.RaisePropertyChanged("CurrentFerien");
        this.DeleteFerienCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles addition a new Ferien to the workspace and model
    /// </summary>
    private void AddFerien()
    {
      var ferien = new Ferien
      {
        Schuljahr = Selection.Instance.Schuljahr.Model,
        Bezeichnung = "Sommerferien",
        ErsterFerientag = DateTime.Now,
        LetzterFerientag = DateTime.Now
      };

      var vm = new FerienViewModel(ferien);

      App.MainViewModel.Ferien.Add(vm);
      this.CurrentFerien = vm;
    }

    /// <summary>
    /// Handles deletion of the current Ferien
    /// </summary>
    private void DeleteCurrentFerien()
    {
      App.MainViewModel.Ferien.RemoveTest(this.CurrentFerien);
      this.CurrentFerien = null;
    }
  }
}
