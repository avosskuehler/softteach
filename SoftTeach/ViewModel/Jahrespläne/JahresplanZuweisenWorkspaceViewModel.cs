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
  using SoftTeach.UndoRedo;

  /// <summary>
  /// ViewModel for managing Jahresplan
  /// </summary>
  public class JahresplanZuweisenWorkspaceViewModel : ViewModelBase, IDropTarget
  {
    /// <summary>
    /// Die Lerngruppe, die als Vorlage dient.
    /// </summary>
    private readonly LerngruppeViewModel lerngruppeSource;

    /// <summary>
    /// Die Lerngruppe, die gefüllt werden soll.
    /// </summary>
    private readonly LerngruppeViewModel lerngruppeTarget;

    /// <summary>
    /// Das Halbjahr das übertragen werden soll. 
    /// </summary>
    private readonly Halbjahr Halbjahr;

    /// <summary>
    /// Gibt an, ob die Ferien mit angezeigt werden sollen.
    /// </summary>
    private bool showFerien;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="JahresplanZuweisenWorkspaceViewModel"/> Klasse. 
    /// </summary>
    /// <param name="lerngruppeSource">Die Lerngruppe, deren Stunden als Vorlage dienen sollen.</param>
    /// <param name="lerngruppeTarget">Die Lerngruppe, in die die Stunden der Vorlage übertragen werden sollen.</param>
    public JahresplanZuweisenWorkspaceViewModel(LerngruppeViewModel lerngruppeSource, LerngruppeViewModel lerngruppeTarget, Halbjahr halbjahr)
    {
      this.lerngruppeSource = lerngruppeSource;
      this.lerngruppeTarget = lerngruppeTarget;
      this.Halbjahr = halbjahr;

      this.Stunden = new ObservableCollection<LerngruppenterminViewModel>();

      // Ergänze Zieltermine
      foreach (var lerngruppentermin in lerngruppeTarget.Lerngruppentermine.OfType<StundeViewModel>().Where(o => o.LerngruppenterminHalbjahr == halbjahr))
      {
        this.Stunden.Add(lerngruppentermin);
      }

      // Ergänze Quelltermine
      foreach (var lerngruppentermin in lerngruppeSource.Lerngruppentermine.OfType<StundeViewModel>().Where(o => o.LerngruppenterminHalbjahr == halbjahr))
      {
        this.Stunden.Add(lerngruppentermin);
      }

      // Ergänze Ferien
      this.ShowFerien = true;
    }

    /// <summary>
    /// Holt die TagespläneDesHalbjahresplans
    /// </summary>
    public ObservableCollection<LerngruppenterminViewModel> Stunden { get; private set; }

    public bool ShowFerien
    {
      get => showFerien;
      set
      {
        showFerien = value;
        if (showFerien)
        {
          AddFerien(lerngruppeSource);
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
        var lgt = new Lerngruppentermin();
        if (ferienzeit.FerienErsterFerientag.Month > 7 || ferienzeit.FerienErsterFerientag.Month == 1)
        {
          lgt.Halbjahr = Halbjahr.Winter;
        }
        else
        {
          lgt.Halbjahr = Halbjahr.Sommer;
        }

        if (lgt.Halbjahr != this.Halbjahr)
        {
          continue;
        }

        lgt.ErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.First().Model;
        lgt.LetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.Last().Model;
        lgt.Beschreibung = ferienzeit.FerienBezeichnung;
        lgt.Datum = ferienzeit.FerienErsterFerientag;
        lgt.Termintyp = Termintyp.Ferien;
        lgt.Lerngruppe = lerngruppe.Model;
        this.Stunden.Add(new LerngruppenterminViewModel(lgt));
      }
    }

    /// <summary>
    /// Löscht die Ferien aus der Übersicht
    /// </summary>
    private void RemoveFerien()
    {
      var ferienTermine = this.Stunden.Where(o => o.TerminTermintyp == Termintyp.Ferien).ToList();
      foreach (var ferien in ferienTermine)
      {
        this.Stunden.Remove(ferien);
      }
    }

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
      if (sourceItem != null && sourceItem.LerngruppenterminLerngruppe == this.lerngruppeSource)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.Effects = DragDropEffects.Copy;
      }

      if (targetItem == null || targetItem.LerngruppenterminLerngruppe != this.lerngruppeTarget)
      {
        dropInfo.Effects = DragDropEffects.None;
      }
    }

    /// <summary>
    /// Führt die Drag and Drop Operation aus.
    /// </summary>
    /// <param name="dropInfo">Ein <see cref="DropInfo"/> mit dem Element was gezogen wird
    /// und dem Element auf das gezogen wurde.</param>
    public void Drop(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data as StundeViewModel;
      var targetItem = dropInfo.TargetItem as StundeViewModel;
      if (sourceItem == null || targetItem == null)
      {
        return;
      }

      if (targetItem.Phasen.Any())
      {
        var result = InformationDialog.Show("Nicht leer", "Die Zielstunde ist nicht leer, sollen die Stunde überschrieben werden?", true);
        switch (result)
        {
          case true:
            break;
          case false:
          case null:
          default:
            return;
        }
      }

      if (targetItem.StundeStundenzahl < sourceItem.StundeStundenzahl)
      {
        var result = InformationDialog.Show("Zeitproblem", "Die Zielstunde ist kürzer als die Quellstunde, sollen trotzdem alle Phasen übertragen werden?", true);
        switch (result)
        {
          case true:
            break;
          case false:
          case null:
          default:
            return;
        }
      }

      if (targetItem.StundeStundenzahl > sourceItem.StundeStundenzahl)
      {
        var result = InformationDialog.Show("Zeitproblem", "Die Zielstunde ist länger als die Quellstunde, die kopierten Phasen werden nicht die ganze Stunde füllen. Soll die Phasen trotzdem übertragen werden?", true);
        switch (result)
        {
          case true:
            break;
          case false:
          case null:
          default:
            return;
        }
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Drag and Drop in der Stundenplanung"), false))
      {

        targetItem.StundeAnsagen = sourceItem.StundeAnsagen;
        targetItem.StundeComputer = sourceItem.StundeComputer;

        // Bestehende Dateiverweise löschen
        foreach (var dateiverweis in targetItem.Dateiverweise.ToList())
        {
          targetItem.Dateiverweise.Remove(dateiverweis);
        }

        // Dateiverweise kopieren
        foreach (var dateiverweis in sourceItem.Dateiverweise.ToList())
        {
          var dateiverweisClone = new Dateiverweis
          {
            Dateiname = dateiverweis.DateiverweisDateiname,
            Dateityp = dateiverweis.DateiverweisDateityp.Model,
            Stunde = (Stunde)targetItem.Model
          };
          targetItem.Dateiverweise.Add(new DateiverweisViewModel(dateiverweisClone));
        }

        targetItem.StundeFach = sourceItem.StundeFach;
        targetItem.StundeHausaufgaben = sourceItem.StundeHausaufgaben;
        targetItem.StundeJahrgang = sourceItem.StundeJahrgang;
        targetItem.StundeKopieren = sourceItem.StundeKopieren;
        targetItem.StundeModul = sourceItem.StundeModul;
        targetItem.TerminBeschreibung = sourceItem.TerminBeschreibung;

        // Bestehende Phasen löschen
        foreach (var phase in targetItem.Phasen.ToList())
        {
          targetItem.Phasen.Remove(phase);
        }

        // Phasen kopieren
        foreach (var phase in sourceItem.Phasen.ToList())
        {
          var phaseClone = new Phase
          {
            Reihenfolge = phase.Reihenfolge,
            Inhalt = phase.PhaseInhalt,
            Medium = phase.PhaseMedium,
            Sozialform = phase.PhaseSozialform,
            Zeit = phase.PhaseZeit,
            Stunde = (Stunde)targetItem.Model
          };
          targetItem.Phasen.Add(new PhaseViewModel(phaseClone));
        }
      }
    }
  }
}
