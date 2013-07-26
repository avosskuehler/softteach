namespace Liduv.ExceptionHandling
{
  using System;
  using System.IO;
  using System.Text;
  using System.Windows;

  /// <summary>
  /// Static methods used for exception handling.
  /// </summary>
  public static class Log
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region METHODS

    /// <summary>
    /// Creates the error message and logs it to the exception file.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to be logged.</param>
    public static void HandleExceptionSilent(Exception ex)
    {
      // Add error to error log
      var exceptionLogFile = Path.Combine(Properties.Settings.Default.LogfilePath, "exception.log");
      var message = GetLogEntryForException(ex);

      using (StreamWriter w = File.AppendText(exceptionLogFile))
      {
        LogMessage(message, w);
      }
    }

    /// <summary>
    /// Creates the error message, logs it to file and displays it in 
    /// an <see cref="ExceptionDialog"/>.
    /// </summary>
    /// <param name="e">The <see cref="Exception"/> to show.</param>
    public static void HandleException(Exception e)
    {
      // Add error to error log
      var exceptionLogFile = Path.Combine(Properties.Settings.Default.LogfilePath, "exception.log");
      var message = GetLogEntryForException(e);

      using (var w = File.AppendText(exceptionLogFile))
      {
        LogMessage(message, w);
      }

      var newExceptionDlg = new ExceptionDialog { ExceptionMessage = e.Message, ExceptionDetails = message };
      newExceptionDlg.ShowDialog();
    }

    /// <summary>
    /// Process any unhandled exceptions that occur in the application.
    /// This code is called by all UI entry points in the application (e.g. button click events)
    /// when an unhandled exception occurs.
    /// You could also achieve this by handling the Application.ThreadException event, however
    /// the VS2005 debugger will break before this event is called.
    /// </summary>
    /// <param name="ex">The unhandled exception</param>
    public static void ProcessUnhandledException(Exception ex)
    {
      // An unhandled exception occured somewhere in our application. Let
      // the 'Global Policy' handler have a try at handling it.
      try
      {
        HandleException(ex);
      }
      catch
      {
        // Something has gone wrong during HandleException (e.g. incorrect configuration of the block).
        // Exit the application
        var errorMsg = "An unexpected exception occured while calling HandleException.";
        errorMsg += Environment.NewLine;

        MessageBox.Show(errorMsg, "Application Error", MessageBoxButton.OK, MessageBoxImage.Stop);

        Application.Current.Shutdown();
      }
    }

    /// <summary>
    /// Raises a "user-friendly" error message dialog.
    /// </summary>
    /// <param name="message">A <see cref="string"/> with the message to display.</param>
    public static void ProcessErrorMessage(string message)
    {
      if (message == null)
      {
        throw new ArgumentException("Error message string is NULL");
      }

      // Add error to error log
      var errorLogFile = Path.Combine(Properties.Settings.Default.LogfilePath, "error.log");
      using (var w = File.AppendText(errorLogFile))
      {
        LogMessage(message, w);
      }

      // Show error message dialog
      new ErrorDialog(message).ShowDialog();
    }

    /// <summary>
    /// Raises a ogama styled message dialog.
    /// </summary>
    /// <param name="title">A <see cref="string"/> with the title to display.</param>
    /// <param name="message">A <see cref="string"/> with the message to display.</param>
    public static void ProcessMessage(string title, string message)
    {
      // Show message dialog
      var newDlg = new InformationDialog(title, message, false);
      newDlg.ShowDialog();
    }

    /// <summary>
    /// This method logs the given message into the file
    /// given by the <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="logMessage">A <see cref="string"/> with the message to log.</param>
    /// <param name="w">The <see cref="TextWriter"/> to write the message to.</param>
    public static void LogMessage(string logMessage, TextWriter w)
    {
      w.Write("Error on ");
      w.WriteLine("{0}, {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
      w.WriteLine("  - {0}", logMessage);
      w.WriteLine("--------------------------------------------------------------------------------");

      // Update the underlying file.
      w.Flush();
    }

    #endregion //METHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER

    /// <summary>
    /// Returns a human readable string for the exception
    /// </summary>
    /// <param name="e">An <see cref="Exception"/> to be processed</param>
    /// <returns>A human readable <see cref="String"/> for the exception</returns>
    private static string GetLogEntryForException(Exception e)
    {
      var sb = new StringBuilder();
      sb.AppendLine("Message: " + e.Message);
      sb.AppendLine("Source: " + e.Source);
      sb.AppendLine("TargetSite: " + e.TargetSite);
      sb.AppendLine("StackTrace: " + e.StackTrace);

      return sb.ToString();
    }

    #endregion //HELPER
  }
}
