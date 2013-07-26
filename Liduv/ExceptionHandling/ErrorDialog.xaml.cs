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

namespace Liduv.ExceptionHandling
{
  /// <summary>
  /// Interaction logic for ErrorDialog.xaml
  /// </summary>
  public partial class ErrorDialog : Window
  {
    /// <summary>
    /// Initializes a new instance of the ErrorDialog class.
    /// </summary>
    /// <param name="errorMessage">A <see cref="String"/>
    /// with the error message to display in the dialog.</param>
    public ErrorDialog(string errorMessage)
    {
      this.InitializeComponent();
      this.ErrorMessageTextBlock.Text = errorMessage;
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the <see cref="Button"/> <see cref="btnErrorLog"/>.
    /// Shows a <see cref="ShowLogDialog"/> with the content
    /// of the "error.log" file
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="EventArgs"/></param>
    private void ShowLogfileButtonClick(object sender, EventArgs e)
    {
      ShowLogDialog logForm =
        new ShowLogDialog(System.IO.Path.Combine(Properties.Settings.Default.LogfilePath, "error.log"));
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
      this.Close();
    }
  }
}
