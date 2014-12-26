// <copyright file="App.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
// Liduv - Lehrerunterrichtsdatenbank
// Copyright (C) 2013 Dr. Adrian Voßkühler
// -----------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published  
// by the Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version. This program is distributed in the 
// hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// ***********************************************************************
// </copyright>
// <author>Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>

namespace Liduv
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Printing;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Threading;

  using Hardcodet.Wpf.TaskbarNotification;

  using Liduv.ExceptionHandling;
  using Liduv.Model;
  using Liduv.Properties;
  using Liduv.Setting;
  using Liduv.View.Main;
  using Liduv.ViewModel;
  using Liduv.ViewModel.Helper;

  using MahApps.Metro.Controls;

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    /// <summary>
    /// Holt den <see cref="MainViewModel"/> for this Application which
    /// is the complete set of data.
    /// </summary>
    public static MainViewModel MainViewModel { get; private set; }

    /// <summary>
    /// Holt den Datenbankkontext
    /// </summary>
    public static UnitOfWork UnitOfWork { get; private set; }

    /// <summary>
    /// Gets the noten erinnerungs icon.
    /// </summary>
    /// <value>The noten erinnerungs icon.</value>
    public static TaskbarIcon NotenErinnerungsIcon { get; private set; }

    /// <summary>
    /// Holt den Befehl, der aufgerufen werden soll, wenn das TrayIcon angeklickt wird.
    /// </summary>
    public DelegateCommand TrayIconClickedCommand { get; private set; }

    /// <summary>
    /// This static mehtod returns an <see cref="Image"/>
    /// for the given filename string, if the image is in the Images
    /// subfolder of the solution.
    /// </summary>
    /// <param name="imageName">A <see cref="String"/> with the images file name</param>
    /// <returns>The <see cref="Image"/> that can be used as a source for
    /// an icon property.</returns>
    public static Image GetImage(string imageName)
    {
      var terminMenuentryIcon = new Image();
      var terminMenuentryIconImage = new BitmapImage();
      terminMenuentryIconImage.BeginInit();
      terminMenuentryIconImage.UriSource = new Uri("pack://application:,,,/Images/" + imageName);
      terminMenuentryIconImage.EndInit();
      terminMenuentryIcon.Source = terminMenuentryIconImage;
      return terminMenuentryIcon;
    }

    /// <summary>
    /// Setzt den Cursor für die gesamte Anwendung.
    /// </summary>
    /// <param name="cursor"> The cursor to be set, null if reset to default.</param>
    public static void SetCursor(Cursor cursor)
    {
      Current.Dispatcher.Invoke(
        () =>
        {
          // The check is required to prevent cursor flickering
          if (Mouse.OverrideCursor != cursor) Mouse.OverrideCursor = cursor;
        });
    }

    public static ImageSource GetImageSource(string imageName)
    {
      var terminMenuentryIconImage = new BitmapImage();
      terminMenuentryIconImage.BeginInit();
      terminMenuentryIconImage.UriSource = new Uri("pack://application:,,,/Images/" + imageName);
      terminMenuentryIconImage.EndInit();
      return terminMenuentryIconImage;
    }

    /// <summary>
    /// Opens the file.
    /// </summary>
    /// <param name="fullPath">The full path.</param>
    public static void OpenFile(string fullPath)
    {
      var fileInfo = new FileInfo(fullPath);

      if (!fileInfo.Exists)
      {
        InformationDialog.Show(
          "Datei nicht gefunden", "Die zu öffnende Datei " + fullPath + " wurde nicht gefunden.", false);
        return;
      }

      var openProcess = new Process { StartInfo = { FileName = fullPath } };
      openProcess.Start();
    }

    /// <summary>
    /// Prints the file.
    /// </summary>
    /// <param name="fullPath">The full path.</param>
    public static void PrintFile(string fullPath)
    {
      var fileInfo = new FileInfo(fullPath);

      if (!fileInfo.Exists)
      {
        InformationDialog.Show(
           "Datei nicht gefunden", "Die zu druckende Datei " + fullPath + " wurde nicht gefunden.", false);
      }

      var printProcess = new Process
        {
          StartInfo =
            {
              FileName = fullPath,
              UseShellExecute = true,
              Verb = "print"
            }
        };

      printProcess.Start();
    }

    /// <summary>
    ///   Returns a PrintTicket based on the current default printer.</summary>
    /// <returns>A PrintTicket for the current local default printer.</returns>
    public static PrintTicket GetPrintTicketFromPrinter()
    {
      PrintQueue printQueue;

      var localPrintServer = new LocalPrintServer();

      // Retrieving collection of local printer on user machine
      var localPrinterCollection = localPrintServer.GetPrintQueues();

      System.Collections.IEnumerator localPrinterEnumerator =
          localPrinterCollection.GetEnumerator();

      if (localPrinterEnumerator.MoveNext())
      {
        // Get PrintQueue from first available printer
        printQueue = (PrintQueue)localPrinterEnumerator.Current;
      }
      else
      {
        // No printer exist, return null PrintTicket
        return null;
      }

      // Get default PrintTicket from printer
      var printTicket = printQueue.DefaultPrintTicket;

      var printCapabilites = printQueue.GetPrintCapabilities();

      // Modify PrintTicket
      if (printCapabilites.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge))
      {
        printTicket.Duplexing = Duplexing.TwoSidedLongEdge;
      }

      return printTicket;
    }

    /// <summary>
    /// Lauches the entry form on startup
    /// </summary>
    /// <param name="e">Arguments of the startup event</param>
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      this.TrayIconClickedCommand = new DelegateCommand(this.TrayIconClicked);

      NotenErinnerungsIcon = (TaskbarIcon)FindResource("NotenNotifyIcon");
      if (NotenErinnerungsIcon != null)
      {
        NotenErinnerungsIcon.LeftClickCommand = this.TrayIconClickedCommand;
      }

      UnitOfWork = new UnitOfWork();
      MainViewModel = new MainViewModel();
      MainViewModel.Populate();
      //var window = new MainRibbonView { DataContext = MainViewModel };
      //window.Show();

      Selection.Instance.PopulateFromSettings();

      var navWin = new MetroNavigationWindow();
      navWin.Closing += navWin_Closing;
      navWin.Title = "Liduv";
      navWin.ShowTitleBar = false;
      navWin.WindowState = WindowState.Maximized;
      navWin.Icon = GetImageSource("LiduvLogo64.png");
      navWin.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Liduv;component/Resources/MetroResources.xaml", UriKind.Absolute) });
      //uncomment the next two lines if you want the clean style.
      //navWin.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Clean/CleanWindow.xaml", UriKind.Absolute) });
      //navWin.SetResourceReference(FrameworkElement.StyleProperty, "CleanWindowStyleKey");
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.MetroWindow = navWin;
      navWin.Show();
      navWin.Navigate(new LandingPage());
    }

    /// <summary>
    /// Hier wird der Dialog zur Noteneingabe aufgerufen
    /// </summary>
    private void TrayIconClicked()
    {
      MainViewModel.StartNoteneingabe();
    }

    /// <summary>
    /// Handles the Closing event of the navWin control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
    private void navWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Selection.Instance.UpdateUserSettings();
      Settings.Default.Save();

      //// Check if there is nothing to change
      //if (((Stack<ChangeSet>)App.MainViewModel.UndoStack).Count == 0)
      //{
      //  return;
      //}

      var dlg = new AskForSavingChangesDialog();
      dlg.ShowDialog();
      if (dlg.Result == null)
      {
        e.Cancel = true;
      }
      else if (dlg.Result.Value)
      {
        App.UnitOfWork.SaveChanges();
      }
    }

    /// <summary>
    /// Cleans up any resources on exit
    /// </summary>
    /// <param name="e">Arguments of the exit event</param>
    protected override void OnExit(ExitEventArgs e)
    {
      UnitOfWork.Dispose();
      NotenErinnerungsIcon.Dispose();
      base.OnExit(e);
    }

    /// <summary>
    /// The <see cref="System.Windows.Application.DispatcherUnhandledException"/> event handler.
    ///   Displays a message for each otherwise unhandled exception.
    /// </summary>
    /// <param name="sender">
    /// Source of the event 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.StartupEventArgs"/> with the event data. 
    /// </param>
    private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      // Raise message box and log to file.
      Log.ProcessUnhandledException(e.Exception);
    }
  }
}
