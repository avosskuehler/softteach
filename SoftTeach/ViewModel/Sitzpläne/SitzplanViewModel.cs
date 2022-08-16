namespace SoftTeach.ViewModel.Sitzpläne
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;

  using GongSolutions.Wpf.DragDrop;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Sitzpläne;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Noten;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// ViewModel of an individual sitzplan
  /// </summary>
  public class SitzplanViewModel : ViewModelBase, IComparable, IDropTarget
  {
    /// <summary>
    /// Der Raumplan der zu diesem Sitzplan gehört
    /// </summary>
    private RaumplanViewModel raumplan;

    /// <summary>
    /// Der Lerngruppe der zu diesem Sitzplan gehört
    /// </summary>
    private LerngruppeViewModel schülerliste;

    /// <summary>
    /// The schüler currently selected
    /// </summary>
    private SitzplaneintragViewModel currentSitzplaneintrag;

    private double sitzplanDrehung;
    private bool nurTeilungsgruppen;
    private bool nurTeilungsgruppeA;
    private bool nurTeilungsgruppeB;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="sitzplan">
    /// The underlying sitzplan this ViewModel is to be based on
    /// </param>
    public SitzplanViewModel(Sitzplan sitzplan)
    {
      this.Model = sitzplan ?? throw new ArgumentNullException(nameof(sitzplan));

      this.SitzplanEinteilenCommand = new DelegateCommand(this.SitzplanEinteilen);
      this.SitzplanLeerenCommand = new DelegateCommand(this.SitzplanLeeren);
      this.SitzplanAusdruckenCommand = new DelegateCommand(this.SitzplanAusdrucken);

      // Build data structures for schülerlisten
      this.Sitzplaneinträge = new ObservableCollection<SitzplaneintragViewModel>();

      if (sitzplan.Sitzplaneinträge.Any())
      {
        foreach (var sitzplaneintrag in sitzplan.Sitzplaneinträge)
        {
          var vm = new SitzplaneintragViewModel(sitzplaneintrag);
          //App.MainViewModel.Sitzplaneinträge.Add(vm);
          this.Sitzplaneinträge.Add(vm);
        }
        this.Sitzplaneinträge.CollectionChanged += this.SitzplaneinträgeCollectionChanged;
      }
      else
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Neuer Sitzplan angelegt"), false))
        {
          this.Sitzplaneinträge.CollectionChanged += this.SitzplaneinträgeCollectionChanged;

          // Noch keine Sitzplaneinträge vorhanden, erstelle also alle  aus den Sitzplätzen des Raumplans
          foreach (var sitzplatz in sitzplan.Raumplan.Sitzplätze)
          {
            var sitzplaneintrag = new Sitzplaneintrag
            {
              Sitzplan = this.Model,
              Sitzplatz = sitzplatz
            };
            //App.UnitOfWork.Context.Sitzplaneinträge.Add(sitzplaneintrag);

            var vm = new SitzplaneintragViewModel(sitzplaneintrag);
            //App.MainViewModel.Sitzplaneinträge.Add(vm);
            this.Sitzplaneinträge.Add(vm);
            this.CurrentSitzplaneintrag = vm;
          }
        }
      }


      // Build data structures for Schülereinträge
      this.UsedSchülereinträge = new ObservableCollection<Schülereintrag>();
      this.AvailableSchülereinträge = new ObservableCollection<Schülereintrag>();
      foreach (var schülereintrag in sitzplan.Lerngruppe.Schülereinträge.OrderBy(o => o.Person.Vorname))
      {
        //var schülerId = schülereintrag.Id;
        //var vm = App.MainViewModel.Schülereinträge.First(o => o.Model.Id == schülerId);
        if (this.Sitzplaneinträge.Any(o => o.SitzplaneintragSchülereintrag != null && o.SitzplaneintragSchülereintrag == schülereintrag))
        {
          this.UsedSchülereinträge.Add(schülereintrag);
        }
        else
        {
          this.AvailableSchülereinträge.Add(schülereintrag);
        }
      }

      this.NurTeilungsgruppeA = true;
    }

    /// <summary>
    /// Holt den Befehl um den Sitzplan  einzuteilen
    /// </summary>
    public DelegateCommand SitzplanEinteilenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um den Sitzplan zu leeren einzuteilen
    /// </summary>
    public DelegateCommand SitzplanLeerenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl um den Sitzplan auszudrucken
    /// </summary>
    public DelegateCommand SitzplanAusdruckenCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Sitzplan this ViewModel is based on
    /// </summary>
    public Sitzplan Model { get; private set; }

    /// <summary>
    /// Holt die schülereinträge for this schülerliste
    /// </summary>
    public ObservableCollection<SitzplaneintragViewModel> Sitzplaneinträge { get; private set; }

    /// <summary>
    /// Holt die im Sitzplan verwendeten Schülereinträge
    /// </summary>
    public ObservableCollection<Schülereintrag> UsedSchülereinträge { get; private set; }

    /// <summary>
    /// Holt die im Sitzplan nicht verwendeten Schülereinträge
    /// </summary>
    public ObservableCollection<Schülereintrag> AvailableSchülereinträge { get; private set; }

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
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string SitzplanBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(SitzplanBezeichnung), this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("SitzplanBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt den Lerngruppe für den Lerngruppeplan
    /// </summary>
    public LerngruppeViewModel SitzplanLerngruppe
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Lerngruppe == null)
        {
          return null;
        }

        if (this.schülerliste == null || this.schülerliste.Model != this.Model.Lerngruppe)
        {
          this.schülerliste = App.MainViewModel.Lerngruppen.SingleOrDefault(d => d.Model == this.Model.Lerngruppe);
        }

        return this.schülerliste;
      }

      set
      {
        if (value == null) return;
        if (this.schülerliste != null)
        {
          if (value.LerngruppeÜberschrift == this.schülerliste.LerngruppeÜberschrift) return;
        }

        this.UndoablePropertyChanging(this, nameof(SitzplanLerngruppe), this.schülerliste, value);
        this.schülerliste = value;
        this.Model.Lerngruppe = value.Model;
        this.RaisePropertyChanged("SitzplanLerngruppe");
      }
    }

    /// <summary>
    /// Holt oder setzt den Raumplan für den Sitzplan
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
          this.raumplan = new RaumplanViewModel(this.Model.Raumplan);
        }

        return this.raumplan;
      }

      set
      {
        if (value == null) return;
        if (this.raumplan != null)
        {
          if (value.RaumplanBezeichnung == this.raumplan.RaumplanBezeichnung) return;
        }

        this.UndoablePropertyChanging(this, nameof(SitzplanRaumplan), this.raumplan, value);
        this.raumplan = value;
        this.Model.Raumplan = value.Model;
        this.RaisePropertyChanged("SitzplanRaumplan");
      }
    }

    /// <summary>
    /// Holt oder setzt das Datum der Gültigkeit
    /// </summary>
    public DateTime SitzplanGültigAb
    {
      get
      {
        return this.Model.GültigAb;
      }

      set
      {
        if (value == this.Model.GültigAb) return;
        this.UndoablePropertyChanging(this, nameof(SitzplanGültigAb), this.Model.GültigAb, value);
        this.Model.GültigAb = value;
        this.RaisePropertyChanged("SitzplanGültigAb");
      }
    }

    /// <summary>
    /// Holt das Datum der Gültigkeit im Format DD.MM.YY
    /// </summary>
    [DependsUpon("SitzplanGültigAb")]
    public string SitzplanGültigAbString
    {
      get
      {
        return "gültig ab " + this.Model.GültigAb.ToShortDateString();
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, wie stark der Sitzplan beim Ausdrucken gedreht werden soll.
    /// </summary>
    public double SitzplanDrehung
    {
      get
      {
        return this.sitzplanDrehung;
      }

      set
      {
        if (value == this.sitzplanDrehung) return;
        this.UndoablePropertyChanging(this, nameof(SitzplanDrehung), this.sitzplanDrehung, value);
        this.sitzplanDrehung = value;
        this.RaisePropertyChanged("SitzplanDrehung");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob beim Sitzplan Jungen und Mädchen möglichst getrennt sitzen sollen.
    /// </summary>
    public bool? SitzplanMädchenJungeNebeneinander { get; set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob beim Sitzplan Teilungsgruppen berücksichtigt werden sollen.
    /// </summary>
    public bool NurTeilungsgruppen
    {
      get
      {
        return this.nurTeilungsgruppen;
      }

      set
      {
        if (value == this.nurTeilungsgruppen) return;
        this.nurTeilungsgruppen = value;
        this.RaisePropertyChanged("NurTeilungsgruppen");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob beim Sitzplan nur die Teilungsgruppe A berücksichtigt wird.
    /// </summary>
    public bool NurTeilungsgruppeA
    {
      get => nurTeilungsgruppeA;
      set
      {
        if (nurTeilungsgruppeA == value)
        {
          return;
        }

        nurTeilungsgruppeA = value;
        this.RaisePropertyChanged("NurTeilungsgruppeA");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob beim Sitzplan nur die Teilungsgruppe B berücksichtigt wird.
    /// </summary>
    public bool NurTeilungsgruppeB
    {
      get => nurTeilungsgruppeB;
      set
      {
        if (nurTeilungsgruppeB == value)
        {
          return;
        }

        nurTeilungsgruppeB = value;
        this.RaisePropertyChanged("NurTeilungsgruppeB");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Sitzplan " + this.SitzplanBezeichnung;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="viewModel">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
    /// </returns>
    /// <exception cref="System.ArgumentException">Object is not a SitzplanViewModel</exception>
    public int CompareTo(object viewModel)
    {
      var sitzplanViewModel = viewModel as SitzplanViewModel;
      if (sitzplanViewModel == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, sitzplanViewModel.SitzplanBezeichnung, StringComparison.Ordinal);
    }

    /// <summary>
    /// Kontrolliert mögliche Dragmanöver
    /// </summary>
    /// <param name="dropInfo">The drop information.</param>
    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data;
      var targetItem = dropInfo.TargetItem;
      if ((sourceItem is SchülereintragViewModel && targetItem is SitzplaneintragViewModel)
        || sourceItem is SitzplaneintragViewModel || (sourceItem is Schülereintrag && targetItem is SitzplaneintragViewModel))
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.Effects = DragDropEffects.Move;
      }
      else
      {
        dropInfo.Effects = DragDropEffects.None;
      }
    }

    /// <summary>
    /// Drops the specified drop information.
    /// </summary>
    /// <param name="dropInfo">The drop information.</param>
    public void Drop(IDropInfo dropInfo)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Drag and Drop im Sitzplan"), false))
      {
        var sourceItem = dropInfo.Data;
        var targetItem = dropInfo.TargetItem;
        if (sourceItem is Schülereintrag && targetItem is SitzplaneintragViewModel)
        {
          var source = sourceItem as Schülereintrag;
          var target = targetItem as SitzplaneintragViewModel;
          target.SitzplaneintragSchülereintrag = source;
          if (this.AvailableSchülereinträge.Contains(source))
          {
            this.AvailableSchülereinträge.RemoveTest(source);
            this.UsedSchülereinträge.Add(source);
          }
        }
        else if (sourceItem is SitzplaneintragViewModel && targetItem is SitzplaneintragViewModel)
        {
          var source = sourceItem as SitzplaneintragViewModel;
          var target = targetItem as SitzplaneintragViewModel;

          //if (target.SitzplaneintragSchülereintrag != null && this.UsedSchülereinträge.Contains(target.SitzplaneintragSchülereintrag))
          //{
          //  this.UsedSchülereinträge.RemoveTest(target.SitzplaneintragSchülereintrag);
          //  this.AvailableSchülereinträge.Add(target.SitzplaneintragSchülereintrag);
          //}
          var temp = target.SitzplaneintragSchülereintrag;
          target.SitzplaneintragSchülereintrag = source.SitzplaneintragSchülereintrag;
          source.SitzplaneintragSchülereintrag = temp;
        }
        else if (sourceItem is SitzplaneintragViewModel)
        {
          var source = sourceItem as SitzplaneintragViewModel;

          if (this.UsedSchülereinträge.Contains(source.SitzplaneintragSchülereintrag))
          {
            this.UsedSchülereinträge.RemoveTest(source.SitzplaneintragSchülereintrag);
            this.AvailableSchülereinträge.Add(source.SitzplaneintragSchülereintrag);
          }

          source.SitzplaneintragSchülereintrag = null;
        }
      }
    }


    /// <summary>
    /// Teilt die verbliebenen Schüler auf die Sitzplätze  auf
    /// </summary>
    public void SitzplanEinteilen()
    {
      //if (this.AvailableSchülereinträge.Count == 0)
      {
        this.SitzplanLeeren();
      }

      IEnumerable<Schülereintrag> schülereinträgeZurVerwendung = this.AvailableSchülereinträge.OrderBy(o => o.Person.Nachname);

      if (this.NurTeilungsgruppen)
      {
        var schülernachGruppen = schülereinträgeZurVerwendung.Chunk(this.AvailableSchülereinträge.Count / 2);
        if (this.NurTeilungsgruppeA)
        {
          schülereinträgeZurVerwendung = schülernachGruppen.First();
        }
        else if (this.NurTeilungsgruppeB)
        {
          schülereinträgeZurVerwendung = schülernachGruppen.Last();
        }
      }

      if (this.SitzplanMädchenJungeNebeneinander.HasValue)
      {
        var mädchen = schülereinträgeZurVerwendung.Where(o => o.Person.Geschlecht == Geschlecht.w).ToList();
        mädchen.Shuffle2();
        var jungen = schülereinträgeZurVerwendung.Where(o => o.Person.Geschlecht != Geschlecht.w).ToList();
        jungen.Shuffle2();

        var größereGruppe = mädchen.Count >= jungen.Count ? mädchen : jungen;
        var kleinereGruppe = mädchen.Count >= jungen.Count ? jungen : mädchen;
        var gleichviel = (int)Math.Min(mädchen.Count, jungen.Count);
        var restlicheSchülerZahl = mädchen.Count + jungen.Count;
        var restlicheSitzplätze = this.Sitzplaneinträge.Where(o => o.SitzplaneintragSchülereintrag == null).OrderBy(o => o.SitzplaneintragSitzplatz.Reihenfolge).ToList();

        if (this.SitzplanMädchenJungeNebeneinander.Value)
        {
          for (int i = 0; i < gleichviel * 2; i++)
          {
            if (restlicheSitzplätze.Count > i)
            {
              var schüler = i % 2 == 0 ? größereGruppe[i / 2] : kleinereGruppe[i / 2];
              restlicheSitzplätze[i].SitzplaneintragSchülereintrag = schüler;
              if (this.AvailableSchülereinträge.Contains(schüler))
              {
                this.AvailableSchülereinträge.Remove(schüler);
                this.UsedSchülereinträge.Add(schüler);
              }
            }
          }

          for (int i = gleichviel * 2; i < restlicheSchülerZahl; i++)
          {
            if (restlicheSitzplätze.Count > i)
            {
              var schüler = größereGruppe[i - gleichviel];
              restlicheSitzplätze[i].SitzplaneintragSchülereintrag = schüler;
              if (this.AvailableSchülereinträge.Contains(schüler))
              {
                this.AvailableSchülereinträge.Remove(schüler);
                this.UsedSchülereinträge.Add(schüler);
              }
            }
          }
        }
        else
        {
          for (int i = 0; i < gleichviel * 2; i++)
          {
            if (restlicheSitzplätze.Count > i)
            {
              var useGrößereGruppe = i % 2 == 0;
              var schülereintrag = useGrößereGruppe ? größereGruppe[i / 2 + i % 2] : kleinereGruppe[i / 2 + i % 2 - 1];
              restlicheSitzplätze[i].SitzplaneintragSchülereintrag = schülereintrag;
              if (this.AvailableSchülereinträge.Contains(schülereintrag))
              {
                this.AvailableSchülereinträge.Remove(schülereintrag);
                this.UsedSchülereinträge.Add(schülereintrag);
              }
            }
          }

          for (int i = gleichviel * 2; i < restlicheSchülerZahl; i++)
          {
            if (restlicheSitzplätze.Count > i)
            {
              var schüler = größereGruppe[i - gleichviel];
              restlicheSitzplätze[i].SitzplaneintragSchülereintrag = schüler;
              if (this.AvailableSchülereinträge.Contains(schüler))
              {
                this.AvailableSchülereinträge.Remove(schüler);
                this.UsedSchülereinträge.Add(schüler);
              }
            }
          }

        }
      }
      else
      {
        var restlicheSchüler = schülereinträgeZurVerwendung.ToList();
        restlicheSchüler.Shuffle2();

        var restlicheSitzplätze = this.Sitzplaneinträge.Where(o => o.SitzplaneintragSchülereintrag == null).OrderBy(o => o.SitzplaneintragSitzplatz.Reihenfolge).ToList();

        for (int i = 0; i < restlicheSchüler.Count(); i++)
        {
          if (restlicheSitzplätze.Count > i)
          {
            var schüler = restlicheSchüler[i];
            restlicheSitzplätze[i].SitzplaneintragSchülereintrag = schüler;
            this.AvailableSchülereinträge.Remove(schüler);
            this.UsedSchülereinträge.Add(schüler);
          }
        }
      }
    }

    /// <summary>
    /// Leert den Sitzplan
    /// </summary>
    public void SitzplanLeeren()
    {
      foreach (var sitzplaneintragViewModel in this.Sitzplaneinträge)
      {
        if (sitzplaneintragViewModel.SitzplaneintragSchülereintrag != null)
        {
          if (this.UsedSchülereinträge.Contains(sitzplaneintragViewModel.SitzplaneintragSchülereintrag))
          {
            this.UsedSchülereinträge.RemoveTest(sitzplaneintragViewModel.SitzplaneintragSchülereintrag);
            this.AvailableSchülereinträge.Add(sitzplaneintragViewModel.SitzplaneintragSchülereintrag);
          }

          sitzplaneintragViewModel.SitzplaneintragSchülereintrag = null;
        }
      }

      //this.UsedSchülereinträge.Clear();

      //foreach (var schülereintrag in this.Model.Lerngruppe.Schülereinträge.OrderBy(o => o.Person.Vorname))
      //{
      //  var schülerId = schülereintrag.Id;
      //  //var vm = App.MainViewModel.Schülereinträge.First(o => o.Model.Id == schülerId);
      //  if (!this.Sitzplaneinträge.Any(o => o.SitzplaneintragSchülereintrag != null && o.SitzplaneintragSchülereintrag == schülereintrag))
      //  {
      //    this.AvailableSchülereinträge.Add(schülereintrag);
      //  }
      //}

    }

    /// <summary>
    /// Druckt den momentanen Sitzplan aus
    /// </summary>
    public void SitzplanAusdrucken()
    {
      // select printer and get printer settings
      var pd = new PrintDialog();
      if (pd.ShowDialog() != true)
      {
        return;
      }

      if (Configuration.Instance.IsMetroMode)
      {
        // create the print output usercontrol
        var content = new MetroSitzplanPrintView { DataContext = this, Width = pd.PrintableAreaWidth, Height = pd.PrintableAreaHeight };
        var title = "Sitzplaneinteilung für " + this.SitzplanBezeichnung;
        pd.PrintVisual(content, title);
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
      UndoableCollectionChanged(this, nameof(Sitzplaneinträge), this.Sitzplaneinträge, e, true, "Änderung der Sitzplaneinträge");
    }
  }
}
