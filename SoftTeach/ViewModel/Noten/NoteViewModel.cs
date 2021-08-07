namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Globalization;
  using System.Linq;
  using System.Windows.Input;

  using SoftTeach.Model;

  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual note
  /// </summary>
  public class NoteViewModel : ViewModelBase
  {
    /// <summary>
    /// The zensur currently assigned to this note
    /// </summary>
    private ZensurViewModel zensur;

    /// <summary>
    /// The arbeit currently assigned to this note
    /// </summary>
    private ArbeitViewModel arbeit;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NoteViewModel"/> Klasse. 
    /// </summary>
    public NoteViewModel()
    {
      var note = new Note();
      note.Datum = DateTime.Now;
      note.IstSchriftlich = false;
      note.Wichtung = 1;
      note.Zensur = App.MainViewModel.Zensuren.First().Model;
      this.Model = note;
      App.UnitOfWork.Context.Noten.Add(note);
      //App.MainViewModel.Noten.Add(this);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NoteViewModel"/> Klasse. 
    /// </summary>
    /// <param name="note">
    /// The underlying note this ViewModel is to be based on
    /// </param>
    public NoteViewModel(Note note)
    {
      if (note == null)
      {
        throw new ArgumentNullException("note");
      }

      this.Model = note;

      //App.MainViewModel.Arbeiten.CollectionChanged += (sender, e) =>
      //{
      //  if (e.OldItems != null && e.OldItems.Contains(this.NoteZensur))
      //  {
      //    this.NoteArbeit = null;
      //  }
      //};

      this.EditNoteCommand = new DelegateCommand(this.EditNote);
      this.DeleteNoteCommand = new DelegateCommand(this.DeleteNote);
    }

    /// <summary>
    /// Holt die Methode zur Editierung der aktuellen Note
    /// </summary>
    public DelegateCommand EditNoteCommand { get; private set; }

    /// <summary>
    /// Holt die Methode zum Löschen der aktuellen Note
    /// </summary>
    public DelegateCommand DeleteNoteCommand { get; private set; }

    /// <summary>
    /// Holt das Modell für dieses ViewModel.
    /// </summary>
    public Note Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung der Note.
    /// D.h. die Aktivität oder Ausarbeitung für die die Note gegeben wurde.
    /// </summary>
    public string NoteBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "NoteBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("NoteBezeichnung");
      }
    }


    /// <summary>
    /// Holt oder setzt die Datim
    /// </summary>
    public DateTime NoteDatum
    {
      get
      {
        return this.Model.Datum;
      }

      set
      {
        if (value == this.Model.Datum) return;
        this.UndoablePropertyChanging(this, "NoteDatum", this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("NoteDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt die Datim
    /// </summary>
    public string NoteDatumFormatted
    {
      get
      {
        return this.Model.Datum.ToShortDateString();
      }
    }

    /// <summary>
    /// Holt den Monat vom Notendatum.
    /// </summary>
    [DependsUpon("NoteDatum")]
    public string NoteMonat
    {
      get
      {
        return this.NoteDatum.ToString("MMMM", new CultureInfo("de-DE"));
      }
    }

    /// <summary>
    /// Holt oder setzt die Notentyp
    /// </summary>
    public Notentyp NoteNotentyp
    {
      get
      {
        return (Notentyp)Enum.Parse(typeof(Notentyp), this.Model.Notentyp);
      }

      set
      {
        if (value.ToString() == this.Model.Notentyp) return;
        this.UndoablePropertyChanging(this, "NoteNotentyp", this.NoteNotentyp, value);
        this.Model.Notentyp = value.ToString();
        this.RaisePropertyChanged("NoteNotentyp");
      }
    }

    /// <summary>
    /// Holt oder setzt den Noten Termintyp
    /// </summary>
    public NotenTermintyp NoteTermintyp
    {
      get
      {
        if (this.Model.NotenTermintyp == null)
        {
          this.Model.NotenTermintyp = NotenTermintyp.Einzeln.ToString();
        }

        return (NotenTermintyp)Enum.Parse(typeof(NotenTermintyp), this.Model.NotenTermintyp);
      }

      set
      {
        if (value.ToString() == this.Model.NotenTermintyp) return;
        this.UndoablePropertyChanging(this, "NoteTermintyp", this.NoteTermintyp, value);
        this.Model.NotenTermintyp = value.ToString();
        this.RaisePropertyChanged("NoteTermintyp");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob diese Note schriftlich ist.
    /// </summary>
    public bool NoteIstSchriftlich
    {
      get
      {
        return this.Model.IstSchriftlich;
      }

      set
      {
        if (value == this.Model.IstSchriftlich) return;
        this.UndoablePropertyChanging(this, "NoteIstSchriftlich", this.Model.IstSchriftlich, value);
        this.Model.IstSchriftlich = value;
        this.RaisePropertyChanged("NoteIstSchriftlich");
      }
    }

    /// <summary>
    /// Holt oder setzt die Gewichtung dieser Note.
    /// </summary>
    public int NoteWichtung
    {
      get
      {
        return this.Model.Wichtung;
      }

      set
      {
        if (value == this.Model.Wichtung) return;
        this.UndoablePropertyChanging(this, "NoteWichtung", this.Model.Wichtung, value);
        this.Model.Wichtung = value;
        this.RaisePropertyChanged("NoteWichtung");
      }
    }

    /// <summary>
    /// Holt einen String, der eine Kurzform der Notengewichtung enthält.
    /// </summary>
    [DependsUpon("NoteWichtung")]
    public string NoteGewichtung
    {
      get
      {
        return "W:" + this.Model.Wichtung.ToString(CultureInfo.InvariantCulture);
      }
    }

    /// <summary>
    /// Holt oder setzt die Zensur currently assigned to this Note
    /// </summary>
    public ZensurViewModel NoteZensur
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Zensur == null)
        {
          return null;
        }

        if (this.zensur == null || this.zensur.Model != this.Model.Zensur)
        {
          this.zensur = App.MainViewModel.Zensuren.SingleOrDefault(d => d.Model == this.Model.Zensur);
        }

        return this.zensur;
      }

      set
      {
        if (this.zensur != null)
        {
          if (value.ZensurNotenpunkte == this.zensur.ZensurNotenpunkte) return;
          this.UndoablePropertyChanging(this, "NoteZensur", this.zensur, value);
        }

        this.zensur = value;
        this.Model.Zensur = value.Model;
        this.RaisePropertyChanged("NoteZensur");
      }
    }

    /// <summary>
    /// Holt die Zensur im korrekten Bepunktungsstil
    /// </summary>
    [DependsUpon("NoteZensur")]
    public string ZensurString
    {
      get
      {
        var bepunktungsTyp =
          (Bepunktungstyp)
          Enum.Parse(typeof(Bepunktungstyp), this.Model.Schülereintrag.Schülerliste.Klasse.Klassenstufe.Jahrgangsstufe.Bepunktungstyp);
        switch (bepunktungsTyp)
        {
          case Bepunktungstyp.GanzeNote:
            return this.NoteZensur.ZensurGanzeNote.ToString(CultureInfo.InvariantCulture);
          case Bepunktungstyp.NoteMitTendenz:
            return this.NoteZensur.ZensurNoteMitTendenz;
          case Bepunktungstyp.Notenpunkte:
            return this.NoteZensur.ZensurNotenpunkte.ToString(CultureInfo.InvariantCulture);
        }

        return string.Empty;
      }
    }

    [DependsUpon("NoteZensur")]
    public int NoteZensurGanzeNote
    {
      get
      {
        return this.NoteZensur.ZensurGanzeNote;
      }
    }

    /// <summary>
    /// Holt oder setzt die Arbeit currently assigned to this Note
    /// </summary>
    public ArbeitViewModel NoteArbeit
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Arbeit == null)
        {
          return null;
        }

        if (this.arbeit == null || this.arbeit.Model != this.Model.Arbeit)
        {
          this.arbeit = App.MainViewModel.Arbeiten.SingleOrDefault(d => d.Model == this.Model.Arbeit);
        }

        return this.arbeit;
      }

      set
      {
        if (value.ArbeitBezeichnung == this.NoteArbeit.ArbeitBezeichnung) return;
        this.UndoablePropertyChanging(this, "NoteArbeit", this.arbeit, value);
        this.arbeit = value;
        this.Model.Arbeit = value.Model;
        this.RaisePropertyChanged("NoteArbeit");
      }
    }

    /// <summary>
    /// Holt das Schuljahr dieser Note
    /// </summary>
    public JahrtypViewModel NoteJahrtyp
    {
      get
      {
        return
          App.MainViewModel.Jahrtypen.SingleOrDefault(d => d.Model == this.Model.Schülereintrag.Schülerliste.Jahrtyp);
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Note: " + this.NoteBezeichnung;
    }

    /// <summary>
    /// Diese Methode wird aufgerufen, wenn der Benutzer eine Note anklickt.
    /// Es öffnet sich der Editierdialog für Noten
    /// </summary>
    private async void EditNote()
    {
      var workspace = new NotenWorkspaceViewModel(this);
      if (Configuration.Instance.IsMetroMode)
      {
        var metroWindow = Configuration.Instance.MetroWindow;
        metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
        var dialog = new MetroNoteDialog(workspace);
        await metroWindow.ShowMetroDialogAsync(dialog);
      }
      else
      {
        var dlg = new AddNoteDialog(workspace);
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          Selection.Instance.Schülereintrag.UpdateNoten();
        }
      }
    }

    /// <summary>
    /// Diese Methode wird aufgerufen, wenn der Benutzer eine Note anklickt.
    /// und dann auf die Entfernen Taste drückt.
    /// </summary>
    private void DeleteNote()
    {
      Selection.Instance.Schülereintrag.DeleteNoteCommand.Execute(null);
    }
  }
}
