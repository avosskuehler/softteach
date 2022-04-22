using SoftTeach.ViewModel.Sitzpläne;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SoftTeach.Resources.Controls
{
  public class SitzplatzShape : Shape
  {
    public SitzplatzShape()
    {
      var fillColor = new SolidColorBrush(Colors.DarkSeaGreen) { Opacity = 0.5 };
      this.RenderTransformOrigin = new Point(0.5, 0.5);
      this.Fill = fillColor;
    }

    protected override Geometry DefiningGeometry
    {
      get { return GetGeometry(); }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);

      drawingContext.DrawText(
       new FormattedText(this.Reihenfolge.ToString(),
          CultureInfo.GetCultureInfo("de-de"),
          FlowDirection.LeftToRight,
          new Typeface("Verdana"),
          18, System.Windows.Media.Brushes.Black),
          new System.Windows.Point(5, 5));
    }

    private Geometry GetGeometry()
    {
      //var formattedText = new FormattedText(
      //          this.Reihenfolge.ToString(),
      //          CultureInfo.GetCultureInfo("de-de"),
      //          FlowDirection.LeftToRight,
      //          new Typeface("Verdana"),
      //          96,
      //          Brushes.Black);

      //Geometry geometry = formattedText.BuildGeometry(new Point(0, 0));

      //PathGeometry pathGeometry = geometry.GetFlattenedPathGeometry();
      var source = string.Format("M 0,0 L {0} , 0 L {0} , {1} L 0, {1} Z", this.Width, this.Height);
      var rechteck = Geometry.Parse(source);
      return rechteck;
      //return Geometry.Combine(geometry, rechteck, GeometryCombineMode.Union, this.LayoutTransform);
    }

    public SitzplatzViewModel Sitzplatz
    {
      get { return (SitzplatzViewModel)GetValue(SitzplatzProperty); }
      set { SetValue(SitzplatzProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TriangleOrientation.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SitzplatzProperty =
        DependencyProperty.Register("Sitzplatz", typeof(SitzplatzViewModel), typeof(SitzplatzShape), new UIPropertyMetadata(null, OnSitzplatzChanged));

    private static void OnSitzplatzChanged(DependencyObject d, DependencyPropertyChangedEventArgs ek)
    {
      var sitzplatz = (SitzplatzShape)d;
      sitzplatz.InvalidateVisual();
    }

    public int Reihenfolge
    {
      get { return (int)GetValue(ReihenfolgeProperty); }
      set { SetValue(ReihenfolgeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TriangleOrientation.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ReihenfolgeProperty =
        DependencyProperty.Register("Reihenfolge", typeof(int), typeof(SitzplatzShape), new UIPropertyMetadata(0, OnReihenfolgeChanged));

    private static void OnReihenfolgeChanged(DependencyObject d, DependencyPropertyChangedEventArgs ek)
    {
      var sitzplatz = (SitzplatzShape)d;
      sitzplatz.InvalidateVisual();
    }
  }
}
