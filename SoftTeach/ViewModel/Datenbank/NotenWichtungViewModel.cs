namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual NotenWichtung
  /// </summary>
  public class NotenWichtungViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="NotenWichtungViewModel"/> Klasse. 
    /// </summary>
    /// <param name="notenWichtung">
    /// The underlying notenWichtung this ViewModel is to be based on
    /// </param>
    public NotenWichtungViewModel(NotenWichtung notenWichtung)
    {
      this.Model = notenWichtung ?? throw new ArgumentNullException(nameof(notenWichtung));
    }

    /// <summary>
    /// Holt den underlying NotenWichtung this ViewModel is based on
    /// </summary>
    public NotenWichtung Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string NotenWichtungBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(NotenWichtungBezeichnung), this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("NotenWichtungBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die MündlichGesamt
    /// </summary>
    public float NotenWichtungMündlichGesamt
    {
      get
      {
        return this.Model.MündlichGesamt;
      }

      set
      {
        if (value == this.Model.MündlichGesamt) return;
        this.UndoablePropertyChanging(this, nameof(NotenWichtungMündlichGesamt), this.Model.MündlichGesamt, value);
        this.Model.MündlichGesamt = value;
        this.RaisePropertyChanged("NotenWichtungMündlichGesamt");
      }
    }


    /// <summary>
    /// Holt oder setzt die MündlichQualität
    /// </summary>
    public float NotenWichtungMündlichQualität
    {
      get
      {
        return this.Model.MündlichQualität;
      }

      set
      {
        if (value == this.Model.MündlichQualität) return;
        this.UndoablePropertyChanging(this, nameof(NotenWichtungMündlichQualität), this.Model.MündlichQualität, value);
        this.Model.MündlichQualität = value;
        this.RaisePropertyChanged("NotenWichtungMündlichQualität");
      }
    }

    /// <summary>
    /// Holt oder setzt die MündlichQuantität
    /// </summary>
    public float NotenWichtungMündlichQuantität
    {
      get
      {
        return this.Model.MündlichQuantität;
      }

      set
      {
        if (value == this.Model.MündlichQuantität) return;
        this.UndoablePropertyChanging(this, nameof(NotenWichtungMündlichQuantität), this.Model.MündlichQuantität, value);
        this.Model.MündlichQuantität = value;
        this.RaisePropertyChanged("NotenWichtungMündlichQuantität");
      }
    }

    /// <summary>
    /// Holt oder setzt die MündlichSonstige
    /// </summary>
    public float NotenWichtungMündlichSonstige
    {
      get
      {
        return this.Model.MündlichSonstige;
      }

      set
      {
        if (value == this.Model.MündlichSonstige) return;
        this.UndoablePropertyChanging(this, nameof(NotenWichtungMündlichSonstige), this.Model.MündlichSonstige, value);
        this.Model.MündlichSonstige = value;
        this.RaisePropertyChanged("NotenWichtungMündlichSonstige");
      }
    }

    /// <summary>
    /// Holt oder setzt die SchriftlichGesamt
    /// </summary>
    public float NotenWichtungSchriftlichGesamt
    {
      get
      {
        return this.Model.SchriftlichGesamt;
      }

      set
      {
        if (value == this.Model.SchriftlichGesamt) return;
        this.UndoablePropertyChanging(this, nameof(NotenWichtungSchriftlichGesamt), this.Model.SchriftlichGesamt, value);
        this.Model.SchriftlichGesamt = value;
        this.RaisePropertyChanged("NotenWichtungSchriftlichGesamt");
      }
    }

    /// <summary>
    /// Holt oder setzt die SchriftlichKlassenarbeit
    /// </summary>
    public float NotenWichtungSchriftlichKlassenarbeit
    {
      get
      {
        return this.Model.SchriftlichKlassenarbeit;
      }

      set
      {
        if (value == this.Model.SchriftlichKlassenarbeit) return;
        this.UndoablePropertyChanging(this, nameof(NotenWichtungSchriftlichKlassenarbeit), this.Model.SchriftlichKlassenarbeit, value);
        this.Model.SchriftlichKlassenarbeit = value;
        this.RaisePropertyChanged("NotenWichtungSchriftlichKlassenarbeit");
      }
    }

    /// <summary>
    /// Holt oder setzt die SchriftlichSonstige
    /// </summary>
    public float NotenWichtungSchriftlichSonstige
    {
      get
      {
        return this.Model.SchriftlichSonstige;
      }

      set
      {
        if (value == this.Model.SchriftlichSonstige) return;
        this.UndoablePropertyChanging(this, nameof(NotenWichtungSchriftlichSonstige), this.Model.SchriftlichSonstige, value);
        this.Model.SchriftlichSonstige = value;
        this.RaisePropertyChanged("NotenWichtungSchriftlichSonstige");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "NotenWichtung: " + this.NotenWichtungBezeichnung;
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
      var compareNotenWichtung = viewModel as NotenWichtungViewModel;
      if (compareNotenWichtung == null)
      {
        return -1;
      }

      return string.Compare(this.Model.Bezeichnung, compareNotenWichtung.NotenWichtungBezeichnung, StringComparison.Ordinal);
    }

  }
}
