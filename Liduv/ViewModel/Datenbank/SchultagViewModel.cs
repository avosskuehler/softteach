namespace Liduv.ViewModel.Datenbank
{
  using System;
  using System.Linq;

  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Schultag
  /// </summary>
  public class SchultagViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The termintyp currently assigned to this SchultagViewModel
    /// </summary>
    private TermintypViewModel termintyp;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchultagViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schultag">
    /// The schultag jahrtyp this ViewModel is to be based on
    /// </param>
    public SchultagViewModel(Schultag schultag)
    {
      if (schultag == null)
      {
        throw new ArgumentNullException("schultag");
      }

      this.Model = schultag;
    }

    /// <summary>
    /// Holt den underlying Schultag this ViewModel is based on
    /// </summary>
    public Schultag Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die termintyp currently assigned to this Schultag
    /// </summary>
    public TermintypViewModel SchultagTermintyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Termintyp == null)
        {
          return null;
        }

        if (this.termintyp == null || this.termintyp.Model != this.Model.Termintyp)
        {
          this.termintyp = App.MainViewModel.Termintypen.SingleOrDefault(d => d.Model == this.Model.Termintyp);
        }

        return this.termintyp;
      }

      set
      {
        if (this.termintyp != null)
        {
          if (value.TermintypBezeichnung == this.termintyp.TermintypBezeichnung) return;
        }

        this.UndoablePropertyChanging(this, "SchultagTermintyp", this.termintyp, value);
        this.termintyp = value;
        this.Model.Termintyp = value.Model;
        this.RaisePropertyChanged("SchultagTermintyp");
      }
    }

    /// <summary>
    /// Holt oder setzt das Datum
    /// </summary>
    public DateTime SchultagDatum
    {
      get
      {
        return this.Model.Datum;
      }

      set
      {
        if (value == this.Model.Datum) return;
        this.UndoablePropertyChanging(this, "SchultagDatum", this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("SchultagDatum");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Schultag: " + this.SchultagDatum.ToShortDateString() + " Typ: " + (this.SchultagTermintyp == null ? "undefiniert" : this.SchultagTermintyp.TermintypBezeichnung);
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
      var compareSchultag = viewModel as SchultagViewModel;
      if (compareSchultag == null)
      {
        return -1;
      }

      return this.Model.Datum.CompareTo(compareSchultag.SchultagDatum);
    }
  }
}
