namespace SoftTeach.ViewModel.Datenbank
{
  using System;

  using System.Linq;

  using SoftTeach.Model.EntityFramework;
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
    /// The jahrgangsstufe currently assigned to this Modul
    /// </summary>
    private JahrgangsstufeViewModel jahrgangsstufe;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ModulViewModel"/> Klasse. 
    /// </summary>
    /// <param name="modul">
    /// The underlying modul this ViewModel is to be based on
    /// </param>
    public ModulViewModel(Modul modul)
    {
      if (modul == null)
      {
        throw new ArgumentNullException("modul");
      }

      this.Model = modul;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Fächer.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.ModulFach))
        {
          this.ModulFach = null;
        }
      };

      App.MainViewModel.Jahrgangsstufen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.ModulJahrgangsstufe))
        {
          this.ModulJahrgangsstufe = null;
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
        this.UndoablePropertyChanging(this, "ModulBezeichnung", this.Model.Bezeichnung, value);
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
        this.UndoablePropertyChanging(this, "ModulFach", this.fach, value);
        this.fach = value;
        this.Model.Fach = value != null ? value.Model : null;

        this.RaisePropertyChanged("ModulFach");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public JahrgangsstufeViewModel ModulJahrgangsstufe
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Jahrgangsstufe == null)
        {
          return null;
        }

        if (this.jahrgangsstufe == null || this.jahrgangsstufe.Model != this.Model.Jahrgangsstufe)
        {
          this.jahrgangsstufe = App.MainViewModel.Jahrgangsstufen.SingleOrDefault(d => d.Model == this.Model.Jahrgangsstufe);
        }

        return this.jahrgangsstufe;
      }

      set
      {
        if (value.JahrgangsstufeBezeichnung == this.jahrgangsstufe.JahrgangsstufeBezeichnung) return;
        this.UndoablePropertyChanging(this, "ModulJahrgangsstufe", this.jahrgangsstufe, value);
        this.jahrgangsstufe = value;
        this.Model.Jahrgangsstufe = value.Model;
        this.RaisePropertyChanged("ModulJahrgangsstufe");
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
        this.UndoablePropertyChanging(this, "ModulStundenbedarf", this.Model.Stundenbedarf, value);
        this.Model.Stundenbedarf = value;
        this.RaisePropertyChanged("ModulStundenbedarf");
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
        this.UndoablePropertyChanging(this, "ModulBausteine", this.Model.Bausteine, value);
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
