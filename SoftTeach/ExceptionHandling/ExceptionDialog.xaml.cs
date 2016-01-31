using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoftTeach.ExceptionHandling
{
  /// <summary>
  /// Interaction logic for ErrorDialog.xaml
  /// </summary>
  public partial class ExceptionDialog : Window
  {
    /// <summary>
    /// Initializes a new instance of the ExceptionDialog class.
    /// </summary>
    public ExceptionDialog()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Sets the exception details to display in the dialog.
    /// </summary>
    /// <value>A <see cref="string"/> with details for the exception like stack trace.</value>
    public string ExceptionDetails
    {
      set { this.ExceptionDetailTextBox.Text = value; }
    }

    /// <summary>
    /// Sets the error message to display in the dialog.
    /// </summary>
    /// <value>A <see cref="string"/> with exception message.</value>
    public string ExceptionMessage
    {
      set { this.ExceptionMessageTextBlock.Text = value; }
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the <see cref="Button"/> <see cref="ShowLogfileButton"/>.
    /// Shows a <see cref="ShowLogDialog"/> with the content
    /// of the "exception.log" file
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="EventArgs"/></param>
    private void ShowLogfileButtonClick(object sender, EventArgs e)
    {
      ShowLogDialog logForm =
        new ShowLogDialog(System.IO.Path.Combine(Properties.Settings.Default.LogfilePath, "exception.log"));
      logForm.ShowDialog();
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the <see cref="Button"/> <see cref="AbbruchButton"/>.
    /// Closes the application.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="EventArgs"/></param>
    private void AbbruchButtonClick(object sender, EventArgs e)
    {
      Application.Current.Shutdown();
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the <see cref="Button"/> <see cref="WeiterButton"/>.
    /// Trys to continue.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="EventArgs"/></param>
    private void WeiterButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the <see cref="Button"/> <see cref="TrySaveButton"/>.
    /// Versucht die Daten zu speichern bevor das Programm beendet wird.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="EventArgs"/></param>
    private void TrySaveButtonClick(object sender, RoutedEventArgs e)
    {
      Properties.Settings.Default.Save();
      App.UnitOfWork.SaveChanges();
    }
  }
}
