// <copyright file="AskForSchuljahr.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for CreateLerngruppenterminDialog.xaml
  /// </summary>
  public partial class AskForSchuljahr
  {
    public SchuljahrViewModel Schuljahr
    {
      get
      {
        return (SchuljahrViewModel)this.SchuljahrCombo.SelectedItem;
      }
    }

    public Halbjahr Halbjahr
    {
      get
      {
        return (Halbjahr)this.HalbjahrCombo.SelectedItem;
      }
    }

    public DateTime GültigAb
    {
      get
      {
        return this.GültigAbDate.SelectedDate.GetValueOrDefault(DateTime.Now);
      }
    }


    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AskForSchuljahr"/> class.
    /// </summary>
    public AskForSchuljahr()
    {
      this.InitializeComponent();
      this.SchuljahrCombo.SelectedItem = Selection.Instance.Schuljahr;
      this.HalbjahrCombo.SelectedItem = Selection.Instance.Halbjahr;
      this.GültigAbDate.SelectedDate = DateTime.Now;
    }

    #endregion

    private void OkClick(object sender, RoutedEventArgs e)
    {
      if (
        App.MainViewModel.Stundenpläne.Any(
          o => o.StundenplanSchuljahr == this.Schuljahr && o.StundenplanHalbjahr == this.Halbjahr))
      {
        new InformationDialog(
          "Stundenplan existiert bereits",
          "Für dieses Schulhalbjahr wurde bereits "
          +
          "ein Stundenplan angelegt. Wenn Sie eine Änderung eintragen wollen, nutzen Sie bitte die entsprechende Funktion.",
          false).ShowDialog();
      }
      else
      {
        this.DialogResult = true;
        this.Close();
      }
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}