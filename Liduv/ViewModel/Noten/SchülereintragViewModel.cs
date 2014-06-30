﻿namespace Liduv.ViewModel.Noten
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Globalization;
  using System.Linq;
  using System.Windows.Input;
  using System.Windows.Media;

  using Liduv.ExceptionHandling;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Noten;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Personen;

  /// <summary>
  /// ViewModel of an individual schülereintrag
  /// </summary>
  public class SchülereintragViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Die zuletzt berechnete Gesamtnote für alle mündlichen Leistungen.
    /// </summary>
    private int mündlicheGesamtnote;

    /// <summary>
    /// Die zuletzt berechnete Gesamtnote für alle schriftlichen Leistungen.
    /// </summary>
    private int schriftlicheGesamtnote;

    /// <summary>
    /// The person currently assigned to this schülereintrag
    /// </summary>
    private PersonViewModel person;

    /// <summary>
    /// The hausaufgabe currently selected
    /// </summary>
    private HausaufgabeViewModel currentHausaufgabe;

    /// <summary>
    /// The note currently selected
    /// </summary>
    private NoteViewModel currentNote;

    /// <summary>
    /// The Notentendenz currently selected
    /// </summary>
    private NotentendenzViewModel currentNotentendenz;

    /// <summary>
    /// Das momentane Ergebnis
    /// </summary>
    private ErgebnisViewModel currentErgebnis;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchülereintragViewModel"/> Klasse. 
    /// </summary>
    public SchülereintragViewModel()
    {
      var schülereintrag = new Schülereintrag();
      schülereintrag.Schülerliste = Selection.Instance.Schülerliste.Model;
      this.Model = schülereintrag;
      App.MainViewModel.Schülereinträge.Add(this);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchülereintragViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schülereintrag">
    /// The underlying schülereintrag this ViewModel is to be based on
    /// </param>
    public SchülereintragViewModel(Schülereintrag schülereintrag)
    {
      if (schülereintrag == null)
      {
        throw new ArgumentNullException("schülereintrag");
      }

      this.Model = schülereintrag;

      // Build data structures for Hausaufgaben
      this.Hausaufgaben = new ObservableCollection<HausaufgabeViewModel>();
      foreach (var hausaufgabe in schülereintrag.Hausaufgaben)
      {
        var vm = new HausaufgabeViewModel(hausaufgabe);
        App.MainViewModel.Hausaufgaben.Add(vm);
        this.Hausaufgaben.Add(vm);
      }

      this.Hausaufgaben.CollectionChanged += this.HausaufgabenCollectionChanged;

      // Build data structures for Noten
      this.Noten = new ObservableCollection<NoteViewModel>();
      foreach (var note in schülereintrag.Noten)
      {
        var vm = new NoteViewModel(note);
        App.MainViewModel.Noten.Add(vm);
        this.Noten.Add(vm);
      }

      this.Noten.CollectionChanged += this.NotenCollectionChanged;

      // Build data structures for Notentendenzen
      this.Notentendenzen = new ObservableCollection<NotentendenzViewModel>();
      foreach (var notentendenz in schülereintrag.Notentendenzen)
      {
        var vm = new NotentendenzViewModel(notentendenz);
        App.MainViewModel.Notentendenzen.Add(vm);
        this.Notentendenzen.Add(vm);
      }

      this.Notentendenzen.CollectionChanged += this.NotentendenzenCollectionChanged;

      this.Ergebnisse = new ObservableCollection<ErgebnisViewModel>();
      this.CurrentArbeitErgebnisse = new ObservableCollection<ErgebnisViewModel>();

      foreach (var ergebnis in schülereintrag.Ergebnisse)
      {
        var vm = new ErgebnisViewModel(ergebnis);
        this.Ergebnisse.Add(vm);
      }

      this.Ergebnisse.CollectionChanged += this.ErgebnisseCollectionChanged;

      this.AddHausaufgabeCommand = new DelegateCommand(this.AddHausaufgabe);
      this.DeleteHausaufgabeCommand = new DelegateCommand(this.DeleteCurrentHausaufgabe, () => this.CurrentHausaufgabe != null);
      this.AddNoteCommand = new DelegateCommand(this.AddNote);
      this.DeleteNoteCommand = new DelegateCommand(this.DeleteCurrentNote, () => this.CurrentNote != null);
      this.AddNotentendenzCommand = new DelegateCommand(this.AddNotentendenz);
      this.DeleteNotentendenzCommand = new DelegateCommand(this.DeleteCurrentNotentendenz, () => this.CurrentNotentendenz != null);
      this.Qualität1Command = new DelegateCommand(this.AddMündlicheQualitätsNote1);
      this.Quantität1Command = new DelegateCommand(this.AddMündlicheQuantitätsNote1);
      this.Qualität2Command = new DelegateCommand(this.AddMündlicheQualitätsNote2);
      this.Quantität2Command = new DelegateCommand(this.AddMündlicheQuantitätsNote2);
      this.Qualität3Command = new DelegateCommand(this.AddMündlicheQualitätsNote3);
      this.Quantität3Command = new DelegateCommand(this.AddMündlicheQuantitätsNote3);
      this.Qualität4Command = new DelegateCommand(this.AddMündlicheQualitätsNote4);
      this.Quantität4Command = new DelegateCommand(this.AddMündlicheQuantitätsNote4);
      this.Qualität5Command = new DelegateCommand(this.AddMündlicheQualitätsNote5);
      this.Quantität5Command = new DelegateCommand(this.AddMündlicheQuantitätsNote5);
      this.Qualität6Command = new DelegateCommand(this.AddMündlicheQualitätsNote6);
      this.Quantität6Command = new DelegateCommand(this.AddMündlicheQuantitätsNote6);
      Selection.Instance.PropertyChanged += this.SelectionPropertyChanged;
    }

    /// <summary>
    /// Holt das Model für den aktuellen Schülereintrag.
    /// </summary>
    public Schülereintrag Model { get; private set; }

    /// <summary>
    /// Holt das Command zur Erstellung einer einzelnen nicht gemachten Hausaufgabe
    /// </summary>
    public DelegateCommand AddHausaufgabeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Hausaufgabe
    /// </summary>
    public DelegateCommand DeleteHausaufgabeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Note a new Note
    /// </summary>
    public DelegateCommand AddNoteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Note
    /// </summary>
    public DelegateCommand DeleteNoteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Notentendenz a new Notentendenz
    /// </summary>
    public DelegateCommand AddNotentendenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Notentendenz
    /// </summary>
    public DelegateCommand DeleteNotentendenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Qualität zu geben
    /// </summary>
    public DelegateCommand Qualität1Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Quantität zu geben
    /// </summary>
    public DelegateCommand Quantität1Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Qualität zu geben
    /// </summary>
    public DelegateCommand Qualität2Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Quantität zu geben
    /// </summary>
    public DelegateCommand Quantität2Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Qualität zu geben
    /// </summary>
    public DelegateCommand Qualität3Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Quantität zu geben
    /// </summary>
    public DelegateCommand Quantität3Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Qualität zu geben
    /// </summary>
    public DelegateCommand Qualität4Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Quantität zu geben
    /// </summary>
    public DelegateCommand Quantität4Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Qualität zu geben
    /// </summary>
    public DelegateCommand Qualität5Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Quantität zu geben
    /// </summary>
    public DelegateCommand Quantität5Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Qualität zu geben
    /// </summary>
    public DelegateCommand Qualität6Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine eins für die mündliche Quantität zu geben
    /// </summary>
    public DelegateCommand Quantität6Command { get; private set; }

    /// <summary>
    /// Holt oder setzt die Noten für diesen Schülereintrag
    /// </summary>
    public ObservableCollection<NoteViewModel> Noten { get; set; }

    /// <summary>
    /// Holt oder setzt die Notentendenzen für diesen Schülereintrag
    /// </summary>
    public ObservableCollection<NotentendenzViewModel> Notentendenzen { get; set; }

    /// <summary>
    /// Holt oder setzt die Hausaufgaben für diesen Schülereintrag
    /// </summary>
    public ObservableCollection<HausaufgabeViewModel> Hausaufgaben { get; set; }

    /// <summary>
    /// Holt oder setzt die Ergebnisse für diesen Schülereintrag
    /// </summary>
    public ObservableCollection<ErgebnisViewModel> Ergebnisse { get; set; }

    /// <summary>
    /// Holt oder setzt die currently selected Hausaufgabe
    /// </summary>
    public HausaufgabeViewModel CurrentHausaufgabe
    {
      get
      {
        return this.currentHausaufgabe;
      }

      set
      {
        this.currentHausaufgabe = value;
        this.RaisePropertyChanged("CurrentHausaufgabe");
        this.DeleteHausaufgabeCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected Note
    /// </summary>
    public NoteViewModel CurrentNote
    {
      get
      {
        return this.currentNote;
      }

      set
      {
        this.currentNote = value;
        this.RaisePropertyChanged("CurrentNote");
        this.DeleteNoteCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected Notentendenz
    /// </summary>
    public NotentendenzViewModel CurrentNotentendenz
    {
      get
      {
        return this.currentNotentendenz;
      }

      set
      {
        this.currentNotentendenz = value;
        this.RaisePropertyChanged("CurrentNotentendenz");
        this.DeleteNotentendenzCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public PersonViewModel SchülereintragPerson
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Person == null)
        {
          return null;
        }

        if (this.person == null || this.person.Model != this.Model.Person)
        {
          this.person = App.MainViewModel.Personen.SingleOrDefault(d => d.Model == this.Model.Person);
        }

        return this.person;
      }

      set
      {
        if (value == this.person) return;
        this.UndoablePropertyChanging(this, "SchülereintragPerson", this.person, value);
        this.person = value;
        this.Model.Person = value.Model;
        this.RaisePropertyChanged("SchülereintragPerson");
      }
    }

    [DependsUpon("SchülereintragPerson")]
    public string SchülereintragSortByNachnameProperty
    {
      get
      {
        return this.SchülereintragPerson.PersonNachname;
      }
    }

    /// <summary>
    /// Holt die Überschrift für diesen Schülereintrag.
    /// Wird im Modul ganz oben angezeigt.
    /// </summary>
    [DependsUpon("SchülereintragPerson")]
    public string SchülereintragÜberschrift
    {
      get
      {
        return this.Model.Schülerliste.Fach.Bezeichnung + ": Noten für " + this.SchülereintragPerson.PersonVorname + " " + this.SchülereintragPerson.PersonNachname;
      }
    }

    /// <summary>
    /// Holt die berechnete Gesamtnote.
    /// </summary>
    public string Gesamtnote
    {
      get { return this.BerechneGesamtnote(); }
    }

    #region Mündlich

    /// <summary>
    /// Holt die Gesamtnote für alle mündlichen Leistungen.
    /// </summary>
    public string MündlicheGesamtNote
    {
      get { return this.BerechneMündlicheGesamtnote(); }
    }

    /// <summary>
    /// Holt eine Liste aller mündlichen Noten.
    /// </summary>
    public IEnumerable<NoteViewModel> MündlicheNotenCollection
    {
      get { return this.Noten.Where(o => o.NoteIstSchriftlich == false); }
    }

    /// <summary>
    /// Holt die Durchschnittsnote der mündlichen Qualität.
    /// </summary>
    public string MündlicheQualitätNote
    {
      get
      {
        var qualitätsNoten = this.Noten.Where(o => o.NoteIstSchriftlich == false
          && o.NoteNotentyp == Notentyp.MündlichQualität);
        return this.BerechneDurchschnittsNote(qualitätsNoten);
      }
    }

    /// <summary>
    /// Holt die Gewichtung der Noten für die mündlichen Qualität
    /// </summary>
    public float MündlicheQualitätWichtung
    {
      get
      {
        return this.Model.Schülerliste.NotenWichtung.MündlichQualität;
      }
    }

    /// <summary>
    /// Holt die Durschnittsnote der mündlichen Quantität
    /// </summary>
    public string MündlicheQuantitätNote
    {
      get
      {
        var quantitätsNoten = this.Noten.Where(o => o.NoteIstSchriftlich == false
          && o.NoteNotentyp == Notentyp.MündlichQuantität);
        return this.BerechneDurchschnittsNote(quantitätsNoten);
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der mündlichen Qualität für
    /// die aktuelle Schülerliste
    /// </summary>
    public float MündlicheQuantitätWichtung
    {
      get { return this.Model.Schülerliste.NotenWichtung.MündlichQuantität; }
    }

    /// <summary>
    /// Holt eine Liste aller mündlichen Noten, die nicht Quantität oder Qualität sind.
    /// </summary>
    public IEnumerable<NoteViewModel> MündlicheWeitereNotenCollection
    {
      get
      {
        var weitere = this.Noten.Where(o => o.NoteIstSchriftlich == false
          && o.NoteNotentyp == Notentyp.MündlichSonstige);
        return weitere;
      }
    }

    /// <summary>
    /// Holt die Durchschnittsnote für alle mündlichen Noten
    /// die nicht Quantität oder Qualität sind
    /// </summary>
    public string MündlicheWeitereNotenGesamtnote
    {
      get
      {
        var sonstigeNoten =
          this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichSonstige);
        return this.BerechneDurchschnittsNote(sonstigeNoten);
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der weiteren mündlichen Noten für
    /// die aktuelle Schülerliste
    /// </summary>
    public float MündlicheWeitereNotenWichtung
    {
      get { return this.Model.Schülerliste.NotenWichtung.MündlichSonstige; }
    }

    #endregion // mündlich

    #region Schriftlich

    /// <summary>
    /// Holt die Gesamtnote für alle schriftlichen Leistungen.
    /// </summary>
    public string SchriftlicheGesamtNote
    {
      get { return this.BerechneSchriftlicheGesamtnote(); }
    }

    /// <summary>
    /// Holt eine Liste aller Klausur und Klassenarbeitsnoten.
    /// </summary>
    public IEnumerable<NoteViewModel> SchriftlichKlausurenNotenCollection
    {
      get
      {
        return this.Noten.Where(o => o.NoteIstSchriftlich
          && o.NoteNotentyp == Notentyp.SchriftlichKlassenarbeit);
      }
    }

    /// <summary>
    /// Holt die Durchschnittsnote der mündlichen Qualität.
    /// </summary>
    public string SchriftlichKlausurenGesamtnote
    {
      get
      {
        var klassenarbeitNoten = this.Noten.Where(o => o.NoteIstSchriftlich
          && o.NoteNotentyp == Notentyp.SchriftlichKlassenarbeit);
        return this.BerechneDurchschnittsNote(klassenarbeitNoten);
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der schriftlichen Klausuren und
    /// Klassenarbeiten für die aktuelle Schülerliste
    /// </summary>
    public float SchriftlichKlausurenWichtung
    {
      get { return this.Model.Schülerliste.NotenWichtung.SchriftlichKlassenarbeit; }
    }

    /// <summary>
    /// Holt eine Liste aller schriftlichen Noten, die nicht Quantität oder Qualität
    /// zu Klassenarbeiten oder Klausuren gehören sind.
    /// </summary>
    public IEnumerable<NoteViewModel> SchriftlichWeitereNotenCollection
    {
      get
      {
        var weitere = this.Noten.Where(
          o => o.NoteIstSchriftlich
          && o.NoteNotentyp == Notentyp.SchriftlichSonstige);
        return weitere;
      }
    }

    /// <summary>
    /// Holt die Durchschnittsnote für alle schriftlichen Noten
    /// die nicht zu Klassenarbeiten oder Klausuren gehören.
    /// </summary>
    public string SchriftlichWeitereNotenGesamtnote
    {
      get
      {
        var sonstigeNoten =
          this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichSonstige);
        return this.BerechneDurchschnittsNote(sonstigeNoten);
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der weiteren schriftlichen Noten für
    /// die aktuelle Schülerliste
    /// </summary>
    public float SchriftlichWeitereNotenWichtung
    {
      get { return this.Model.Schülerliste.NotenWichtung.SchriftlichSonstige; }
    }

    #endregion // Schriftlich

    /// <summary>
    /// Holt das passende Pfeilbild zur Tendenz der gemachten Hausaufgaben.
    /// </summary>
    public ImageSource HausaufgabenTendenzImage
    {
      get
      {
        switch (this.BerechneHausaufgabenBepunktung())
        {
          default:
          case 0:
            return App.GetImageSource("PfeilO32.png");
          case -1:
            return App.GetImageSource("PfeilSO32.png");
          case -2:
            return App.GetImageSource("PfeilS32.png");
        }
      }
    }

    /// <summary>
    /// Holt ein Bild, dass die Notentendenzen zusammenfasst.
    /// </summary>
    public ImageSource TendenzenTendenzImage
    {
      get
      {
        switch (this.BerechneTendenzBepunktung())
        {
          case 2:
            return App.GetImageSource("PfeilN32.png");
          case 1:
            return App.GetImageSource("PfeilNO32.png");
          default:
          case 0:
            return App.GetImageSource("PfeilO32.png");
          case -1:
            return App.GetImageSource("PfeilSO32.png");
          case -2:
            return App.GetImageSource("PfeilS32.png");
        }
      }
    }

    /// <summary>
    /// Holt die Anzahl nicht gemachter Hausaufgaben
    /// </summary>
    public string NichtgemachteHausaufgaben
    {
      get { return "Nicht gemacht: " + this.Hausaufgaben.Count(); }
    }

    /// <summary>
    /// Holt die Anzahl nicht gemachter Hausaufgaben.
    /// </summary>
    public int NichtGemachteHausaufgabenAnzahl
    {
      get
      {
        return this.Hausaufgaben.Count;
      }
    }

    /// <summary>
    /// Holt die Anzahl nachgereichter Hausaufgaben
    /// </summary>
    public string NachgereichteHausaufgaben
    {
      get
      {
        var nachgereicht = this.Hausaufgaben.Count(o => o.HausaufgabeIstNachgereicht);
        return "davon nachgereicht: " + nachgereicht;
      }
    }

    /// <summary>
    /// Holt oder setzt die Ergebnisse für diesen Schülereintrag
    /// </summary>
    public ObservableCollection<ErgebnisViewModel> CurrentArbeitErgebnisse { get; set; }

    /// <summary>
    /// Holt die berechnete Summe aller Punkte der aktuellen Arbeit
    /// </summary>
    public int CurrentArbeitPunktsumme
    {
      get
      {
        var ergebnisseDerAktuellenArbeit =
          this.Ergebnisse.Where(o => o.Model.Aufgabe.Arbeit.Id == Selection.Instance.Arbeit.Model.Id);
        var summe = ergebnisseDerAktuellenArbeit.Sum(o => o.ErgebnisPunktzahl);
        return summe.GetValueOrDefault(0);
      }
    }

    /// <summary>
    /// Holt den Prozentsatz der errreichten Punkte der momentanen Arbeit.
    /// </summary>
    public string CurrentArbeitProzentsatz
    {
      get
      {
        var gesamtPunktzahl = Selection.Instance.Arbeit.ArbeitGesamtpunktzahl;
        var prozentSatz = (float)this.CurrentArbeitPunktsumme / gesamtPunktzahl * 100;

        return prozentSatz.ToString("N0") + "%";
      }
    }

    /// <summary>
    /// Holt die berechnete Gesamtnote für die aktuelle Arbeit.
    /// </summary>
    public string CurrentArbeitNote
    {
      get
      {
        var punktZahl = this.CurrentArbeitPunktsumme;
        if (punktZahl == 0)
        {
          return string.Empty;
        }

        var gesamtPunktzahl = Selection.Instance.Arbeit.ArbeitGesamtpunktzahl;
        var prozentSatz = (float)punktZahl / gesamtPunktzahl;
        var bewertungsschema = Selection.Instance.Arbeit.ArbeitBewertungsschema;
        var bewertungsTyp = Selection.Instance.Arbeit.ArbeitBepunktungstyp;
        ZensurViewModel zensur =
          bewertungsschema.Prozentbereiche.OrderBy(o => o.ProzentbereichVonProzent).Last().ProzentbereichZensur;

        foreach (var prozentbereich in bewertungsschema.Prozentbereiche.OrderBy(o => o.ProzentbereichVonProzent))
        {
          if (prozentSatz > prozentbereich.ProzentbereichBisProzent)
          {
            continue;
          }

          zensur = prozentbereich.ProzentbereichZensur;
          break;
        }

        if (zensur == null)
        {
          return string.Empty;
        }

        // Note des Schülereintrags für die Arbeit wird aus der Arbeitsnote erzeugt,
        // bzw. aktualisiert
        var existingNotes =
          this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteDatum == Selection.Instance.Arbeit.ArbeitDatum);

        var noteViewModels = existingNotes as IList<NoteViewModel> ?? existingNotes.ToList();
        if (!noteViewModels.Any())
        {
          var note = new Note();
          note.Arbeit = Selection.Instance.Arbeit.Model;
          note.Bezeichnung = Selection.Instance.Arbeit.ArbeitBezeichnung;
          note.Datum = Selection.Instance.Arbeit.ArbeitDatum;
          note.IstSchriftlich = true;
          note.Notentyp = Selection.Instance.Arbeit.ArbeitIstKlausur
                            ? Notentyp.SchriftlichKlassenarbeit.ToString()
                            : Notentyp.SchriftlichSonstige.ToString();
          note.Wichtung = 1;
          note.Zensur = zensur.Model;
          note.Schülereintrag = this.Model;
          var vm = new NoteViewModel(note);
          App.MainViewModel.Noten.Add(vm);
          this.Noten.Add(vm);
          this.currentNote = vm;
        }
        else
        {
          var existingNote = noteViewModels.First();
          existingNote.NoteArbeit = Selection.Instance.Arbeit;
          existingNote.NoteBezeichnung = Selection.Instance.Arbeit.ArbeitBezeichnung;
          existingNote.NoteDatum = Selection.Instance.Arbeit.ArbeitDatum;
          existingNote.NoteIstSchriftlich = true;
          existingNote.NoteNotentyp = Selection.Instance.Arbeit.ArbeitIstKlausur
                                        ? Notentyp.SchriftlichKlassenarbeit
                                        : Notentyp.SchriftlichSonstige;
          existingNote.NoteWichtung = 1;
          existingNote.NoteZensur = zensur;
          this.currentNote = existingNote;
        }

        switch (bewertungsTyp)
        {
          case Bepunktungstyp.GanzeNote:
            return zensur.ZensurGanzeNote.ToString(CultureInfo.InvariantCulture);
          case Bepunktungstyp.Notenpunkte:
            return zensur.ZensurNotenpunkte.ToString(CultureInfo.InvariantCulture);
          case Bepunktungstyp.NoteMitTendenz:
            return zensur.ZensurNoteMitTendenz;
          default:
            return zensur.ZensurNoteMitTendenz;
        }
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Schülereintrag: " + this.SchülereintragÜberschrift;
    }

    /// <summary>
    /// Mit dieser Methode werden alle nachrangig berechneten Notenproperties 
    /// aktualisiert.
    /// </summary>
    public void UpdateNoten()
    {
      this.RaisePropertyChanged("CurrentArbeitPunktsumme");
      this.RaisePropertyChanged("CurrentArbeitNote");
      this.RaisePropertyChanged("NachgereichteHausaufgaben");
      this.RaisePropertyChanged("NichtgemachteHausaufgaben");
      this.RaisePropertyChanged("NichtGemachteHausaufgabenAnzahl");
      this.RaisePropertyChanged("HausaufgabenTendenzImage");
      this.RaisePropertyChanged("TendenzenTendenzImage");

      this.RaisePropertyChanged("MündlicheNotenCollection");
      this.RaisePropertyChanged("MündlicheQualitätNote");
      this.RaisePropertyChanged("MündlicheQuantitätNote");
      this.RaisePropertyChanged("MündlicheWeitereNotenCollection");
      this.RaisePropertyChanged("MündlichWeitereNotenGesamtnote");
      this.RaisePropertyChanged("MündlicheGesamtNote");

      this.RaisePropertyChanged("SchriftlichWeitereNotenCollection");
      this.RaisePropertyChanged("SchriftlichWeitereNotenGesamtnote");
      this.RaisePropertyChanged("SchriftlichKlausurenNotenCollection");
      this.RaisePropertyChanged("SchriftlichKlausurenGesamtnote");
      this.RaisePropertyChanged("SchriftlicheGesamtnote");

      this.RaisePropertyChanged("Gesamtnote");
    }

    /// <summary>
    /// Mit dieser Methode werden alle nachrangig berechneten Arbeitsproperties 
    /// aktualisiert.
    /// </summary>
    public void UpdateErgebnisse()
    {
      this.CurrentArbeitErgebnisse.Clear();
      foreach (var ergebnis in this.Ergebnisse.Where(ergebnis => ergebnis.Model.Aufgabe.Arbeit == Selection.Instance.Arbeit.Model))
      {
        this.CurrentArbeitErgebnisse.Add(ergebnis);
      }

      this.RaisePropertyChanged("CurrentArbeitErgebnisse");
      this.RaisePropertyChanged("CurrentArbeitNote");
      this.RaisePropertyChanged("CurrentArbeitProzentsatz");
      this.RaisePropertyChanged("CurrentArbeitPunktsumme");
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="viewModel">The object to be compared with this instance</param>
    /// <returns>Less than zero if This object is less than the other parameter. 
    /// Zero if This object is equal to other. Greater than zero if This object is greater than other.
    /// </returns>
    public int CompareTo(object viewModel)
    {
      var compareSchülereintrag = viewModel as SchülereintragViewModel;
      if (compareSchülereintrag == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Person.Vorname, compareSchülereintrag.Model.Person.Vorname, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tritt auf, wenn die ErgebnisseCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void ErgebnisseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Ergebnisse", this.Ergebnisse, e, false, "Änderung der Ergebnisse");
    }

    /// <summary>
    /// The selection property changed.
    /// </summary>
    /// <param name="sender">The sender. </param>
    /// <param name="e">The e.</param>
    private void SelectionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Arbeit" && Selection.Instance.Arbeit != null)
      {
        this.UpdateErgebnisse();
      }
    }

    /// <summary>
    /// Berechnet die Gesamtnote aus mündlichen und schriftlichen Teilnoten
    /// in Abhängigkeit der Gesamtgewichtung.
    /// </summary>
    /// <returns>Gesamtnote für diesen Schüler im Format des Klassenstufen <see cref="Bepunktungstyp"/>s,
    /// oder "?" wenn nicht genügend Noten vorhanden sind.</returns>
    private string BerechneGesamtnote()
    {
      //if (this.mündlicheGesamtnote == 0 || this.schriftlicheGesamtnote == 0)
      //{
      //  return "?";
      //}

      var mündlicheWichtung = this.Model.Schülerliste.NotenWichtung.MündlichGesamt;
      var schriftlichWichtung = this.Model.Schülerliste.NotenWichtung.SchriftlichGesamt;
      var gesamtNote = (this.mündlicheGesamtnote * mündlicheWichtung) + (this.schriftlicheGesamtnote * schriftlichWichtung);
      var gesamtNoteGerundet = (int)Math.Round(gesamtNote, 0);
      gesamtNoteGerundet += this.BerechneHausaufgabenBepunktung();
      gesamtNoteGerundet += this.BerechneTendenzBepunktung();
      gesamtNoteGerundet = Math.Max(gesamtNoteGerundet, 0);
      gesamtNoteGerundet = Math.Min(gesamtNoteGerundet, 15);
      var zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == gesamtNoteGerundet);
      return this.GetNotenString(zensur);
    }

    /// <summary>
    /// Berechnet die mündliche Gesamtnote aus mündlichen Teilnoten
    /// in Abhängigkeit der Teilgewichtungen.
    /// </summary>
    /// <returns>Gesamtnote für die mündliche Mitarbeit dieses Schülers
    /// im Format des Klassenstufen <see cref="Bepunktungstyp"/>s</returns>
    private string BerechneMündlicheGesamtnote()
    {
      var qualitätsNoten =
        this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichQualität);
      var qualitätsNotenDurchschnitt = this.BerechneDurchschnittsNotenwert(qualitätsNoten);
      var quantitätsNoten =
        this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichQuantität);
      var quantitätsNotenDurchschnitt = this.BerechneDurchschnittsNotenwert(quantitätsNoten);
      var sonstigeNoten =
        this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichSonstige);
      var sonstigeNotenDurchschnitt = this.BerechneDurchschnittsNotenwert(sonstigeNoten);

      var mündlichGesamt = 0;
      if (sonstigeNoten.Any())
      {
        mündlichGesamt = (int)Math.Round(
          (qualitätsNotenDurchschnitt * this.MündlicheQualitätWichtung)
          + (quantitätsNotenDurchschnitt * this.MündlicheQuantitätWichtung)
          + (sonstigeNotenDurchschnitt * this.MündlicheWeitereNotenWichtung),
          0);
      }
      else
      {
        mündlichGesamt = (int)Math.Round(
          (qualitätsNotenDurchschnitt * (this.MündlicheQualitätWichtung + this.MündlicheWeitereNotenWichtung / 2))
          + (quantitätsNotenDurchschnitt * (this.MündlicheQuantitätWichtung + this.MündlicheWeitereNotenWichtung / 2)),
          0);
      }

      // || sonstigeNotenDurchschnitt == 0)
      if (!qualitätsNoten.Any() || !quantitätsNoten.Any())
      {
        return "?";
      }

      this.mündlicheGesamtnote = mündlichGesamt;

      var zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == mündlichGesamt);
      return this.GetNotenString(zensur);
    }

    /// <summary>
    /// Berechnet die schriftliche Gesamtnote aus schriftlichen Teilnoten
    /// in Abhängigkeit der Teilgewichtungen.
    /// </summary>
    /// <returns>Gesamtnote für die schriftliche Mitarbeit dieses Schülers
    /// im Format des Klassenstufen <see cref="Bepunktungstyp"/>s</returns>
    private string BerechneSchriftlicheGesamtnote()
    {
      var klausurenNoten =
        this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichKlassenarbeit);
      var klausurNotenDurchschnitt = this.BerechneDurchschnittsNotenwert(klausurenNoten);
      var sonstigeNoten =
        this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichSonstige);
      var sonstigeNotenDurchschnitt = this.BerechneDurchschnittsNotenwert(sonstigeNoten);

      var schriftlichGesamt = 0;
      if (sonstigeNoten.Any())
      {
        schriftlichGesamt = (int)Math.Round(
         (klausurNotenDurchschnitt * this.SchriftlichKlausurenWichtung)
         + (sonstigeNotenDurchschnitt * this.SchriftlichWeitereNotenWichtung),
         0);
      }
      else
      {
        schriftlichGesamt = (int)Math.Round(
         klausurNotenDurchschnitt * (this.SchriftlichKlausurenWichtung + this.SchriftlichWeitereNotenWichtung),
         0);
      }

      if (!klausurenNoten.Any())
      {
        return "?";
      }

      this.schriftlicheGesamtnote = schriftlichGesamt;

      var zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == schriftlichGesamt);
      return this.GetNotenString(zensur);
    }

    /// <summary>
    /// Diese Funktion berechnet aus den gegebenen Noten die Durchschnittsnote
    /// unter Verwendung der Gewichtungsfaktoren jeder einzelnen Note.
    /// </summary>
    /// <param name="notenCollection">Eine Liste an NoteViewModel Noten,
    /// deren Mittelwert berechnet werden soll</param>
    /// <returns>Den gewichteten Mittelwert im Bepunktungstyp der ersten
    /// Note der Liste.</returns>
    private string BerechneDurchschnittsNote(IEnumerable<NoteViewModel> notenCollection)
    {
      var noteViewModels = notenCollection as IList<NoteViewModel> ?? notenCollection.ToList();
      if (!noteViewModels.Any())
      {
        return string.Empty;
      }

      int punkteSumme = 0;
      int count = 0;
      foreach (var noteViewModel in noteViewModels)
      {
        var wichtung = noteViewModel.NoteWichtung;
        punkteSumme += wichtung * noteViewModel.NoteZensur.ZensurNotenpunkte;
        count += wichtung;
      }

      var averageNote = (float)punkteSumme / count;
      var roundedNote = (int)Math.Round(averageNote, 0);
      var zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == roundedNote);

      return this.GetNotenString(zensur);
    }

    /// <summary>
    /// Diese Funktion berechnet aus den gegebenen Noten die Durchschnittsnote
    /// unter Verwendung der Gewichtungsfaktoren jeder einzelnen Note.
    /// </summary>
    /// <param name="notenCollection">Eine Liste an NoteViewModel Noten,
    /// deren Mittelwert berechnet werden soll</param>
    /// <returns>Den gewichteten Mittelwert in Notenpunkten.</returns>
    private int BerechneDurchschnittsNotenwert(IEnumerable<NoteViewModel> notenCollection)
    {
      var noteViewModels = notenCollection as IList<NoteViewModel> ?? notenCollection.ToList();

      if (!noteViewModels.Any())
      {
        return 0;
      }

      int punkteSumme = 0;
      int count = 0;
      foreach (var noteViewModel in noteViewModels)
      {
        var wichtung = noteViewModel.NoteWichtung;
        punkteSumme += wichtung * noteViewModel.NoteZensur.ZensurNotenpunkte;
        count += wichtung;
      }

      var averageNote = (float)punkteSumme / count;
      var roundedNote = (int)Math.Round(averageNote, 0);
      return roundedNote;
    }

    /// <summary>
    /// Diese Funktion wandelt die gegebene Zensur in einen String um,
    /// der die Note im Format der Klassenstufe (eine der <see cref="Bepunktungstyp"/> Varianten)
    /// ausgibt.
    /// </summary>
    /// <param name="zensur">Das <see cref="ZensurViewModel"/> mit der umzuwandelnden Note.</param>
    /// <returns>Die Note als String im Format der Klassenstufe.</returns>
    private string GetNotenString(ZensurViewModel zensur)
    {
      var bepunktungsTyp =
        (Bepunktungstyp)
        Enum.Parse(typeof(Bepunktungstyp), this.Model.Schülerliste.Klasse.Klassenstufe.Jahrgangsstufe.Bepunktungstyp);
      switch (bepunktungsTyp)
      {
        case Bepunktungstyp.GanzeNote:
          return zensur.ZensurGanzeNote.ToString(CultureInfo.InvariantCulture);
        case Bepunktungstyp.NoteMitTendenz:
          return zensur.ZensurNoteMitTendenz;
        case Bepunktungstyp.Notenpunkte:
          return zensur.ZensurNotenpunkte.ToString(CultureInfo.InvariantCulture);
      }

      return string.Empty;
    }

    /// <summary>
    /// Tritt auf, wenn die HausaufgabenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void HausaufgabenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Hausaufgaben", this.Hausaufgaben, e, false, "Änderung der Hausaufgaben");
      this.UpdateNoten();
    }

    /// <summary>
    /// Handles deletion of the current Hausaufgabe
    /// </summary>
    private void DeleteCurrentHausaufgabe()
    {
      this.DeleteHausaufgabe(this.CurrentHausaufgabe);
    }

    /// <summary>
    /// Löscht die gegebene nicht gemachte Hausaufgabe
    /// </summary>
    /// <param name="hausaufgabeViewModel">
    /// Die Hausaufgabe die gelöscht werden soll.
    /// </param>
    private void DeleteHausaufgabe(HausaufgabeViewModel hausaufgabeViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgabe {0} gelöscht.", hausaufgabeViewModel), false))
      {
        var success = App.MainViewModel.Hausaufgaben.RemoveTest(hausaufgabeViewModel);
        var result = this.Hausaufgaben.RemoveTest(hausaufgabeViewModel);
        this.CurrentHausaufgabe = null;
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Handles addition a new Hausaufgabe to this Schülereintrag
    /// </summary>
    private void AddHausaufgabe()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgabe erstellt."), false))
      {
        if (this.Hausaufgaben.Count == 0)
        {
          this.ErstelleHausaufgabe();
          this.RaisePropertyChanged("NichtGemachteHausaufgabenAnzahl");
          return;
        }

        var bestehendeAufgaben =
          this.Hausaufgaben.Where(
            o =>
            o.HausaufgabeBezeichnung == Selection.Instance.HausaufgabeBezeichnung
            && o.HausaufgabeDatum == Selection.Instance.HausaufgabeDatum);

        var hausaufgabeViewModels = bestehendeAufgaben as IList<HausaufgabeViewModel> ?? bestehendeAufgaben.ToList();
        if (hausaufgabeViewModels.Any())
        {
          var aufgabe = hausaufgabeViewModels[0];

          // Wenn Die Hausaufgabe vorhanden ist, wird sie entweder als 
          // nachgereicht tituliert oder gelöscht.
          if (!aufgabe.HausaufgabeIstNachgereicht)
          {
            aufgabe.HausaufgabeIstNachgereicht = true;
          }
          else
          {
            this.DeleteHausaufgabe(aufgabe);
          }
        }
        else
        {
          this.ErstelleHausaufgabe();
        }

        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Diese Methode legt eine neue Hausaufgabe in der Datenbank an.
    /// </summary>
    private void ErstelleHausaufgabe()
    {
      var hausaufgabe = new Hausaufgabe();
      hausaufgabe.Bezeichnung = Selection.Instance.HausaufgabeBezeichnung;
      hausaufgabe.Datum = Selection.Instance.HausaufgabeDatum;
      hausaufgabe.IstNachgereicht = false;
      hausaufgabe.Schülereintrag = this.Model;

      var vm = new HausaufgabeViewModel(hausaufgabe);
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgabe {0} erstellt.", vm), false))
      {
        App.MainViewModel.Hausaufgaben.Add(vm);
        this.Hausaufgaben.Add(vm);
        this.CurrentHausaufgabe = vm;

        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Tritt auf, wenn die NotenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void NotenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Noten", this.Noten, e, false, "Änderung der Noten");
      this.UpdateNoten();
    }

    /// <summary>
    /// Handles deletion of the current Note
    /// </summary>
    private void DeleteCurrentNote()
    {
      this.DeleteNote(this.CurrentNote);
    }

    /// <summary>
    /// Handles deletion of the given phase
    /// </summary>
    /// <param name="noteViewModel"> The note View Model. </param>
    private void DeleteNote(NoteViewModel noteViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Note {0} gelöscht.", noteViewModel), false))
      {
        var success = App.MainViewModel.Noten.RemoveTest(noteViewModel);
        var result = this.Noten.RemoveTest(noteViewModel);
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Handles addition a new Note to this Schülereintrag
    /// </summary>
    private void AddNote()
    {
      var note = new Note();
      note.Bezeichnung = string.Empty;
      note.Datum = DateTime.Now;
      note.IstSchriftlich = false;
      note.Notentyp = Notentyp.MündlichQualität.ToString();
      note.Wichtung = 1;
      note.Schülereintrag = this.Model;
      var vm = new NoteViewModel(note);
      var workspace = new NotenWorkspaceViewModel(vm);
      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Note {0} hinzugefügt.", vm), false))
      {
        var dlg = new AddNoteDialog(workspace);
        undo = !dlg.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
      else
      {
        App.MainViewModel.Noten.Add(vm);
        this.Noten.Add(vm);
        this.CurrentNote = vm;
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Erstellt eine mündliche 1 für Qualität.
    /// </summary>
    private void AddMündlicheQualitätsNote1()
    {
      this.AddMündlicheNote(Notentyp.MündlichQualität, 1);
    }

    /// <summary>
    /// Erstellt eine mündliche 2 für Qualität.
    /// </summary>
    private void AddMündlicheQualitätsNote2()
    {
      this.AddMündlicheNote(Notentyp.MündlichQualität, 2);
    }

    /// <summary>
    /// Erstellt eine mündliche 3 für Qualität.
    /// </summary>
    private void AddMündlicheQualitätsNote3()
    {
      this.AddMündlicheNote(Notentyp.MündlichQualität, 3);
    }

    /// <summary>
    /// Erstellt eine mündliche 4 für Qualität.
    /// </summary>
    private void AddMündlicheQualitätsNote4()
    {
      this.AddMündlicheNote(Notentyp.MündlichQualität, 4);
    }

    /// <summary>
    /// Erstellt eine mündliche 5.
    /// </summary>
    private void AddMündlicheQualitätsNote5()
    {
      this.AddMündlicheNote(Notentyp.MündlichQualität, 5);
    }

    /// <summary>
    /// Erstellt eine mündliche 6 für Qualität.
    /// </summary>
    private void AddMündlicheQualitätsNote6()
    {
      this.AddMündlicheNote(Notentyp.MündlichQualität, 6);
    }

    /// <summary>
    /// Erstellt eine mündliche 1 für Quantität.
    /// </summary>
    private void AddMündlicheQuantitätsNote1()
    {
      this.AddMündlicheNote(Notentyp.MündlichQuantität, 1);
    }

    /// <summary>
    /// Erstellt eine mündliche 2 für Quantität.
    /// </summary>
    private void AddMündlicheQuantitätsNote2()
    {
      this.AddMündlicheNote(Notentyp.MündlichQuantität, 2);
    }

    /// <summary>
    /// Erstellt eine mündliche 3 für Quantität.
    /// </summary>
    private void AddMündlicheQuantitätsNote3()
    {
      this.AddMündlicheNote(Notentyp.MündlichQuantität, 3);
    }

    /// <summary>
    /// Erstellt eine mündliche 4 für Quantität.
    /// </summary>
    private void AddMündlicheQuantitätsNote4()
    {
      this.AddMündlicheNote(Notentyp.MündlichQuantität, 4);
    }

    /// <summary>
    /// Erstellt eine mündliche 5.
    /// </summary>
    private void AddMündlicheQuantitätsNote5()
    {
      this.AddMündlicheNote(Notentyp.MündlichQuantität, 5);
    }

    /// <summary>
    /// Erstellt eine mündliche 6 für Quantität.
    /// </summary>
    private void AddMündlicheQuantitätsNote6()
    {
      this.AddMündlicheNote(Notentyp.MündlichQuantität, 6);
    }

    /// <summary>
    /// Erstellt eine neue mündliche Note mit den gegebenen Parametern
    /// </summary>
    /// <param name="notentyp">Der <see cref="Notentyp"/> der Note.</param>
    /// <param name="notenwert">Ein ganzzahliger Notenwert für die Note.</param>
    private void AddMündlicheNote(Notentyp notentyp, int notenwert)
    {
      var note = new Note();
      note.Datum = DateTime.Now;
      var stundenentwurf = Selection.Instance.Stundenentwurf;
      if (stundenentwurf != null)
      {
        note.Bezeichnung = stundenentwurf.StundenentwurfStundenthema;
        note.Datum = stundenentwurf.StundenentwurfDatum;

        var alteNote =
          this.Noten.FirstOrDefault(o => o.NoteDatum == stundenentwurf.StundenentwurfDatum & o.NoteNotentyp == notentyp);
        if (alteNote != null)
        {
          var result = this.Noten.Remove(alteNote);
        }
      }

      note.IstSchriftlich = false;
      note.Notentyp = notentyp.ToString();
      note.Wichtung = 1;
      note.Zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNoteMitTendenz == notenwert.ToString(CultureInfo.InvariantCulture)).Model;
      note.Schülereintrag = this.Model;
      var vm = new NoteViewModel(note);
      using (new UndoBatch(App.MainViewModel, string.Format("Note {0} hinzugefügt.", vm), false))
      {
        if (!this.Noten.Contains(vm))
        {
          App.MainViewModel.Noten.Add(vm);
          this.Noten.Add(vm);
        }

        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Tritt auf, wenn die NotentendenzenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void NotentendenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Notentendenzen", this.Notentendenzen, e, false, "Änderung der Notentendenzen");
      this.UpdateNoten();
    }

    /// <summary>
    /// Handles deletion of the current Notentendenz
    /// </summary>
    private void DeleteCurrentNotentendenz()
    {
      this.DeleteNotentendenz(this.CurrentNotentendenz);
    }

    /// <summary>
    /// Handles deletion of the given notentendenzViewModel
    /// </summary>
    /// <param name="notentendenzViewModel">The notentendenz View Model.</param>
    private void DeleteNotentendenz(NotentendenzViewModel notentendenzViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Notentendenz {0} hinzugefügt.", notentendenzViewModel), false))
      {
        var success = App.MainViewModel.Notentendenzen.RemoveTest(notentendenzViewModel);
        var result = this.Notentendenzen.RemoveTest(notentendenzViewModel);
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Handles addition a new Notentendenz to this Schülereintrag
    /// </summary>
    private void AddNotentendenz()
    {
      var notentendenz = new Notentendenz();
      notentendenz.Bezeichnung = string.Empty;
      notentendenz.Datum = DateTime.Now;
      notentendenz.Tendenz = App.MainViewModel.Tendenzen.First().Model;
      notentendenz.Tendenztyp = App.MainViewModel.Tendenztypen.First().Model;
      notentendenz.Schülereintrag = this.Model;
      var vm = new NotentendenzViewModel(notentendenz);
      var dlg = new NotentendenzDialog { CurrentNotentendenz = vm };

      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Notentendenz {0} hinzugefügt.", vm), false))
      {
        dlg.CurrentNotentendenz = vm;
        undo = !dlg.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
      else
      {
        vm = dlg.CurrentNotentendenz;
        App.MainViewModel.Notentendenzen.Add(vm);
        this.Notentendenzen.Add(vm);
        this.CurrentNotentendenz = vm;
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Berechnet aus den nicht gemachten und nachgereichten Hausaufgaben einen Punktabzug.
    /// Nachgereichte zählen einen Punkt, nicht nachgereichte zwei Punkte.
    /// 0/1 Punkt = kein Abzug
    /// 2/3 Punkte = ein Notenpunkt Abzug
    /// >3 Punkte = zwei Notenpunkte Abzug
    /// </summary>
    /// <returns>Ein Wert in Notenpunkten, der zur Gesamtnote hinzugerechnet werden muss (da negativ)</returns>
    private int BerechneHausaufgabenBepunktung()
    {
      var nichtGemachteHausaufgaben = this.Hausaufgaben.Count();
      var nachgereichteHausaufgaben = this.Hausaufgaben.Count(o => o.HausaufgabeIstNachgereicht);
      var nichtGemachteNichtNachgereichteHausaufgaben = nichtGemachteHausaufgaben - nachgereichteHausaufgaben;

      var bewertung = 0;
      bewertung += nichtGemachteNichtNachgereichteHausaufgaben * 2;
      bewertung += nachgereichteHausaufgaben;

      return bewertung > 6 ? -2 : bewertung < 4 ? 0 : -1;
    }

    /// <summary>
    /// Berechnete aus Tendenzen einen Punktgewinn oder -abzug.
    /// </summary>
    /// <returns>Einen Wert in Notenpunkten der zur Gesamtnote hinzugerechnet werden muss.</returns>
    private int BerechneTendenzBepunktung()
    {
      var anzahlTendenzen = this.Notentendenzen.Count();
      if (anzahlTendenzen == 0)
      {
        return 0;
      }

      var tendenzSumme = this.Notentendenzen.Sum(o => o.NotentendenzTendenz.TendenzWichtung);
      var schlüssel = (int)Math.Round(tendenzSumme / (float)anzahlTendenzen, 0);
      return schlüssel != 0 ? schlüssel * -1 + 3 : 2;
    }

  }
}
