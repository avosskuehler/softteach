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
  using SoftTeach.ViewModel.Stundenentwürfe;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// ViewModel for managing Curriculum
  /// </summary>
  public class CurriculumZuweisenWorkspaceViewModel : ViewModelBase, IDropTarget
  {
    /// <summary>
    /// The Curriculum currently selected
    /// </summary>
    private readonly CurriculumViewModel currentCurriculum;

    /// <summary>
    /// The Curriculum currently selected
    /// </summary>
    private readonly HalbjahresplanViewModel currentHalbjahresplan;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="CurriculumZuweisenWorkspaceViewModel"/> Klasse. 
    /// </summary>
    /// <param name="curriculumViewModel">
    /// This should be a clone of a curriculum to be adapted,
    /// that is later removed, cause the changes in this dialog should not be persistant
    /// to any saved curriculum.
    /// </param>
    /// <param name="halbjahresplanViewModel">
    /// The HalbjahresplanViewModelwhich 
    /// contains the available stunden to be planned.
    /// </param>
    public CurriculumZuweisenWorkspaceViewModel(CurriculumViewModel curriculumViewModel, HalbjahresplanViewModel halbjahresplanViewModel)
    {
      this.currentCurriculum = curriculumViewModel;
      this.currentHalbjahresplan = halbjahresplanViewModel;
      this.TagespläneDesHalbjahresplans = new ObservableCollection<TagesplanViewModel>();
      this.PopulateTagespläne();
      this.UsedSequenzenDesCurriculums = new ObservableCollection<SequenzViewModel>();
      this.AvailableSequenzenDesCurriculums = new ObservableCollection<SequenzViewModel>();
      this.PopulateSequenzen();
      this.TagespläneAndSequenzenCollection = new ObservableCollection<ViewModelBase>();
      this.PopulateBoth();

      // Listen for changes
      this.TagespläneDesHalbjahresplans.CollectionChanged += this.TagespläneDesHalbjahresplansCollectionChanged;
      this.UsedSequenzenDesCurriculums.CollectionChanged += this.UsedSequenzenDesCurriculumsCollectionChanged;
      this.AvailableSequenzenDesCurriculums.CollectionChanged += this.AvailableSequenzenDesCurriculumsCollectionChanged;
      this.TagespläneAndSequenzenCollection.CollectionChanged += this.TagespläneAndSequenzenCollectionCollectionChanged;

      this.UpdateHalbjahresplanWithCurriculumCommand = new DelegateCommand(this.UpdateHalbjahresplanWithCurriculum);
    }

    /// <summary>
    /// Holt den Befehl zur UpdateHalbjahresplanWithCurriculum
    /// </summary>
    public DelegateCommand UpdateHalbjahresplanWithCurriculumCommand { get; private set; }

    /// <summary>
    /// Holt die TagespläneDesHalbjahresplans
    /// </summary>
    public ObservableCollection<TagesplanViewModel> TagespläneDesHalbjahresplans { get; private set; }

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
    public ObservableCollection<ViewModelBase> TagespläneAndSequenzenCollection { get; private set; }

    /// <summary>
    /// Tritt auf, wenn die TagespläneDesJahresplansCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void TagespläneDesHalbjahresplansCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "TagespläneDesHalbjahresplans", this.TagespläneDesHalbjahresplans, e, true, "Änderung der TagespläneDesHalbjahresplans");
    }

    /// <summary>
    /// Tritt auf, wenn die UsedSequenzenDesCurriculumsCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void UsedSequenzenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "UsedSequenzenDesCurriculums", this.UsedSequenzenDesCurriculums, e, true, "Änderung der UsedSequenzenDesCurriculums");
    }

    /// <summary>
    /// Tritt auf, wenn die AvailableSequenzenDesCurriculumsCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void AvailableSequenzenDesCurriculumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "AvailableSequenzenDesCurriculums", this.AvailableSequenzenDesCurriculums, e, true, "Änderung der AvailableSequenzenDesCurriculums");
    }

    /// <summary>
    /// Tritt auf, wenn die TagespläneAndSequenzenCollectionCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void TagespläneAndSequenzenCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "TagespläneAndSequenzenCollection", this.TagespläneAndSequenzenCollection, e, true, "Änderung der TagespläneAndSequenzenCollection");
    }

    /// <summary>
    /// Füllt die Collections
    /// </summary>
    private void PopulateSequenzen()
    {
      foreach (SequenzViewModel sequenzViewModel in this.currentCurriculum.ReihenSequenzen.Where(o => o is SequenzViewModel).OrderBy(o => o.Reihenfolge))
      {
        this.UsedSequenzenDesCurriculums.Add(sequenzViewModel);
      }

      // ResequenceList
      SequencingService.SetCollectionSequence(this.UsedSequenzenDesCurriculums);

      foreach (ReiheViewModel reiheViewModel in this.currentCurriculum.ReihenSequenzen.Where(o => o is ReiheViewModel).OrderBy(o => o.Reihenfolge))
      {
        foreach (var sequenzViewModel in reiheViewModel.AvailableSequenzen.OrderBy(o => o.Reihenfolge))
        {
          this.AvailableSequenzenDesCurriculums.Add(sequenzViewModel);
        }
      }
    }

    /// <summary>
    /// Füllt die Halbjahresübersicht mit Tagesplänen
    /// </summary>
    private void PopulateTagespläne()
    {
      // Verfügbare Stunden aus Stundenplan in Jahresplan importieren
      //this.currentHalbjahresplan.PullStunden();
      foreach (var monatsplanViewModel in this.currentHalbjahresplan.Monatspläne)
      {
        foreach (var tagesplanViewModel in monatsplanViewModel.Tagespläne)
        {
          this.TagespläneDesHalbjahresplans.Add(tagesplanViewModel);
        }
      }
    }

    /// <summary>
    /// Füllt die Übersichtstabelle mit Tagesplänen und Sequenzen.
    /// </summary>
    private void PopulateBoth()
    {
      this.TagespläneAndSequenzenCollection.Clear();
      foreach (var tagesplan in this.TagespläneDesHalbjahresplans)
      {
        this.TagespläneAndSequenzenCollection.Add(tagesplan);
      }

      foreach (var usedSequenzDesCurriculums in this.UsedSequenzenDesCurriculums)
      {
        this.TagespläneAndSequenzenCollection.Add(usedSequenzDesCurriculums);
      }
    }

    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data as SequenzViewModel;
      var targetItem = dropInfo.TargetItem as TagesplanViewModel;
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
          var newIndex = dropInfo.InsertIndex;// -this.TagespläneDesHalbjahresplans.Count;
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
      var sequenzenInCollection = this.TagespläneAndSequenzenCollection.OfType<SequenzViewModel>().Count();
      var tagespläneInCollection = this.TagespläneAndSequenzenCollection.Count - sequenzenInCollection;
      for (int i = tagespläneInCollection; i < sequenzenInCollection + tagespläneInCollection; i++)
      {
        this.TagespläneAndSequenzenCollection.RemoveAt(this.TagespläneAndSequenzenCollection.Count - 1);
      }

      foreach (var usedSequenzDesCurriculums in this.UsedSequenzenDesCurriculums)
      {
        this.TagespläneAndSequenzenCollection.Add(usedSequenzDesCurriculums);
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

      using (new UndoBatch(App.MainViewModel, string.Format("Stundenentwürfe aus Curriculum angepasst."), false))
      {
        foreach (TagesplanViewModel tagesplan in this.TagespläneDesHalbjahresplans)
        {
          if (aktuelleSequenz == null)
          {
            break;
          }

          foreach (var lerngruppentermin in tagesplan.Lerngruppentermine)
          {
            if (lerngruppentermin is StundeViewModel)
            {
              var stunde = lerngruppentermin as StundeViewModel;
              StundenentwurfViewModel entwurfViewModel = null;

              if (stunde.StundeStundenentwurf == null)
              {
                var entwurf = new Stundenentwurf();
                entwurf.Datum = DateTime.Now;
                entwurf.Fach = tagesplan.Model.Monatsplan.Halbjahresplan.Jahresplan.Fach;
                entwurf.Jahrgangsstufe =
                  tagesplan.Model.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Klassenstufe.Jahrgangsstufe;
                entwurf.Stundenzahl = stunde.TerminStundenanzahl;
                entwurf.Ansagen = string.Empty;
                entwurf.Computer = false;
                entwurf.Hausaufgaben = string.Empty;
                entwurf.Kopieren = false;
                entwurf.Stundenthema = aktuelleSequenz.SequenzThema;
                entwurf.Modul = aktuelleSequenz.SequenzReihe.ReiheModul.Model;
                //App.UnitOfWork.Context.Stundenentwürfe.Add(entwurf);
                entwurfViewModel = new StundenentwurfViewModel(entwurf);
                App.MainViewModel.Stunden.Add(entwurfViewModel);
                stunde.StundeStundenentwurf = entwurfViewModel;
              }
              else
              {
                // if entwurf is empty update also
                if (stunde.StundeStundenentwurf.Phasen.Count == 0)
                {
                  entwurfViewModel = stunde.StundeStundenentwurf;
                  entwurfViewModel.StundenentwurfDatum = DateTime.Now;
                  entwurfViewModel.StundenentwurfStundenthema = aktuelleSequenz.SequenzThema;
                  entwurfViewModel.StundenentwurfModul = aktuelleSequenz.SequenzReihe.ReiheModul;
                }
              }

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
                    entwurfViewModel.StundenentwurfStundenthema += "+ " + nächsteSequenz.SequenzThema;
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

          tagesplan.UpdateBeschreibung();
        }
      }
      InformationDialog.Show("Fertig", "Jahresplan wurde aktualisiert", false);
    }
  }
}
