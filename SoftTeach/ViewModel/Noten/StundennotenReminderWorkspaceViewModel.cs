namespace SoftTeach.ViewModel.Noten
{
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Data;

  using Helper;

  using SoftTeach.Setting;
  using SoftTeach.Model.TeachyModel;

  /// <summary>
  /// ViewModel for managing Stundennote
  /// </summary>
  public class StundennotenReminderWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// Der aktuell ausgewählten Schülereintrag
    /// </summary>
    private SchülereintragViewModel currentSchülereintrag;

    /// <summary>
    /// Holt oder setzt die Stunde currently selected
    /// </summary>
    private Stunde currentStunde;

    ///// <summary>
    ///// Ein View von allen Schülern der momentanten Stunde.
    ///// </summary>
    //private ICollectionView currentSchülerView;

    /// <summary>
    /// Ein View von allen unbenoteten Stunden.
    /// </summary>
    private ObservableCollection<Stunde> nichtBenoteteStunden;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="StundennotenReminderWorkspaceViewModel"/> Klasse. 
    /// </summary>
    /// <param name="nichtBenoteteStunden">Die Liste der noch nicht benoteten Stunden.</param>
    public StundennotenReminderWorkspaceViewModel(ObservableCollection<Stunde> nichtBenoteteStunden)
    {
      this.nichtBenoteteStunden = nichtBenoteteStunden;
      this.CurrentStunde = nichtBenoteteStunden[0];

      this.NoteneintragErledigtCommand = new DelegateCommand(this.NoteneintragErledigt);
    }

    /// <summary>
    /// Holt den Befehl, um anzugeben, dass der Noteneintrag erledigt ist
    /// </summary>
    public DelegateCommand NoteneintragErledigtCommand { get; private set; }

    /// <summary>
    /// Holt ein View der Stunden, für die die Schüler benotet werden sollen.
    /// </summary>
    public ICollectionView StundenView
    {
      get
      {
        return CollectionViewSource.GetDefaultView(this.nichtBenoteteStunden);
      }
    }

    /// <summary>
    /// Holt ein View der Schüler, die benotet werden sollen.
    /// </summary>
    [DependsUpon("CurrentStunde")]
    public ObservableCollection<SchülereintragViewModel> CurrentSchülereinträge
    {
      get
      {
        var schülerliste =
          App.MainViewModel.Lerngruppen.FirstOrDefault(
            o =>
            o.LerngruppeFach.FachBezeichnung == this.CurrentStunde.Fach.Bezeichnung
            && o.LerngruppeSchuljahr.SchuljahrBezeichnung == this.CurrentStunde.Lerngruppe.Schuljahr.Bezeichnung
            && o.LerngruppeBezeichnung == this.CurrentStunde.Lerngruppe.Bezeichnung);

        Selection.Instance.Lerngruppe = schülerliste;

        if (schülerliste != null)
        {
          // Felder für zufällige Auswahl zurücksetzen
          schülerliste.Schülereinträge.Each(
            o =>
            {
              o.IstZufälligAusgewählt = false;
            });

          // Wähle zufällig Schüler aus und markiere diese
          var subset = schülerliste.Schülereinträge.TakeRandom(Configuration.Instance.NotenProStunde);
          foreach (var schülereintragViewModel in subset)
          {
            schülereintragViewModel.IstZufälligAusgewählt = true;
          }

          return schülerliste.Schülereinträge;
        }

        return null;
      }
    }

    /// <summary>
    /// Holt oder setzt die Stunde currently selected in this workspace
    /// </summary>
    public Stunde CurrentStunde
    {
      get
      {
        return this.currentStunde;
      }

      set
      {
        this.currentStunde = value;
        if (value != null)
        {
          //Selection.Instance.Stunde = this.currentStunde;
          this.RaisePropertyChanged("CurrentStunde");
        }
      }
    }

    /// <summary>
    /// Holt oder setzt den momentan ausgewählten Schülereintrag
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
        Selection.Instance.Schülereintrag = this.currentSchülereintrag;
        this.RaisePropertyChanged("CurrentSchülereintrag");
      }
    }

    /// <summary>
    /// Wenn die Notengebung für die Stunde erfolgt ist...
    /// </summary>
    private void NoteneintragErledigt()
    {
      if (this.CurrentStunde != null)
      {
        this.CurrentStunde.IstBenotet = true;
        var result = this.nichtBenoteteStunden.Remove(this.currentStunde);
        if (this.nichtBenoteteStunden.Count > 0)
        {
          this.CurrentStunde = this.nichtBenoteteStunden[0];
        }

        this.RaisePropertyChanged("StundenView");
      }
    }

  }
}
