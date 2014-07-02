﻿namespace Liduv.ViewModel.Personen
{
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using System.Windows.Input;

  using Liduv.ExceptionHandling;
  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Personen;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel for managing Schülerliste
  /// </summary>
  public class SchülerlisteWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Schülerliste currently selected
    /// </summary>
    private SchülerlisteViewModel currentSchülerliste;

    /// <summary>
    /// Das Fach, dessen Schülerlisten nur dargestellt werden sollen.
    /// </summary>
    private FachViewModel fachFilter;

    /// <summary>
    /// Die Jahrgangsstufe, deren Schülerlisten nur dargestellt werden sollen.
    /// </summary>
    private JahrtypViewModel jahrtypFilter;

    /// <summary>
    /// Der Halbjahrtyp, deren Schülerlisten nur dargestellt werden sollen.
    /// </summary>
    private HalbjahrtypViewModel halbjahrtypFilter;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchülerlisteWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SchülerlisteWorkspaceViewModel()
    {
      this.AddSchülerlisteCommand = new DelegateCommand(this.AddSchülerliste);
      this.MoveSchülerlisteCommand = new DelegateCommand(this.MoveCurrentSchülerliste, () => this.CurrentSchülerliste != null);
      this.DeleteSchülerlisteCommand = new DelegateCommand(this.DeleteCurrentSchülerliste, () => this.CurrentSchülerliste != null);
      this.ResetJahrtypFilterCommand = new DelegateCommand(() => this.JahrtypFilter = null, () => this.JahrtypFilter != null);
      this.ResetFachFilterCommand = new DelegateCommand(() => this.FachFilter = null, () => this.FachFilter != null);
      this.CurrentSchülerliste = App.MainViewModel.Schülerlisten.Count > 0 ? App.MainViewModel.Schülerlisten[0] : null;

      this.SchülerlistenView = CollectionViewSource.GetDefaultView(App.MainViewModel.Schülerlisten);
      this.SchülerlistenView.Filter = this.CustomFilter;
      this.SchülerlistenView.SortDescriptions.Add(new SortDescription("SchülerlisteJahrtyp", ListSortDirection.Ascending));
      this.SchülerlistenView.SortDescriptions.Add(new SortDescription("SchülerlisteFach", ListSortDirection.Ascending));
      this.SchülerlistenView.SortDescriptions.Add(new SortDescription("SchülerlisteKlasse", ListSortDirection.Ascending));
      this.SchülerlistenView.SortDescriptions.Add(new SortDescription("SchülerlisteHalbjahrtyp.HalbjahrtypIndex", ListSortDirection.Ascending));
      this.SchülerlistenView.Refresh();

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Schülerlisten.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentSchülerliste))
        {
          this.CurrentSchülerliste = null;
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Schülerliste
    /// </summary>
    public DelegateCommand AddSchülerlisteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um die Schülerliste ins nächste Halbjahr zu übernehmen
    /// </summary>
    public DelegateCommand MoveSchülerlisteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Schülerliste
    /// </summary>
    public DelegateCommand DeleteSchülerlisteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetJahrtypFilterCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Jahresplan
    /// </summary>
    public DelegateCommand ResetFachFilterCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die gefilterten Schülerlisten
    /// </summary>
    public ICollectionView SchülerlistenView { get; set; }

    /// <summary>
    /// Holt oder setzt die stundenentwurf currently selected in this workspace
    /// </summary>
    public SchülerlisteViewModel CurrentSchülerliste
    {
      get
      {
        return this.currentSchülerliste;
      }

      set
      {
        this.currentSchülerliste = value;
        Selection.Instance.Schülerliste = value;
        this.RaisePropertyChanged("CurrentSchülerliste");
        this.DeleteSchülerlisteCommand.RaiseCanExecuteChanged();
        this.MoveSchülerlisteCommand.RaiseCanExecuteChanged();
        if (this.currentSchülerliste != null)
        {
          this.currentSchülerliste.UpdateSort();
        }
      }
    }

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
        this.SchülerlistenView.Refresh();
        this.ResetFachFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die jahrgangsstufe filter for the stundenentwurf list.
    /// </summary>
    public JahrtypViewModel JahrtypFilter
    {
      get
      {
        return this.jahrtypFilter;
      }

      set
      {
        this.jahrtypFilter = value;
        this.RaisePropertyChanged("JahrtypFilter");
        this.SchülerlistenView.Refresh();
        this.ResetJahrtypFilterCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt den halbjahr filter für die Schülerlisten
    /// </summary>
    public HalbjahrtypViewModel HalbjahrtypFilter
    {
      get
      {
        return this.halbjahrtypFilter;
      }

      set
      {
        this.halbjahrtypFilter = value;
        this.RaisePropertyChanged("HalbjahrtypFilter");
        this.SchülerlistenView.Refresh();
      }
    }


    /// <summary>
    /// Filtert die Terminliste nach Jahrtyp und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool CustomFilter(object item)
    {
      var schülerlisteViewModel = item as SchülerlisteViewModel;
      if (schülerlisteViewModel == null)
      {
        return false;
      }

      if (this.jahrtypFilter != null && this.fachFilter != null && this.halbjahrtypFilter != null)
      {
        return schülerlisteViewModel.SchülerlisteJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung
          && schülerlisteViewModel.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung == this.halbjahrtypFilter.HalbjahrtypBezeichnung
          && schülerlisteViewModel.SchülerlisteFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      if (this.jahrtypFilter != null && this.fachFilter != null)
      {
        return schülerlisteViewModel.SchülerlisteJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung
          && schülerlisteViewModel.SchülerlisteFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      if (this.jahrtypFilter != null && this.halbjahrtypFilter != null)
      {
        return schülerlisteViewModel.SchülerlisteJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung
          && schülerlisteViewModel.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung == this.halbjahrtypFilter.HalbjahrtypBezeichnung;
      }

      if (this.fachFilter != null && this.halbjahrtypFilter != null)
      {
        return schülerlisteViewModel.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung == this.halbjahrtypFilter.HalbjahrtypBezeichnung
          && schülerlisteViewModel.SchülerlisteFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      if (this.jahrtypFilter != null)
      {
        return schülerlisteViewModel.SchülerlisteJahrtyp.JahrtypBezeichnung == this.jahrtypFilter.JahrtypBezeichnung;
      }

      if (this.halbjahrtypFilter != null)
      {
        return schülerlisteViewModel.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung == this.halbjahrtypFilter.HalbjahrtypBezeichnung;
      }

      if (this.fachFilter != null)
      {
        return schülerlisteViewModel.SchülerlisteFach.FachBezeichnung == this.fachFilter.FachBezeichnung;
      }

      return true;
    }

    /// <summary>
    /// Handles addition a new Stundenentwurf to the workspace and model
    /// </summary>
    private void AddSchülerliste()
    {
      // Check for existing schülerliste
      var dlg = new AskForSchülerlisteToAddDialog();
      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      if (
        App.MainViewModel.Schülerlisten.Any(
          o =>
          o.SchülerlisteHalbjahrtyp.HalbjahrtypIndex == dlg.Halbjahrtyp.HalbjahrtypIndex
          && o.SchülerlisteJahrtyp.JahrtypJahr == dlg.Jahrtyp.JahrtypJahr
          && o.SchülerlisteKlasse.KlasseBezeichnung == dlg.Klasse.KlasseBezeichnung
          && o.SchülerlisteFach.FachBezeichnung == dlg.Fach.FachBezeichnung))
      {
        Log.ProcessMessage(
          "Schülerliste bereits vorhanden",
          "Diese Schülerliste ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      var schülerliste = new Schülerliste();
      schülerliste.Klasse = dlg.Klasse.Model;
      schülerliste.Jahrtyp = dlg.Jahrtyp.Model;
      schülerliste.Halbjahrtyp = dlg.Halbjahrtyp.Model;
      schülerliste.Fach = dlg.Fach.Model;
      schülerliste.NotenWichtung = dlg.NotenWichtung.Model;
      var vm = new SchülerlisteViewModel(schülerliste);
      using (new UndoBatch(App.MainViewModel, string.Format("Neue Schülerliste {0} angelegt.", vm), false))
      {
        App.MainViewModel.Schülerlisten.Add(vm);
        this.CurrentSchülerliste = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current Stundenentwurf
    /// </summary>
    private void MoveCurrentSchülerliste()
    {
      // Check for existing schülerliste
      var newHalbjahrtypIndex = this.CurrentSchülerliste.SchülerlisteHalbjahrtyp.HalbjahrtypIndex == 1 ? 2 : 1;
      var newHalbjahrtyp = App.MainViewModel.Halbjahrtypen.First(o => o.HalbjahrtypIndex == newHalbjahrtypIndex);
      var isNewJahr = this.CurrentSchülerliste.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung == "Sommer";
      var newJahrtypJahr = isNewJahr ? this.CurrentSchülerliste.SchülerlisteJahrtyp.JahrtypJahr + 1 : this.CurrentSchülerliste.SchülerlisteJahrtyp.JahrtypJahr;
      var newJahrtyp = App.MainViewModel.Jahrtypen.FirstOrDefault(o => o.JahrtypJahr == newJahrtypJahr);

      // Wenn nur das zweite Halbjahr ergänzt werden soll
      if (!isNewJahr)
      {
        this.CreateSchülerliste(newHalbjahrtyp, newJahrtyp);
        return;
      }

      // Schülerliste soll ins nächste Jahr übernommen werden
      if (newJahrtyp == null)
      {
        InformationDialog.Show(
          "Schuljahr fehlt",
          "Bitte legen Sie zuerst das neue Schuljahr an, bevor Sie die Schülerlisten erzeugen",
          false);
        return;
      }

      var oldKlasseString = this.CurrentSchülerliste.SchülerlisteKlasse.KlasseBezeichnung;
      var oldKlassenstufeString = this.CurrentSchülerliste.SchülerlisteKlasse.Model.Klassenstufe.Bezeichnung;
      int oldKlassenstufe;
      KlasseViewModel newKlasse;
      if (!int.TryParse(oldKlassenstufeString, out oldKlassenstufe))
      {
        InformationDialog.Show(
          "Klasse nicht möglich",
          "Schüler der Oberstufe können nicht ins nächste Schuljahr übernommen werden",
          false);
        return;
      }

      if (oldKlassenstufe == 10)
      {
        InformationDialog.Show(
          "Klasse nicht möglich",
          "Schüler der 10. Klassen können nicht ins nächste Schuljahr übernommen werden",
          false);
        return;
      }

      var newKlassenstufe = oldKlassenstufe + 1;
      var newKlasseBezeichnung = newKlassenstufe + oldKlasseString.StripRight(1);
      newKlasse = App.MainViewModel.Klassen.FirstOrDefault(o => o.KlasseBezeichnung == newKlasseBezeichnung);

      if (
        App.MainViewModel.Schülerlisten.Any(
          o =>
          o.SchülerlisteHalbjahrtyp.HalbjahrtypIndex == newHalbjahrtypIndex
          && o.SchülerlisteJahrtyp.JahrtypJahr == newJahrtypJahr
          && o.SchülerlisteKlasse.KlasseBezeichnung == newKlasse.KlasseBezeichnung
          && o.SchülerlisteFach.FachBezeichnung == this.CurrentSchülerliste.SchülerlisteFach.FachBezeichnung))
      {
        Log.ProcessMessage(
          "Schülerliste bereits vorhanden",
          "Diese Schülerliste ist bereits in der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Schülerliste kopiert."), false))
      {
        var newSchülerliste = (SchülerlisteViewModel)this.CurrentSchülerliste.Clone();
        newSchülerliste.SchülerlisteHalbjahrtyp = newHalbjahrtyp;
        newSchülerliste.SchülerlisteJahrtyp = newJahrtyp;
        newSchülerliste.SchülerlisteKlasse = newKlasse;
        this.CurrentSchülerliste = newSchülerliste;
      }
    }

    private void CreateSchülerliste(HalbjahrtypViewModel newHalbjahrtyp, JahrtypViewModel newJahrtyp)
    {
      if (
        App.MainViewModel.Schülerlisten.Any(
          o =>
          o.SchülerlisteHalbjahrtyp.HalbjahrtypIndex == newHalbjahrtyp.HalbjahrtypIndex
          && o.SchülerlisteJahrtyp.JahrtypJahr == newJahrtyp.JahrtypJahr
          && o.SchülerlisteKlasse.KlasseBezeichnung == this.CurrentSchülerliste.SchülerlisteKlasse.KlasseBezeichnung
          && o.SchülerlisteFach.FachBezeichnung == this.CurrentSchülerliste.SchülerlisteFach.FachBezeichnung))
      {
        Log.ProcessMessage(
          "Schülerliste bereits vorhanden",
          "Diese Schülerliste ist bereits in der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Schülerliste kopiert."), false))
      {
        var newSchülerliste = (SchülerlisteViewModel)this.CurrentSchülerliste.Clone();
        newSchülerliste.SchülerlisteHalbjahrtyp = newHalbjahrtyp;
        newSchülerliste.SchülerlisteJahrtyp = newJahrtyp;
        this.CurrentSchülerliste = newSchülerliste;
      }
    }

    /// <summary>
    /// Handles deletion of the current Stundenentwurf
    /// </summary>
    private void DeleteCurrentSchülerliste()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Schülerliste {0} gelöscht.", this.CurrentSchülerliste), false))
      {
        // Dazugehörige Arbeiten löschen
        var arbeiten =
          App.MainViewModel.Arbeiten.Where(
            o =>
            o.ArbeitJahrtyp == this.CurrentSchülerliste.SchülerlisteJahrtyp
            && o.ArbeitHalbjahrtyp == this.CurrentSchülerliste.SchülerlisteHalbjahrtyp
            && o.ArbeitKlasse == this.CurrentSchülerliste.SchülerlisteKlasse
            && o.ArbeitFach == this.CurrentSchülerliste.SchülerlisteFach);
        var list = arbeiten.ToList();
        foreach (var arbeitViewModel in list)
        {
          bool success = App.MainViewModel.Arbeiten.RemoveTest(arbeitViewModel);
        }

        // Jetzt die Schülerliste löschen
        App.MainViewModel.Schülerlisten.RemoveTest(this.CurrentSchülerliste);
        this.CurrentSchülerliste = null;
      }
    }
  }
}
