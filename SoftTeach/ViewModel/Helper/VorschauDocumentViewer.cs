namespace SoftTeach.ViewModel.Helper
{
  using System.Printing;
  using System.Windows.Controls;
  using System.Windows.Documents;

  /// <summary>
  /// Eine Abwandlung des DocumentViewers, welche die korrekten Einstellungen der Vorschau übernimmt
  /// </summary>
  public class VorschauDocumentViewer : DocumentViewer
  {
    /// <summary>
    /// Handles the <see cref="P:System.Windows.Input.ApplicationCommands.Print" /> routed command.
    /// </summary>
    protected override void OnPrintCommand()
    {
      // get a print dialog, defaulted to default printer and default printer's preferences.
      var printDialog = new PrintDialog
      {
        PrintQueue = LocalPrintServer.GetDefaultPrintQueue(),
        UserPageRangeEnabled = true,
        CurrentPageEnabled = true,
      };
      printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;

      // get a reference to the FixedDocumentSequence for the viewer.
      var docSeq = this.Document as FixedDocumentSequence;

      if (printDialog.ShowDialog() == true)
      {
        // set the print ticket for the document sequence and write it to the printer.
        docSeq.PrintTicket = printDialog.PrintTicket;
        DocumentPaginator paginator = docSeq.DocumentPaginator;

        if (printDialog.PageRangeSelection == PageRangeSelection.UserPages)
        {
          paginator = new PageRangeDocumentPaginator(
                           docSeq.DocumentPaginator,
                           printDialog.PageRange);
        }

        printDialog.PrintDocument(paginator, "BESS-Ausdruck");
      }

      //if (printDialog.ShowDialog() == true)
      //{
      //  // set the print ticket for the document sequence and write it to the printer.
      //  docSeq.PrintTicket = printDialog.PrintTicket;

      //  XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
      //  writer.WriteAsync(docSeq, printDialog.PrintTicket);
      //}
    }
  }
}
