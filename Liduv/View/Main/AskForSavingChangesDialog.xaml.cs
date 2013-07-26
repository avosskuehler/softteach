namespace Liduv.View.Main
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for InformationDialog.xaml
  /// </summary>
  public partial class AskForSavingChangesDialog : Window
  {
    public bool? Result { get; set; }

    /// <summary>
    /// Initializes a new instance of the AskForSavingChangesDialog class.
    /// </summary>
    public AskForSavingChangesDialog()
    {
      this.InitializeComponent();
    }

    private void JaButtonClick(object sender, RoutedEventArgs e)
    {
      this.Result = true;
      this.Close();
    }

    private void NeinButtonClick(object sender, RoutedEventArgs e)
    {
      this.Result = false;
      this.Close();
    }

    private void AbbruchButtonClick(object sender, RoutedEventArgs e)
    {
      this.Result = null;
      this.Close();
    }
  }
}
