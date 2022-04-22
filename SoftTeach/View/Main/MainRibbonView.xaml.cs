namespace SoftTeach.View.Main
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using SoftTeach.ExceptionHandling;

  using SoftTeach.Properties;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Datenbank;
  using SoftTeach.View.Noten;
  using SoftTeach.View.Personen;
  using SoftTeach.View.Sitzpläne;
  using SoftTeach.View.Stundenentwürfe;
  using SoftTeach.View.Stundenpläne;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Curricula;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// Interaction logic for MainRibbonView.xaml
  /// </summary>
  public partial class MainRibbonView
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MainRibbonView"/> Klasse. 
    /// </summary>
    public MainRibbonView()
    {
      this.DataContext = App.MainViewModel;
      this.InitializeComponent();
    }

    private void MainRibbonViewClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Selection.Instance.UpdateUserSettings();
      Settings.Default.Save();

      // Check if there is nothing to change
      if (((Stack<ChangeSet>)App.MainViewModel.UndoStack).Count == 0)
      {
        return;
      }

      var dlg = new AskForSavingChangesDialog();
      dlg.ShowDialog();
      if (dlg.Result == null)
      {
        e.Cancel = true;
      }
      else if (dlg.Result.Value)
      {
        App.UnitOfWork.SaveChanges();
      }
    }

    #region Ribbon

    #region QAT
    #endregion

    #region PersonenTab

    /// <summary>
    /// Event handler for the Person button in the ribbon section Personen, ribbon group Personen.
    /// Shows a workspace for Personen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void PersonButtonClick(object sender, RoutedEventArgs e)
    {
      var personenWorkspace = new PersonenWorkspace();
      personenWorkspace.ShowDialog();
    }

    #endregion // Personen

    #region DatabaseTab

    #region DatabaseGroup

    /// <summary>
    /// Submits the changes button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void SubmitChangesButtonClick(object sender, RoutedEventArgs e)
    {
      App.UnitOfWork.SaveChanges();
    }

    /// <summary>
    /// Rejects the changes button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void RejectChangesButtonClick(object sender, RoutedEventArgs e)
    {
      InformationDialog.Show(
        "Funktionalität fehlt",
        "Diese Funktion ist im Moment nicht implementiert." +
        "Bitte schließen sie das Programm ohne zu speichern, um den gleichen Effekt zu erreichen.",
        false);
    }

    /// <summary>
    /// Cleanups the database button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void CleanupDatabaseButtonClick(object sender, RoutedEventArgs e)
    {
      var dlg = new CleanupDatabaseDialog();
      dlg.ShowDialog();
    }

    #endregion //DatabaseGroup

    #region Curricula

    /// <summary>
    /// Event handler for the reihe button in the ribbon section database, ribbon group Curricula.
    /// Shows a workspace for the reihe.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void ReiheButtonClick(object sender, RoutedEventArgs e)
    {
      var reiheView = new ReiheWorkspaceView();
      reiheView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the sequenz button in the ribbon section database, ribbon group Curricula.
    /// Shows a workspace for the sequenz.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void SequenzButtonClick(object sender, RoutedEventArgs e)
    {
      var sequenzView = new SequenzWorkspaceView();
      sequenzView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the fächer button in the ribbon section database, ribbon group Curricula.
    /// Shows a workspace for the fächer.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void FächerButtonClick(object sender, RoutedEventArgs e)
    {
      var fachView = new FachWorkspaceView { DataContext = App.MainViewModel.FachWorkspace };
      fachView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the fachstunden button in the ribbon section database, ribbon group Curricula.
    /// Shows a workspace for the fachstunden.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void FachstundenButtonClick(object sender, RoutedEventArgs e)
    {
      var fachstundenView = new FachstundenanzahlWorkspaceView { DataContext = App.MainViewModel.FachstundenanzahlWorkspace };
      fachstundenView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the modul button in the ribbon section database, ribbon group Curricula.
    /// Shows a workspace for the modul.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void ModuleButtonClick(object sender, RoutedEventArgs e)
    {
      var modulView = new ModulWorkspaceView { DataContext = App.MainViewModel.ModulWorkspace };
      modulView.ShowDialog();
    }

    #endregion // Curricula

    #region Stundenentwürfe

     /// <summary>
    /// Event handler for the Dateiverweis button in the ribbon section database, ribbon group Stundenentwürfe.
    /// Shows a workspace for Dateiverweise.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void DateiverweisButtonClick(object sender, RoutedEventArgs e)
    {
      var dateiverweisView = new DateiverweisWorkspaceView();
      dateiverweisView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Dateitypen button in the ribbon section database, ribbon group Stundenentwürfe.
    /// Shows a workspace for Dateitypen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void DateitypenButtonClick(object sender, RoutedEventArgs e)
    {
      var dateitypenView = new DateitypWorkspaceView { DataContext = App.MainViewModel.DateitypWorkspace };
      dateitypenView.ShowDialog();
    }

    #endregion // Stundenentwürfe

    #region Termine

    /// <summary>
    /// Event handler for the Termin button in the ribbon section database, ribbon group Termine.
    /// Shows a workspace for Termine.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void TerminButtonClick(object sender, RoutedEventArgs e)
    {
      var terminView = new TerminWorkspaceView();
      terminView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Ferien button in the ribbon section database, ribbon group Termine.
    /// Shows a workspace for Ferien.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void FerienButtonClick(object sender, RoutedEventArgs e)
    {
      var ferienView = new FerienWorkspaceView { DataContext = App.MainViewModel.FerienWorkspace };
      ferienView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Schuljahre button in the ribbon section database, ribbon group Termine.
    /// Shows a workspace for Schuljahre.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void SchuljahreButtonClick(object sender, RoutedEventArgs e)
    {
      var schuljahrView = new SchuljahrWorkspaceView { DataContext = App.MainViewModel.SchuljahrWorkspace };
      schuljahrView.ShowDialog();
    }

    #endregion // Termine

    #region Stundenplan

    /// <summary>
    /// Event handler for the Stundenpläne button in the ribbon section database, ribbon group Stundenplan.
    /// Shows a workspace for Stundenpläne.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void StundenpläneButtonClick(object sender, RoutedEventArgs e)
    {
      var stundenpläneView = new Datenbank.StundenplanWorkspaceView();
      stundenpläneView.ShowDialog();
    }

    #endregion // Stundenplan

    #region Personen

    /// <summary>
    /// Event handler for the Lerngruppe button in the ribbon section database, ribbon group Personen.
    /// Shows a workspace for Lerngruppen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void LerngruppeButtonClick(object sender, RoutedEventArgs e)
    {
      var schülerlisteView = new LerngruppeWorkspace();
      schülerlisteView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Schülereintrag button in the ribbon section database, ribbon group Personen.
    /// Shows a workspace for Schülereinträge.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void SchülereintragButtonClick(object sender, RoutedEventArgs e)
    {
      var schülereintragView = new SchülereintragWorkspace();
      schülereintragView.ShowDialog();
    }

    #endregion // Personen

    #region Noten

    /// <summary>
    /// Event handler for the Noten button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for Noten (which are Schülereinträge).
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void NotenButtonClick(object sender, RoutedEventArgs e)
    {
      var schülereintragView = new SchülereintragWorkspace();
      schülereintragView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Bewertungsschemata button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for Bewertungsschemata.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void BewertungsschemataButtonClick(object sender, RoutedEventArgs e)
    {
      var bewertungsschemataView = new BewertungsschemaWorkspaceView { DataContext = App.MainViewModel.BewertungsschemaWorkspace };
      bewertungsschemataView.ShowDialog();
    }

    #endregion // Noten

    #region Sitzpläne

    /// <summary>
    /// Event handler for the Tendenzen button in the ribbon section main, ribbon group Noten.
    /// Shows a workspace for Tendenzen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void RäumeButtonClick(object sender, RoutedEventArgs e)
    {
      var raumView = new RaumWorkspace { DataContext = App.MainViewModel.RaumWorkspace };
      raumView.ShowDialog();
    }

    #endregion // Sitzpläne

    #endregion // DatabaseTab

    private void ArbeitAnlegenButtonClick(object sender, RoutedEventArgs e)
    {
      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Arbeiten erstellt/verändert"), false))
      {
        var arbeitenView = new ArbeitWorkspace();
        undo = !arbeitenView.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    private void SchultermineButtonClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.ReNotifySchuljahr();
      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Schultermine verändert"), false))
      {
        var schultermineView = new AddSchulterminDialog();
        undo = !schultermineView.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
      else
      {
        var dlg = new InformationDialog(
          "Jahrespläne aktualisieren?",
          "Wollen Sie in allen betreffenden Jahresplänen die geänderten/neuen/gelöschten Termin aktualisieren?",
          true);

        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          SchulterminWorkspaceViewModel.UpdateJahrespläne();
        }
      }
    }

    private void NeuerStundenplanButtonClick(object sender, RoutedEventArgs e)
    {
      App.MainViewModel.LoadRäume();
      var dlg = new AskForSchuljahr();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        bool undo;
        using (new UndoBatch(App.MainViewModel, string.Format("Stundenplan angelegt."), false))
        {
          var stundenplanView = new AddStundenplanDialog(dlg.Schuljahr, dlg.Halbjahr, dlg.GültigAb);
          undo = !stundenplanView.ShowDialog().GetValueOrDefault(false);
        }

        if (undo)
        {
          App.MainViewModel.ExecuteUndoCommand();
        }
      }
    }

    private void StundenplanZeigenButtonClick(object sender, RoutedEventArgs e)
    {
      App.MainViewModel.LoadRäume();
      var stundenplanView = new ShowStundenplanDialog();
      stundenplanView.ShowDialog();
    }

    private void StundenplanÄnderungButtonClick(object sender, RoutedEventArgs e)
    {
      App.MainViewModel.LoadRäume();
      var aktuellerStundenplan = App.MainViewModel.StundenplanWorkspace.GetAktuellenStundenplan();
      App.MainViewModel.StundenplanWorkspace.AddStundenplanÄnderung(aktuellerStundenplan);
    }

    #endregion // Ribbon

    private void CloseCommandHandler(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Handles the OnClick event of the MetroButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MetroButton_OnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      this.Hide();
    }

    private void SpezialButtonClick(object sender, RoutedEventArgs e)
    {
      //foreach (var schülerlisteViewModel in App.MainViewModel.Lerngruppen)
      //{
      //  // Wenn Sommer
      //  if (schülerlisteViewModel.LerngruppeHalbjahr.HalbjahrIndex == 2)
      //  {
      //    var winterListe =
      //      App.MainViewModel.Lerngruppen.First(
      //        o =>
      //        o.LerngruppeSchuljahr == schülerlisteViewModel.LerngruppeSchuljahr
      //        && o.LerngruppeFach == schülerlisteViewModel.LerngruppeFach
      //        && o.LerngruppeKlasse == schülerlisteViewModel.LerngruppeKlasse
      //        && o.LerngruppeHalbjahr.HalbjahrIndex == 1);

      //    foreach (var schülereintragViewModel in schülerlisteViewModel.Schülereinträge)
      //    {
      //      var schülereintraginWinterliste =
      //        winterListe.Schülereinträge.FirstOrDefault(
      //          o => o.SchülereintragPerson == schülereintragViewModel.SchülereintragPerson);
      //      if (schülereintraginWinterliste != null)
      //      {
      //        foreach (var noteViewModel in schülereintragViewModel.Noten)
      //        {
      //          schülereintraginWinterliste.AddNote(noteViewModel);
      //        }

      //        foreach (var hausaufgabeViewModel in schülereintragViewModel.Hausaufgaben)
      //        {
      //          schülereintraginWinterliste.AddHausaufgabe(hausaufgabeViewModel);
      //        }

      //        foreach (var notentendenzViewModel in schülereintragViewModel.Notentendenzen)
      //        {
      //          schülereintraginWinterliste.AddNotentendenz(notentendenzViewModel);
      //        }

      //        foreach (var ergebnisViewModel in schülereintragViewModel.Ergebnisse)
      //        {
      //          schülereintraginWinterliste.AddErgebnis(ergebnisViewModel);
      //        }

      //        schülereintragViewModel.Noten.Clear();
      //        schülereintragViewModel.Hausaufgaben.Clear();
      //        schülereintragViewModel.Notentendenzen.Clear();
      //        schülereintragViewModel.Ergebnisse.Clear();
      //      }
      //      else
      //      {
      //        winterListe.AddSchülereintrag(schülereintragViewModel);
      //      }
      //    }

      //    schülerlisteViewModel.Schülereinträge.Clear();
      //  }
      //}

      //var deleteListen =
      //  App.MainViewModel.Lerngruppen.Where(o => o.LerngruppeHalbjahr.HalbjahrIndex == 2).ToList();
      //foreach (var schülerlisteViewModel in deleteListen)
      //{
      //  App.MainViewModel.Lerngruppen.Remove(schülerlisteViewModel);
      //}
    }

    private void TabItemCurricula_Selected(object sender, RoutedEventArgs e)
    {
      App.MainViewModel.LoadCurricula();
    }
  }
}
