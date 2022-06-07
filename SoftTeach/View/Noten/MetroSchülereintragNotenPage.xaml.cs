namespace SoftTeach.View.Noten
{
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

  using OxyPlot.Wpf;

  using SoftTeach.ViewModel.Noten;

  /// <summary>
  /// Interaction logic for MetroSchülereintragNotenPage.xaml
  /// </summary>
  public partial class MetroSchülereintragNotenPage
  {
    /// <summary>
    /// Die Entfernung in Pixeln, die mindestens mit dem Finger "gewischt" werden muss
    /// um die Note zu löschen 
    /// </summary>
    private const float DeleteDistance = 200f;

    /// <summary>
    /// Der erste Berührpunkt
    /// </summary>
    private Point touchPoint;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="MetroSchülereintragNotenPage"/> Klasse.
    /// </summary>
    public MetroSchülereintragNotenPage(SchülereintragViewModel vm)
    {
      this.InitializeComponent();
      this.DataContext = vm;
      if (vm != null)
      {
        vm.PropertyChanged += this.SchülereintragViewModelPropertyChanged;
      }
    }

    /// <summary>
    /// Handles the PropertyChanged event of the schülereintragViewModel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void SchülereintragViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      // Die Property ist egal, in UpdateNoten werden alle aufgerufen
      // der Check ist nur damit nur einmal geupdated wird
      if (e.PropertyName == "Gesamtnote")
      {
        // Refresh the plot view
        this.QualitätPlot.InvalidatePlot();
        this.QuantitätPlot.InvalidatePlot();
      }
    }
    
    /// <summary>
    /// Handles the OnTouchDown event of the Border control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="TouchEventArgs"/> instance containing the event data.</param>
    private void Note_OnTouchDown(object sender, TouchEventArgs e)
    {
      this.touchPoint = e.GetTouchPoint(this).Position;
    }

    /// <summary>
    /// Handles the TouchMove event of the Note control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Input.TouchEventArgs"/> instance containing the event data.</param>
    private void Note_TouchMove(object sender, TouchEventArgs e)
    {
      var border = e.OriginalSource as Border;
      if (border != null)
      {
        var points = e.TouchDevice.GetTouchPoint(this);

        var distance = Point.Subtract(this.touchPoint, points.Position).Length;
        //var distance = this.touchPoint.DistanceTo(points.Position);
        if (distance > DeleteDistance)
        {
          var schülereintragViewModel = this.DataContext as SchülereintragViewModel;
          if (schülereintragViewModel != null)
          {
            // Es ist entweder eine Note, eine Hausaufgabe oder eine Notentendenz
            var noteViewModel = border.DataContext as NoteViewModel;
            schülereintragViewModel.DeleteNote(noteViewModel);
            var hausaufgabeViewModel = border.DataContext as HausaufgabeViewModel;
            schülereintragViewModel.DeleteHausaufgabe(hausaufgabeViewModel);
            var notentendenzViewModel = border.DataContext as NotentendenzViewModel;
            schülereintragViewModel.DeleteNotentendenz(notentendenzViewModel);
          }
        }

        // Create a TransformGroup to contain the transforms 
        // and add the transforms to it.
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(new ScaleTransform((DeleteDistance - distance) / DeleteDistance, (DeleteDistance - distance) / DeleteDistance));
        transformGroup.Children.Add(new TranslateTransform(points.Position.X - this.touchPoint.X, points.Position.Y - this.touchPoint.Y));
        border.RenderTransform = transformGroup;
      }
    }

    /// <summary>
    /// Handles the OnTouchUp event of the Border control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="TouchEventArgs"/> instance containing the event data.</param>
    private void Note_OnTouchUp(object sender, TouchEventArgs e)
    {
      var border = e.OriginalSource as Border;
      if (border != null)
      {
        border.RenderTransform = Transform.Identity;
      }
    }
  }
}
