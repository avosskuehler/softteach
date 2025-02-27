﻿// <copyright file="AddArbeitDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Datenbank
{
  using System.Linq;
  using System.Windows;

  using SoftTeach.ViewModel.Helper;

  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.Model.TeachyModel;
  using System.Windows.Documents;
  using SoftTeach.ViewModel.Termine;
  using System.Collections.Generic;

  /// <summary>
  /// Ein Dialog um nicht gemachte Arbeiten einzutragen.
  /// </summary>
  public partial class CleanupDatabaseDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="CleanupDatabaseDialog"/> Klasse. 
    /// </summary>
    public CleanupDatabaseDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
    }

    #endregion

    /// <summary>
    /// Holt oder setzt den Schuljahr für die Arbeit
    /// </summary>
    public SchuljahrViewModel Schuljahr { get; set; }

    /// <summary>
    /// Holt oder setzt den Halbjahr für die Arbeit
    /// </summary>
    public Halbjahr Halbjahr { get; set; }

    /// <summary>
    /// Holt oder setzt das Fach für die Arbeit
    /// </summary>
    public FachViewModel Fach { get; set; }


    /// <summary> The ok click. </summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="e"> The e. </param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    /// <summary> The cancel click. </summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="e"> The e. </param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    /// <summary>
    /// Deletes the schuljahr click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void DeleteSchuljahrClick(object sender, RoutedEventArgs e)
    {
      App.MainViewModel.Schuljahre.RemoveTest(this.Schuljahr);
      App.UnitOfWork.SaveChanges();
    }

    /// <summary>
    /// Deletes the fach click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void DeleteFachClick(object sender, RoutedEventArgs e)
    {
      App.MainViewModel.Fächer.RemoveTest(this.Fach);
      App.UnitOfWork.SaveChanges();
    }

    /// <summary>
    /// Deletes the personen click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void DeletePersonenClick(object sender, RoutedEventArgs e)
    {
      var personenUnused =
        App.MainViewModel.Personen.Where(o => o.PersonIstSchüler)
          .Where(personViewModel => !personViewModel.Model.Schülereinträge.Any()).ToList();

      foreach (var personViewModel in personenUnused)
      {
        App.MainViewModel.Personen.RemoveTest(personViewModel);
      }

      App.UnitOfWork.SaveChanges();
    }

    private void DeleteLeereStundenClick(object sender, RoutedEventArgs e)
    {
      App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;

      // Stunden ohne Stundenentwürfe löschen
      var leereStunden = App.UnitOfWork.Context.Termine.OfType<Stunde>().Where(o => !o.Phasen.Any()).ToList();

      App.UnitOfWork.Context.Termine.RemoveRange(leereStunden);

      App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;

      App.UnitOfWork.SaveChanges();
    }

    private void DeleteLeereCurriculaClick(object sender, RoutedEventArgs e)
    {
      App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;

      // Curricula ohne Reihen löschen
      var leereCurricula = App.UnitOfWork.Context.Curricula.Where(o => !o.Reihen.Any(a => a.Reihenfolge != -1)).ToList();

      App.UnitOfWork.Context.Curricula.RemoveRange(leereCurricula);

      App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;

      App.UnitOfWork.SaveChanges();
    }

    private void DeleteDoppelteStundenClick(object sender, RoutedEventArgs e)
    {
      App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;

      var zulöschendeStunden = new List<Lerngruppentermin>();

      // Von identischen, aber mehrfach vorhandenen Lerngruppentermine, die überzähligen löschen
      var stundenGruppen = App.UnitOfWork.Context.Termine.OfType<Lerngruppentermin>().GroupBy(o => o.Datum.ToString() + "#" + o.LerngruppeId + "#" + o.ErsteUnterrichtsstundeId).ToList().Where(o => o.Count() > 1);
      foreach (var gruppe in stundenGruppen)
      {
        if (gruppe.Count() == 1)
        {
          continue;
        }

        foreach (var einzelstunde in gruppe.Skip(1))
        {
          zulöschendeStunden.Add(einzelstunde);
        }
      }

      App.UnitOfWork.Context.Termine.RemoveRange(zulöschendeStunden);

      App.UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;

      App.UnitOfWork.SaveChanges();
    }
  }
}