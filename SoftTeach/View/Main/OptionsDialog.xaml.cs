namespace SoftTeach.View.Main
{
  using System.Windows;

  using SoftTeach.Setting;

  /// <summary>
  /// Interaction logic for MessageDialog.xaml
  /// </summary>
  public partial class OptionsDialog
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="OptionsDialog"/> Klasse. 
    /// </summary>
    public OptionsDialog()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Der event handler für den OK und Ja Button. Setzt DialogResult=true
    /// und beendet den Dialog.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void SaveButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      Configuration.Serialize();
      this.Close();
    }

    /// <summary>
    /// Der event handler für den Nein Button. Setzt DialogResult=false
    /// und beendet den Dialog.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void CancelButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      Configuration.Deserialize();
      this.Close();
    }
  }
}
