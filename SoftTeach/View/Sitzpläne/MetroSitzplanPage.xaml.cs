namespace SoftTeach.View.Sitzpläne
{
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Shapes;

  using SoftTeach.ViewModel.Sitzpläne;

  /// <summary>
  /// Interaction logic for MetroSitzplänePage.xaml
  /// </summary>
  public partial class MetroSitzplanPage
  {
    ///// <summary>
    ///// Die Position den Mausklicks
    ///// </summary>
    //private Point mouseDownPoint;

    ///// <summary>
    ///// Der Abstand zwischen linker oberer Ecke und Klickort
    ///// </summary>
    //private Point topLeftOffset;

    ///// <summary>
    ///// Der momentan bearbeitete Sitzplaneintrag
    ///// </summary>
    //private SitzplaneintragViewModel currentSitzplaneintrag;

    ///// <summary>
    ///// Gibt an, ob ein Sitzplaneintrag verschoben wird
    ///// </summary>
    //private bool isMovingSitzplaneintrag;

    ///// <summary>
    ///// Gibt an, dass der Sitzplaneintrag auch kopiert werden kann
    ///// </summary>
    //private bool canCopySitzplaneintrag;

    ///// <summary>
    ///// Die Liste der Sitzplanrechtecke
    ///// </summary>
    //private List<Rectangle> sitzplatzShapes;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="MetroSitzplanPage"/> Klasse.
    /// </summary>
    public MetroSitzplanPage()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Handles the OnMouseLeftButtonDown event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var sitzplan = this.DataContext as SitzplanViewModel;
      if (sitzplan == null)
      {
        return;
      }

      //var clickLocation = e.GetPosition(this.SitzplanCanvas);
      //this.currentSitzplaneintrag = this.GetSitzplaneintragUnderCursor(clickLocation);
      //this.mouseDownPoint = clickLocation;
      //if (this.currentSitzplaneintrag == null)
      //{
      //  sitzplan.AddSitzplaneintrag(clickLocation.X, clickLocation.Y, 0, 0);
      //  this.SitzplanCanvas.Children.Add(sitzplan.CurrentSitzplaneintrag.Shape);
      //  this.currentSitzplaneintrag = sitzplan.CurrentSitzplaneintrag;
      //  this.isMovingSitzplaneintrag = false;
      //}
      //else
      //{
      //  this.SitzplanCanvas.Cursor = Cursors.SizeAll;
      //  this.isMovingSitzplaneintrag = true;
      //  this.topLeftOffset = new Point(
      //    clickLocation.X - Canvas.GetLeft(this.currentSitzplaneintrag.Shape),
      //    clickLocation.Y - Canvas.GetTop(this.currentSitzplaneintrag.Shape));
      //  this.canCopySitzplaneintrag = true;
      //}
    }

    /// <summary>
    /// Handles the OnMouseRightButtonDown event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void Canvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      var sitzplan = this.DataContext as SitzplanViewModel;
      if (sitzplan == null)
      {
        return;
      }

      //var clickLocation = e.GetPosition(this.SitzplanCanvas);
      //this.currentSitzplaneintrag = this.GetSitzplaneintragUnderCursor(clickLocation);
      //if (this.currentSitzplaneintrag != null)
      //{
      //  sitzplan.DeleteSitzplaneintrag(this.currentSitzplaneintrag);
      //  this.SitzplanCanvas.Children.Remove(this.currentSitzplaneintrag.Shape);
      //  this.currentSitzplaneintrag = null;
      //  this.isMovingSitzplaneintrag = false;
      //  this.canCopySitzplaneintrag = false;
      //}
    }

    /// <summary>
    /// Handles the OnMouseMove event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
    {
      var sitzplan = this.DataContext as SitzplanViewModel;
      if (sitzplan == null)
      {
        return;
      }

      //var moveLocation = e.GetPosition(this.SitzplanCanvas);
      //if (e.LeftButton == MouseButtonState.Pressed)
      //{
      //  if (this.isMovingSitzplaneintrag)
      //  {
      //    if ((Keyboard.Modifiers & ModifierKeys.Control) > 0 && this.canCopySitzplaneintrag)
      //    {
      //      Canvas.SetTop(this.currentSitzplaneintrag.Shape, this.mouseDownPoint.Y - this.topLeftOffset.Y);
      //      Canvas.SetLeft(this.currentSitzplaneintrag.Shape, this.mouseDownPoint.X - this.topLeftOffset.X);
      //      sitzplan.AddSitzplaneintrag(moveLocation.X, moveLocation.Y, this.currentSitzplaneintrag.Shape.Width, this.currentSitzplaneintrag.Shape.Height);
      //      this.SitzplanCanvas.Children.Add(this.Sitzplan.CurrentSitzplaneintrag.Shape);
      //      this.currentSitzplaneintrag = this.Sitzplan.CurrentSitzplaneintrag;
      //      this.isMovingSitzplaneintrag = true;
      //      this.canCopySitzplaneintrag = false;
      //    }
      //    else
      //    {
      //      Canvas.SetLeft(this.currentSitzplaneintrag.Shape, moveLocation.X - this.topLeftOffset.X);
      //      Canvas.SetTop(this.currentSitzplaneintrag.Shape, moveLocation.Y - this.topLeftOffset.Y);
      //    }
      //  }
      //  else
      //  {
      //    if (this.currentSitzplaneintrag != null)
      //    {
      //      this.currentSitzplaneintrag.Shape.Width = System.Math.Abs(moveLocation.X - this.mouseDownPoint.X);
      //      this.currentSitzplaneintrag.Shape.Height = System.Math.Abs(moveLocation.Y - this.mouseDownPoint.Y);
      //    }
      //  }
      //}
      //else
      //{
      //  if (this.GetSitzplaneintragUnderCursor(moveLocation) != null)
      //  {
      //    this.SitzplanCanvas.Cursor = Cursors.Hand;
      //  }
      //  else
      //  {
      //    this.SitzplanCanvas.Cursor = Cursors.Arrow;
      //  }
      //}
    }

    /// <summary>
    /// Handles the OnMouseLeftButtonUp event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      var sitzplan = this.DataContext as SitzplanViewModel;
      if (sitzplan == null)
      {
        return;
      }

      //if (this.currentSitzplaneintrag == null)
      //{
      //  return;
      //}

      //var upLocation = e.GetPosition(this.SitzplanCanvas);
      //if (this.isMovingSitzplaneintrag)
      //{
      //  Canvas.SetLeft(this.currentSitzplaneintrag.Shape, upLocation.X - this.topLeftOffset.X);
      //  Canvas.SetTop(this.currentSitzplaneintrag.Shape, upLocation.Y - this.topLeftOffset.Y);
      //}
      //else
      //{
      //  this.currentSitzplaneintrag.Shape.Width = System.Math.Abs(upLocation.X - this.mouseDownPoint.X);
      //  this.currentSitzplaneintrag.Shape.Height = System.Math.Abs(upLocation.Y - this.mouseDownPoint.Y);
      //}
    }

    /// <summary>
    /// Gets the sitzplatz under cursor.
    /// </summary>
    /// <param name="mouseLocation">The mouse location.</param>
    /// <returns>Das <see cref="SitzplaneintragViewModel"/> oder null, wenn kein Sitzplaneintrag unter der Maus.</returns>
    private static SitzplaneintragViewModel GetSitzplaneintragUnderCursor(Point mouseLocation)
    {
      //var result = VisualTreeHelper.HitTest(this.SitzplanCanvas, mouseLocation);

      //if (result == null)
      //{
      //  return null;
      //}

      //var rect = result.VisualHit as Border;
      //if (rect == null)
      //{
      //  return null;
      //}

      //var viewModel = rect.Tag as SitzplaneintragViewModel;
      //return viewModel;
      return null;
    }

    /// <summary>
    /// Adds the sitzplaneintrag shapes.
    /// </summary>
    private static void AddSitzplaneintragShapes()
    {
      //var sitzplan = this.DataContext as SitzplanViewModel;
      //if (sitzplan == null)
      //{
      //  return;
      //}

      //foreach (var sitzplaneintragViewModel in sitzplan.Sitzplaneinträge)
      //{
      //  this.SitzplanCanvas.Children.Add(sitzplaneintragViewModel.Shape);
      //}
    }

    /// <summary>
    /// Removes the sitzplatz shapes.
    /// </summary>
    private void RemoveSitzplaneintragShapes()
    {
      var sitzplan = this.DataContext as SitzplanViewModel;
      if (sitzplan == null)
      {
        return;
      }

      //foreach (var sitzplaneintragViewModel in sitzplan.Sitzplaneinträge)
      //{
      //  this.SitzplanCanvas.Children.Remove(sitzplaneintragViewModel.Shape);
      //}
    }

    private void MetroSitzplanPage_OnLoaded(object sender, RoutedEventArgs e)
    {
      //this.AddSitzplaneintragShapes();
    }
  }
}
