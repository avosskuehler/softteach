// <copyright file="AddSonderterminDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Wochenpläne
{
  using System.Windows;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.View.Termine;
  using SoftTeach.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for AddSonderterminDialog.xaml
  /// </summary>
  public partial class AddSonderterminDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AddLerngruppenterminDialog"/> class.
    /// </summary>
    public AddSonderterminDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;

      // Select first Termintyp
      this.TerminTermintyp = Termintyp.Sondertermin;
      this.TerminBezeichnung.Focus();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Holt oder setzt die Terminbezeichnng
    /// </summary>
    public string TerminBeschreibung { get; set; }
    
    /// <summary>
    /// Holt oder setzt die TerminOrt
    /// </summary>
    public string TerminOrt { get; set; }
    
    /// <summary>
    /// Holt oder setzt die Termintyp
    /// </summary>
    public Termintyp TerminTermintyp { get; set; }

    #endregion

    #region Methods

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