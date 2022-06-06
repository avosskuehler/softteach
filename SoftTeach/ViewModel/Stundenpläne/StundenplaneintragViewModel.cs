namespace SoftTeach.ViewModel.Stundenpläne
{
  using System;
  using System.Linq;
  using System.Windows.Media;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Stundenpläne;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Sitzpläne;

  /// <summary>
  /// ViewModel of an individual stundenplaneintrag
  /// </summary>
  public class StundenplaneintragViewModel : ViewModelBase
  {
    ///// <summary>
    ///// The fach currently assigned to this stundenplaneintrag
    ///// </summary>
    //private FachViewModel fach;

    /// <summary>
    /// The klasse currently assigned to this stundenplaneintrag
    /// </summary>
    private LerngruppeViewModel lerngruppe;

    /// <summary>
    /// The raum currently assigned to this stundenplaneintrag
    /// </summary>
    private RaumViewModel raum;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="StundenplaneintragViewModel"/> Klasse. 
    /// </summary>
    /// <param name="stundenplaneintrag">
    /// The underlying stundenplaneintrag this ViewModel is to be based on
    /// </param>
    public StundenplaneintragViewModel(Stundenplaneintrag stundenplaneintrag)
    {
      this.Model = stundenplaneintrag ?? throw new ArgumentNullException(nameof(stundenplaneintrag));
    }

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="StundenplaneintragViewModel"/> Klasse. 
    /// </summary>
    /// <param name="parent">
    /// The stundenplan this ViewModel belongs to.
    /// </param>
    /// <param name="stundenplaneintrag">
    /// The underlying stundenplaneintrag this ViewModel is to be based on
    /// </param>
    public StundenplaneintragViewModel(StundenplanViewModel parent, Stundenplaneintrag stundenplaneintrag)
    {
      this.Model = stundenplaneintrag ?? throw new ArgumentNullException(nameof(stundenplaneintrag));
      this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));

      this.EditStundenplaneintragCommand = new DelegateCommand(this.EditStundenplaneintrag);
      this.RemoveStundenplaneintragCommand = new DelegateCommand(this.RemoveStundenplaneintrag);
    }

    /// <summary>
    /// Holt den parent of this stundenplaneintrag
    /// </summary>
    public StundenplanViewModel Parent { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Stundenplaneintrag
    /// </summary>
    public DelegateCommand EditStundenplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Stundenplaneintrag
    /// </summary>
    public DelegateCommand RemoveStundenplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Stundenplaneintrag this ViewModel is based on
    /// </summary>
    public Stundenplaneintrag Model { get; private set; }

    /// <summary>
    /// Holt oder setzt den Raum für den Stundenplaneintrag
    /// </summary>
    public RaumViewModel StundenplaneintragRaum
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Raum == null)
        {
          return null;
        }

        if (this.raum == null || this.raum.Model != this.Model.Raum)
        {
          this.raum = App.MainViewModel.Räume.SingleOrDefault(d => d.Model == this.Model.Raum);
        }

        return this.raum;
      }

      set
      {
        if (value == null) return;
        if (value == this.raum)
        {
          return;
        }

        this.UndoablePropertyChanging(this, nameof(StundenplaneintragRaum), this.raum, value);
        this.raum = value;
        this.Model.Raum = value.Model;
        this.RaisePropertyChanged("StundenplaneintragRaum");
      }
    }


    /// <summary>
    /// Holt oder setzt die WochentagIndex
    /// </summary>
    public int StundenplaneintragWochentagIndex
    {
      get
      {
        return this.Model.WochentagIndex;
      }

      set
      {
        if (value == this.Model.WochentagIndex) return;
        this.UndoablePropertyChanging(this, nameof(StundenplaneintragWochentagIndex), this.Model.WochentagIndex, value);
        this.Model.WochentagIndex = value;
        this.RaisePropertyChanged("StundenplaneintragWochentagIndex");
      }
    }

    /// <summary>
    /// Holt oder setzt die Klasse currently assigned to this Jahresplan
    /// </summary>
    public LerngruppeViewModel StundenplaneintragLerngruppe
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Lerngruppe == null)
        {
          return null;
        }

        if (this.lerngruppe == null || this.lerngruppe.Model != this.Model.Lerngruppe)
        {
          this.lerngruppe = App.MainViewModel.Lerngruppen.SingleOrDefault(d => d.Model == this.Model.Lerngruppe);
        }

        return this.lerngruppe;
      }

      set
      {
        if (value == null) return;
        if (this.lerngruppe != null)
        {
          if (value.LerngruppeBezeichnung == this.lerngruppe.LerngruppeBezeichnung) return;
        }

        this.UndoablePropertyChanging(this, nameof(StundenplaneintragLerngruppe), this.lerngruppe, value);
        this.lerngruppe = value;
        this.Model.Lerngruppe = value.Model;
        this.RaisePropertyChanged("StundenplaneintragLerngruppe");
      }
    }

    ///// <summary>
    ///// Holt oder setzt die fach currently assigned to this Stundenentwurf
    ///// </summary>
    //public FachViewModel StundenplaneintragFach
    //{
    //  get
    //  {
    //    // We need to reflect any changes made in the model so we check the current value before returning
    //    if (this.Model.Fach == null)
    //    {
    //      return null;
    //    }

    //    if (this.fach == null || this.fach.Model != this.Model.Fach)
    //    {
    //      this.fach = App.MainViewModel.Fächer.SingleOrDefault(d => d.Model == this.Model.Fach);
    //    }

    //    return this.fach;
    //  }

    //  set
    //  {
    //    if (value == null) return;
    //    if (this.fach != null)
    //    {
    //      if (value.FachBezeichnung == this.fach.FachBezeichnung) return;
    //    }

    //    this.UndoablePropertyChanging(this, "StundenplaneintragFach", this.fach, value);
    //    this.fach = value;
    //    this.Model.Fach = value.Model;
    //    this.RaisePropertyChanged("StundenplaneintragFach");
    //  }
    //}

    /// <summary>
    /// Holt oder setzt die ErsteUnterrichtsstundeIndex
    /// </summary>
    public int StundenplaneintragErsteUnterrichtsstundeIndex
    {
      get
      {
        return this.Model.ErsteUnterrichtsstundeIndex;
      }

      set
      {
        if (value == this.Model.ErsteUnterrichtsstundeIndex) return;
        this.UndoablePropertyChanging(
          this, nameof(StundenplaneintragErsteUnterrichtsstundeIndex), this.Model.ErsteUnterrichtsstundeIndex, value);
        this.Model.ErsteUnterrichtsstundeIndex = value;
        this.RaisePropertyChanged("StundenplaneintragErsteUnterrichtsstundeIndex");
      }
    }

    /// <summary>
    /// Holt oder setzt die LetzteUnterrichtsstundeIndex
    /// </summary>
    public int StundenplaneintragLetzteUnterrichtsstundeIndex
    {
      get
      {
        return this.Model.LetzteUnterrichtsstundeIndex;
      }

      set
      {
        if (value == this.Model.LetzteUnterrichtsstundeIndex) return;
        this.UndoablePropertyChanging(
          this, nameof(StundenplaneintragLetzteUnterrichtsstundeIndex), this.Model.LetzteUnterrichtsstundeIndex, value);
        this.Model.LetzteUnterrichtsstundeIndex = value;
        this.RaisePropertyChanged("StundenplaneintragLetzteUnterrichtsstundeIndex");
      }
    }

    /// <summary>
    /// Holt den Stundenanzahl of this Stundenplaneintrag
    /// </summary>
    [DependsUpon("StundenplaneintragErsteUnterrichtsstundeIndex")]
    [DependsUpon("StundenplaneintragLetzteUnterrichtsstundeIndex")]
    public int Stundenanzahl
    {
      get
      {
        return this.Model.LetzteUnterrichtsstundeIndex - this.Model.ErsteUnterrichtsstundeIndex + 1;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob dieser Stundenplaneintrag ein Dummy ist.
    /// </summary>
    [DependsUpon("StundenplaneintragLerngruppe")]
    public bool IsDummy
    {
      get
      {
        return this.Model.Lerngruppe == null;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob dieser Stundenplaneintrag in Editmode ist.
    /// </summary>
    public bool IsEditMode
    {
      get
      {
        return this.Parent == null || (this.Parent.ViewMode & StundenplanViewMode.Edit) == StundenplanViewMode.Edit;
      }
    }

    /// <summary>
    /// Holt die Hintergrundfarbe für diesen Stundenplaneintrag
    /// </summary>
    [DependsUpon("StundenplaneintragLerngruppe")]
    public SolidColorBrush StundenplaneintragBackground
    {
      get
      {
        if (this.StundenplaneintragLerngruppe == null || this.StundenplaneintragLerngruppe.LerngruppeFach == null)
        {
          return Brushes.Transparent;
        }

        var newColor = this.StundenplaneintragLerngruppe.LerngruppeFach.FachFarbe;
        return new SolidColorBrush(newColor);
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Stundenplaneintrag: " + this.StundenplaneintragErsteUnterrichtsstundeIndex + ". bis "
             + this.StundenplaneintragLetzteUnterrichtsstundeIndex + ". Stunde";
    }

    /// <summary>
    /// Handles addition a new Stundenplaneintrag to this stundenplan
    /// </summary>
    private void EditStundenplaneintrag()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Stundenplaneintrag {0} geändert.", this), false))
      {
        var dlg = new AddStundenplaneintragDialog(this);
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          //this.parent.UpdateProperties(this);
        }
      }
    }

    /// <summary>
    /// Handles deletion of the current Stundenplaneintrag
    /// </summary>
    private void RemoveStundenplaneintrag()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Stundenplaneintrag {0} gelöscht.", this), false))
      {
        this.Parent.DeleteStundenplaneintrag(this);
        this.Parent.UpdateProperties(this);
      }
    }
  }
}
