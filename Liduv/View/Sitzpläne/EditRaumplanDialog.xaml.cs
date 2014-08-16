﻿// <copyright file="AskForSchülerlisteToAddDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
// Liduv - Lehrerunterrichtsdatenbank
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

namespace Liduv.View.Sitzpläne
{
  using System.Collections.Generic;
  using System.Drawing;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Media;

  using Liduv.ViewModel.Sitzpläne;

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
    /// Der momentan bearbeitete Sitzplatz
    /// </summary>
    private SitzplatzViewModel currentSitzplatz;

    /// <summary>
    /// Gibt an, ob ein Sitzplatz verschoben wird
    /// </summary>
    private bool isMovingSitzplatz;

    /// <summary>
    /// Gibt an, dass der Sitzplatz auch kopiert werden kann
    /// </summary>
    private bool canCopySitzplatz;

    /// <summary>
    /// Die Liste der Sitzplanrechtecke
    /// </summary>
    private List<Rectangle> sitzplatzShapes;

    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="EditRaumplanDialog"/> Klasse. 
    /// </summary>
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
    private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var clickLocation = e.GetPosition(this.RaumplanCanvas);
      this.currentSitzplatz = this.GetSitzplatzUnderCursor(clickLocation);
      this.mouseDownPoint = clickLocation;
      if (this.currentSitzplatz == null)
      {
        this.Raumplan.AddSitzplatz(clickLocation.X, clickLocation.Y, 0, 0);
        this.RaumplanCanvas.Children.Add(this.Raumplan.CurrentSitzplatz.Shape);
        this.currentSitzplatz = this.Raumplan.CurrentSitzplatz;
        this.isMovingSitzplatz = false;
      }
      else
      {
        this.RaumplanCanvas.Cursor = Cursors.SizeAll;
        this.isMovingSitzplatz = true;
        this.topLeftOffset = new Point(
          clickLocation.X - Canvas.GetLeft(this.currentSitzplatz.Shape),
          clickLocation.Y - Canvas.GetTop(this.currentSitzplatz.Shape));
        this.canCopySitzplatz = true;
      }
    }

    /// <summary>
    /// Handles the OnMouseRightButtonDown event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void Canvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      var clickLocation = e.GetPosition(this.RaumplanCanvas);
      this.currentSitzplatz = this.GetSitzplatzUnderCursor(clickLocation);
      if (this.currentSitzplatz != null)
      {
        this.Raumplan.DeleteSitzplatz(this.currentSitzplatz);
        this.RaumplanCanvas.Children.Remove(this.currentSitzplatz.Shape);
        this.currentSitzplatz = null;
        this.isMovingSitzplatz = false;
        this.canCopySitzplatz = false;
      }
    }

    /// <summary>
    /// Handles the OnMouseMove event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
    {
      var moveLocation = e.GetPosition(this.RaumplanCanvas);
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (this.isMovingSitzplatz)
        {
          if ((Keyboard.Modifiers & ModifierKeys.Control) > 0 && this.canCopySitzplatz)
          {
            Canvas.SetTop(this.currentSitzplatz.Shape, this.mouseDownPoint.Y - this.topLeftOffset.Y);
            Canvas.SetLeft(this.currentSitzplatz.Shape, this.mouseDownPoint.X - this.topLeftOffset.X);
            this.Raumplan.AddSitzplatz(moveLocation.X, moveLocation.Y,this.currentSitzplatz.Shape.Width, this.currentSitzplatz.Shape.Height);
            this.RaumplanCanvas.Children.Add(this.Raumplan.CurrentSitzplatz.Shape);
            this.currentSitzplatz = this.Raumplan.CurrentSitzplatz;
            this.isMovingSitzplatz = true;
            this.canCopySitzplatz = false;
          }
          else
          {
            Canvas.SetLeft(this.currentSitzplatz.Shape, moveLocation.X - this.topLeftOffset.X);
            Canvas.SetTop(this.currentSitzplatz.Shape, moveLocation.Y - this.topLeftOffset.Y);
          }
        }
        else
        {
          if (this.currentSitzplatz != null)
          {
            this.currentSitzplatz.Shape.Width = System.Math.Abs(moveLocation.X - this.mouseDownPoint.X);
            this.currentSitzplatz.Shape.Height = System.Math.Abs(moveLocation.Y - this.mouseDownPoint.Y);
          }
        }
      }
      else
      {
        if (this.GetSitzplatzUnderCursor(moveLocation) != null)
        {
          this.RaumplanCanvas.Cursor = Cursors.Hand;
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
    private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.currentSitzplatz == null)
      {
        return;
      }

      var upLocation = e.GetPosition(this.RaumplanCanvas);
      if (this.isMovingSitzplatz)
      {
        Canvas.SetLeft(this.currentSitzplatz.Shape, upLocation.X - this.topLeftOffset.X);
        Canvas.SetTop(this.currentSitzplatz.Shape, upLocation.Y - this.topLeftOffset.Y);
      }
      else
      {
        this.currentSitzplatz.Shape.Width = System.Math.Abs(upLocation.X - this.mouseDownPoint.X);
        this.currentSitzplatz.Shape.Height = System.Math.Abs(upLocation.Y - this.mouseDownPoint.Y);
      }
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