namespace Liduv.ViewModel.Sitzpläne
{
  using System;
  using System.Windows;

  using Liduv.Model.EntityFramework;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual sitzplatz
  /// </summary>
  public class SitzplatzViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplatzViewModel"/> Klasse. 
    /// </summary>
    /// <param name="sitzplatz">
    /// The underlying sitzplatz this ViewModel is to be based on
    /// </param>
    public SitzplatzViewModel(Sitzplatz sitzplatz)
    {
      if (sitzplatz == null)
      {
        throw new ArgumentNullException("sitzplatz");
      }

      this.Model = sitzplatz;
    }

    /// <summary>
    /// Holt den underlying Sitzplatz this ViewModel is based on
    /// </summary>
    public Sitzplatz Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die x Position des linken oberern Sitzplatzrechtecks.
    /// </summary>
    public double SitzplatzLinksObenX
    {
      get
      {
        return this.Model.LinksObenX;
      }

      set
      {
        if (value == this.Model.LinksObenX) return;
        this.UndoablePropertyChanging(this, "SitzplatzLinksObenX", this.Model.LinksObenX, value);
        this.Model.LinksObenX = value;
        this.RaisePropertyChanged("SitzplatzLinksObenX");
      }
    }

    /// <summary>
    /// Holt oder setzt die y Position des linken oberern Sitzplatzrechtecks.
    /// </summary>
    public double SitzplatzLinksObenY
    {
      get
      {
        return this.Model.LinksObenY;
      }

      set
      {
        if (value == this.Model.LinksObenY) return;
        this.UndoablePropertyChanging(this, "SitzplatzLinksObenY", this.Model.LinksObenY, value);
        this.Model.LinksObenY = value;
        this.RaisePropertyChanged("SitzplatzLinksObenY");
      }
    }

    /// <summary>
    /// Holt oder setzt Breite des Sitzplatzrechtecks.
    /// </summary>
    public double SitzplatzBreite
    {
      get
      {
        return this.Model.Breite;
      }

      set
      {
        if (value == this.Model.Breite) return;
        this.UndoablePropertyChanging(this, "SitzplatzBreite", this.Model.Breite, value);
        this.Model.Breite = value;
        this.RaisePropertyChanged("SitzplatzBreite");
      }
    }

    /// <summary>
    /// Holt oder setzt die Höhe des  Sitzplatzrechtecks.
    /// </summary>
    public double SitzplatzHöhe
    {
      get
      {
        return this.Model.Höhe;
      }

      set
      {
        if (value == this.Model.Höhe) return;
        this.UndoablePropertyChanging(this, "SitzplatzHöhe", this.Model.Höhe, value);
        this.Model.Höhe = value;
        this.RaisePropertyChanged("SitzplatzHöhe");
      }
    }

    /// <summary>
    /// Holt das Umfassungrechteck für den Sitzplatz.
    /// </summary>
    public Rect Bounds
    {
      get
      {
        return new Rect(this.SitzplatzLinksObenX, this.SitzplatzLinksObenY, this.SitzplatzBreite, this.SitzplatzHöhe);
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Sitzplatz";
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
    /// </returns>
    /// <exception cref="System.ArgumentException">Object is not a SitzplatzViewModel</exception>
    public int CompareTo(object obj)
    {
      var otherSitzplatzViewModel = obj as SitzplatzViewModel;
      if (otherSitzplatzViewModel != null)
      {
        return this.Bounds == otherSitzplatzViewModel.Bounds ? 0 : 1;
      }

      throw new ArgumentException("Object is not a SitzplatzViewModel");

    }
  }
}
