namespace SoftTeach.View.Sitzpläne
{
  using System;
  using System.Windows;
  using System.Windows.Documents;
  using System.Windows.Media;
  using System.Windows.Media.Animation;
  using System.Windows.Shapes;

  /// <summary>
  /// Ein Platzhalter für zu kopierende oder zu verschiebende Sitzplätze
  /// </summary>
  public class SitzplatzAdorner : Adorner
  {
    /// <summary>
    /// Der Adornerplatzhalter
    /// </summary>
    private Rectangle child;

    /// <summary>
    /// Offset links
    /// </summary>
    private double leftOffset;

    /// <summary>
    /// Offset rechts
    /// </summary>
    private double topOffset;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SitzplatzAdorner"/> Klasse.
    /// </summary>
    /// <param name="adornedElement"> The adorned element.</param>
    public SitzplatzAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
      var brush = new VisualBrush(adornedElement);

      this.child = new Rectangle();
      this.child.Width = adornedElement.RenderSize.Width;
      this.child.Height = adornedElement.RenderSize.Height;


      var animation = new DoubleAnimation(0.3, 1, new Duration(TimeSpan.FromSeconds(1)));
      animation.AutoReverse = true;
      animation.RepeatBehavior = RepeatBehavior.Forever;
      brush.BeginAnimation(Brush.OpacityProperty, animation);

      this.child.Fill = brush;
    }

    /// <summary>
    /// When overridden in a derived class, participates in rendering operations that are directed by the 
    /// layout system. The rendering instructions for this element are not used directly when this method 
    /// is invoked, and are instead preserved for later asynchronous use by layout and drawing.
    /// A common way to implement an adorner's rendering behavior is to override the OnRender
    /// method, which is called by the layout subsystem as part of a rendering pass.
    /// </summary>
    /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
      // Get a rectangle that represents the desired size of the rendered element
      // after the rendering pass.  This will be used to draw at the corners of the 
      // adorned element.
      var adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

      // Some arbitrary drawing implements.
      var renderBrush = new SolidColorBrush(Colors.Green);
      renderBrush.Opacity = 0.2;
      var renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
      var renderRadius = 5.0;

      var rotateBrush = new SolidColorBrush(Colors.Red);
      rotateBrush.Opacity = 0.2;
      var rotatePen = new Pen(new SolidColorBrush(Colors.DarkRed), 1.5);
      var rotateRadius = 10.0;

      // Just draw a circle at each corner.
      drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
      drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
      drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
      drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
      drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
      var center = new Point(
        adornedElementRect.X + adornedElementRect.Width / 2,
        adornedElementRect.Y + adornedElementRect.Height / 2);
      drawingContext.DrawEllipse(rotateBrush, rotatePen, center, rotateRadius, rotateRadius);
    }

    /// <summary>
    /// Implements any custom measuring behavior for the adorner.
    /// </summary>
    /// <param name="constraint">A size to constrain the adorner to.</param>
    /// <returns>
    /// A <see cref="T:System.Windows.Size" /> object representing the amount of layout space needed by the adorner.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
      this.child.Measure(constraint);
      return this.child.DesiredSize;
    }

    /// <summary>
    /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class.
    /// </summary>
    /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
    /// <returns>
    /// The actual size used.
    /// </returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      this.child.Arrange(new Rect(finalSize));
      return finalSize;
    }

    /// <summary>
    /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />, and returns a child at the specified index from a collection of child elements.
    /// </summary>
    /// <param name="index">The zero-based index of the requested child element in the collection.</param>
    /// <returns>
    /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
    /// </returns>
    protected override Visual GetVisualChild(int index)
    {
      return this.child;
    }

    /// <summary>
    /// Gets the number of visual child elements within this element.
    /// </summary>
    /// <returns>The number of visual child elements for this element.</returns>
    protected override int VisualChildrenCount
    {
      get
      {
        return 1;
      }
    }

    /// <summary>
    /// Gets or sets the left offset.
    /// </summary>
    /// <value>
    /// The left offset.
    /// </value>
    public double LeftOffset
    {
      get
      {
        return this.leftOffset;
      }

      set
      {
        this.leftOffset = value;
        this.UpdatePosition();
      }
    }

    /// <summary>
    /// Gets or sets the top offset.
    /// </summary>
    /// <value>
    /// The top offset.
    /// </value>
    public double TopOffset
    {
      get
      {
        return this.topOffset;
      }

      set
      {
        this.topOffset = value;
        this.UpdatePosition();
      }
    }

    /// <summary>
    /// Returns a <see cref="T:System.Windows.Media.Transform" /> for the adorner, based on the transform that is currently applied to the adorned element.
    /// </summary>
    /// <param name="transform">The transform that is currently applied to the adorned element.</param>
    /// <returns>
    /// A transform to apply to the adorner.
    /// </returns>
    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
      GeneralTransformGroup result = new GeneralTransformGroup();
      result.Children.Add(base.GetDesiredTransform(transform));
      result.Children.Add(new TranslateTransform(this.leftOffset, this.topOffset));
      return result;
    }

    /// <summary>
    /// Updates the position.
    /// </summary>
    private void UpdatePosition()
    {
      var adornerLayer = this.Parent as AdornerLayer;
      if (adornerLayer != null)
      {
        adornerLayer.Update(this.AdornedElement);
      }
    }
  }
}