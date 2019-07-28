namespace SoftTeach.ViewModel.Sitzpläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Printing;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Documents;

  using GongSolutions.Wpf.DragDrop;

  using SoftTeach.View.Personen;
  using SoftTeach.Model.EntityFramework;
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
    /// Der Schülerliste der zu diesem Sitzplan gehört
    /// </summary>
    private SchülerlisteViewModel schülerliste;

    /// <summary>
    /// The schüler currently selected
    /// </summary>
    private SitzplaneintragViewModel currentSitzplaneintrag;

    private double sitzplanDrehung;

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

      this.SitzplanNeuEinteilenCommand = new DelegateCommand(this.SitzplanNeuEinteilen);
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
      }
      else
      {
        // Noch keine Sitzplaneinträge vorhanden, erstelle also alle neu aus den Sitzplätzen des Raumplans
        foreach (var sitzplatz in sitzplan.Raumplan.Sitzplätze)
        {
          var sitzplaneintrag = new Sitzplaneintrag();
          sitzplaneintrag.Sitzplan = this.Model;
          sitzplaneintrag.Sitzplatz = sitzplatz;

          var vm = new SitzplaneintragViewModel(sitzplaneintrag);
          //App.MainViewModel.Sitzplaneinträge.Add(vm);
          this.Sitzplaneinträge.Add(vm);
          this.CurrentSitzplaneintrag = vm;
        }
      }

      this.Sitzplaneinträge.CollectionChanged += this.SitzplaneinträgeCollectionChanged;

      // Build data structures for Schülereinträge
      this.UsedSchülereinträge = new ObservableCollection<SchülereintragViewModel>();
      this.AvailableSchülereinträge = new ObservableCollection<SchülereintragViewModel>();
      foreach (var schülereintrag in sitzplan.Schülerliste.Schülereinträge.OrderBy(o => o.Person.Vorname))
      {
        var schülerId = schülereintrag.Id;
        var vm = App.MainViewModel.Schülereinträge.First(o => o.Model.Id == schülerId);
        if (this.Sitzplaneinträge.Any(o => o.SitzplaneintragSchülereintrag != null && o.SitzplaneintragSchülereintrag == vm))
        {
          this.UsedSchülereinträge.Add(vm);
        }
        else
        {
          this.AvailableSchülereinträge.Add(vm);
        }
      }
    }

    /// <summary>
    /// Holt den Befehl um den Sitzplan neu einzuteilen
    /// </summary>
    public DelegateCommand SitzplanNeuEinteilenCommand { get; private set; }

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
    public ObservableCollection<SchülereintragViewModel> UsedSchülereinträge { get; private set; }

    /// <summary>
    /// Holt die im Sitzplan nicht verwendeten Schülereinträge
    /// </summary>
    public ObservableCollection<SchülereintragViewModel> AvailableSchülereinträge { get; private set; }

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
        this.UndoablePropertyChanging(this, "SitzplanBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("SitzplanBezeichnung");
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

        this.UndoablePropertyChanging(this, "SitzplanRaumplan", this.raumplan, value);
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
        this.UndoablePropertyChanging(this, "SitzplanGültigAb", this.Model.GültigAb, value);
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
        this.UndoablePropertyChanging(this, "SitzplanDrehung", this.sitzplanDrehung, value);
        this.sitzplanDrehung = value;
        this.RaisePropertyChanged("SitzplanDrehung");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob beim Sitzplan Jungen und Mädchen möglichst getrennt sitzen sollen.
    /// </summary>
    public bool? SitzplanMädchenJungeNebeneinander { get; set; }

    private bool nurTeilungsgruppen;

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
    public bool NurTeilungsgruppeA { get; set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob beim Sitzplan nur die Teilungsgruppe B berücksichtigt wird.
    /// </summary>
    public bool NurTeilungsgruppeB { get; set; }

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
        || sourceItem is SitzplaneintragViewModel)
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
        if (sourceItem is SchülereintragViewModel && targetItem is SitzplaneintragViewModel)
        {
          var source = sourceItem as SchülereintragViewModel;
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
    /// Teilt die verbliebenen Schüler auf die Sitzplätze neu auf
    /// </summary>
    public void SitzplanNeuEinteilen()
    {
      if (this.AvailableSchülereinträge.Count == 0)
      {
        this.SitzplanLeeren();
      }

      if (this.NurTeilungsgruppen)
      {
        var schülernachGruppen = this.AvailableSchülereinträge.OrderBy(o => o.SchülereintragPerson.PersonNachname).Chunk(this.AvailableSchülereinträge.Count / 2);
        if (this.NurTeilungsgruppeA)
        {
          this.AvailableSchülereinträge = schülernachGruppen.First().ToObservableCollection();
        }
        else if (this.NurTeilungsgruppeB)
        {
          this.AvailableSchülereinträge = schülernachGruppen.Last().ToObservableCollection();
        }
      }

      if (this.SitzplanMädchenJungeNebeneinander.HasValue)
      {
        var mädchen = this.AvailableSchülereinträge.Where(o => o.SchülereintragPerson.PersonIstWeiblich).Shuffle().ToList();
        var jungen = this.AvailableSchülereinträge.Where(o => !o.SchülereintragPerson.PersonIstWeiblich).Shuffle().ToList();

        var größereGruppe = mädchen.Count >= jungen.Count ? mädchen : jungen;
        var kleinereGruppe = mädchen.Count >= jungen.Count ? jungen : mädchen;
        var gleichviel = (int)Math.Min(mädchen.Count, jungen.Count);
        var restlicheSchülerZahl = mädchen.Count + jungen.Count;
        var restlicheSitzplätze = this.Sitzplaneinträge.Where(o => o.SitzplaneintragSchülereintrag == null).ToList();

        if (this.SitzplanMädchenJungeNebeneinander.Value)
        {
          for (int i = 0; i <= gleichviel * 2; i++)
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

          for (int i = gleichviel * 2 + 1; i < restlicheSchülerZahl; i++)
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
          for (int i = 0; i <= gleichviel * 2; i++)
          {
            if (restlicheSitzplätze.Count > i)
            {
              var useGrößereGruppe = (i / 2) % 2 == 0;
              var schülereintrag = useGrößereGruppe ? größereGruppe[i / 2 + i % 2] : kleinereGruppe[i / 2 + i % 2 - 1];
              restlicheSitzplätze[i].SitzplaneintragSchülereintrag = schülereintrag;
              if (this.AvailableSchülereinträge.Contains(schülereintrag))
              {
                this.AvailableSchülereinträge.Remove(schülereintrag);
                this.UsedSchülereinträge.Add(schülereintrag);
              }
            }
          }

          for (int i = gleichviel * 2 + 1; i < restlicheSchülerZahl; i++)
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
        var restlicheSchüler = this.AvailableSchülereinträge.Shuffle().ToList();
        var restlicheSitzplätze = this.Sitzplaneinträge.Where(o => o.SitzplaneintragSchülereintrag == null).Shuffle().ToList();
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
          //if (this.UsedSchülereinträge.Contains(sitzplaneintragViewModel.SitzplaneintragSchülereintrag))
          //{
          //  this.UsedSchülereinträge.RemoveTest(sitzplaneintragViewModel.SitzplaneintragSchülereintrag);
          //  //this.AvailableSchülereinträge.Add(sitzplaneintragViewModel.SitzplaneintragSchülereintrag);
          //}

          sitzplaneintragViewModel.SitzplaneintragSchülereintrag = null;
        }
      }

      this.UsedSchülereinträge.Clear();

      foreach (var schülereintrag in this.Model.Schülerliste.Schülereinträge.OrderBy(o => o.Person.Vorname))
      {
        var schülerId = schülereintrag.Id;
        var vm = App.MainViewModel.Schülereinträge.First(o => o.Model.Id == schülerId);
        if (!this.Sitzplaneinträge.Any(o => o.SitzplaneintragSchülereintrag != null && o.SitzplaneintragSchülereintrag == vm))
        {
          this.AvailableSchülereinträge.Add(vm);
        }
      }

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
      this.UndoableCollectionChanged(this, "Sitzplaneinträge", this.Sitzplaneinträge, e, false, "Änderung der Sitzplaneinträge");
    }
  }
}
