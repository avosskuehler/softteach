namespace Liduv.View.Personen
{
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

  using Liduv.ViewModel.Noten;
  using Liduv.ViewModel.Personen;

  using OxyPlot.Wpf;

  /// <summary>
  /// Interaction logic for MetroGruppenPage.xaml
  /// </summary>
  public partial class MetroGruppenPage
  {
    /// <summary>
    /// Die Entfernung in Pixeln, die mindestens mit dem Finger "gewischt" werden muss
    /// um die Person "in die Hand" zu nehmen
    /// </summary>
    private const float MinDragDistance = 0f;

    /// <summary>
    /// Der erste Berührpunkt
    /// </summary>
    private Point touchPoint;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroGruppenPage"/> Klasse.
    /// </summary>
    public MetroGruppenPage()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Handles the OnTouchDown event of the Border control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="TouchEventArgs"/> instance containing the event data.</param>
    private void Person_OnTouchDown(object sender, TouchEventArgs e)
    {
      this.touchPoint = e.GetTouchPoint(this).Position;
    }

    /// <summary>
    /// Handles the TouchMove event of the Note control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Input.TouchEventArgs"/> instance containing the event data.</param>
    private void Person_TouchMove(object sender, TouchEventArgs e)
    {
      var border = e.OriginalSource as Border;
      if (border != null)
      {
        var points = e.TouchDevice.GetTouchPoint(this);
        var distance = this.touchPoint.DistanceTo(points.Position);
        if (distance > MinDragDistance)
        {
          //border.RenderTransform = new TranslateTransform(points.Position.X - this.touchPoint.X, points.Position.Y - this.touchPoint.Y);

          var schülereintragViewModel = border.DataContext as SchülereintragViewModel;
          if (schülereintragViewModel != null)
          {
            DragDrop.DoDragDrop(this, schülereintragViewModel, DragDropEffects.Move);
          }
        }

      }
    }

    /// <summary>
    /// Handles the OnTouchUp event of the Border control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="TouchEventArgs"/> instance containing the event data.</param>
    private void Person_OnTouchUp(object sender, TouchEventArgs e)
    {
      var border = e.OriginalSource as Border;
      if (border != null)
      {
        border.RenderTransform = Transform.Identity;
      }
    }

    private void Group_OnDrop(object sender, DragEventArgs e)
    {
      var person = e.Data;
      var personViewModel = person.GetData(typeof(SchülereintragViewModel));
      var newGroup = e.Source;
    }

    private void Group_OnDragOver(object sender, DragEventArgs e)
    {
      var sourceGroup = e.Source as ListBox;
      var sourceList = (KeyValuePair<string, IEnumerable<SchülereintragViewModel>>)sourceGroup.DataContext;
      var targetGroup = sender as ListBox;
      var targetList = (KeyValuePair<string, IEnumerable<SchülereintragViewModel>>)targetGroup.DataContext;
      if (sourceList.Key == targetList.Key)
      {
        //e.Effects = DragDropEffects.None;
      }
      else
      {
        e.Effects = DragDropEffects.Move;
      }

    }
  }
}
