namespace Liduv.ViewModel.Datenbank
{
  using System;
  using System.Linq;

  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual ferien
  /// </summary>
  public class FerienViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The jahrtyp currently assigned to this ferien
    /// </summary>
    private JahrtypViewModel jahrtyp;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="FerienViewModel"/> Klasse. 
    /// </summary>
    public FerienViewModel()
    {
      var ferien = new Ferien();
      ferien.Jahrtyp = Selection.Instance.Jahrtyp.Model;
      ferien.Bezeichnung = "Neue Ferien";
      ferien.ErsterFerientag = DateTime.Now;
      ferien.LetzterFerientag = DateTime.Now;
      this.Model = ferien;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="FerienViewModel"/> Klasse. 
    /// </summary>
    /// <param name="ferien">
    /// The underlying ferien this ViewModel is to be based on
    /// </param>
    public FerienViewModel(Ferien ferien)
    {
      if (ferien == null)
      {
        throw new ArgumentNullException("ferien");
      }

      this.Model = ferien;
    }

    /// <summary>
    /// Holt den underlying Ferien this ViewModel is based on
    /// </summary>
    public Ferien Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string FerienBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "FerienBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("FerienBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die ErsterFerientag
    /// </summary>
    public DateTime FerienErsterFerientag
    {
      get
      {
        return this.Model.ErsterFerientag;
      }

      set
      {
        if (value == this.Model.ErsterFerientag) return;
        this.UndoablePropertyChanging(this, "FerienErsterFerientag", this.Model.ErsterFerientag, value);
        this.Model.ErsterFerientag = value;
        this.RaisePropertyChanged("FerienErsterFerientag");
      }
    }

    /// <summary>
    /// Holt oder setzt die LetzterFerientag
    /// </summary>
    public DateTime FerienLetzterFerientag
    {
      get
      {
        return this.Model.LetzterFerientag;
      }

      set
      {
        if (value == this.Model.LetzterFerientag) return;
        this.UndoablePropertyChanging(this, "FerienLetzterFerientag", this.Model.LetzterFerientag, value);
        this.Model.LetzterFerientag = value;
        this.RaisePropertyChanged("FerienLetzterFerientag");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public JahrtypViewModel FerienJahrtyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Jahrtyp == null)
        {
          return null;
        }

        if (this.jahrtyp == null || this.jahrtyp.Model != this.Model.Jahrtyp)
        {
          this.jahrtyp = App.MainViewModel.Jahrtypen.SingleOrDefault(d => d.Model == this.Model.Jahrtyp);
        }

        return this.jahrtyp;
      }

      set
      {
        if (this.jahrtyp != null)
        {
          if (value.JahrtypBezeichnung == this.jahrtyp.JahrtypBezeichnung) return;
        }

        this.UndoablePropertyChanging(this, "FerienJahrtyp", this.jahrtyp, value);
        this.jahrtyp = value;
        this.Model.Jahrtyp = value.Model;
        this.RaisePropertyChanged("FerienJahrtyp");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.FerienBezeichnung + " " + this.FerienJahrtyp.JahrtypBezeichnung;
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
      var compareFerien = viewModel as FerienViewModel;
      if (compareFerien == null)
      {
        return -1;
      }

      return this.Model.ErsterFerientag.CompareTo(compareFerien.FerienErsterFerientag);
    }
  }
}
