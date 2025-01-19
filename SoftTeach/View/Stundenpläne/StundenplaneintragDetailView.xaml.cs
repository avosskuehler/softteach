
namespace SoftTeach.View.Stundenpläne
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

  using SoftTeach.ViewModel.Stundenpläne;

  /// <summary>
  /// Interaction logic for StundenplaneintragDetailView.xaml
  /// </summary>
  public partial class StundenplaneintragDetailView : UserControl
  {
    private bool isCurrentlyClicked;

    public StundenplaneintragDetailView()
    {
      this.InitializeComponent();
      StundenplanSelection.Instance.Plangrid.MouseLeftButtonDown += this.PlangridMouseLeftButtonDown;
      StundenplanSelection.Instance.Plangrid.MouseLeftButtonUp += this.PlangridMouseLeftButtonUp;
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
        var stundenplanEintragViewModel = this.DataContext as StundenplaneintragViewModel;

        // Enable edit on edit view
        if ((stundenplanEintragViewModel.Parent.ViewMode & StundenplanViewMode.Edit) == StundenplanViewMode.Edit)
        {
          if (StundenplanSelection.Instance.WochentagIndex != stundenplanEintragViewModel.StundenplaneintragWochentagIndex)
          {
            return;
          }

          this.SurroundBorder.Background = Brushes.LightCoral;

          //StundenplanSelection.Instance.StundenplaneintragWochentagIndex = stundenplanEintragViewModel.StundenplaneintragWochentagIndex;
          int storedFirstIndex = StundenplanSelection.Instance.ErsteUnterrichtsstundeIndex;
          int storedLastIndex = StundenplanSelection.Instance.LetzteUnterrichtsstundeIndex;
          var newFirstIndex = stundenplanEintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex;
          var newLastIndex = stundenplanEintragViewModel.StundenplaneintragLetzteUnterrichtsstundeIndex;
          StundenplanSelection.Instance.ErsteUnterrichtsstundeIndex = Math.Min(
            Math.Min(storedFirstIndex, newFirstIndex), Math.Min(storedFirstIndex, newLastIndex));
          StundenplanSelection.Instance.LetzteUnterrichtsstundeIndex = Math.Max(
            Math.Max(storedLastIndex, newFirstIndex), Math.Max(storedLastIndex, newLastIndex));
        }
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
      var stundenplanEintragViewModel = this.DataContext as StundenplaneintragViewModel;
      if (stundenplanEintragViewModel.Parent == null)
      {
        return;
      }

      // Enable edit on edit view
      if ((stundenplanEintragViewModel.Parent.ViewMode & StundenplanViewMode.Edit) == StundenplanViewMode.Edit)
      {
        if (stundenplanEintragViewModel.IsDummy)
        {
          this.isCurrentlyClicked = true;
          this.SurroundBorder.Background = Brushes.LightCoral;
          StundenplanSelection.Instance.WochentagIndex = stundenplanEintragViewModel.StundenplaneintragWochentagIndex;
          StundenplanSelection.Instance.ErsteUnterrichtsstundeIndex =
            stundenplanEintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex;
          StundenplanSelection.Instance.LetzteUnterrichtsstundeIndex =
            stundenplanEintragViewModel.StundenplaneintragLetzteUnterrichtsstundeIndex;
          return;
        }
      }

      // Enable Drag and drop on default view
      if ((stundenplanEintragViewModel.Parent.ViewMode & StundenplanViewMode.DragDrop) == StundenplanViewMode.DragDrop)
      {
        // If we clicked on a not dummy stundenplaneintrag, start a drag operation
        var result = DragDrop.DoDragDrop(StundenplanSelection.Instance.Plangrid, stundenplanEintragViewModel, DragDropEffects.Move);
        this.SurroundBorder.AllowDrop = true;
      }
    }

    private void SurroundBorderDragOver(object sender, DragEventArgs e)
    {
      if (e.Data.GetData(typeof(StundenplaneintragViewModel)) is not StundenplaneintragViewModel)
      {
        return;
      }

      var stundenplanEintragViewModel = this.DataContext as StundenplaneintragViewModel;

      if (!stundenplanEintragViewModel.IsDummy)
      {
        this.SurroundBorder.AllowDrop = false;
      }
    }

    private void SurroundBorderDrop(object sender, DragEventArgs e)
    {
      if (e.Data.GetData(typeof(StundenplaneintragViewModel)) is not StundenplaneintragViewModel)
      {
        return;
      }

      var stundenplanEintragViewModel = this.DataContext as StundenplaneintragViewModel;

      if (stundenplanEintragViewModel != null && !stundenplanEintragViewModel.IsDummy)
      {
        return;
      }

      var movedEintrag = (StundenplaneintragViewModel)e.Data.GetData(typeof(StundenplaneintragViewModel));
      var wochentag = movedEintrag.StundenplaneintragWochentagIndex;
      var ersteStundeIndex = movedEintrag.StundenplaneintragErsteUnterrichtsstundeIndex;
      var stundenzahl = movedEintrag.Stundenanzahl;

      if (stundenplanEintragViewModel != null)
      {
        movedEintrag.StundenplaneintragErsteUnterrichtsstundeIndex = stundenplanEintragViewModel.StundenplaneintragErsteUnterrichtsstundeIndex;
        movedEintrag.StundenplaneintragLetzteUnterrichtsstundeIndex = movedEintrag.StundenplaneintragErsteUnterrichtsstundeIndex + stundenzahl - 1;
        movedEintrag.StundenplaneintragWochentagIndex = stundenplanEintragViewModel.StundenplaneintragWochentagIndex;
        stundenplanEintragViewModel.Parent.UpdateProperties(movedEintrag.StundenplaneintragWochentagIndex, movedEintrag.StundenplaneintragErsteUnterrichtsstundeIndex, stundenzahl);
        stundenplanEintragViewModel.Parent.UpdateProperties(wochentag, ersteStundeIndex, stundenzahl);
      }

      this.SurroundBorder.BorderBrush = Brushes.Transparent;

      var änderung = new StundenplanÄnderung(
        StundenplanÄnderungUpdateType.ChangedTimeSlot,
        wochentag,
        ersteStundeIndex,
        movedEintrag);
      movedEintrag.Parent.ÄnderungsListe.Add(änderung);
    }

    private void SurroundBorderDragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetData(typeof(StundenplaneintragViewModel)) is not StundenplaneintragViewModel)
      {
        return;
      }

      this.SurroundBorder.BorderBrush = Brushes.Red;
    }

    private void SurroundBorderDragLeave(object sender, DragEventArgs e)
    {
      if (e.Data.GetData(typeof(StundenplaneintragViewModel)) is not StundenplaneintragViewModel)
      {
        return;
      }

      this.SurroundBorder.BorderBrush = Brushes.Transparent;
    }
  }
}
