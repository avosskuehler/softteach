// <copyright file="AddLerngruppenterminDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace Liduv.View.Jahrespläne
{
  using System.Windows;

  using Liduv.ViewModel;
  using Liduv.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for CreateLerngruppenterminDialog.xaml
  /// </summary>
  public partial class AddLerngruppenterminDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AddLerngruppenterminDialog"/> class.
    /// </summary>
    public AddLerngruppenterminDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;

      // Select first Termintyp
      this.TerminTermintyp = App.MainViewModel.Termintypen[0];
      this.TerminErsteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[0];
      this.TerminLetzteUnterrichtsstunde = App.MainViewModel.Unterrichtsstunden[6];
      this.TerminBezeichnung.Focus();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Holt oder setzt die Terminbezeichnng
    /// </summary>
    public string Terminbezeichnung
    {
      get
      {
        return this.TerminBezeichnung.Text;
      }

      set
      {
        this.TerminBezeichnung.Text = value;
      }
    }

    /// <summary>
    /// Holt oder setzt die Termintyp
    /// </summary>
    public TermintypViewModel TerminTermintyp { get; set; }

    /// <summary>
    /// Holt oder setzt die erste Unterrichtsstunde
    /// </summary>
    public UnterrichtsstundeViewModel TerminErsteUnterrichtsstunde { get; set; }

    /// <summary>
    /// Holt oder setzt die zweite Unterrichtsstunde
    /// </summary>
    public UnterrichtsstundeViewModel TerminLetzteUnterrichtsstunde { get; set; }

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