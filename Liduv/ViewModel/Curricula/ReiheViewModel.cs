﻿namespace Liduv.ViewModel.Curricula
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows.Input;

  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Curricula;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual reihe
  /// </summary>
  public class ReiheViewModel : SequencedViewModel
  {
    /// <summary>
    /// The modul currently assigned to this reihe
    /// </summary>
    private ModulViewModel modul;

    /// <summary>
    /// The sequenz currently selected
    /// </summary>
    private SequenzViewModel currentSequenz;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ReiheViewModel"/> Klasse.
    /// </summary>
    /// <param name="reihe">
    /// The underlying reihe this ViewModel is to be based on
    /// </param>
    public ReiheViewModel(Reihe reihe)
    {
      if (reihe == null)
      {
        throw new ArgumentNullException("reihe");
      }

      this.Model = reihe;

      // Build data structures for sequenzen
      this.UsedSequenzen = new ObservableCollection<SequenzViewModel>();
      this.AvailableSequenzen = new ObservableCollection<SequenzViewModel>();

      // Sort Sequenzen by AbfolgeIndex
      foreach (var sequenz in reihe.Sequenzen.OrderBy(o => o.AbfolgeIndex))
      {
        var vm = new SequenzViewModel(this, sequenz);
        App.MainViewModel.Sequenzen.Add(vm);
        if (sequenz.AbfolgeIndex == -1)
        {
          this.AvailableSequenzen.Add(vm);
        }
        else
        {
          this.UsedSequenzen.Add(vm);
        }
      }

      // Listen for changes
      this.UsedSequenzen.CollectionChanged += this.UsedSequenzenCollectionChanged;
      this.AvailableSequenzen.CollectionChanged += this.AvailableSequenzenCollectionChanged;

      this.AddSequenzCommand = new DelegateCommand(this.AddSequenz);
      this.DeleteSequenzCommand = new DelegateCommand(this.DeleteCurrentSequenz, () => this.CurrentSequenz != null);
      this.LengthenReiheCommand = new DelegateCommand(this.LengthenReihe);
      this.ShortenReiheCommand = new DelegateCommand(this.ShortenReihe);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ReiheViewModel"/> Klasse.
    /// </summary>
    /// <param name="reihe">
    /// The underlying reihe this ViewModel is to be based on
    /// </param>
    public ReiheViewModel(Reihe reihe, bool notInContext)
    {
      if (reihe == null)
      {
        throw new ArgumentNullException("reihe");
      }

      this.Model = reihe;

      // Build data structures for sequenzen
      this.UsedSequenzen = new ObservableCollection<SequenzViewModel>();
      this.AvailableSequenzen = new ObservableCollection<SequenzViewModel>();

      // Sort Sequenzen by AbfolgeIndex
      foreach (var sequenz in reihe.Sequenzen.OrderBy(o => o.AbfolgeIndex))
      {
        var vm = new SequenzViewModel(this, sequenz);
        if (!notInContext)
        {
          App.MainViewModel.Sequenzen.Add(vm);
        }

        if (sequenz.AbfolgeIndex == -1)
        {
          this.AvailableSequenzen.Add(vm);
        }
        else
        {
          this.UsedSequenzen.Add(vm);
        }
      }

      // Listen for changes
      this.UsedSequenzen.CollectionChanged += this.UsedSequenzenCollectionChanged;
      this.AvailableSequenzen.CollectionChanged += this.AvailableSequenzenCollectionChanged;

      this.AddSequenzCommand = new DelegateCommand(this.AddSequenz);
      this.DeleteSequenzCommand = new DelegateCommand(this.DeleteCurrentSequenz, () => this.CurrentSequenz != null);
      this.LengthenReiheCommand = new DelegateCommand(this.LengthenReihe);
      this.ShortenReiheCommand = new DelegateCommand(this.ShortenReihe);
    }


    /// <summary>
    /// Holt den underlying Reihe this ViewModel is based on
    /// </summary>
    public Reihe Model { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new sequenz
    /// </summary>
    public DelegateCommand AddSequenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current sequenz
    /// </summary>
    public DelegateCommand DeleteSequenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur lengthen this reihe for 1 hour
    /// </summary>
    public DelegateCommand LengthenReiheCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur shorten this reihe with 1 hour
    /// </summary>
    public DelegateCommand ShortenReiheCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected sequenz
    /// </summary>
    public SequenzViewModel CurrentSequenz
    {
      get
      {
        return this.currentSequenz;
      }

      set
      {
        this.currentSequenz = value;
        this.RaisePropertyChanged("CurrentSequenz");
        this.DeleteSequenzCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public ModulViewModel ReiheModul
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
        if (value.ModulBezeichnung == this.modul.ModulBezeichnung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "ReiheModul", this.modul, value);
        this.modul = value;
        this.Model.Modul = value.Model;
        this.RaisePropertyChanged("ReiheModul");
      }
    }

    /// <summary>
    /// Holt oder setzt die Thema of this reihe
    /// </summary>
    public string ReiheThema
    {
      get
      {
        return this.Model.Thema;
      }

      set
      {
        if (value == this.Model.Thema)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "ReiheThema", this.Model.Thema, value);
        this.Model.Thema = value;
        this.RaisePropertyChanged("ReiheThema");
      }
    }

    /// <summary>
    /// Holt die bausteine for the modul of this reihe
    /// </summary>
    [DependsUpon("ReiheModul")]
    public string ReiheBausteine
    {
      get
      {
        return this.Model.Modul.Bausteine;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob dies eine PseudoReihe ist.
    /// </summary>
    [DependsUpon("ReiheThema")]
    public bool ReiheIsDummy
    {
      get
      {
        return this.ReiheThema == "NN";
      }
    }

    /// <summary>
    /// Holt oder setzt die Abfolgindex of this reihe
    /// </summary>
    public override int AbfolgeIndex
    {
      get
      {
        return this.Model.AbfolgeIndex;
      }

      set
      {
        if (value == this.Model.AbfolgeIndex)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "AbfolgeIndex", this.Model.AbfolgeIndex, value);
        this.Model.AbfolgeIndex = value;
        this.RaisePropertyChanged("AbfolgeIndex");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stundenbedarf of this reihe
    /// </summary>
    public int ReiheStundenbedarf
    {
      get
      {
        return this.Model.Stundenbedarf;
      }

      set
      {
        if (value == this.Model.Stundenbedarf)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "ReiheStundenbedarf", this.Model.Stundenbedarf, value);
        this.Model.Stundenbedarf = value;
        this.RaisePropertyChanged("ReiheStundenbedarf");
        var vm = App.MainViewModel.Curricula.First(o => o.Model == this.Model.Curriculum);
        vm.UpdateUsedStunden();
      }
    }

    /// <summary>
    /// Holt den Stundenbedarf as a string
    /// </summary>
    [DependsUpon("ReiheStundenbedarf")]
    public string ReiheStundenbedarfString
    {
      get
      {
        return this.Model.Stundenbedarf + "h";
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    [DependsUpon("ReiheStundenbedarf")]
    [DependsUpon("ReiheModul")]
    public int ReiheBreite
    {
      get
      {
        var fachstundenanzahl =
  App.MainViewModel.Fachstundenanzahl.First(
    o =>
    o.FachstundenanzahlFach.FachBezeichnung == Selection.Instance.Fach.FachBezeichnung
    && o.FachstundenanzahlKlassenstufe.Model == Selection.Instance.Klasse.Model.Klassenstufe);
        var wochenstunden = fachstundenanzahl.FachstundenanzahlStundenzahl
                            + fachstundenanzahl.FachstundenanzahlTeilungsstundenzahl;
        return (int)(this.Model.Stundenbedarf / (float)wochenstunden * Properties.Settings.Default.Wochenbreite);
      }
    }

    /// <summary>
    /// Holt den used sequenzen of this curriculum
    /// </summary>
    public ObservableCollection<SequenzViewModel> UsedSequenzen { get; private set; }

    /// <summary>
    /// Holt den available sequenzen of this curriculum
    /// </summary>
    public ObservableCollection<SequenzViewModel> AvailableSequenzen { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.ReiheThema;
    }

    /// <summary>
    /// Handles addition a new sequenz to this curriculum
    /// </summary>
    private void AddSequenz()
    {
      var sequenz = new Sequenz();
      sequenz.AbfolgeIndex = -1;
      sequenz.Stundenbedarf = 10;
      sequenz.Thema = "Neues Thema";
      sequenz.Reihe = this.Model;

      var vm = new SequenzViewModel(this, sequenz);
      using (new UndoBatch(App.MainViewModel, string.Format("Sequenz {0} hinzugefügt", vm), false))
      {
        var dlg = new SequenzDialog { Sequenz = vm };
        dlg.ShowDialog();

        this.AvailableSequenzen.Add(vm);
        App.MainViewModel.Sequenzen.Add(vm);
        this.CurrentSequenz = vm;
      }
    }

    /// <summary>
    /// Handles deletion of the current sequenz
    /// </summary>
    private void DeleteCurrentSequenz()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sequenz {0} entfernt", this.CurrentSequenz), false))
      {
        App.MainViewModel.Sequenzen.RemoveTest(this.CurrentSequenz);
        if (this.AvailableSequenzen.Contains(this.CurrentSequenz))
        {
          this.AvailableSequenzen.RemoveTest(this.CurrentSequenz);
        }

        if (this.UsedSequenzen.Contains(this.CurrentSequenz))
        {
          this.UsedSequenzen.RemoveTest(this.CurrentSequenz);
        }
      }
    }

    /// <summary>
    /// This is the method that is called when the user selects to increase
    /// the number of stunden this sequenz should last during adaption
    /// of the curriculum
    /// </summary>
    private void LengthenReihe()
    {
      this.ReiheStundenbedarf++;
    }

    /// <summary>
    /// This is the method that is called when the user selects to decrease
    /// the number of stunden this sequenz should last during adaption
    /// of the curriculum
    /// </summary>
    private void ShortenReihe()
    {
      if (this.ReiheStundenbedarf > 1)
      {
        this.ReiheStundenbedarf--;
      }
    }

    /// <summary>
    /// Tritt auf, wenn die UsedSequenzenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void UsedSequenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "UsedSequenzen", this.UsedSequenzen, e, false, "Änderung der UsedSequenzen");
    }

    /// <summary>
    /// Tritt auf, wenn die AvailableSequenzenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void AvailableSequenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "AvailableSequenzen", this.AvailableSequenzen, e, false, "Änderung der AvailableSequenzen");
    }
  }
}
