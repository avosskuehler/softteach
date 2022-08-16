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
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.View.Noten;

  using PrintDialog = System.Windows.Controls.PrintDialog;

  /// <summary>
  /// ViewModel für den Notenlisten dialog.
  /// </summary>
  public class NotenlistenWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Lerngruppe currently selected
    /// </summary>
    private LerngruppeViewModel currentLerngruppe;

    /// <summary>
    /// The Lerngruppe originally choosen
    /// </summary>
    private LerngruppeViewModel schülerlisteBackup;

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
    public NotenlistenWorkspaceViewModel(LerngruppeViewModel schülerliste)
    {
      this.schülerlisteBackup = schülerliste;
      this.CurrentLerngruppe = schülerliste;
      if (this.CurrentLerngruppe != null)
      {
        this.CurrentSchülereintrag = this.currentLerngruppe.Schülereinträge.First();
      }

      this.NotenTermintyp = NotenTermintyp.Zwischenstand;
      Selection.Instance.Lerngruppe = this.CurrentLerngruppe;
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
      foreach (var schülereintragViewModel in CurrentLerngruppe.Schülereinträge)
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
          o.NoteSchuljahr == this.CurrentLerngruppe.LerngruppeSchuljahr
          && o.NoteTermintyp != NotenTermintyp.Einzeln && o.NoteNotentyp == Notentyp.GesamtStand);
      foreach (var noteViewModel in zeugnisnoten)
      {
        var eintrag = new NotenlistenEintrag(noteViewModel.NoteTermintyp, this.CurrentLerngruppe.LerngruppeSchuljahr, noteViewModel.NoteDatum);
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
    /// Holt den Befehl, um die Notenliste der aktuellen Lerngruppe auszudrucken
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

        this.CurrentLerngruppe.NotenDatum = this.Notendatum;
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
        Selection.Instance.Lerngruppe = value;

        this.RaisePropertyChanged("CurrentLerngruppe");
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
        this.currentLerngruppe.NotenDatum = value;
        Selection.Instance.Lerngruppe.NotenDatum = value;
        foreach (var schülereintragViewModel in this.currentLerngruppe.Schülereinträge)
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
        if (this.CurrentLerngruppe != null)
        {
          title = string.Format(
            "Hier werden Noten für die Klasse {2} im Fach {1} für das Schuljahr {0} gemacht.",
            this.CurrentLerngruppe.LerngruppeSchuljahr.SchuljahrBezeichnung,
            this.CurrentLerngruppe.LerngruppeFach.FachBezeichnung,
            this.CurrentLerngruppe.LerngruppeBezeichnung);
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
      foreach (var schülereintragViewModel in CurrentLerngruppe.Schülereinträge)
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

    public void ResetLerngruppe()
    {
      this.CurrentLerngruppe = this.schülerlisteBackup;
    }
  }
}
