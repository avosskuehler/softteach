namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual arbeit
  /// </summary>
  public class ArbeitViewModel : ViewModelBase
  {
    /// <summary>
    /// Die Anzahl der Einsen dieser Arbeit
    /// </summary>
    private int anzahlNoten1;

    /// <summary>
    /// Die Anzahl der Zweien dieser Arbeit
    /// </summary>
    private int anzahlNoten2;

    /// <summary>
    /// Die Anzahl der Dreien dieser Arbeit
    /// </summary>
    private int anzahlNoten3;

    /// <summary>
    /// Die Anzahl der Vieren dieser Arbeit
    /// </summary>
    private int anzahlNoten4;

    /// <summary>
    /// Die Anzahl der Fünfen dieser Arbeit
    /// </summary>
    private int anzahlNoten5;

    /// <summary>
    /// Die Anzahl der Sechsen dieser Arbeit
    /// </summary>
    private int anzahlNoten6;

    /// <summary>
    /// Der Notendurchschnitt dieser Arbeit
    /// </summary>
    private float notendurchschnitt;

    /// <summary>
    /// The fach currently assigned to this arbeit
    /// </summary>
    private FachViewModel fach;

    /// <summary>
    /// The jahrtyp currently assigned to this arbeit
    /// </summary>
    private JahrtypViewModel jahrtyp;

    /// <summary>
    /// The schuljahr currently assigned to this arbeit
    /// </summary>
    private HalbjahrtypViewModel halbjahrtyp;

    /// <summary>
    /// The klasse currently assigned to this arbeit
    /// </summary>
    private KlasseViewModel klasse;

    /// <summary>
    /// Das Bewertungsschema für diese Arbeit.
    /// </summary>
    private BewertungsschemaViewModel bewertungsschema;

    /// <summary>
    /// Die momentan ausgewählte Aufgabe
    /// </summary>
    private AufgabeViewModel currentAufgabe;

    /// <summary>
    /// Der momentan ausgewählte Schülereintrag
    /// </summary>
    private SchülereintragViewModel currentSchülereintrag;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ArbeitViewModel"/> Klasse. 
    /// </summary>
    /// <param name="arbeit">
    /// The underlying arbeit this ViewModel is to be based on
    /// </param>
    public ArbeitViewModel(Arbeit arbeit)
    {
      if (arbeit == null)
      {
        throw new ArgumentNullException("arbeit");
      }

      this.Model = arbeit;

      this.AddAufgabeCommand = new DelegateCommand(this.AddAufgabe);
      this.EditAufgabeCommand = new DelegateCommand(this.EditAufgabe);
      this.DeleteAufgabeCommand = new DelegateCommand(this.DeleteCurrentAufgabe, () => this.CurrentAufgabe != null);
      this.PrintNotenlisteCommand = new DelegateCommand(this.PrintNotenliste);

      // Build data structures for phasen
      this.Aufgaben = new ObservableCollection<AufgabeViewModel>();
      foreach (var aufgabe in arbeit.Aufgaben.OrderBy(o => o.LfdNr))
      {
        var vm = new AufgabeViewModel(aufgabe);
        //App.MainViewModel.Aufgaben.Add(vm);
        vm.PropertyChanged += this.AufgabePropertyChanged;
        this.Aufgaben.Add(vm);
      }

      this.Aufgaben.CollectionChanged += this.AufgabenCollectionChanged;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Email address
    /// </summary>
    public DelegateCommand AddAufgabeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Bearbeitung eine Aufgabe
    /// </summary>
    public DelegateCommand EditAufgabeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current employee
    /// </summary>
    public DelegateCommand DeleteAufgabeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um die Notenliste der aktuellen Arbeit auszudrucken
    /// </summary>
    public DelegateCommand PrintNotenlisteCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Arbeit this ViewModel is based on
    /// </summary>
    public Arbeit Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Aufgaben dieser Arbeit
    /// </summary>
    public ObservableCollection<AufgabeViewModel> Aufgaben { get; set; }

    /// <summary>
    /// Holt oder setzt die currently selected phase
    /// </summary>
    public AufgabeViewModel CurrentAufgabe
    {
      get
      {
        return this.currentAufgabe;
      }

      set
      {
        this.currentAufgabe = value;
        this.RaisePropertyChanged("Aufgabe");
        this.DeleteAufgabeCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt den aktuell ausgewählten Schülereintrag
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
        this.RaisePropertyChanged("CurrentSchülereintrag");
        Selection.Instance.Schülereintrag = value;
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string ArbeitBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "ArbeitBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("ArbeitBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die LfdNr
    /// </summary>
    public int ArbeitLfdNr
    {
      get
      {
        return this.Model.LfdNr;
      }

      set
      {
        if (value == this.Model.LfdNr) return;
        this.UndoablePropertyChanging(this, "ArbeitLfdNr", this.Model.LfdNr, value);
        this.Model.LfdNr = value;
        this.RaisePropertyChanged("ArbeitLfdNr");
      }
    }

    /// <summary>
    /// Holt oder setzt die Datum
    /// </summary>
    public DateTime ArbeitDatum
    {
      get
      {
        return this.Model.Datum;
      }

      set
      {
        if (value == this.Model.Datum) return;
        this.UndoablePropertyChanging(this, "ArbeitDatum", this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("ArbeitDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob diese Arbeit eine Klassenarbeit
    /// oder Klausur ist (und daher nicht zu den sonstigen schriftlichen Arbeiten
    /// gezählt wird).
    /// </summary>
    public bool ArbeitIstKlausur
    {
      get
      {
        return this.Model.IstKlausur;
      }

      set
      {
        if (value == this.Model.IstKlausur) return;
        this.UndoablePropertyChanging(this, "ArbeitIstKlausur", this.Model.IstKlausur, value);
        this.Model.IstKlausur = value;
        this.RaisePropertyChanged("ArbeitIstKlausur");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Arbeit
    /// </summary>
    public FachViewModel ArbeitFach
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Fach == null)
        {
          return null;
        }

        if (this.fach == null || this.fach.Model != this.Model.Fach)
        {
          this.fach = App.MainViewModel.Fächer.SingleOrDefault(d => d.Model == this.Model.Fach);
        }

        return this.fach;
      }

      set
      {
        if (value.FachBezeichnung == this.fach.FachBezeichnung) return;
        this.UndoablePropertyChanging(this, "ArbeitFach", this.fach, value);
        this.fach = value;
        this.Model.Fach = value.Model;
        this.RaisePropertyChanged("ArbeitFach");
      }
    }

    /// <summary>
    /// Holt oder setzt die Jahrtyp currently assigned to this Arbeit
    /// </summary>
    public JahrtypViewModel ArbeitJahrtyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Jahrtyp == null)
        {
          return null;
        }

        if (this.jahrtyp == null || this.jahrtyp.Model != this.Model.Jahrtyp)
        {
          this.jahrtyp =
            App.MainViewModel.Jahrtypen.SingleOrDefault(d => d.Model == this.Model.Jahrtyp);
        }

        return this.jahrtyp;
      }

      set
      {
        if (value.JahrtypBezeichnung == this.jahrtyp.JahrtypBezeichnung) return;
        this.UndoablePropertyChanging(this, "ArbeitJahrtyp", this.jahrtyp, value);
        this.jahrtyp = value;
        this.Model.Jahrtyp = value.Model;
        this.RaisePropertyChanged("ArbeitJahrtyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die Schuljahr currently assigned to this Arbeit
    /// </summary>
    public HalbjahrtypViewModel ArbeitHalbjahrtyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Halbjahrtyp == null)
        {
          return null;
        }

        if (this.halbjahrtyp == null || this.halbjahrtyp.Model != this.Model.Halbjahrtyp)
        {
          this.halbjahrtyp = App.MainViewModel.Halbjahrtypen.SingleOrDefault(d => d.Model == this.Model.Halbjahrtyp);
        }

        return this.halbjahrtyp;
      }

      set
      {
        if (value.HalbjahrtypBezeichnung == this.halbjahrtyp.HalbjahrtypBezeichnung) return;
        this.UndoablePropertyChanging(this, "ArbeitHalbjahrtyp", this.halbjahrtyp, value);
        this.halbjahrtyp = value;
        this.Model.Halbjahrtyp = value.Model;
        this.RaisePropertyChanged("ArbeitHalbjahrtyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die Klasse currently assigned to this Arbeit
    /// </summary>
    public KlasseViewModel ArbeitKlasse
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Klasse == null)
        {
          return null;
        }

        if (this.klasse == null || this.klasse.Model != this.Model.Klasse)
        {
          this.klasse = App.MainViewModel.Klassen.SingleOrDefault(d => d.Model == this.Model.Klasse);
        }

        return this.klasse;
      }

      set
      {
        if (value.KlasseBezeichnung == this.klasse.KlasseBezeichnung) return;
        this.UndoablePropertyChanging(this, "ArbeitKlasse", this.klasse, value);
        this.klasse = value;
        this.Model.Klasse = value.Model;
        this.RaisePropertyChanged("ArbeitKlasse");
      }
    }

    /// <summary>
    /// Holt oder setzt das Bewertungsschema für diese Arbeit
    /// </summary>
    public BewertungsschemaViewModel ArbeitBewertungsschema
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Bewertungsschema == null)
        {
          return null;
        }

        if (this.bewertungsschema == null || this.bewertungsschema.Model != this.Model.Bewertungsschema)
        {
          this.bewertungsschema = App.MainViewModel.Bewertungsschemata.SingleOrDefault(d => d.Model == this.Model.Bewertungsschema);
        }

        return this.bewertungsschema;
      }

      set
      {
        if (value.BewertungsschemaBezeichnung == this.bewertungsschema.BewertungsschemaBezeichnung) return;
        this.UndoablePropertyChanging(this, "ArbeitBewertungsschema", this.bewertungsschema, value);
        this.bewertungsschema = value;
        this.Model.Bewertungsschema = value.Model;
        this.RaisePropertyChanged("ArbeitBewertungsschema");
        foreach (var schülereintragViewModel in this.Schülereinträge)
        {
          schülereintragViewModel.UpdateNoten();
        }

        this.BerechneNotenspiegel();
      }
    }

    /// <summary>
    /// Holt die Schülereinträge zu dieser Arbeit.
    /// </summary>
    public ObservableCollection<SchülereintragViewModel> Schülereinträge
    {
      get
      {
        var schülerliste = App.MainViewModel.Schülerlisten.FirstOrDefault(
          o =>
          o.SchülerlisteJahrtyp.JahrtypJahr == this.ArbeitJahrtyp.JahrtypJahr
          && o.SchülerlisteFach.FachBezeichnung == this.ArbeitFach.FachBezeichnung
          && o.SchülerlisteKlasse.KlasseBezeichnung == this.ArbeitKlasse.KlasseBezeichnung);
        return schülerliste == null ? new ObservableCollection<SchülereintragViewModel>() : schülerliste.Schülereinträge;
      }
    }

    /// <summary>
    /// Holt die Gesamtpunktzahl der Arbeit
    /// </summary>
    public float ArbeitGesamtpunktzahl
    {
      get
      {
        return this.Aufgaben.Sum(o => o.AufgabeMaxPunkte);
      }
    }

    /// <summary>
    /// Holt oder setzt den <see cref="Bepunktungstyp"/> für diese Arbeit.
    /// </summary>
    public Bepunktungstyp ArbeitBepunktungstyp
    {
      get
      {
        return (Bepunktungstyp)Enum.Parse(typeof(Bepunktungstyp), this.Model.Bepunktungstyp);
      }

      set
      {
        if (value.ToString() == this.Model.Bepunktungstyp) return;
        this.UndoablePropertyChanging(this, "ArbeitBepunktungstyp", this.Model.Bepunktungstyp, value.ToString());
        this.Model.Bepunktungstyp = value.ToString();
        this.RaisePropertyChanged("ArbeitBepunktungstyp");
        this.UpdateNoten();
      }
    }

    /// <summary>
    /// Holt die Überschrift für die Notenliste dieser Schülerliste
    /// </summary>
    [DependsUpon("ArbeitKlasse")]
    [DependsUpon("ArbeitFach")]
    [DependsUpon("ArbeitBezeichnung")]
    [DependsUpon("ArbeitDatum")]
    public string NotenlisteTitel
    {
      get
      {
        return this.ArbeitKlasse.KlasseBezeichnung + " " +
           this.ArbeitFach.FachBezeichnung + ", " +
          this.ArbeitBezeichnung + " am " +
          this.ArbeitDatum.ToShortDateString();
      }
    }

    /// <summary>
    /// Holt oder setzt die Anzahl der Einsen
    /// </summary>
    public int ArbeitAnzahlNoten1
    {
      get
      {
        return this.anzahlNoten1;
      }

      set
      {
        this.anzahlNoten1 = value;
        this.RaisePropertyChanged("ArbeitAnzahlNoten1");
      }
    }

    /// <summary>
    /// Holt oder setzt die Anzahl der Zweien
    /// </summary>
    public int ArbeitAnzahlNoten2
    {
      get
      {
        return this.anzahlNoten2;
      }

      set
      {
        this.anzahlNoten2 = value;
        this.RaisePropertyChanged("ArbeitAnzahlNoten2");
      }
    }

    /// <summary>
    /// Holt oder setzt die Anzahl der Dreien
    /// </summary>
    public int ArbeitAnzahlNoten3
    {
      get
      {
        return this.anzahlNoten3;
      }

      set
      {
        this.anzahlNoten3 = value;
        this.RaisePropertyChanged("ArbeitAnzahlNoten3");
      }
    }

    /// <summary>
    /// Holt oder setzt die Anzahl der Vieren
    /// </summary>
    public int ArbeitAnzahlNoten4
    {
      get
      {
        return this.anzahlNoten4;
      }

      set
      {
        this.anzahlNoten4 = value;
        this.RaisePropertyChanged("ArbeitAnzahlNoten4");
      }
    }

    /// <summary>
    /// Holt oder setzt die Anzahl der Fünfen
    /// </summary>
    public int ArbeitAnzahlNoten5
    {
      get
      {
        return this.anzahlNoten5;
      }

      set
      {
        this.anzahlNoten5 = value;
        this.RaisePropertyChanged("ArbeitAnzahlNoten5");
      }
    }

    /// <summary>
    /// Holt oder setzt die Anzahl der Sechsen
    /// </summary>
    public int ArbeitAnzahlNoten6
    {
      get
      {
        return this.anzahlNoten6;
      }

      set
      {
        this.anzahlNoten6 = value;
        this.RaisePropertyChanged("ArbeitAnzahlNoten6");
      }
    }

    /// <summary>
    /// Holt oder setzt den Notendurchschnitt dieser Arbeit
    /// </summary>
    public float ArbeitNotendurchschnitt
    {
      get
      {
        return this.notendurchschnitt;
      }

      set
      {
        this.notendurchschnitt = value;
        this.RaisePropertyChanged("ArbeitNotendurchschnitt");
      }
    }

    /// <summary>
    /// Holt den Notendurchschnitt dieser Arbeit als String
    /// </summary>
    [DependsUpon("ArbeitNotendurchschnitt")]
    public string ArbeitNotendurchschnittString
    {
      get
      {
        return "Durchschnitt: " + this.notendurchschnitt.ToString("N2");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Arbeit: " + this.ArbeitBezeichnung;
    }

    /// <summary>
    /// Berechnet den Notenspiegel für diese Arbeit.
    /// </summary>
    public void BerechneNotenspiegel()
    {
      try
      {
        var noten1 = 0;
        var noten2 = 0;
        var noten3 = 0;
        var noten4 = 0;
        var noten5 = 0;
        var noten6 = 0;
        foreach (var schülereintragViewModel in this.Schülereinträge)
        {
          var klassenarbeitNoten = schülereintragViewModel.Noten.Where(
            o => o.NoteIstSchriftlich
              && (o.NoteNotentyp == Notentyp.SchriftlichKlassenarbeit
              || o.NoteNotentyp == Notentyp.SchriftlichSonstige)
              && o.NoteArbeit != null
              && o.NoteArbeit.ArbeitDatum == this.ArbeitDatum
              && o.NoteArbeit.ArbeitLfdNr == this.ArbeitLfdNr);
          var noteViewModels = klassenarbeitNoten as IList<NoteViewModel> ?? klassenarbeitNoten.ToList();
          if (noteViewModels.Count() > 1)
          {
            // Nur eine Note pro Arbeit zugelassen, nimm die erste und lösche die anderen
            for (int i = 1; i < noteViewModels.Count(); i++)
            {
              schülereintragViewModel.Noten.RemoveTest(noteViewModels[i]);
            }
          }

          if (noteViewModels.Count == 0)
          {
            continue;
          }

          if (schülereintragViewModel.CurrentArbeitNote == string.Empty)
          {
            continue;
          }

          var noten = noteViewModels.First();

          switch (noten.NoteZensur.ZensurGanzeNote)
          {
            case 1:
              noten1++;
              break;
            case 2:
              noten2++;
              break;
            case 3:
              noten3++;
              break;
            case 4:
              noten4++;
              break;
            case 5:
              noten5++;
              break;
            case 6:
              noten6++;
              break;
          }
        }

        this.ArbeitAnzahlNoten1 = noten1;
        this.ArbeitAnzahlNoten2 = noten2;
        this.ArbeitAnzahlNoten3 = noten3;
        this.ArbeitAnzahlNoten4 = noten4;
        this.ArbeitAnzahlNoten5 = noten5;
        this.ArbeitAnzahlNoten6 = noten6;
        var summe = noten1 + noten2 + noten3 + noten4 + noten5 + noten6;
        this.ArbeitNotendurchschnitt = (float)(noten1 * 1 + noten2 * 2 + noten3 * 3 + noten4 * 4 + noten5 * 5 + noten6 * 6)
                                       / summe;
      }
      catch (Exception ex)
      {
        Log.HandleException(ex);
      }
    }

    /// <summary>
    /// Aktualisiert die Gesamtpunktzahl und alle nachrangigen Properties.
    /// </summary>
    public void UpdateNoten()
    {
      this.RaisePropertyChanged("ArbeitGesamtpunktzahl");
      foreach (var schülereintragViewModel in this.Schülereinträge)
      {
        schülereintragViewModel.UpdateNoten();
      }

      this.BerechneNotenspiegel();
    }

    /// <summary>
    /// Tritt auf, wenn die ModelCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void AufgabenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Aufgaben", this.Aufgaben, e, true, "Änderung der Aufgaben");
      this.RaisePropertyChanged("ArbeitGesamtpunktzahl");

      // Resequence list
      SequencingService.SetCollectionSequence(this.Aufgaben);
    }

    /// <summary>
    /// Handles addition a new phase to this arbeit
    /// </summary>
    private void AddAufgabe()
    {
      var aufgabe = new Aufgabe();
      aufgabe.LfdNr = this.Aufgaben.Count + 1;
      aufgabe.Bezeichnung = "Nr. " + aufgabe.LfdNr;
      aufgabe.MaxPunkte = 10;
      aufgabe.Arbeit = this.Model;
      var aufgabeViewModel = new AufgabeViewModel(aufgabe);
      var dlg = new AddAufgabeDialog { Aufgabe = aufgabeViewModel };
      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Neue Aufgabe {0} erstellt.", aufgabeViewModel), false))
      {
        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
        //App.UnitOfWork.Context.Aufgaben.Add(aufgabe);
        //App.MainViewModel.Aufgaben.Add(aufgabeViewModel);
        aufgabeViewModel.PropertyChanged += this.AufgabePropertyChanged;
        this.Aufgaben.Add(aufgabeViewModel);
        this.CurrentAufgabe = aufgabeViewModel;

        foreach (var schülereintragViewModel in this.Schülereinträge)
        {
          var ergebnis = new Ergebnis();
          ergebnis.Schülereintrag = schülereintragViewModel.Model;
          ergebnis.Aufgabe = aufgabe;

          //App.UnitOfWork.Context.Ergebnisse.Add(ergebnis);
          var ergebnisViewModel = new ErgebnisViewModel(ergebnis);
          //App.MainViewModel.Ergebnisse.Add(ergebnisViewModel);
          aufgabeViewModel.Ergebnisse.Add(ergebnisViewModel);
          schülereintragViewModel.Ergebnisse.Add(ergebnisViewModel);
          aufgabeViewModel.CurrentErgebnis = ergebnisViewModel;
          schülereintragViewModel.UpdateErgebnisse();
        }

        App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;

        //this.UpdateAufgabenColumnsCollection();
      }
    }

    /// <summary>
    /// Ruft den Aufgaben Dialog zur Bearbeitung auf
    /// </summary>
    private void EditAufgabe()
    {
      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Aufgabe {0} geändert.", this.CurrentAufgabe), false))
      {
        var dlg = new AddAufgabeDialog { Aufgabe = this.CurrentAufgabe };
        undo = !dlg.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
      else
      {
        //this.UpdateAufgabenColumnsCollection();
      }
    }

    private void AufgabePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "AufgabeZeit")
      {
      }
    }

    /// <summary>
    /// Handles deletion of the current phase
    /// </summary>
    private void DeleteCurrentAufgabe()
    {
      this.DeleteAufgabe(this.CurrentAufgabe);
    }

    /// <summary>
    /// Handles deletion of the given phase
    /// </summary>
    /// <param name="aufgabeViewModel"> The aufgabe View Model. </param>
    private void DeleteAufgabe(AufgabeViewModel aufgabeViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Aufgabe {0} gelöscht.", aufgabeViewModel), false))
      {
        //App.UnitOfWork.Context.Aufgaben.Remove(aufgabeViewModel.Model);
        //App.MainViewModel.Aufgaben.RemoveTest(aufgabeViewModel);
        aufgabeViewModel.PropertyChanged -= this.AufgabePropertyChanged;
        var result = this.Aufgaben.RemoveTest(aufgabeViewModel);
      }
    }

    /// <summary>
    /// Druckt die Noten der Arbeit aus
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
      var content = new ArbeitNotenlistePrintView
      {
        DataContext = this,
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
      var title = "Noten für " + this.ArbeitKlasse.KlasseBezeichnung;
      pd.PrintVisual(fixedPage, title);
    }
  }
}
