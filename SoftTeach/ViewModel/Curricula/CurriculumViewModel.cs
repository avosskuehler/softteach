﻿namespace SoftTeach.ViewModel.Curricula
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;

  using GongSolutions.Wpf.DragDrop;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Curricula;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual curriculum
  /// </summary>
  public class CurriculumViewModel : ViewModelBase, IDropTarget
  {
    /// <summary>
    /// The fach currently assigned to this curriculum
    /// </summary>
    private FachViewModel fach;

    ///// <summary>
    ///// The klassenstufe currently assigned to this curriculum
    ///// </summary>
    //private int klassenstufe;

    /// <summary>
    /// The schuljahr currently assigned to this curriculum
    /// </summary>
    private SchuljahrViewModel schuljahr;

    /// <summary>
    /// The reihe currently selected
    /// </summary>
    private ReiheViewModel currentReihe;

    /// <summary>
    /// The reihe currently selected in the available list view
    /// </summary>
    private ReiheViewModel selectedAvailableReihe;

    /// <summary>
    /// The item currently selected, can be Schulwoche, Reihe and sequenz
    /// </summary>
    private ViewModelBase currentItem;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="CurriculumViewModel"/> Klasse. 
    /// </summary>
    /// <param name="curriculum">
    /// The underlying curriculum this ViewModel is to be based on
    /// </param>
    public CurriculumViewModel(Curriculum curriculum)
    {
      this.Model = curriculum ?? throw new ArgumentNullException(nameof(curriculum));

      this.AdaptForJahresplanCommand = new DelegateCommand(this.AdaptForJahresplan);
      this.AddReiheCommand = new DelegateCommand(this.AddReihe);
      this.DeleteReiheCommand = new DelegateCommand(this.DeleteCurrentReihe, () => this.CurrentReihe != null);
      this.AddSequenzCommand = new DelegateCommand(this.AddSequenz);
      this.DeleteSequenzCommand = new DelegateCommand(this.DeleteCurrentSequenz, () => this.CurrentReihe != null && this.CurrentReihe.CurrentSequenz != null);

      // Build data structures for Reihen
      this.BausteineDesCurriculums = new ObservableCollection<SequencedViewModel>();
      //this.AvailableReihenDesCurriculums = new ObservableCollection<ReiheViewModel>();
      foreach (var reihe in curriculum.Reihen.OrderBy(o => o.Reihenfolge).ToList())
      {
        var vm = new ReiheViewModel(reihe);
        this.BausteineDesCurriculums.Add(vm);

        foreach (var sequenzViewModel in vm.Sequenzen)
        {
          this.BausteineDesCurriculums.Add(sequenzViewModel);
        }
      }

      this.ReihenViewSource = new CollectionViewSource() { Source = this.BausteineDesCurriculums };
      using (this.ReihenViewSource.DeferRefresh())
      {
        this.ReihenViewSource.SortDescriptions.Add(new SortDescription("Thema", ListSortDirection.Ascending));
        this.ReihenViewSource.Filter += this.ReihenViewSource_Filter;
      }

      this.SequenzenViewSource = new CollectionViewSource() { Source = this.BausteineDesCurriculums };
      using (this.SequenzenViewSource.DeferRefresh())
      {
        this.SequenzenViewSource.SortDescriptions.Add(new SortDescription("Thema", ListSortDirection.Ascending));
        this.SequenzenViewSource.Filter += this.SequenzenViewSource_Filter;
      }

      this.BausteineViewSource = new CollectionViewSource() { Source = this.BausteineDesCurriculums };
      using (this.BausteineViewSource.DeferRefresh())
      {
        this.BausteineViewSource.SortDescriptions.Add(new SortDescription("Reihenfolge", ListSortDirection.Ascending));
        this.BausteineViewSource.GroupDescriptions.Add(new PropertyGroupDescription("ViewModelType"));
        this.BausteineViewSource.Filter += this.BausteineViewSource_Filter;
      }


      // Listen for changes
      this.BausteineDesCurriculums.CollectionChanged += this.BausteineDesCurriculumsCollectionChanged;
      //this.AvailableReihenDesCurriculums.CollectionChanged += this.AvailableReihenDesCurriculumsCollectionChanged;
      //this.SequenzenDesCurriculums.CollectionChanged += this.SequenzenDesCurriculumsCollectionChanged;
      //this.SequenzenDerReihen.CollectionChanged += this.ReihenSequenzenCollectionChanged;

      this.CreateModuleClonesIfReihenListIsEmpty();
    }

    /// <summary>
    /// Holt den Befehl zur adapting this curriculum on a jahresplan
    /// </summary>
    public DelegateCommand AdaptForJahresplanCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Curriculum this ViewModel is based on
    /// </summary>
    public Curriculum Model { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new reihe
    /// </summary>
    public DelegateCommand AddReiheCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current reihe
    /// </summary>
    public DelegateCommand DeleteReiheCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new sequenz
    /// </summary>
    public DelegateCommand AddSequenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current sequenz
    /// </summary>
    public DelegateCommand DeleteSequenzCommand { get; private set; }

    ///// <summary>
    ///// Holt die im Curriculum verwendeten Sequenzen.
    ///// </summary>
    //public ObservableCollection<SequenzViewModel> SequenzenDesCurriculums { get; private set; }

    /// <summary>
    /// Holt die im Curriculum vorhandenen Reihen
    /// </summary>
    public ObservableCollection<SequencedViewModel> BausteineDesCurriculums { get; private set; }

    /// <summary>
    /// Holt oder setzt die View Source der Tage des ersten Halbjahres
    /// </summary>
    public CollectionViewSource BausteineViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes und gruppiertes View der Curriculumsbausteine
    /// </summary>
    public ICollectionView BausteineView => this.BausteineViewSource.View;

    /// <summary>
    /// Holt oder setzt die View Source der Tage des ersten Halbjahres
    /// </summary>
    public CollectionViewSource ReihenViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Reihen
    /// </summary>
    public ICollectionView ReihenView => this.ReihenViewSource.View;

    /// <summary>
    /// Holt oder setzt die View Source der Tage des ersten Halbjahres
    /// </summary>
    public CollectionViewSource SequenzenViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Sequenzen
    /// </summary>
    public ICollectionView SequenzenView => this.SequenzenViewSource.View;

    ///// <summary>
    ///// Holt die im Curriculum nicht verwendeten Reihen
    ///// </summary>
    //public ObservableCollection<ReiheViewModel> AvailableReihenDesCurriculums { get; private set; }

    ///// <summary>
    ///// Holt alle verfügbaren Reihen und Sequenzen
    ///// </summary>
    //public ObservableCollection<SequencedViewModel> SequenzenDerReihen { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected reihe
    /// </summary>
    public ReiheViewModel CurrentReihe
    {
      get
      {
        return this.currentReihe;
      }

      set
      {
        this.currentReihe = value;
        this.RaisePropertyChanged("CurrentReihe");

        this.DeleteReiheCommand.RaiseCanExecuteChanged();
        this.DeleteSequenzCommand.RaiseCanExecuteChanged();
        this.SequenzenView.Refresh();
      }
    }

    ///// <summary>
    ///// Holt die in der Reihe nicht verwendeten Sequenzen
    ///// </summary>
    //[DependsUpon("CurrentReihe")]
    //public ObservableCollection<SequenzViewModel> AvailableSequenzenDerReihe
    //{
    //  get
    //  {
    //    if (this.CurrentReihe != null)
    //    {
    //      return this.CurrentReihe.AvailableSequenzen;
    //    }

    //    return null;
    //  }
    //}

    ///// <summary>
    ///// Holt die in der Reihe verwendeten Sequenzen
    ///// </summary>
    //public ObservableCollection<SequenzViewModel> UsedSequenzenDerReihe
    //{
    //  get
    //  {
    //    if (this.CurrentReihe != null)
    //    {
    //      return this.CurrentReihe.Sequenzen;
    //    }

    //    return null;
    //  }
    //}

    /// <summary>
    /// Holt oder setzt die currently selected reihe
    /// </summary>
    public ReiheViewModel SelectedAvailableReihe
    {
      get
      {
        return this.selectedAvailableReihe;
      }

      set
      {
        this.selectedAvailableReihe = value;
        this.RaisePropertyChanged("SelectedAvailableReihe");
        this.CurrentReihe = value;
      }
    }

    /// <summary>
    /// Holt oder setzt die currently selected reihe
    /// </summary>
    public ViewModelBase CurrentItem
    {
      get
      {
        return this.currentItem;
      }

      set
      {
        this.currentItem = value;
        this.RaisePropertyChanged("CurrentItem");
        if (this.currentItem is ReiheViewModel)
        {
          this.CurrentReihe = value as ReiheViewModel;
        }
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Curriculum
    /// </summary>
    public FachViewModel CurriculumFach
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
        if (value.FachBezeichnung == this.fach.FachBezeichnung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(CurriculumFach), this.fach, value);
        this.fach = value;
        this.Model.Fach = value.Model;
        this.RaisePropertyChanged("CurriculumFach");
      }
    }

    /// <summary>
    /// Holt oder setzt die schuljahr currently assigned to this Curriculum
    /// </summary>
    public SchuljahrViewModel CurriculumSchuljahr
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
        if (value.SchuljahrBezeichnung == this.schuljahr.SchuljahrBezeichnung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(CurriculumSchuljahr), this.schuljahr, value);
        this.schuljahr = value;
        this.Model.Schuljahr = value.Model;
        this.RaisePropertyChanged("CurriculumSchuljahr");
      }
    }

    /// <summary>
    /// Holt oder setzt das Halbjahr currently assigned to this Curriculum
    /// </summary>
    public Halbjahr CurriculumHalbjahr
    {
      get
      {
        return this.Model.Halbjahr;
      }

      set
      {
        if (value == this.Model.Halbjahr)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(CurriculumHalbjahr), this.Model.Halbjahr, value);
        this.Model.Halbjahr = value;
        this.RaisePropertyChanged("CurriculumHalbjahr");
      }
    }

    /// <summary>
    /// Holt oder setzt den Jahrgang currently assigned to this Curriculum
    /// </summary>
    public int CurriculumJahrgang
    {
      get
      {
        return this.Model.Jahrgang;
      }

      set
      {
        if (value == this.Model.Jahrgang)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(CurriculumJahrgang), this.Model.Jahrgang, value);
        this.Model.Jahrgang = value;
        this.RaisePropertyChanged("CurriculumJahrgang");
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung of this Curriculum
    /// </summary>
    public string CurriculumBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(CurriculumBezeichnung), this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("CurriculumBezeichnung");
      }
    }

    /// <summary>
    /// Holt einen Textbaustein für die Titel des Curriculummoduls.
    /// </summary>
    [DependsUpon("CurriculumBezeichnung")]
    [DependsUpon("CurriculumJahrgang")]
    public string CurriculumTitelHeader
    {
      get
      {
        return string.Format(
          "Curriculum {0} für Jahrgang {1} im {2}halbjahr",
          this.Model.Bezeichnung,
          this.CurriculumJahrgang,
          this.CurriculumHalbjahr);
      }
    }

    /// <summary>
    /// Holt den number of needed lessons for this curriculum
    /// </summary>
    [DependsUpon("CurriculumFach")]
    [DependsUpon("CurriculumJahrgang")]
    public int CurriculumVerfügbareStunden
    {
      get
      {
        //var unterrichtstyp = Termintyp.Unterricht;
        var fachstundenViewModel =
          App.MainViewModel.Fachstundenanzahl.First(
            o => o.FachstundenanzahlFach.FachBezeichnung == this.CurriculumFach.FachBezeichnung &&
            o.FachstundenanzahlJahrgang == this.CurriculumJahrgang);
        var fachstundenanzahl = fachstundenViewModel.FachstundenanzahlStundenzahl
                                + fachstundenViewModel.FachstundenanzahlTeilungsstundenzahl;

        return fachstundenanzahl * 15;
      }
    }

    /// <summary>
    /// Holt die Anzahl der bereits verplanten Stunden des Curriculums.
    /// Wenn keine Sequenzen vorliegen wird die Reihenstundenzahl gezählt, sonst die Sequenzen.
    /// </summary>
    [DependsUpon("BausteineDesCurriculums")]
    public int CurriculumVerplanteStunden
    {
      get
      {
        var summe = 0;
        if (!this.BausteineDesCurriculums.OfType<SequenzViewModel>().Any(o => o.Reihenfolge != -1))
        {
          summe += this.BausteineDesCurriculums.OfType<ReiheViewModel>().Where(o => o.Reihenfolge != -1).Sum(o => o.ReiheStundenbedarf);
        }
        else
        {
          summe += this.BausteineDesCurriculums.OfType<SequenzViewModel>().Where(o => o.Reihenfolge != -1).Sum(o => o.SequenzStundenbedarf);
        }

        return summe;
      }
    }

    /// <summary>
    /// Holt die Überschrift für die Sequenzliste des Moduls
    /// </summary>
    [DependsUpon("CurrentReihe")]
    public string CurriculumSequenzenGroupHeader
    {
      get
      {
        if (this.CurrentReihe != null)
        {
          return "Sequenzen des Moduls " + this.CurrentReihe.Thema;
        }

        return "Bitte Modul auswählen, um Sequenzen anzuzeigen";
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.CurriculumBezeichnung;
    }

    /// <summary>
    /// Aktualisiert die Sequenzliste
    /// </summary>
    public void UpdateFocus()
    {
      if (this.currentItem is ReiheViewModel)
      {
        this.CurrentReihe = this.currentItem as ReiheViewModel;
      }
    }

    /// <summary>
    /// Aktualisiert CurriculumVerplanteStunden
    /// </summary>
    public void UpdateUsedStunden()
    {
      this.RaisePropertyChanged("CurriculumVerplanteStunden");
    }

    /// <summary>
    /// The drag over.
    /// </summary>
    /// <param name="dropInfo">
    /// The drop info.
    /// </param>
    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data;
      var targetItem = dropInfo.TargetItem;
      if (sourceItem is ReiheViewModel || sourceItem is SequenzViewModel)
      {
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

      //if (targetItem is SchultagViewModel
      //  || (targetItem is ReiheViewModel && sourceItem is SequenzViewModel)
      //  || (targetItem is SequenzViewModel && sourceItem is ReiheViewModel))
      //{
      //  dropInfo.Effects = DragDropEffects.None;
      //}
    }

    public void Drop(IDropInfo dropInfo)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Drag and Drop im Curriculum"), false))
      {
        var dropItem = dropInfo.Data;
        if (dropItem is ReiheViewModel)
        {
          var reiheViewModel = dropItem as ReiheViewModel;

          if (dropInfo.VisualTarget is ListBox)
          {
            var targetListBox = dropInfo.VisualTarget as ListBox;
            if (targetListBox.Name == "AvailableReihenListBox")
            {
              reiheViewModel.Reihenfolge = -1;
              //if (this.ReihenDesCurriculums.Contains(reiheViewModel))
              //{
              //  this.ReihenDesCurriculums.RemoveTest(reiheViewModel);
              //  this.AvailableReihenDesCurriculums.Add(reiheViewModel);
              //}
            }
            else if (targetListBox.Name == "UsedItemsListBox")
            {
              var newIndex = dropInfo.InsertIndex;
              if (newIndex < 0)
              {
                newIndex = this.BausteineDesCurriculums.Count(o => o.Reihenfolge != -1);
              }

              reiheViewModel.Reihenfolge = newIndex;

              //if (reiheViewModel.Reihenfolge != -1)
              //{
              //  // Reihe wird nur verschoben
              //  reiheViewModel.Reihenfolge = newIndex;
              //}
              //else
              //{
              //  // Insert at position
              //  this.CurrentReihe = reiheViewModel;
              //  if (newIndex > this.ReihenDesCurriculums.Count)
              //  {
              //    newIndex = this.ReihenDesCurriculums.Count;
              //  }

              //  this.ReihenDesCurriculums.Insert(newIndex, reiheViewModel);
              //  this.AvailableReihenDesCurriculums.RemoveTest(reiheViewModel);
              //}
            }
          }
        }

        if (dropItem is SequenzViewModel)
        {
          var sequenzViewModel = dropItem as SequenzViewModel;

          if (dropInfo.VisualTarget is ListBox)
          {
            var targetListBox = dropInfo.VisualTarget as ListBox;
            if (targetListBox.Name == "AvailableSequenzenListBox")
            {
              sequenzViewModel.Reihenfolge = -1;
            }
            else if (targetListBox.Name == "UsedItemsListBox")
            {
              sequenzViewModel.Reihenfolge = dropInfo.InsertIndex;
              sequenzViewModel.IstZuerst = true;
            }
          }
        }

        this.UpdateReihenfolgeIndex();
        this.SequenzenView.Refresh();
        this.ReihenView.Refresh();
        this.BausteineView.Refresh();
      }
    }

    private void CreateModuleClonesIfReihenListIsEmpty()
    {
      // Wenn keine Reihen vorhanden sind, um sie
      // ins Curriculum einzufügen, werden sie aus den Modulen als Vorlage  erstellt
      if (this.BausteineDesCurriculums.Count == 0)
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Neue Module anlegen"), false))
        {
          foreach (var modulViewModel in App.MainViewModel.Module.Where(o => o.ModulFach.FachBezeichnung == this.CurriculumFach.FachBezeichnung
            && o.ModulJahrgang == this.CurriculumJahrgang))
          {
            var reihe = new Reihe
            {
              Stundenbedarf = modulViewModel.ModulStundenbedarf,
              Thema = modulViewModel.ModulBezeichnung,
              Modul = modulViewModel.Model,
              Reihenfolge = -1,
              Curriculum = this.Model
            };
            //App.UnitOfWork.Context.Reihen.Add(reihe);

            var bausteine = modulViewModel.ModulBausteine.Trim().Split(',');
            foreach (var baustein in bausteine)
            {
              if (baustein == string.Empty) continue;
              var sequenz = new Sequenz
              {
                Reihenfolge = -1,
                Reihe = reihe,

                // Stundenbedarf schätzen
                Stundenbedarf = Math.Max((int)(modulViewModel.ModulStundenbedarf / (float)bausteine.Count()), 1),
                Thema = baustein.Trim()
              };
              reihe.Sequenzen.Add(sequenz);
              //App.UnitOfWork.Context.Sequenzen.Add(sequenz);
            }

            var vm = new ReiheViewModel(reihe);
            this.BausteineDesCurriculums.Add(vm);

            foreach (var sequenzViewModel in vm.Sequenzen)
            {
              this.BausteineDesCurriculums.Add(sequenzViewModel);
            }

            this.CurrentReihe = vm;
          }
        }
      }
    }

    //private void PopulateBoth()
    //{
    //  this.SequenzenDerReihen.Clear();
    //  foreach (var usedReiheDesCurriculums in this.ReihenDesCurriculums)
    //  {
    //    this.SequenzenDerReihen.Add(usedReiheDesCurriculums);
    //  }

    //  foreach (var usedSequenzenDesCurriculums in this.SequenzenDesCurriculums)
    //  {
    //    this.SequenzenDerReihen.Add(usedSequenzenDesCurriculums);
    //  }
    //}

    /// <summary>
    /// Handles addition a new reihe to this curriculum
    /// </summary>
    private void AddReihe()
    {
      var modul = App.MainViewModel.Module.FirstOrDefault(o => o.ModulFach == this.fach && o.ModulJahrgang == this.CurriculumJahrgang);
      var reihe = new Reihe { Stundenbedarf = 3, Thema = "Neues Thema", Curriculum = this.Model, Modul = modul.Model, Reihenfolge = -1 };
      var vm = new ReiheViewModel(reihe);
      //App.MainViewModel.Reihen.Add(vm);
      this.BausteineDesCurriculums.Add(vm);
      this.CurrentReihe = vm;
    }

    /// <summary>
    /// Handles deletion of the current reihe
    /// </summary>
    private void DeleteCurrentReihe()
    {
      //App.MainViewModel.Reihen.RemoveTest(this.CurrentReihe);
      //if (this.AvailableReihenDesCurriculums.Contains(this.CurrentReihe))
      //{
      //  this.AvailableReihenDesCurriculums.RemoveTest(this.currentReihe);
      //}
      if (this.BausteineDesCurriculums.Contains(this.CurrentReihe))
      {
        this.BausteineDesCurriculums.RemoveTest(this.currentReihe);
      }
    }

    /// <summary>
    /// Handles addition a new sequenz to this curriculum
    /// </summary>
    private void AddSequenz()
    {
      if (this.CurrentReihe != null)
      {
        this.CurrentReihe.AddSequenzCommand.Execute(null);
        this.BausteineDesCurriculums.Add(this.CurrentReihe.CurrentSequenz);
      }
    }

    /// <summary>
    /// Handles deletion of the current sequenz
    /// </summary>
    private void DeleteCurrentSequenz()
    {
      if (this.CurrentReihe != null)
      {
        this.CurrentReihe.DeleteSequenzCommand.Execute(null);
      }
    }

    private void UpdateReihenfolgeIndex()
    {
      var sequenceNumber = 1;

      // Resequence
      var collection = this.BausteineDesCurriculums.OfType<ReiheViewModel>().Where(o => o.Reihenfolge != -1).OrderBy(o => o.Reihenfolge).ThenBy(o => o.IstZuerst);
      foreach (var sequencedObject in collection)
      {
        sequencedObject.Reihenfolge = sequenceNumber;
        sequencedObject.IstZuerst = false;
        sequenceNumber++;
      }

      sequenceNumber = 1;

      // Resequence
      var collection2 = this.BausteineDesCurriculums.OfType<SequenzViewModel>().Where(o => o.Reihenfolge != -1).OrderBy(o => o.Reihenfolge).ThenByDescending(o => o.IstZuerst);
      foreach (var sequencedObject in collection2)
      {
        sequencedObject.Reihenfolge = sequenceNumber;
        sequencedObject.IstZuerst = false;
        sequenceNumber++;
      }
    }

    /// <summary>
    /// This method is used to adapt the current curriculum such that
    /// the choosen jahresplan has the curriculum implemented.
    /// </summary>
    private void AdaptForJahresplan()
    {
      var undoAll = false;

      var dlg = new AskForLerngruppeToAdaptDialog(this.CurriculumFach, this.CurriculumJahrgang);
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
        using (new UndoBatch(App.MainViewModel, string.Format("Curriculum an Jahresplan angepasst"), false))
        {
          Selection.Instance.Fach = this.CurriculumFach;

          if (dlg.SelectedLerngruppe == null)
          {
            InformationDialog.Show("Fehler", "Lerngruppe nicht gefunden", false);
            return;
          }

          Selection.Instance.Lerngruppe = dlg.SelectedLerngruppe;
          Selection.Instance.Fach = dlg.SelectedLerngruppe.LerngruppeFach;
          Selection.Instance.Halbjahr = this.CurriculumHalbjahr;

          // Create a clone of this curriculum for the adaption dialog
          var curriculumClone = new Curriculum
          {
            Bezeichnung = this.CurriculumBezeichnung + " Kopie",
            Fach = this.CurriculumFach.Model,
            Schuljahr = this.CurriculumSchuljahr.Model,
            Halbjahr = this.CurriculumHalbjahr,
            Jahrgang = this.CurriculumJahrgang
          };
          //App.UnitOfWork.Context.Curricula.Add(curriculumClone);

          foreach (var reihe in this.Model.Reihen)
          {
            var reiheClone = new Reihe
            {
              Reihenfolge = reihe.Reihenfolge,
              Modul = reihe.Modul,
              Stundenbedarf = reihe.Stundenbedarf,
              Thema = reihe.Thema,
              Curriculum = curriculumClone
            };
            //App.UnitOfWork.Context.Reihen.Add(reiheClone);

            foreach (var sequenz in reihe.Sequenzen)
            {
              var sequenzClone = new Sequenz
              {
                Reihenfolge = sequenz.Reihenfolge,
                Stundenbedarf = sequenz.Stundenbedarf,
                Thema = sequenz.Thema,
                Reihe = reiheClone
              };
              //App.UnitOfWork.Context.Sequenzen.Add(sequenzClone);
              reiheClone.Sequenzen.Add(sequenzClone);
            }

            curriculumClone.Reihen.Add(reiheClone);
          }

          var curriculumCloneViewModel = new CurriculumViewModel(curriculumClone);
          var curriculumZuweisenWorkspace = new CurriculumZuweisenWorkspaceViewModel(curriculumCloneViewModel, dlg.SelectedLerngruppe, this.CurriculumHalbjahr);
          var dlgZuweisen = new CurriculumZuweisenDialog { DataContext = curriculumZuweisenWorkspace };

          if (dlgZuweisen.ShowDialog().GetValueOrDefault(false))
          {
            if (InformationDialog.Show("Änderungen speichern ?", "Wollen Sie das geänderte Curriculum speichern?", true).GetValueOrDefault(false))
            {
              var dlgName = new AskForCurriculumNameDialog();
              dlgName.ShowDialog();
              curriculumCloneViewModel.CurriculumBezeichnung = dlgName.CurriculumBezeichnung;
            }
          }
          else
          {
            undoAll = true;
          }
        }
        App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;
      }

      if (undoAll)
      {
        UndoService.Current[App.MainViewModel].Undo();
      }
    }

    ///// <summary>
    ///// Tritt auf, wenn die ReihenSequenzenCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void ReihenSequenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  UndoableCollectionChanged(this, nameof(SequenzenDerReihen), this.SequenzenDerReihen, e, true, "Änderung der ReihenSequenzen");
    //  this.RaisePropertyChanged("CurriculumVerplanteStunden");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die UsedSequenzenDesCurriculumsCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void SequenzenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  UndoableCollectionChanged(this, nameof(SequenzenDesCurriculums), this.SequenzenDesCurriculums, e, true, "Änderung der UsedSequenzenDesCurriculums");
    //  this.RaisePropertyChanged("CurriculumVerplanteStunden");
    //}

    /// <summary>
    /// Tritt auf, wenn die UsedReihenDesCurriculumsCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void BausteineDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(BausteineDesCurriculums), this.BausteineDesCurriculums, e, true, "Änderung der BausteineDesCurriculums");
      this.RaisePropertyChanged("CurriculumVerplanteStunden");
    }

    ///// <summary>
    ///// Tritt auf, wenn die AvailableReihenDesCurriculumsCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void AvailableReihenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  UndoableCollectionChanged(this, nameof(AvailableReihenDesCurriculums), this.AvailableReihenDesCurriculums, e, true, "Änderung der AvailableReihenDesCurriculums");
    //}

    private void BausteineViewSource_Filter(object sender, FilterEventArgs e)
    {
      var vm = e.Item as SequencedViewModel;
      if (vm == null)
      {
        e.Accepted = false;
        return;
      }

      if (vm.Reihenfolge == -1)
      {
        e.Accepted = false;
        return;
      }

      e.Accepted = true;
      return;
    }

    private void SequenzenViewSource_Filter(object sender, FilterEventArgs e)
    {
      var vm = e.Item as SequenzViewModel;
      if (vm == null)
      {
        e.Accepted = false;
        return;
      }

      if (vm.Reihenfolge != -1)
      {
        e.Accepted = false;
        return;
      }

      if (this.CurrentReihe == null)
      {
        e.Accepted = false;
        return;
      }

      if (vm.SequenzReihe != this.CurrentReihe)
      {
        e.Accepted = false;
        return;
      }

      e.Accepted = true;
      return;
    }

    private void ReihenViewSource_Filter(object sender, FilterEventArgs e)
    {
      var vm = e.Item as ReiheViewModel;
      if (vm == null)
      {
        e.Accepted = false;
        return;
      }

      if (vm.Reihenfolge != -1)
      {
        e.Accepted = false;
        return;
      }

      e.Accepted = true;
      return;
    }

  }
}
