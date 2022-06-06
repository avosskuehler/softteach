namespace SoftTeach.ViewModel.Curricula
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;

  using GongSolutions.Wpf.DragDrop;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Stundenentwürfe;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// ViewModel for managing Curriculum
  /// </summary>
  public class CurriculumZuweisenWorkspaceViewModel : ViewModelBase, IDropTarget
  {
    /// <summary>
    /// Das Curriculum mit der Vorlage
    /// </summary>
    private readonly CurriculumViewModel curriculumSource;

    /// <summary>
    /// Die Lerngruppe, die bearbeitet werden soll
    /// </summary>
    private readonly LerngruppeViewModel lerngruppeTarget;

    /// <summary>
    /// Das Halbjahr, für das die Übertragung stattfinden soll
    /// </summary>
    private readonly Halbjahr currentHalbjahr;

    /// <summary>
    /// Gibt an, ob die Ferien mit angezeigt werden sollen.
    /// </summary>
    private bool showFerien;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="CurriculumZuweisenWorkspaceViewModel"/> Klasse. 
    /// </summary>
    /// <param name="curriculumViewModel">
    /// This should be a clone of a curriculum to be adapted,
    /// that is later removed, cause the changes in this dialog should not be persistant
    /// to any saved curriculum.
    /// </param>
    /// <param name="lerngruppeTarget">Die Lerngruppe, die angepasst werden soll.</param>
    /// <param name="halbjahr">Das Halbjahr für das die Übertragung stattfinden soll</param>
    public CurriculumZuweisenWorkspaceViewModel(CurriculumViewModel curriculumViewModel, LerngruppeViewModel lerngruppeTarget, Halbjahr halbjahr)
    {
      this.curriculumSource = curriculumViewModel;
      this.lerngruppeTarget = lerngruppeTarget;
      this.currentHalbjahr = halbjahr;

      this.StundenDerLerngruppe = new ObservableCollection<StundeViewModel>();
      this.PopulateStunden();
      this.UsedSequenzenDesCurriculums = new ObservableCollection<SequenzViewModel>();
      this.AvailableSequenzenDesCurriculums = new ObservableCollection<SequenzViewModel>();
      this.PopulateSequenzen();
      this.StundenAndSequenzenCollection = new ObservableCollection<ViewModelBase>();
      this.PopulateBoth();

      //// Listen for changes
      //this.StundenDerLerngruppe.CollectionChanged += this.StundenDerLerngruppeCollectionChanged;
      //this.UsedSequenzenDesCurriculums.CollectionChanged += this.UsedSequenzenDesCurriculumsCollectionChanged;
      //this.AvailableSequenzenDesCurriculums.CollectionChanged += this.AvailableSequenzenDesCurriculumsCollectionChanged;
      //this.StundenAndSequenzenCollection.CollectionChanged += this.TagespläneAndSequenzenCollectionCollectionChanged;

      this.UpdateHalbjahresplanWithCurriculumCommand = new DelegateCommand(this.UpdateHalbjahresplanWithCurriculum);

      // Ergänze Ferien
      this.ShowFerien = true;
    }

    /// <summary>
    /// Holt den Befehl zur UpdateHalbjahresplanWithCurriculum
    /// </summary>
    public DelegateCommand UpdateHalbjahresplanWithCurriculumCommand { get; private set; }

    /// <summary>
    /// Holt die TagespläneDesHalbjahresplans
    /// </summary>
    public ObservableCollection<StundeViewModel> StundenDerLerngruppe { get; private set; }

    /// <summary>
    /// Holt die UsedSequenzenDesCurriculums
    /// </summary>
    public ObservableCollection<SequenzViewModel> UsedSequenzenDesCurriculums { get; private set; }

    /// <summary>
    /// Holt die AvailableSequenzenDesCurriculums
    /// </summary>
    public ObservableCollection<SequenzViewModel> AvailableSequenzenDesCurriculums { get; private set; }

    /// <summary>
    /// Holt die TagespläneAndSequenzenCollection
    /// </summary>
    public ObservableCollection<ViewModelBase> StundenAndSequenzenCollection { get; private set; }

    ///// <summary>
    ///// Tritt auf, wenn die StundenDerLerngruppeCollectionChanged verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void StundenDerLerngruppeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "StundenDerLerngruppe", this.StundenDerLerngruppe, e, true, "Änderung der StundenDerLerngruppe");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die UsedSequenzenDesCurriculumsCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void UsedSequenzenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "UsedSequenzenDesCurriculums", this.UsedSequenzenDesCurriculums, e, true, "Änderung der UsedSequenzenDesCurriculums");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die AvailableSequenzenDesCurriculumsCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void AvailableSequenzenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "AvailableSequenzenDesCurriculums", this.AvailableSequenzenDesCurriculums, e, true, "Änderung der AvailableSequenzenDesCurriculums");
    //}

    ///// <summary>
    ///// Tritt auf, wenn die TagespläneAndSequenzenCollectionCollection verändert wurde.
    ///// Gibt die Änderungen an den Undostack weiter.
    ///// </summary>
    ///// <param name="sender">Die auslösende Collection</param>
    ///// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    //private void TagespläneAndSequenzenCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //  this.UndoableCollectionChanged(this, "TagespläneAndSequenzenCollection", this.StundenAndSequenzenCollection, e, true, "Änderung der TagespläneAndSequenzenCollection");
    //}

    public bool ShowFerien
    {
      get => showFerien;
      set
      {
        showFerien = value;
        if (showFerien)
        {
          AddFerien(lerngruppeTarget);
        }
        else
        {
          RemoveFerien();
        }

        this.RaisePropertyChanged("ShowFerien");
      }
    }

    /// <summary>
    /// Ergänzt die Ferien als Lerngruppentermine für die Übersicht
    /// </summary>
    /// <param name="lerngruppe">Lerngruppe, für die die Ferien ergänzt werden sollen</param>
    private void AddFerien(LerngruppeViewModel lerngruppe)
    {
      var ferienSource = App.MainViewModel.Ferien.Where(o => o.Model.Schuljahr.Jahr == lerngruppe.LerngruppeSchuljahr.SchuljahrJahr);
      foreach (var ferienzeit in ferienSource)
      {
        var lgt = new Stunde();
        if (ferienzeit.FerienErsterFerientag.Month > 7 || ferienzeit.FerienErsterFerientag.Month == 1)
        {
          lgt.Halbjahr = Halbjahr.Winter;
        }
        else
        {
          lgt.Halbjahr = Halbjahr.Sommer;
        }

        if (lgt.Halbjahr != this.currentHalbjahr)
        {
          continue;
        }

        lgt.ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.First().Model;
        lgt.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.Last().Model;
        lgt.Beschreibung = ferienzeit.FerienBezeichnung;
        lgt.Datum = ferienzeit.FerienErsterFerientag;
        lgt.Termintyp = Termintyp.Ferien;
        lgt.Lerngruppe = lerngruppe.Model;
        this.StundenAndSequenzenCollection.Add(new StundeViewModel(lgt));
      }
    }

    /// <summary>
    /// Löscht die Ferien aus der Übersicht
    /// </summary>
    private void RemoveFerien()
    {
      var ferienTermine = this.StundenAndSequenzenCollection.OfType<StundeViewModel>().Where(o => o.TerminTermintyp == Termintyp.Ferien).ToList();
      foreach (var ferien in ferienTermine)
      {
        this.StundenAndSequenzenCollection.Remove(ferien);
      }
    }

    /// <summary>
    /// Füllt die Collections
    /// </summary>
    private void PopulateSequenzen()
    {
      foreach (SequenzViewModel sequenzViewModel in this.curriculumSource.ReihenSequenzen.Where(o => o is SequenzViewModel).OrderBy(o => o.Reihenfolge))
      {
        this.UsedSequenzenDesCurriculums.Add(sequenzViewModel);
      }

      // ResequenceList
      SequencingService.SetCollectionSequence(this.UsedSequenzenDesCurriculums);

      foreach (ReiheViewModel reiheViewModel in this.curriculumSource.ReihenSequenzen.Where(o => o is ReiheViewModel).OrderBy(o => o.Reihenfolge))
      {
        foreach (var sequenzViewModel in reiheViewModel.AvailableSequenzen.OrderBy(o => o.Reihenfolge))
        {
          this.AvailableSequenzenDesCurriculums.Add(sequenzViewModel);
        }
      }
    }

    /// <summary>
    /// Füllt die Halbjahresübersicht mit den verfügbaren Stunden der Lerngruppe
    /// </summary>
    private void PopulateStunden()
    {
      // Verfügbare Stunden aus Lerngruppe importieren
      foreach (var stundeViewModel in this.lerngruppeTarget.Lerngruppentermine.OfType<StundeViewModel>().Where(o => o.LerngruppenterminHalbjahr == this.currentHalbjahr))
      {
        this.StundenDerLerngruppe.Add(stundeViewModel);
      }
    }

    /// <summary>
    /// Füllt die Übersichtstabelle mit Stunden und Sequenzen.
    /// </summary>
    private void PopulateBoth()
    {
      this.StundenAndSequenzenCollection.Clear();
      foreach (var stunde in this.StundenDerLerngruppe)
      {
        this.StundenAndSequenzenCollection.Add(stunde);
      }

      foreach (var usedReiheDesCurriculums in this.curriculumSource.UsedReihenDesCurriculums.OrderBy(o => o.Reihenfolge))
      {
        this.StundenAndSequenzenCollection.Add(usedReiheDesCurriculums);
      }

      foreach (var usedSequenzDesCurriculums in this.UsedSequenzenDesCurriculums)
      {
        this.StundenAndSequenzenCollection.Add(usedSequenzDesCurriculums);
      }
    }

    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data as SequenzViewModel;
      var targetItem = dropInfo.TargetItem as StundeViewModel;
      if (sourceItem != null)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Move;
      }

      if (targetItem != null)
      {
        dropInfo.Effects = DragDropEffects.None;
      }
    }

    public void Drop(IDropInfo dropInfo)
    {
      var sequenzViewModel = (SequenzViewModel)dropInfo.Data;

      if (dropInfo.VisualTarget is ListBox)
      {
        var targetListBox = dropInfo.VisualTarget as ListBox;
        if (targetListBox.Name == "AvailableItemsListBox")
        {
          if (this.UsedSequenzenDesCurriculums.Contains(sequenzViewModel))
          {
            this.AvailableSequenzenDesCurriculums.Add(sequenzViewModel);
            this.UsedSequenzenDesCurriculums.RemoveTest(sequenzViewModel);
          }
        }
        else if (targetListBox.Name == "CurriculumItemsListBox")
        {
          var newIndex = dropInfo.InsertIndex - this.StundenDerLerngruppe.Count;
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
            this.AvailableSequenzenDesCurriculums.RemoveTest(sequenzViewModel);
            this.UsedSequenzenDesCurriculums.Insert(newIndex, sequenzViewModel);
          }
        }
      }

      this.UpdateReihenfolgeIndex();
    }

    private void UpdateReihenfolgeIndex()
    {
      SequencingService.SetCollectionSequence(this.UsedSequenzenDesCurriculums);

      foreach (var model in AvailableSequenzenDesCurriculums)
      {
        model.Reihenfolge = -1;
      }

      this.UpdateTagespläneAndSequenzenCollection();
    }

    /// <summary>
    /// Removes all SequenzViewModels from the TagespläneAndSequenzenCollection
    /// and readds the updated values from the UsedSequenzenDesCurriculums collection.
    /// </summary>
    private void UpdateTagespläneAndSequenzenCollection()
    {
      var sequenzenInCollection = this.StundenAndSequenzenCollection.OfType<SequenzViewModel>().Count();
      var tagespläneInCollection = this.StundenAndSequenzenCollection.Count - sequenzenInCollection;
      for (int i = tagespläneInCollection; i < sequenzenInCollection + tagespläneInCollection; i++)
      {
        this.StundenAndSequenzenCollection.RemoveAt(this.StundenAndSequenzenCollection.Count - 1);
      }

      foreach (var usedSequenzDesCurriculums in this.UsedSequenzenDesCurriculums)
      {
        this.StundenAndSequenzenCollection.Add(usedSequenzDesCurriculums);
      }
    }

    /// <summary>
    /// Trägt die Stundenentwurfvorlagen in den Halbjahresplan ein.
    /// </summary>
    private void UpdateHalbjahresplanWithCurriculum()
    {
      var sequenzIndex = 0;
      var stundenZähler = 0;
      var aktuelleSequenz = this.UsedSequenzenDesCurriculums[sequenzIndex];
      this.ShowFerien = false;

      using (new UndoBatch(App.MainViewModel, string.Format("Stundenentwürfe aus Curriculum angepasst."), false))
      {
        foreach (StundeViewModel stunde in this.StundenDerLerngruppe)
        {
          if (aktuelleSequenz == null)
          {
            break;
          }

          stunde.StundeAnsagen = string.Empty;
          stunde.StundeComputer = false;
          stunde.StundeHausaufgaben = string.Empty;
          stunde.StundeKopieren = false;
          stunde.TerminBeschreibung = aktuelleSequenz.SequenzThema;
          stunde.StundeModul = aktuelleSequenz.SequenzReihe.ReiheModul;

          stundenZähler += stunde.TerminStundenanzahl;
          if (stundenZähler == aktuelleSequenz.SequenzStundenbedarf)
          {
            stundenZähler = 0;
            sequenzIndex++;

            if (this.UsedSequenzenDesCurriculums.Count > sequenzIndex)
            {
              aktuelleSequenz = this.UsedSequenzenDesCurriculums[sequenzIndex];
            }
            else
            {
              aktuelleSequenz = null;
              break;
            }
          }
          else if (stundenZähler > aktuelleSequenz.SequenzStundenbedarf)
          {
            sequenzIndex++;

            SequenzViewModel nächsteSequenz;
            if (this.UsedSequenzenDesCurriculums.Count > sequenzIndex)
            {
              nächsteSequenz = this.UsedSequenzenDesCurriculums[sequenzIndex];
              if (nächsteSequenz.SequenzStundenbedarf + aktuelleSequenz.SequenzStundenbedarf == stundenZähler)
              {
                stunde.TerminBeschreibung += "+ " + nächsteSequenz.SequenzThema;
                sequenzIndex++;
                aktuelleSequenz = this.UsedSequenzenDesCurriculums[sequenzIndex];
              }
              stundenZähler = 0;
            }
            else
            {
              nächsteSequenz = null;
              break;
            }
          }
        }
      }
      InformationDialog.Show("Fertig", "Jahresplan wurde aktualisiert", false);
    }
  }
}
