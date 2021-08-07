namespace SoftTeach.ViewModel.Wochenpläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Media;

  using SoftTeach.Model.EntityFramework;

  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Noten;
  using SoftTeach.ViewModel.Termine;

  /// <summary>
  /// Für jeden Termin der Woche (Unterrichtsstunde, AG, Treffen) wird ein Terminplaneintrag erstellt.
  /// Er erscheint im Wochenplan oder im Tagesplan
  /// </summary>
  public class TerminplanEintrag : ViewModelBase
  {
    /// <summary>
    /// Der 0-basierte Index des Wochentags für diesen Terminplaneintrag
    /// </summary>
    private int wochentagIndex;

    /// <summary>
    /// Die erste Stunde dieses Termins.
    /// </summary>
    private int ersteUnterrichtsstundeIndex;

    /// <summary>
    /// Die letzte Stunde dieses Termins.
    /// </summary>
    private int letzteUnterrichtsstundeIndex;

    /// <summary>
    /// The termin view model
    /// </summary>
    private TerminViewModel terminViewModel;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TerminplanEintrag"/> Klasse. 
    /// </summary>
    /// <param name="parent"> The underlying terminplanworkspace this ViewModel is to be based on </param>
    /// <param name="terminViewModel"> The termin View Model. </param>
    public TerminplanEintrag(TerminplanWorkspaceViewModel parent, TerminViewModel terminViewModel)
    {
      if (parent == null)
      {
        throw new ArgumentNullException("parent");
      }

      this.TerminViewModel = terminViewModel;
      this.Parent = parent;
      this.ViewTerminplaneintragCommand = new DelegateCommand(this.ViewTerminplaneintrag, () => this.TerminViewModel != null);
      this.EditTerminplaneintragCommand = new DelegateCommand(this.EditTerminplaneintrag, () => this.TerminViewModel != null);
      this.RemoveTerminplaneintragCommand = new DelegateCommand(this.RemoveTerminplaneintrag, () => this.TerminViewModel != null);
      this.ProofTerminplaneintragCommand = new DelegateCommand(this.ProofTerminplaneintrag, () => this.TerminViewModel != null);
      this.AddNotenCommand = new DelegateCommand(this.AddNoten, () => this.TerminViewModel != null);
      this.AddHausaufgabenCommand = new DelegateCommand(this.AddHausaufgaben, () => this.TerminViewModel != null);
      this.AddSonstigeNotenCommand = new DelegateCommand(this.AddSonstigeNoten, () => this.TerminViewModel != null);
    }

    /// <summary>
    /// Holt den Befehl zur proof or unproof the current Terminplaneintrag
    /// </summary>
    public DelegateCommand ProofTerminplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Ansicht des aktuellen Terminplaneintrags
    /// </summary>
    public DelegateCommand ViewTerminplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur editing the current Terminplaneintrag
    /// </summary>
    public DelegateCommand EditTerminplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur removing the current Terminplaneintrag
    /// </summary>
    public DelegateCommand RemoveTerminplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl Noten einzutragen
    /// </summary>
    public DelegateCommand AddNotenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um vergessen Hausaufgaben anzulegen.
    /// </summary>
    public DelegateCommand AddHausaufgabenCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl, um sonstige Noten anzulegen.
    /// </summary>
    public DelegateCommand AddSonstigeNotenCommand { get; private set; }


    /// <summary>
    /// Holt the parent <see cref="TerminplanWorkspaceViewModel"/> to which this Terminplaneintrag
    /// is added to.
    /// </summary>
    public TerminplanWorkspaceViewModel Parent { get; private set; }

    /// <summary>
    /// Holt oder setzt den Termin, der zu diesem Terminplaneintrag gehört.
    /// </summary>
    public TerminViewModel TerminViewModel
    {
      get
      {
        return this.terminViewModel;
      }

      set
      {
        if (this.terminViewModel != null)
        {
          this.terminViewModel.PropertyChanged -= this.TerminViewModelPropertyChanged;
        }

        this.terminViewModel = value;

        if (value != null)
        {
          this.terminViewModel.PropertyChanged += this.TerminViewModelPropertyChanged;
        }
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob dieser Terminplaneintrag leer ist (ohne Termine).
    /// </summary>
    public bool IsDummy
    {
      get
      {
        return this.TerminViewModel == null;
      }
    }

    /// <summary>
    /// Holt oder setzt den 0-basierte Index des Wochentags für diesen Terminplaneintrag
    /// </summary>
    public int WochentagIndex
    {
      get
      {
        if (this.TerminViewModel != null)
        {
          if (this.TerminViewModel is LerngruppenterminViewModel)
          {
            var lerngruppenterminViewModel = this.TerminViewModel as LerngruppenterminViewModel;
            this.wochentagIndex = lerngruppenterminViewModel.LerngruppenterminWochentagIndex;
          }
          else if (this.TerminViewModel is SchulterminViewModel)
          {
            var schulterminViewModel = this.TerminViewModel as SchulterminViewModel;
            this.wochentagIndex = (int)schulterminViewModel.SchulterminDatum.DayOfWeek;
          }
        }

        return this.wochentagIndex;
      }

      set
      {
        if (!this.IsDummy)
        {
          throw new ArgumentOutOfRangeException("value", "Der Wochentagindex kann nicht zugewiesen werden.");
        }

        this.wochentagIndex = value;
        this.RaisePropertyChanged("WochentagIndex");
      }
    }

    /// <summary>
    /// Holt oder setzt den Index für die erste Stunde dieses Termins.
    /// </summary>
    public int ErsteUnterrichtsstundeIndex
    {
      get
      {
        if (this.TerminViewModel != null)
        {
          this.ersteUnterrichtsstundeIndex = this.TerminViewModel.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex;
        }

        return this.ersteUnterrichtsstundeIndex;
      }

      set
      {
        if (!this.IsDummy)
        {
          throw new ArgumentOutOfRangeException("value", "Der StundenplaneintragErsteUnterrichtsstundeIndex kann nicht zugewiesen werden.");
        }

        this.ersteUnterrichtsstundeIndex = value;
        this.RaisePropertyChanged("ErsteUnterrichtsstundeIndex");
      }
    }

    /// <summary>
    /// Holt oder setzt den index der letzten Unterrichtstunde des Wochenplaneintrags
    /// </summary>
    public int LetzteUnterrichtsstundeIndex
    {
      get
      {
        if (this.TerminViewModel != null)
        {
          this.letzteUnterrichtsstundeIndex = this.TerminViewModel.TerminLetzteUnterrichtsstunde.UnterrichtsstundeIndex;
        }

        return this.letzteUnterrichtsstundeIndex;
      }

      set
      {
        if (!this.IsDummy)
        {
          throw new ArgumentOutOfRangeException("value", "Der StundenplaneintragLetzteUnterrichtsstundeIndex kann nicht zugewiesen werden.");
        }

        this.letzteUnterrichtsstundeIndex = value;
        this.RaisePropertyChanged("ErsteUnterrichtsstundeIndex");
      }
    }

    /// <summary>
    /// Holt den Stundenanzahl des Wochenplaneintrags
    /// </summary>
    [DependsUpon("ErsteUnterrichtsstundeIndex")]
    [DependsUpon("LetzteUnterrichtsstundeIndex")]
    public int Stundenanzahl
    {
      get
      {
        return this.LetzteUnterrichtsstundeIndex - this.ErsteUnterrichtsstundeIndex + 1;
      }
    }

    public int RowIndex
    {
      get
      {
        return 0;
      }
    }

    public int ColumnIndex { get; set; }

    public int RowSpan
    {
      get
      {
        return this.Stundenanzahl;
      }
    }

    public int ColumnSpan
    {
      get
      {
        return 1;
      }
    }

    /// <summary>
    /// Holt den Ort des Wochenplaneintrags
    /// </summary>
    public string TerminplaneintragOrt
    {
      get
      {
        if (this.TerminViewModel != null)
        {
          return this.TerminViewModel.TerminOrt;
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Holt die beschreibung des Wochenplaneintrags
    /// </summary>
    public string TerminplaneintragThema
    {
      get
      {
        var thema = string.Empty;

        if (this.TerminViewModel != null)
        {
          thema = this.TerminViewModel.TerminBeschreibung;
        }

        return thema;
      }
    }

    /// <summary>
    /// Holt die Klasse des Wochenplaneintrags
    /// </summary>
    public string TerminplaneintragKlasse
    {
      get
      {
        if (this.TerminViewModel != null)
        {
          if (this.TerminViewModel is StundeViewModel)
          {
            var stunde = this.TerminViewModel as StundeViewModel;
            if (stunde.LerngruppenterminFach == "Vertretungsstunden")
            {
              return "Vertretung";
            }

            return stunde.LerngruppenterminKlasse + "-" + stunde.LerngruppenterminFach;
          }

          if (this.TerminViewModel is LerngruppenterminViewModel)
          {
            var lerngruppentermin = this.TerminViewModel as LerngruppenterminViewModel;
            if (lerngruppentermin.LerngruppenterminFach == "Vertretungsstunden")
            {
              return "Termin";
            }
          }

          if (this.TerminViewModel is SchulterminViewModel)
          {
            var schultermin = this.TerminViewModel as SchulterminViewModel;
            var klassenstring = string.Empty;
            foreach (var betroffeneKlasseViewModel in schultermin.BetroffeneKlassen)
            {
              klassenstring += betroffeneKlasseViewModel.BetroffeneKlasseKlasse.KlasseBezeichnung + ",";
            }

            return klassenstring;
          }

          return this.TerminViewModel.TerminBeschreibung;
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Holt den <see cref="SolidColorBrush"/> to display this wochenplaneintrag
    /// depending on fach and klasse
    /// </summary>
    public SolidColorBrush TerminplaneintragBackground
    {
      get
      {
        if (this.TerminViewModel != null)
        {
          var color = this.TerminViewModel.TerminTermintyp.TermintypKalenderfarbe;
          color.A = 200;
          return new SolidColorBrush(color);
        }

        return Brushes.Transparent;
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Eintrag geprüft ist.
    /// </summary>
    public bool TerminplaneintragIstGeprüft
    {
      get
      {
        if (this.TerminViewModel != null)
        {
          return this.TerminViewModel.TerminIstGeprüft;
        }

        return false;
      }

      set
      {
        if (this.TerminViewModel == null)
        {
          return;
        }

        this.TerminViewModel.TerminIstGeprüft = value;
        this.RaisePropertyChanged("WochenplaneintragIstGeprüft");
      }
    }

    /// <summary>
    /// Holt die imagesource for the proof icon
    /// </summary>
    [DependsUpon("WochenplaneintragIstGeprüft")]
    public ImageSource ProofImage
    {
      get
      {
        return App.GetImageSource(this.TerminplaneintragIstGeprüft ? "ProofValid16.png" : "ProofOpen16.png");
      }
    }

    /// <summary>
    /// Holt die imagesource for the proof icon
    /// </summary>
    [DependsUpon("WochenplaneintragIstGeprüft")]
    public ImageSource ProofImage48
    {
      get
      {
        return App.GetImageSource(this.TerminplaneintragIstGeprüft ? "ProofValid48.png" : "ProofOpen48.png");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return string.Format("TerminplanEintrag: {0} {1}, Raum {2}", this.TerminplaneintragKlasse, this.TerminplaneintragThema, this.TerminplaneintragOrt);
    }

    /// <summary>
    /// Event handler der reagiert, wenn eine Eigenschaft des zugehörigen Termins sich ändert.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An <see cref="PropertyChangedEventArgs"/> with the events data</param>
    private void TerminViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "StundeStundenentwurf" || e.PropertyName == "TerminBeschreibung")
      {
        this.Parent.UpdateProperties(this.wochentagIndex, this.ErsteUnterrichtsstundeIndex, this.Stundenanzahl);
        this.RaisePropertyChanged("TerminplaneintragThema");
      }
    }

    /// <summary>
    /// Handles deletion of the current Wochenplaneintrag
    /// </summary>
    private void RemoveTerminplaneintrag()
    {
      if (this.TerminViewModel != null)
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Terminplaneintrag {0} gelöscht.", this), false))
        {
          var wochenIndex = this.wochentagIndex;
          var ersteStundeIndex = this.ErsteUnterrichtsstundeIndex;
          var stundenzahl = this.Stundenanzahl;

          this.Parent.RemoveTerminplaneintrag(this);
          this.TerminViewModel.DeleteTerminCommand.Execute(null);
          this.TerminViewModel = null;
          this.Parent.UpdateProperties(wochenIndex, ersteStundeIndex, stundenzahl);
          this.RaisePropertyChanged("TerminplaneintragThema");
        }
      }
    }

    /// <summary>
    /// Zeigt den aktuellen Terminplaneintrag
    /// </summary>
    private void ViewTerminplaneintrag()
    {
      if (this.TerminViewModel == null)
      {
        return;
      }


      using (new UndoBatch(App.MainViewModel, string.Format("Wochenplaneintrag {0} editiert.", this), false))
      {
        if (this.TerminViewModel is LerngruppenterminViewModel)
        {
          var lerngruppentermin = this.TerminViewModel as LerngruppenterminViewModel;
          Selection.Instance.Fach =
            App.MainViewModel.Fächer.First(o => o.FachBezeichnung == lerngruppentermin.LerngruppenterminFach);
          Selection.Instance.Klasse =
            App.MainViewModel.Klassen.First(o => o.KlasseBezeichnung == lerngruppentermin.LerngruppenterminKlasse);
          if (lerngruppentermin is StundeViewModel)
          {
            var stunde = lerngruppentermin as StundeViewModel;
            if (stunde.StundeStundenentwurf != null && stunde.StundeStundenentwurf.StundenentwurfModul != null)
            {
              Selection.Instance.Modul = stunde.StundeStundenentwurf.StundenentwurfModul;
            }
          }

          lerngruppentermin.ViewLerngruppenterminCommand.Execute(null);
        }
        else if (this.TerminViewModel is SchulterminViewModel)
        {
          var schultermin = this.TerminViewModel as SchulterminViewModel;
          schultermin.EditSchultermin();
        }

        this.Parent.UpdateProperties(this.wochentagIndex, this.ErsteUnterrichtsstundeIndex, this.Stundenanzahl);
        this.RaisePropertyChanged("TerminplaneintragThema");
      }
    }

    /// <summary>
    /// Handles edit of the current Terminplaneintrag
    /// </summary>
    private void EditTerminplaneintrag()
    {
      if (this.TerminViewModel == null)
      {
        return;
      }

      using (new UndoBatch(App.MainViewModel, string.Format("Wochenplaneintrag {0} editiert.", this), false))
      {
        if (this.TerminViewModel is LerngruppenterminViewModel)
        {
          var lerngruppentermin = this.TerminViewModel as LerngruppenterminViewModel;
          Selection.Instance.Fach =
            App.MainViewModel.Fächer.First(o => o.FachBezeichnung == lerngruppentermin.LerngruppenterminFach);
          Selection.Instance.Klasse =
            App.MainViewModel.Klassen.First(o => o.KlasseBezeichnung == lerngruppentermin.LerngruppenterminKlasse);
          if (lerngruppentermin is StundeViewModel)
          {
            var stunde = lerngruppentermin as StundeViewModel;
            if (stunde.StundeStundenentwurf != null && stunde.StundeStundenentwurf.StundenentwurfModul != null)
            {
              Selection.Instance.Modul = stunde.StundeStundenentwurf.StundenentwurfModul;
            }
          }

          lerngruppentermin.EditLerngruppenterminCommand.Execute(null);
        }
        else if (this.TerminViewModel is SchulterminViewModel)
        {
          var schultermin = this.TerminViewModel as SchulterminViewModel;
          schultermin.EditSchultermin();
        }

        this.Parent.UpdateProperties(this.wochentagIndex, this.ErsteUnterrichtsstundeIndex, this.Stundenanzahl);
        this.RaisePropertyChanged("TerminplaneintragThema");
      }
    }

    /// <summary>
    /// Fragt Noten für diesen Termin ab.
    /// </summary>
    private void AddNoten()
    {
      if (this.TerminViewModel == null)
      {
        return;
      }

      if (!(this.TerminViewModel is StundeViewModel))
      {
        return;
      }

      var stunde = this.TerminViewModel as StundeViewModel;
      stunde.StundeStundenentwurf.StundenentwurfDatum = stunde.LerngruppenterminDatum;
      Selection.Instance.Fach = App.MainViewModel.Fächer.First(o => o.FachBezeichnung == stunde.LerngruppenterminFach);
      Selection.Instance.Klasse = App.MainViewModel.Klassen.First(o => o.KlasseBezeichnung == stunde.LerngruppenterminKlasse);
      Selection.Instance.Stunde = stunde.Model as Stunde;
      Selection.Instance.Stundenentwurf = stunde.StundeStundenentwurf;

      var schülerliste =
        App.MainViewModel.Schülerlisten.FirstOrDefault(
          o =>
          o.SchülerlisteFach.FachBezeichnung == stunde.LerngruppenterminFach
          && o.SchülerlisteJahrtyp.JahrtypBezeichnung == stunde.LerngruppenterminSchuljahr
          && o.SchülerlisteKlasse.KlasseBezeichnung == stunde.LerngruppenterminKlasse);

      if (schülerliste == null)
      {
        return;
      }

      var viewModel = new StundennotenWorkspaceViewModel(schülerliste, stunde);

      bool undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Noten hinzugefügt"), false))
      {
        if (Configuration.Instance.IsMetroMode)
        {
          //  var notenPage = new MetroStundennotenPage();
          //  notenPage.DataContext = viewModel;
          //  Configuration.Instance.NavigationService.Navigate(notenPage);
          //
          var stunden = new ObservableCollection<Stunde>();
          stunden.Add(stunde.Model as Stunde);
          var viewModelReminder = new StundennotenReminderWorkspaceViewModel(stunden);
          var dlg = new MetroStundennotenReminderWindow { DataContext = viewModelReminder };
          dlg.ShowDialog();
        }
        else
        {
          var dlg = new StundennotenDialog();
          dlg.DataContext = viewModel;
          undo = !dlg.ShowDialog().GetValueOrDefault(false);
        }

        stunde.StundeIstBenotet = true;
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
        stunde.StundeIstBenotet = false;
      }
    }

    /// <summary>
    /// Hier wird der Dialog zur Hausaufgabenkontrolle aufgerufen
    /// </summary>
    private async void AddHausaufgaben()
    {
      if (this.TerminViewModel == null)
      {
        return;
      }

      if (!(this.TerminViewModel is StundeViewModel))
      {
        return;
      }

      var stunde = this.TerminViewModel as StundeViewModel;
      var schülerliste =
        App.MainViewModel.Schülerlisten.First(
          o =>
          o.SchülerlisteFach.FachBezeichnung == stunde.LerngruppenterminFach
          && o.SchülerlisteJahrtyp.JahrtypBezeichnung == stunde.LerngruppenterminSchuljahr
          && o.SchülerlisteKlasse.KlasseBezeichnung == stunde.LerngruppenterminKlasse);

      if (Configuration.Instance.IsMetroMode)
      {
        var addDlg = new MetroAddHausaufgabeDialog(stunde.LerngruppenterminDatum);
        var metroWindow = Configuration.Instance.MetroWindow;
        await metroWindow.ShowMetroDialogAsync(addDlg);
        return;
      }

      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgaben hinzugefügt."), false))
      {
        var addDlg = new AddHausaufgabeDialog { Datum = stunde.LerngruppenterminDatum };
        if (addDlg.ShowDialog().GetValueOrDefault(false))
        {
          Selection.Instance.HausaufgabeDatum = addDlg.Datum;
          Selection.Instance.HausaufgabeBezeichnung = addDlg.Bezeichnung;

          // Reset currently selected hausaufgaben
          foreach (var schülereintragViewModel in schülerliste.Schülereinträge)
          {
            schülereintragViewModel.CurrentHausaufgabe = null;
          }

          var dlg = new HausaufgabenDialog { Schülerliste = schülerliste };
          undo = !dlg.ShowDialog().GetValueOrDefault(false);
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Hier wird der Dialog zur Hausaufgabenkontrolle aufgerufen
    /// </summary>
    private async void AddSonstigeNoten()
    {
      if (this.TerminViewModel == null)
      {
        return;
      }

      if (!(this.TerminViewModel is StundeViewModel))
      {
        return;
      }

      var stunde = this.TerminViewModel as StundeViewModel;
      var schülerliste =
        App.MainViewModel.Schülerlisten.First(
          o =>
          o.SchülerlisteFach.FachBezeichnung == stunde.LerngruppenterminFach
          && o.SchülerlisteJahrtyp.JahrtypBezeichnung == stunde.LerngruppenterminSchuljahr
          && o.SchülerlisteKlasse.KlasseBezeichnung == stunde.LerngruppenterminKlasse);

      if (Configuration.Instance.IsMetroMode)
      {
        //var addDlg = new MetroAddHausaufgabeDialog(this.stunde.LerngruppenterminDatum);
        //var metroWindow = Configuration.Instance.MetroWindow;
        //await metroWindow.ShowMetroDialogAsync(addDlg);
        //return;
      }

      var undo = false;
      using (new UndoBatch(App.MainViewModel, string.Format("Sonstige Noten hinzugefügt."), false))
      {
        var addDlg = new AddSonstigeNoteDialog() { Datum = stunde.LerngruppenterminDatum };
        if (addDlg.ShowDialog().GetValueOrDefault(false))
        {
          Selection.Instance.SonstigeNoteDatum = addDlg.Datum;
          Selection.Instance.SonstigeNoteBezeichnung = addDlg.Bezeichnung;
          Selection.Instance.SonstigeNoteNotentyp = addDlg.Notentyp;
          Selection.Instance.SonstigeNoteWichtung = addDlg.Wichtung;

          // Reset currently selected note
          foreach (var schülereintragViewModel in schülerliste.Schülereinträge)
          {
            schülereintragViewModel.CurrentNote = null;
          }

          var dlg = new SonstigeNotenDialog() { Schülerliste = schülerliste };
          undo = !dlg.ShowDialog().GetValueOrDefault(false);
        }
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Ändert den Prüfstatus des Terminplaneintrags.
    /// </summary>
    private void ProofTerminplaneintrag()
    {
      this.TerminplaneintragIstGeprüft = !this.TerminplaneintragIstGeprüft;
    }
  }
}
