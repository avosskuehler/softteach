
namespace SoftTeach.View.Main
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using System.Windows.Documents;
  using System.Windows.Media;
  using SoftTeach.ExceptionHandling;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Properties;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Datenbank;
  using SoftTeach.View.Noten;
  using SoftTeach.View.Personen;
  using SoftTeach.View.Sitzpläne;
  using SoftTeach.View.Stundenpläne;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Curricula;
  using SoftTeach.ViewModel.Helper;
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
      var wochenplan = App.MainViewModel.WochenplanWorkspace;
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
    /// Event handler for the curriculum button in the ribbon section database, ribbon group Curricula.
    /// Shows a workspace for the curriculum.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void CurriculumButtonClick(object sender, RoutedEventArgs e)
    {
      var curriculumView = new CurriculumDBView();
      curriculumView.ShowDialog();
    }

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

    #region Planung

    /// <summary>
    /// Event handler for the Jahrespläne button in the ribbon section database, ribbon group Plänung.
    /// Shows a workspace for Jahrespläne.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void JahrespläneButtonClick(object sender, RoutedEventArgs e)
    {
      var jahresplanView = new JahresplanWorkspaceView();
      jahresplanView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Halbjahresplan button in the ribbon section database, ribbon group Plänung.
    /// Shows a workspace for Halbjahresplan.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void HalbjahresplanButtonClick(object sender, RoutedEventArgs e)
    {
      var halbjahresplanView = new HalbjahresplanWorkspaceView();
      halbjahresplanView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Monatsplan button in the ribbon section database, ribbon group Plänung.
    /// Shows a workspace for Monatsplan.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void MonatsplanButtonClick(object sender, RoutedEventArgs e)
    {
      var monatsplanView = new MonatsplanWorkspaceView();
      monatsplanView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Tagesplan button in the ribbon section database, ribbon group Plänung.
    /// Shows a workspace for Tagesplan.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void TagesplanButtonClick(object sender, RoutedEventArgs e)
    {
      var tagesplanView = new TagesplanWorkspaceView();
      tagesplanView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Schulwoche button in the ribbon section database, ribbon group Plänung.
    /// Shows a workspace for Schulwoche.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void SchulwocheButtonClick(object sender, RoutedEventArgs e)
    {
      var schulwocheView = new SchulwocheWorkspaceView();
      schulwocheView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Schultag button in the ribbon section database, ribbon group Plänung.
    /// Shows a workspace for Schultag.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void SchultagButtonClick(object sender, RoutedEventArgs e)
    {
      var schultagView = new SchultagWorkspaceView();
      schultagView.ShowDialog();
    }

    #endregion // Planung

    #region Stundenentwürfe

    /// <summary>
    /// Event handler for the Stundenentwuerfe button in the ribbon section database, ribbon group Stundenentwürfe.
    /// Shows a workspace for Stundenentwuerfe.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void StundenentwuerfeButtonClick(object sender, RoutedEventArgs e)
    {
      var stundenentwurfView = new StundenentwurfWorkspaceView();
      stundenentwurfView.DataContext = App.MainViewModel.StundenentwurfWorkspace;
      stundenentwurfView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Phasen button in the ribbon section database, ribbon group Stundenentwürfe.
    /// Shows a workspace for Phasen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void PhasenButtonClick(object sender, RoutedEventArgs e)
    {
      var phaseView = new PhaseWorkspaceView();
      phaseView.ShowDialog();
    }

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
    /// Event handler for the Medien button in the ribbon section database, ribbon group Stundenentwürfe.
    /// Shows a workspace for Medien.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void MedienButtonClick(object sender, RoutedEventArgs e)
    {
      var medienView = new MediumWorkspaceView { DataContext = App.MainViewModel.MediumWorkspace };
      medienView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Sozialformen button in the ribbon section database, ribbon group Stundenentwürfe.
    /// Shows a workspace for Sozialformen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void SozialformenButtonClick(object sender, RoutedEventArgs e)
    {
      var sozialformenView = new SozialformWorkspaceView { DataContext = App.MainViewModel.SozialformWorkspace };
      sozialformenView.ShowDialog();
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
    /// Event handler for the BetroffeneKlassen button in the ribbon section database, ribbon group Termine.
    /// Shows a workspace for BetroffeneKlassen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void BetroffeneKlassenButtonClick(object sender, RoutedEventArgs e)
    {
      var betroffeneKlasseView = new BetroffeneKlassenDBView();
      betroffeneKlasseView.ShowDialog();
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
    /// Event handler for the Termintypen button in the ribbon section database, ribbon group Termine.
    /// Shows a workspace for Termintypen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void TermintypenButtonClick(object sender, RoutedEventArgs e)
    {
      var termintypenView = new TermintypenDBView { DataContext = App.MainViewModel.TermintypWorkspace };
      termintypenView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Schuljahre button in the ribbon section database, ribbon group Termine.
    /// Shows a workspace for Schuljahre.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void SchuljahreButtonClick(object sender, RoutedEventArgs e)
    {
      var jahrtypView = new JahrtypWorkspaceView { DataContext = App.MainViewModel.JahrtypWorkspace };
      jahrtypView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Halbjahrtyp button in the ribbon section database, ribbon group Termine.
    /// Shows a workspace for Halbjahrtypen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void HalbjahrtypButtonClick(object sender, RoutedEventArgs e)
    {
      var halbjahrtypView = new HalbjahrtypWorkspaceView();
      halbjahrtypView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Monatstyp button in the ribbon section database, ribbon group Termine.
    /// Shows a workspace for Monatstypen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void MonatstypButtonClick(object sender, RoutedEventArgs e)
    {
      var monatstypView = new MonatstypWorkspaceView();
      monatstypView.ShowDialog();
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

    /// <summary>
    /// Event handler for the Stundenplaneintrag button in the ribbon section database, ribbon group Stundenplan.
    /// Shows a workspace for Stundenplaneintrag.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void StundenplaneintragButtonClick(object sender, RoutedEventArgs e)
    {
      var stundenplaneintragView = new StundenplaneintragWorkspaceView();
      stundenplaneintragView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Unterrichtsstunden button in the ribbon section database, ribbon group Stundenplan.
    /// Shows a workspace for Unterrichtsstunden.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void UnterrichtsstundenButtonClick(object sender, RoutedEventArgs e)
    {
      var unterrichtsstundenView = new UnterrichtsstundenDBView
      {
        DataContext = App.MainViewModel.UnterrichtsstundeWorkspace
      };
      unterrichtsstundenView.ShowDialog();
    }

    #endregion // Stundenplan

    #region Personen

    /// <summary>
    /// Event handler for the Person button in the ribbon section database, ribbon group Personen.
    /// Shows a database workspace for Personen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void PersonDBButtonClick(object sender, RoutedEventArgs e)
    {
      var personenView = new PersonenDBView();
      personenView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Schülerliste button in the ribbon section database, ribbon group Personen.
    /// Shows a workspace for Schülerlisten.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void SchülerlisteButtonClick(object sender, RoutedEventArgs e)
    {
      var schülerlisteView = new SchülerlisteWorkspace();
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

    /// <summary>
    /// Event handler for the Jahrgangsstufen button in the ribbon section database, ribbon group Personen.
    /// Shows a workspace for Jahrgangsstufen, Klassenstufen and Klassen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void JahrgangsstufenButtonClick(object sender, RoutedEventArgs e)
    {
      var jahrgangsstufenView = new JahrgangsstufeWorkspaceView { DataContext = App.MainViewModel.JahrgangsstufeWorkspace };
      jahrgangsstufenView.ShowDialog();
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
    /// Event handler for the Notentendenzen button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for Notentendenzen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void NotentendenzenButtonClick(object sender, RoutedEventArgs e)
    {

    }

    /// <summary>
    /// Event handler for the Hausaufgaben button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for Hausaufgaben.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void HausaufgabenButtonClick(object sender, RoutedEventArgs e)
    {
      var hausaufgabenView = new HausaufgabenDBView();
      hausaufgabenView.ShowDialog();
    }

    private void ArbeitenButtonClick(object sender, RoutedEventArgs e)
    {

    }

    private void AufgabenButtonClick(object sender, RoutedEventArgs e)
    {

    }

    private void ErgebnisseButtonClick(object sender, RoutedEventArgs e)
    {

    }

    /// <summary>
    /// Event handler for the NotenWichtungen button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for NotenWichtungen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void NotenWichtungenButtonClick(object sender, RoutedEventArgs e)
    {
      var notenWichtungView = new NotenWichtungenDBView { DataContext = App.MainViewModel.NotenWichtungWorkspace };
      notenWichtungView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Tendenztypen button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for Tendenztypen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void TendenztypenButtonClick(object sender, RoutedEventArgs e)
    {
      var tendenztypView = new TendenztypenDBView { DataContext = App.MainViewModel.TendenztypWorkspace };
      tendenztypView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Tendenzen button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for Tendenzen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void TendenzenButtonClick(object sender, RoutedEventArgs e)
    {
      var tendenzenView = new TendenzenDBView { DataContext = App.MainViewModel.TendenzWorkspace };
      tendenzenView.ShowDialog();
    }

    /// <summary>
    /// Event handler for the Zensuren button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for Zensuren.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void ZensurenButtonClick(object sender, RoutedEventArgs e)
    {
      var zensurenView = new ZensurenDBView { DataContext = App.MainViewModel.ZensurWorkspace };
      zensurenView.ShowDialog();
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
    /// Event handler for the Tendenzen button in the ribbon section database, ribbon group Noten.
    /// Shows a workspace for Tendenzen.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="RoutedEventArgs"/> with the event data.</param>
    private void RäumeDBButtonClick(object sender, RoutedEventArgs e)
    {
      var raumView = new RäumeDBView { DataContext = App.MainViewModel.RaumWorkspace };
      raumView.ShowDialog();
    }

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
      var dlg = new AskForSchuljahr();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        bool undo;
        using (new UndoBatch(App.MainViewModel, string.Format("Stundenplan angelegt."), false))
        {
          var stundenplanView = new AddStundenplanDialog(dlg.Jahrtyp, dlg.Halbjahrtyp, dlg.GültigAb);
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
      var stundenplanView = new ShowStundenplanDialog();
      stundenplanView.ShowDialog();
    }

    private void StundenplanÄnderungButtonClick(object sender, RoutedEventArgs e)
    {
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
      //foreach (var schülerlisteViewModel in App.MainViewModel.Schülerlisten)
      //{
      //  // Wenn Sommer
      //  if (schülerlisteViewModel.SchülerlisteHalbjahrtyp.HalbjahrtypIndex == 2)
      //  {
      //    var winterListe =
      //      App.MainViewModel.Schülerlisten.First(
      //        o =>
      //        o.SchülerlisteJahrtyp == schülerlisteViewModel.SchülerlisteJahrtyp
      //        && o.SchülerlisteFach == schülerlisteViewModel.SchülerlisteFach
      //        && o.SchülerlisteKlasse == schülerlisteViewModel.SchülerlisteKlasse
      //        && o.SchülerlisteHalbjahrtyp.HalbjahrtypIndex == 1);

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
      //  App.MainViewModel.Schülerlisten.Where(o => o.SchülerlisteHalbjahrtyp.HalbjahrtypIndex == 2).ToList();
      //foreach (var schülerlisteViewModel in deleteListen)
      //{
      //  App.MainViewModel.Schülerlisten.Remove(schülerlisteViewModel);
      //}
    }

    private void TabItemCurricula_Selected(object sender, RoutedEventArgs e)
    {
      if (!App.MainViewModel.Curricula.Any())
      {
        foreach (var curriculum in App.UnitOfWork.Context.Curricula)
        {
          App.MainViewModel.Curricula.Add(new CurriculumViewModel(curriculum));
        }
      }
    }
  }
}
