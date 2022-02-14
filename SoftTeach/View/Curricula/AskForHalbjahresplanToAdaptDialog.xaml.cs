// <copyright file="AskForHalbjahresplanToAdaptDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Jahrespläne;

  /// <summary>
  /// Interaction logic for AskForHalbjahresplanToAdaptDialog.xaml
  /// </summary>
  public partial class AskForHalbjahresplanToAdaptDialog
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AskForHalbjahresplanToAdaptDialog"/> Klasse. 
    /// </summary>
    /// <param name="fachViewModel"> The fach View Model.  </param>
    /// <param name="klassenstufeViewModel"> The klassenstufe View Model.  </param>
    /// <param name="halbschuljahrViewModel"> The halbschuljahr View Model. </param>
    public AskForHalbjahresplanToAdaptDialog(
      FachViewModel fachViewModel,
      int klassenstufeViewModel,
      Halbjahr halbschuljahrViewModel)
    {
      this.InitializeComponent();
      this.DataContext = this;
      var filteredHalbjahrespläne = App.MainViewModel.Jahrespläne.SelectMany(a => a.Halbjahrespläne.Where(
                o =>
          o.HalbjahresplanFach == fachViewModel
          && o.HalbjahresplanKlasse.Model.Klassenstufe == klassenstufeViewModel.Model
          && o.HalbjahresplanHalbjahr == halbschuljahrViewModel));
      this.FilteredHalbjahrespläne = new ObservableCollection<HalbjahresplanViewModel>();
      foreach (var halbjahresplanViewModel in filteredHalbjahrespläne)
      {
        this.FilteredHalbjahrespläne.Add(halbjahresplanViewModel);
      }
    }

    public HalbjahresplanViewModel Halbjahresplan { get; set; }

    public ObservableCollection<HalbjahresplanViewModel> FilteredHalbjahrespläne { get; private set; }

    /// <summary>
    /// Holt oder setzt den Titel des Dialogs.
    /// </summary>
    public string Title
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
      if (!this.FilteredHalbjahrespläne.Any())
      {
        InformationDialog.Show(
          "Nicht gefunden",
          "Es wurde kein passender Halbjahresplan zur Anpassung gefunden. Bitte"
          + " erstellen Sie zunächst einen für dieses Fach und diese Klassenstufe", false);
        this.DialogResult = false;
        this.Close();
      }
      else
      {
        this.Halbjahresplan = this.FilteredHalbjahrespläne[0];
      }
    }
  }
}