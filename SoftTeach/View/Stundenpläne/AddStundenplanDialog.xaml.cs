// <copyright file="AddStundenplanDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Stundenpläne;

  /// <summary>
  /// Interaction logic for AddStundenplanDialog.xaml
  /// </summary>
  public partial class AddStundenplanDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AddStundenplanDialog"/> class.
    /// </summary>
    public AddStundenplanDialog(SchuljahrViewModel schuljahr, Halbjahr halbschuljahr, DateTime gültigAb)
    {
      App.MainViewModel.StundenplanWorkspace.AddStundenplan(schuljahr, halbschuljahr, gültigAb);
      this.StundenplanViewModel = App.MainViewModel.StundenplanWorkspace.CurrentStundenplan;
      this.StundenplanViewModel.ViewMode = StundenplanViewMode.Default | StundenplanViewMode.Edit;
      this.InitializeComponent();
      this.DataContext = this.StundenplanViewModel;
    }

    #endregion

    /// <summary>
    /// Holt den StundenplanViewModel.
    /// </summary>
    public StundenplanViewModel StundenplanViewModel { get; private set; }

    /// <summary>
    /// Diese Methode erstellt Jahrespläne für alle im Stundenplan eingetragenen Fächer und Klassen
    /// </summary>
    private void CreateJahrespläne()
    {
      this.Cursor = Cursors.Wait;
      using (new UndoBatch(App.MainViewModel, string.Format("Jahrespläne für Stundenplan neu angelegt"), false))
      {
        // TODO
        //foreach (var stundenplaneintragViewModel in this.StundenplanViewModel.Stundenplaneinträge)
        //{
        //  // Create a new Jahresplan
        //  App.MainViewModel.JahresplanWorkspace.AddJahresplan(
        //    this.StundenplanViewModel.StundenplanSchuljahr,
        //    stundenplaneintragViewModel.StundenplaneintragFach,
        //    stundenplaneintragViewModel.StundenplaneintragLerngruppe,
        //    this.StundenplanViewModel.StundenplanHalbjahr.HalbjahrBezeichnung == "Sommer");
        //}

        //// Create special jahresplan for vertretungsstunden etc.
        //App.MainViewModel.JahresplanWorkspace.AddJahresplan(
        //  this.StundenplanViewModel.StundenplanSchuljahr,
        //  App.MainViewModel.Fächer.Single(o => o.FachBezeichnung == "Vertretungsstunden"),
        //  App.MainViewModel.Klassen.Single(o => o.KlasseBezeichnung == "Alle"),
        //  this.StundenplanViewModel.StundenplanHalbjahr.HalbjahrBezeichnung == "Sommer");
      }

      this.Cursor = Cursors.Arrow;
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      var dlg = new AskForJahresplanCreateDialog();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        this.CreateJahrespläne();
      }

      this.Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}