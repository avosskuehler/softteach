// <copyright file="DialogHeader.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
// LEUDA - Lehrerunterrichtsdatenbank
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

namespace SoftTeach.Resources.Controls
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  /// <summary>
  /// Interaction logic for SmallDialogHeader.xaml
  /// </summary>
  public partial class SmallDialogHeader : UserControl
  {
    /// <summary>
    ///   The Icon property.
    /// </summary>
    public static readonly DependencyProperty IconContentProperty =
      DependencyProperty.Register("IconContent", typeof(UIElement), typeof(SmallDialogHeader), new PropertyMetadata());

    /// <summary>
    ///   The Title property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(string), typeof(SmallDialogHeader), new PropertyMetadata("Dialog Titel"));

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallDialogHeader"/> class.
    /// </summary>
    public SmallDialogHeader()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Holt oder setzt das UIElement für das Icon
    /// </summary>
    public UIElement IconContent
    {
      get => (UIElement)this.GetValue(IconContentProperty);

      set => this.SetValue(IconContentProperty, value);
    }

    /// <summary>
    /// Holt oder setzt den Titel.
    /// </summary>
    public string Title
    {
      get => (string)this.GetValue(TitleProperty);

      set => this.SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Sets Icon.
    /// </summary>
    public ImageSource Icon
    {
      get
      {
        return this.icon.Source;
      }
      set
      {
        this.icon.Source = value;
      }
    }
  }
}