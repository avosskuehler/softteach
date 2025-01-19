// <copyright file="ReiheDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Data;
  using SoftTeach.ViewModel.Curricula;
  using SoftTeach.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for ReiheDialog.xaml
  /// </summary>
  public partial class ReiheDialog
  {
    private ReiheViewModel reihe;

   /// <summary>
   /// Initialisiert eine neue Instanz der <see cref="ReiheDialog"/> Klasse. 
   /// </summary>
    public ReiheDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.ModulView = new ListCollectionView(App.MainViewModel.Module)
      {
        Filter = this.ModulFilter
      };
      this.ModulView.SortDescriptions.Add(new SortDescription("ModulBezeichnung", ListSortDirection.Ascending));
      this.ModulView.Refresh();
    }

    /// <summary>
    /// Holt oder setzt die gefilterten Module
    /// </summary>
    public ICollectionView ModulView { get; set; }

    /// <summary>
    /// Holt oder setzt das <see cref="ReiheViewModel"/> zur Bearbeitung
    /// </summary>
    public ReiheViewModel Reihe
    {
      get => this.reihe;
      set
      {
        this.reihe = value;
        this.ModulView.Refresh();
      }
    }

    /// <summary>
    /// Filtert die Terminliste nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Das TerminViewModel, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private bool ModulFilter(object item)
    {
      var modulViewModel = item as ModulViewModel;
      if (modulViewModel == null)
      {
        return false;
      }

      if (this.Reihe == null || this.Reihe.ReiheModul == null)
      {
        return false;
      }

      if (modulViewModel.ModulFach.FachBezeichnung != this.Reihe.ReiheModul.ModulFach.FachBezeichnung
        || modulViewModel.ModulJahrgang != this.Reihe.ReiheModul.ModulJahrgang)
      {
        return false;
      }

      return true;
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