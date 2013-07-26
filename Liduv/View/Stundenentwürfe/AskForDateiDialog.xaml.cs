// <copyright file="AskForDatei.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace Liduv.View.Stundenentwürfe
{
  using System.Linq;
  using System.Windows;
  using System.Windows.Forms;
  using Liduv.Setting;
  using Liduv.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for CreateLerngruppenterminDialog.xaml
  /// </summary>
  public partial class AskForDateiDialog
  {
    public DateitypViewModel Dateityp
    {
      get
      {
        return (DateitypViewModel)this.DateitypCombo.SelectedItem;
      }
    }

    public string DateinameMitPfad
    {
      get
      {
        return this.Dateiname.Text;
      }
    }


    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AskForDatei"/> class.
    /// </summary>
    public AskForDateiDialog()
    {
      this.InitializeComponent();
      this.DateitypCombo.SelectedItem = App.MainViewModel.Dateitypen.First();
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

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      var ofd = new OpenFileDialog
        {
          CheckFileExists = true,
          CheckPathExists = true,
          AutoUpgradeEnabled = true,
          //InitialDirectory = Configuration.GetMyDocumentsPath(),
          Multiselect = false,
          Title = "Bitte Datei auswählen"
        };

      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        this.Dateiname.Text = ofd.FileName;
      }
      else
      {
        this.DialogResult = false;
        this.Close();
      }
    }
  }
}