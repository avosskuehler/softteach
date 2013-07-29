
namespace Liduv.ViewModel.Termine
{
  using System;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Markup;

  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Jahrespläne;
  using Liduv.View.Stundenentwürfe;
  using Liduv.View.Termine;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Jahrespläne;
  using Liduv.ViewModel.Stundenentwürfe;

  /// <summary>
  /// ViewModel of an individual stunde
  /// </summary>
  public class StundeViewModel : LerngruppenterminViewModel, ISequencedObject
  {
    /// <summary>
    /// The stundenentwurf currently assigned to this stunde
    /// </summary>
    private StundenentwurfViewModel stundenentwurf;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StundeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="parentTagesplan">
    /// The tagesplan this ViewModel belongs to
    /// </param>
    /// <param name="stunde">
    /// The underlying stunde this ViewModel is to be based on
    /// </param>
    public StundeViewModel(TagesplanViewModel parentTagesplan, Stunde stunde)
      : base(parentTagesplan, stunde)
    {
      this.AddStundenentwurfCommand = new DelegateCommand(this.AddStundenentwurf);
      this.RemoveStundenentwurfCommand = new DelegateCommand(this.RemoveStundenentwurf, () => this.StundeStundenentwurf != null);
      this.SearchStundenentwurfCommand = new DelegateCommand(this.SearchStundenentwurf);
      this.PreviewStundenentwurfCommand = new DelegateCommand(this.PreviewStundenentwurf, () => this.StundeStundenentwurf != null);
      this.PrintStundenentwurfCommand = new DelegateCommand(this.PrintStundenentwurf, () => this.StundeStundenentwurf != null);
      this.PrintAllStundenentwurfCommand = new DelegateCommand(this.PrintAllStundenentwurf, () => this.StundeStundenentwurf != null);
    }

    /// <summary>
    /// Holt den Befehl zur adding a new stundenentwurf
    /// </summary>
    public DelegateCommand AddStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur removing a new stundenentwurf
    /// </summary>
    public DelegateCommand RemoveStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur searching an existing stundenentwurf
    /// </summary>
    public DelegateCommand SearchStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur adding a new stundenentwurf
    /// </summary>
    public DelegateCommand PreviewStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur printing the current stundenentwurf
    /// </summary>
    public DelegateCommand PrintStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur printing the current stundenentwurf including
    /// attached files
    /// </summary>
    public DelegateCommand PrintAllStundenentwurfCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die stundenentwurf currently assigned to this Stunde
    /// </summary>
    public StundenentwurfViewModel StundeStundenentwurf
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (((Stunde)this.Model).Stundenentwurf == null)
        {
          return null;
        }

        if (this.stundenentwurf == null || this.stundenentwurf.Model != ((Stunde)this.Model).Stundenentwurf)
        {
          this.stundenentwurf = App.MainViewModel.Stundenentwürfe.SingleOrDefault(d => d.Model == ((Stunde)this.Model).Stundenentwurf);
        }

        return this.stundenentwurf;
      }

