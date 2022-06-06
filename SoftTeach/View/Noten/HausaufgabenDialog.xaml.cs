﻿// <copyright file="HausaufgabenDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// Ein Dialog um nicht gemachte Hausaufgaben einzutragen.
  /// </summary>
  public partial class HausaufgabenDialog
  {

    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="HausaufgabenDialog"/> Klasse.
    /// </summary>
    public HausaufgabenDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
    }

    #endregion

    public LerngruppeViewModel Lerngruppe { get; set; }
    
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