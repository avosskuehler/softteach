namespace SoftTeach.ViewModel.Sitzpläne
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Media;
  using System.Windows.Shapes;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Resources.Controls;
  using SoftTeach.View.Sitzpläne;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual sitzplatz
  /// </summary>
  public class SitzplatzViewModel : ViewModelBase, IComparable, ISequencedObject
  {
    private SitzplatzShape shape;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="SitzplatzViewModel"/> Klasse. 
    /// </summary>
    /// <param name="sitzplatz">
    /// The underlying sitzplatz this ViewModel is to be based on
    /// </param>
    public SitzplatzViewModel(Sitzplatz sitzplatz)
    {
      this.Model = sitzplatz ?? throw new ArgumentNullException(nameof(sitzplatz));

      this.CreateShape();
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
        this.UndoablePropertyChanging(this, nameof(SitzplatzLinksObenX), this.Model.LinksObenX, value);
        this.Model.LinksObenX = value;
        Canvas.SetLeft(this.shape, this.SitzplatzLinksObenX);
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
        this.UndoablePropertyChanging(this, nameof(SitzplatzLinksObenY), this.Model.LinksObenY, value);
        this.Model.LinksObenY = value;
        Canvas.SetLeft(this.shape, this.SitzplatzLinksObenY);
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
        this.UndoablePropertyChanging(this, nameof(SitzplatzBreite), this.Model.Breite, value);
        this.Model.Breite = value;
        this.shape.Width = this.SitzplatzBreite;
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
        this.UndoablePropertyChanging(this, nameof(SitzplatzHöhe), this.Model.Höhe, value);
        this.Model.Höhe = value;
        this.shape.Height = this.SitzplatzHöhe;
        this.RaisePropertyChanged("SitzplatzHöhe");
      }
    }

    /// <summary>
    /// Holt oder setzt die Höhe des  Sitzplatzrechtecks.
    /// </summary>
    public double SitzplatzDrehwinkel
    {
      get
      {
        return this.Model.Drehwinkel;
      }

      set
      {
        if (value == this.Model.Drehwinkel) return;
        this.UndoablePropertyChanging(this, nameof(SitzplatzDrehwinkel), this.Model.Drehwinkel, value);
        this.Model.Drehwinkel = value;
        this.shape.RenderTransform = new RotateTransform(this.SitzplatzDrehwinkel);
        this.RaisePropertyChanged("SitzplatzDrehwinkel");
      }
    }

    /// <summary>
    /// Holt oder setzt die laufende Nummer des Sitzplatzes.
    /// </summary>
    public int Reihenfolge
    {
      get
      {
        return this.Model.Reihenfolge;
      }

      set
      {
        if (value == this.Model.Reihenfolge) return;
        this.UndoablePropertyChanging(this, nameof(Reihenfolge), this.Model.Reihenfolge, value);
        this.Model.Reihenfolge = value;
        this.shape.Reihenfolge = value;
        this.RaisePropertyChanged("Reihenfolge");
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
    /// Holt das Umfassungrechteck für den Sitzplatz als Shape
    /// </summary>
    [DependsUpon("SitzplatzLinksObenX")]
    [DependsUpon("SitzplatzLinksObenY")]
    [DependsUpon("SitzplatzHöhe")]
    [DependsUpon("SitzplatzBreite")]
    public SitzplatzShape Shape
    {
      get
      {
        this.CreateShape();

        return this.shape;
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

    /// <summary>
    /// Updates the model from the shapes coordinates.
    /// </summary>
    public void UpdateModelFromShape()
    {
      this.Model.LinksObenX = Canvas.GetLeft(this.shape);
      this.Model.LinksObenY = Canvas.GetTop(this.shape);
      this.Model.Breite = this.shape.Width;
      this.Model.Höhe = this.shape.Height;
      this.Model.Drehwinkel = ((RotateTransform)this.shape.RenderTransform).Angle;
    }

    /// <summary>
    /// Creates the shape.
    /// </summary>
    private void CreateShape()
    {
      if (this.shape == null)
      {
        this.shape = new SitzplatzShape
        {
          Width = this.SitzplatzBreite,
          Height = this.SitzplatzHöhe,
          RenderTransform = new RotateTransform(this.SitzplatzDrehwinkel),
          Sitzplatz = this,
          Reihenfolge = this.Model.Reihenfolge
        };
        Canvas.SetTop(this.shape, this.SitzplatzLinksObenY);
        Canvas.SetLeft(this.shape, this.SitzplatzLinksObenX);
      }
    }
  }
}
