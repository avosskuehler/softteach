// <copyright file="MetroNotentendenzDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

  using SoftTeach.Model.EntityFramework;

  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Noten;

  /// <summary>
  /// The add note dialog.
  /// </summary>
  public partial class MetroNotentendenzDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroNotentendenzDialog"/> Klasse.
    /// </summary>
    /// <param name="notentendenzViewModel">
    /// The noten Workspace View Model.
    /// </param>
    public MetroNotentendenzDialog(NotentendenzViewModel notentendenzViewModel)
    {
      this.DataContext = notentendenzViewModel;
      this.InitializeComponent();
    }

    #endregion

    /// <summary>
    /// Handles the OnClick event of the Fertig button control.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private async void FertigButtonOnClick(object sender, RoutedEventArgs e)
    {
      var metroWindow = Configuration.Instance.MetroWindow;
      await metroWindow.HideMetroDialogAsync(this);
      Selection.Instance.Schülereintrag.UpdateNoten();
    }
  }
}