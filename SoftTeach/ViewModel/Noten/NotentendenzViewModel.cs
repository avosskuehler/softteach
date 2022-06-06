namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Globalization;
  using System.Linq;

  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual note
  /// </summary>
  public class NotentendenzViewModel : ViewModelBase
  {
    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="NotentendenzViewModel"/> Klasse. 
    /// </summary>
    public NotentendenzViewModel()
    {
      var notentendenz = new Notentendenz
      {
        Datum = DateTime.Now,
        Tendenztyp = Tendenztyp.Leistung,
        Tendenz = Tendenz.Null
      };
      this.Model = notentendenz;
    }

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="NotentendenzViewModel"/> Klasse. 
    /// </summary>
    /// <param name="note">
    /// The underlying note this ViewModel is to be based on
    /// </param>
    public NotentendenzViewModel(Notentendenz note)
    {
      this.Model = note ?? throw new ArgumentNullException(nameof(note));

      this.EditNotentendenzCommand = new DelegateCommand(this.EditNotentendenz);
    }

    /// <summary>
    /// Holt den underlying Notentendenz this ViewModel is based on
    /// </summary>
    public Notentendenz Model { get; private set; }

    /// <summary>
    /// Holt das Command zur Änderung einer einzelnen nicht gemachten Notentendenz
    /// </summary>
    public DelegateCommand EditNotentendenzCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string NotentendenzBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(NotentendenzBezeichnung), this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("NotentendenzBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt die Datum
    /// </summary>
    public DateTime NotentendenzDatum
    {
      get
      {
        return this.Model.Datum;
      }

      set
      {
        if (value == this.Model.Datum) return;
        this.UndoablePropertyChanging(this, nameof(NotentendenzDatum), this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("NotentendenzDatum");
      }
    }

    /// <summary>
    /// Holt oder setzt die Notentendenz Tendenz
    /// </summary>
    public Tendenz NotentendenzTendenz
    {
      get
      {
        return this.Model.Tendenz;
      }

      set
      {
        if (value == this.Model.Tendenz) return;
        this.UndoablePropertyChanging(this, nameof(NotentendenzTendenz), this.Model.Tendenz, value);
        this.Model.Tendenz = value;
        this.RaisePropertyChanged("NotentendenzTendenz");
      }
    }

    /// <summary>
    /// Holt oder setzt die Notentendenz Tendenz
    /// </summary>
    public Tendenztyp NotentendenzTendenztyp
    {
      get
      {
        return this.Model.Tendenztyp;
      }

      set
      {
        if (value == this.Model.Tendenztyp) return;
        this.UndoablePropertyChanging(this, nameof(NotentendenzTendenztyp), this.Model.Tendenztyp, value);
        this.Model.Tendenztyp = value;
        this.RaisePropertyChanged("NotentendenzTendenztyp");
      }
    }

    /// <summary>
    /// Holt das Datum der Notentendenz als String.
    /// </summary>
    [DependsUpon("NotentendenzDatum")]
    public string NotentendenzDatumString
    {
      get
      {
        return this.Model.Datum.ToShortDateString();
      }
    }

    /// <summary>
    /// Holt den Monat der Notentendenz.
    /// </summary>
    [DependsUpon("NotentendenzDatum")]
    public string NotentendenzMonat
    {
      get
      {
        return this.NotentendenzDatum.ToString("MMMM", new CultureInfo("de-DE"));
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Notentendenz: " + this.NotentendenzBezeichnung;
    }

    /// <summary>
    /// Gibt dem Titel für diese Notentendenz
    /// </summary>
    /// <value>
    /// The notentendenz titel.
    /// </value>
    public string NotentendenzTitel
    {
      get
      {
        return
          this.Model.Schülereintrag.Lerngruppe.Fach.Bezeichnung +
          " Tendenz von " +
          this.Model.Schülereintrag.Person.Vorname +
          " " +
          this.Model.Schülereintrag.Person.Nachname;
      }
    }

    /// <summary>
    /// Ändert die Notentendenz.
    /// </summary>
    private async void EditNotentendenz()
    {
      if (Configuration.Instance.IsMetroMode)
      {
        var metroWindow = Configuration.Instance.MetroWindow;
        metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
        var dialog = new MetroNotentendenzDialog(this);
        await metroWindow.ShowMetroDialogAsync(dialog);
        return;
      }

      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Notentendenz {0} geändert.", this), false))
      {

        var dlg = new NotentendenzDialog { CurrentNotentendenz = this };
        undo = !dlg.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
      else
      {
        Selection.Instance.Schülereintrag.UpdateNoten();
      }
    }
  }
}
