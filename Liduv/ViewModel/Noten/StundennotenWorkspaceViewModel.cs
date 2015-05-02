namespace Liduv.ViewModel.Noten
{
  using System;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;
  using System.Windows.Input;

  using Helper;

  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Noten;
  using Liduv.ViewModel.Personen;
  using Liduv.ViewModel.Termine;

  using MahApps.Metro.Controls.Dialogs;

  /// <summary>
  /// ViewModel for managing Stundennote
  /// </summary>
  public class StundennotenWorkspaceViewModel : ViewModelBase
  {
   /// <summary>
    /// Holt oder setzt die Schülerliste für die es Noten geben soll.
    /// </summary>
    private SchülerlisteViewModel schülerliste;

    /// <summary>
    /// Holt oder setzt die Stunde currently selected
    /// </summary>
    private StundeViewModel stunde;

    /// <summary>
    /// Gibt an, ob im Dialog alle Schüler angezeigt werden sollen.
    /// </summary>
    private bool isShowingAlleSchüler;

    /// <summary>
    /// Ein View von allen Schülern der Liste.
    /// </summary>
    private ICollectionView completeSchülerView;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundennotenWorkspaceViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schülerlisteViewModel">Die Schülerliste für die Stundennoten gegeben werden sollen.</param>
    /// <param name="stundeViewModel">Die Stunde für die Noten gegeben werden sollen.</param>
    public StundennotenWorkspaceViewModel(SchülerlisteViewModel schülerlisteViewModel, StundeViewModel stundeViewModel)
    {
      this.schülerliste = schülerlisteViewModel;
      this.completeSchülerView = CollectionViewSource.GetDefaultView(this.schülerliste.Schülereinträge);
      this.Stunde = stundeViewModel;

      this.IsShowingAlleSchüler = false;

      this.AddHausaufgabenCommand = new DelegateCommand(this.AddHausaufgaben);
      this.AddSonstigeNotenCommand = new DelegateCommand(this.AddSonstigeNoten);
    }

    /// <summary>
    /// Holt den Befehl, um vergessen Hausaufgaben anzulegen.
    /// </summary>
    public DelegateCommand AddHausaufgabenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um sonstige Noten anzulegen.
    /// </summary>
    public DelegateCommand AddSonstigeNotenCommand { get; private set; }

    /// <summary>
    /// Holt ein View der Schüler, die benotet werden sollen.
    /// </summary>
    [DependsUpon("IsShowingAlleSchüler")]
    public ICollectionView SchülerView
    {
      get
      {
        if (this.isShowingAlleSchüler)
        {
          return this.completeSchülerView;
        }

        // Wähle zufällig Schüler aus
        var subset = this.schülerliste.Schülereinträge.TakeRandom(Configuration.Instance.NotenProStunde);
        //var subset = this.schülerliste.Schülereinträge.OrderBy(x => this.randomGenerator.Next())
        //      .Take(Configuration.Instance.NotenProStunde);

        return CollectionViewSource.GetDefaultView(subset);
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob alle Schüler angezeigt werden sollen,
    /// oder nur eine zufällig Auswahl.
    /// </summary>
    public bool IsShowingAlleSchüler
    {
      get
      {
        return this.isShowingAlleSchüler;
      }

      set
      {
        this.isShowingAlleSchüler = value;
        this.RaisePropertyChanged("IsShowingAlleSchüler");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stunde currently selected in this workspace
    /// </summary>
    public StundeViewModel Stunde
    {
      get
      {
        return this.stunde;
      }

      set
      {
        this.stunde = value;
        this.RaisePropertyChanged("Stunde");
      }
    }

    /// <summary>
    /// Hier wird der Dialog zur Hausaufgabenkontrolle aufgerufen
    /// </summary>
    private async void AddHausaufgaben()
    {
      Selection.Instance.Schülerliste = this.schülerliste;

      if (Configuration.Instance.IsMetroMode)
      {
        var addDlg = new MetroAddHausaufgabeDialog(this.stunde.LerngruppenterminDatum);
        var metroWindow = Configuration.Instance.MetroWindow;
        await metroWindow.ShowMetroDialogAsync(addDlg);
        return;
      }

      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgaben hinzugefügt."), false))
      {
        var addDlg = new AddHausaufgabeDialog { Datum = this.stunde.LerngruppenterminDatum };
        if (addDlg.ShowDialog().GetValueOrDefault(false))
        {
          Selection.Instance.HausaufgabeDatum = addDlg.Datum;
          Selection.Instance.HausaufgabeBezeichnung = addDlg.Bezeichnung;

          // Reset currently selected hausaufgaben
          foreach (var schülereintragViewModel in this.schülerliste.Schülereinträge)
          {
            schülereintragViewModel.CurrentHausaufgabe = null;
          }

          var dlg = new HausaufgabenDialog { Schülerliste = this.schülerliste };
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
    private async void AddSonstigeNoten()
    {
      Selection.Instance.Schülerliste = this.schülerliste;

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
        var addDlg = new AddSonstigeNoteDialog() { Datum = this.stunde.LerngruppenterminDatum };
        if (addDlg.ShowDialog().GetValueOrDefault(false))
        {
          Selection.Instance.SonstigeNoteDatum = addDlg.Datum;
          Selection.Instance.SonstigeNoteBezeichnung = addDlg.Bezeichnung;
          Selection.Instance.SonstigeNoteNotentyp = addDlg.Notentyp;
          Selection.Instance.SonstigeNoteWichtung = addDlg.Wichtung;

          // Reset currently selected note
          foreach (var schülereintragViewModel in this.schülerliste.Schülereinträge)
          {
            schülereintragViewModel.CurrentNote = null;
          }

          var dlg = new SonstigeNotenDialog() { Schülerliste = this.schülerliste };
          undo = !dlg.ShowDialog().GetValueOrDefault(false);
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }
  }
}
