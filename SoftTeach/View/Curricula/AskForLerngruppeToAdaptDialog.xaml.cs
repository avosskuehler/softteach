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

namespace SoftTeach.View.Curricula
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

  /// <summary>
  /// Interaction logic for AskForLerngruppeToAdaptDialog.xaml
  /// </summary>
  public partial class AskForLerngruppeToAdaptDialog
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AskForLerngruppeToAdaptDialog"/> Klasse. 
    /// </summary>
    /// <param name="fachViewModel"> The fach View Model.  </param>
    /// <param name="jahrgang">Der Jahrgang</param>
    public AskForLerngruppeToAdaptDialog(
      FachViewModel fachViewModel,
      int jahrgang)
    {
      this.InitializeComponent();
      this.DataContext = this;

      this.FilteredLerngruppen = App.UnitOfWork.Context.Lerngruppen.Where(o => o.FachId == fachViewModel.Model.Id && o.Jahrgang == jahrgang).OrderBy(o => o.Schuljahr.Jahr).ToObservableCollection();
    }

    /// <summary>
    /// Holt alle möglichen Lerngruppen mit gleichem Fach und Jahrgang
    /// </summary>
    public ObservableCollection<Lerngruppe> FilteredLerngruppen { get; private set; }

    /// <summary>
    /// Holt oder setzt die ausgewählte Lerngruppe
    /// </summary>
    public Lerngruppe SelectedLerngruppe { get; set; }


    /// <summary>
    /// Holt oder setzt den Titel des Dialogs.
    /// </summary>
    public new string Title
    {
      get
      {
        return this.Header.Title;
      }
      set
      {
        this.Header.Title = value;
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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (!this.FilteredLerngruppen.Any())
      {
        InformationDialog.Show(
          "Nicht gefunden",
          "Es wurde kein passender Jahresplan zur Anpassung gefunden. Bitte"
          + " erstellen Sie zunächst einen für dieses Fach und diese Klassenstufe", false);
        this.DialogResult = false;
        this.Close();
      }
      else
      {
        this.LerngruppeCombo.SelectedItem = this.FilteredLerngruppen[0];
      }
    }
  }
}