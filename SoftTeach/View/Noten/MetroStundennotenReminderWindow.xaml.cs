// <copyright file="MetroStundennotenPageNew.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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

namespace SoftTeach.View.Noten
{
  using SoftTeach.ViewModel.Noten;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Ein Dialog um Noten für die Stunde einzutragen.
  /// </summary>
  public partial class MetroStundennotenReminderWindow
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroStundennotenReminderWindow"/> Klasse.
    /// </summary>
    public MetroStundennotenReminderWindow()
    {
      this.InitializeComponent();
    }

    #endregion

    /// <summary>
    /// Handles the OnSelectionChanged event of the StundenListBox control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
    private void StundenListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.StundenListBox.Items.Count == 1)
      {
        this.StundenListBox.Visibility = Visibility.Collapsed;
        return;
      }

      if (this.StundenListBox.Items.Count == 0)
      {
        this.Close();
      }
    }

    private void ResetFlyout()
    {
      this.Qualität1Radio.IsChecked = false;
      this.Qualität2Radio.IsChecked = false;
      this.Qualität3Radio.IsChecked = false;
      this.Qualität4Radio.IsChecked = false;
      this.Qualität5Radio.IsChecked = false;
      this.Qualität6Radio.IsChecked = false;
      this.Quantität1Radio.IsChecked = false;
      this.Quantität2Radio.IsChecked = false;
      this.Quantität3Radio.IsChecked = false;
      this.Quantität4Radio.IsChecked = false;
      this.Quantität5Radio.IsChecked = false;
      this.Quantität6Radio.IsChecked = false;

      var s = this.DataContext as SchülereintragViewModel;
      if (s != null)
      {
        s.IstZufälligAusgewählt = false;
      }
    }

    private void RadioOnClick(object sender, RoutedEventArgs e)
    {
      this.ResetFlyout();
    }

    private void NotenListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.ResetFlyout();
    }
  }
}