      set
      {
        if (value == this.stundenentwurf) return;
        this.UndoablePropertyChanging(this, "StundeStundenentwurf", this.stundenentwurf, value);
        this.stundenentwurf = value;
        if (value != null)
        {
          ((Stunde)this.Model).Stundenentwurf = value.Model;
        }
        else
        {
          ((Stunde)this.Model).Stundenentwurf = null;
        }

        Selection.Instance.Stundenentwurf = value;
        this.RaisePropertyChanged("StundeStundenentwurf");
        this.RemoveStundenentwurfCommand.RaiseCanExecuteChanged();
        this.PreviewStundenentwurfCommand.RaiseCanExecuteChanged();
        this.PrintStundenentwurfCommand.RaiseCanExecuteChanged();
        this.PrintAllStundenentwurfCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt das Modul für die Stunde, wenn ein Stundenentwurf existiert,
    /// ansonsten null.
    /// </summary>
    [DependsUpon("StundeStundenentwurf")]
    public string StundeModul
    {
      get
      {
        if (this.StundeStundenentwurf != null)
        {
          return this.StundeStundenentwurf.StundenentwurfModul.ModulKurzbezeichnung;
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Holt die Stundennummer dieser Stunde.
    /// Das ist die laufende Nummer im aktuellen Halbjahresplan.
    /// </summary>
    public int StundeLaufendeStundennummer
    {
      get
      {
        var stundeCounter = 1;

        foreach (var monatsplan in ((Stunde)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Monatspläne)
        {
          foreach (var tagesplan in monatsplan.Tagespläne.OrderBy(o => o.Datum))
          {
            var tagesplanViewModel = App.MainViewModel.Tagespläne.First(o => o.Model == tagesplan);
            foreach (var lerngruppentermin in tagesplanViewModel.Lerngruppentermine)
            {
              if (lerngruppentermin is StundeViewModel)
              {
                var stunde = lerngruppentermin as StundeViewModel;
                if (stunde == this)
                {
                  return stundeCounter;
                }

                var stundenzahl = stunde.TerminLetzteUnterrichtsstunde.UnterrichtsstundeIndex
                  - stunde.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex + 1;

                stundeCounter += stundenzahl;
              }
            }
          }
        }

        return stundeCounter;
      }
    }

    /// <summary>
    /// Holt oder setzt den AbfolgeIndex dieser Stunde im laufenden Jahresplan.
    /// </summary>
    public int AbfolgeIndex { get; set; }

    /// <summary>
    /// Holt einen String mit dem Stundenbereich der laufenden Nummer.
    /// </summary>
    [DependsUpon("StundeLaufendeStundennummer")]
    [DependsUpon("TerminStundenanzahl")]
    public string StundeLaufendeNummerStundebereich
    {
      get
      {
        if (this.TerminStundenanzahl == 1)
        {
          return "Stunde " + this.StundeLaufendeStundennummer;
        }

        var letzteStundeNummer = this.StundeLaufendeStundennummer + this.TerminStundenanzahl - 1;
        return "Stunden " + this.StundeLaufendeStundennummer + "-" + letzteStundeNummer;
      }
    }

    /// <summary>
    /// Holt oder setzt die Beschreibung of this Lerngruppentermin
    /// </summary>
    public override string TerminBeschreibung
    {
      get
      {
        return this.StundeStundenentwurf == null ? this.StundeLaufendeNummerStundebereich : this.StundeStundenentwurf.StundenentwurfStundenthema;
      }

      set
      {
        if (value == this.Model.Beschreibung)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "TerminBeschreibung", this.Model.Beschreibung, value);
        this.Model.Beschreibung = value;
        this.RaisePropertyChanged("TerminBeschreibung");
      }
    }

    /// <summary>
    /// Holt den Stundenthema of the Stundenentwurf assigned to this stunde
    /// </summary>
    [DependsUpon("StundeStundenentwurf")]
    public string StundeStundenentwurfTitel
    {
      get
      {
        if (this.StundeStundenentwurf == null)
        {
          return string.Empty;
        }

        return this.StundeStundenentwurf.StundenentwurfStundenthema ?? string.Empty;
      }
    }

    /// <summary>
    /// Holt den date of this stunde as as string
    /// </summary>
    [DependsUpon("LerngruppenterminDatum")]
    public string StundeDatum
    {
      get
      {
        return this.LerngruppenterminDatum.ToString("dd.MM.yy", new CultureInfo("de-DE"));
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    [DependsUpon("TerminStundenanzahl")]
    public int StundeBreite
    {
      get
      {
        return this.TerminStundenanzahl * Properties.Settings.Default.Stundenbreite;
      }
    }

    /// <summary>
    /// Holt den Stundenbedarf as a string
    /// </summary>
    [DependsUpon("TerminStundenanzahl")]
    public string StundeStundenbedarfString
    {
      get
      {
        return this.TerminStundenanzahl + "h";
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    [DependsUpon("StundeBreite")]
    public int StundeDetailBreite
    {
      get
      {
        return this.StundeBreite * 4;
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Stunde: " + this.TerminStundenbereich;
    }

    private void UpdateStundenentwurfPhasenzeitraum()
    {
      if (this.StundeStundenentwurf == null)
      {
        return;
      }

      this.UpdateStundenentwurfStundenzahl();

      // Update phasen with correct zeitraum
      var phasen = this.StundeStundenentwurf.Phasen;

      // Get begin time for this stunde
      var startzeit = this.TerminErsteUnterrichtsstunde.UnterrichtsstundeBeginn;
      var phasenstartzeit = startzeit;
      for (int x = 0; x < phasen.Count; x++)
      {
        var phase = phasen[x];

        var phasenendzeit = phasenstartzeit + new TimeSpan(0, phase.PhaseZeit, 0);
        var phasenzeitraum = phasenstartzeit.ToString(@"hh\:mm") + "-" + phasenendzeit.ToString(@"hh\:mm");
        phasenstartzeit = phasenendzeit;
        phase.PhaseZeitraum = phasenzeitraum;
      }
    }

    protected override void EditLerngruppentermin()
    {
      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Stunde {0} editieren", this), false))
      {
        var dlg = new AddStundeDialog(this);
        Selection.Instance.Stunde = this;
        Selection.Instance.Stundenentwurf = this.StundeStundenentwurf;
        if (!(undo = !dlg.ShowDialog().GetValueOrDefault(false)))
        {
          this.ParentTagesplan.UpdateBeschreibung();
          this.TerminBeschreibung = dlg.StundeViewModel.TerminBeschreibung;
          this.TerminTermintyp = dlg.StundeViewModel.TerminTermintyp;
          this.TerminErsteUnterrichtsstunde = dlg.StundeViewModel.TerminErsteUnterrichtsstunde;
          this.TerminLetzteUnterrichtsstunde = dlg.StundeViewModel.TerminLetzteUnterrichtsstunde;
          this.StundeStundenentwurf = dlg.StundeViewModel.StundeStundenentwurf;
          this.TerminTermintyp = dlg.StundeViewModel.TerminTermintyp;
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Handles addition a new Stundenentwurf to the workspace and model
    /// </summary>
    private void AddStundenentwurf()
    {
      var entwurf = new Stundenentwurf();
      entwurf.Datum = DateTime.Now;
      entwurf.Fach = ((Stunde)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Fach;
      entwurf.Jahrgangsstufe = ((Stunde)this.Model).Tagesplan.Monatsplan.Halbjahresplan.Jahresplan.Klasse.Klassenstufe.Jahrgangsstufe;
      entwurf.Stundenzahl = this.TerminStundenanzahl;
      entwurf.Ansagen = string.Empty;
      entwurf.Computer = false;
      entwurf.Hausaufgaben = string.Empty;
      entwurf.Kopieren = false;
      entwurf.Stundenthema = "Neues Thema";
      var availableModule =
        App.MainViewModel.Module.Where(
          o => o.ModulJahrgangsstufe.Model == entwurf.Jahrgangsstufe && o.ModulFach.Model == entwurf.Fach);
      entwurf.Modul = availableModule.First().Model;

      var vm = new StundenentwurfViewModel(entwurf);
      App.MainViewModel.Stundenentwürfe.Add(vm);
      this.StundeStundenentwurf = vm;
    }

    /// <summary>
    /// Removes the current assigned Stundenentwurf from this stunde
    /// </summary>
    private void RemoveStundenentwurf()
    {
      App.MainViewModel.Stundenentwürfe.RemoveTest(this.StundeStundenentwurf);
      this.StundeStundenentwurf = null;
    }

    /// <summary>
    /// Searches a Stundenentwurf for this stunde.
    /// </summary>
    private void SearchStundenentwurf()
    {
      App.MainViewModel.StundenentwurfWorkspace.CurrentStundenentwurf = this.StundeStundenentwurf;

      var dlg = new SearchStundenentwurfDialog(App.MainViewModel.StundenentwurfWorkspace);
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        if (this.StundeStundenentwurf != null &&
          this.StundeStundenentwurf.StundenentwurfPhasenKurzform == string.Empty) App.MainViewModel.Stundenentwürfe.RemoveTest(this.StundeStundenentwurf);
        this.StundeStundenentwurf = dlg.SelectedStundenentwurfViewModel;
        this.UpdateStundenentwurfStundenzahl();
      }
    }

    /// <summary>
    /// Zeigt eine Druckvorschau des Stundenentwurfs für den Ausdruck.
    /// </summary>
    private void PreviewStundenentwurf()
    {
      // select printer and get printer settings
      var pd = new PrintDialog();

      // create a document
      var document = new FixedDocument { Name = "StundenentwurfAusdruck" };
      document.DocumentPaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

      // create a page
      var page1 = new FixedPage
      {
        Width = document.DocumentPaginator.PageSize.Width,
        Height = document.DocumentPaginator.PageSize.Height
      };

      // Set correct times
      this.UpdateStundenentwurfPhasenzeitraum();

      // create the print output usercontrol
      var content = new StundenentwurfPrintView
      {
        DataContext = this,
        Width = page1.Width,
        Height = page1.Height
      };

      page1.Children.Add(content);

      // add the page to the document
      var page1Content = new PageContent();
      ((IAddChild)page1Content).AddChild(page1);
      document.Pages.Add(page1Content);

      // create Preview
      var preview = new StundenentwurfFixedDocument
        {
          Owner = Application.Current.MainWindow,
          Document = document
        };

      preview.ShowDialog();
    }

    /// <summary>
    /// Druckt den Stundenentwurf ohne Nachfrage aus.
    /// </summary>
    private void PrintStundenentwurf()
    {
      // select printer and get printer settings
      var pd = new PrintDialog();
      //if (pd.ShowDialog() != true) return;

      // create a document
      var document = new FixedDocument { Name = "StundenentwurfAusdruck" };
      document.DocumentPaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

      // create a page
      var fixedPage = new FixedPage
      {
        Width = document.DocumentPaginator.PageSize.Width,
        Height = document.DocumentPaginator.PageSize.Height
      };

      // Set correct times
      this.UpdateStundenentwurfPhasenzeitraum();

      // create the print output usercontrol
      var content = new StundenentwurfPrintView
      {
        DataContext = this,
        Width = fixedPage.Width,
        Height = fixedPage.Height
      };

      fixedPage.Children.Add(content);

      // Update the layout of our FixedPage
      var size = document.DocumentPaginator.PageSize;
      fixedPage.Measure(size);
      fixedPage.Arrange(new Rect(new Point(), size));
      fixedPage.UpdateLayout();

      // print it out
      pd.PrintVisual(fixedPage, "Stundenentwurf-Ausdruck");
    }

    /// <summary>
    /// Druckt den Stundenentwurf und alle dazugehörigen Dateien aus.
    /// </summary>
    private void PrintAllStundenentwurf()
    {
      this.PrintStundenentwurf();
      foreach (var dateiverweisViewModel in this.StundeStundenentwurf.Dateiverweise)
      {
        App.PrintFile(dateiverweisViewModel.DateiverweisDateiname);
      }
    }
  }
}
