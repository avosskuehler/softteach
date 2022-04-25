// <copyright file="AddStundenplaneintragDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Stundenpläne
{
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Stundenpläne;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;

  /// <summary>
  /// Interaction logic for CreateStundenplaneintragDialog.xaml
  /// </summary>
  public partial class AddStundenplaneintragDialog
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="AddStundenplaneintragDialog"/> class.
    /// </summary>
    public AddStundenplaneintragDialog(StundenplaneintragViewModel stundenplaneintragViewModel)
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.StundenplaneintragViewModel = stundenplaneintragViewModel;
      this.Lerngruppen = App.MainViewModel.Lerngruppen.Where(o => o.LerngruppeSchuljahr.SchuljahrJahr == stundenplaneintragViewModel.Model.Stundenplan.Schuljahr.Jahr).ToList();
    }

    public List<LerngruppeViewModel> Lerngruppen { get; set; }

    public StundenplaneintragViewModel StundenplaneintragViewModel { get; private set; }

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