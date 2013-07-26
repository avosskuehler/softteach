namespace Liduv.ViewModel.Personen
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;

  using Liduv.Model.EntityFramework;
  using Liduv.UndoRedo;
  using Liduv.View.Personen;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Noten;

  /// <summary>
  /// ViewModel of an individual Schülerliste
  /// </summary>
  public class SchülerlisteViewModel : ViewModelBase, IComparable, ICloneable
  {
    /// <summary>
    /// The schüler currently selected
    /// </summary>
    private SchülereintragViewModel currentSchülereintrag;

    /// <summary>
    /// The jahrtyp currently assigned to this Schülerliste
    /// </summary>
    private JahrtypViewModel jahrtyp;

    /// <summary>
    /// The halbjahrtyp currently assigned to this Schülerliste
    /// </summary>
    private HalbjahrtypViewModel halbjahrtyp;

    /// <summary>
    /// The klasse currently assigned to this Schülerliste
    /// </summary>
    private KlasseViewModel klasse;

    /// <summary>
    /// The fach currently assigned to this Schülerliste
    /// </summary>
    private FachViewModel fach;

    /// <summary>
    /// The notenWichtung currently assigned to this Schülerliste
    /// </summary>
    private NotenWichtungViewModel notenWichtung;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchülerlisteViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schülerliste">
    /// The underlying schülerliste this ViewModel is to be based on
    /// </param>
    public SchülerlisteViewModel(Schülerliste schülerliste)
    {
      if (schülerliste == null)
      {
        throw new ArgumentNullException("schülerliste");
      }

      this.Model = schülerliste;

      this.AddSchülereintragCommand = new DelegateCommand(this.AddSchülereintrag);
      this.DeleteSchülereintragCommand = new DelegateCommand(this.DeleteCurrentSchülereintrag, () => this.CurrentSchülereintrag != null);

      // Build data structures for schülerlisten
      this.Schülereinträge = new ObservableCollection<SchülereintragViewModel>();
      foreach (var schülereintrag in schülerliste.Schülereinträge)//.OrderBy(o => o.Person.Nachname))
      {
        var vm = new SchülereintragViewModel(schülereintrag);
        App.MainViewModel.Schülereinträge.Add(vm);
        this.Schülereinträge.Add(vm);
      }

      this.Schülereinträge.CollectionChanged += this.SchülereinträgeCollectionChanged;

      this.SchülereinträgeView = CollectionViewSource.GetDefaultView(this.Schülereinträge);
      this.SchülereinträgeView.SortDescriptions.Add(new SortDescription("SchülereintragSortByNachnameProperty", ListSortDirection.Ascending));
      this.SchülereinträgeView.Refresh();

      App.MainViewModel.NotenWichtungen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.SchülerlisteNotenWichtung))
        {
          this.SchülerlisteNotenWichtung = App.MainViewModel.NotenWichtungen.FirstOrDefault();
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
    /// Holt den underlying Schülerliste this ViewModel is based on
    /// </summary>
    public Schülerliste Model { get; private set; }

    /// <summary>
    /// Holt den schülereinträge for this schülerliste
    /// </summary>
    public ObservableCollection<SchülereintragViewModel> Schülereinträge { get; private set; }

    /// <summary>
    /// Holt oder setzt die sortierten Phasen
    /// </summary>
    public ICollectionView SchülereinträgeView { get; set; }

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
    /// Holt oder setzt die Jahrtyp currently assigned to this Schülerliste
    /// </summary>
    public JahrtypViewModel SchülerlisteJahrtyp
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
          this.jahrtyp = App.MainViewModel.Jahrtypen.SingleOrDefault(d => d.Model == this.Model.Jahrtyp);
        }

        return this.jahrtyp;
      }

      set
      {
        if (value.JahrtypBezeichnung == this.jahrtyp.JahrtypBezeichnung) return;
        this.UndoablePropertyChanging(this, "SchülerlisteJahrtyp", this.jahrtyp, value);
        this.jahrtyp = value;
        this.Model.Jahrtyp = value.Model;
        this.RaisePropertyChanged("SchülerlisteJahrtyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die Halbjahrtyp currently assigned to this Schülerliste
    /// </summary>
    public HalbjahrtypViewModel SchülerlisteHalbjahrtyp
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
        this.UndoablePropertyChanging(this, "SchülerlisteHalbjahrtyp", this.halbjahrtyp, value);
        this.halbjahrtyp = value;
        this.Model.Halbjahrtyp = value.Model;
        this.RaisePropertyChanged("SchülerlisteHalbjahrtyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die Klasse currently assigned to this Schülerliste
    /// </summary>
    public KlasseViewModel SchülerlisteKlasse
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
        this.UndoablePropertyChanging(this, "SchülerlisteKlasse", this.klasse, value);
        this.klasse = value;
        this.Model.Klasse = value.Model;
        this.RaisePropertyChanged("SchülerlisteKlasse");
      }
    }

    /// <summary>
    /// Holt oder setzt die Fach currently assigned to this Schülerliste
    /// </summary>
    public FachViewModel SchülerlisteFach
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
        this.UndoablePropertyChanging(this, "SchülerlisteFach", this.fach, value);
        this.fach = value;
        this.Model.Fach = value.Model;
        this.RaisePropertyChanged("SchülerlisteFach");
      }
    }

    /// <summary>
    /// Holt oder setzt die Halbjahrtyp currently assigned to this Schülerliste
    /// </summary>
    public NotenWichtungViewModel SchülerlisteNotenWichtung
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
        this.UndoablePropertyChanging(this, "SchülerlisteNotenWichtung", this.notenWichtung, value);
        this.notenWichtung = value;
        this.Model.NotenWichtung = value.Model;
        this.RaisePropertyChanged("SchülerlisteNotenWichtung");
      }
    }

    /// <summary>
    /// Holt den header for the list of pupils in this class
    /// </summary>
    [DependsUpon("SchülerlisteKlasse")]
    [DependsUpon("SchülerlisteFach")]
    [DependsUpon("SchülerlisteHalbjahrtyp")]
    [DependsUpon("SchülerlisteJahrtyp")]
    public string SchülerlisteÜberschrift
    {
      get
      {
        if (this.SchülerlisteKlasse == null)
        {
          return string.Empty;
        }

        return "SchülerInnen der Klasse " + this.SchülerlisteKlasse.KlasseBezeichnung + " in " +
          this.SchülerlisteFach.FachBezeichnung + " im " +
          this.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung + " " +
          this.SchülerlisteJahrtyp.JahrtypBezeichnung;
      }
    }

    /// <summary>
    /// Holt den short header for the list of pupils in this class
    /// </summary>
    [DependsUpon("SchülerlisteKlasse")]
    [DependsUpon("SchülerlisteFach")]
    [DependsUpon("SchülerlisteHalbjahrtyp")]
    [DependsUpon("SchülerlisteJahrtyp")]
    public string SchülerlisteKurzbezeichnung
    {
      get
      {
        if (this.SchülerlisteKlasse == null)
        {
          return string.Empty;
        }

        return "Klasse " + this.SchülerlisteKlasse.KlasseBezeichnung + ", " +
          this.SchülerlisteFach.FachBezeichnung + ", " +
          this.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung + " " +
          this.SchülerlisteJahrtyp.JahrtypBezeichnung;
      }
    }

    /// <summary>
    /// Holt die Überschrift für die Notenliste dieser Schülerliste
    /// </summary>
    [DependsUpon("SchülerlisteKlasse")]
    [DependsUpon("SchülerlisteFach")]
    [DependsUpon("SchülerlisteHalbjahrtyp")]
    [DependsUpon("SchülerlisteJahrtyp")]
    public string NotenlisteTitel
    {
      get
      {
        return "Noten für " + this.SchülerlisteKlasse.KlasseBezeichnung + ", " +
          this.SchülerlisteFach.FachBezeichnung + ", " +
          this.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung + " " +
          this.SchülerlisteJahrtyp.JahrtypBezeichnung;
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
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherSchülerlisteViewModel = obj as SchülerlisteViewModel;
      if (otherSchülerlisteViewModel != null)
      {
        return StringLogicalComparer.Compare(
          this.SchülerlisteKlasse.KlasseBezeichnung,
          otherSchülerlisteViewModel.SchülerlisteKlasse.KlasseBezeichnung);
      }

      throw new ArgumentException("Object is not a SchülerlisteViewModel");
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Schülerliste: " + this.SchülerlisteKurzbezeichnung;
    }

    /// <summary>
    /// Dupliziert diese Schülerliste, aber mit leeren Eigenschaften in
    /// den Schülereinträgen.
    /// </summary>
    /// <returns>Eine Schülerliste mit den selben Eigenschaften,
    /// aber leeren Schülereinträgen.</returns>
    public object Clone()
    {
      var schülerlisteClone = new Schülerliste();
      schülerlisteClone.Klasse = this.SchülerlisteKlasse.Model;
      schülerlisteClone.Jahrtyp = this.SchülerlisteJahrtyp.Model;
      schülerlisteClone.Halbjahrtyp = this.SchülerlisteHalbjahrtyp.Model;
      schülerlisteClone.Fach = this.SchülerlisteFach.Model;
      schülerlisteClone.NotenWichtung = this.SchülerlisteNotenWichtung.Model;
      foreach (var schülereintragViewModel in this.Schülereinträge.OrderBy(o => o.SchülereintragPerson.PersonNachname))
      {
        var schülereintragClone = new Schülereintrag();
        schülereintragClone.Person = schülereintragViewModel.Model.Person;
        schülereintragClone.Schülerliste = schülerlisteClone;
        schülerlisteClone.Schülereinträge.Add(schülereintragClone);
      }

      var vm = new SchülerlisteViewModel(schülerlisteClone);
      App.MainViewModel.Schülerlisten.Add(vm);
      return vm;
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
        using (new UndoBatch(App.MainViewModel, string.Format("Neue Schüler in Schülerlist {0} eingetragen.", this), false))
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
                  o.SchülereintragPerson.PersonVorname == person.PersonVorname
                  && o.SchülereintragPerson.PersonNachname == person.PersonNachname
                  && o.SchülereintragPerson.PersonGeburtstag == person.PersonGeburtstag))
              {
                continue;
              }

              // perform add
              var schülereintrag = new Schülereintrag();
              schülereintrag.Person = person.Model;
              schülereintrag.Schülerliste = this.Model;
              var vm = new SchülereintragViewModel(schülereintrag);
              App.MainViewModel.Schülereinträge.Add(vm);
              this.Schülereinträge.Add(vm);
              this.CurrentSchülereintrag = vm;
            }
          }
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current schülereintrag
    /// </summary>
    private void DeleteCurrentSchülereintrag()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Schüler {0} aus Schülerliste {1} gelöscht.", this.CurrentSchülereintrag, this), false))
      {
        App.MainViewModel.Schülereinträge.RemoveTest(this.CurrentSchülereintrag);
        this.Schülereinträge.RemoveTest(this.CurrentSchülereintrag);
        this.CurrentSchülereintrag = null;
      }
    }

    /// <summary>
    /// Tritt auf, wenn die SchülereinträgeCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SchülereinträgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Schülereinträge", this.Schülereinträge, e, false, "Änderung der Schülereinträge");
      this.RaisePropertyChanged("Schülerzahl");
    }


    internal void UpdateSort()
    {
      this.SchülereinträgeView.SortDescriptions.Clear();
      this.SchülereinträgeView.SortDescriptions.Add(new SortDescription("SchülereintragSortByNachnameProperty", ListSortDirection.Ascending));
      this.SchülereinträgeView.Refresh();
    }
  }
}