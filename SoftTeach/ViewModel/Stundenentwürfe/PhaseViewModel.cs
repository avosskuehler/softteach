namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;
  using System.Linq;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual phase
  /// </summary>
  public class PhaseViewModel : ViewModelBase, ISequencedObject, ICloneable
  {
    /// <summary>
    /// The medium currently assigned to this phase
    /// </summary>
    private MediumViewModel medium;

    /// <summary>
    /// The sozialform currently assigned to this phase
    /// </summary>
    private SozialformViewModel sozialform;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="PhaseViewModel"/> Klasse. 
    /// </summary>
    public PhaseViewModel()
    {
      var phase = new Phase();
      phase.AbfolgeIndex = Selection.Instance.Stundenentwurf.Phasen.Count + 1;
      phase.Zeit = 10;
      phase.Inhalt = string.Empty;
      phase.Medium = App.MainViewModel.Medien.First().Model;
      phase.Sozialform = App.MainViewModel.Sozialformen.First().Model;
      phase.Stundenentwurf = Selection.Instance.Stundenentwurf.Model;
      this.Model = phase;
      App.UnitOfWork.Context.Phasen.Add(phase);
      //App.MainViewModel.Phasen.Add(this);
      Selection.Instance.Stundenentwurf.AttachPhaseChangedEvent(this);
      //Selection.Instance.Stundenentwurf.Phasen.Add(this);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="PhaseViewModel"/> Klasse. 
    /// </summary>
    /// <param name="phase">
    /// The underlying phase this ViewModel is to be based on
    /// </param>
    public PhaseViewModel(Phase phase)
    {
      if (phase == null)
      {
        throw new ArgumentNullException("phase");
      }

      this.Model = phase;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Medien.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.PhaseMedium))
        {
          this.PhaseMedium = App.MainViewModel.Medien.FirstOrDefault();
        }
      };

      App.MainViewModel.Sozialformen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.PhaseSozialform))
        {
          this.PhaseSozialform = App.MainViewModel.Sozialformen.FirstOrDefault();
        }
      };
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
        this.UndoablePropertyChanging(this, "PhaseInhalt", this.Model.Inhalt, value);
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
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public MediumViewModel PhaseMedium
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Medium == null)
        {
          return null;
        }

        if (this.medium == null || this.medium.Model != this.Model.Medium)
        {
          this.medium = App.MainViewModel.Medien.SingleOrDefault(d => d.Model.Bezeichnung == this.Model.Medium.Bezeichnung);
        }

        return this.medium;
      }

      set
      {
        if (value.MediumBezeichnung == this.medium.MediumBezeichnung) return;
        this.UndoablePropertyChanging(this, "PhaseMedium", this.medium, value);
        this.medium = value;
        this.Model.Medium = value.Model;
        this.RaisePropertyChanged("PhaseMedium");
      }
    }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public SozialformViewModel PhaseSozialform
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Sozialform == null)
        {
          return null;
        }

        if (this.sozialform == null || this.sozialform.Model != this.Model.Sozialform)
        {
          this.sozialform = App.MainViewModel.Sozialformen.SingleOrDefault(d => d.Model.Bezeichnung == this.Model.Sozialform.Bezeichnung);
        }

        return this.sozialform;
      }

      set
      {
        if (value.SozialformBezeichnung == this.PhaseSozialform.SozialformBezeichnung) return;
        this.UndoablePropertyChanging(this, "PhaseSozialform", this.sozialform, value);
        this.sozialform = value;
        this.Model.Sozialform = value.Model;
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
        this.UndoablePropertyChanging(this, "PhaseZeit", this.Model.Zeit, value);
        this.Model.Zeit = value;
        this.RaisePropertyChanged("PhaseZeit");
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
    /// Holt oder setzt die AbfolgeIndex
    /// </summary>
    public int AbfolgeIndex
    {
      get
      {
        return this.Model.AbfolgeIndex;
      }

      set
      {
        if (value == this.Model.AbfolgeIndex) return;
        this.UndoablePropertyChanging(this, "AbfolgeIndex", this.Model.AbfolgeIndex, value);
        this.Model.AbfolgeIndex = value;
        this.RaisePropertyChanged("AbfolgeIndex");
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
      return "Phase: " + this.PhaseInhalt + ", Index:" + this.AbfolgeIndex;
    }

    /// <summary>
    /// Returns a deep copy of this PhaseViewModel.
    /// </summary>
    /// <returns>A cloned <see cref="PhaseViewModel"/></returns>
    public object Clone()
    {
      var phaseClone = new Phase();
      phaseClone.AbfolgeIndex = this.Model.AbfolgeIndex;
      phaseClone.Inhalt = this.Model.Inhalt;
      phaseClone.Medium = this.Model.Medium;
      phaseClone.Sozialform = this.Model.Sozialform;
      phaseClone.Zeit = this.Model.Zeit;
      phaseClone.Stundenentwurf = this.Model.Stundenentwurf;
      App.UnitOfWork.Context.Phasen.Add(phaseClone);

      var vm = new PhaseViewModel(phaseClone);
      //App.MainViewModel.Phasen.Add(vm);

      return vm;
    }
  }
}
