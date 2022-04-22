namespace SoftTeach.ViewModel.Sitzpläne
{
  using System;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual sitzplaneintrag
  /// </summary>
  public class SitzplaneintragViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Das Rechteck, das den Sitzplatz repräsentiert
    /// </summary>
    private Border shape;

    /// <summary>
    /// Der Sitzplan der zu diesem Sitzplaneintrag gehört
    /// </summary>
    private SitzplanViewModel sitzplan;

    /// <summary>
    /// Der Sitzplatz der zu diesem Sitzplaneintrag gehört
    /// </summary>
    private SitzplatzViewModel sitzplatz;

    ///// <summary>
    ///// Der Schülereintrag der zu diesem Sitzplaneintrag gehört
    ///// </summary>
    //private SchülereintragNeu schülereintrag;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplaneintragViewModel"/> Klasse. 
    /// </summary>
    /// <param name="sitzplaneintrag">
    /// The underlying sitzplaneintrag this ViewModel is to be based on
    /// </param>
    public SitzplaneintragViewModel(SitzplaneintragNeu sitzplaneintrag)
    {
      if (sitzplaneintrag == null)
      {
        throw new ArgumentNullException("sitzplaneintrag");
      }

      this.Model = sitzplaneintrag;

      this.CreateShape();
    }

    /// <summary>
    /// Holt den underlying Sitzplaneintrag this ViewModel is based on
    /// </summary>
    public SitzplaneintragNeu Model { get; private set; }

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
    /// Holt den Namen für den Ausdruck des Sitzplans, das ist in der Regel
    /// der Vorname, bei mehreren Schülern mit gleichem Vornamen wird der erste
    /// Buchstabe des Nachnamens ergänzt.
    /// </summary>
    [DependsUpon("SitzplaneintragSchülereintrag")]
    public string SitzplanSchülername
    {
      get
      {
        if (this.Model.Schülereintrag == null)
        {
          return string.Empty;
        }

        if (this.SitzplaneintragSitzplan.UsedSchülereinträge == null)
        {
          return this.Model.Schülereintrag.Person.Vorname;
        }

        var anzahlSchülerMitGleichemVornamen = this.SitzplaneintragSitzplan.UsedSchülereinträge.Count(o => o.Person.Vorname == this.Model.Schülereintrag.Person.Vorname);
        if (anzahlSchülerMitGleichemVornamen > 1)
        {
          return string.Format("{0} {1}.", this.Model.Schülereintrag.Person.Vorname, this.Model.Schülereintrag.Person.Nachname.First());
        }

        return this.Model.Schülereintrag.Person.Vorname;
      }
    }

    /// <summary>
    /// Holt oder setzt den Schülereintrag für den Schülereintragplan
    /// </summary>
    public SchülereintragNeu SitzplaneintragSchülereintrag
    {
      get
      {
        //// We need to reflect any changes made in the model so we check the current value before returning
        //if (this.Model.Schülereintrag == null)
        //{
        //  return null;
        //}

        //if (this.schülereintrag == null || this.schülereintrag.Model != this.Model.Schülereintrag)
        //{
        //  this.schülereintrag = new SchülereintragViewModel(this.Model.Schülereintrag);
        //}

        //return this.schülereintrag;
        return this.Model.Schülereintrag;
      }

      set
      {
        //if (this.schülereintrag != null && value != null)
        //{
        //  if (value.SchülereintragÜberschrift == this.schülereintrag.SchülereintragÜberschrift) return;
        //}

        //if (this.schülereintrag == value) return;

        //this.UndoablePropertyChanging(this, "SitzplaneintragSchülereintrag", this.schülereintrag, value);
        //this.schülereintrag = value;
        this.Model.Schülereintrag = value != null ? value : null;
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
          this.sitzplatz = new SitzplatzViewModel(this.Model.Sitzplatz);
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
    /// Holt das Umfassungrechteck für den Sitzplaneintrag als Shape
    /// </summary>
    [DependsUpon("SitzplaneintragSitzplatz")]
    public Border Shape
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

    /// <summary>
    /// Creates the shape.
    /// </summary>
    private void CreateShape()
    {
      if (this.shape == null)
      {
        this.shape = new Border();
        var fillColor = new SolidColorBrush(Colors.DarkSeaGreen) { Opacity = 0.25 };
        this.shape.Background = fillColor;
        this.shape.CornerRadius = new CornerRadius(5);
        this.shape.Width = this.SitzplaneintragSitzplatz.SitzplatzBreite;
        this.shape.Height = this.SitzplaneintragSitzplatz.SitzplatzHöhe;
        this.shape.RenderTransformOrigin = new Point(0.5, 0.5);
        this.shape.RenderTransform = new RotateTransform(this.SitzplaneintragSitzplatz.SitzplatzDrehwinkel);
        this.shape.Tag = this;
        Canvas.SetTop(this.shape, this.SitzplaneintragSitzplatz.SitzplatzLinksObenY);
        Canvas.SetLeft(this.shape, this.SitzplaneintragSitzplatz.SitzplatzLinksObenX);
        var shapeLabel = new Label();
        if (this.SitzplaneintragSchülereintrag != null)
        {
          shapeLabel.Content = this.SitzplaneintragSchülereintrag.Person.Vorname;
        }
        else
        {
          shapeLabel.Content = "Platz";
        }

        this.shape.Child = shapeLabel;
        this.shape.Tag = this;
      }
    }

  }
}
