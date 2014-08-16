// <copyright file="AskForSchuljahr.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
// Liduv - Lehrerunterrichtsdatenbank
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

namespace Liduv.View.Stundenpläne
{
  using System;
  using System.Linq;
  using System.Windows;

  using Liduv.ExceptionHandling;
  using Liduv.ViewModel.Datenbank;
  using Liduv.Setting;

  /// <summary>
  /// Interaction logic for CreateLerngruppenterminDialog.xaml
  /// </summary>
  public partial class AskForSchuljahr
  {
    public JahrtypViewModel Jahrtyp
    {
      get
      {
        return (JahrtypViewModel)this.JahrtypCombo.SelectedItem;
      }
    }

    public HalbjahrtypViewModel Halbjahrtyp
    {
      get
      {
        return (HalbjahrtypViewModel)this.HalbjahrtypCombo.SelectedItem;
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
      this.JahrtypCombo.SelectedItem = Selection.Instance.Jahrtyp;
      this.HalbjahrtypCombo.SelectedItem = Selection.Instance.Halbjahr;
      this.GültigAbDate.SelectedDate = DateTime.Now;
    }

    #endregion

    private void OkClick(object sender, RoutedEventArgs e)
    {
      if (
        App.MainViewModel.Stundenpläne.Any(
          o => o.StundenplanJahrtyp == this.Jahrtyp && o.StundenplanHalbjahrtyp == this.Halbjahrtyp))
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