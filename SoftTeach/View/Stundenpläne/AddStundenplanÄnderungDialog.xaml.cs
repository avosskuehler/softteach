// <copyright file="AddStundenplanÄnderungDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;

  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Stundenpläne;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// Interaction logic for AddStundenplanÄnderungDialog.xaml
  /// </summary>
  public partial class AddStundenplanÄnderungDialog
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AddStundenplanÄnderungDialog"/> class.
    /// </summary>
    public AddStundenplanÄnderungDialog(StundenplanViewModel stundenplanViewModel)
    {
      this.StundenplanViewModel = stundenplanViewModel;
      this.InitializeComponent();
      this.StundenplanViewModel.ViewMode |= StundenplanViewMode.DragDrop;
      this.StundenplanViewModel.ViewMode |= StundenplanViewMode.Edit;
      this.DataContext = stundenplanViewModel;
    }

    #endregion

    /// <summary>
    /// Holt den StundenplanViewModel.
    /// </summary>
    public StundenplanViewModel StundenplanViewModel { get; private set; }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      var dlg = new AskForJahresplanUpdateDialog();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        this.UpdateJahrespläne();
      }

      this.Close();
    }

    private void UpdateJahrespläne()
    {
      foreach (var änderung in this.StundenplanViewModel.ÄnderungsListe)
      {
        var jahresplanToChange =
          App.MainViewModel.Jahrespläne.FirstOrDefault(
            o =>
            o.JahresplanSchuljahr.SchuljahrJahr == änderung.ModifiedEntry.Model.Stundenplan.Schuljahr.Jahr
            && o.JahresplanFach.FachBezeichnung == änderung.ModifiedEntry.StundenplaneintragFach.FachBezeichnung
            && o.JahresplanKlasse.KlasseBezeichnung == änderung.ModifiedEntry.StundenplaneintragKlasse.KlasseBezeichnung);

        if (jahresplanToChange == null)
        {
          // Create a new Jahresplan
          App.MainViewModel.JahresplanWorkspace.AddJahresplan(
            this.StundenplanViewModel.StundenplanSchuljahr,
            änderung.ModifiedEntry.StundenplaneintragFach,
            änderung.ModifiedEntry.StundenplaneintragKlasse,
            this.StundenplanViewModel.StundenplanHalbjahr.HalbjahrBezeichnung == "Sommer");

          continue;
        }

        HalbjahresplanViewModel halbjahresplanToChange = null;
        switch (änderung.ModifiedEntry.Model.Stundenplan.Halbjahr.Bezeichnung)
        {
          case "Sommer":
            halbjahresplanToChange = jahresplanToChange.CurrentJahresplanSommerhalbjahr;
            break;
          case "Winter":
            halbjahresplanToChange = jahresplanToChange.CurrentJahresplanWinterhalbjahr;
            break;
        }

        if (halbjahresplanToChange == null)
        {
          continue;
        }

        for (var i = 0; i < halbjahresplanToChange.Monatspläne.Count; i++)
        {
          var monatsplanViewModel = halbjahresplanToChange.Monatspläne[i];

          var sortedTagesPläne = monatsplanViewModel.Tagespläne.OrderBy(o => o.TagesplanDatum).ToList();
          for (int j = 0; j < sortedTagesPläne.Count; j++)
          {
            var tagesplanViewModel = sortedTagesPläne[j];

            // Skip if the modified stundenplan is not valid
            if (tagesplanViewModel.TagesplanDatum < änderung.ModifiedEntry.Model.Stundenplan.GültigAb)
            {
              continue;
            }

            // Check for correct wochentag and ferien
            if (tagesplanViewModel.TagesplanWochentagIndex == änderung.OldWochentagIndex
              && !tagesplanViewModel.TagesplanFerientag)
            {
              var stundenToChange =
                tagesplanViewModel.Lerngruppentermine.Where(
                  o =>
                  o.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex
                  == änderung.OldErsteStundeIndex);

              // Check if there is any stunde to be moved
              var lerngruppenterminViewModels = stundenToChange as IList<LerngruppenterminViewModel> ?? stundenToChange.ToList();
              if (!lerngruppenterminViewModels.Any())
              {
                continue;
              }

              // Die Stunde im Jahresplan, die durch die Änderung betroffen ist
              // es darf nur eine sein...
              var stundeToChange = lerngruppenterminViewModels.First();
              var monthIndex = i;

              var indexOld = änderung.OldWochentagIndex;
              var indexNew = änderung.ModifiedEntry.StundenplaneintragWochentagIndex;
              var dayIndex = j - (indexOld - indexNew);

              if (indexOld > indexNew)
              {
                // stunde moved to earlier this week and lies in the preceding month
                if (dayIndex < 0)
                {
                  monthIndex--;
                  dayIndex = halbjahresplanToChange.Monatspläne[monthIndex].Tagespläne.Count + dayIndex;
                }
              }
              else
              {
                // stunde moved to later this week and lies in the next month
                if (dayIndex > monatsplanViewModel.Tagespläne.Count - 1)
                {
                  dayIndex = dayIndex - halbjahresplanToChange.Monatspläne[monthIndex].Tagespläne.Count;
                  monthIndex++;
                }
              }

              // Now care for the changetype
              switch (änderung.UpdateType)
              {
                case StundenplanÄnderungUpdateType.Added:
                  if (stundeToChange is StundeViewModel)
                  {
                    var tagesplanToMoveTo = halbjahresplanToChange.Monatspläne[monthIndex].Tagespläne.OrderBy(o => o.TagesplanDatum).ElementAt(dayIndex);
                    tagesplanToMoveTo.AddStunde(stundeToChange as StundeViewModel);
                    tagesplanToMoveTo.UpdateBeschreibung();
                  }

                  break;
                case StundenplanÄnderungUpdateType.Removed:
                  tagesplanViewModel.DeleteLerngruppentermin(stundeToChange);
                  tagesplanViewModel.UpdateBeschreibung();
                  break;
                case StundenplanÄnderungUpdateType.ChangedKlasse:
                case StundenplanÄnderungUpdateType.ChangedRoom:
                  // Do nothing
                  break;
                case StundenplanÄnderungUpdateType.ChangedTimeSlot:
                  stundeToChange.TerminErsteUnterrichtsstunde =
                    App.MainViewModel.Unterrichtsstunden[änderung.ModifiedEntry.StundenplaneintragErsteUnterrichtsstundeIndex - 1];
                  stundeToChange.TerminLetzteUnterrichtsstunde =
                    App.MainViewModel.Unterrichtsstunden[änderung.ModifiedEntry.StundenplaneintragLetzteUnterrichtsstundeIndex - 1];

                  // Wenn auch der Unterrichtstag gewechselt wurde
                  // Stunde verschieben
                  if (dayIndex != j)
                  {
                    if (stundeToChange is StundeViewModel)
                    {
                      var tagesplanToMoveTo = halbjahresplanToChange.Monatspläne[monthIndex].Tagespläne.OrderBy(o => o.TagesplanDatum).ElementAt(dayIndex);
                      tagesplanToMoveTo.AddStundeToTagesplan(stundeToChange as StundeViewModel);
                    }

                    tagesplanViewModel.RemoveStundeFromTagesplan(stundeToChange);
                  }

                  tagesplanViewModel.UpdateBeschreibung();

                  break;
              }
            }
          }
        }
      }
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}