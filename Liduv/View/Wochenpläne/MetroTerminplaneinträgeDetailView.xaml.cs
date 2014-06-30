
namespace Liduv.View.Wochenpläne
{
  using System;
  using System.Linq;
  using System.Windows.Input;
  using System.Windows.Media;

  using Liduv.ViewModel.Wochenpläne;

  /// <summary>
  /// Interaction logic for MetroTerminplaneinträgeDetailView.xaml
  /// </summary>
  public partial class MetroTerminplaneinträgeDetailView
  {
    private bool isCurrentlyClicked;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroTerminplaneinträgeDetailView"/> Klasse.
    /// </summary>
    public MetroTerminplaneinträgeDetailView()
    {
      this.InitializeComponent();
      WochenplanSelection.Instance.Plangrid.PreviewMouseLeftButtonDown += this.PlangridMouseLeftButtonDown;
      WochenplanSelection.Instance.Plangrid.PreviewMouseLeftButtonUp += this.PlangridMouseLeftButtonUp;
    }

    void PlangridMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (this.SurroundBorder.Background == Brushes.LightCoral && !this.isCurrentlyClicked)
      {
        this.SurroundBorder.Background = Brushes.Transparent;
      }
    }

    void PlangridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.isCurrentlyClicked = false;
    }

    private void SurroundBorderMouseEnter(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        var wochenplanEintragCollection = this.DataContext as TerminplanEintragCollection;
        if (wochenplanEintragCollection == null) return;
        if (WochenplanSelection.Instance.WochentagIndex != wochenplanEintragCollection.WochentagIndex) return;

        // Enable edit
        this.SurroundBorder.Background = Brushes.LightCoral;

        int storedFirstIndex = WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex;
        int storedLastIndex = WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex;
        var newFirstIndex = wochenplanEintragCollection.ErsteUnterrichtsstundeIndex;
        var newLastIndex = wochenplanEintragCollection.LetzteUnterrichtsstundeIndex;
        WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex = Math.Min(
          Math.Min(storedFirstIndex, newFirstIndex), Math.Min(storedFirstIndex, newLastIndex));
        WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex = Math.Max(
          Math.Max(storedLastIndex, newFirstIndex), Math.Max(storedLastIndex, newLastIndex));
      }
    }

    private void SurroundBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var wochenplanEintragCollection = this.DataContext as TerminplanEintragCollection;

      // Enable edit 
      if (wochenplanEintragCollection != null && wochenplanEintragCollection.IsDummy)
      {
        this.isCurrentlyClicked = true;
        this.SurroundBorder.Background = Brushes.LightCoral;
        WochenplanSelection.Instance.WochentagIndex = wochenplanEintragCollection.WochentagIndex;
        WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex =
          wochenplanEintragCollection.ErsteUnterrichtsstundeIndex;
        WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex =
          wochenplanEintragCollection.LetzteUnterrichtsstundeIndex;
      }
    }

    private void UIElement_OnGotMouseCapture(object sender, MouseEventArgs e)
    {
      // Wenn kein gültiger Wochenplaneintrag ausgewählt ist,
      // muss für die Markierung von Stunden im Wochenplan das Mousecapturing
      // deaktiviert werden, sonst werden die MouseEnter events 
      // für die anderen Stunden nicht ausgelöst.
      var collection = this.SurroundBorder.DataContext as TerminplanEintragCollection;
      if (collection != null && collection.IsDummy)
      {
        Mouse.Capture(null);
      }
    }
  }
}
