// <copyright file="AddNoteDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

  using MahApps.Metro.Controls.Dialogs;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Noten;

  /// <summary>
  /// The add note dialog.
  /// </summary>
  public partial class MetroNoteDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroNoteDialog"/> Klasse.
    /// </summary>
    /// <param name="notenWorkspaceViewModel">
    /// The noten Workspace View Model.
    /// </param>
    public MetroNoteDialog(NotenWorkspaceViewModel notenWorkspaceViewModel)
    {
      this.DataContext = notenWorkspaceViewModel;
      this.InitializeComponent();

      if (notenWorkspaceViewModel == null)
      {
        return;
      }

      if (notenWorkspaceViewModel.CurrentNote == null)
      {
        return;
      }

      switch (notenWorkspaceViewModel.CurrentNote.NoteNotentyp)
      {
        case Notentyp.MündlichQualität:
          this.QualitätButton.IsChecked = true;
          break;
        case Notentyp.MündlichQuantität:
          this.QuantitätButton.IsChecked = true;
          break;
        case Notentyp.MündlichSonstige:
          this.WeitereMündlichButton.IsChecked = true;
          break;
        case Notentyp.SchriftlichSonstige:
          this.WeitereSchriftlichButton.IsChecked = true;
          break;
      }
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

    /// <summary>
    /// Handles the OnChecked event of the QualitätButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void QualitätButton_OnChecked(object sender, RoutedEventArgs e)
    {
      var notenWorkspace = this.DataContext as NotenWorkspaceViewModel;
      if (notenWorkspace != null)
      {
        notenWorkspace.CurrentNote.NoteNotentyp = Notentyp.MündlichQualität;
        notenWorkspace.CurrentNote.NoteIstSchriftlich = false;
      }
    }

    /// <summary>
    /// Handles the OnChecked event of the QuantitätButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void QuantitätButton_OnChecked(object sender, RoutedEventArgs e)
    {
      var notenWorkspace = this.DataContext as NotenWorkspaceViewModel;
      if (notenWorkspace != null)
      {
        notenWorkspace.CurrentNote.NoteNotentyp = Notentyp.MündlichQuantität;
        notenWorkspace.CurrentNote.NoteIstSchriftlich = false;
      }
    }

    /// <summary>
    /// Handles the OnChecked event of the WeitereMündlichButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void WeitereMündlichButton_OnChecked(object sender, RoutedEventArgs e)
    {
      var notenWorkspace = this.DataContext as NotenWorkspaceViewModel;
      if (notenWorkspace != null)
      {
        notenWorkspace.CurrentNote.NoteNotentyp = Notentyp.MündlichSonstige;
        notenWorkspace.CurrentNote.NoteIstSchriftlich = false;
      }
    }

    /// <summary>
    /// Handles the OnChecked event of the WeitereSchriftlichButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void WeitereSchriftlichButton_OnChecked(object sender, RoutedEventArgs e)
    {
      var notenWorkspace = this.DataContext as NotenWorkspaceViewModel;
      if (notenWorkspace != null)
      {
        notenWorkspace.CurrentNote.NoteNotentyp = Notentyp.SchriftlichSonstige;
        notenWorkspace.CurrentNote.NoteIstSchriftlich = true;
      }
    }
  }
}