namespace SoftTeach.ViewModel.Jahrespläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;

  using GongSolutions.Wpf.DragDrop;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Stundenentwürfe;
  using SoftTeach.ViewModel.Termine;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// ViewModel for managing Jahresplan
  /// </summary>
  public class HalbjahresplanZuweisenWorkspaceViewModel : ViewModelBase, IDropTarget
  {
    /// <summary>
    /// Der Halbjahresplan der als Vorlage dient.
    /// </summary>
    private readonly LerngruppeViewModel halbjahresplanTemplate;

    /// <summary>
    /// Der Halbjahresplan der gefüllt werden soll.
    /// </summary>
    private readonly LerngruppeViewModel currentHalbjahresplan;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="HalbjahresplanZuweisenWorkspaceViewModel"/> Klasse. 
    /// </summary>
    /// <param name="halbjahresplanTemplateViewModel">
    /// This should be a clone of a jahresplan to be adapted,
    /// that is later removed, cause the changes in this dialog should not be persistant
    /// to any saved jahresplan.
    /// </param>
    /// <param name="halbjahresplanViewModel">
    /// The HalbjahresplanViewModelwhich 
    /// contains the available stunden to be planned.
    /// </param>
    public HalbjahresplanZuweisenWorkspaceViewModel(LerngruppeViewModel halbjahresplanTemplateViewModel, LerngruppeViewModel halbjahresplanViewModel)
    {
      this.halbjahresplanTemplate = halbjahresplanTemplateViewModel;
      this.currentHalbjahresplan = halbjahresplanViewModel;
      this.StundenDerLerngruppe = new ObservableCollection<StundeViewModel>();
      this.PopulateTagespläne();
      this.UsedStundenDesHalbjahresplans = new ObservableCollection<StundeViewModel>();
      this.AvailableStundenDesHalbjahresplans = new ObservableCollection<StundeViewModel>();
      this.PopulateStunden();
      this.TagespläneAndStundenCollection = new ObservableCollection<ViewModelBase>();
      this.PopulateBoth();

      // Listen for changes
      this.StundenDerLerngruppe.CollectionChanged += this.TagespläneDesJahresplansCollectionChanged;
      this.UsedStundenDesHalbjahresplans.CollectionChanged += this.UsedStundenDesJahresplansCollectionChanged;
      this.AvailableStundenDesHalbjahresplans.CollectionChanged += this.AvailableStundenDesJahresplansCollectionChanged;
      this.TagespläneAndStundenCollection.CollectionChanged += this.TagespläneAndStundenCollectionCollectionChanged;

      this.UpdateHalbjahresplanFromTemplateCommand = new DelegateCommand(this.UpdateHalbjahresplanFromTemplate);
      this.StundeEinfügenCommand = new DelegateCommand(this.StundeEinfügen);
    }

    /// <summary>
    /// Holt den Befehl zur UpdateHalbjahresplanFromTemplate
    /// </summary>
    public DelegateCommand UpdateHalbjahresplanFromTemplateCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl eine neue Stunde einzufügen
    /// </summary>
    public DelegateCommand StundeEinfügenCommand { get; private set; }

    /// <summary>
    /// Holt die TagespläneDesHalbjahresplans
    /// </summary>
    public ObservableCollection<StundeViewModel> StundenDerLerngruppe { get; private set; }

    /// <summary>
    /// Holt die UsedStundenDesHalbjahresplans
    /// </summary>
    public ObservableCollection<StundeViewModel> UsedStundenDesHalbjahresplans { get; private set; }

    /// <summary>
    /// Holt die AvailableStundenDesHalbjahresplans
    /// </summary>
    public ObservableCollection<StundeViewModel> AvailableStundenDesHalbjahresplans { get; private set; }

    /// <summary>
    /// Holt die TagespläneAndStundenCollection
    /// </summary>
    public ObservableCollection<ViewModelBase> TagespläneAndStundenCollection { get; private set; }

    /// <summary>
    /// DragOver wird aufgerufen, wenn ein Element über eines der ListViews
    /// gezogen wird. Hier wird festgelegt, ob die Operation erlaubt wird oder nicht.
    /// </summary>
    /// <param name="dropInfo">Ein <see cref="DropInfo"/> mit dem Element was gezogen wird
    /// und dem Element auf das gezogen wurde.</param>
    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data as StundeViewModel;
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

    /// <summary>
    /// Führt die Darg and Drop Operation aus.
    /// </summary>
    /// <param name="dropInfo">Ein <see cref="DropInfo"/> mit dem Element was gezogen wird
    /// und dem Element auf das gezogen wurde.</param>
    public void Drop(IDropInfo dropInfo)
    {
      var stundeViewModel = (StundeViewModel)dropInfo.Data;

      if (dropInfo.VisualTarget is ListBox)
      {
        var targetListBox = dropInfo.VisualTarget as ListBox;
        if (targetListBox.Name == "AvailableItemsListBox")
        {
          if (this.UsedStundenDesHalbjahresplans.Contains(stundeViewModel))
          {
            this.AvailableStundenDesHalbjahresplans.Add(stundeViewModel);
            this.UsedStundenDesHalbjahresplans.RemoveTest(stundeViewModel);
          }
        }
        else if (targetListBox.Name == "JahresplanItemsListBox")
        {
          var newIndex = dropInfo.InsertIndex;
          if (newIndex < 0)
          {
            newIndex = this.UsedStundenDesHalbjahresplans.Count;
          }

          if (this.UsedStundenDesHalbjahresplans.Contains(stundeViewModel))
          {
            var oldIndex = this.UsedStundenDesHalbjahresplans.IndexOf(stundeViewModel);
            if (newIndex > oldIndex)
            {
              newIndex--;
            }

            this.UsedStundenDesHalbjahresplans.Move(oldIndex, newIndex);
          }
          else
          {
            this.AvailableStundenDesHalbjahresplans.RemoveTest(stundeViewModel);
            this.UsedStundenDesHalbjahresplans.Insert(newIndex, stundeViewModel);
          }
        }
      }

      this.UpdateReihenfolgeIndex();
    }

    /// <summary>
    /// Tritt auf, wenn die TagespläneDesJahresplansCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void TagespläneDesJahresplansCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "TagespläneDesHalbjahresplans", this.StundenDerLerngruppe, e, true, "Änderung der TagespläneDesHalbjahresplans");
    }

    /// <summary>
    /// Tritt auf, wenn die UsedStundenDesJahresplansCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void UsedStundenDesJahresplansCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "UsedStundenDesHalbjahresplans", this.UsedStundenDesHalbjahresplans, e, true, "Änderung der UsedStundenDesHalbjahresplans");
    }

    /// <summary>
    /// Tritt auf, wenn die AvailableStundenDesJahresplansCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void AvailableStundenDesJahresplansCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "AvailableStundenDesHalbjahresplans", this.AvailableStundenDesHalbjahresplans, e, true, "Änderung der AvailableStundenDesHalbjahresplans");
    }

    /// <summary>
    /// Tritt auf, wenn die TagespläneAndStundenCollectionCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void TagespläneAndStundenCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "TagespläneAndStundenCollection", this.TagespläneAndStundenCollection, e, true, "Änderung der TagespläneAndStundenCollection");
    }

    /// <summary>
    /// Füllt die Stundencollection mit den Vorlagestunden
    /// </summary>
    private void PopulateStunden()
    {
      foreach (StundeViewModel stundeViewModel in this.halbjahresplanTemplate.Model.Lerngruppentermine.OfType<Stunde.OrderBy(o => o.StundeLaufendeStundennummer))
      {
        this.UsedStundenDesHalbjahresplans.Add(stundeViewModel);
      }

      // ResequenceList
      SequencingService.SetCollectionSequence(this.UsedStundenDesHalbjahresplans);
    }

    /// <summary>
    /// Füllt die Halbjahresübersicht mit Tagesplänen
    /// </summary>
    private void PopulateTagespläne()
    {
      // Tagespläne, die ergänzt werden sollen in Stundenplan laden
      foreach (var monatsplanViewModel in this.currentHalbjahresplan.Monatspläne)
      {
        foreach (var tagesplanViewModel in monatsplanViewModel.Tagespläne)
        {
          this.StundenDerLerngruppe.Add(tagesplanViewModel);
        }
      }
    }

    /// <summary>
    /// Füllt die Übersichtstabelle mit Tagesplänen und Stunden.
    /// </summary>
    private void PopulateBoth()
    {
      this.TagespläneAndStundenCollection.Clear();
      foreach (var tagesplan in this.StundenDerLerngruppe)
      {
        this.TagespläneAndStundenCollection.Add(tagesplan);
      }

      foreach (var usedSequenzDesJahresplans in this.UsedStundenDesHalbjahresplans)
      {
        this.TagespläneAndStundenCollection.Add(usedSequenzDesJahresplans);
      }
    }

    private void UpdateReihenfolgeIndex()
    {
      SequencingService.SetCollectionSequence(this.UsedStundenDesHalbjahresplans);

      foreach (var model in this.AvailableStundenDesHalbjahresplans)
      {
        model.Reihenfolge = -1;
      }

      this.UpdateTagespläneAndStundenCollection();
    }

    /// <summary>
    /// Removes all StundeViewModels from the TagespläneAndStundenCollection
    /// and readds the updated values from the UsedStundenDesHalbjahresplans collection.
    /// </summary>
    private void UpdateTagespläneAndStundenCollection()
    {
      var stundenInCollection = this.TagespläneAndStundenCollection.OfType<StundeViewModel>().Count();
      var tagespläneInCollection = this.TagespläneAndStundenCollection.Count - stundenInCollection;
      for (int i = tagespläneInCollection; i < stundenInCollection + tagespläneInCollection; i++)
      {
        this.TagespläneAndStundenCollection.RemoveAt(this.TagespläneAndStundenCollection.Count - 1);
      }

      foreach (var usedSequenzDesJahresplans in this.UsedStundenDesHalbjahresplans)
      {
        this.TagespläneAndStundenCollection.Add(usedSequenzDesJahresplans);
      }
    }

    private void UpdateHalbjahresplanFromTemplate()
    {
      var stundeIndex = 0;
      var stundenZähler = 0;
      var aktuelleStunde = this.UsedStundenDesHalbjahresplans[stundeIndex];

      foreach (TagesplanViewModel tagesplan in this.StundenDerLerngruppe)
      {
        if (aktuelleStunde == null)
        {
          break;
        }

        foreach (var lerngruppentermin in tagesplan.Lerngruppentermine)
        {
          if (lerngruppentermin is StundeViewModel)
          {
            var stunde = lerngruppentermin as StundeViewModel;

            // Wenn kein Entwurf vorhanden, oder leer
            if (stunde.StundeStundenentwurf == null ||
              stunde.StundeStundenentwurf.Phasen.Count == 0)
            {
              stunde.StundeStundenentwurf = (StundenentwurfViewModel)aktuelleStunde.StundeStundenentwurf.Clone();
            }

            stundenZähler += stunde.TerminStundenanzahl;
            if (stundenZähler >= aktuelleStunde.TerminStundenanzahl)
            {
              stundenZähler = 0;
              stundeIndex++;

              if (this.UsedStundenDesHalbjahresplans.Count > stundeIndex)
              {
                aktuelleStunde = this.UsedStundenDesHalbjahresplans[stundeIndex];
              }
              else
              {
                aktuelleStunde = null;
                break;
              }
            }
          }
        }
      }

      InformationDialog.Show("Fertig", "Halbjahresplan wurde aktualisiert", false);
    }

    private void StundeEinfügen()
    {
      var ersteStunde = this.TagespläneAndStundenCollection.OfType<StundeViewModel>().First();

      var neuerEntwurf = new Stundenentwurf();
      neuerEntwurf.Datum = DateTime.Now;
      neuerEntwurf.Stundenthema = "Neue Stunde";
      neuerEntwurf.Fach = ersteStunde.StundeStundenentwurf.StundenentwurfFach.Model;
      neuerEntwurf.Jahrgangsstufe = ersteStunde.StundeStundenentwurf.StundenentwurfJahrgangsstufe.Model;
      neuerEntwurf.Modul = ersteStunde.StundeStundenentwurf.StundenentwurfModul.Model;
      neuerEntwurf.Stundenzahl = 1;
      neuerEntwurf.Ansagen = string.Empty;
      neuerEntwurf.Computer = false;
      neuerEntwurf.Hausaufgaben = string.Empty;
      neuerEntwurf.Kopieren = false;

      App.MainViewModel.Stunden.Add(new StundenentwurfViewModel(neuerEntwurf));

      var neueStunde = new Stunde();
      neueStunde.Termintyp = App.MainViewModel.Termintypen.First(
          termintyp => termintyp.TermintypBezeichnung == "Unterricht").Model;
      neueStunde.ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.FirstOrDefault(o => o.UnterrichtsstundeIndex == 1).Model;
      neueStunde.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.FirstOrDefault(o => o.UnterrichtsstundeIndex == 1).Model;
      neueStunde.Stundenentwurf = neuerEntwurf;
      var vm = new StundeViewModel(ersteStunde.ParentTagesplan, neueStunde);
      this.AvailableStundenDesHalbjahresplans.Add(vm);
    }
  }
}
