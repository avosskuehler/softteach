// <copyright file="WochenplanSelection.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace Liduv.View.Wochenpläne
{
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// The selection.
  /// </summary>
  public class WochenplanSelection : DependencyObject, INotifyPropertyChanged
  {
    #region Constants and Fields

    /// <summary>
    ///   The Plangrid property.
    /// </summary>
    public static readonly DependencyProperty PlangridProperty = DependencyProperty.Register(
      "Plangrid", typeof(Grid), typeof(WochenplanSelection), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    ///   The SelectedDayIndex property.
    /// </summary>
    public static readonly DependencyProperty WochentagIndexProperty = DependencyProperty.Register(
      "StundenplaneintragWochentagIndex", typeof(int), typeof(WochenplanSelection), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    ///   The SelectedErsteStundeIndex property.
    /// </summary>
    public static readonly DependencyProperty ErsteUnterrichtsstundeIndexProperty = DependencyProperty.Register(
      "StundenplaneintragErsteUnterrichtsstundeIndex", typeof(int), typeof(WochenplanSelection), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    ///   The SelectedLetzteStundeIndex property.
    /// </summary>
    public static readonly DependencyProperty LetzteUnterrichtsstundeIndexProperty = DependencyProperty.Register(
      "StundenplaneintragLetzteUnterrichtsstundeIndex", typeof(int), typeof(WochenplanSelection), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    ///   The instance.
    /// </summary>
    private static WochenplanSelection instance;

    /// <summary>
    /// The notify property changed.
    /// </summary>
    private readonly bool notifyPropertyChanged;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Prevents a default instance of the <see cref = "WochenplanSelection" /> class from being created. 
    ///   Initializes a new instance of the Selection class.
    /// </summary>
    private WochenplanSelection()
    {
      this.notifyPropertyChanged = true;
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Holt den <see cref = "WochenplanSelection" /> singleton.
    ///   If the underlying instance is null, a instance will be created.
    /// </summary>
    public static WochenplanSelection Instance
    {
      get
      {
        // check again, if the underlying instance is null
        // return the existing/new instance
        return instance ?? (instance = new WochenplanSelection());
      }
    }

    /// <summary>
    ///   Gets or sets Plangrid.
    /// </summary>
    public Grid Plangrid
    {
      get
      {
        return (Grid)this.GetValue(PlangridProperty);
      }

      set
      {
        this.SetValue(PlangridProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets StundenplaneintragWochentagIndex.
    /// </summary>
    public int WochentagIndex
    {
      get
      {
        return (int)this.GetValue(WochentagIndexProperty);
      }

      set
      {
        this.SetValue(WochentagIndexProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets StundenplaneintragErsteUnterrichtsstundeIndex.
    /// </summary>
    public int ErsteUnterrichtsstundeIndex
    {
      get
      {
        return (int)this.GetValue(ErsteUnterrichtsstundeIndexProperty);
      }

      set
      {
        this.SetValue(ErsteUnterrichtsstundeIndexProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets StundenplaneintragLetzteUnterrichtsstundeIndex.
    /// </summary>
    public int LetzteUnterrichtsstundeIndex
    {
      get
      {
        return (int)this.GetValue(LetzteUnterrichtsstundeIndexProperty);
      }

      set
      {
        this.SetValue(LetzteUnterrichtsstundeIndexProperty, value);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name.
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null && this.notifyPropertyChanged)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null && this.notifyPropertyChanged)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var selection = obj as WochenplanSelection;
      if (selection != null)
      {
        selection.OnPropertyChanged(args);
      }
    }

    #endregion
  }
}