namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Linq;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual prozentbereich
  /// </summary>
  public class ProzentbereichViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The zensur currently assigned to this Prozentbereich
    /// </summary>
    private ZensurViewModel zensur;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ProzentbereichViewModel"/> Klasse. 
    /// </summary>
    /// <param name="prozentbereich">
    /// The underlying prozentbereich this ViewModel is to be based on
    /// </param>
    public ProzentbereichViewModel(ProzentbereichNeu prozentbereich)
    {
      if (prozentbereich == null)
      {
        throw new ArgumentNullException("prozentbereich");
      }

      this.Model = prozentbereich;
    }

    /// <summary>
    /// Holt das zugrundeliegende Modell des Prozentbereichs.
    /// </summary>
    public ProzentbereichNeu Model { get; private set; }

    /// <summary>
    /// Holt oder setzt den Prozentbereich ab dem die Zensur erteilt wird
    /// </summary>
    public double ProzentbereichVonProzent
    {
      get
      {
        return this.Model.VonProzent;
      }

      set
      {
        if (value == this.Model.VonProzent) return;
        this.UndoablePropertyChanging(this, "Prozentbereich", this.Model.VonProzent, value);
        this.Model.VonProzent = value;
        this.RaisePropertyChanged("Prozentbereich");
      }
    }


    /// <summary>
    /// Holt oder setzt den Prozentbereich bis zu dem die Zensur erteilt wird
    /// </summary>
    public double ProzentbereichBisProzent
    {
      get
      {
        return this.Model.BisProzent;
      }

      set
      {
        if (value == this.Model.BisProzent) return;
        this.UndoablePropertyChanging(this, "ProzentbereichBisProzent", this.Model.BisProzent, value);
        this.Model.BisProzent = value;
        this.RaisePropertyChanged("ProzentbereichBisProzent");
      }
    }

    /// <summary>
    /// Holt oder setzt die Zensur für diesen Prozentbereich
    /// </summary>
    public ZensurViewModel ProzentbereichZensur
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Zensur == null)
        {
          return null;
        }

        if (this.zensur == null || this.zensur.Model != this.Model.Zensur)
        {
          this.zensur = App.MainViewModel.Zensuren.SingleOrDefault(d => d.Model == this.Model.Zensur);
        }

        return this.zensur;
      }

      set
      {
        if (value.ZensurNotenpunkte == this.zensur.ZensurNotenpunkte) return;
        this.UndoablePropertyChanging(this, "ProzentbereichZensur", this.zensur, value);
        this.zensur = value;
        this.Model.Zensur = value.Model;
        this.RaisePropertyChanged("ProzentbereichZensur");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Prozentbereich: Note " + this.ProzentbereichZensur + " von " + this.ProzentbereichVonProzent
        + "% bis " + this.ProzentbereichBisProzent + "%";
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
      var compareProzentbereich = viewModel as ProzentbereichViewModel;
      if (compareProzentbereich == null)
      {
        return -1;
      }

      return this.Model.VonProzent.CompareTo(compareProzentbereich.ProzentbereichVonProzent);
    }
  }
}
