// <copyright file="EditStundenplanDialog.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
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
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Stundenpläne;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// Interaction logic for EditStundenplanDialog.xaml
  /// </summary>
  public partial class EditStundenplanDialog
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EditStundenplanDialog"/> class.
    /// </summary>
    public EditStundenplanDialog(StundenplanViewModel stundenplan)
    {
      this.StundenplanViewModel = stundenplan;
      this.StundenplanViewModel.ViewMode = StundenplanViewMode.Default | StundenplanViewMode.Edit | StundenplanViewMode.DragDrop;
      this.InitializeComponent();
      this.DataContext = this.StundenplanViewModel;
    }

    /// <summary>
    /// Holt den StundenplanViewModel.
    /// </summary>
    public StundenplanViewModel StundenplanViewModel { get; private set; }

    private void UpdateJahrespläne()
    {
      var geänderteLerngruppen = new List<LerngruppeViewModel>();

      foreach (var änderung in this.StundenplanViewModel.ÄnderungsListe)
      {
        var lerngruppeToChange = App.MainViewModel.Lerngruppen.FirstOrDefault(o => o == änderung.ModifiedEntry.StundenplaneintragLerngruppe);

        if (lerngruppeToChange == null)
        {
          continue;
        }

        if (!geänderteLerngruppen.Contains(lerngruppeToChange))
        {
          geänderteLerngruppen.Add(lerngruppeToChange);
        }

        // Eine e Unterrichtsstunde wurde ergänzt, also für alle Wochen.
        if (änderung.UpdateType == StundenplanÄnderungUpdateType.Added)
        {
          var endeMonat = this.StundenplanViewModel.StundenplanHalbjahr == Halbjahr.Winter ? 2 : 8;
          var endeSchuljahr = new DateTime(this.StundenplanViewModel.StundenplanHalbjahr == Halbjahr.Winter ? this.StundenplanViewModel.StundenplanSchuljahr.SchuljahrJahr : this.StundenplanViewModel.StundenplanSchuljahr.SchuljahrJahr + 1, endeMonat, 1);
          var startdatum = änderung.ModifiedEntry.Model.Stundenplan.GültigAb.StartOfWeek();
          startdatum = startdatum.AddDays(änderung.ModifiedEntry.StundenplaneintragWochentagIndex);
          while (startdatum < endeSchuljahr)
          {
            var stunde = new Stunde
            {
              ErsteUnterrichtsstunde =
              App.MainViewModel.Unterrichtsstunden[änderung.ModifiedEntry.StundenplaneintragErsteUnterrichtsstundeIndex - 1].Model,
              LetzteUnterrichtsstunde =
              App.MainViewModel.Unterrichtsstunden[änderung.ModifiedEntry.StundenplaneintragLetzteUnterrichtsstundeIndex - 1].Model,

              Datum = startdatum,
              Termintyp = Model.TeachyModel.Termintyp.Unterricht,
              Lerngruppe = änderung.ModifiedEntry.StundenplaneintragLerngruppe.Model,
              Hausaufgaben = string.Empty,
              Ansagen = string.Empty,
              Jahrgang = änderung.ModifiedEntry.StundenplaneintragLerngruppe.LerngruppeJahrgang,
              Fach = änderung.ModifiedEntry.StundenplaneintragLerngruppe.LerngruppeFach.Model,
              Halbjahr = this.StundenplanViewModel.StundenplanHalbjahr,
              Ort = änderung.ModifiedEntry.StundenplaneintragRaum.RaumBezeichnung
            };

            var vm = new StundeViewModel(stunde);
            if (!lerngruppeToChange.Lerngruppentermine.Any(o => o.LerngruppenterminDatum == startdatum && o.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex == änderung.ModifiedEntry.StundenplaneintragErsteUnterrichtsstundeIndex)
              && !App.MainViewModel.Ferien.Any(o => o.FerienSchuljahr == this.StundenplanViewModel.StundenplanSchuljahr && o.FerienErsterFerientag < startdatum && o.FerienLetzterFerientag > startdatum))
            {
              lerngruppeToChange.Lerngruppentermine.Add(vm);
            }
            startdatum = startdatum.AddDays(7);
          }
          continue;
        }

        var stunden = lerngruppeToChange.Lerngruppentermine.OfType<StundeViewModel>().OrderBy(o => o.LerngruppenterminDatum).ToList();

        for (int j = 0; j < stunden.Count(); j++)
        {
          var stundeViewModel = stunden[j];

          // Wenn Stunde früher als Beginn der Stundenplanänderung, dann ignorieren.
          if (stundeViewModel.LerngruppenterminDatum < änderung.ModifiedEntry.Model.Stundenplan.GültigAb)
          {
            continue;
          }

          // Wenn Stunde nicht am alten Wochentag liegt, dann ignorieren
          if ((int)stundeViewModel.LerngruppenterminDatum.DayOfWeek != änderung.OldWochentagIndex)
          {
            continue;
          }

          // Wenn Stunde ein Ferientermin ist, dann ignorieren
          if (stundeViewModel.TerminTermintyp == Termintyp.Ferien)
          {
            continue;
          }

          // Wenn Stunde nicht an richtiger Tageszeit liegt, dann ignorieren
          if (stundeViewModel.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex != änderung.OldErsteStundeIndex)
          {
            continue;
          }

          // Hier müsste es die Stunde der Lerngruppe sein, die durch die Änderung betroffen ist
          var indexOld = änderung.OldWochentagIndex;
          var indexNew = änderung.ModifiedEntry.StundenplaneintragWochentagIndex;
          var dayMove = indexNew- indexOld;

          // Now care for the changetype
          switch (änderung.UpdateType)
          {
            case StundenplanÄnderungUpdateType.Added:
              // PullStunden ...
              break;
            case StundenplanÄnderungUpdateType.Removed:
              stundeViewModel.DeleteTerminCommand.Execute(null);
              break;
            case StundenplanÄnderungUpdateType.ChangedKlasse:
            case StundenplanÄnderungUpdateType.ChangedRoom:
              stundeViewModel.TerminOrt = änderung.ModifiedEntry.StundenplaneintragRaum.RaumBezeichnung;
              stundeViewModel.LerngruppenterminLerngruppe = änderung.ModifiedEntry.StundenplaneintragLerngruppe;
              break;
            case StundenplanÄnderungUpdateType.ChangedTimeSlot:
              stundeViewModel.TerminErsteUnterrichtsstunde =
                App.MainViewModel.Unterrichtsstunden[änderung.ModifiedEntry.StundenplaneintragErsteUnterrichtsstundeIndex - 1];
              stundeViewModel.TerminLetzteUnterrichtsstunde =
                App.MainViewModel.Unterrichtsstunden[änderung.ModifiedEntry.StundenplaneintragLetzteUnterrichtsstundeIndex - 1];

              // Wenn auch der Unterrichtstag gewechselt wurde
              // Stunde verschieben
              if (dayMove != 0)
              {
                stundeViewModel.LerngruppenterminDatum = stundeViewModel.LerngruppenterminDatum.AddDays(dayMove);
              }

              break;
          }
        }
      }

      foreach (var lg in geänderteLerngruppen)
      {
        var jahresplan = App.MainViewModel.Jahrespläne.FirstOrDefault(o => o.Lerngruppe == lg);
        if (jahresplan != null)
        {
          jahresplan.KalenderErstellen();
        }
      }
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      //var dlg = new AskForJahresplanCreateDialog();
      //if (dlg.ShowDialog().GetValueOrDefault(false))
      //{
      //  this.CreateJahrespläne();
      //}
      var dlg = new AskForJahresplanUpdateDialog();
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        this.UpdateJahrespläne();
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