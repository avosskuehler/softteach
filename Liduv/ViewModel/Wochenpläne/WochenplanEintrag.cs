namespace Liduv.ViewModel.Wochenpläne
{
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Media;

  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.View.Noten;
  using Liduv.ViewModel.Helper;
  using Liduv.ViewModel.Noten;
  using Liduv.ViewModel.Termine;

  /// <summary>
  /// Für jeden Termin der Woche (Unterrichtsstunde, AG, Treffen) wird ein Wochenplaneintrag erstellt.
  /// Er erscheint im Wochenplan.
  /// </summary>
  public class WochenplanEintrag : ViewModelBase
  {
    /// <summary>
    /// Der 0-basierte Index des Wochentags für diesen Wochenplaneintrag
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

    private TerminViewModel terminViewModel;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="WochenplanEintrag"/> Klasse. 
    /// </summary>
    /// <param name="parent"> The underlying stundenplaneintrag this ViewModel is to be based on </param>
    /// <param name="terminViewModel"> The termin View Model. </param>
    public WochenplanEintrag(WochenplanWorkspaceViewModel parent, TerminViewModel terminViewModel)
    {
      if (parent == null)
      {
        throw new ArgumentNullException("parent");
      }

      this.TerminViewModel = terminViewModel;
      this.Parent = parent;
      this.EditWochenplaneintragCommand = new DelegateCommand(this.EditWochenplaneintrag, () => this.TerminViewModel != null);
      this.RemoveWochenplaneintragCommand = new DelegateCommand(this.RemoveWochenplaneintrag, () => this.TerminViewModel != null);
      this.ProofWochenplaneintragCommand = new DelegateCommand(this.ProofWochenplaneintrag, () => this.TerminViewModel != null);
      this.AddNotenCommand = new DelegateCommand(this.AddNoten, () => this.TerminViewModel != null);
    }

    /// <summary>
    /// Holt den Befehl zur proof or unproof the current Wochenplaneintrag
    /// </summary>
    public DelegateCommand ProofWochenplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur editing the current Wochenplaneintrag
    /// </summary>
    public DelegateCommand EditWochenplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur removing the current Wochenplaneintrag
    /// </summary>
    public DelegateCommand RemoveWochenplaneintragCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl Noten einzutragen
    /// </summary>
    public DelegateCommand AddNotenCommand { get; private set; }

    /// <summary>
    /// Holt the parent <see cref="WochenplanWorkspaceViewModel"/> to which this WochenplanEintrag
    /// is added to.
    /// </summary>
    public WochenplanWorkspaceViewModel Parent { get; private set; }

    /// <summary>
    /// Holt oder setzt den Termin, der zu diesem Wochenplaneintrag gehört.
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
    /// Holt einen Wert, der angibt, ob dieser Wochenplaneintrag leer ist (ohne Termine).
    /// </summary>
    public bool IsDummy
    {
      get
      {
        return this.TerminViewModel == null;
      }
    }

    /// <summary>
    /// Holt oder setzt den 0-basierte Index des Wochentags für diesen Wochenplaneintrag
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

    /// <summary>
    /// Holt den Ort des Wochenplaneintrags
    /// </summary>
    public string WochenplaneintragOrt
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
    public string WochenplaneintragThema
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
    public string WochenplaneintragKlasse
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
    public SolidColorBrush WochenplaneintragBackground
    {
      get
      {
        if (this.TerminViewModel != null)
        {
          return new SolidColorBrush(this.TerminViewModel.TerminTermintyp.TermintypKalenderfarbe);
        }

        return Brushes.Transparent;
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob dieser Eintrag geprüft ist.
    /// </summary>
    public bool WochenplaneintragIstGeprüft
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
        return App.GetImageSource(this.WochenplaneintragIstGeprüft ? "ProofValid16.png" : "ProofOpen16.png");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return string.Format("WochenplanEintrag: {0} {1}, Raum {2}", this.WochenplaneintragKlasse, this.WochenplaneintragThema, this.WochenplaneintragOrt);
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
        this.RaisePropertyChanged("WochenplaneintragThema");
      }
    }

    /// <summary>
    /// Handles deletion of the current Wochenplaneintrag
    /// </summary>
    private void RemoveWochenplaneintrag()
    {
      if (this.TerminViewModel != null)
      {
        using (new UndoBatch(App.MainViewModel, string.Format("Wochenplaneintrag {0} gelöscht.", this), false))
        {
          var wochenIndex = this.wochentagIndex;
          var ersteStundeIndex = this.ErsteUnterrichtsstundeIndex;
          var stundenzahl = this.Stundenanzahl;

          this.Parent.RemoveWochenplaneintrag(this);
          this.TerminViewModel.DeleteTerminCommand.Execute(null);
          this.TerminViewModel = null;
          this.Parent.UpdateProperties(wochenIndex, ersteStundeIndex, stundenzahl);
          this.RaisePropertyChanged("WochenplaneintragThema");
        }
      }
    }

    /// <summary>
    /// Handles edit of the current Wochenplaneintrag
    /// </summary>
    private void EditWochenplaneintrag()
    {
      if (this.TerminViewModel == null)
      {
        return;
      }

      //using (new UndoBatch(App.MainViewModel, string.Format("Wochenplaneintrag {0} editiert.", this), false))
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
        this.RaisePropertyChanged("WochenplaneintragThema");
      }
    }

    /// <summary>
    /// Fragt Noten für diese Stunde ab.
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
      Selection.Instance.Fach = App.MainViewModel.Fächer.First(o => o.FachBezeichnung == stunde.LerngruppenterminFach);
      Selection.Instance.Klasse = App.MainViewModel.Klassen.First(o => o.KlasseBezeichnung == stunde.LerngruppenterminKlasse);
      Selection.Instance.Stundenentwurf = stunde.StundeStundenentwurf;

      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Wochenplaneintrag {0} gelöscht.", this), false))
      {
        var dlg = new StundennotenDialog();
        var schülerliste =
          App.MainViewModel.Schülerlisten.First(
            o =>
            o.SchülerlisteFach.FachBezeichnung == stunde.LerngruppenterminFach
            && o.SchülerlisteHalbjahrtyp.HalbjahrtypBezeichnung == stunde.LerngruppenterminHalbjahr
            && o.SchülerlisteJahrtyp.JahrtypBezeichnung == stunde.LerngruppenterminSchuljahr
            && o.SchülerlisteKlasse.KlasseBezeichnung == stunde.LerngruppenterminKlasse);
        var viewModel = new StundennotenWorkspaceViewModel(schülerliste, stunde);
        dlg.DataContext = viewModel;
        undo = dlg.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Ändert den Prüfstatus des Wochenplaneintrags.
    /// </summary>
    private void ProofWochenplaneintrag()
    {
      this.WochenplaneintragIstGeprüft = !this.WochenplaneintragIstGeprüft;
    }
  }
}
