namespace SoftTeach.ViewModel.Helper
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Windows.Forms;
  using System.Windows.Navigation;

  using Microsoft.Office.Interop.Excel;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// Enthält Hilfsfunktionen um Daten in andere Programme zu exportieren.
  /// </summary>
  public class ExportData
  {
    /// <summary>
    /// Exportiert eine Schülerliste nach Excel
    /// </summary>
    /// <param name="viewModel">Das view model der Schülerliste.</param>
    public static void ToXls(ViewModelBase viewModel)
    {
      try
      {
        var xla = new Microsoft.Office.Interop.Excel.Application { Visible = true };

        xla.Workbooks.Add(XlSheetType.xlWorksheet);

        var ws = (Worksheet)xla.ActiveSheet;

        var row = 1;

        if (viewModel is SchülerlisteViewModel)
        {
          var schülerliste = viewModel as SchülerlisteViewModel;
          ws.Cells[row, 1] = "Vorname";
          ws.Cells[row, 2] = "Nachname";
          row++;
          foreach (var person in schülerliste.Schülereinträge.OrderBy(o => o.SchülereintragPerson.PersonVorname))
          {
            ws.Cells[row, 1] = person.SchülereintragPerson.PersonVorname;
            ws.Cells[row, 2] = person.SchülereintragPerson.PersonNachname;
            row++;
          }
        }
      }
      catch (Exception ex)
      {
        Log.HandleException(ex);
      }
    }

    /// <summary>
    /// Importiert personen aus einer CSV Datei.
    /// </summary>
    /// <returns>Eine Liste der Personen</returns>
    public static List<PersonViewModel> FromCSV()
    {
      var returnList = new List<PersonViewModel>();
      var ofd = new OpenFileDialog
      {
        CheckFileExists = true,
        CheckPathExists = true,
        AutoUpgradeEnabled = true,
        //InitialDirectory = Configuration.GetMyDocumentsPath(),
        Multiselect = false,
        Title = "Bitte Datei auswählen"
      };

      if (ofd.ShowDialog() == DialogResult.OK)
      {
        int foundCounter = 0;
        int newCounter = 0;

        var enc = Encoding.GetEncoding(1252);
        using (var streamReader = new StreamReader(ofd.FileName, enc))
        {
          // In der ersten Zeile sind Überschriften
          var line = streamReader.ReadLine();

          // Read and display lines from the file until the end of  
          // the file is reached. 
          while ((line = streamReader.ReadLine()) != null)
          {
            var items = line.Split('\t');
            var nr = items[0];
            var nachname = items[1];
            var vorname = items[2];
            var geschlecht = items[3] == "w";
            var geburtstag = DateTime.Parse(items[4]);

            var existiert =
              App.MainViewModel.Personen.FirstOrDefault(
                o => o.PersonIstWeiblich == geschlecht && o.PersonVorname == vorname && o.PersonNachname == nachname);

            if (existiert != null)
            {
              existiert.PersonGeburtstag = geburtstag;
              returnList.Add(existiert);
              foundCounter++;
            }
            else
            {
              var person = new Person();
              person.Nachname = nachname;
              person.Vorname = vorname;
              person.Geschlecht = geschlecht;
              person.Geburtstag = geburtstag;
              var vm = new PersonViewModel(person);
              App.MainViewModel.Personen.Add(vm);
              returnList.Add(vm);
              newCounter++;
            }
          }
        }

        InformationDialog.Show(
          "Neue Personen",
          string.Format("{0} neue Personen angelegt und {1} bestehende Personen gefunden.", newCounter, foundCounter),
          false);
      }

      return returnList;
    }
  }
}
