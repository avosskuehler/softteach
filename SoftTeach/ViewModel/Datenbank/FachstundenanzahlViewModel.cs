namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Linq;

  using SoftTeach.Model;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Fachstundenanzahl
  /// </summary>
  public class FachstundenanzahlViewModel : ViewModelBase
  {
    /// <summary>
    /// The klassenstufe currently assigned to this FachstundenanzahlViewModel
    /// </summary>
    private KlassenstufeViewModel klassenstufe;

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
      if (fachstundenanzahl == null)
      {
        throw new ArgumentNullException("fachstundenanzahl");
      }

      this.Model = fachstundenanzahl;
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
        this.UndoablePropertyChanging(this, "FachstundenanzahlStundenzahl", this.Model.Stundenzahl, value);
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
        this.UndoablePropertyChanging(this, "FachstundenanzahlTeilungsstundenzahl", this.Model.Teilungsstundenzahl, value);
        this.Model.Teilungsstundenzahl = value;
        this.RaisePropertyChanged("FachstundenanzahlTeilungsstundenzahl");
      }
    }

    /// <summary>
    /// Holt oder setzt die klassenstufeViewModel currently assigned to this Stundenentwurf
    /// </summary>
    public KlassenstufeViewModel FachstundenanzahlKlassenstufe
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Klassenstufe == null)
        {
          return null;
        }

        if (this.klassenstufe == null || this.klassenstufe.Model != this.Model.Klassenstufe)
        {
          this.klassenstufe = App.MainViewModel.Klassenstufen.SingleOrDefault(d => d.Model == this.Model.Klassenstufe);
        }

        return this.klassenstufe;
      }

      set
      {
        if (value.KlassenstufeBezeichnung == this.klassenstufe.KlassenstufeBezeichnung) return;
        this.UndoablePropertyChanging(this, "FachstundenanzahlKlassenstufe", this.klassenstufe, value);
        this.klassenstufe = value;
        this.Model.Klassenstufe = value.Model;
        this.RaisePropertyChanged("FachstundenanzahlKlassenstufe");
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

        if (this.klassenstufe == null || this.fach.Model != this.Model.Fach)
        {
          this.fach = App.MainViewModel.Fächer.SingleOrDefault(d => d.Model == this.Model.Fach);
        }

        return this.fach;
      }

      set
      {
        if (value.FachBezeichnung == this.fach.FachBezeichnung) return;
        this.UndoablePropertyChanging(this, "FachstundenanzahlFach", this.fach, value);
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
      return this.FachstundenanzahlFach.FachBezeichnung + " " + this.FachstundenanzahlKlassenstufe.KlassenstufeBezeichnung;
    }
  }
}
