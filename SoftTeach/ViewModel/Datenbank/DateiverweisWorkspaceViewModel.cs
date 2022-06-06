namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using System.Windows.Data;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Stundenentwürfe;

  /// <summary>
  /// ViewModel for managing Datenbank
  /// </summary>
  public class DateiverweisWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Dateiverweis currently selected
    /// </summary>
    private Dateiverweis currentDateiverweis;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="DateiverweisWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public DateiverweisWorkspaceViewModel()
    {
      this.RepariereDateiverweiseCommand = new DelegateCommand(this.RepariereDateiverweise);
      this.EntferneDateiverweisCommand = new DelegateCommand(this.EntferneDateiverweis, () => this.CurrentDateiverweis != null);

      this.Dateiverweise = new List<Dateiverweis>();
      foreach (var dateiverweis in App.UnitOfWork.Context.Dateiverweise)
      {
        this.Dateiverweise.Add(dateiverweis);
      }

      this.DateiverweiseViewSource = new CollectionViewSource() { Source = this.Dateiverweise };
      using (this.DateiverweiseViewSource.DeferRefresh())
      {
        this.DateiverweiseViewSource.SortDescriptions.Add(new SortDescription("Stunde.Lerngruppe.Schuljahr.Jahr", ListSortDirection.Ascending));
        this.DateiverweiseViewSource.SortDescriptions.Add(new SortDescription("Stunde.Fach.Bezeichnung", ListSortDirection.Ascending));
        this.DateiverweiseViewSource.SortDescriptions.Add(new SortDescription("Stunde.Modul.Bezeichnung", ListSortDirection.Ascending));
        this.DateiverweiseViewSource.SortDescriptions.Add(new SortDescription("DateinameOhnePfad", ListSortDirection.Ascending));
      }
      this.SelectedDateiverweise = new List<Dateiverweis>();
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Dateiverweis
    /// </summary>
    public DelegateCommand RepariereDateiverweiseCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl einen Dateiverweis zu löschen
    /// </summary>
    public DelegateCommand EntferneDateiverweisCommand { get; private set; }

    public List<Dateiverweis> Dateiverweise { get; private set; }

    /// <summary>
    /// Holt oder setzt die ViewSource der Sitzpläne
    /// </summary>
    public CollectionViewSource DateiverweiseViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Sitzpläne
    /// </summary>
    public ICollectionView DateiverweiseView => this.DateiverweiseViewSource.View;

    /// <summary>
    /// Holt die markierten Dateiverweise
    /// </summary>
    public System.Collections.IList SelectedDateiverweise { get; set; }

    /// <summary>
    /// Holt oder setzt die dateiverweis currently selected in this workspace
    /// </summary>
    public Dateiverweis CurrentDateiverweis
    {
      get
      {
        return this.currentDateiverweis;
      }

      set
      {
        this.currentDateiverweis = value;
        this.RaisePropertyChanged("CurrentDateiverweis");
        this.EntferneDateiverweisCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Handles deletion of the current Dateiverweis
    /// </summary>
    private void EntferneDateiverweis()
    {
      App.UnitOfWork.Context.Dateiverweise.Remove(this.CurrentDateiverweis);
      this.Dateiverweise.RemoveTest(this.CurrentDateiverweis);
      this.CurrentDateiverweis = null;
      this.DateiverweiseView.Refresh();
    }

    /// <summary>
    /// Handles addition a new Dateiverweis to the workspace and model
    /// </summary>
    private void RepariereDateiverweise()
    {
      foreach (Dateiverweis dateiverweis in this.SelectedDateiverweise)
      {
        if (!File.Exists(dateiverweis.Dateiname))
        {
          var rootPath = string.Empty;
          switch (dateiverweis.Stunde.Fach.Bezeichnung)
          {
            case "Physik":
              rootPath = @"C:\Users\VK\Documents\Schule\Physik\Planung\";
              break;
            case "Mathematik":
              rootPath = @"C:\Users\VK\Documents\Schule\Mathe\Planung\";
              break;
            case "ITG":
              rootPath = @"C:\Users\VK\Documents\Schule\ITG\Planung\";
              break;
            default:
              rootPath = @"C:\Users\VK\Documents\Schule\";
              break;
          }

          var dirInfo = new DirectoryInfo(rootPath);
          WalkDirectoryTree(dirInfo, dateiverweis);
        }
      }
    }

    static void WalkDirectoryTree(System.IO.DirectoryInfo root, Dateiverweis dateiverweis)
    {
      System.IO.FileInfo[] files = null;
      System.IO.DirectoryInfo[] subDirs = null;

      // First, process all the files directly under this folder
      try
      {
        files = root.GetFiles("*.*");
      }
      // This is thrown if even one of the files requires permissions greater
      // than the application provides.
      catch (UnauthorizedAccessException e)
      {
        // This code just writes out the message and continues to recurse.
        // You may decide to do something different here. For example, you
        // can try to elevate your privileges and access the file again.
        InformationDialog.Show("Fehler", e.Message, false);
      }

      catch (System.IO.DirectoryNotFoundException e)
      {
        Console.WriteLine(e.Message);
      }

      if (files != null)
      {
        foreach (System.IO.FileInfo fi in files)
        {
          if (fi.Name == dateiverweis.DateinameOhnePfad)
          {
            dateiverweis.Dateiname = fi.FullName;
            return;
          }
        }

        // Now find all the subdirectories under this directory.
        subDirs = root.GetDirectories();

        foreach (System.IO.DirectoryInfo dirInfo in subDirs)
        {
          // Resursive call for each subdirectory.
          WalkDirectoryTree(dirInfo, dateiverweis);
        }
      }
    }

  }
}
