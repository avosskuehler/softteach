namespace SoftTeach.ViewModel.Datenbank
{
  using System;

  using System.Linq;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Modul
  /// </summary>
  public class ModulViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The fach currently assigned to this Modul
    /// </summary>
    private FachViewModel fach;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ModulViewModel"/> Klasse. 
    /// </summary>
    /// <param name="modul">
    /// The underlying modul this ViewModel is to be based on
    /// </param>
    public ModulViewModel(Modul modul)
    {
      this.Model = modul ?? throw new ArgumentNullException(nameof(modul));

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Fächer.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.ModulFach))
        {
          this.ModulFach = null;
        }
      };
    }

    /// <summary>
    /// Holt den underlying Modul this ViewModel is based on
    /// </summary>
    public Modul Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string ModulBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(ModulBezeichnung), this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("ModulBezeichnung");
      }
    }

    /// <summary>
    /// Holt den Stundenbedarf as a string
    /// </summary>
    [DependsUpon("ModulBezeichnung")]
    public string ModulKurzbezeichnung
    {
      get
      {
        return this.ModulBezeichnung.Substring(0, Math.Min(this.ModulBezeichnung.Length, 15)) + "...";
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public FachViewModel ModulFach
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
        if (value == this.fach) return;
        this.UndoablePropertyChanging(this, nameof(ModulFach), this.fach, value);
        this.fach = value;
        this.Model.Fach = value != null ? value.Model : null;

        this.RaisePropertyChanged("ModulFach");
      }
    }

    /// <summary>
    /// Holt oder setzt den Jahrgang für das Modul
    /// </summary>
    public int ModulJahrgang
    {
      get
      {
        return this.Model.Jahrgang;
      }

      set
      {
        if (value == this.Model.Jahrgang) return;
        this.UndoablePropertyChanging(this, nameof(ModulJahrgang), this.Model.Jahrgang, value);
        this.Model.Jahrgang = value;
        this.RaisePropertyChanged("ModulJahrgang");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stundenbedarf
    /// </summary>
    public int ModulStundenbedarf
    {
      get
      {
        return this.Model.Stundenbedarf;
      }

      set
      {
        if (value == this.Model.Stundenbedarf) return;
        this.UndoablePropertyChanging(this, nameof(ModulStundenbedarf), this.Model.Stundenbedarf, value);
        this.Model.Stundenbedarf = value;
        this.RaisePropertyChanged("ModulStundenbedarf");
      }
    }

    /// <summary>
    /// Holt oder setzt die Anzahl der Entwürfe zu dem Modul
    /// </summary>
    public int ModulEntwürfe
    {
      get
      {
        return this.Model.Stunden.Count;
      }
    }

    /// <summary>
    /// Holt den Stundenbedarf as a string
    /// </summary>
    [DependsUpon("ModulStundenbedarf")]
    public string ModulStundenbedarfString
    {
      get
      {
        return this.Model.Stundenbedarf + "h";
      }
    }

    /// <summary>
    /// Holt oder setzt die Bausteine
    /// </summary>
    public string ModulBausteine
    {
      get
      {
        return this.Model.Bausteine;
      }

      set
      {
        if (value == this.Model.Bausteine) return;
        this.UndoablePropertyChanging(this, nameof(ModulBausteine), this.Model.Bausteine, value);
        this.Model.Bausteine = value;
        this.RaisePropertyChanged("ModulBausteine");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Modul: " + this.ModulBezeichnung;
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="viewModel">The object to be compared with this instance</param>
    /// <returns>Less than zero if This object is less than the other parameter. 
    /// Zero if This object is equal to other. Greater than zero if This object is greater than other.
    /// </returns>
    public int CompareTo(object viewModel)
    {
      var compareModul = viewModel as ModulViewModel;
      if (compareModul == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareModul.ModulBezeichnung, StringComparison.Ordinal);
    }
  }
}
