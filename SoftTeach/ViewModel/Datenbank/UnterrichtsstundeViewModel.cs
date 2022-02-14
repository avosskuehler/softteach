namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Unterrichtsstunde
  /// </summary>
  public class UnterrichtsstundeViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="UnterrichtsstundeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="unterrichtsstunde">
    /// The underlying unterrichtsstunde this ViewModel is to be based on
    /// </param>
    public UnterrichtsstundeViewModel(UnterrichtsstundeNeu unterrichtsstunde)
    {
      if (unterrichtsstunde == null)
      {
        throw new ArgumentNullException("unterrichtsstunde");
      }

      this.Model = unterrichtsstunde;
    }

    /// <summary>
    /// Holt den underlying Unterrichtsstunde this ViewModel is based on
    /// </summary>
    public UnterrichtsstundeNeu Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string UnterrichtsstundeBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "UnterrichtsstundeBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("UnterrichtsstundeBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die Beginn
    /// </summary>
    public TimeSpan UnterrichtsstundeBeginn
    {
      get
      {
        return this.Model.Beginn;
      }

      set
      {
        if (value == this.Model.Beginn) return;
        this.UndoablePropertyChanging(this, "UnterrichtsstundeBeginn", this.Model.Beginn, value);
        this.Model.Beginn = value;
        this.RaisePropertyChanged("UnterrichtsstundeBeginn");
      }
    }

    /// <summary>
    /// Holt oder setzt die Ende
    /// </summary>
    public TimeSpan UnterrichtsstundeEnde
    {
      get
      {
        return this.Model.Ende;
      }

      set
      {
        if (value == this.Model.Ende) return;
        this.UndoablePropertyChanging(this, "UnterrichtsstundeEnde", this.Model.Ende, value);
        this.Model.Ende = value;
        this.RaisePropertyChanged("UnterrichtsstundeEnde");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stundenindex
    /// </summary>
    public int UnterrichtsstundeIndex
    {
      get
      {
        return this.Model.Stundenindex;
      }

      set
      {
        if (value == this.Model.Stundenindex) return;
        this.UndoablePropertyChanging(this, "UnterrichtsstundeIndex", this.Model.Stundenindex, value);
        this.Model.Stundenindex = value;
        this.RaisePropertyChanged("UnterrichtsstundeIndex");
      }
    }

    /// <summary>
    /// Holt den Unterrichtszeitraum
    /// </summary>
    [DependsUpon("UnterrichtsstundeBeginn")]
    [DependsUpon("UnterrichtsstundeEnde")]
    public string UnterrichtsstundeZeitraum
    {
      get
      {
        return this.UnterrichtsstundeBeginn.ToString(@"hh\:mm") + "-" + this.UnterrichtsstundeEnde.ToString(@"hh\:mm");
      }
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
      var compareStunde = viewModel as UnterrichtsstundeViewModel;
      if (compareStunde == null)
      {
        return -1;
      }

      return this.Model.Stundenindex.CompareTo(compareStunde.UnterrichtsstundeIndex);
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Unterrichtsstunde: " + this.UnterrichtsstundeBezeichnung;
    }
  }
}
