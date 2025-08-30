namespace SoftTeach.ViewModel.Helper
{
  using System.Windows.Documents;
  using System.Windows.Input;

  /// <summary>
  /// Interaction logic for PreviewFixedDocument.xaml
  /// </summary>
  public partial class PreviewFixedDocument
  {
    public PreviewFixedDocument()
    {
      this.InitializeComponent();
    }

    public IDocumentPaginatorSource Document
    {
      get => this.DocumentPreview.Document;
      set => this.DocumentPreview.Document = value;
    }

    private void CommandBindingCanExecuteCopy(object sender, CanExecuteRoutedEventArgs e)
    {
      // Disable copy button
      e.Handled = true;
    }
  }
}

