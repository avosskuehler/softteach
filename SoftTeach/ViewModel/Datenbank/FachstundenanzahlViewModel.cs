namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Linq;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Fachstundenanzahl
  /// </summary>
  public class FachstundenanzahlViewModel : ViewModelBase
  {
    ///// <summary>
    ///// The klassenstufe currently assigned to this FachstundenanzahlViewModel
    ///// </summary>
    //private int jahrgang;

    /// <summary>
    /// The fach currently assigned to this FachstundenanzahlViewModel
    /// </summary>
    private FachViewModel fach;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="FachstundenanzahlViewModel"/> Klasse. 
    /// </summary>
    /// <param name="fachstundenanzahl">
    /// The underlying fachstundenanzahl this ViewModel is to be based on
    /// </param>
    public FachstundenanzahlViewModel(Fachstundenanzahl fachstundenanzahl)
    {
      this.Model = fachstundenanzahl ?? throw new ArgumentNullException(nameof(fachstundenanzahl));
    }

    /// <summary>
    /// Holt den underlying Fachstundenanzahl this ViewModel is based on
    /// </summary>
    public Fachstundenanzahl Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Stundenzahl
    /// </summary>
    public int FachstundenanzahlStundenzahl
    {
      get
      {
        return this.Model.Stundenzahl;
      }

      set
      {
        if (value == this.Model.Stundenzahl) return;
        this.UndoablePropertyChanging(this, nameof(FachstundenanzahlStundenzahl), this.Model.Stundenzahl, value);
        this.Model.Stundenzahl = value;
        this.RaisePropertyChanged("FachstundenanzahlStundenzahl");
      }
    }

    /// <summary>
    /// Holt oder setzt die Teilungsstundenzahl
    /// </summary>
    public int FachstundenanzahlTeilungsstundenzahl
    {
      get
      {
        return this.Model.Teilungsstundenzahl;
      }

      set
      {
        if (value == this.Model.Teilungsstundenzahl) return;
        this.UndoablePropertyChanging(this, nameof(FachstundenanzahlTeilungsstundenzahl), this.Model.Teilungsstundenzahl, value);
        this.Model.Teilungsstundenzahl = value;
        this.RaisePropertyChanged("FachstundenanzahlTeilungsstundenzahl");
      }
    }

    /// <summary>
    /// Holt oder setzt die klassenstufeViewModel currently assigned to this Stundenentwurf
    /// </summary>
    public int FachstundenanzahlJahrgang
    {
      get
      {
        return this.Model.Jahrgang;
      }

      set
      {
        if (value == this.Model.Jahrgang) return;
        this.UndoablePropertyChanging(this, nameof(FachstundenanzahlJahrgang), this.Model.Jahrgang, value);
        this.Model.Jahrgang = value;
        this.RaisePropertyChanged("FachstundenanzahlJahrgang");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public FachViewModel FachstundenanzahlFach
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
        if (value.FachBezeichnung == this.fach.FachBezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(FachstundenanzahlFach), this.fach, value);
        this.fach = value;
        this.Model.Fach = value.Model;
        this.RaisePropertyChanged("FachstundenanzahlFach");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.FachstundenanzahlFach.FachBezeichnung + " " + this.FachstundenanzahlJahrgang;
    }
  }
}
