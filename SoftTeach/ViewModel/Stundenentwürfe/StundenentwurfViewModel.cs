namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Media;

  using GongSolutions.Wpf.DragDrop;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Stundenentwürfe;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual stundenentwurf
  /// </summary>
  public class StundenentwurfViewModel : ViewModelBase, ICloneable, IDropTarget
  {
    /// <summary>
    /// The fach currently assigned to this stundenentwurf
    /// </summary>
    private FachViewModel fach;

    /// <summary>
    /// The jahrgangsstufe currently assigned to this stundenentwurf
    /// </summary>
    private JahrgangsstufeViewModel jahrgangsstufe;

    /// <summary>
    /// The schuljahr currently assigned to this stundenentwurf
    /// </summary>
    private ModulViewModel modul;

    /// <summary>
    /// The phase currently selected
    /// </summary>
    private PhaseViewModel currentPhase;

    /// <summary>
    /// The dateiverweis currently selected
    /// </summary>
    private DateiverweisViewModel currentDateiverweis;

    /// <summary>
    /// In diese Liste werden alle Phasen geschrieben, die in die 
    /// Zwischenablage kopiert werden
    /// </summary>
    private List<PhaseContainer> copyToClipboardList;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundenentwurfViewModel"/> Klasse. 
    /// </summary>
    /// <param name="stundenentwurf">
    /// The underlying stundenentwurf this ViewModel is to be based on
    /// </param>
    public StundenentwurfViewModel(Stundenentwurf stundenentwurf)
    {
      if (stundenentwurf == null)
      {
        throw new ArgumentNullException("stundenentwurf");
      }

      this.Model = stundenentwurf;

      this.AddPhaseCommand = new DelegateCommand(this.AddPhase);
      this.MovePhaseCommand = new DelegateCommand(this.MovePhase, () => this.CurrentPhase != null);
      this.DeletePhaseCommand = new DelegateCommand(this.DeleteCurrentPhase, () => this.CurrentPhase != null);
      this.AddDateiverweisCommand = new DelegateCommand(this.AddDateiverweis);
      this.DeleteDateiverweisCommand = new DelegateCommand(this.DeleteCurrentDateiverweis, () => this.CurrentDateiverweis != null);
      this.CreateDateiCommand = new DelegateCommand(this.CreateDatei);
      this.CopyCommand = new DelegateCommand(this.Copy);
      this.PasteCommand = new DelegateCommand(this.Paste);

      // Build data structures for phasen
      this.Phasen = new ObservableCollection<PhaseViewModel>();
      foreach (var phase in stundenentwurf.Phasen.OrderBy(o => o.AbfolgeIndex))
      {
        var vm = new PhaseViewModel(phase);
        //App.MainViewModel.Phasen.Add(vm);
        vm.PropertyChanged += this.PhasePropertyChanged;
        this.Phasen.Add(vm);
      }

      this.Phasen.CollectionChanged += this.PhasenCollectionChanged;
      this.copyToClipboardList = new List<PhaseContainer>();
      this.SelectedPhasen = new List<PhaseViewModel>();

      // Build data structures for dateiverweise
      this.Dateiverweise = new ObservableCollection<DateiverweisViewModel>();
      foreach (var dateiverweis in stundenentwurf.Dateiverweise)
      {
        var vm = new DateiverweisViewModel(dateiverweis);
        //App.MainViewModel.Dateiverweise.Add(vm);
        this.Dateiverweise.Add(vm);
      }

      this.Dateiverweise.CollectionChanged += this.DateiverweiseCollectionChanged;

      this.ModulView = new ListCollectionView(App.MainViewModel.Module);
      this.ModulView.Filter = this.ModulFilter;
      this.ModulView.SortDescriptions.Add(new SortDescription("ModulBezeichnung", ListSortDirection.Ascending));
      this.ModulView.Refresh();
    }

    /// <summary>
    /// Holt den Befehl ein phase zu erstellen
    /// </summary>
    public DelegateCommand AddPhaseCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl die phasen in die nächste Stunde zu verschieben.
    /// </summary>
    public DelegateCommand MovePhaseCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current phase
    /// </summary>
    public DelegateCommand DeletePhaseCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Dateiverweis
    /// </summary>
    public DelegateCommand AddDateiverweisCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the Dateiverweis
    /// </summary>
    public DelegateCommand DeleteDateiverweisCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur creating a new datei
    /// </summary>
    public DelegateCommand CreateDateiCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zum Einfügen
    /// </summary>
    public DelegateCommand PasteCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zum Einfügen
    /// </summary>
    public DelegateCommand CopyCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Stundenentwurf this ViewModel is based on
    /// </summary>
    public Stundenentwurf Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die gefilterten Module
    /// </summary>
    public ICollectionView ModulView { get; set; }

    /// <summary>
    /// Holt die Phasen for this stundenentwurf
    /// </summary>
    public ObservableCollection<PhaseViewModel> Phasen { get; private set; }

    /// <summary>
    /// Holt die markierten Phasen im Stundenentwurf
    /// </summary>
    public IList SelectedPhasen { get; set; }

    /// <summary>
    /// Holt den Dateiverweise for this stundenentwurf
    /// </summary>
    public ObservableCollection<DateiverweisViewModel> Dateiverweise { get; private set; }

    /// <summary>
    /// Holt die Stunden, denen dieser Stundenentwurf zugeordnet ist.
    /// </summary>
    public ICollection<Stunde> StundenentwurfStundenCollection
    {
      get
      {
        return this.Model.Stunden;
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected phase
    /// </summary>
    public PhaseViewModel CurrentPhase
    {
      get
      {
        return this.currentPhase;
      }

      set
      {
        this.currentPhase = value;
        this.RaisePropertyChanged("CurrentPhase");
        this.DeletePhaseCommand.RaiseCanExecuteChanged();
        this.MovePhaseCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected Dateiverweis
    /// </summary>
    public DateiverweisViewModel CurrentDateiverweis
    {
      get
      {
        return this.currentDateiverweis;
      }

      set
      {
        this.currentDateiverweis = value;
        this.RaisePropertyChanged("CurrentDateiverweis");
        this.DeleteDateiverweisCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Ansagen
    /// </summary>
    public string StundenentwurfAnsagen
    {
      get
      {
        return this.Model.Ansagen;
      }

      set
      {
        if (value == this.Model.Ansagen) return;
        this.UndoablePropertyChanging(this, "StundenentwurfAnsagen", this.Model.Ansagen, value);
        this.Model.Ansagen = value;
        this.RaisePropertyChanged("StundenentwurfAnsagen");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob der Computer benötigt wird
    /// </summary>
    public bool StundenentwurfComputer
    {
      get
      {
        return this.Model.Computer;
      }

      set
      {
        if (value == this.Model.Computer) return;
        this.UndoablePropertyChanging(this, "StundenentwurfComputer", this.Model.Computer, value);
        this.Model.Computer = value;
        this.RaisePropertyChanged("StundenentwurfComputer");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public FachViewModel StundenentwurfFach
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
        this.UndoablePropertyChanging(this, "StundenentwurfFach", this.fach, value);
        this.fach = value;
        this.Model.Fach = value.Model;
        this.RaisePropertyChanged("StundenentwurfFach");
        this.ModulView.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt die Hausaufgaben
    /// </summary>
    public string StundenentwurfHausaufgaben
    {
      get
      {
        return this.Model.Hausaufgaben;
      }

      set
      {
        if (value == this.Model.Hausaufgaben) return;
        this.UndoablePropertyChanging(this, "StundenentwurfHausaufgaben", this.Model.Hausaufgaben, value);
        this.Model.Hausaufgaben = value;
        this.RaisePropertyChanged("StundenentwurfHausaufgaben");
      }
    }

    /// <summary>
    /// Holt oder setzt die Jahrgangsstufe currently assigned to this Stundenentwurf
    /// </summary>
    public JahrgangsstufeViewModel StundenentwurfJahrgangsstufe
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Jahrgangsstufe == null)
        {
          return null;
        }

        if (this.jahrgangsstufe == null || this.jahrgangsstufe.Model != this.Model.Jahrgangsstufe)
        {
          this.jahrgangsstufe =
            App.MainViewModel.Jahrgangsstufen.SingleOrDefault(d => d.Model == this.Model.Jahrgangsstufe);
        }

        return this.jahrgangsstufe;
      }

      set
      {
        if (value.JahrgangsstufeBezeichnung == this.jahrgangsstufe.JahrgangsstufeBezeichnung) return;
        this.UndoablePropertyChanging(this, "StundenentwurfJahrgangsstufe", this.jahrgangsstufe, value);
        this.jahrgangsstufe = value;
        this.Model.Jahrgangsstufe = value.Model;
        this.RaisePropertyChanged("StundenentwurfJahrgangsstufe");
        this.ModulView.Refresh();
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob kopiert werden muss
    /// </summary>
    public bool StundenentwurfKopieren
    {
      get
      {
        return this.Model.Kopieren;
      }

      set
      {
        if (value == this.Model.Kopieren) return;
        this.UndoablePropertyChanging(this, "StundenentwurfKopieren", this.Model.Kopieren, value);
        this.Model.Kopieren = value;
        this.RaisePropertyChanged("StundenentwurfKopieren");
      }
    }

    /// <summary>
    /// Holt oder setzt die Schuljahr currently assigned to this Stundenentwurf
    /// </summary>
    public ModulViewModel StundenentwurfModul
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Modul == null)
        {
          return null;
        }

        if (this.modul == null || this.modul.Model != this.Model.Modul)
        {
          this.modul = App.MainViewModel.Module.SingleOrDefault(d => d.Model == this.Model.Modul);
        }

        return this.modul;
      }

      set
      {
        if (value == null) return;
        if (value.ModulBezeichnung == this.StundenentwurfModul.ModulBezeichnung) return;
        this.UndoablePropertyChanging(this, "StundenentwurfModul", this.modul, value);
        this.modul = value;
        this.Model.Modul = value.Model;
        this.RaisePropertyChanged("StundenentwurfModul");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stundenthema
    /// </summary>
    public string StundenentwurfStundenthema
    {
      get
      {
        return this.Model.Stundenthema;
      }

      set
      {
        if (value == this.Model.Stundenthema) return;
        this.UndoablePropertyChanging(this, "StundenentwurfStundenthema", this.Model.Stundenthema, value);
        this.Model.Stundenthema = value;
        this.RaisePropertyChanged("StundenentwurfStundenthema");
      }
    }

    /// <summary>
    /// Holt oder setzt die Datum
    /// </summary>
    public DateTime StundenentwurfDatum
    {
      get
      {
        return this.Model.Datum;
      }

      set
      {
        if (value == this.Model.Datum) return;
        this.UndoablePropertyChanging(this, "StundenentwurfDatum", this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("StundenentwurfDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stundenzahl
    /// </summary>
    public int StundenentwurfStundenzahl
    {
      get
      {
        return this.Model.Stundenzahl;
      }

      set
      {
        if (value == this.Model.Stundenzahl) return;
        this.UndoablePropertyChanging(this, "StundenentwurfStundenzahl", this.Model.Stundenzahl, value);
        this.Model.Stundenzahl = value;
        this.RaisePropertyChanged("StundenentwurfStundenzahl");
      }
    }

    /// <summary>
    /// Holt a linebreaked string with a short form string for the phasen
    /// of this stundenentwurf
    /// </summary>
    public string StundenentwurfPhasenKurzform
    {
      get
      {
        var kurzform = string.Empty;
        foreach (var phaseViewModel in this.Phasen)
        {
          var inhalt = phaseViewModel.PhaseInhalt;
          kurzform += phaseViewModel.PhaseZeit + "' ";
          kurzform += phaseViewModel.PhaseSozialform.SozialformBezeichnung + ": ";
          kurzform += inhalt.Length > 0 ? inhalt.Substring(0, Math.Min(100, inhalt.Length)) : string.Empty;
          if (inhalt.Length > 100)
          {
            kurzform += " ... ";
          }

          kurzform += Environment.NewLine;
        }

        return kurzform;
      }
    }

    /// <summary>
    /// Holt a linebreaked string with a short form string for the phasen
    /// of this stundenentwurf
    /// </summary>
    public string StundenentwurfPhasenLangform
    {
      get
      {
        var langform = new StringBuilder();

        foreach (var phaseViewModel in this.Phasen)
        {
          //          var inhalt = phaseViewModel.PhaseInhalt;
          langform.Append(phaseViewModel.PhaseZeit);
          langform.Append("' ");
          langform.Append(phaseViewModel.PhaseSozialform.SozialformBezeichnung);
          langform.Append(": ");
          langform.AppendLine(phaseViewModel.PhaseInhalt);
        }

        return langform.ToString();
      }
    }

    /// <summary>
    /// Holt a linebreaked string with a short form string for the phasen
    /// of this stundenentwurf
    /// </summary>
    public string StundenentwurfDateiliste
    {
      get
      {
        var dateiliste = new StringBuilder();

        foreach (var dateiverweis in this.Dateiverweise)
        {
          dateiliste.AppendLine(dateiverweis.DateiverweisDateinameOhnePfad);
        }

        return dateiliste.ToString();
      }
    }

    /// <summary>
    /// Holt den total minute amount that are used in the phasen collection
    /// without counting phases quoted as "Pause"
    /// </summary>
    public int StundenentwurfVerplanteMinuten
    {
      get
      {
        return this.Phasen.Where(
          o => o.PhaseMedium != null
          && o.PhaseMedium.MediumBezeichnung != "Pause"
          && o.PhaseInhalt != "Pause").Sum(phaseViewModel => phaseViewModel.PhaseZeit);
      }
    }

    /// <summary>
    /// Holt den total minute amount that are used in the phasen collection
    /// without counting phases quoted as "Pause" in relation to the available minutes as string
    /// </summary>
    [DependsUpon("StundenentwurfVerplanteMinuten")]
    public string StundenentwurfVerplanteMinutenVonAllen
    {
      get
      {
        return string.Format("{0} von {1}", this.StundenentwurfVerplanteMinuten, this.StundenentwurfStundenzahl * 45);
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob für den Stundenentwurf zuviel Zeit verplant wurde.
    /// </summary>
    [DependsUpon("StundenentwurfVerplanteMinuten")]
    public bool StundenentwurfIstZuvielZeitVerplant
    {
      get
      {
        return this.StundenentwurfVerplanteMinuten > this.StundenentwurfStundenzahl * 45;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob für den Stundenentwurf zuwenig Zeit verplant wurde.
    /// </summary>
    [DependsUpon("StundenentwurfVerplanteMinuten")]
    public bool StundenentwurfIstZuwenigZeitVerplant
    {
      get
      {
        return this.StundenentwurfVerplanteMinuten < this.StundenentwurfStundenzahl * 45;
      }
    }

    /// <summary>
    /// Holt die Farbe für den Minutencheck.
    /// </summary>
    [DependsUpon("StundenentwurfVerplanteMinuten")]
    public SolidColorBrush StundentwurfMinutenCheckBrush
    {
      get
      {
        if (this.StundenentwurfIstZuvielZeitVerplant)
        {
          return Brushes.Red;
        }

        if (this.StundenentwurfIstZuwenigZeitVerplant)
        {
          return Brushes.Orange;
        }

        return Brushes.Green;
      }
    }

    /// <summary>
    /// Erstellt einen Dateiverweispfad für die Dateien, die zu dem gegebenen Stunden-
    /// entwurf gehören.
    /// </summary>
    /// <param name="stundenentwurf">Der Stundenentwurf, zu dem der Datveiverweis gehört.</param>
    /// <returns>Einen Pfad ohne Separator am Schluss.</returns>
    public static string GetDateiverweispfad(Stundenentwurf stundenentwurf)
    {
      var fachString = stundenentwurf.Fach.Bezeichnung;
      var jahrgangsstufeString = stundenentwurf.Jahrgangsstufe.Bezeichnung;
      jahrgangsstufeString = jahrgangsstufeString.Replace("/", "+");
      var modulString = stundenentwurf.Modul.Bezeichnung;
      var pathToCopyTo = Path.Combine(Configuration.GetMyDocumentsPath(), fachString);
      pathToCopyTo = Path.Combine(pathToCopyTo, jahrgangsstufeString);
      pathToCopyTo = Path.Combine(pathToCopyTo, modulString);
      return pathToCopyTo;
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Stundenentwurf: " + this.StundenentwurfStundenthema;
    }

    /// <summary>
    /// Returns a deep copy of this StundenentwurfViewModel
    /// </summary>
    /// <returns>A cloned <see cref="StundenentwurfViewModel"/></returns>
    public object Clone()
    {
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
      var entwurfClone = new Stundenentwurf();
      using (new UndoBatch(App.MainViewModel, string.Format("Stundenentwurf geklont"), false))
      {
        entwurfClone.Ansagen = this.Model.Ansagen;
        entwurfClone.Computer = this.Model.Computer;
        foreach (var dateiverweis in this.Model.Dateiverweise.ToList())
        {
          var dateiverweisClone = new Dateiverweis();
          dateiverweisClone.Dateiname = dateiverweis.Dateiname;
          dateiverweisClone.Dateityp = dateiverweis.Dateityp;
          dateiverweisClone.Stundenentwurf = entwurfClone;
          //App.UnitOfWork.Context.Dateiverweise.Add(dateiverweisClone);
          entwurfClone.Dateiverweise.Add(dateiverweis);
        }

        entwurfClone.Datum = this.Model.Datum;
        entwurfClone.Fach = this.Model.Fach;
        entwurfClone.Hausaufgaben = this.Model.Hausaufgaben;
        entwurfClone.Jahrgangsstufe = this.Model.Jahrgangsstufe;
        entwurfClone.Kopieren = this.Model.Kopieren;
        entwurfClone.Modul = this.Model.Modul;
        entwurfClone.Stundenthema = this.Model.Stundenthema;
        entwurfClone.Stundenzahl = this.Model.Stundenzahl;
        foreach (var phase in this.Model.Phasen.ToList())
        {
          var phaseClone = new Phase();
          phaseClone.AbfolgeIndex = phase.AbfolgeIndex;
          phaseClone.Inhalt = phase.Inhalt;
          phaseClone.Medium = phase.Medium;
          phaseClone.Sozialform = phase.Sozialform;
          phaseClone.Zeit = phase.Zeit;
          phaseClone.Stundenentwurf = entwurfClone;
          //App.UnitOfWork.Context.Phasen.Add(phaseClone);
          entwurfClone.Phasen.Add(phaseClone);
        }
        //App.UnitOfWork.Context.Stundenentwürfe.Add(entwurfClone);
      }

      var vm = new StundenentwurfViewModel(entwurfClone);
      App.MainViewModel.Stundenentwürfe.Add(vm);
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;

      return vm;
    }

    /// <summary>
    /// Aktualisiert die verplanten Minuten und die abhängigen Eigenschaften
    /// </summary>
    public void NotifyPhaseZeitChanged()
    {
      this.RaisePropertyChanged("StundenentwurfVerplanteMinuten");
    }

    /// <summary>
    /// Registriert die gegebene Phase, so dass deren Zeitänderungen um Stundenentwurf
    /// berücksichtigt werden können
    /// </summary>
    /// <param name="phaseViewModel">Die Phase, die kontrolliert werden soll.</param>
    public void AttachPhaseChangedEvent(PhaseViewModel phaseViewModel)
    {
      phaseViewModel.PropertyChanged += this.PhasePropertyChanged;
    }

    /// <summary>
    /// DragOver wird aufgerufen, wenn ein Element über eines der ListViews
    /// gezogen wird. Hier wird festgelegt, ob die Operation erlaubt wird oder nicht.
    /// </summary>
    /// <param name="dropInfo">Ein <see cref="DropInfo"/> mit dem Element was gezogen wird
    /// und dem Element auf das gezogen wurde.</param>
    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data as PhaseViewModel;
      var sourceItemCollection = dropInfo.Data as List<PhaseViewModel>;
      if (sourceItem == null && sourceItemCollection == null)
      {
        dropInfo.Effects = DragDropEffects.None;
        return;
      }

      if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Copy;
      }
      else
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Move;
      }
    }

    /// <summary>
    /// Führt die Darg and Drop Operation aus.
    /// </summary>
    /// <param name="dropInfo">Ein <see cref="DropInfo"/> mit dem Element was gezogen wird
    /// und dem Element auf das gezogen wurde.</param>
    public void Drop(IDropInfo dropInfo)
    {
      if (!(dropInfo.VisualTarget is DataGrid))
      {
        return;
      }

      var targetGrid = dropInfo.VisualTarget as DataGrid;
      if (targetGrid.Name != "PhasenGrid")
      {
        return;
      }


      if (dropInfo.Data is PhaseViewModel)
      {
        this.PerformDropWithSinglePhase(dropInfo);
      }
      else if (dropInfo.Data is List<PhaseViewModel>)
      {
        this.PerformDropWithPhasencollection(dropInfo);
      }

      SequencingService.SetCollectionSequence(this.Phasen);

      // Editing stoppen, da sonst kein Refresh funktioniert
      targetGrid.CommitEdit();
      targetGrid.CancelEdit();
    }

    private void PerformDropWithSinglePhase(IDropInfo dropInfo)
    {
      var phaseViewModel = (PhaseViewModel)dropInfo.Data;

      var newIndex = dropInfo.InsertIndex;
      if (newIndex < 0)
      {
        newIndex = this.Phasen.Count;
      }

      var oldIndex = this.Phasen.IndexOf(phaseViewModel);
      if (newIndex > oldIndex)
      {
        newIndex--;
      }

      // Wenn Control gedrückt ist
      if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      {
        if (newIndex > this.Phasen.Count)
        {
          newIndex = this.Phasen.Count;
        }

        this.Phasen.Insert(newIndex, (PhaseViewModel)phaseViewModel.Clone());
      }
      else
      {
        if (newIndex >= this.Phasen.Count)
        {
          newIndex = this.Phasen.Count - 1;
        }

        if (newIndex != oldIndex)
        {
          this.Phasen.Move(oldIndex, newIndex);
        }
      }

      SequencingService.SetCollectionSequence(this.Phasen);
    }

    private void PerformDropWithPhasencollection(IDropInfo dropInfo)
    {
      var phaseViewModels = (List<PhaseViewModel>)dropInfo.Data;

      var newIndex = dropInfo.InsertIndex;
      if (newIndex < 0)
      {
        newIndex = this.Phasen.Count;
      }

      foreach (var phaseViewModel in phaseViewModels.OrderByDescending(o => o.AbfolgeIndex))
      {
        var oldIndex = this.Phasen.IndexOf(phaseViewModel);
        if (newIndex > oldIndex)
        {
          newIndex--;
        }

        // Wenn Control gedrückt ist
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
          if (newIndex > this.Phasen.Count)
          {
            newIndex = this.Phasen.Count;
          }

          this.Phasen.Insert(newIndex, (PhaseViewModel)phaseViewModel.Clone());
        }
        else
        {
          if (newIndex >= this.Phasen.Count)
          {
            newIndex = this.Phasen.Count - 1;
          }

          if (newIndex != oldIndex)
          {
            this.Phasen.Move(oldIndex, newIndex);
          }
        }
      }

      SequencingService.SetCollectionSequence(this.Phasen);
    }

    /// <summary>
    /// Tritt auf, wenn die PhasenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void PhasenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Phasen", this.Phasen, e, true, "Änderung der Phasen");

      if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        foreach (var oldItem in e.OldItems)
        {
          var phaseViewModel = oldItem as PhaseViewModel;
          if (phaseViewModel != null)
          {
            phaseViewModel.PropertyChanged -= this.PhasePropertyChanged;
            //App.UnitOfWork.Context.Phasen.Remove(phaseViewModel.Model);
            this.Phasen.Remove(phaseViewModel);
            //App.MainViewModel.Phasen.RemoveTest(phaseViewModel);
          }
        }
      }

      this.RaisePropertyChanged("StundenentwurfPhasenKurzform");
      this.NotifyPhaseZeitChanged();
    }

    /// <summary>
    /// Tritt auf, wenn die PhasenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void DateiverweiseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Dateiverweise", this.Dateiverweise, e, true, "Änderung der Dateiverweise");
    }

    /// <summary>
    /// Handles addition a new phase to this stundenentwurf
    /// </summary>
    private void AddPhase()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Neue Phase angelegt"), false))
      {
        var phase = new Phase();
        phase.AbfolgeIndex = this.Phasen.Count;
        phase.Zeit = 10;
        phase.Inhalt = string.Empty;
        phase.Medium = App.MainViewModel.Medien.First().Model;
        phase.Sozialform = App.MainViewModel.Sozialformen.First().Model;
        phase.Stundenentwurf = this.Model;
        //App.UnitOfWork.Context.Phasen.Add(phase);
        var vm = new PhaseViewModel(phase);

        vm.PropertyChanged += this.PhasePropertyChanged;
        this.Phasen.Add(vm);
        this.CurrentPhase = vm;
        this.NotifyPhaseZeitChanged();
      }
    }

    /// <summary>
    /// Fügt eine gegebene Phase diesem Stundenentwurf hinzu.
    /// An erster Stelle und unter zurücksetzung des
    /// AbfolgeIndexes.
    /// </summary>
    /// <param name="vm"> Das ViewModel der hinzuzufügenden Phase </param>
    /// <param name="index"> The index. </param>
    private void AddPhase(PhaseViewModel vm, int index)
    {
      vm.Model.Stundenentwurf = this.Model;
      vm.PropertyChanged += this.PhasePropertyChanged;
      this.Phasen.Insert(index, vm);
      SequencingService.SetCollectionSequence(this.Phasen);
      this.CurrentPhase = vm;
      this.NotifyPhaseZeitChanged();
    }

    /// <summary>
    /// Fügt eine gegebene Phase diesem Stundenentwurf ans Ende hinzu.
    /// </summary>
    /// <param name="vm"> Das ViewModel der hinzuzufügenden Phase </param>
    public void AddPhase(PhaseViewModel vm)
    {
      vm.Model.Stundenentwurf = this.Model;
      vm.PropertyChanged += this.PhasePropertyChanged;
      this.Phasen.Add(vm);
      SequencingService.SetCollectionSequence(this.Phasen);
      this.CurrentPhase = vm;
      this.NotifyPhaseZeitChanged();
    }

    /// <summary>
    /// Schiebt die gewählte(n) Phase(n) in die nächste Stunde.
    /// </summary>
    private void MovePhase()
    {
      if (this.SelectedPhasen == null)
      {
        return;
      }

      var stundeViewModel = Selection.Instance.Stunde;
      var nächsteStunde = App.UnitOfWork.Context.Termine.OfType<Stunde>()
        .Where(o => o.Tagesplan.Monatsplan.Halbjahresplan.Id == stundeViewModel.Tagesplan.Monatsplan.Halbjahresplan.Id)
        .OrderBy(o => o.Tagesplan.Datum)
        .FirstOrDefault(o => o.Tagesplan.Datum > stundeViewModel.Tagesplan.Datum);

      using (new UndoBatch(App.MainViewModel, string.Format("Phase verschoben"), false))
      {

        if (nächsteStunde != null)
        {
          if (nächsteStunde.Stundenentwurf == null)
          {
            // Stundenentwurf erstellen
            var entwurf = new Stundenentwurf();
            entwurf.Datum = DateTime.Now;
            entwurf.Fach = stundeViewModel.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Fach;
            entwurf.Jahrgangsstufe = stundeViewModel.Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Klassenstufe.Jahrgangsstufe;
            entwurf.Stundenzahl = stundeViewModel.Stundenentwurf.Stundenzahl;
            entwurf.Ansagen = string.Empty;
            entwurf.Computer = false;
            entwurf.Hausaufgaben = string.Empty;
            entwurf.Kopieren = false;
            entwurf.Stundenthema = stundeViewModel.Stundenentwurf.Stundenthema;
            entwurf.Modul = stundeViewModel.Stundenentwurf.Modul;

            var vm = new StundenentwurfViewModel(entwurf);
            //App.UnitOfWork.Context.Stundenentwürfe.Add(entwurf);
            App.MainViewModel.Stundenentwürfe.Add(vm);
            nächsteStunde.Stundenentwurf = entwurf;
          }

          var entwurfViewModel = App.MainViewModel.Stundenentwürfe.FirstOrDefault(o => o.Model == nächsteStunde.Stundenentwurf);
          if (entwurfViewModel != null)
          {
            var moveItems = new List<PhaseViewModel>(this.SelectedPhasen.Cast<PhaseViewModel>());
            foreach (var phaseViewModel in moveItems)
            {
              var phaseClone = (PhaseViewModel)phaseViewModel.Clone();
              entwurfViewModel.AddPhase(phaseClone, 0);
              this.DeletePhase(phaseViewModel);
            }
          }
        }
      }
    }

    /// <summary>
    /// Event handler für die PropertyChanged event der Phase.
    /// Aktualisiert die Zeiten des Stundenentwurfs.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="PropertyChangedEventArgs"/> with the property</param>
    private void PhasePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "PhaseZeit")
      {
        this.NotifyPhaseZeitChanged();
      }
    }

    /// <summary>
    /// Handles deletion of the current phase
    /// </summary>
    private void DeleteCurrentPhase()
    {
      this.DeletePhase(this.CurrentPhase);
    }

    /// <summary>
    /// Handles deletion of the given phase
    /// </summary>
    /// <param name="phaseViewModel"> The phase View Model. </param>
    private void DeletePhase(PhaseViewModel phaseViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Phase {0} gelöscht.", phaseViewModel), false))
      {
        phaseViewModel.PropertyChanged -= this.PhasePropertyChanged;
        //App.UnitOfWork.Context.Phasen.Remove(phaseViewModel.Model);
        //App.MainViewModel.Phasen.RemoveTest(phaseViewModel);
        var result = this.Phasen.RemoveTest(phaseViewModel);
      }
    }

    /// <summary>
    /// Handles addition a new Dateiverweis to this stundenentwurf
    /// </summary>
    private void AddDateiverweis()
    {
      if (this.StundenentwurfFach == null)
      {
        return;
      }

      var dlg = new AskForDateiDialog();
      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      var dateiverweis = new Dateiverweis();
      var filename = this.CopyFileToBaseDirectory(dlg.DateinameMitPfad);
      dateiverweis.Dateiname = filename;
      dateiverweis.Dateityp = dlg.Dateityp.Model;
      dateiverweis.Stundenentwurf = this.Model;

      //App.UnitOfWork.Context.Dateiverweise.Add(dateiverweis);
      var vm = new DateiverweisViewModel(dateiverweis);
      using (new UndoBatch(App.MainViewModel, string.Format("Dateiverweis {0} erstellt.", vm), false))
      {
        //App.MainViewModel.Dateiverweise.Add(vm);
        this.Dateiverweise.Add(vm);
        this.CurrentDateiverweis = vm;
      }
    }

    /// <summary>
    /// Kopiert die Datei, die zum Stundenentwurf hinzugefügt werden soll
    /// in das Lagerverzeichnis von SoftTeach, an die richtige Stelle.
    /// </summary>
    /// <param name="filenameWithPath">Der Dateiname mit Pfad der Datei.</param>
    /// <returns>Das Zielverzeichnis der Datei</returns>
    private string CopyFileToBaseDirectory(string filenameWithPath)
    {
      var pathToCopyTo = GetDateiverweispfad(this.Model);
      var filenameWithoutPath = Path.GetFileName(filenameWithPath);
      var destination = pathToCopyTo + Path.DirectorySeparatorChar + filenameWithoutPath;
      if (!File.Exists(destination))
      {
        if (!Directory.Exists(pathToCopyTo))
        {
          Directory.CreateDirectory(pathToCopyTo);
        }

        File.Copy(filenameWithPath, destination);
      }

      return destination;
    }

    /// <summary>
    /// Handles deletion of the current Dateiverweis
    /// </summary>
    private void DeleteCurrentDateiverweis()
    {
      if (this.CurrentDateiverweis == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Dateiverweis {0} gelöscht.", this.CurrentDateiverweis), false))
      {
        //App.UnitOfWork.Context.Dateiverweise.Remove(this.CurrentDateiverweis.Model);
        //var success = App.MainViewModel.Dateiverweise.RemoveTest(this.CurrentDateiverweis);
        this.Dateiverweise.RemoveTest(this.CurrentDateiverweis);
        this.CurrentDateiverweis = null;
      }
    }

    /// <summary>
    /// Handles creation of a new Datei
    /// </summary>
    private void Copy()
    {
      Clipboard.Clear();
      this.copyToClipboardList.Clear();

      if (this.SelectedPhasen == null)
      {
        throw new ArgumentNullException("SelectedPhasen sind null!");
      }

      foreach (var row in this.SelectedPhasen)
      {
        if (!(row is PhaseViewModel))
        {
          continue;
        }

        var phaseViewModel = row as PhaseViewModel;
        this.copyToClipboardList.Add(new PhaseContainer(
          phaseViewModel.PhaseInhalt,
          phaseViewModel.PhaseMedium.MediumBezeichnung,
          phaseViewModel.PhaseSozialform.SozialformBezeichnung,
          phaseViewModel.PhaseZeit));
      }

      var newObject = new DataObject("PhasenCollection", this.copyToClipboardList);
      Clipboard.SetDataObject(newObject);
    }

    /// <summary>
    /// Handles creation of a new Datei
    /// </summary>
    private void Paste()
    {
      var data = Clipboard.GetDataObject();
      if (data == null || !data.GetDataPresent("PhasenCollection"))
      {
        return;
      }

      var phasenCollection = data.GetData("PhasenCollection") as List<PhaseContainer>;
      var insertIndex = this.Phasen.Count;
      if (phasenCollection == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Phasen eingefügt"), false))
      {
        phasenCollection.Reverse();
        foreach (var phaseContainer in phasenCollection)
        {
          var phase = new Phase();
          phase.AbfolgeIndex = insertIndex;
          phase.Zeit = phaseContainer.Zeit;
          phase.Inhalt = phaseContainer.Inhalt;
          var mediumViewModel = App.MainViewModel.Medien.FirstOrDefault(o => o.MediumBezeichnung == phaseContainer.Medium);
          if (mediumViewModel != null)
          {
            var medium = mediumViewModel.Model;
            phase.Medium = medium ?? App.MainViewModel.Medien.First().Model;
          }

          var sozialformViewModel = App.MainViewModel.Sozialformen.FirstOrDefault(o => o.SozialformBezeichnung == phaseContainer.Sozialform);
          if (sozialformViewModel != null)
          {
            var sozialform = sozialformViewModel.Model;
            phase.Sozialform = sozialform ?? App.MainViewModel.Sozialformen.First().Model;
          }

          phase.Stundenentwurf = this.Model;
          //App.UnitOfWork.Context.Phasen.Add(phase);
          var vm = new PhaseViewModel(phase);

          //App.MainViewModel.Phasen.Add(vm);
          vm.PropertyChanged += this.PhasePropertyChanged;
          this.Phasen.Add(vm);
          SequencingService.SetCollectionSequence(this.Phasen);
          this.CurrentPhase = vm;
          this.NotifyPhaseZeitChanged();
        }
      }
      //this.PhasenView.Refresh();
    }

    /// <summary>
    /// Handles creation of a new Datei
    /// </summary>
    private void CreateDatei()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Filtert die Terminliste nach Jahrtyp und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool ModulFilter(object item)
    {
      var modulViewModel = item as ModulViewModel;
      if (modulViewModel == null)
      {
        return false;
      }

      if (modulViewModel.ModulFach.FachBezeichnung != this.StundenentwurfFach.FachBezeichnung
        || modulViewModel.ModulJahrgangsstufe.JahrgangsstufeBezeichnung != this.StundenentwurfJahrgangsstufe.JahrgangsstufeBezeichnung)
      {
        return false;
      }

      return true;
    }
  }
}
