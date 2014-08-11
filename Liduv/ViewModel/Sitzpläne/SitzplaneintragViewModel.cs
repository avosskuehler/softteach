namespace Liduv.ViewModel.Sitzpläne
{
  using System;
  using System.Linq;

  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Noten;

  /// <summary>
  /// ViewModel of an individual sitzplaneintrag
  /// </summary>
  public class SitzplaneintragViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Der Sitzplan der zu diesem Sitzplaneintrag gehört
    /// </summary>
    private SitzplanViewModel sitzplan;

    /// <summary>
    /// Der Sitzplatz der zu diesem Sitzplaneintrag gehört
    /// </summary>
    private SitzplatzViewModel sitzplatz;
    
    /// <summary>
    /// Der Schülereintrag der zu diesem Sitzplaneintrag gehört
    /// </summary>
    private SchülereintragViewModel schülereintrag;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplaneintragViewModel"/> Klasse. 
    /// </summary>
    /// <param name="sitzplaneintrag">
    /// The underlying sitzplaneintrag this ViewModel is to be based on
    /// </param>
    public SitzplaneintragViewModel(Sitzplaneintrag sitzplaneintrag)
    {
      if (sitzplaneintrag == null)
      {
        throw new ArgumentNullException("sitzplaneintrag");
      }

      this.Model = sitzplaneintrag;
    }

    /// <summary>
    /// Holt den underlying Sitzplaneintrag this ViewModel is based on
    /// </summary>
    public Sitzplaneintrag Model { get; private set; }

    /// <summary>
    /// Holt oder setzt den Sitzplan für den Sitzplanplan
    /// </summary>
    public SitzplanViewModel SitzplaneintragSitzplan
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Sitzplan == null)
        {
          return null;
        }

        if (this.sitzplan == null || this.sitzplan.Model != this.Model.Sitzplan)
        {
          this.sitzplan = App.MainViewModel.Sitzpläne.SingleOrDefault(d => d.Model == this.Model.Sitzplan);
        }

        return this.sitzplan;
      }

      set
      {
        if (value == null) return;
        if (this.sitzplan != null)
        {
          if (value.Model.Id == this.sitzplan.Model.Id) return;
        }

        this.UndoablePropertyChanging(this, "SitzplaneintragSitzplan", this.sitzplan, value);
        this.sitzplan = value;
        this.Model.Sitzplan = value.Model;
        this.RaisePropertyChanged("SitzplaneintragSitzplan");
      }
    }

    /// <summary>
    /// Holt oder setzt den Schülereintrag für den Schülereintragplan
    /// </summary>
    public SchülereintragViewModel SitzplaneintragSchülereintrag
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Schülereintrag == null)
        {
          return null;
        }

        if (this.schülereintrag == null || this.schülereintrag.Model != this.Model.Schülereintrag)
        {
          this.schülereintrag = App.MainViewModel.Schülereinträge.SingleOrDefault(d => d.Model == this.Model.Schülereintrag);
        }

        return this.schülereintrag;
      }

      set
      {
        if (value == null) return;
        if (this.schülereintrag != null)
        {
          if (value.SchülereintragÜberschrift == this.schülereintrag.SchülereintragÜberschrift) return;
        }

        this.UndoablePropertyChanging(this, "SitzplaneintragSchülereintrag", this.schülereintrag, value);
        this.schülereintrag = value;
        this.Model.Schülereintrag = value.Model;
        this.RaisePropertyChanged("SitzplaneintragSchülereintrag");
      }
    }

    /// <summary>
    /// Holt oder setzt den Sitzplatz für den Sitzplatzplan
    /// </summary>
    public SitzplatzViewModel SitzplaneintragSitzplatz
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Sitzplatz == null)
        {
          return null;
        }

        if (this.sitzplatz == null || this.sitzplatz.Model != this.Model.Sitzplatz)
        {
          this.sitzplatz = App.MainViewModel.Sitzplätze.SingleOrDefault(d => d.Model == this.Model.Sitzplatz);
        }

        return this.sitzplatz;
      }

      set
      {
        if (value == null) return;
        if (this.sitzplatz != null)
        {
          if (value.Bounds == this.sitzplatz.Bounds) return;
        }

        this.UndoablePropertyChanging(this, "SitzplaneintragSitzplatz", this.sitzplatz, value);
        this.sitzplatz = value;
        this.Model.Sitzplatz = value.Model;
        this.RaisePropertyChanged("SitzplaneintragSitzplatz");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Sitzplaneintrag";
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
    /// </returns>
    /// <exception cref="System.ArgumentException">Object is not a SitzplaneintragViewModel</exception>
    public int CompareTo(object obj)
    {
      var otherSitzplaneintragViewModel = obj as SitzplaneintragViewModel;
      if (otherSitzplaneintragViewModel != null)
      {
        return this.SitzplaneintragSitzplatz.Bounds == otherSitzplaneintragViewModel.SitzplaneintragSitzplatz.Bounds ? 0 : 1;
      }

      throw new ArgumentException("Object is not a SitzplaneintragViewModel");

    }
  }
}
