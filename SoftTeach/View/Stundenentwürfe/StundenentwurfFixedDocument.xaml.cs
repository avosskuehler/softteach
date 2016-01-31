
namespace SoftTeach.View.Stundenentwürfe
{
  using System.Windows.Documents;

  /// <summary>
  /// Interaction logic for StundenentwurfFixedDocument.xaml
  /// </summary>
  public partial class StundenentwurfFixedDocument
  {
    public StundenentwurfFixedDocument()
    {
      InitializeComponent();
    }

    public IDocumentPaginatorSource Document
    {
      get { return documentViewer.Document; }
      set { documentViewer.Document = value; }
    }

    private void CommandBindingCanExecuteCopy(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
      // Disable copy button
      e.Handled = true;
    }
  }
}
