// <copyright file="AskForTerminSpanDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace Liduv.View.Termine
{
  using System;
  using System.Windows;

  /// <summary>
  /// Interaction logic for AskForTerminSpanDialog.xaml
  /// </summary>
  public partial class AskForTerminSpanDialog
  {
    public DateTime StartDatum
    {
      get
      {
        return this.StartDate.SelectedDate.GetValueOrDefault(DateTime.Now);
      }
    }

    public DateTime EndDatum
    {
      get
      {
        return this.EndDate.SelectedDate.GetValueOrDefault(DateTime.Now);
      }
    }

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AskForTerminSpanDialog"/> class.
    /// </summary>
    public AskForTerminSpanDialog(DateTime startdate)
    {
      this.InitializeComponent();
      this.StartDate.SelectedDate = startdate;
      this.EndDate.SelectedDate = startdate.AddDays(3);
    }

    #endregion

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }
  }
}