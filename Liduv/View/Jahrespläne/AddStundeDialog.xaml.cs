// <copyright file="AddStundeDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using Liduv.ViewModel.Stundenentwürfe;
  using Liduv.ViewModel.Termine;

  /// <summary>
  /// Interaction logic for CreateLerngruppenterminDialog.xaml
  /// </summary>
  public partial class AddStundeDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Stundenplan.AddStundeDialog"/> class.
    /// </summary>
    public AddStundeDialog(StundeViewModel stundeViewModel)
    {
      this.StundeViewModel = stundeViewModel;
      this.InitializeComponent();
      this.DataContext = stundeViewModel;
    }

    #endregion

    /// <summary>
    /// Gets StundeViewModel.
    /// </summary>
    public StundeViewModel StundeViewModel { get; private set; }

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