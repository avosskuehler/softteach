namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual aufgabe
  /// </summary>
  public class AufgabeViewModel : ViewModelBase, ISequencedObject
  {
    /// <summary>
    /// The ergebnis currently selected
    /// </summary>
    private ErgebnisViewModel currentErgebnis;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AufgabeViewModel"/> Klasse. 
    /// </summary>
    public AufgabeViewModel()
    {
      var aufgabe = new Aufgabe();
      aufgabe.LfdNr = Selection.Instance.Arbeit.Aufgaben.Count;
      aufgabe.MaxPunkte = 10;
      aufgabe.Bezeichnung = string.Empty;
      aufgabe.Arbeit = Selection.Instance.Arbeit.Model;
      this.Model = aufgabe;
      App.MainViewModel.Aufgaben.Add(this);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AufgabeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="aufgabe">
    /// The underlying aufgabe this ViewModel is to be based on
    /// </param>
    public AufgabeViewModel(Aufgabe aufgabe)
    {
      if (aufgabe == null)
      {
        throw new ArgumentNullException("aufgabe");
      }

      this.Model = aufgabe;

      // Build data structures for ergebnisse
      this.Ergebnisse = new ObservableCollection<ErgebnisViewModel>();
      foreach (var ergebnis in aufgabe.Ergebnisse)
      {
        var vm = new ErgebnisViewModel(ergebnis);
        App.MainViewModel.Ergebnisse.Add(vm);
        vm.PropertyChanged += this.ErgebnisPropertyChanged;
        this.Ergebnisse.Add(vm);
      }

      this.Ergebnisse.CollectionChanged += this.ErgebnisseCollectionChanged;

      this.EditAufgabeCommand = new DelegateCommand(this.EditAufgabe);
      this.AddErgebnisCommand = new DelegateCommand(this.AddErgebnis);
      this.DeleteErgebnisCommand = new DelegateCommand(this.DeleteCurrentErgebnis, () => this.CurrentErgebnis != null);
    }

    /// <summary>
    /// Holt den underlying Aufgabe this ViewModel is based on
    /// </summary>
    public Aufgabe Model { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Editierung der Aufgabe
    /// </summary>
    public DelegateCommand EditAufgabeCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new Ergebnis
    /// </summary>
    public DelegateCommand AddErgebnisCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Ergebnis
    /// </summary>
    public DelegateCommand DeleteErgebnisCommand { get; private set; }

    /// <summary>
    /// Holt den phasen for this stundenentwurf
    /// </summary>
    public ObservableCollection<ErgebnisViewModel> Ergebnisse { get; set; }

    /// <summary>
    /// Holt oder setzt die currently selected Ergebnis
    /// </summary>
    public ErgebnisViewModel CurrentErgebnis
    {
      get
      {
        return this.currentErgebnis;
      }

      set
      {
        this.currentErgebnis = value;
        this.RaisePropertyChanged("CurrentErgebnis");
        this.DeleteErgebnisCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string AufgabeBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "AufgabeBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("AufgabeBezeichnung");
      }
    }

    /// <summary>
    /// Holt die Spaltenüberschrift für diese Aufgabe
    /// </summary>
    [DependsUpon("AufgabeBezeichnung")]
    [DependsUpon("AbfolgeIndex")]
    [DependsUpon("AufgabeMaxPunkte")]
    public string AufgabeColumnHeader
    {
      get
      {
        var aufgabenName = this.AufgabeBezeichnung.Trim() == string.Empty
                             ? "Nr." + this.AbfolgeIndex
                             : this.AufgabeBezeichnung;
        var header = aufgabenName + " (" + this.AufgabeMaxPunkte + "P)";
        return header;
      }
    }

    /// <summary>
    /// Holt oder setzt die LfdNr
    /// </summary>
    public int AbfolgeIndex
    {
      get
      {
        return this.Model.LfdNr;
      }

      set
      {
        if (value == this.Model.LfdNr) return;
        this.UndoablePropertyChanging(this, "AbfolgeIndex", this.Model.LfdNr, value);
        this.Model.LfdNr = value;
        this.RaisePropertyChanged("AbfolgeIndex");
      }
    }

    /// <summary>
    /// Holt oder setzt die MaxPunkte
    /// </summary>
    public float AufgabeMaxPunkte
    {
      get
      {
        return this.Model.MaxPunkte;
      }

      set
      {
        if (value == this.Model.MaxPunkte) return;
        this.UndoablePropertyChanging(this, "AufgabeMaxPunkte", this.Model.MaxPunkte, value);
        this.Model.MaxPunkte = value;
        this.RaisePropertyChanged("AufgabeMaxPunkte");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Aufgabe: " + this.AufgabeBezeichnung;
    }

    /// <summary>
    /// Mit dieser Methode wird die aktuelle Sequenz bearbeitet.
    /// </summary>
    private void EditAufgabe()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Aufgabe {0} geändert.", this), false))
      {
        var dlg = new AddAufgabeDialog { Aufgabe = this };
        dlg.ShowDialog();
      }

      App.MainViewModel.Arbeiten.First(o => o.Model == this.Model.Arbeit).UpdateNoten();
    }

    private void ErgebnisPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ErgebnisIstNachgereicht")
      {
      }
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
    /// Handles deletion of the current Ergebnisntendenz
    /// </summary>
    private void DeleteCurrentErgebnis()
    {
      this.DeleteErgebnis(this.CurrentErgebnis);
    }

    /// <summary>
    /// Löscht das gegeben Ergebnis
    /// </summary>
    /// <param name="ergebnisViewModel"> The ergebnis View Model to delete </param>
    private void DeleteErgebnis(ErgebnisViewModel ergebnisViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Ergebnis {0} gelöscht.", ergebnisViewModel), false))
      {
        App.MainViewModel.Ergebnisse.RemoveTest(ergebnisViewModel);
        ergebnisViewModel.PropertyChanged -= this.ErgebnisPropertyChanged;
        var result = this.Ergebnisse.RemoveTest(ergebnisViewModel);
      }
    }

    /// <summary>
    /// Erstellt ein neues Ergebnis
    /// </summary>
    private void AddErgebnis()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Ergebnis erstellt."), false))
      {
        var ergebnis = new Ergebnis { Punktzahl = 0, Aufgabe = this.Model };
        var vm = new ErgebnisViewModel(ergebnis);
        vm.PropertyChanged += this.ErgebnisPropertyChanged;
        App.MainViewModel.Ergebnisse.Add(vm);
        this.Ergebnisse.Add(vm);
        this.CurrentErgebnis = vm;
      }
    }
  }
}
