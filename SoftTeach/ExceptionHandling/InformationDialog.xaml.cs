using System;
using System.Windows;
using System.Windows.Controls;

namespace SoftTeach.ExceptionHandling
{
  /// <summary>
  /// Interaction logic for InformationDialog.xaml
  /// </summary>
  public partial class InformationDialog : Window
  {
    /// <summary>
    /// Initializes a new instance of the InformationDialog class.
    /// </summary>
    /// <param name="errorMessage">A <see cref="String"/>
    /// with the error message to display in the dialog.</param>
    public InformationDialog(string title, string message, bool isYesNoCancel)
    {
      this.InitializeComponent();
      this.Header.Title = title;
      this.ErrorMessageTextBlock.Text = message;

      if (isYesNoCancel)
      {
        this.YesNoCancelPanel.Visibility = Visibility.Visible;
        this.OkPanel.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.YesNoCancelPanel.Visibility = Visibility.Collapsed;
        this.OkPanel.Visibility = Visibility.Visible;
      }
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the <see cref="Button"/> <see cref="WeiterButton"/>.
    /// Trys to continue.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="EventArgs"/></param>
    private void OkButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    /// <summary>
    /// Displays an ogama styled message box with specified text, caption, buttons, and icon. 
    /// </summary>
    /// <param name="caption">The text to display in the title of the message box. </param>
    /// <param name="message">The text to display in the message box.</param>
    /// <param name="isYesNoCancel"><strong>True</strong> when yes no cancel buttons should be shown,
    /// otherwise <strong>false</strong> only OK button is shown.</param>
    /// <returns>One of the <see cref="DialogResult"/> values.</returns>
    public static bool? Show(string caption, string message, bool isYesNoCancel)
    {
      InformationDialog dlg = new InformationDialog(caption, message, isYesNoCancel);
      return dlg.ShowDialog();
    }

    private void JaButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void NeinButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    private void AbbruchButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = null;
      this.Close();
    }
  }
}
