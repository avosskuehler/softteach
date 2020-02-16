// <copyright file="AskForTerminSpanDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Termine
{
  using System;
  using System.Windows;

  /// <summary>
  /// Interaction logic for AskForStundeneigenschaftenDialog.xaml
  /// </summary>
  public partial class AskForStundeneigenschaftenDialog
  {

    public string Stundenthema
    {
      get
      {
        return this.StundenThema.Text;
      }
      set
      {
        this.StundenThema.Text = value;
      }
    }

    public int Stundenzahl
    {
      get
      {
        return (int)this.Stundenanzahl.Value;
      }
      set
      {
        this.Stundenanzahl.Value = value;
      }
    }

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AskForStundeneigenschaftenDialog"/> class.
    /// </summary>
    public AskForStundeneigenschaftenDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }
  }
}