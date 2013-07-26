
namespace Liduv.View.Wochenpläne
{
  using System;
  using System.Windows.Input;
  using System.Windows.Media;

  using Liduv.ViewModel.Wochenpläne;

  /// <summary>
  /// Interaction logic for WochenplaneintragDetailView.xaml
  /// </summary>
  public partial class WochenplaneintragDetailView
  {
    private bool isCurrentlyClicked;

    public WochenplaneintragDetailView()
    {
      this.InitializeComponent();
      WochenplanSelection.Instance.Plangrid.MouseLeftButtonDown += PlangridMouseLeftButtonDown;
      WochenplanSelection.Instance.Plangrid.MouseLeftButtonUp += this.PlangridMouseLeftButtonUp;
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
        var wochenplanEintragViewModel = this.DataContext as WochenplanEintrag;
        if (wochenplanEintragViewModel != null && WochenplanSelection.Instance.WochentagIndex != wochenplanEintragViewModel.WochentagIndex)
        {
          return;
        }

        // Enable edit
        this.SurroundBorder.Background = Brushes.LightCoral;

        //WochenplanSelection.Instance.StundenplaneintragWochentagIndex = wochenplanEintragViewModel.StundenplaneintragWochentagIndex;
        int storedFirstIndex = WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex;
        int storedLastIndex = WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex;
        var newFirstIndex = wochenplanEintragViewModel.ErsteUnterrichtsstundeIndex;
        var newLastIndex = wochenplanEintragViewModel.LetzteUnterrichtsstundeIndex;
        WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex = Math.Min(
          Math.Min(storedFirstIndex, newFirstIndex), Math.Min(storedFirstIndex, newLastIndex));
        WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex = Math.Max(
          Math.Max(storedLastIndex, newFirstIndex), Math.Max(storedLastIndex, newLastIndex));
      }
    }

    private void SurroundBorderMouseLeave(object sender, MouseEventArgs e)
    {
      //if (e.LeftButton == MouseButtonState.Released)
      //{
      //  this.SurroundBorder.Background = Brushes.Transparent;
      //}
    }

    private void SurroundBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var wochenplanEintragViewModel = this.DataContext as WochenplanEintrag;

      // Enable edit 
      if (wochenplanEintragViewModel != null && wochenplanEintragViewModel.IsDummy)
      {
        this.isCurrentlyClicked = true;
        this.SurroundBorder.Background = Brushes.LightCoral;
        WochenplanSelection.Instance.WochentagIndex = wochenplanEintragViewModel.WochentagIndex;
        WochenplanSelection.Instance.ErsteUnterrichtsstundeIndex =
          wochenplanEintragViewModel.ErsteUnterrichtsstundeIndex;
        WochenplanSelection.Instance.LetzteUnterrichtsstundeIndex =
          wochenplanEintragViewModel.LetzteUnterrichtsstundeIndex;
      }
    }
  }
}
