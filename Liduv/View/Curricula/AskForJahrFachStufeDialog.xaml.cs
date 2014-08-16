// <copyright file="AskForJahrFachStufeDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace Liduv.View.Curricula
{
  using System.Windows;

  using Liduv.Setting;
  using Liduv.View.Stundenpläne;
  using Liduv.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for AskForJahrFachStufeDialog.xaml
  /// </summary>
  public partial class AskForJahrFachStufeDialog
  {
    public JahrtypViewModel Jahrtyp { get; set; }
    public HalbjahrtypViewModel Halbjahrtyp { get; set; }
    public FachViewModel Fach { get; set; }
    public KlassenstufeViewModel Klassenstufe { get; set; }
    public string Bezeichnung { get; set; }

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AskForGültigAb"/> class.
    /// </summary>
    public AskForJahrFachStufeDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.Jahrtyp = Selection.Instance.Jahrtyp;
      this.Halbjahrtyp = Selection.Instance.Halbjahr;
      this.Fach = App.MainViewModel.Fächer[0];
      this.Klassenstufe = App.MainViewModel.Klassenstufen[0];
      this.Bezeichnung = "Neue Bezeichnung";
    }

    #endregion

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