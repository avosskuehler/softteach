// <copyright file="AskForLerngruppeToAdaptDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Sitzpläne;

  /// <summary>
  /// Interaction logic for AskForRaumToAdaptDialog.xaml
  /// </summary>
  public partial class AskForRaumToAdaptDialog
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AskForRaumToAdaptDialog"/> Klasse. 
    /// </summary>
    public AskForRaumToAdaptDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
    }

    /// <summary>
    /// Holt oder setzt den ausgewählten Raum
    /// </summary>
    public RaumViewModel SelectedRaum { get; set; }

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