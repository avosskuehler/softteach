namespace SoftTeach.ExceptionHandling
{
  using System;
  using System.Windows;
  using System.Text;
  using System.IO;
  using System.Windows.Documents;

  /// <summary>
  /// Interaction logic for ShowLogDialog.xaml
  /// </summary>
  public partial class ShowLogDialog : Window
  {
    /// <summary>
    /// Initializes a new instance of the ShowLogDialog class.
    /// The Constructor with the file to specify.
    /// </summary>
    /// <param name="file">A <see cref="string"/> with the full filename with path
    /// to the log file to show.</param>
    public ShowLogDialog(string file)
    {
      this.InitializeComponent();
      this.PopulateDialogWithFileContents(file);
    }

    /// <summary>
    /// Der event handler für den Send Button. Beendet den Dialog und sendet die Mail.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void SendButtonClick(object sender, RoutedEventArgs e)
    {
      StringBuilder mailtoStatement = new StringBuilder();
      mailtoStatement.Append("mailto:adrian.vosskuehler@physik.fu-berlin.de");
      mailtoStatement.Append("?subject=OGAMA%20error%20report");
      mailtoStatement.Append("&body=Please insert a copy of the logfile here by pressing Ctrl+V.");
      Hyperlink link = new Hyperlink();
      link.NavigateUri = new Uri(mailtoStatement.ToString());
      link.DoClick();
      this.Close();
    }

    /// <summary>
    /// Der event handler für den Close Button. Beendet den Dialog.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void CloseButtonClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Thies method populates the filename label and the file
    /// content text box with the contents of the given file.
    /// </summary>
    /// <param name="file">A <see cref="string"/> with the full filename with path
    /// to the log file to show.</param>
    private void PopulateDialogWithFileContents(string file)
    {
      this.FileNameLabel.Content = Path.GetFileName(file);
      if (File.Exists(file))
      {
        string backupLogFile = Path.Combine(Properties.Settings.Default.LogfilePath, "copyOfLog.log");
        File.Copy(file, backupLogFile, true);

        // Create an instance of StreamReader to read from a file.
        // The using statement also closes the StreamReader.
        using (StreamReader sr = new StreamReader(backupLogFile))
        {
          // Read and display lines from the file until the end of 
          // the file is reached.
          this.LogFileContent.Text = sr.ReadToEnd();
        }

        File.Delete(backupLogFile);
      }
    }
  }
}
