namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;
  using System.Linq;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual phase
  /// </summary>
  public class PhaseViewModel : ViewModelBase, ISequencedObject, ICloneable
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="PhaseViewModel"/> Klasse. 
    /// </summary>
    public PhaseViewModel()
    {
      var phase = new Phase
      {
        Reihenfolge = Selection.Instance.Stunde.Phasen.Count + 1,
        Zeit = 10,
        Inhalt = string.Empty,
        Medium = Medium.Tafel,
        Sozialform = Sozialform.UG,
        Stunde = Selection.Instance.Stunde.Model as Stunde
      };
      this.Model = phase;
      //App.UnitOfWork.Context.Phasen.Add(phase);
      //App.MainViewModel.Phasen.Add(this);
      //Selection.Instance.Stundenentwurf.Phasen.Add(this);
    }

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="PhaseViewModel"/> Klasse. 
    /// </summary>
    /// <param name="phase">
    /// The underlying phase this ViewModel is to be based on
    /// </param>
    public PhaseViewModel(Phase phase)
    {
      this.Model = phase ?? throw new ArgumentNullException(nameof(phase));
    }

    /// <summary>
    /// Holt den underlying Phase this ViewModel is based on
    /// </summary>
    public Phase Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Inhalt
    /// </summary>
    public string PhaseInhalt
    {
      get
      {
        return this.Model.Inhalt;
      }

      set
      {
        if (value == this.Model.Inhalt) return;
        this.UndoablePropertyChanging(this, nameof(PhaseInhalt), this.Model.Inhalt, value);
        this.Model.Inhalt = value;
        this.RaisePropertyChanged("PhaseInhalt");
      }
    }

    /// <summary>
    /// Holt eine Kurzform des Inhalts
    /// </summary>
    [DependsUpon("PhaseInhalt")]
    public string PhaseKurzinhalt
    {
      get
      {
        var indexOfCarriageReturn = this.PhaseInhalt.IndexOf('\\');
        if (indexOfCarriageReturn < 0)
        {
          return this.PhaseInhalt.Substring(0, Math.Min(20, this.PhaseInhalt.Length));
        }

        return this.PhaseInhalt.Substring(0, indexOfCarriageReturn);
      }
    }

    /// <summary>
    /// Holt oder setzt das Medium halbjahr currently assigned to this Phase
    /// </summary>
    public Medium PhaseMedium
    {
      get
      {
        return this.Model.Medium;
      }

      set
      {
        if (value == this.Model.Medium) return;
        this.UndoablePropertyChanging(this, nameof(PhaseMedium), this.Model.Medium, value);
        this.Model.Medium = value;
        this.RaisePropertyChanged("PhaseMedium");
      }
    }

    /// <summary>
    /// Holt oder setzt die Sozialform currently assigned to this Phase
    /// </summary>
    public Sozialform PhaseSozialform
    {
      get
      {
        return this.Model.Sozialform;
      }

      set
      {
        if (value == this.Model.Sozialform) return;
        this.UndoablePropertyChanging(this, nameof(PhaseSozialform), this.Model.Sozialform, value);
        this.Model.Sozialform = value;
        this.RaisePropertyChanged("PhaseSozialform");
      }
    }

    /// <summary>
    /// Holt oder setzt die Zeit
    /// </summary>
    public int PhaseZeit
    {
      get
      {
        return this.Model.Zeit;
      }

      set
      {
        if (value == this.Model.Zeit) return;
        this.UndoablePropertyChanging(this, nameof(PhaseZeit), this.Model.Zeit, value);
        this.Model.Zeit = value;
        this.RaisePropertyChanged("PhaseZeit");
        Selection.Instance.Stunde.NotifyPhaseZeitChanged();
      }
    }

    /// <summary>
    /// Holt den Zeitbedarf als String in der Form (10') 
    /// </summary>
    public string PhaseZeitbedarf
    {
      get
      {
        return string.Format("({0}')", this.PhaseZeit);
      }
    }

    /// <summary>
    /// Holt oder setzt die Reihenfolge
    /// </summary>
    public int Reihenfolge
    {
      get
      {
        return this.Model.Reihenfolge;
      }

      set
      {
        if (value == this.Model.Reihenfolge) return;
        this.UndoablePropertyChanging(this, nameof(Reihenfolge), this.Model.Reihenfolge, value);
        this.Model.Reihenfolge = value;
        this.RaisePropertyChanged("Reihenfolge");
      }
    }

    /// <summary>
    /// Holt oder setzt die Zeitraum of this phase (using a specific start time)
    /// </summary>
    public string PhaseZeitraum { get; set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Phase: " + this.PhaseInhalt + ", Index:" + this.Reihenfolge;
    }

    /// <summary>
    /// Returns a deep copy of this PhaseViewModel.
    /// </summary>
    /// <returns>A cloned <see cref="PhaseViewModel"/></returns>
    public object Clone()
    {
      var phaseClone = new Phase
      {
        Reihenfolge = this.Model.Reihenfolge,
        Inhalt = this.Model.Inhalt,
        Medium = this.Model.Medium,
        Sozialform = this.Model.Sozialform,
        Zeit = this.Model.Zeit,
        Stunde = this.Model.Stunde
      };
      //App.UnitOfWork.Context.Phasen.Add(phaseClone);

      var vm = new PhaseViewModel(phaseClone);
      //App.MainViewModel.Phasen.Add(vm);

      return vm;
    }
  }
}
