﻿// <copyright file="AskForJahrFachStufeDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Curricula
{
  using System.Windows;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.View.Stundenpläne;
  using SoftTeach.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for AskForJahrFachStufeDialog.xaml
  /// </summary>
  public partial class AskForJahrFachStufeDialog
  {
    public SchuljahrViewModel Schuljahr { get; set; }
    public Halbjahr Halbjahr { get; set; }
    public FachViewModel Fach { get; set; }
    public int Jahrgang { get; set; }
    public string Bezeichnung { get; set; }

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AskForGültigAb"/> class.
    /// </summary>
    public AskForJahrFachStufeDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.Schuljahr = Selection.Instance.Schuljahr;
      this.Halbjahr = Selection.Instance.Halbjahr;
      this.Fach = App.MainViewModel.Fächer[0];
      this.Jahrgang = App.MainViewModel.Jahrgänge[0];
      this.Bezeichnung = "Neue Bezeichnung";
    }

    #endregion

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