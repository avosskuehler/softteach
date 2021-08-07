namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Documents;

  using Helper;

  using Personen;
  using Setting;

  using SoftTeach.View.Noten;

  using PrintDialog = System.Windows.Controls.PrintDialog;

  /// <summary>
  /// ViewModel für den Notenlisten dialog.
  /// </summary>
  public class NotenlistenWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Schülerliste currently selected
    /// </summary>
    private SchülerlisteViewModel currentSchülerliste;

    /// <summary>
    /// The Schülerliste originally choosen
    /// </summary>
    private SchülerlisteViewModel schülerlisteBackup;

    /// <summary>
    /// The Schülereintrag currently selected
    /// </summary>
    private SchülereintragViewModel currentSchülereintrag;

    /// <summary>
    /// The ZeugnisnotenEintrag currently selected
    /// </summary>
    private NotenlistenEintrag currentNotenlistenEintrag;

    private NotenTermintyp notenTermintyp;

    private DateTime notendatum;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NotenlistenWorkspaceViewModel" /> Klasse.
    /// </summary>
    /// <param name="schülerliste">The schülerliste.</param>
    public NotenlistenWorkspaceViewModel(SchülerlisteViewModel schülerliste)
    {
      this.schülerlisteBackup = schülerliste;
      this.CurrentSchülerliste = schülerliste;
      if (this.CurrentSchülerliste != null)
      {
        this.CurrentSchülereintrag = this.currentSchülerliste.Schülereinträge.First();
      }

      this.NotenTermintyp = NotenTermintyp.Zwischenstand;
      Selection.Instance.Schülerliste = this.CurrentSchülerliste;
      this.NotenlistenEinträge = new ObservableCollection<NotenlistenEintrag>();
      this.Notendatum = DateTime.Today;
      this.PopulateNotenlisten();
      this.DeleteNotenlisteCommand = new DelegateCommand(this.DeleteNotenliste);
      //this.AddSonstigeNotenCommand = new DelegateCommand(this.AddSonstigeNoten);
      this.PrintNotenlisteCommand = new DelegateCommand(this.PrintNotenliste);
      //this.AddZeugnisnotenCommand = new DelegateCommand(this.AddZeugnisnoten);
    }

    /// <summary>
    /// Löscht die aktuellen Notenliste
    /// </summary>
    private void DeleteNotenliste()
    {
      foreach (var schülereintragViewModel in CurrentSchülerliste.Schülereinträge)
      {
        var notenToDelete =
          schülereintragViewModel.Noten.Where(
            o => o.NoteTermintyp == this.notenTermintyp && o.NoteDatum == this.notendatum).ToList();
        foreach (var noteViewModel in notenToDelete)
        {
          //App.UnitOfWork.Context.Noten.Remove(noteViewModel.Model);
          schülereintragViewModel.Noten.Remove(noteViewModel);
          //var success = App.MainViewModel.Noten.RemoveTest(noteViewModel);
          var result = schülereintragViewModel.Noten.RemoveTest(noteViewModel);
          schülereintragViewModel.UpdateNoten();
        }
      }

      this.PopulateNotenlisten();
    }

    /// <summary>
    /// Populates the Notenlisten.
    /// </summary>
    private void PopulateNotenlisten()
    {
      this.NotenlistenEinträge.Clear();

      var zeugnisnoten =
        this.CurrentSchülereintrag.Noten.Where(
          o =>
          o.NoteJahrtyp == this.CurrentSchülerliste.SchülerlisteJahrtyp
          && o.NoteTermintyp != NotenTermintyp.Einzeln && o.NoteNotentyp == Notentyp.GesamtStand);
      foreach (var noteViewModel in zeugnisnoten)
      {
        var eintrag = new NotenlistenEintrag(noteViewModel.NoteTermintyp, this.CurrentSchülerliste.SchülerlisteJahrtyp, noteViewModel.NoteDatum);
        this.NotenlistenEinträge.Add(eintrag);
      }
    }

    /// <summary>
    /// Holt den Befehl, um die aktuelle Notenliste zu löschen.
    /// </summary>
    public DelegateCommand DeleteNotenlisteCommand { get; private set; }

    ///// <summary>
    ///// Holt den Befehl, um sonstige Noten anzulegen.
    ///// </summary>
    //public DelegateCommand AddSonstigeNotenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um die Notenliste der aktuellen Schülerliste auszudrucken
    /// </summary>
    public DelegateCommand PrintNotenlisteCommand { get; private set; }

    ///// <summary>
    ///// Holt den Befehl, um Zeugnisnoten zu machen
    ///// </summary>
    //public DelegateCommand AddZeugnisnotenCommand { get; private set; }

    /// <summary>
    /// Gets or sets the zeugnisnoten.
    /// </summary>
    public ObservableCollection<NotenlistenEintrag> NotenlistenEinträge { get; set; }

    /// <summary>
    /// Holt oder setzt den aktuell ausgewählten Zeugnisnoteneintrag
    /// </summary>
    public NotenlistenEintrag CurrentNotenlistenEintrag
    {
      get
      {
        return this.currentNotenlistenEintrag;
      }

      set
      {
        this.currentNotenlistenEintrag = value;
        if (this.currentNotenlistenEintrag != null)
        {
          this.Notendatum = this.currentNotenlistenEintrag.Termin;
          this.NotenTermintyp = this.currentNotenlistenEintrag.NotenTermintyp;
        }

        this.CurrentSchülerliste.NotenDatum = this.Notendatum;
        this.RaisePropertyChanged("CurrentNotenlistenEintrag");
      }
    }

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

    public NotenTermintyp NotenTermintyp
    {
      get
      {
        return this.notenTermintyp;
      }
      set
      {
        this.notenTermintyp = value;
        this.RaisePropertyChanged("NotenTermintyp");
      }
    }

    /// <summary>
    /// Gets the datums label.
    /// </summary>
    /// <value>The datums label.</value>
    [DependsUpon("NotenTermintyp")]
    public string DatumsLabel
    {
      get
      {
        switch (this.notenTermintyp)
        {
          case NotenTermintyp.Zwischenstand:
            return "Datum des Zwischenstands";
          case NotenTermintyp.Halbjahr:
            return "Datum des Halbjahreszeugnisses";
          case NotenTermintyp.Ganzjahr:
            return "Datum des Zeugnisses für das ganze Jahr";
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the notendatum.
    /// </summary>
    /// <value>The notendatum.</value>
    public DateTime Notendatum
    {
      get
      {
        return this.notendatum;
      }

      set
      {
        this.notendatum = value;
        this.currentSchülerliste.NotenDatum = value;
        Selection.Instance.Schülerliste.NotenDatum = value;
        foreach (var schülereintragViewModel in this.currentSchülerliste.Schülereinträge)
        {
          schülereintragViewModel.AnpassungenAuslesen();
          schülereintragViewModel.UpdateNoten();
        }

        this.RaisePropertyChanged("Notendatum");
      }
    }

    /// <summary>
    /// Gets the dialog title.
    /// </summary>
    public string DialogTitle
    {
      get
      {
        var title = "Hier werden Noten gemacht";
        if (this.CurrentSchülerliste != null)
        {
          title = string.Format(
            "Hier werden Noten für die Klasse {2} im Fach {1} für das Schuljahr {0} gemacht.",
            this.CurrentSchülerliste.SchülerlisteJahrtyp.JahrtypBezeichnung,
            this.CurrentSchülerliste.SchülerlisteFach.FachBezeichnung,
            this.CurrentSchülerliste.SchülerlisteKlasse.KlasseBezeichnung);
        }

        return title;
      }
    }

    /// <summary>
    /// Checks if zeugnis an diesem Tag existiert bereits.
    /// </summary>
    /// <returns><c>true</c> if Zeugnis existiert, <c>false</c> otherwise.</returns>
    public bool CheckIfZeugnisExists()
    {
      return this.NotenlistenEinträge.Any(o => o.Termin == this.Notendatum);
    }

    /// <summary>
    /// Zeugnisnoten anlegen.
    /// </summary>
    public void ZeugnisnotenAnlegen()
    {
      foreach (var schülereintragViewModel in CurrentSchülerliste.Schülereinträge)
      {
        schülereintragViewModel.AddNote(Notentyp.MündlichStand, this.NotenTermintyp, schülereintragViewModel.MündlicheGesamtNoteInPunkten, this.Notendatum);
        schülereintragViewModel.AddNote(Notentyp.SchriftlichStand, this.NotenTermintyp, schülereintragViewModel.SchriftlicheGesamtNoteInPunkten, this.Notendatum);
        schülereintragViewModel.AddNote(Notentyp.GesamtStand, this.NotenTermintyp, schülereintragViewModel.GesamtNoteInPunkten, this.Notendatum);
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

    public void ResetSchülerliste()
    {
      this.CurrentSchülerliste = this.schülerlisteBackup;
    }
  }
}
