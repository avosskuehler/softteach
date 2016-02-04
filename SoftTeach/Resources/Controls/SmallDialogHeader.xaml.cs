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
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  /// <summary>
  /// Interaction logic for SmallDialogHeader.xaml
  /// </summary>
  public partial class SmallDialogHeader : UserControl
  {
    ///// <summary>
    /////   The Title property.
    ///// </summary>
    //public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SmallDialogHeader),
    //  new FrameworkPropertyMetadata(TitlePropertyChanged));

    //private static void TitlePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    //{
    //  var smallDialogHeader = dependencyObject as SmallDialogHeader;
    //  if (smallDialogHeader != null)
    //  {
    //    smallDialogHeader.OnPropertyChanged(e);
    //  }
    //}

    //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    //{
    //  if (e.Property == TitleProperty)
    //  {
    //    this.Header.Text = e.NewValue.ToString();
    //  }
    //}

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallDialogHeader"/> class.
    /// </summary>
    public SmallDialogHeader()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Public Properties

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

    /// <summary>
    /// Gets or sets Title.
    /// </summary>
    public string Title
    {
      get
      {
        return this.Header.Text;
      }

      set
      {
        this.Header.Text = value;
      }
    }

    ///// <summary>
    ///// Gets or sets Title.
    ///// </summary>
    //public string Title
    //{
    //  get
    //  {
    //    return (string)this.GetValue(TitleProperty);
    //  }

    //  set
    //  {
    //    this.SetValue(TitleProperty, value);
    //  }
    //}

    #endregion
  }
}