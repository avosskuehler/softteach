using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Liduv.ViewModel.Personen;
using Microsoft.Office.Interop.Excel;
using Liduv.ExceptionHandling;

namespace Liduv.ViewModel.Helper
{
  /// <summary>
  /// Enthält Hilfsfunktionen um Daten in andere Programme zu exportieren.
  /// </summary>
  public class ExportData
  {
    /// <summary>
    /// The to xls.
    /// </summary>
    /// <param name="dataSource">
    /// The data source. 
    /// </param>
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
  }
}
