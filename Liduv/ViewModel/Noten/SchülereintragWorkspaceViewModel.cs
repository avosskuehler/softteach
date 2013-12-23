namespace Liduv.ViewModel.Noten
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;

  using Helper;

  using Liduv.UndoRedo;
  using Liduv.View.Noten;

  using Personen;
  using Setting;

  /// <summary>
  /// ViewModel for managing Schülereintrag
  /// </summary>
  public class SchülereintragWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Schülerliste currently selected
    /// </summary>
    private SchülerlisteViewModel currentSchülerliste;

    /// <summary>
    /// The Schülereintrag currently selected
    /// </summary>
    private SchülereintragViewModel currentSchülereintrag;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchülereintragWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public SchülereintragWorkspaceViewModel()
    {
      this.CurrentSchülerliste = App.MainViewModel.Schülerlisten.Count > 0 ? App.MainViewModel.Schülerlisten[0] : null;
      if (this.CurrentSchülerliste != null)
      {
        this.CurrentSchülereintrag = this.currentSchülerliste.CurrentSchülereintrag;
      }

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Schülereinträge.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentSchülereintrag))
        {
          this.CurrentSchülereintrag = null;
        }
      };

      this.AddHausaufgabenCommand = new DelegateCommand(this.AddHausaufgaben);
      this.PrintNotenlisteCommand = new DelegateCommand(this.PrintNotenliste);
      this.PrintNotenlisteDetailCommand = new DelegateCommand(this.PrintNotenlisteDetail);
    }

    /// <summary>
    /// Holt den Befehl, um fehlende Hausaufgaben einzutragen.
    /// </summary>
    public DelegateCommand AddHausaufgabenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um die Notenliste der aktuellen Schülerliste auszudrucken
    /// </summary>
    public DelegateCommand PrintNotenlisteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um die Notenliste der aktuellen Schülerliste auszudrucken
    /// </summary>
    public DelegateCommand PrintNotenlisteDetailCommand { get; private set; }

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
    /// Holt oder setzt die Schülerliste currently selected in this workspace
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
      }
    }

    /// <summary>
    /// Ruft Dialoge auf, mit denen nicht gemachte Hausaufgaben eingetragen werden.
    /// </summary>
    private void AddHausaufgaben()
    {
      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgaben hinzugefügt."), false))
      {
        var addDlg = new AddHausaufgabeDialog { Datum = DateTime.Now };
        if (addDlg.ShowDialog().GetValueOrDefault(false))
        {
          Selection.Instance.HausaufgabeDatum = addDlg.Datum;
          Selection.Instance.HausaufgabeBezeichnung = addDlg.Bezeichnung;

          // Reset currently selected hausaufgaben
          foreach (var schülereintragViewModel in this.currentSchülerliste.Schülereinträge)
          {
            schülereintragViewModel.CurrentHausaufgabe = null;
          }

          var dlg = new HausaufgabenDialog { Schülerliste = this.currentSchülerliste };
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
        DataContext = this.CurrentSchülerliste,
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
      var title = "Noten" + this.CurrentSchülerliste.SchülerlisteKlasse.KlasseBezeichnung + this.CurrentSchülerliste.SchülerlisteFach.FachBezeichnung;
      pd.PrintVisual(fixedPage, title);
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
        DataContext = this.CurrentSchülerliste,
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
      var title = "Noten" + this.CurrentSchülerliste.SchülerlisteKlasse.KlasseBezeichnung + this.CurrentSchülerliste.SchülerlisteFach.FachBezeichnung;
      pd.PrintVisual(fixedPage, title);
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

    //  if (App.MainViewModel.Schülereinträge.Any(o => o.SchülereintragHalbjahrtyp == dlg.Halbjahrtyp
    //                                               && o.SchülereintragJahrtyp == dlg.Jahrtyp
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
    //  schülerliste.Jahrtyp = dlg.Jahrtyp.Model;
    //  schülerliste.Halbjahrtyp = dlg.Halbjahrtyp.Model;
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
