namespace SoftTeach.ViewModel.Termine
{
  using System;
  using System.Linq;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual termin
  /// </summary>
  public abstract class TerminViewModel : ViewModelBase
  {
    /// <summary>
    /// The ersteUnterrichtsstunde currently assigned to this termin
    /// </summary>
    private UnterrichtsstundeViewModel ersteUnterrichtsstunde;

    /// <summary>
    /// The letzteUnterrichtsstunde currently assigned to this termin
    /// </summary>
    private UnterrichtsstundeViewModel letzteUnterrichtsstunde;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TerminViewModel"/> Klasse. 
    /// </summary>
    /// <param name="termin">
    /// The underlying termin this ViewModel is to be based on
    /// </param>
    protected TerminViewModel(TerminNeu termin)
    {
      if (termin == null)
      {
        throw new ArgumentNullException("termin");
      }

      this.Model = termin;

      App.MainViewModel.Unterrichtsstunden.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.TerminErsteUnterrichtsstunde))
        {
          this.TerminErsteUnterrichtsstunde = null;
        }

        if (e.OldItems != null && e.OldItems.Contains(this.TerminLetzteUnterrichtsstunde))
        {
          this.TerminLetzteUnterrichtsstunde = null;
        }
      };

      this.DeleteTerminCommand = new DelegateCommand(this.DeleteTermin);
    }

    /// <summary>
    /// Holt den Befehl zur deleting this termin
    /// </summary>
    public DelegateCommand DeleteTerminCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Termin this ViewModel is based on
    /// </summary>
    public TerminNeu Model { get; private set; }

    /// <summary>
    /// Holt oder setzt den termintyp currently assigned to this Termin
    /// </summary>
    public Termintyp TerminTermintyp
    {
      get
      {
        return this.Model.Termintyp;
      }

      set
      {
        if (value == this.Model.Termintyp)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "TerminTermintyp", this.Model.Termintyp, value);
        this.Model.Termintyp = value;
        this.RaisePropertyChanged("TerminTermintyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die Beschreibung of this Termin
    /// </summary>
    public virtual string TerminBeschreibung
    {
      get
      {
        return this.Model.Beschreibung;
      }

      set
      {
        if (value == this.Model.Beschreibung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "TerminBeschreibung", this.Model.Beschreibung, value);
        this.Model.Beschreibung = value;
        this.RaisePropertyChanged("TerminBeschreibung");
      }
    }

    /// <summary>
    /// Holt oder setzt die ErsteUnterrichtsstunde currently assigned to this Termin
    /// </summary>
    public UnterrichtsstundeViewModel TerminErsteUnterrichtsstunde
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.ErsteUnterrichtsstunde == null)
        {
          return null;
        }

        if (this.ersteUnterrichtsstunde == null || this.ersteUnterrichtsstunde.Model != this.Model.ErsteUnterrichtsstunde)
        {
          this.ersteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.SingleOrDefault(d => d.Model == this.Model.ErsteUnterrichtsstunde);
        }

        return this.ersteUnterrichtsstunde;
      }

      set
      {
        if (value.UnterrichtsstundeIndex == this.Model.ErsteUnterrichtsstunde.Stundenindex)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "TerminErsteUnterrichtsstunde", this.TerminErsteUnterrichtsstunde, value);
        this.ersteUnterrichtsstunde = value;
        this.Model.ErsteUnterrichtsstunde = value.Model;
        this.RaisePropertyChanged("TerminErsteUnterrichtsstunde");

        this.UpdateStundenentwurfStundenzahl();
      }
    }

    /// <summary>
    /// Holt oder setzt die LetzteUnterrichtsstunde currently assigned to this Termin
    /// </summary>
    public UnterrichtsstundeViewModel TerminLetzteUnterrichtsstunde
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.LetzteUnterrichtsstunde == null)
        {
          return null;
        }

        if (this.letzteUnterrichtsstunde == null || this.letzteUnterrichtsstunde.Model != this.Model.LetzteUnterrichtsstunde)
        {
          this.letzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden.SingleOrDefault(d => d.Model == this.Model.LetzteUnterrichtsstunde);
        }

        return this.letzteUnterrichtsstunde;
      }

      set
      {
        if (value.UnterrichtsstundeIndex == this.TerminLetzteUnterrichtsstunde.UnterrichtsstundeIndex)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "TerminLetzteUnterrichtsstunde", this.TerminLetzteUnterrichtsstunde, value);
        this.letzteUnterrichtsstunde = value;
        this.Model.LetzteUnterrichtsstunde = value.Model;
        this.RaisePropertyChanged("TerminLetzteUnterrichtsstunde");
        this.UpdateStundenentwurfStundenzahl();
      }
    }

    /// <summary>
    /// Holt oder setzt die Ort of this Termin
    /// </summary>
    public string TerminOrt
    {
      get
      {
        return this.Model.Ort;
      }

      set
      {
        if (value == this.Model.Ort)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "TerminOrt", this.Model.Ort, value);
        this.Model.Ort = value;
        this.RaisePropertyChanged("TerminOrt");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob this Termin
    /// is geprüft.
    /// </summary>
    public bool TerminIstGeprüft
    {
      get
      {
        return this.Model.IstGeprüft;
      }

      set
      {
        if (value == this.Model.IstGeprüft)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "TerminIstGeprüft", this.Model.IstGeprüft, value);
        this.Model.IstGeprüft = value;
        this.RaisePropertyChanged("TerminIstGeprüft");
      }
    }

    /// <summary>
    /// Holt den Stundenbereich für diesen Termin.
    /// </summary>
    [DependsUpon("TerminErsteUnterrichtsstunde")]
    [DependsUpon("TerminLetzteUnterrichtsstunde")]
    public string TerminStundenbereich
    {
      get
      {
        if (this.Model.ErsteUnterrichtsstunde == null || this.Model.LetzteUnterrichtsstunde == null)
        {
          return string.Empty;
        }

        var ersteStunde = this.Model.ErsteUnterrichtsstunde.Bezeichnung;
        var letzteStunde = this.Model.LetzteUnterrichtsstunde.Bezeichnung;
        if (ersteStunde == letzteStunde)
        {
          return ersteStunde;
        }

        return ersteStunde + "-" + letzteStunde;
      }
    }

    /// <summary>
    /// Holt den Stundenanzahl of this Termin
    /// </summary>
    [DependsUpon("TerminErsteUnterrichtsstunde")]
    [DependsUpon("TerminLetzteUnterrichtsstunde")]
    public int TerminStundenanzahl
    {
      get
      {
        return this.Model.LetzteUnterrichtsstunde.Stundenindex - this.Model.ErsteUnterrichtsstunde.Stundenindex + 1;
      }
    }

    /// <summary>
    /// Handles deletion of the current termin
    /// </summary>
    protected abstract void DeleteTermin();

    /// <summary>
    /// Updates the stundezahl of the contained stundenentwurf, if this is a stunde
    /// and there is any stundenentwurf created already.
    /// </summary>
    protected void UpdateStundenentwurfStundenzahl()
    {
      // TODO not needed
      //if (this is StundeViewModel)
      //{
      //  var stunde = this as StundeViewModel;
      //  if (stunde != null && stunde.StundeStundenentwurf != null)
      //  {
      //    stunde.StundeStundenentwurf.StundenentwurfStundenzahl = this.TerminStundenanzahl;
      //  }
      //}
    }
  }
}
