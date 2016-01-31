// <copyright file="AskForSchülerlisteToAddDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
// SoftTeach - Lehrerunterrichtsdatenbank
// Copyright (C) 2013 Dr. Adrian Voßkühler
// -----------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published  
// by the Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version. This program is distributed in the 
// hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// ***********************************************************************
// </copyright>
// <author>Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>

namespace SoftTeach.View.Sitzpläne
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Drawing;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Media;

  using SoftTeach.ViewModel.Sitzpläne;

  using Brush = System.Windows.Media.Brush;
  using Point = System.Windows.Point;
  using Rectangle = System.Windows.Shapes.Rectangle;

  /// <summary>
  /// Interaction logic for EditRaumplanDialog.xaml
  /// </summary>
  public partial class EditRaumplanDialog
  {
    /// <summary>
    /// Die Position den Mausklicks
    /// </summary>
    private Point mouseDownPoint;

    /// <summary>
    /// Der Abstand zwischen linker oberer Ecke und Klickort
    /// </summary>
    private Point topLeftOffset;

    /// <summary>
    /// Die linke obere X-Koordinate des aktuellen Sitzplatzes
    /// </summary>
    private double originalLeft;

    /// <summary>
    /// Die linke obere Y-Koordinate des aktuellen Sitzplatzes
    /// </summary>
    private double originalTop;

    /// <summary>
    /// Der momentan bearbeitete Sitzplatz
    /// </summary>
    private SitzplatzViewModel currentSitzplatz;

    /// <summary>
    /// Gibt an, ob ein Sitzplatz neu erstellt wird.
    /// </summary>
    private bool isCreatingSitzplatz;

    /// <summary>
    /// Gibt an, ob ein Sitzplatz bewegt wird
    /// </summary>
    private bool isDragging;

    /// <summary>
    /// Gibt an, ob der Sitzplatz schon kopiert wurde
    /// </summary>
    private bool hasCopiedSitzplatz;

    /// <summary>
    /// Die Liste der Sitzplanrechtecke
    /// </summary>
    private List<Rectangle> sitzplatzShapes;

    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="EditRaumplanDialog"/> Klasse. 
    /// </summary>
    /// <param name="raumplan">
    /// The raumplan.
    /// </param>
    public EditRaumplanDialog(RaumplanViewModel raumplan)
    {
      this.Raumplan = raumplan;
      this.DataContext = this;
      this.InitializeComponent();
      this.AddSitzplatzShapes();
    }

    #endregion

    /// <summary>
    /// Holt oder setzt den zu bearbeitenden Raumplan
    /// </summary>
    public RaumplanViewModel Raumplan { get; set; }

    /// <summary>
    /// Schließt den Dialog und übernimmt die Änderungen
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;

      foreach (var sitzplatzViewModel in this.Raumplan.Sitzplätze)
      {
        sitzplatzViewModel.UpdateModelFromShape();
      }

      this.RemoveSitzplatzShapes();
      this.Close();
    }

    /// <summary>
    /// Schließt den Dialog ohne die Änderungen zu übernehmen
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.RemoveSitzplatzShapes();
      this.Close();
    }

    /// <summary>
    /// Handles the OnMouseLeftButtonDown event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var clickLocation = e.GetPosition(this.RaumplanCanvas);
      this.currentSitzplatz = this.GetSitzplatzUnderCursor(clickLocation);
      this.mouseDownPoint = clickLocation;
      if (this.currentSitzplatz == null)
      {
        // Sitzplatz neu erstellen
        this.isCreatingSitzplatz = true;
        this.Raumplan.AddSitzplatz(clickLocation.X, clickLocation.Y, 0, 0, 0);
        this.RaumplanCanvas.Children.Add(this.Raumplan.CurrentSitzplatz.Shape);
        this.currentSitzplatz = this.Raumplan.CurrentSitzplatz;
      }
      else
      {
        // Sitzplatz verschieben oder drehen
        this.isCreatingSitzplatz = false;
        this.originalLeft = Canvas.GetLeft(this.currentSitzplatz.Shape);
        this.originalTop = Canvas.GetTop(this.currentSitzplatz.Shape);
        this.topLeftOffset = new Point(
          clickLocation.X - Canvas.GetLeft(this.currentSitzplatz.Shape),
          clickLocation.Y - Canvas.GetTop(this.currentSitzplatz.Shape));
      }

      e.Handled = true;
    }

    /// <summary>
    /// Handles the OnMouseRightButtonDown event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      var clickLocation = e.GetPosition(this.RaumplanCanvas);
      this.currentSitzplatz = this.GetSitzplatzUnderCursor(clickLocation);
      if (this.currentSitzplatz != null)
      {
        this.Raumplan.DeleteSitzplatz(this.currentSitzplatz);
        this.RaumplanCanvas.Children.Remove(this.currentSitzplatz.Shape);
        this.currentSitzplatz = null;
        this.isCreatingSitzplatz = false;
        this.hasCopiedSitzplatz = false;
      }

      e.Handled = true;
    }

    /// <summary>
    /// Handles the PreviewKeyDown event of the Window control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
    private void WindowPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape && this.isDragging)
      {
        Canvas.SetTop(this.currentSitzplatz.Shape, this.originalTop);
        Canvas.SetLeft(this.currentSitzplatz.Shape, this.originalLeft);
        e.Handled = true;
      }

      if (this.currentSitzplatz == null)
      {
        return;
      }

      if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
      {
        // Drehen
        this.RaumplanCanvas.Cursor = ((TextBlock)this.Resources["CursorRotate"]).Cursor;
        e.Handled = true;
      }
      else if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
      {
        // Drehen
        this.RaumplanCanvas.Cursor = ((TextBlock)this.Resources["CursorCopy"]).Cursor;

        if (this.isDragging && !this.hasCopiedSitzplatz)
        {
          if (this.currentSitzplatz != null)
          {
            // Reset original position before copying
            Canvas.SetLeft(this.currentSitzplatz.Shape, this.originalLeft);
            Canvas.SetTop(this.currentSitzplatz.Shape, this.originalTop);
          }

          var moveLocation = Mouse.GetPosition(this.RaumplanCanvas);
          this.Raumplan.AddSitzplatz(moveLocation.X, moveLocation.Y, this.currentSitzplatz.Shape);
          this.RaumplanCanvas.Children.Add(this.Raumplan.CurrentSitzplatz.Shape);
          this.currentSitzplatz = this.Raumplan.CurrentSitzplatz;
          Canvas.SetLeft(this.currentSitzplatz.Shape, moveLocation.X);
          Canvas.SetTop(this.currentSitzplatz.Shape, moveLocation.Y);
          Debug.WriteLine("KeyDownCreateShape");
          this.hasCopiedSitzplatz = true;
        }

        e.Handled = true;
      }
      else
      {
        // Verschieben
        this.RaumplanCanvas.Cursor = Cursors.Hand;
        e.Handled = true;
      }
    }

    /// <summary>
    /// Handles the OnMouseMove event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      var moveLocation = e.GetPosition(this.RaumplanCanvas);
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (this.currentSitzplatz == null)
        {
          return;
        }

        if (this.isCreatingSitzplatz)
        {
          if (this.currentSitzplatz != null)
          {
            this.currentSitzplatz.Shape.Width = Math.Abs(moveLocation.X - this.mouseDownPoint.X);
            this.currentSitzplatz.Shape.Height = Math.Abs(moveLocation.Y - this.mouseDownPoint.Y);
          }
        }
        else
        {
          if (this.currentSitzplatz != null && !this.isDragging && (Math.Abs(moveLocation.X - this.mouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
              Math.Abs(moveLocation.Y - this.mouseDownPoint.Y) > SystemParameters.MinimumVerticalDragDistance))
          {
            this.isDragging = true;
          }

          if (this.isDragging)
          {
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
              Canvas.SetLeft(this.currentSitzplatz.Shape, moveLocation.X - this.topLeftOffset.X);
              Canvas.SetTop(this.currentSitzplatz.Shape, moveLocation.Y - this.topLeftOffset.Y);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
            {
              this.currentSitzplatz.Shape.RenderTransform = new RotateTransform(moveLocation.X - this.mouseDownPoint.X);
            }
            else
            {
              Canvas.SetLeft(this.currentSitzplatz.Shape, moveLocation.X - this.topLeftOffset.X);
              Canvas.SetTop(this.currentSitzplatz.Shape, moveLocation.Y - this.topLeftOffset.Y);
            }
          }
        }
      }
      else
      {
        this.currentSitzplatz = this.GetSitzplatzUnderCursor(moveLocation);
        if (this.currentSitzplatz != null)
        {
          if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
          {
            // Drehen
            this.RaumplanCanvas.Cursor = ((TextBlock)this.Resources["CursorRotate"]).Cursor;
          }
          else if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
          {
            // Drehen
            this.RaumplanCanvas.Cursor = ((TextBlock)this.Resources["CursorCopy"]).Cursor;
          }
          else
          {
            // Verschieben
            this.RaumplanCanvas.Cursor = Cursors.Hand;
          }
        }
        else
        {
          this.RaumplanCanvas.Cursor = Cursors.Arrow;
        }
      }
    }

    /// <summary>
    /// Handles the OnMouseLeftButtonUp event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.currentSitzplatz == null)
      {
        return;
      }

      var upLocation = e.GetPosition(this.RaumplanCanvas);

      if (this.isCreatingSitzplatz)
      {
        this.currentSitzplatz.Shape.Width = Math.Abs(upLocation.X - this.mouseDownPoint.X);
        this.currentSitzplatz.Shape.Height = Math.Abs(upLocation.Y - this.mouseDownPoint.Y);
      }
      else
      {
        if (this.isDragging)
        {
          Canvas.SetTop(this.currentSitzplatz.Shape, upLocation.Y - this.topLeftOffset.Y);
          Canvas.SetLeft(this.currentSitzplatz.Shape, upLocation.X - this.topLeftOffset.X);
        }
      }

      this.isDragging = false;
      this.isCreatingSitzplatz = false;
      this.hasCopiedSitzplatz = false;
    }

    /// <summary>
    /// Gets the sitzplatz under cursor.
    /// </summary>
    /// <param name="mouseLocation">The mouse location.</param>
    /// <returns>Das <see cref="SitzplatzViewModel"/> oder null, wenn kein Sitzplatz unter der Maus.</returns>
    private SitzplatzViewModel GetSitzplatzUnderCursor(Point mouseLocation)
    {
      var result = VisualTreeHelper.HitTest(this.RaumplanCanvas, mouseLocation);

      if (result == null)
      {
        return null;
      }

      var rect = result.VisualHit as Rectangle;
      if (rect == null)
      {
        return null;
      }

      var viewModel = rect.Tag as SitzplatzViewModel;
      return viewModel;
    }

    /// <summary>
    /// Adds the sitzplatz shapes.
    /// </summary>
    private void AddSitzplatzShapes()
    {
      foreach (var sitzplatzViewModel in this.Raumplan.Sitzplätze)
      {
        this.RaumplanCanvas.Children.Add(sitzplatzViewModel.Shape);
      }
    }

    /// <summary>
    /// Removes the sitzplatz shapes.
    /// </summary>
    private void RemoveSitzplatzShapes()
    {
      foreach (var sitzplatzViewModel in this.Raumplan.Sitzplätze)
      {
        this.RaumplanCanvas.Children.Remove(sitzplatzViewModel.Shape);
      }
    }
  }
}