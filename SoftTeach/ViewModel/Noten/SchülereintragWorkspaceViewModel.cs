namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Documents;

  using Helper;

  using MahApps.Metro.Controls.Dialogs;

  using Personen;
  using Setting;

  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;

  /// <summary>
  /// ViewModel for managing Schülereintrag
  /// </summary>
  public class SchülereintragWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Lerngruppe currently selected
    /// </summary>
    private LerngruppeViewModel currentLerngruppe;

    /// <summary>
    /// The Schülereintrag currently selected
    /// </summary>
    private SchülereintragViewModel currentSchülereintrag;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchülereintragWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SchülereintragWorkspaceViewModel()
    {

      this.LerngruppenViewSource = new CollectionViewSource() { Source = App.MainViewModel.Lerngruppen };
      using (this.LerngruppenViewSource.DeferRefresh())
      {
        this.LerngruppenViewSource.Filter += this.LerngruppenViewSource_Filter;
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("LerngruppeSchuljahr", ListSortDirection.Ascending));
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("LerngruppeFach", ListSortDirection.Ascending));
      }

      this.AddHausaufgabenCommand = new DelegateCommand(this.AddHausaufgaben);
      this.AddSonstigeNotenCommand = new DelegateCommand(this.AddSonstigeNoten);
      this.PrintNotenlisteCommand = new DelegateCommand(this.PrintNotenliste);
      this.AddZeugnisnotenCommand = new DelegateCommand(this.AddZeugnisnoten);

      // Erste Lerngruppe laden      
      var enumerator = this.LerngruppenView.GetEnumerator();
      enumerator.MoveNext(); // sets it to the first element

      this.CurrentLerngruppe = (LerngruppeViewModel)enumerator.Current;

    }

    /// <summary>
    /// Holt den Befehl, um fehlende Hausaufgaben einzutragen.
    /// </summary>
    public DelegateCommand AddHausaufgabenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um sonstige Noten anzulegen.
    /// </summary>
    public DelegateCommand AddSonstigeNotenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um die Notenliste der aktuellen Lerngruppe auszudrucken
    /// </summary>
    public DelegateCommand PrintNotenlisteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um Zeugnisnoten zu machen
    /// </summary>
    public DelegateCommand AddZeugnisnotenCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die LerngruppenViewSource
    /// </summary>
    public CollectionViewSource LerngruppenViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Lerngruppen
    /// </summary>
    public ICollectionView LerngruppenView => this.LerngruppenViewSource.View;

    /// <summary>
    /// Holt oder setzt die Schülereintrag currently selected in this workspace
    /// </summary>
    public SchülereintragViewModel CurrentSchülereintrag
    {
      get
      {
        return this.currentSchülereintrag;
      }

      set
      {
        this.currentSchülereintrag = value;
        Selection.Instance.Schülereintrag = value;
        this.RaisePropertyChanged("CurrentSchülereintrag");
      }
    }

    /// <summary>
    /// Holt oder setzt die Lerngruppe currently selected in this workspace
    /// </summary>
    public LerngruppeViewModel CurrentLerngruppe
    {
      get
      {
        return this.currentLerngruppe;
      }

      set
      {
        this.currentLerngruppe = value;
        if (this.currentLerngruppe != null)
        {
          this.currentLerngruppe.NotenDatum = DateTime.Now;
        }

        Selection.Instance.Lerngruppe = value;
        this.RaisePropertyChanged("CurrentLerngruppe");
      }
    }

    /// <summary>
    /// Ruft Dialoge auf, mit denen nicht gemachte Hausaufgaben eingetragen werden.
    /// </summary>
    private async void AddHausaufgaben()
    {
      Selection.Instance.Lerngruppe = this.CurrentLerngruppe;

      if (Configuration.Instance.IsMetroMode)
      {
        var addDlg = new MetroAddHausaufgabeDialog(DateTime.Now);
        var metroWindow = Configuration.Instance.MetroWindow;
        await metroWindow.ShowMetroDialogAsync(addDlg);
        return;
      }

      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgaben hinzugefügt."), false))
      {
        var addDlg = new AddHausaufgabeDialog { Datum = DateTime.Now };
        if (addDlg.ShowDialog().GetValueOrDefault(false))
        {
          Selection.Instance.HausaufgabeDatum = addDlg.Datum;
          Selection.Instance.HausaufgabeBezeichnung = addDlg.Bezeichnung;

          // Reset currently selected hausaufgaben
          foreach (var schülereintragViewModel in this.currentLerngruppe.Schülereinträge)
          {
            schülereintragViewModel.CurrentHausaufgabe = null;
          }

          var dlg = new HausaufgabenDialog { Lerngruppe = this.currentLerngruppe };
          undo = !dlg.ShowDialog().GetValueOrDefault(false);
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Hier wird der Dialog zur Hausaufgabenkontrolle aufgerufen
    /// </summary>
    private void AddSonstigeNoten()
    {
      Selection.Instance.Lerngruppe = this.CurrentLerngruppe;

      if (Configuration.Instance.IsMetroMode)
      {
        //var addDlg = new MetroAddHausaufgabeDialog(this.stunde.LerngruppenterminDatum);
        //var metroWindow = Configuration.Instance.MetroWindow;
        //await metroWindow.ShowMetroDialogAsync(addDlg);
        //return;
      }

      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Sonstige Noten hinzugefügt."), false))
      {
        var addDlg = new AddSonstigeNoteDialog() { Datum = DateTime.Now };
        if (addDlg.ShowDialog().GetValueOrDefault(false))
        {
          Selection.Instance.SonstigeNoteDatum = addDlg.Datum;
          Selection.Instance.SonstigeNoteBezeichnung = addDlg.Bezeichnung;
          Selection.Instance.SonstigeNoteNotentyp = addDlg.Notentyp;
          Selection.Instance.SonstigeNoteWichtung = addDlg.Wichtung;

          // Reset currently selected note
          foreach (var schülereintragViewModel in this.CurrentLerngruppe.Schülereinträge)
          {
            schülereintragViewModel.CurrentNote = null;
          }

          var dlg = new SonstigeNotenDialog() { Lerngruppe = this.CurrentLerngruppe };
          undo = !dlg.ShowDialog().GetValueOrDefault(false);
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Druckt die aktuelle Notenliste der Klasse aus
    /// </summary>
    private void PrintNotenliste()
    {
      // select printer and get printer settings
      var pd = new PrintDialog();
      if (pd.ShowDialog() != true)
      {
        return;
      }

      // create a document
      var document = new FixedDocument { Name = "NotenlisteAusdruck" };
      document.DocumentPaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

      // create a page
      var fixedPage = new FixedPage
      {
        Width = document.DocumentPaginator.PageSize.Width,
        Height = document.DocumentPaginator.PageSize.Height
      };

      // create the print output usercontrol
      var content = new NotenlistePrintView
      {
        DataContext = this.CurrentLerngruppe,
        Width = fixedPage.Width,
        Height = fixedPage.Height
      };

      fixedPage.Children.Add(content);

      // Update the layout of our FixedPage
      var size = document.DocumentPaginator.PageSize;
      fixedPage.Measure(size);
      fixedPage.Arrange(new Rect(new Point(), size));
      fixedPage.UpdateLayout();

      // print it out
      var title = "Noten" + this.CurrentLerngruppe.LerngruppeBezeichnung + this.CurrentLerngruppe.LerngruppeFach.FachBezeichnung;
      pd.PrintVisual(fixedPage, title);
    }

    /// <summary>
    /// Ruft einen Dialog auf, mit dem Zeugnisnoten erstellt werden können.
    /// </summary>
    private void AddZeugnisnoten()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("e Zeugnisnoten erstellt"), false))
      {
        var workspace = new NotenlistenWorkspaceViewModel(this.CurrentLerngruppe);
        var dlg = new NotenlistenDialog { DataContext = workspace };
        dlg.ShowDialog();
      }
    }

    /// <summary>
    /// Druckt die aktuelle Notenliste der Klasse aus
    /// </summary>
    private void PrintNotenlisteDetail()
    {
      // select printer and get printer settings
      var pd = new PrintDialog();
      if (pd.ShowDialog() != true)
      {
        return;
      }

      // create a document
      var document = new FixedDocument { Name = "NotenlisteAusdruck" };
      document.DocumentPaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

      // create a page
      var fixedPage = new FixedPage
      {
        Width = document.DocumentPaginator.PageSize.Width,
        Height = document.DocumentPaginator.PageSize.Height
      };

      // create the print output usercontrol
      var content = new NotenlisteDetailPrintView
      {
        DataContext = this.CurrentLerngruppe,
        Width = fixedPage.Width,
        Height = fixedPage.Height
      };

      fixedPage.Children.Add(content);

      // Update the layout of our FixedPage
      var size = document.DocumentPaginator.PageSize;
      fixedPage.Measure(size);
      fixedPage.Arrange(new Rect(new Point(), size));
      fixedPage.UpdateLayout();

      // print it out
      var title = "Noten" + this.CurrentLerngruppe.LerngruppeBezeichnung + this.CurrentLerngruppe.LerngruppeFach.FachBezeichnung;
      pd.PrintVisual(fixedPage, title);
    }

    /// <summary>
    /// Filtert die Lerngruppen, so dass nur zu benotende Lerngruppen erscheinen
    /// </summary>
    /// <param name="item">Das LerngruppeViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private void LerngruppenViewSource_Filter(object sender, FilterEventArgs e)
    {
      var lerngruppeViewModel = e.Item as LerngruppeViewModel;
      if (lerngruppeViewModel == null)
      {
        e.Accepted = false;
        return;
      }

      if (!lerngruppeViewModel.LerngruppeFach.FachMitNoten)
      {
        e.Accepted = false;
        return;
      }

      e.Accepted = true;
    }

    ///// <summary>
    ///// Handles addition a new Stundenentwurf to the workspace and model
    ///// </summary>
    //private void AddSchülereintrag()
    //{
    //  // Check for existing schülerliste
    //  var dlg = new AskForSchülereintragToAddDialog();
    //  if (!dlg.ShowDialog().GetValueOrDefault(false))
    //  {
    //    return;
    //  }

    //  if (App.MainViewModel.Schülereinträge.Any(o => o.SchülereintragHalbjahr == dlg.Halbjahr
    //                                               && o.SchülereintragSchuljahr == dlg.Schuljahr
    //                                               && o.SchülereintragKlasse == dlg.Klasse))
    //  {
    //    ExceptionMethods.ProcessMessage(
    //      "Schülereintrag bereits vorhanden",
    //      "Diese Schülereintrag ist bereits in " + "der Datenbank vorhanden und kann nicht doppelt angelegt werden.");
    //    return;
    //  }

    //  var schülerliste = new Schülereintrag>();
    //  App.UnitOfWork.Schülereintrag.Add(schülerliste);
    //  schülerliste.Klasse = dlg.Klasse.Model;
    //  schülerliste.Schuljahr = dlg.Schuljahr.Model;
    //  schülerliste.Halbjahr = dlg.Halbjahr.Model;
    //  if (dlg.Fach != null)
    //  {
    //    schülerliste.Fach = dlg.Fach.Model;
    //  }

    //  var vm = new SchülereintragViewModel(schülerliste);

    //  App.MainViewModel.Schülereinträge.Add(vm);
    //  this.CurrentSchülereintrag = vm;
    //}

    ///// <summary>
    ///// Handles deletion of the current Stundenentwurf
    ///// </summary>
    //private void DeleteCurrentSchülereintrag()
    //{
    //  App.UnitOfWork.RemoveSchülereintrag(this.CurrentSchülereintrag.Model);
    //  App.MainViewModel.Schülereinträge.RemoveTest(this.CurrentSchülereintrag);
    //  this.CurrentSchülereintrag = null;
    //}
  }
}
