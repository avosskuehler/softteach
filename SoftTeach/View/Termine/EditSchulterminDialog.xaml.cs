// <copyright file="EditSchulterminDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// Interaction logic for EditSchulterminDialog.xaml
  /// </summary>
  public partial class EditSchulterminDialog
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EditSchulterminDialog"/> class.
    /// </summary>
    public EditSchulterminDialog()
    {
      this.InitializeComponent();
      this.DataContext = App.MainViewModel;
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void UpdateClick(object sender, RoutedEventArgs e)
    {
      SchulterminWorkspaceViewModel.UpdateJahrespläne();
    }
  }
}