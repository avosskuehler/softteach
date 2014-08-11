namespace Liduv.ViewModel.Sitzpläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;

  using Liduv.Model.EntityFramework;
  using Liduv.UndoRedo;
  using Liduv.View.Personen;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Noten;
  using Liduv.ViewModel.Personen;

  /// <summary>
  /// ViewModel of an individual sitzplan
  /// </summary>
  public class SitzplanViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Der Raumplan der zu diesem Sitzplan gehört
    /// </summary>
    private RaumplanViewModel raumplan;
    
    /// <summary>
    /// Der Schülerliste der zu diesem Sitzplan gehört
    /// </summary>
    private SchülerlisteViewModel schülerliste;

    /// <summary>
    /// The schüler currently selected
    /// </summary>
    private SitzplaneintragViewModel currentSitzplaneintrag;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="sitzplan">
    /// The underlying sitzplan this ViewModel is to be based on
    /// </param>
    public SitzplanViewModel(Sitzplan sitzplan)
    {
      if (sitzplan == null)
      {
        throw new ArgumentNullException("sitzplan");
      }

      this.Model = sitzplan;

      this.AddSitzplaneintragCommand = new DelegateCommand(this.AddSitzplaneintrag);
      this.DeleteSitzplaneintragCommand = new DelegateCommand(this.DeleteCurrentSitzplaneintrag, () => this.CurrentSitzplaneintrag != null);

      // Build data structures for schülerlisten
      this.Sitzplaneinträge = new ObservableCollection<SitzplaneintragViewModel>();
      foreach (var sitzplaneintrag in sitzplan.Sitzplaneinträge)
      {
        var vm = new SitzplaneintragViewModel(sitzplaneintrag);
        App.MainViewModel.Sitzplaneinträge.Add(vm);
        this.Sitzplaneinträge.Add(vm);
      }

      this.Sitzplaneinträge.CollectionChanged += this.SitzplaneinträgeCollectionChanged;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Schüler
    /// </summary>
    public DelegateCommand AddSitzplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Schüler
    /// </summary>
    public DelegateCommand DeleteSitzplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Sitzplan this ViewModel is based on
    /// </summary>
    public Sitzplan Model { get; private set; }

    /// <summary>
    /// Holt die schülereinträge for this schülerliste
    /// </summary>
    public ObservableCollection<SitzplaneintragViewModel> Sitzplaneinträge { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected sitzplaneintrag
    /// </summary>
    public SitzplaneintragViewModel CurrentSitzplaneintrag
    {
      get
      {
        return this.currentSitzplaneintrag;
      }

      set
      {
        this.currentSitzplaneintrag = value;
        this.RaisePropertyChanged("CurrentSitzplaneintrag");
        this.DeleteSitzplaneintragCommand.RaiseCanExecuteChanged();
      }
    }


    /// <summary>
    /// Holt oder setzt den Schülerliste für den Schülerlisteplan
    /// </summary>
    public SchülerlisteViewModel SitzplanSchülerliste
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Schülerliste == null)
        {
          return null;
        }

        if (this.schülerliste == null || this.schülerliste.Model != this.Model.Schülerliste)
        {
          this.schülerliste = App.MainViewModel.Schülerlisten.SingleOrDefault(d => d.Model == this.Model.Schülerliste);
        }

        return this.schülerliste;
      }

      set
      {
        if (value == null) return;
        if (this.schülerliste != null)
        {
          if (value.SchülerlisteÜberschrift == this.schülerliste.SchülerlisteÜberschrift) return;
        }

        this.UndoablePropertyChanging(this, "SitzplanSchülerliste", this.schülerliste, value);
        this.schülerliste = value;
        this.Model.Schülerliste = value.Model;
        this.RaisePropertyChanged("SitzplanSchülerliste");
      }
    }

    /// <summary>
    /// Holt oder setzt den Raumplan für den Raumplanplan
    /// </summary>
    public RaumplanViewModel SitzplanRaumplan
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Raumplan == null)
        {
          return null;
        }

        if (this.raumplan == null || this.raumplan.Model != this.Model.Raumplan)
        {
          this.raumplan = App.MainViewModel.Raumpläne.SingleOrDefault(d => d.Model == this.Model.Raumplan);
        }

        return this.raumplan;
      }

      set
      {
        if (value == null) return;
        if (this.raumplan != null)
        {
          if (value.RaumplanDateiname == this.raumplan.RaumplanDateiname) return;
        }

        this.UndoablePropertyChanging(this, "SitzplanRaumplan", this.raumplan, value);
        this.raumplan = value;
        this.Model.Raumplan = value.Model;
        this.RaisePropertyChanged("SitzplanRaumplan");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Sitzplan";
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
    /// </returns>
    /// <exception cref="System.ArgumentException">Object is not a SitzplanViewModel</exception>
    public int CompareTo(object obj)
    {
      var otherSitzplanViewModel = obj as SitzplanViewModel;
      if (otherSitzplanViewModel != null)
      {
        return this.Model.Id == otherSitzplanViewModel.Model.Id ? 0 : 1;
      }

      throw new ArgumentException("Object is not a SitzplanViewModel");

    }

    /// <summary>
    /// Handles addition a new sitzplaneintrag to this schülerliste
    /// </summary>
    private void AddSitzplaneintrag()
    {
      //// Show a dialog which has multiselect option
      //var dlg = new SelectSchülerDialog();
      //if (dlg.ShowDialog().GetValueOrDefault(false))
      //{
      //  using (new UndoBatch(App.MainViewModel, string.Format("Neue Schüler in Schülerliste {0} eingetragen.", this), false))
      //  {
      //    foreach (var obj in dlg.SelectedSchüler)
      //    {
      //      // Care for empty rows
      //      if (obj is PersonViewModel)
      //      {
      //        // Cast to valid object
      //        var person = obj as PersonViewModel;

      //        // Check if already there
      //        if (
      //          this.Sitzplaneinträge.Any(
      //            o =>
      //            o.SitzplaneintragPerson.PersonVorname == person.PersonVorname
      //            && o.SitzplaneintragPerson.PersonNachname == person.PersonNachname
      //            && o.SitzplaneintragPerson.PersonGeburtstag == person.PersonGeburtstag))
      //        {
      //          continue;
      //        }

      //        // perform add
      //        var sitzplaneintrag = new Sitzplaneintrag();
      //        sitzplaneintrag.Person = person.Model;
      //        sitzplaneintrag.Schülerliste = this.Model;
      //        var vm = new SitzplaneintragViewModel(sitzplaneintrag);
      //        App.MainViewModel.Sitzplaneinträge.Add(vm);
      //        this.Sitzplaneinträge.Add(vm);
      //        this.CurrentSitzplaneintrag = vm;
      //      }
      //    }
      //  }
      //}
    }

    /// <summary>
    /// Handles deletion of the current sitzplaneintrag
    /// </summary>
    private void DeleteCurrentSitzplaneintrag()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sitzplaneintrag {0} aus Sitzplan {1} gelöscht.", this.CurrentSitzplaneintrag, this), false))
      {
        App.MainViewModel.Sitzplaneinträge.RemoveTest(this.CurrentSitzplaneintrag);
        this.Sitzplaneinträge.RemoveTest(this.CurrentSitzplaneintrag);
        this.CurrentSitzplaneintrag = null;
      }
    }

    /// <summary>
    /// Tritt auf, wenn die SitzplaneinträgeCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SitzplaneinträgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Sitzplaneinträge", this.Sitzplaneinträge, e, false, "Änderung der Sitzplaneinträge");
    }


  }
}
