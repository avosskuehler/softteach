namespace SoftTeach.ViewModel.Curricula
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
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
    /// Initialisiert eine e Instanz der <see cref="CurriculumViewModel"/> Klasse. 
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
      this.UsedReihenDesCurriculums = new ObservableCollection<ReiheViewModel>();
      this.AvailableReihenDesCurriculums = new ObservableCollection<ReiheViewModel>();
      foreach (var reihe in curriculum.Reihen.OrderBy(o => o.Reihenfolge).ToList())
      {
        var vm = new ReiheViewModel(reihe);
        //App.MainViewModel.Reihen.Add(vm);
        if (vm.Reihenfolge == -1)
        {
          this.AvailableReihenDesCurriculums.Add(vm);
        }
        else
        {
          this.UsedReihenDesCurriculums.Add(vm);
        }
      }

      this.CreateModuleClonesIfReihenListIsEmpty();

      this.UsedSequenzenDesCurriculums = new ObservableCollection<SequenzViewModel>();
      this.PopulateSequenzen();

      this.ReihenSequenzen = new ObservableCollection<SequencedViewModel>();
      this.PopulateBoth();

      // Listen for changes
      this.UsedReihenDesCurriculums.CollectionChanged += this.UsedReihenDesCurriculumsCollectionChanged;
      this.AvailableReihenDesCurriculums.CollectionChanged += this.AvailableReihenDesCurriculumsCollectionChanged;
      this.UsedSequenzenDesCurriculums.CollectionChanged += this.UsedSequenzenDesCurriculumsCollectionChanged;
      this.ReihenSequenzen.CollectionChanged += this.ReihenSequenzenCollectionChanged;
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

    /// <summary>
    /// Holt die im Curriculum verwendeten Sequenzen.
    /// </summary>
    public ObservableCollection<SequenzViewModel> UsedSequenzenDesCurriculums { get; private set; }

    /// <summary>
    /// Holt die im Curriculum verwendeten Reihen
    /// </summary>
    public ObservableCollection<ReiheViewModel> UsedReihenDesCurriculums { get; private set; }

    /// <summary>
    /// Holt die im Curriculum nicht verwendeten Reihen
    /// </summary>
    public ObservableCollection<ReiheViewModel> AvailableReihenDesCurriculums { get; private set; }

    /// <summary>
    /// Holt alle verfügbaren Reihen und Sequenzen
    /// </summary>
    public ObservableCollection<SequencedViewModel> ReihenSequenzen { get; private set; }

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
      }
    }

    /// <summary>
    /// Holt die in der Reihe nicht verwendeten Sequenzen
    /// </summary>
    [DependsUpon("CurrentReihe")]
    public ObservableCollection<SequenzViewModel> AvailableSequenzenDerReihe
    {
      get
      {
        if (this.CurrentReihe != null)
        {
          return this.CurrentReihe.AvailableSequenzen;
        }

        return null;
      }
    }

    /// <summary>
    /// Holt die in der Reihe verwendeten Sequenzen
    /// </summary>
    public ObservableCollection<SequenzViewModel> UsedSequenzenDerReihe
    {
      get
      {
        if (this.CurrentReihe != null)
        {
          return this.CurrentReihe.UsedSequenzen;
        }

        return null;
      }
    }

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
    /// Holt den number of needed lessons for this curriculum
    /// </summary>
    [DependsUpon("UsedReihenDesCurriculums")]
    public int CurriculumVerplanteStunden
    {
      get
      {
        var summe = 0;
        foreach (var reiheViewModel in this.UsedReihenDesCurriculums)
        {
          // if there are reihe defined used this for the count
          // otherwise use the predefined value
          if (reiheViewModel.UsedSequenzen.Count > 0)
          {
            summe += reiheViewModel.UsedSequenzen.Sum(sequenzViewModel => sequenzViewModel.SequenzStundenbedarf);
          }
          else
          {
            summe += reiheViewModel.ReiheStundenbedarf;
          }
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
          return "Sequenzen des Moduls " + this.CurrentReihe.ReiheThema;
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
              if (this.UsedReihenDesCurriculums.Contains(reiheViewModel))
              {
                this.UsedReihenDesCurriculums.RemoveTest(reiheViewModel);
                this.AvailableReihenDesCurriculums.Add(reiheViewModel);
              }
            }
            else if (targetListBox.Name == "UsedItemsListBox")
            {
              var newIndex = dropInfo.InsertIndex;
              if (newIndex < 0)
              {
                newIndex = this.UsedReihenDesCurriculums.Count;
              }

              if (this.UsedReihenDesCurriculums.Contains(reiheViewModel))
              {
                if (dropInfo.Effects == DragDropEffects.Copy)
                {
                  // Create a clone
                  var reiheClone = new Reihe
                  {
                    Stundenbedarf = reiheViewModel.ReiheStundenbedarf,
                    Thema = reiheViewModel.ReiheThema,
                    Modul = reiheViewModel.ReiheModul.Model,
                    Reihenfolge = -1,
                    Curriculum = this.Model
                  };
                  //App.UnitOfWork.Context.Reihen.Add(reiheClone);

                  foreach (var sequenz in reiheViewModel.AvailableSequenzen)
                  {
                    var sequenzClone = new Sequenz
                    {
                      Reihenfolge = sequenz.Reihenfolge,
                      Reihe = reiheClone,
                      Stundenbedarf = sequenz.SequenzStundenbedarf,
                      Thema = sequenz.SequenzThema
                    };
                    //App.UnitOfWork.Context.Sequenzen.Add(sequenzClone);
                    reiheClone.Sequenzen.Add(sequenzClone);
                  }

                  foreach (var sequenz in reiheViewModel.UsedSequenzen)
                  {
                    var sequenzClone = new Sequenz
                    {
                      Reihenfolge = -1,
                      Reihe = reiheClone,
                      Stundenbedarf = sequenz.SequenzStundenbedarf,
                      Thema = sequenz.SequenzThema
                    };
                    //App.UnitOfWork.Context.Sequenzen.Add(sequenzClone);
                    reiheClone.Sequenzen.Add(sequenzClone);
                  }

                  var vm = new ReiheViewModel(reiheClone);
                  //App.MainViewModel.Reihen.Add(vm);
                  this.UsedReihenDesCurriculums.Add(vm);
                }
                else
                {
                  var oldIndex = this.UsedReihenDesCurriculums.IndexOf(reiheViewModel);
                  if (newIndex > oldIndex)
                  {
                    newIndex--;
                  }

                  this.UsedReihenDesCurriculums.Move(oldIndex, newIndex);
                }
              }
              else
              {
                // Insert at position
                this.CurrentReihe = reiheViewModel;
                if (newIndex > this.UsedReihenDesCurriculums.Count)
                {
                  newIndex = this.UsedReihenDesCurriculums.Count;
                }

                this.UsedReihenDesCurriculums.Insert(newIndex, reiheViewModel);
                this.AvailableReihenDesCurriculums.RemoveTest(reiheViewModel);
              }
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
              if (this.UsedSequenzenDesCurriculums.Contains(sequenzViewModel))
              {
                sequenzViewModel.SequenzReihe.AvailableSequenzen.Add(sequenzViewModel);
                sequenzViewModel.SequenzReihe.UsedSequenzen.RemoveTest(sequenzViewModel);
                this.UsedSequenzenDesCurriculums.RemoveTest(sequenzViewModel);
              }
            }
            else if (targetListBox.Name == "UsedItemsListBox")
            {
              var newIndex = dropInfo.InsertIndex - this.UsedReihenDesCurriculums.Count;
              if (newIndex < 0)
              {
                newIndex = this.UsedSequenzenDesCurriculums.Count;
              }

              if (this.UsedSequenzenDesCurriculums.Contains(sequenzViewModel))
              {
                var oldIndex = this.UsedSequenzenDesCurriculums.IndexOf(sequenzViewModel);
                if (newIndex > oldIndex)
                {
                  newIndex--;
                }

                this.UsedSequenzenDesCurriculums.Move(oldIndex, newIndex);
              }
              else
              {
                // Sequenz wird  hinzugefügt
                this.AvailableSequenzenDerReihe.RemoveTest(sequenzViewModel);
                sequenzViewModel.Reihenfolge = -1;
                this.UsedSequenzenDesCurriculums.Insert(newIndex, sequenzViewModel);
                this.UsedSequenzenDerReihe.Add(sequenzViewModel);
              }
            }
          }
        }

        this.UpdateReihenfolgeIndex();
      }
    }

    private void CreateModuleClonesIfReihenListIsEmpty()
    {
      // Wenn keine Reihen vorhanden sind, um sie
      // ins Curriculum einzufügen, werden sie aus den Modulen als Vorlage  erstellt
      if (this.AvailableReihenDesCurriculums.Count == 0 && this.UsedReihenDesCurriculums.Count == 0)
      {
        using (new UndoBatch(App.MainViewModel, string.Format("e Module anlegen"), false))
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
            //App.MainViewModel.Reihen.Add(vm);
            this.AvailableReihenDesCurriculums.Add(vm);
            this.CurrentReihe = vm;
          }
        }
      }
    }

    private void PopulateSequenzen()
    {
      this.UsedSequenzenDesCurriculums.Clear();

      foreach (var reiheViewModel in this.UsedReihenDesCurriculums)
      {
        foreach (var sequenzViewModel in reiheViewModel.UsedSequenzen)
        {
          this.UsedSequenzenDesCurriculums.Add(sequenzViewModel);
        }
      }

      this.UsedSequenzenDesCurriculums.BubbleSort();
    }

    private void PopulateBoth()
    {
      this.ReihenSequenzen.Clear();
      foreach (var usedReiheDesCurriculums in this.UsedReihenDesCurriculums)
      {
        this.ReihenSequenzen.Add(usedReiheDesCurriculums);
      }

      foreach (var usedSequenzenDesCurriculums in this.UsedSequenzenDesCurriculums)
      {
        this.ReihenSequenzen.Add(usedSequenzenDesCurriculums);
      }
    }

    /// <summary>
    /// Handles addition a new reihe to this modul
    /// </summary>
    private void AddReihe()
    {
      var modul = App.MainViewModel.Module.FirstOrDefault(o => o.ModulFach == this.fach && o.ModulJahrgang == this.CurriculumJahrgang);
      var reihe = new Reihe { Stundenbedarf = 3, Thema = "Neues Thema", Curriculum = this.Model, Modul = modul.Model };
      var vm = new ReiheViewModel(reihe);
      //App.MainViewModel.Reihen.Add(vm);
      this.AvailableReihenDesCurriculums.Add(vm);
      this.CurrentReihe = vm;
    }

    /// <summary>
    /// Handles deletion of the current phase
    /// </summary>
    private void DeleteCurrentReihe()
    {
      //App.MainViewModel.Reihen.RemoveTest(this.CurrentReihe);
      if (this.AvailableReihenDesCurriculums.Contains(this.CurrentReihe))
      {
        this.AvailableReihenDesCurriculums.RemoveTest(this.currentReihe);
      }
      else if (this.UsedReihenDesCurriculums.Contains(this.CurrentReihe))
      {
        this.UsedReihenDesCurriculums.RemoveTest(this.currentReihe);
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
      SequencingService.SetCollectionSequence(this.UsedReihenDesCurriculums);

      foreach (var model in this.AvailableReihenDesCurriculums)
      {
        model.Reihenfolge = -1;
      }

      SequencingService.SetCollectionSequence(this.UsedSequenzenDesCurriculums);

      if (this.AvailableSequenzenDerReihe != null)
      {
        foreach (var model in this.AvailableSequenzenDerReihe)
        {
          model.Reihenfolge = -1;
        }
      }

      this.UpdateReihenSequenzenCollection();
    }

    /// <summary>
    /// Removes all ReiheViewModels from the ReihenSequenzen
    /// and readds the updated values from the UsedReihenDesCurriculums collection.
    /// </summary>
    private void UpdateReihenSequenzenCollection()
    {
      var sequenzenInCollection = this.ReihenSequenzen.OfType<SequenzViewModel>().Count();
      var reihenInCollection = this.ReihenSequenzen.OfType<ReiheViewModel>().Count();
      var backup = this.CurrentItem;
      for (int i = 0; i < reihenInCollection + sequenzenInCollection; i++)
      {
        this.ReihenSequenzen.RemoveAt(this.ReihenSequenzen.Count - 1);
      }

      foreach (var usedReiheDesCurriculums in this.UsedReihenDesCurriculums)
      {
        this.ReihenSequenzen.Add(usedReiheDesCurriculums);
      }

      foreach (var usedSequenzDesCurriculums in this.UsedSequenzenDesCurriculums)
      {
        this.ReihenSequenzen.Add(usedSequenzDesCurriculums);
      }

      this.CurrentItem = backup;
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

          var lerngruppe = App.UnitOfWork.Context.Lerngruppen.FirstOrDefault(o => o.SchuljahrId == dlg.SelectedLerngruppe.SchuljahrId && o.FachId == dlg.SelectedLerngruppe.FachId && o.Jahrgang == dlg.SelectedLerngruppe.Jahrgang);
          if (lerngruppe == null)
          {
            InformationDialog.Show("Fehler", "Lerngruppe nicht gefunden", false);
            return;
          }

          var vm = App.MainViewModel.LoadLerngruppe(lerngruppe);

          Selection.Instance.Lerngruppe = vm;
          Selection.Instance.Fach = vm.LerngruppeFach;
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
          var curriculumZuweisenWorkspace = new CurriculumZuweisenWorkspaceViewModel(curriculumCloneViewModel, vm, this.CurriculumHalbjahr);
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

    /// <summary>
    /// Tritt auf, wenn die ReihenSequenzenCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void ReihenSequenzenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(ReihenSequenzen), this.ReihenSequenzen, e, true, "Änderung der ReihenSequenzen");
      this.RaisePropertyChanged("CurriculumVerplanteStunden");
    }

    /// <summary>
    /// Tritt auf, wenn die UsedSequenzenDesCurriculumsCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void UsedSequenzenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(UsedSequenzenDesCurriculums), this.UsedSequenzenDesCurriculums, e, true, "Änderung der UsedSequenzenDesCurriculums");
      this.RaisePropertyChanged("CurriculumVerplanteStunden");
    }

    /// <summary>
    /// Tritt auf, wenn die UsedReihenDesCurriculumsCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void UsedReihenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(UsedReihenDesCurriculums), this.UsedReihenDesCurriculums, e, true, "Änderung der UsedReihenDesCurriculums");
      this.RaisePropertyChanged("CurriculumVerplanteStunden");
    }

    /// <summary>
    /// Tritt auf, wenn die AvailableReihenDesCurriculumsCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void AvailableReihenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(AvailableReihenDesCurriculums), this.AvailableReihenDesCurriculums, e, true, "Änderung der AvailableReihenDesCurriculums");
    }
  }
}
