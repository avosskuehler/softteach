namespace SoftTeach.ViewModel.Personen
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Documents;

  using GongSolutions.Wpf.DragDrop;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Personen;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Noten;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// ViewModel of an individual Lerngruppe
  /// </summary>
  public class LerngruppeViewModel : ViewModelBase, IComparable, ICloneable, IDropTarget
  {
    private ICollectionView gruppenView;

    private ICollectionView metroGruppenView;

    /// <summary>
    /// The schüler currently selected
    /// </summary>
    private SchülereintragViewModel currentSchülereintrag;

    /// <summary>
    /// The schuljahr currently assigned to this Lerngruppe
    /// </summary>
    private SchuljahrViewModel schuljahr;

    /// <summary>
    /// The fach currently assigned to this Lerngruppe
    /// </summary>
    private FachViewModel fach;

    /// <summary>
    /// The notenWichtung currently assigned to this Lerngruppe
    /// </summary>
    private NotenWichtungViewModel notenWichtung;

    private int gruppenmitgliederanzahl;

    private int gruppenanzahl;

    private DateTime notenDatum;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="LerngruppeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="lerngruppe">
    /// The underlying schülerliste this ViewModel is to be based on
    /// </param>
    public LerngruppeViewModel(LerngruppeNeu lerngruppe)
    {
      if (lerngruppe == null)
      {
        throw new ArgumentNullException("lerngruppe");
      }

      this.Model = lerngruppe;
      this.NotenDatum = DateTime.Today;
      this.AddSchülereintragCommand = new DelegateCommand(this.AddSchülereintrag);
      this.DeleteSchülereintragCommand = new DelegateCommand(this.DeleteCurrentSchülereintrag, () => this.CurrentSchülereintrag != null);
      this.ExportLerngruppeCommand = new DelegateCommand(this.ExportLerngruppe);
      this.ImportSchülerCommand = new DelegateCommand(this.ImportSchüler);
      this.GruppenEinteilenCommand = new DelegateCommand(this.GruppenEinteilen);
      this.GruppenNeuEinteilenCommand = new DelegateCommand(this.GruppenNeuEinteilen);
      this.GruppenAusdruckenCommand = new DelegateCommand(this.GruppenAusdrucken);

      // Build data structures for schülerlisten
      this.Schülereinträge = new ObservableCollection<SchülereintragViewModel>();
      foreach (var schülereintrag in lerngruppe.Schülereinträge)
      {
        var vm = new SchülereintragViewModel(schülereintrag);
        //App.MainViewModel.Schülereinträge.Add(vm);
        this.Schülereinträge.Add(vm);
      }

      this.Lerngruppentermine = new ObservableCollection<LerngruppenterminViewModel>();
      foreach (var lerngruppenTermin in lerngruppe.Lerngruppentermine)
      {
        var vm = new LerngruppenterminViewModel(lerngruppenTermin);
        this.Lerngruppentermine.Add(vm);
      }

      this.Schülereinträge.CollectionChanged += this.SchülereinträgeCollectionChanged;

      this.SchülereinträgeView = CollectionViewSource.GetDefaultView(this.Schülereinträge);
      this.UpdateSort();

      this.SchülereinträgeGemischt = this.Schülereinträge.ToList();

      this.metroGruppenView = CollectionViewSource.GetDefaultView(this.SchülereinträgeGemischt);
      if (this.metroGruppenView.GroupDescriptions.Count == 0)
      {
        this.metroGruppenView.GroupDescriptions.Add(new PropertyGroupDescription("SchülereintragSortByGruppennummerProperty"));
      }

      if (this.metroGruppenView.SortDescriptions.Count == 0)
      {
        this.metroGruppenView.SortDescriptions.Add(new SortDescription("SchülereintragSortByGruppennummerProperty", ListSortDirection.Ascending));
        this.metroGruppenView.SortDescriptions.Add(new SortDescription("SchülereintragSortByVornameProperty", ListSortDirection.Ascending));
      }

      this.metroGruppenView.Filter = item =>
      {
        var schülereintragViewModel = item as SchülereintragViewModel;
        if (schülereintragViewModel == null) return false;

        return !schülereintragViewModel.IstKrank;
      };

      this.Gruppenmitgliederanzahl = 4;
      this.Gruppenanzahl = 8;

      App.MainViewModel.NotenWichtungen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.LerngruppeNotenWichtung))
        {
          this.LerngruppeNotenWichtung = App.MainViewModel.NotenWichtungen.FirstOrDefault();
        }
      };
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Schüler
    /// </summary>
    public DelegateCommand AddSchülereintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Schüler
    /// </summary>
    public DelegateCommand DeleteSchülereintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl  Schüler aus einer CSV Datei zu importieren
    /// </summary>
    public DelegateCommand ImportSchülerCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl die Lerngruppe nach Excel zu exportieren
    /// </summary>
    public DelegateCommand ExportLerngruppeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl einen Dialog aufzurufen, indem
    /// die Schüler der Lerngruppe in Gruppen eingeteilt werden.
    /// </summary>
    public DelegateCommand GruppenEinteilenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl die Schüler der Lerngruppe in Gruppen einzuteilen.
    /// </summary>
    public DelegateCommand GruppenNeuEinteilenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl die eingeteilten Gruppen auszudrucken.
    /// </summary>
    public DelegateCommand GruppenAusdruckenCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Lerngruppe this ViewModel is based on
    /// </summary>
    public LerngruppeNeu Model { get; private set; }

    /// <summary>
    /// Holt die schülereinträge for this schülerliste
    /// </summary>
    public ObservableCollection<SchülereintragViewModel> Schülereinträge { get; private set; }

    /// <summary>
    /// Holt die Lerngruppentermine for this Lerngruppe
    /// </summary>
    public ObservableCollection<LerngruppenterminViewModel> Lerngruppentermine { get; private set; }

    /// <summary>
    /// Holt die schülereinträge for this schülerliste
    /// </summary>
    public List<SchülereintragViewModel> SchülereinträgeGemischt { get; private set; }

    /// <summary>
    /// Holt oder setzt die Schülereinträge
    /// </summary>
    public ICollectionView SchülereinträgeView { get; set; }

    /// <summary>
    /// Holt oder setzt die gemischten Gruppen
    /// </summary>
    public ICollectionView GruppenView
    {
      get
      {
        var gruppen = this.Schülereinträge.Split(this.gruppenanzahl);
        var gruppenWithTitle = new Dictionary<string, IEnumerable<SchülereintragViewModel>>();
        var i = 1;
        foreach (var gruppe in gruppen)
        {
          gruppenWithTitle.Add("Gruppe " + i, gruppe);
          i++;
        }

        this.gruppenView = CollectionViewSource.GetDefaultView(gruppenWithTitle);
        if (this.gruppenView.SortDescriptions.Count == 0)
        {
          this.gruppenView.SortDescriptions.Add(new SortDescription("Key", ListSortDirection.Ascending));
        }

        this.gruppenView.Refresh();
        return this.gruppenView;
      }
    }

    /// <summary>
    /// Holt oder setzt die gemischten Gruppen
    /// </summary>
    public ICollectionView MetroGruppenView
    {
      get
      {
        this.metroGruppenView.Refresh();
        return this.metroGruppenView;
      }
    }

    /// <summary>
    /// Holt oder setzt die maximale Anzahl der Gruppenmitglieder
    /// </summary>
    public int Gruppenmitgliederanzahl
    {
      get
      {
        return this.gruppenmitgliederanzahl;
      }

      set
      {
        this.gruppenmitgliederanzahl = value;
        var anzahlAller = this.Schülereinträge.Count(o => !o.IstKrank);
        this.gruppenanzahl = (int)Math.Round(anzahlAller / (float)this.gruppenmitgliederanzahl, 0);
        this.RaisePropertyChanged("Gruppenmitgliederanzahl");
        this.RaisePropertyChanged("Gruppenanzahl");
      }
    }

    /// <summary>
    /// Holt oder setzt die gewünscht Gruppenanzahl
    /// </summary>
    public int Gruppenanzahl
    {
      get
      {
        return this.gruppenanzahl;
      }

      set
      {
        this.gruppenanzahl = value;
        var anzahlAller = this.Schülereinträge.Count(o => !o.IstKrank);
        this.gruppenmitgliederanzahl = (int)Math.Round(anzahlAller / (float)this.gruppenanzahl, 0);
        this.RaisePropertyChanged("Gruppenanzahl");
        this.RaisePropertyChanged("Gruppenmitgliederanzahl");
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected schülereintrag
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
        this.DeleteSchülereintragCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung dieser Lerngruppe.
    /// </summary>
    public string LerngruppeBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "LerngruppeBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("LerngruppeBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die Schuljahr currently assigned to this Lerngruppe
    /// </summary>
    public SchuljahrViewModel LerngruppeSchuljahr
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Schuljahr == null)
        {
          return null;
        }

        if (this.schuljahr == null || this.schuljahr.Model != this.Model.Schuljahr)
        {
          this.schuljahr = App.MainViewModel.Schuljahre.SingleOrDefault(d => d.Model == this.Model.Schuljahr);
        }

        return this.schuljahr;
      }

      set
      {
        if (value.SchuljahrBezeichnung == this.schuljahr.SchuljahrBezeichnung) return;
        this.UndoablePropertyChanging(this, "LerngruppeSchuljahr", this.schuljahr, value);
        this.schuljahr = value;
        this.Model.Schuljahr = value.Model;
        this.RaisePropertyChanged("LerngruppeSchuljahr");
      }
    }

    /// <summary>
    /// Holt oder setzt das Halbjahr dieser Lerngruppe.
    /// </summary>
    public Halbjahr LerngruppeHalbjahr
    {
      get
      {
        return this.Model.Halbjahr;
      }

      set
      {
        if (value == this.Model.Halbjahr) return;
        this.UndoablePropertyChanging(this, "LerngruppeHalbjahr", this.Model.Halbjahr, value);
        this.Model.Halbjahr = value;
        this.RaisePropertyChanged("LerngruppeHalbjahr");
      }
    }

    /// <summary>
    /// Holt oder setzt die Fach currently assigned to this Lerngruppe
    /// </summary>
    public FachViewModel LerngruppeFach
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
        this.UndoablePropertyChanging(this, "LerngruppeFach", this.fach, value);
        this.fach = value;
        this.Model.Fach = value.Model;
        this.RaisePropertyChanged("LerngruppeFach");
      }
    }

    /// <summary>
    /// Holt oder setzt den Jahrgang dieser Lerngruppe.
    /// </summary>
    public int LerngruppeJahrgang
    {
      get
      {
        return this.Model.Jahrgang;
      }

      set
      {
        if (value == this.Model.Jahrgang) return;
        this.UndoablePropertyChanging(this, "LerngruppeJahrgang", this.Model.Jahrgang, value);
        this.Model.Jahrgang = value;
        this.RaisePropertyChanged("LerngruppeJahrgang");
      }
    }

    /// <summary>
    /// Holt oder setzt den Bepunktungstyp dieser Lerngruppe.
    /// </summary>
    public Bepunktungstyp LerngruppeBepunktungstyp
    {
      get
      {
        return this.Model.Bepunktungstyp;
      }

      set
      {
        if (value == this.Model.Bepunktungstyp) return;
        this.UndoablePropertyChanging(this, "LerngruppeBepunktungstyp", this.Model.Bepunktungstyp, value);
        this.Model.Bepunktungstyp = value;
        this.RaisePropertyChanged("LerngruppeBepunktungstyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die Halbjahr currently assigned to this Lerngruppe
    /// </summary>
    public NotenWichtungViewModel LerngruppeNotenWichtung
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.NotenWichtung == null)
        {
          return null;
        }

        if (this.notenWichtung == null || this.notenWichtung.Model != this.Model.NotenWichtung)
        {
          this.notenWichtung = App.MainViewModel.NotenWichtungen.SingleOrDefault(d => d.Model == this.Model.NotenWichtung);
        }

        return this.notenWichtung;
      }

      set
      {
        if (value.NotenWichtungBezeichnung == this.notenWichtung.NotenWichtungBezeichnung) return;
        this.UndoablePropertyChanging(this, "LerngruppeNotenWichtung", this.notenWichtung, value);
        this.notenWichtung = value;
        this.Model.NotenWichtung = value.Model;
        this.RaisePropertyChanged("LerngruppeNotenWichtung");
      }
    }

    /// <summary>
    /// Holt den header for the list of pupils in this class
    /// </summary>
    [DependsUpon("LerngruppeFach")]
    [DependsUpon("LerngruppeSchuljahr")]
    public string LerngruppeÜberschrift
    {
      get
      {
        return "SchülerInnen der " + this.LerngruppeBezeichnung + " in " +
          this.LerngruppeFach.FachBezeichnung + " im Schuljahr " +
          this.LerngruppeSchuljahr.SchuljahrBezeichnung;
      }
    }

    /// <summary>
    /// Holt den short header for the list of pupils in this class
    /// </summary>
    [DependsUpon("LerngruppeKlasse")]
    [DependsUpon("LerngruppeFach")]
    [DependsUpon("LerngruppeSchuljahr")]
    public string LerngruppeKurzbezeichnung
    {
      get
      {
        return this.LerngruppeBezeichnung + ", " +
          this.LerngruppeFach.FachBezeichnung + ", " +
          this.LerngruppeSchuljahr.SchuljahrBezeichnung;
      }
    }

    /// <summary>
    /// Holt die Überschrift für die Notenliste dieser Lerngruppe
    /// </summary>
    [DependsUpon("LerngruppeKlasse")]
    [DependsUpon("LerngruppeFach")]
    [DependsUpon("LerngruppeSchuljahr")]
    public string NotenlisteTitel
    {
      get
      {
        return "Noten für " + this.LerngruppeBezeichnung + ", " +
          this.LerngruppeFach.FachBezeichnung + ", Stand: " + this.NotenDatum.ToString("ddd dd.MM.yyyy") + " Schuljahr " +
          this.LerngruppeSchuljahr.SchuljahrBezeichnung;
      }
    }

    /// <summary>
    /// Holt den current schülercount
    /// </summary>
    public int Schülerzahl
    {
      get { return this.Schülereinträge.Count; }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob in den Gruppen Jungen und Mädchen möglichst getrennt sein sollen.
    /// </summary>
    public bool? MädchenJungeGemischt { get; set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob bei der Gruppeneinteilung die Teilungsgruppen beachtet werden sollen.
    /// </summary>
    public bool TeilungsgruppenBeachten { get; set; }

    /// <summary>
    /// Gets or sets the noten datum.
    /// </summary>
    /// <value>The noten datum.</value>
    public DateTime NotenDatum
    {
      get
      {
        return this.notenDatum;
      }
      set
      {
        this.notenDatum = value;
        this.RaisePropertyChanged("NotenDatum");

        if (this.Schülereinträge != null)
        {
          foreach (var schülereintragViewModel in this.Schülereinträge)
          {
            schülereintragViewModel.AnpassungenAuslesen();
            schülereintragViewModel.UpdateNoten();
          }
        }
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherLerngruppeViewModel = obj as LerngruppeViewModel;
      if (otherLerngruppeViewModel != null)
      {
        return StringLogicalComparer.Compare(
          this.LerngruppeBezeichnung,
          otherLerngruppeViewModel.LerngruppeBezeichnung);
      }

      throw new ArgumentException("Object is not a LerngruppeViewModel");
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Lerngruppe: " + this.LerngruppeKurzbezeichnung;
    }

    /// <summary>
    /// Dupliziert diese Lerngruppe, aber mit leeren Eigenschaften in
    /// den Schülereinträgen.
    /// </summary>
    /// <returns>Eine Lerngruppe mit den selben Eigenschaften,
    /// aber leeren Schülereinträgen.</returns>
    public object Clone()
    {
      var schülerlisteClone = new LerngruppeNeu();
      schülerlisteClone.Bezeichnung = this.LerngruppeBezeichnung;
      schülerlisteClone.Schuljahr = this.LerngruppeSchuljahr.Model;
      schülerlisteClone.Halbjahr = this.LerngruppeHalbjahr;
      schülerlisteClone.Fach = this.LerngruppeFach.Model;
      schülerlisteClone.Jahrgang = this.LerngruppeJahrgang;
      schülerlisteClone.Bepunktungstyp = this.LerngruppeBepunktungstyp;
      schülerlisteClone.NotenWichtung = this.LerngruppeNotenWichtung.Model;
      foreach (var schülereintragViewModel in this.Schülereinträge.OrderBy(o => o.SchülereintragPerson.Nachname))
      {
        var schülereintragClone = new SchülereintragNeu();
        schülereintragClone.Person = schülereintragViewModel.Model.Person;
        schülereintragClone.Lerngruppe = schülerlisteClone;
        schülerlisteClone.Schülereinträge.Add(schülereintragClone);
      }

      var vm = new LerngruppeViewModel(schülerlisteClone);
      App.MainViewModel.Lerngruppen.Add(vm);

      return vm;
    }

    /// <summary>
    /// Updates the sort.
    /// </summary>
    public void UpdateSort()
    {
      this.SchülereinträgeView.SortDescriptions.Clear();
      this.SchülereinträgeView.SortDescriptions.Add(new SortDescription("SchülereintragSortByNachnameProperty", ListSortDirection.Ascending));
      this.SchülereinträgeView.Refresh();
    }

    /// <summary>
    /// Drag over event handler
    /// </summary>
    /// <param name="dropInfo">The drop information.</param>
    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data;
      var targetItem = dropInfo.TargetItem;
      if (sourceItem is SchülereintragViewModel && targetItem is SchülereintragViewModel)
      {
        var source = sourceItem as SchülereintragViewModel;
        var target = targetItem as SchülereintragViewModel;
        if (source.SchülereintragPerson.Gruppennummer != target.SchülereintragPerson.Gruppennummer)
        {
          dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
          dropInfo.Effects = DragDropEffects.Move;
        }
        else
        {
          dropInfo.Effects = DragDropEffects.None;
        }
      }
      else if (targetItem == null)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.Effects = DragDropEffects.Move;
      }
    }

    /// <summary>
    /// Drops the specified drop information.
    /// </summary>
    /// <param name="dropInfo">The drop information.</param>
    public void Drop(IDropInfo dropInfo)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Drag and Drop im Curriculum"), false))
      {
        var sourceItem = dropInfo.Data;
        var targetItem = dropInfo.TargetItem;
        if (sourceItem is SchülereintragViewModel && targetItem is SchülereintragViewModel)
        {
          var source = sourceItem as SchülereintragViewModel;
          var target = targetItem as SchülereintragViewModel;
          var targetGruppenNummer = target.SchülereintragPerson.Gruppennummer;
          target.SchülereintragPerson.Gruppennummer = source.SchülereintragPerson.Gruppennummer;
          source.SchülereintragPerson.Gruppennummer = targetGruppenNummer;
        }
        else if (targetItem == null)
        {
          var source = sourceItem as SchülereintragViewModel;
          var sourceGruppenNummer = source.SchülereintragPerson.Gruppennummer;
          var up = dropInfo.DropPosition.X - dropInfo.DragInfo.DragStartPosition.X > 0;
          if (!up && dropInfo.DropPosition.Y > dropInfo.DragInfo.DragStartPosition.Y + 100)
          {
            up = true;
          }

          var newGruppennummer = up ? sourceGruppenNummer + 1 : sourceGruppenNummer - 1;
          if (newGruppennummer > this.gruppenanzahl)
          {
            newGruppennummer = 1;
          }

          if (newGruppennummer < 1)
          {
            newGruppennummer = this.gruppenanzahl;
          }

          source.SchülereintragPerson.Gruppennummer = newGruppennummer;
        }

        this.RaisePropertyChanged("MetroGruppenView");
      }
    }

    /// <summary>
    /// Setzt alle Schüler auf Gesund zurück.
    /// </summary>
    public void ResetKrankenstand()
    {
      this.Schülereinträge.Each(o => o.IstKrank = false);
    }

    /// <summary>
    /// Handles addition a new schülereintrag to this schülerliste
    /// </summary>
    private void AddSchülereintrag()
    {
      // Show a dialog which has multiselect option
      var dlg = new SelectSchülerDialog();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Neue Schüler in Lerngruppe {0} eingetragen.", this), false))
        {
          foreach (var obj in dlg.SelectedSchüler)
          {
            // Care for empty rows
            if (obj is PersonViewModel)
            {
              // Cast to valid object
              var person = obj as PersonViewModel;

              // Check if already there
              if (
                this.Schülereinträge.Any(
                  o =>
                  o.SchülereintragPerson.Vorname == person.Vorname
                  && o.SchülereintragPerson.Nachname == person.Nachname
                  && o.SchülereintragPerson.PersonGeburtstag == person.PersonGeburtstag))
              {
                continue;
              }

              // perform add
              var schülereintrag = new SchülereintragNeu();
              schülereintrag.Person = person.Model;
              schülereintrag.Lerngruppe = this.Model;
              //App.UnitOfWork.Context.Schülereinträge.Add(schülereintrag);
              var vm = new SchülereintragViewModel(schülereintrag);
              //App.MainViewModel.Schülereinträge.Add(vm);
              this.Schülereinträge.Add(vm);
              this.CurrentSchülereintrag = vm;
            }
          }
        }
      }
    }

    /// <summary>
    /// Packt den existierenden Schülereintrag in die momentane Liste
    /// </summary>
    /// <param name="schülereintragViewModel">The schülereintrag view model.</param>
    public void AddSchülereintrag(SchülereintragViewModel schülereintragViewModel)
    {
      schülereintragViewModel.Model.Lerngruppe = this.Model;
      this.Schülereinträge.Add(schülereintragViewModel);
      this.CurrentSchülereintrag = schülereintragViewModel;
    }

    /// <summary>
    /// Handles deletion of the current schülereintrag
    /// </summary>
    private void DeleteCurrentSchülereintrag()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Schüler {0} aus Lerngruppe {1} gelöscht.", this.CurrentSchülereintrag, this), false))
      {
        //App.UnitOfWork.Context.Schülereinträge.Remove(this.CurrentSchülereintrag.Model);
        this.Schülereinträge.RemoveTest(this.CurrentSchülereintrag);
        this.CurrentSchülereintrag = null;
      }
    }

    /// <summary>
    /// Exportiert die momentane Lerngruppe nach Excel
    /// </summary>
    private void ExportLerngruppe()
    {
      ExportData.ToXls(this);
    }

    /// <summary>
    /// Importiert Schüler aus einer CSV Datei in die Lerngruppe
    /// </summary>
    private void ImportSchüler()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Schüler importiert"), false))
      {
        var personen = ExportData.FromCSV();
        foreach (var personViewModel in personen)
        {
          // Check if already there
          if (
            this.Schülereinträge.Any(
              o =>
              o.SchülereintragPerson.Vorname == personViewModel.Vorname
              && o.SchülereintragPerson.Nachname == personViewModel.Nachname
              && o.SchülereintragPerson.PersonGeburtstag == personViewModel.PersonGeburtstag))
          {
            continue;
          }

          // perform add
          var schülereintrag = new SchülereintragNeu();
          schülereintrag.Person = personViewModel.Model;
          schülereintrag.Lerngruppe = this.Model;
          //App.UnitOfWork.Context.Schülereinträge.Add(schülereintrag);
          var vm = new SchülereintragViewModel(schülereintrag);
          //App.MainViewModel.Schülereinträge.Add(vm);
          this.Schülereinträge.Add(vm);
          this.CurrentSchülereintrag = vm;
        }
      }
    }

    /// <summary>
    /// Öffnet einen Dialog mit dem Gruppen eingeteilt werden können
    /// </summary>
    private void GruppenEinteilen()
    {
      var dlg = new GruppenErstellenDialog(this);
      dlg.ShowDialog();
    }

    /// <summary>
    /// Mischt die Gruppen nach den gegebenen Parametern neu
    /// </summary>
    private void GruppenNeuEinteilen()
    {
      // Reset Gruppennummern
      this.Schülereinträge.Each(o => o.SchülereintragPerson.Gruppennummer = 0);

      if (this.TeilungsgruppenBeachten)
      {
        var schülernachGruppen = this.Schülereinträge.OrderBy(o => o.SchülereintragPerson.Nachname).Chunk(this.Schülereinträge.Count / 2);

        var gruppenNummer = 0;
        foreach (var schülergruppe in schülernachGruppen)
        {
          if (this.MädchenJungeGemischt.HasValue)
          {
            // Mische die Schüler nach Jungen und Mädchen getrennt
            var mädchen = schülergruppe.Where(o => !o.IstKrank && o.SchülereintragPerson.PersonIstWeiblich).Shuffle().ToList();
            var jungen = schülergruppe.Where(o => !o.IstKrank && !o.SchülereintragPerson.PersonIstWeiblich).Shuffle().ToList();
            if (this.MädchenJungeGemischt.Value)
            {
              var gruppenMädchen = mädchen.Split(this.gruppenanzahl / 2);
              var gruppenJungen = jungen.Split(this.gruppenanzahl / 2);

              // Mädchen auf Gruppen verteilen
              foreach (var gruppe in gruppenMädchen)
              {
                gruppenNummer++;
                foreach (var schülereintragViewModel in gruppe)
                {
                  schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
                }
              }

              // Jungen auf Gruppen verteilen
              //gruppenNummer = this.gruppenanzahl / 2;
              foreach (var gruppe in gruppenJungen)
              {
                foreach (var schülereintragViewModel in gruppe)
                {
                  schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
                }

                gruppenNummer--;
              }

              gruppenNummer = this.gruppenanzahl / 2;
            }
            else
            {
              var anzahlMädchen = mädchen.Count;
              var anzahlJungen = jungen.Count;
              var gesamtZahl = anzahlMädchen + anzahlJungen;

              var gruppenMädchen = mädchen.Split((int)Math.Round(this.gruppenanzahl * anzahlMädchen / (float)gesamtZahl));
              var gruppenJungen = jungen.Split((int)Math.Round(this.gruppenanzahl * anzahlJungen / (float)gesamtZahl));

              foreach (var gruppe in gruppenMädchen)
              {
                gruppenNummer++;
                foreach (var schülereintragViewModel in gruppe)
                {
                  schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
                }
              }

              foreach (var gruppe in gruppenJungen)
              {
                gruppenNummer++;
                foreach (var schülereintragViewModel in gruppe)
                {
                  schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
                }
              }
            }
          }
          else
          {
            // Mische die Schüler beliebig
            this.SchülereinträgeGemischt = this.Schülereinträge.Where(o => !o.IstKrank).Shuffle().ToList();

            var gruppen = this.SchülereinträgeGemischt.Split(this.gruppenanzahl);

            foreach (var gruppe in gruppen)
            {
              gruppenNummer++;
              foreach (var schülereintragViewModel in gruppe)
              {
                schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
              }
            }
          }
        }
      }
      else
      {
        if (this.MädchenJungeGemischt.HasValue)
        {
          // Mische die Schüler nach Jungen und Mädchen getrennt
          var mädchen = this.Schülereinträge.Where(o => !o.IstKrank && o.SchülereintragPerson.PersonIstWeiblich).Shuffle().ToList();
          var jungen = this.Schülereinträge.Where(o => !o.IstKrank && !o.SchülereintragPerson.PersonIstWeiblich).Shuffle().ToList();
          if (this.MädchenJungeGemischt.Value)
          {
            var gruppenMädchen = mädchen.Split(this.gruppenanzahl);
            var gruppenJungen = jungen.Split(this.gruppenanzahl);

            // Mädchen auf Gruppen verteilen
            var gruppenNummer = 0;
            foreach (var gruppe in gruppenMädchen)
            {
              gruppenNummer++;
              foreach (var schülereintragViewModel in gruppe)
              {
                schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
              }
            }

            // Jungen auf Gruppen verteilen
            gruppenNummer = this.gruppenanzahl;
            foreach (var gruppe in gruppenJungen)
            {
              foreach (var schülereintragViewModel in gruppe)
              {
                schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
              }

              gruppenNummer--;
            }
          }
          else
          {
            var anzahlMädchen = mädchen.Count;
            var anzahlJungen = jungen.Count;
            var gesamtZahl = anzahlMädchen + anzahlJungen;

            var gruppenMädchen = mädchen.Split((int)Math.Round(this.gruppenanzahl * anzahlMädchen / (float)gesamtZahl));
            var gruppenJungen = jungen.Split((int)Math.Round(this.gruppenanzahl * anzahlJungen / (float)gesamtZahl));

            var gruppenNummer = 0;
            foreach (var gruppe in gruppenMädchen)
            {
              gruppenNummer++;
              foreach (var schülereintragViewModel in gruppe)
              {
                schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
              }
            }

            foreach (var gruppe in gruppenJungen)
            {
              gruppenNummer++;
              foreach (var schülereintragViewModel in gruppe)
              {
                schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
              }
            }
          }
        }
        else
        {
          // Mische die Schüler beliebig
          this.SchülereinträgeGemischt = this.Schülereinträge.Where(o => !o.IstKrank).Shuffle().ToList();

          var gruppen = this.SchülereinträgeGemischt.Split(this.gruppenanzahl);
          var gruppenNummer = 0;

          foreach (var gruppe in gruppen)
          {
            gruppenNummer++;
            foreach (var schülereintragViewModel in gruppe)
            {
              schülereintragViewModel.SchülereintragPerson.Gruppennummer = gruppenNummer;
            }
          }
        }
      }

      this.RaisePropertyChanged("GruppenView");
      this.RaisePropertyChanged("MetroGruppenView");
    }

    /// <summary>
    /// Druckt die aktuelle Gruppenzusammenstellung aus.
    /// </summary>
    private void GruppenAusdrucken()
    {
      // select printer and get printer settings
      var pd = new PrintDialog();
      if (pd.ShowDialog() != true)
      {
        return;
      }

      // create a document
      var document = new FixedDocument { Name = "GruppenAusdruck" };
      document.DocumentPaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

      // create a page
      var fixedPage = new FixedPage
      {
        Width = document.DocumentPaginator.PageSize.Width,
        Height = document.DocumentPaginator.PageSize.Height
      };

      if (Configuration.Instance.IsMetroMode)
      {
        // create the print output usercontrol
        var content = new MetroGruppenPrintView { DataContext = this, Width = fixedPage.Width, Height = fixedPage.Height };
        fixedPage.Children.Add(content);
      }
      else
      {
        // create the print output usercontrol
        var content = new GruppenPrintView { DataContext = this, Width = fixedPage.Width, Height = fixedPage.Height };
        fixedPage.Children.Add(content);
      }

      // Update the layout of our FixedPage
      var size = document.DocumentPaginator.PageSize;
      fixedPage.Measure(size);
      fixedPage.Arrange(new Rect(new Point(), size));
      fixedPage.UpdateLayout();

      // print it out
      var title = "Gruppeneinteilung für " + this.LerngruppeKurzbezeichnung;
      pd.PrintVisual(fixedPage, title);
    }

    /// <summary>
    /// Tritt auf, wenn die SchülereinträgeCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SchülereinträgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Schülereinträge", this.Schülereinträge, e, true, "Änderung der Schülereinträge");
      this.RaisePropertyChanged("Schülerzahl");
    }
  }
}