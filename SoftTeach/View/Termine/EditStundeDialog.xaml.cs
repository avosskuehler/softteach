// <copyright file="AddStundeDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Termine
{
  using System.Windows;
  using System.Windows.Controls;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// Interaction logic for EditStundeDialog.xaml
  /// </summary>
  public partial class EditStundeDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Stundenplan.EditStundeDialog"/> class.
    /// </summary>
    public EditStundeDialog(StundeViewModel stundeViewModel)
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

    /// <summary>
    /// Handles the OnSelected event of the PhasenGrid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void PhasenGrid_OnCellSelected(object sender, RoutedEventArgs e)
    {
      // Lookup for the source to be DataGridCell
      if (e.OriginalSource.GetType() == typeof(DataGridCell))
      {
        // Starts the Edit on the row;
        var grd = (DataGrid)sender;
        //grd.BeginEdit(e);
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