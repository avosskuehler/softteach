namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using System.Windows.Media;

  using MahApps.Metro.Controls.Dialogs;
  using OxyPlot;
  using OxyPlot.Axes;
  using OxyPlot.Series;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Personen;
  using Selection = Setting.Selection;

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
    /// Die zuletzt berechnete Gesamtnote.
    /// </summary>
    private int gesamtnote;

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

    ///// <summary>
    ///// Das momentane Ergebnis
    ///// </summary>
    //private ErgebnisViewModel currentErgebnis;

    /// <summary>
    /// Gibt an, ob dieser Schüler zufällig ausgewählt ist.
    /// </summary>
    private bool istZufälligAusgewählt;

    /// <summary>
    /// Gibt an, ob der Schüler krank isti
    /// </summary>
    private bool istKrank;

    private int gesamtAnpassung;

    private int mündlicheAnpassung;

    private int schriftlicheAnpassung;

    private PlotModel plotModelQualität;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="SchülereintragViewModel"/> Klasse. 
    /// </summary>
    public SchülereintragViewModel()
    {
      var schülereintrag = new Schülereintrag
      {
        Lerngruppe = Selection.Instance.Lerngruppe.Model
      };
      //App.UnitOfWork.Context.Schülereinträge.Add(schülereintrag);
      this.Model = schülereintrag;
      //App.MainViewModel.Schülereinträge.Add(this);
      this.plotModelQualität = new PlotModel();

      // < oxy:DateTimeAxis FirstDateTime = "{Binding MündlicheQualitätFirstDateTime}" LastDateTime = "{Binding MündlicheQualitätLastDateTime}" />
      this.plotModelQualität.Axes.Add(new DateTimeAxis
      {
        Position = AxisPosition.Bottom,
        IntervalType = DateTimeIntervalType.Months,
        MinimumPadding = 0.1,
        MaximumPadding = 0.1,
        StringFormat = "MMMM",
      });

      this.plotModelQualität.Axes.Add(new LinearAxis
      {
        Position = AxisPosition.Left,
        TickStyle = TickStyle.None,
        MinimumPadding = 0.1,
        MaximumPadding = 0.1,
        StartPosition = 1,
        EndPosition = 0,
        Minimum = 0.5,
        Maximum = 6.5,
        MajorStep = 1,
        MinorStep = 1,
        MajorGridlineStyle = LineStyle.Solid
      });

      var s = new LineSeries
      {
        ItemsSource = this.MündlicheQualitätNotenCollection,
        DataFieldX = "NoteDatum",
        DataFieldY = "NoteZensurGanzeNote",
        MarkerType = MarkerType.Circle
      };
      this.plotModelQualität.Series.Add(s);

    }

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="SchülereintragViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schülereintrag">
    /// The underlying schülereintrag this ViewModel is to be based on
    /// </param>
    public SchülereintragViewModel(Schülereintrag schülereintrag)
    {
      this.Model = schülereintrag ?? throw new ArgumentNullException(nameof(schülereintrag));

      // Build data structures for Hausaufgaben
      this.Hausaufgaben = new ObservableCollection<HausaufgabeViewModel>();
      foreach (var hausaufgabe in schülereintrag.Hausaufgaben)
      {
        var vm = new HausaufgabeViewModel(hausaufgabe);
        //App.MainViewModel.Hausaufgaben.Add(vm);
        this.Hausaufgaben.Add(vm);
      }

      this.Hausaufgaben.CollectionChanged += this.HausaufgabenCollectionChanged;

      // Build data structures for Noten
      this.Noten = new ObservableCollection<NoteViewModel>();
      foreach (var note in schülereintrag.Noten)
      {
        var vm = new NoteViewModel(note);
        //App.MainViewModel.Noten.Add(vm);
        this.Noten.Add(vm);
      }

      this.Noten.CollectionChanged += this.NotenCollectionChanged;

      // Build data structures for Notentendenzen
      this.Notentendenzen = new ObservableCollection<NotentendenzViewModel>();
      foreach (var notentendenz in schülereintrag.Notentendenzen)
      {
        var vm = new NotentendenzViewModel(notentendenz);
        //App.MainViewModel.Notentendenzen.Add(vm);
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

      this.MarkAsKrankCommand = new DelegateCommand(this.MarkAsKrank);
      this.AddHausaufgabeCommand = new DelegateCommand(this.AddHausaufgabe);
      this.DeleteHausaufgabeCommand = new DelegateCommand(
        this.DeleteCurrentHausaufgabe,
        () => this.CurrentHausaufgabe != null);
      this.NoteGegebenCommand = new DelegateCommand(this.NoteGegeben);
      this.AddNoteCommand = new DelegateCommand(this.AddNote);
      this.DeleteNoteCommand = new DelegateCommand(this.DeleteCurrentNote, () => this.CurrentNote != null);
      this.AddNotentendenzCommand = new DelegateCommand(this.AddNotentendenz);
      this.DeleteNotentendenzCommand = new DelegateCommand(
        this.DeleteCurrentNotentendenz,
        () => this.CurrentNotentendenz != null);
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
      this.SonstigeNote1Command = new DelegateCommand(this.AddSonstigeNote1);
      this.SonstigeNote2Command = new DelegateCommand(this.AddSonstigeNote2);
      this.SonstigeNote3Command = new DelegateCommand(this.AddSonstigeNote3);
      this.SonstigeNote4Command = new DelegateCommand(this.AddSonstigeNote4);
      this.SonstigeNote5Command = new DelegateCommand(this.AddSonstigeNote5);
      this.SonstigeNote6Command = new DelegateCommand(this.AddSonstigeNote6);
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
    /// Holt den Befehl der ausgeführt wird, wenn der Schüler als krank markiert wurde
    /// </summary>
    public DelegateCommand MarkAsKrankCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl der ausgeführt wird, wenn die Note gegeben wurde
    /// </summary>
    public DelegateCommand NoteGegebenCommand { get; private set; }

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
    /// Holt den Befehl um eine eins für die sonstige Note zu geben
    /// </summary>
    public DelegateCommand SonstigeNote1Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine zwei für die sonstige Note zu geben
    /// </summary>
    public DelegateCommand SonstigeNote2Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine drei für die sonstige Note zu geben
    /// </summary>
    public DelegateCommand SonstigeNote3Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine vier für die sonstige Note zu geben
    /// </summary>
    public DelegateCommand SonstigeNote4Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine fünf für die sonstige Note zu geben
    /// </summary>
    public DelegateCommand SonstigeNote5Command { get; private set; }

    /// <summary>
    /// Holt den Befehl um eine sechs für die sonstige Note zu geben
    /// </summary>
    public DelegateCommand SonstigeNote6Command { get; private set; }

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
        this.UndoablePropertyChanging(this, nameof(SchülereintragPerson), this.person, value);
        this.person = value;
        this.Model.Person = value.Model;
        this.RaisePropertyChanged("SchülereintragPerson");
      }
    }

    /// <summary>
    /// Gets the schülereintrag sort by nachname property.
    /// </summary>
    /// <value>
    /// The schülereintrag sort by nachname property.
    /// </value>
    [DependsUpon("SchülereintragPerson")]
    public string SchülereintragSortByNachnameProperty
    {
      get
      {
        return this.Model.Person.Nachname;
      }
    }

    /// <summary>
    /// Gets the schülereintrag sort by vorname property.
    /// </summary>
    /// <value>
    /// The schülereintrag sort by vorname property.
    /// </value>
    [DependsUpon("SchülereintragPerson")]
    public string SchülereintragSortByVornameProperty
    {
      get
      {
        return this.Model.Person.Vorname;
      }
    }

    /// <summary>
    /// Gets the schülereintrag sort by gruppennummer property.
    /// </summary>
    /// <value>
    /// The schülereintrag sort by gruppennummer property.
    /// </value>
    [DependsUpon("SchülereintragPerson")]
    public int SchülereintragSortByGruppennummerProperty
    {
      get
      {
        return this.SchülereintragPerson == null ? 0 : this.SchülereintragPerson.Gruppennummer;
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
        return this.Model.Lerngruppe.Fach.Bezeichnung + ": Noten für " + this.Model.Person.Vorname + " "
               + this.Model.Person.Nachname;
      }
    }

    public PlotModel PlotModelQualität
    {
      get
      {
        return this.plotModelQualität;
      }
    }

    /// <summary>
    /// Holt die berechnete Gesamtnote.
    /// </summary>
    public string Gesamtnote
    {
      get
      {
        return this.BerechneGesamtnote();
      }
    }

    /// <summary>
    /// Holt die Gesamtnote in Punkten
    /// </summary>
    [DependsUpon("Gesamtnote")]
    public int GesamtNoteInPunkten
    {
      get
      {
        return this.gesamtnote;
      }
    }

    /// <summary>
    /// Holt oder setzt einen Anpassungwert für die Gesamtnote
    /// </summary>
    public int GesamtAnpassung
    {
      get
      {
        return this.gesamtAnpassung;
      }
      set
      {
        if (value != this.gesamtAnpassung)
        {
          this.gesamtAnpassung = value;
          this.RaisePropertyChanged("GesamtAnpassung");
          this.RaisePropertyChanged("Gesamtnote");
        }
      }
    }

    /// <summary>
    /// Holt eine Liste aller Gesamtstandnoten.
    /// </summary>
    public IEnumerable<NoteViewModel> GesamtstandNotenCollection
    {
      get
      {
        var stand = this.Noten.Where(
          o => o.NoteNotentyp == Notentyp.GesamtStand && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
        return stand;
      }
    }

    #region Mündlich

    /// <summary>
    /// Holt die Gesamtnote für alle mündlichen Leistungen.
    /// </summary>
    public string MündlicheGesamtNote
    {
      get
      {
        return this.BerechneMündlicheGesamtnote();
      }
    }

    /// <summary>
    /// Holt die Gesamtnote für alle mündlichen Leistungen in Punkten
    /// </summary>
    [DependsUpon("MündlicheGesamtNote")]
    public int MündlicheGesamtNoteInPunkten
    {
      get
      {
        return this.mündlicheGesamtnote;
      }
    }

    /// <summary>
    /// Holt oder setzt einen Anpassungswert für die mündliche Gesamtnote
    /// </summary>
    /// <value>The mündliche anpassung.</value>
    public int MündlicheAnpassung
    {
      get
      {
        return this.mündlicheAnpassung;
      }
      set
      {
        if (value != this.mündlicheAnpassung)
        {
          this.mündlicheAnpassung = value;
          this.RaisePropertyChanged("MündlicheAnpassung");
          this.RaisePropertyChanged("MündlicheGesamtNote");
        }
      }
    }

    /// <summary>
    /// Holt eine Liste aller mündlichen Noten.
    /// </summary>
    public IEnumerable<NoteViewModel> MündlicheNotenCollection
    {
      get
      {
        return this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      }
    }

    /// <summary>
    /// Holt eine Liste aller mündlichen Noten zur Qualität.
    /// </summary>
    public IEnumerable<NoteViewModel> MündlicheQualitätNotenCollection
    {
      get
      {
        return this.Noten.Where(o => o.NoteNotentyp == Notentyp.MündlichQualität && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      }
    }

    /// <summary>
    /// Holt das erste Datum der aktuellen mündlichen Qualitätsnoten
    /// </summary>
    [DependsUpon("MündlicheQualitätNotenCollection")]
    public DateTime MündlicheQualitätFirstDateTime
    {
      get
      {
        if (!this.MündlicheQualitätNotenCollection.Any())
        {
          return DateTime.Now;
        }

        var qualitätNoten = this.MündlicheQualitätNotenCollection.OrderBy(o => o.NoteDatum);
        return qualitätNoten.First().NoteDatum.AddDays(-10);
      }
    }

    /// <summary>
    /// Holt das späteste Datum der aktuellen mündlichen Qualitätsnoten
    /// </summary>
    [DependsUpon("MündlicheQualitätNotenCollection")]
    public DateTime MündlicheQualitätLastDateTime
    {
      get
      {
        if (!this.MündlicheQualitätNotenCollection.Any())
        {
          return DateTime.Now;
        }

        var qualitätNoten = this.MündlicheQualitätNotenCollection.OrderBy(o => o.NoteDatum);
        return qualitätNoten.Last().NoteDatum.AddDays(10);
      }
    }

    /// <summary>
    /// Holt das erste Datum der aktuellen mündlichen Quantitätsnoten
    /// </summary>
    [DependsUpon("MündlicheQuantitätNotenCollection")]
    public DateTime MündlicheQuantitätFirstDateTime
    {
      get
      {
        if (!this.MündlicheQuantitätNotenCollection.Any())
        {
          return DateTime.Now;
        }

        var quantitätNoten = this.MündlicheQuantitätNotenCollection.OrderBy(o => o.NoteDatum);
        return quantitätNoten.First().NoteDatum.AddDays(-10);
      }
    }

    /// <summary>
    /// Holt das späteste Datum der aktuellen mündlichen Quantitätsnoten
    /// </summary>
    [DependsUpon("MündlicheQuantitätNotenCollection")]
    public DateTime MündlicheQuantitätLastDateTime
    {
      get
      {
        if (!this.MündlicheQuantitätNotenCollection.Any())
        {
          return DateTime.Now;
        }

        var quantitätNoten = this.MündlicheQuantitätNotenCollection.OrderBy(o => o.NoteDatum);
        return quantitätNoten.Last().NoteDatum.AddDays(10);
      }
    }


    /// <summary>
    /// Holt eine Liste aller mündlichen Noten zur Quantität.
    /// </summary>
    public IEnumerable<NoteViewModel> MündlicheQuantitätNotenCollection
    {
      get
      {
        return this.Noten.Where(o => o.NoteNotentyp == Notentyp.MündlichQuantität && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      }
    }

    /// <summary>
    /// Holt die Durchschnittsnote der mündlichen Qualität.
    /// </summary>
    public string MündlicheQualitätNote
    {
      get
      {
        var qualitätsNoten =
          this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichQualität && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
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
        return this.Model.Lerngruppe.NotenWichtung.MündlichQualität;
      }
    }

    /// <summary>
    /// Holt die Durschnittsnote der mündlichen Quantität
    /// </summary>
    public string MündlicheQuantitätNote
    {
      get
      {
        var quantitätsNoten =
          this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichQuantität && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
        return this.BerechneDurchschnittsNote(quantitätsNoten);
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der mündlichen Qualität für
    /// die aktuelle Lerngruppe
    /// </summary>
    public float MündlicheQuantitätWichtung
    {
      get
      {
        return this.Model.Lerngruppe.NotenWichtung.MündlichQuantität;
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der mündlichen Note für
    /// die aktuelle Lerngruppe
    /// </summary>
    public float MündlicheNotenWichtung
    {
      get
      {
        return this.Model.Lerngruppe.NotenWichtung.MündlichGesamt;
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der schriftlichen Note für
    /// die aktuelle Lerngruppe
    /// </summary>
    public float SchriftlicheNotenWichtung
    {
      get
      {
        return this.Model.Lerngruppe.NotenWichtung.SchriftlichGesamt;
      }
    }

    /// <summary>
    /// Holt eine Liste aller mündlichen Noten, die nicht Quantität oder Qualität sind.
    /// </summary>
    public IEnumerable<NoteViewModel> MündlicheWeitereNotenCollection
    {
      get
      {
        var weitere = this.Noten.Where(
          o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichSonstige && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
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
          this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichSonstige && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
        return this.BerechneDurchschnittsNote(sonstigeNoten);
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der weiteren mündlichen Noten für
    /// die aktuelle Lerngruppe
    /// </summary>
    public float MündlicheWeitereNotenWichtung
    {
      get
      {
        return this.Model.Lerngruppe.NotenWichtung.MündlichSonstige;
      }
    }

    /// <summary>
    /// Holt eine Liste aller mündlichen Noten, die nicht Quantität oder Qualität sind.
    /// </summary>
    public IEnumerable<NoteViewModel> MündlicheStandNotenCollection
    {
      get
      {
        var stand = this.Noten.Where(
          o => o.NoteNotentyp == Notentyp.MündlichStand && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
        return stand;
      }
    }

    #endregion // mündlich

    #region Schriftlich

    /// <summary>
    /// Holt die Gesamtnote für alle schriftlichen Leistungen.
    /// </summary>
    public string SchriftlicheGesamtNote
    {
      get
      {
        return this.BerechneSchriftlicheGesamtnote();
      }
    }

    /// <summary>
    /// Holt oder setzt einen Anpassungswert für die schriftliche Gesamtnote
    /// </summary>
    public int SchriftlicheAnpassung
    {
      get
      {
        return this.schriftlicheAnpassung;
      }
      set
      {
        if (value != this.schriftlicheAnpassung)
        {
          this.schriftlicheAnpassung = value;
          this.RaisePropertyChanged("SchriftlicheAnpassung");
          this.RaisePropertyChanged("SchriftlicheGesamtNote");
        }
      }
    }

    /// <summary>
    /// Holt die Gesamtnote für alle mündlichen Leistungen in Punkten
    /// </summary>
    [DependsUpon("SchriftlicheGesamtNote")]
    public int SchriftlicheGesamtNoteInPunkten
    {
      get
      {
        return this.schriftlicheGesamtnote;
      }
    }

    /// <summary>
    /// Holt eine Liste aller Klausur und Klassenarbeitsnoten.
    /// </summary>
    public IEnumerable<NoteViewModel> SchriftlichKlausurenNotenCollection
    {
      get
      {
        return this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichKlassenarbeit && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      }
    }

    /// <summary>
    /// Holt die Durchschnittsnote der mündlichen Qualität.
    /// </summary>
    public string SchriftlichKlausurenGesamtnote
    {
      get
      {
        var klassenarbeitNoten =
          this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichKlassenarbeit && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
        return this.BerechneDurchschnittsNote(klassenarbeitNoten);
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der schriftlichen Klausuren und
    /// Klassenarbeiten für die aktuelle Lerngruppe
    /// </summary>
    public float SchriftlichKlausurenWichtung
    {
      get
      {
        return this.Model.Lerngruppe.NotenWichtung.SchriftlichKlassenarbeit;
      }
    }

    /// <summary>
    /// Holt eine Liste aller schriftlichen Noten, die nicht Quantität oder Qualität
    /// zu Klassenarbeiten oder Klausuren gehören sind.
    /// </summary>
    public IEnumerable<NoteViewModel> SchriftlichWeitereNotenCollection
    {
      get
      {
        var weitere = this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichSonstige && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
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
        var sonstigeNoten = this.Noten.Where(
          o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichSonstige && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
        return this.BerechneDurchschnittsNote(sonstigeNoten);
      }
    }

    /// <summary>
    /// Holt den Gewichtungsanteil der weiteren schriftlichen Noten für
    /// die aktuelle Lerngruppe
    /// </summary>
    public float SchriftlichWeitereNotenWichtung
    {
      get
      {
        return this.Model.Lerngruppe.NotenWichtung.SchriftlichSonstige;
      }
    }

    /// <summary>
    /// Holt eine Liste aller schritlichen Standnoten.
    /// </summary>
    public IEnumerable<NoteViewModel> SchriftlicheStandNotenCollection
    {
      get
      {
        var stand = this.Noten.Where(
          o => o.NoteNotentyp == Notentyp.SchriftlichStand && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
        return stand;
      }
    }

    #endregion // Schriftlich

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Schüler bei der Benotung zufällig ausgewählt wurde.
    /// </summary>
    public bool IstZufälligAusgewählt
    {
      get
      {
        return this.istZufälligAusgewählt;
      }

      set
      {
        this.istZufälligAusgewählt = value;
        this.RaisePropertyChanged("IstZufälligAusgewählt");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Schüler krank ist.
    /// </summary>
    public bool IstKrank
    {
      get
      {
        return this.istKrank;
      }

      set
      {
        this.istKrank = value;
        this.RaisePropertyChanged("IstKrank");
      }
    }

    /// <summary>
    /// Holt die Hintergrundfarbe für die Notengebungsseite
    /// </summary>
    [DependsUpon("IstZufälligAusgewählt")]
    public Color NotengebungsHintergrund
    {
      get
      {
        if (this.IstZufälligAusgewählt)
        {
          return Colors.ForestGreen;
        }

        // Anzahl der Noten des letzten Monats ermitteln
        var von = DateTime.Now.AddDays(-31);
        var qualitätsNotenzahl = this.Noten.Count(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichQualität && o.NoteDatum > von);
        var quantitätsNotenzahl = this.Noten.Count(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichQuantität && o.NoteDatum > von);
        if (qualitätsNotenzahl + quantitätsNotenzahl <= 1)
        {
          return Colors.OrangeRed;
        }

        return Colors.LightSteelBlue;
      }
    }

    /// <summary>
    /// Holt das passende Pfeilbild zur Tendenz der gemachten Hausaufgaben.
    /// </summary>
    public Style HausaufgabenTendenzStyle
    {
      get
      {
        switch (this.BerechneHausaufgabenBepunktung())
        {
          default:
          case 0:
            return App.GetIconStyle("PfeilRechts32");
          case -1:
            return App.GetIconStyle("PfeilRechtsUnten32");
          case -2:
            return App.GetIconStyle("PfeilUnten32");
        }
      }
    }

    /// <summary>
    /// Holt ein Bild, dass die Notentendenzen zusammenfasst.
    /// </summary>
    public Style TendenzenTendenzStyle
    {
      get
      {
        switch (this.BerechneTendenzBepunktung())
        {
          case 2:
            return App.GetIconStyle("PfeilOben32");
          case 1:
            return App.GetIconStyle("PfeilRechtsOben32");
          default:
          case 0:
            return App.GetIconStyle("PfeilRechts32");
          case -1:
            return App.GetIconStyle("PfeilRechtsUnten32");
          case -2:
            return App.GetIconStyle("PfeilUnten32");
        }
      }
    }

    /// <summary>
    /// Holt die Anzahl nicht gemachter Hausaufgaben
    /// </summary>
    public string NichtgemachteHausaufgaben
    {
      get
      {
        return "nicht gemacht: " + this.Hausaufgaben.Count();
      }
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
    public double CurrentArbeitPunktsumme
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
          using (new UndoBatch(App.MainViewModel, string.Format("Note angelegt"), false))
          {

            var note = new Note
            {
              Arbeit = Selection.Instance.Arbeit.Model,
              Bezeichnung = Selection.Instance.Arbeit.ArbeitBezeichnung,
              Datum = Selection.Instance.Arbeit.ArbeitDatum,
              IstSchriftlich = true,
              Notentyp = Selection.Instance.Arbeit.ArbeitIstKlausur
                              ? Notentyp.SchriftlichKlassenarbeit
                              : Notentyp.SchriftlichSonstige,
              Wichtung = 1,
              Zensur = zensur.Model,
              Schülereintrag = this.Model
            };
            //App.UnitOfWork.Context.Noten.Add(note);
            var vm = new NoteViewModel(note);
            //App.MainViewModel.Noten.Add(vm);
            this.Noten.Add(vm);
            this.currentNote = vm;
          }
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

      this.RaisePropertyChanged("Hausaufgaben");
      this.RaisePropertyChanged("NachgereichteHausaufgaben");
      this.RaisePropertyChanged("NichtgemachteHausaufgaben");
      this.RaisePropertyChanged("NichtGemachteHausaufgabenAnzahl");
      this.RaisePropertyChanged("HausaufgabenTendenzStyle");

      this.RaisePropertyChanged("Notentendenzen");
      this.RaisePropertyChanged("TendenzenTendenzStyle");

      this.RaisePropertyChanged("MündlicheStandNotenCollection");
      this.RaisePropertyChanged("MündlicheNotenCollection");
      this.RaisePropertyChanged("MündlicheQualitätNotenCollection");
      this.RaisePropertyChanged("MündlicheQuantitätNotenCollection");
      this.RaisePropertyChanged("MündlicheQualitätNote");
      this.RaisePropertyChanged("MündlicheQuantitätNote");
      this.RaisePropertyChanged("MündlicheWeitereNotenCollection");
      this.RaisePropertyChanged("MündlichWeitereNotenGesamtnote");
      this.RaisePropertyChanged("MündlicheGesamtNote");

      this.RaisePropertyChanged("SchriftlicheStandNotenCollection");
      this.RaisePropertyChanged("SchriftlichWeitereNotenCollection");
      this.RaisePropertyChanged("SchriftlichWeitereNotenGesamtnote");
      this.RaisePropertyChanged("SchriftlichKlausurenNotenCollection");
      this.RaisePropertyChanged("SchriftlichKlausurenGesamtnote");
      this.RaisePropertyChanged("SchriftlicheGesamtnote");

      this.RaisePropertyChanged("GesamtstandNotenCollection");
      this.RaisePropertyChanged("Gesamtnote");
    }

    /// <summary>
    /// Mit dieser Methode werden alle nachrangig berechneten Arbeitsproperties 
    /// aktualisiert.
    /// </summary>
    public void UpdateErgebnisse()
    {
      this.CurrentArbeitErgebnisse.Clear();
      foreach (
        var ergebnis in
          this.Ergebnisse.Where(ergebnis => ergebnis.Model.Aufgabe.Arbeit == Selection.Instance.Arbeit.Model))
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

      return string.Compare(
        this.Model.Person.Vorname,
        compareSchülereintrag.Model.Person.Vorname,
        StringComparison.Ordinal);
    }

    /// <summary>
    /// Tritt auf, wenn die ErgebnisseCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void ErgebnisseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(Ergebnisse), this.Ergebnisse, e, true, "Änderung der Ergebnisse");
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

      var mündlicheWichtung = this.Model.Lerngruppe.NotenWichtung.MündlichGesamt;
      var schriftlichWichtung = this.Model.Lerngruppe.NotenWichtung.SchriftlichGesamt;
      var localNote = (this.mündlicheGesamtnote * mündlicheWichtung)
                       + (this.schriftlicheGesamtnote * schriftlichWichtung);
      localNote = (int)Math.Round(localNote, 0);
      localNote += this.BerechneHausaufgabenBepunktung();
      localNote += this.BerechneTendenzBepunktung();
      localNote = Math.Max(localNote, 0);
      localNote = Math.Min(localNote, 15);

      var rechnerischeNote = (int)localNote;
      var rechnerischeZensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == rechnerischeNote);

      this.gesamtnote = (int)localNote + this.GesamtAnpassung;

      if (this.gesamtnote > 15)
      {
        this.gesamtnote = 15;
      }
      else if (this.gesamtnote < 0)
      {
        this.gesamtnote = 0;
      }

      var zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == this.gesamtnote);

      if (rechnerischeNote != this.gesamtnote)
      {
        return this.GetNotenString(zensur) + "(" + this.GetNotenString(rechnerischeZensur) + ")";
      }

      return this.GetNotenString(zensur);
    }

    public void AnpassungenAuslesen()
    {
      if (Selection.Instance.Lerngruppe == null)
      {
        return;
      }

      this.gesamtAnpassung = 0;
      this.mündlicheAnpassung = 0;
      this.schriftlicheAnpassung = 0;

      this.BerechneMündlicheGesamtnote();
      var terminNoten =
        this.Noten.Where(
          o =>
          o.NoteTermintyp != NotenTermintyp.Einzeln
          && o.NoteDatum.Date == Selection.Instance.Lerngruppe.NotenDatum.Date
          && o.NoteNotentyp == Notentyp.MündlichStand).OrderBy(o => o.NoteDatum);

      if (terminNoten.Any())
      {
        this.MündlicheAnpassung = terminNoten.Last().NoteZensur.ZensurNotenpunkte - this.mündlicheGesamtnote;
      }

      this.BerechneSchriftlicheGesamtnote();
      terminNoten =
        this.Noten.Where(
        o => o.NoteTermintyp != NotenTermintyp.Einzeln
          && o.NoteDatum.Date == Selection.Instance.Lerngruppe.NotenDatum.Date
          && o.NoteNotentyp == Notentyp.SchriftlichStand).OrderBy(o => o.NoteDatum);
      if (terminNoten.Any())
      {
        this.SchriftlicheAnpassung = terminNoten.Last().NoteZensur.ZensurNotenpunkte - this.schriftlicheGesamtnote;
      }

      this.BerechneGesamtnote();
      terminNoten = this.Noten.Where(
        o => o.NoteTermintyp != NotenTermintyp.Einzeln
          && o.NoteDatum.Date == Selection.Instance.Lerngruppe.NotenDatum.Date
          && o.NoteNotentyp == Notentyp.GesamtStand).OrderBy(o => o.NoteDatum);

      if (terminNoten.Any())
      {
        var anpassung = terminNoten.Last().NoteZensur.ZensurNotenpunkte - this.gesamtnote;
        if (anpassung != this.gesamtAnpassung)
        {
          this.GesamtAnpassung = anpassung;
        }
      }
    }

    /// <summary>
    /// Berechnet die mündliche Gesamtnote aus mündlichen Teilnoten
    /// in Abhängigkeit der Teilgewichtungen.
    /// </summary>
    /// <returns>Gesamtnote für die mündliche Mitarbeit dieses Schülers
    /// im Format des Klassenstufen <see cref="Bepunktungstyp"/>s</returns>
    private string BerechneMündlicheGesamtnote()
    {
      if (Selection.Instance.Lerngruppe == null)
      {
        return string.Empty;
      }

      var qualitätsNoten =
        this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichQualität && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      var qualitätsNotenDurchschnitt = BerechneDurchschnittsNotenwert(qualitätsNoten);
      var quantitätsNoten =
        this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichQuantität && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      var quantitätsNotenDurchschnitt = BerechneDurchschnittsNotenwert(quantitätsNoten);
      var sonstigeNoten =
        this.Noten.Where(o => o.NoteIstSchriftlich == false && o.NoteNotentyp == Notentyp.MündlichSonstige && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      var sonstigeNotenDurchschnitt = BerechneDurchschnittsNotenwert(sonstigeNoten);

      var mündlichGesamt = 0;
      if (sonstigeNoten.Any())
      {
        mündlichGesamt =
          (int)
          Math.Round(
            (qualitätsNotenDurchschnitt * this.MündlicheQualitätWichtung)
            + (quantitätsNotenDurchschnitt * this.MündlicheQuantitätWichtung)
            + (sonstigeNotenDurchschnitt * this.MündlicheWeitereNotenWichtung),
            0);
      }
      else
      {
        mündlichGesamt =
          (int)
          Math.Round(
            (qualitätsNotenDurchschnitt * (this.MündlicheQualitätWichtung + this.MündlicheWeitereNotenWichtung / 2))
            + (quantitätsNotenDurchschnitt * (this.MündlicheQuantitätWichtung + this.MündlicheWeitereNotenWichtung / 2)),
            0);
      }

      // || sonstigeNotenDurchschnitt == 0)
      if (!qualitätsNoten.Any() || !quantitätsNoten.Any())
      {
        return "?";
      }

      var rechnerischeNote = (int)mündlichGesamt;
      var rechnerischeZensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == rechnerischeNote);


      this.mündlicheGesamtnote = mündlichGesamt + this.MündlicheAnpassung;
      if (this.mündlicheGesamtnote > 15)
      {
        this.mündlicheGesamtnote = 15;
      }
      else if (this.mündlicheGesamtnote < 0)
      {
        this.mündlicheGesamtnote = 0;
      }

      var zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == this.mündlicheGesamtnote);

      if (rechnerischeNote != this.mündlicheGesamtnote)
      {
        return this.GetNotenString(zensur) + "(" + this.GetNotenString(rechnerischeZensur) + ")";
      }

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
        this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichKlassenarbeit && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      var klausurNotenDurchschnitt = BerechneDurchschnittsNotenwert(klausurenNoten);
      var sonstigeNoten = this.Noten.Where(o => o.NoteIstSchriftlich && o.NoteNotentyp == Notentyp.SchriftlichSonstige && o.NoteDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      var sonstigeNotenDurchschnitt = BerechneDurchschnittsNotenwert(sonstigeNoten);

      var schriftlichGesamt = 0;
      if (sonstigeNoten.Any())
      {
        schriftlichGesamt =
          (int)
          Math.Round(
            (klausurNotenDurchschnitt * this.SchriftlichKlausurenWichtung)
            + (sonstigeNotenDurchschnitt * this.SchriftlichWeitereNotenWichtung),
            0);
      }
      else
      {
        schriftlichGesamt =
          (int)
          Math.Round(
            klausurNotenDurchschnitt * (this.SchriftlichKlausurenWichtung + this.SchriftlichWeitereNotenWichtung),
            0);
      }

      if (!klausurenNoten.Any())
      {
        return "?";
      }

      var rechnerischeNote = (int)schriftlichGesamt;
      var rechnerischeZensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == rechnerischeNote);

      this.schriftlicheGesamtnote = schriftlichGesamt + this.SchriftlicheAnpassung;
      if (this.schriftlicheGesamtnote > 15)
      {
        this.schriftlicheGesamtnote = 15;
      }
      else if (this.schriftlicheGesamtnote < 0)
      {
        this.schriftlicheGesamtnote = 0;
      }

      var zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == this.schriftlicheGesamtnote);

      if (rechnerischeNote != this.schriftlicheGesamtnote)
      {
        return this.GetNotenString(zensur) + "(" + this.GetNotenString(rechnerischeZensur) + ")";
      }

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
        if (wichtung == 0) wichtung = 1;
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
    private static int BerechneDurchschnittsNotenwert(IEnumerable<NoteViewModel> notenCollection)
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
        if (wichtung == 0) wichtung = 1;
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
      switch (this.Model.Lerngruppe.Bepunktungstyp)
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
      UndoableCollectionChanged(this, nameof(Hausaufgaben), this.Hausaufgaben, e, true, "Änderung der Hausaufgaben");
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
    public void DeleteHausaufgabe(HausaufgabeViewModel hausaufgabeViewModel)
    {
      if (hausaufgabeViewModel == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgabe {0} gelöscht.", hausaufgabeViewModel), false))
      {
        //App.UnitOfWork.Context.Hausaufgaben.Remove(hausaufgabeViewModel.Model);
        //var success = App.MainViewModel.Hausaufgaben.RemoveTest(hausaufgabeViewModel);
        var result = this.Hausaufgaben.RemoveTest(hausaufgabeViewModel);
        this.CurrentHausaufgabe = null;
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Markiert diesen Schüler als krank
    /// </summary>
    private void MarkAsKrank()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Als krank markiert."), false))
      {
        this.IstKrank = !this.IstKrank;
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
    /// Fügt eine bestehende Hausaufgabe zum Schülereintrag hinzu.
    /// </summary>
    /// <param name="hausaufgabeViewModel">Die hinzuzufügende Hausaufgabe.</param>
    public void AddHausaufgabe(HausaufgabeViewModel hausaufgabeViewModel)
    {
      hausaufgabeViewModel.Model.Schülereintrag = this.Model;
      this.Hausaufgaben.Add(hausaufgabeViewModel);
    }

    /// <summary>
    /// Diese Methode legt eine e Hausaufgabe in der Datenbank an.
    /// </summary>
    private void ErstelleHausaufgabe()
    {
      var hausaufgabe = new Hausaufgabe
      {
        Bezeichnung = Selection.Instance.HausaufgabeBezeichnung,
        Datum = Selection.Instance.HausaufgabeDatum,
        IstNachgereicht = false,
        Schülereintrag = this.Model
      };

      var vm = new HausaufgabeViewModel(hausaufgabe);
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgabe {0} erstellt.", vm), false))
      {
        //App.MainViewModel.Hausaufgaben.Add(vm);
        //App.UnitOfWork.Context.Hausaufgaben.Add(hausaufgabe);
        this.Hausaufgaben.Add(vm);
        this.CurrentHausaufgabe = vm;

        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Fügt ein bestehendes Ergebnis zum Schülereintrag hinzu.
    /// </summary>
    /// <param name="ergebnisViewModel">Das hinzuzufügende Ergebnis.</param>
    public void AddErgebnis(ErgebnisViewModel ergebnisViewModel)
    {
      ergebnisViewModel.Model.Schülereintrag = this.Model;
      this.Ergebnisse.Add(ergebnisViewModel);
    }

    /// <summary>
    /// Tritt auf, wenn die NotenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void NotenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(Noten), this.Noten, e, true, "Änderung der Noten");
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
    public void DeleteNote(NoteViewModel noteViewModel)
    {
      if (noteViewModel == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Note {0} gelöscht.", noteViewModel), false))
      {
        //App.UnitOfWork.Context.Noten.Remove(noteViewModel.Model);
        //var success = App.MainViewModel.Noten.RemoveTest(noteViewModel);
        var result = this.Noten.RemoveTest(noteViewModel);
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Führt Operationen aus, nachdem die Note gegeben wurde.
    /// </summary>
    private void NoteGegeben()
    {
      this.IstZufälligAusgewählt = false;
    }

    /// <summary>
    /// Handles addition a new Note to this Schülereintrag
    /// </summary>
    private async void AddNote()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Note ergänzt"), false))
      {

        var note = new Note
        {
          Bezeichnung = string.Empty,
          Datum = DateTime.Now,
          IstSchriftlich = false,
          Notentyp = Notentyp.MündlichQualität,
          NotenTermintyp = NotenTermintyp.Einzeln,
          Wichtung = 1,
          Zensur = App.MainViewModel.Zensuren.FirstOrDefault().Model,
          Schülereintrag = this.Model
        };
        //App.UnitOfWork.Context.Noten.Add(note);
        var vm = new NoteViewModel(note);
        var workspace = new NotenWorkspaceViewModel(vm);
        bool undo;
        using (new UndoBatch(App.MainViewModel, string.Format("Note {0} hinzugefügt.", vm), false))
        {
          if (Configuration.Instance.IsMetroMode)
          {
            var metroWindow = Configuration.Instance.MetroWindow;
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            var dialog = new MetroNoteDialog(workspace);
            await metroWindow.ShowMetroDialogAsync(dialog);
            undo = false;
          }
          else
          {
            var dlg = new AddNoteDialog(workspace);
            undo = !dlg.ShowDialog().GetValueOrDefault(false);
          }
        }

        if (undo)
        {
          App.MainViewModel.ExecuteUndoCommand();
        }
        else
        {
          //App.MainViewModel.Noten.Add(vm);
          this.Noten.Add(vm);
          this.CurrentNote = vm;
          this.UpdateNoten();
        }
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
    /// Erstellt eine mündliche 1 für die sonstige Note
    /// </summary>
    private void AddSonstigeNote1()
    {
      this.AddSonstigeNote(1);
    }

    /// <summary>
    /// Erstellt eine mündliche 2 für die sonstige Note
    /// </summary>
    private void AddSonstigeNote2()
    {
      this.AddSonstigeNote(2);
    }

    /// <summary>
    /// Erstellt eine mündliche 3 für die sonstige Note
    /// </summary>
    private void AddSonstigeNote3()
    {
      this.AddSonstigeNote(3);
    }

    /// <summary>
    /// Erstellt eine mündliche 4 für die sonstige Note
    /// </summary>
    private void AddSonstigeNote4()
    {
      this.AddSonstigeNote(4);
    }

    /// <summary>
    /// Erstellt eine mündliche 5 für die sonstige Note
    /// </summary>
    private void AddSonstigeNote5()
    {
      this.AddSonstigeNote(5);
    }

    /// <summary>
    /// Erstellt eine mündliche 6 für die sonstige Note
    /// </summary>
    private void AddSonstigeNote6()
    {
      this.AddSonstigeNote(6);
    }

    /// <summary>
    /// Erstellt eine e sonstige Note mit den gegebenen Parametern
    /// </summary>
    /// <param name="notenwert">Ein ganzzahliger Notenwert für die Note.</param>
    private void AddSonstigeNote(int notenwert)
    {
      var note = new Note
      {
        Datum = Selection.Instance.SonstigeNoteDatum,
        Bezeichnung = Selection.Instance.SonstigeNoteBezeichnung,
        IstSchriftlich = Selection.Instance.SonstigeNoteNotentyp != Notentyp.MündlichSonstige,
        Notentyp = Selection.Instance.SonstigeNoteNotentyp,
        NotenTermintyp = NotenTermintyp.Einzeln,
        Wichtung = Selection.Instance.SonstigeNoteWichtung,
        Zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNoteMitTendenz == notenwert.ToString(CultureInfo.InvariantCulture)).Model,
        Schülereintrag = this.Model
      };
      var vm = new NoteViewModel(note);
      using (new UndoBatch(App.MainViewModel, string.Format("Note {0} hinzugefügt.", vm), false))
      {
        if (!this.Noten.Contains(vm))
        {
          //App.UnitOfWork.Context.Noten.Add(note);
          //App.MainViewModel.Noten.Add(vm);
          this.Noten.Add(vm);
        }

        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Erstellt eine e mündliche Note mit den gegebenen Parametern
    /// </summary>
    /// <param name="notentyp">Der <see cref="Notentyp"/> der Note.</param>
    /// <param name="notenwert">Ein ganzzahliger Notenwert für die Note.</param>
    private void AddMündlicheNote(Notentyp notentyp, int notenwert)
    {
      var note = new Note
      {
        Datum = DateTime.Now
      };
      var stunde = Selection.Instance.Stunde;
      if (stunde != null)
      {
        note.Bezeichnung = stunde.TerminBeschreibung;
        note.Datum = stunde.LerngruppenterminDatum;

        var alteNote = this.Noten.FirstOrDefault(o => o.NoteDatum == stunde.LerngruppenterminDatum & o.NoteNotentyp == notentyp);
        if (alteNote != null)
        {
          var result = this.Noten.Remove(alteNote);
        }
      }

      note.IstSchriftlich = false;
      note.Notentyp = notentyp;
      note.NotenTermintyp = NotenTermintyp.Einzeln;
      note.Wichtung = 1;
      note.Zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNoteMitTendenz == notenwert.ToString(CultureInfo.InvariantCulture)).Model;
      note.Schülereintrag = this.Model;
      var vm = new NoteViewModel(note);
      using (new UndoBatch(App.MainViewModel, string.Format("Note {0} hinzugefügt.", vm), false))
      {
        if (!this.Noten.Contains(vm))
        {
          //App.UnitOfWork.Context.Noten.Add(note);
          //App.MainViewModel.Noten.Add(vm);
          this.Noten.Add(vm);
        }

        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Erstellt eine e Notenlistennote mit den gegebenen Parametern
    /// </summary>
    /// <param name="notentyp">Der <see cref="Notentyp"/> der Note.</param>
    /// <param name="notenwert">Ein ganzzahliger Notenwert in Notenpunkte für die Note.</param>
    public void AddNote(Notentyp notentyp, NotenTermintyp termintyp, int notenwert, DateTime datum)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Note ergänzt"), false))
      {

        var note = new Note
        {
          Datum = datum,
          Bezeichnung = termintyp.ToString()
        };

        var alteNote =
          this.Noten.FirstOrDefault(o => o.NoteDatum == datum && o.NoteNotentyp == notentyp && o.NoteTermintyp == termintyp);
        if (alteNote != null)
        {
          //App.UnitOfWork.Context.Noten.Remove(alteNote.Model);
          var result = this.Noten.RemoveTest(alteNote);
          //App.MainViewModel.Noten.RemoveTest(alteNote);
        }

        switch (notentyp)
        {
          case Notentyp.MündlichStand:
            note.IstSchriftlich = false;
            break;
          case Notentyp.SchriftlichStand:
            note.IstSchriftlich = true;
            break;
          case Notentyp.GesamtStand:
            note.IstSchriftlich = false;
            break;
        }
        note.Notentyp = notentyp;
        note.NotenTermintyp = termintyp;
        note.Wichtung = 1;
        note.Zensur = App.MainViewModel.Zensuren.First(o => o.ZensurNotenpunkte == notenwert).Model;
        note.Schülereintrag = this.Model;
        var vm = new NoteViewModel(note);

        if (!this.Noten.Contains(vm))
        {
          //App.UnitOfWork.Context.Noten.Add(note);
          //App.MainViewModel.Noten.Add(vm);
          this.Noten.Add(vm);
        }

        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Fügt eine bestehende Note zum Schülereintrag hinzu.
    /// </summary>
    /// <param name="noteViewModel">The note view model.</param>
    public void AddNote(NoteViewModel noteViewModel)
    {
      noteViewModel.Model.Schülereintrag = this.Model;
      this.Noten.Add(noteViewModel);
    }

    /// <summary>
    /// Tritt auf, wenn die NotentendenzenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void NotentendenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(Notentendenzen), this.Notentendenzen, e, true, "Änderung der Notentendenzen");
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
    public void DeleteNotentendenz(NotentendenzViewModel notentendenzViewModel)
    {
      if (notentendenzViewModel == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Notentendenz {0} gelöscht.", notentendenzViewModel), false))
      {
        //App.UnitOfWork.Context.Notentendenzen.Remove(notentendenzViewModel.Model);
        //var success = App.MainViewModel.Notentendenzen.RemoveTest(notentendenzViewModel);
        var result = this.Notentendenzen.RemoveTest(notentendenzViewModel);
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Handles addition a new Notentendenz to this Schülereintrag
    /// </summary>
    private async void AddNotentendenz()
    {
      var notentendenz = new Notentendenz
      {
        Bezeichnung = string.Empty,
        Datum = DateTime.Now,
        Tendenz = Tendenz.Null,
        Tendenztyp = Tendenztyp.Leistung,
        Schülereintrag = this.Model
      };
      var vm = new NotentendenzViewModel(notentendenz);

      bool undo;
      if (Configuration.Instance.IsMetroMode)
      {
        var metroWindow = Configuration.Instance.MetroWindow;
        metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
        var dialog = new MetroNotentendenzDialog(vm);
        await metroWindow.ShowMetroDialogAsync(dialog);
        undo = false;
      }
      else
      {
        var dlg = new NotentendenzDialog { CurrentNotentendenz = vm };
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
        }
      }

      if (!undo)
      {
        //App.UnitOfWork.Context.Notentendenzen.Add(notentendenz);
        //App.MainViewModel.Notentendenzen.Add(vm);
        this.Notentendenzen.Add(vm);
        this.CurrentNotentendenz = vm;
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Fügt eine bestehende Notentendenz zum Schülereintrag hinzu.
    /// </summary>
    /// <param name="notentendenzViewModel">The note view model.</param>
    public void AddNotentendenz(NotentendenzViewModel notentendenzViewModel)
    {
      notentendenzViewModel.Model.Schülereintrag = this.Model;
      this.Notentendenzen.Add(notentendenzViewModel);
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
      var nichtGemachteHausaufgaben = this.Hausaufgaben.Count(o => o.HausaufgabeDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      var nachgereichteHausaufgaben = this.Hausaufgaben.Count(o => o.HausaufgabeDatum <= Selection.Instance.Lerngruppe.NotenDatum && o.HausaufgabeIstNachgereicht);
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
      var anzahlTendenzen = this.Notentendenzen.Count(o => o.NotentendenzDatum <= Selection.Instance.Lerngruppe.NotenDatum);
      if (anzahlTendenzen == 0)
      {
        return 0;
      }

      var tendenzSumme = this.Notentendenzen.Sum(o => (int)o.NotentendenzTendenz);
      var schlüssel = (int)Math.Round(tendenzSumme / (float)anzahlTendenzen, 0);
      return schlüssel != 0 ? schlüssel * -1 + 3 : 2;
    }
  }
}
