namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Linq;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual ferien
  /// </summary>
  public class FerienViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The schuljahr currently assigned to this ferien
    /// </summary>
    private SchuljahrViewModel schuljahr;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="FerienViewModel"/> Klasse. 
    /// </summary>
    public FerienViewModel()
    {
      var ferien = new Ferien
      {
        Schuljahr = Selection.Instance.Schuljahr.Model,
        Bezeichnung = "e Ferien",
        ErsterFerientag = DateTime.Now,
        LetzterFerientag = DateTime.Now
      };
      App.UnitOfWork.Context.Ferien.Add(ferien);
      this.Model = ferien;
    }

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="FerienViewModel"/> Klasse. 
    /// </summary>
    /// <param name="ferien">
    /// The underlying ferien this ViewModel is to be based on
    /// </param>
    public FerienViewModel(Ferien ferien)
    {
      this.Model = ferien ?? throw new ArgumentNullException(nameof(ferien));
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
        this.UndoablePropertyChanging(this, nameof(FerienBezeichnung), this.Model.Bezeichnung, value);
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
        this.UndoablePropertyChanging(this, nameof(FerienErsterFerientag), this.Model.ErsterFerientag, value);
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
        this.UndoablePropertyChanging(this, nameof(FerienLetzterFerientag), this.Model.LetzterFerientag, value);
        this.Model.LetzterFerientag = value;
        this.RaisePropertyChanged("FerienLetzterFerientag");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public SchuljahrViewModel FerienSchuljahr
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Schuljahr == null)
        {
          return null;
        }

        if (this.schuljahr == null || this.schuljahr.Model != this.Model.Schuljahr)
        {
          this.schuljahr = App.MainViewModel.Schuljahre.SingleOrDefault(d => d.Model == this.Model.Schuljahr);
        }

        return this.schuljahr;
      }

      set
      {
        if (this.schuljahr != null)
        {
          if (value.SchuljahrBezeichnung == this.schuljahr.SchuljahrBezeichnung) return;
        }

        this.UndoablePropertyChanging(this, nameof(FerienSchuljahr), this.schuljahr, value);
        this.schuljahr = value;
        this.Model.Schuljahr = value.Model;
        this.RaisePropertyChanged("FerienSchuljahr");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.FerienBezeichnung + " " + this.FerienSchuljahr.SchuljahrBezeichnung;
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
