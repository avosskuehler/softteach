// <copyright file="AddNotentendenzDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Noten
{
  using System.Windows;

  using SoftTeach.ViewModel.Noten;

  /// <summary>
  /// The add note dialog.
  /// </summary>
  public partial class NotentendenzDialog
  {
    /// <summary>
    /// Die aktuell zu bearbeitende Notentendenz.
    /// </summary>
    private NotentendenzViewModel currentNotentendenz;

    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NotentendenzDialog"/> Klasse.
    /// </summary>
    public NotentendenzDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
    }

    #endregion

    /// <summary>
    /// Holt oder setzt die aktuell zu bearbeitende Notentendenz
    /// </summary>
    public NotentendenzViewModel CurrentNotentendenz
    {
      get
      {
        return this.currentNotentendenz;
      }

      set
      {
        this.currentNotentendenz = value;
      }
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}