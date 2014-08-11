// <copyright file="AskForSchülerlisteToAddDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
    private Rectangle currentSitzplatz;

    /// <summary>
    /// Gibt an, ob ein Sitzplatz verschoben wird
    /// </summary>
    private bool isMovingSitzplatz;

    private List<Rectangle> sitzplatzShapes;

    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="EditRaumplanDialog"/> Klasse. 
    /// </summary>
    public EditRaumplanDialog(RaumplanViewModel raumplan)
    {
      this.Raumplan = raumplan;
      this.CreateSitzplatzShapes();
      this.DataContext = this;
      this.InitializeComponent();
    }

    private void CreateSitzplatzShapes()
    {
      foreach (var sitzplatzViewModel in this.Raumplan.Sitzplätze)
      {
        var rect = new Rectangle();
        rect.Fill = System.Windows.Media.Brushes.CadetBlue;
        rect.Width = sitzplatzViewModel.SitzplatzBreite;
        rect.Height = sitzplatzViewModel.SitzplatzHöhe;
        Canvas.SetTop(rect, sitzplatzViewModel.SitzplatzLinksObenY);
        Canvas.SetLeft(rect, sitzplatzViewModel.SitzplatzLinksObenX);
        this.RaumplanCanvas.Children.Add(rect);
      }
    }

    #endregion

    /// <summary>
    /// Holt oder setzt den zu bearbeitenden Raumplan
    /// </summary>
    public RaumplanViewModel Raumplan { get; set; }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Raumplan.RemoveAllSitzplätze();
      foreach (var shape in this.sitzplatzShapes)
      {
        this.Raumplan.AddSitzplatz(shape);
      }

      this.Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var clickLocation = e.GetPosition(this.RaumplanCanvas);
      this.currentSitzplatz = this.GetSitzplatzUnderCursor(clickLocation);
      if (this.currentSitzplatz == null)
      {
        this.mouseDownPoint = clickLocation;
        var rect = new Rectangle();
        rect.Fill = System.Windows.Media.Brushes.CadetBlue;
        rect.Width = 0;
        rect.Height = 0;
        Canvas.SetTop(rect, clickLocation.Y);
        Canvas.SetLeft(rect, clickLocation.X);
        this.RaumplanCanvas.Children.Add(rect);
        this.currentSitzplatz = rect;
        this.isMovingSitzplatz = false;
      }
      else
      {
        this.RaumplanCanvas.Cursor = Cursors.SizeAll;
        this.isMovingSitzplatz = true;
        this.topLeftOffset = new Point(
          clickLocation.X - Canvas.GetLeft(this.currentSitzplatz),
          clickLocation.Y - Canvas.GetTop(this.currentSitzplatz));
      }
    }

    private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
    {
      var moveLocation = e.GetPosition(this.RaumplanCanvas);
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (this.isMovingSitzplatz)
        {
          Canvas.SetLeft(this.currentSitzplatz, moveLocation.X + this.topLeftOffset.X);
          Canvas.SetTop(this.currentSitzplatz, moveLocation.Y + this.topLeftOffset.Y);
        }
        else
        {
          this.currentSitzplatz.Width = moveLocation.X - this.mouseDownPoint.X;
          this.currentSitzplatz.Height = moveLocation.Y - this.mouseDownPoint.Y;
        }
      }
      else
      {
        if (this.GetSitzplatzUnderCursor(moveLocation) != null)
        {
          this.RaumplanCanvas.Cursor = Cursors.Hand;
        }
      }
    }


    private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      var upLocation = e.GetPosition(this.RaumplanCanvas);
      if (this.isMovingSitzplatz)
      {
        Canvas.SetLeft(this.currentSitzplatz, upLocation.X + this.topLeftOffset.X);
        Canvas.SetTop(this.currentSitzplatz, upLocation.Y + this.topLeftOffset.Y);
      }
      else
      {
        this.currentSitzplatz.Width = upLocation.X - this.mouseDownPoint.X;
        this.currentSitzplatz.Height = upLocation.Y - this.mouseDownPoint.Y;
      }
    }

    private Rectangle GetSitzplatzUnderCursor(Point mouseLocation)
    {
      var result = VisualTreeHelper.HitTest(this.RaumplanCanvas, mouseLocation);

      if (result != null)
      {
        var rect = result.VisualHit as Rectangle;
        return rect;
      }

      return null;
    }
  }
}