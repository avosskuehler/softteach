// <copyright file="App.xaml.cs" company="Paul Natorp Gymnasium, Berlin">        
// SoftTeach - Lehrerunterrichtsdatenbank
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

namespace SoftTeach
{
  using Hardcodet.Wpf.TaskbarNotification;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model;
  using SoftTeach.Properties;
  using SoftTeach.Resources.FontAwesome;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Main;
  using SoftTeach.ViewModel;
  using SoftTeach.ViewModel.Helper;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Printing;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Markup;
  using System.Windows.Navigation;
  using System.Windows.Threading;
  using Cursor = System.Windows.Input.Cursor;

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

    public static System.Windows.Style GetIconStyle(string imageName)
    {
      return App.Current.FindResource(imageName) as System.Windows.Style;
    }

    public static IconBlock GetIcon(string imageName)
    {
      var icon = new IconBlock
      {
        VerticalAlignment = VerticalAlignment.Center
      };

      icon.Style = App.Current.FindResource(imageName) as System.Windows.Style;

      return icon;
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
      
      ProcessStartInfo psi = new ProcessStartInfo
      {
        FileName = fullPath,
        UseShellExecute = true
      };
      Process.Start(psi);
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
    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
      base.OnStartup(e);

      UnitOfWork = new UnitOfWork();
      MainViewModel = new MainViewModel();
      MainViewModel.Populate();

      var navWin = new StartView();

      // Sprache der UI auf Current Culture setzen
      System.Windows.FrameworkElement.LanguageProperty.OverrideMetadata(
        typeof(System.Windows.FrameworkElement),
        new System.Windows.FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.MetroWindow = navWin;
      navWin.Show();
      this.MainWindow = navWin;

      navWin.Navigate(new LandingPage());
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
