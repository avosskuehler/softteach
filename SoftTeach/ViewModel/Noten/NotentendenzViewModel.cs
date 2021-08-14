namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Globalization;
  using System.Linq;

  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Model.EntityFramework;
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
    /// The tendenztyp currently assigned to this Notentendenz
    /// </summary>
    private TendenztypViewModel tendenztyp;

    /// <summary>
    /// The tendenz currently assigned to this Notentendenz
    /// </summary>
    private TendenzViewModel tendenz;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NotentendenzViewModel"/> Klasse. 
    /// </summary>
    public NotentendenzViewModel()
    {
      var notentendenz = new Notentendenz();
      notentendenz.Datum = DateTime.Now;
      notentendenz.Tendenztyp = App.MainViewModel.Tendenztypen.First().Model;
      notentendenz.Tendenz = App.MainViewModel.Tendenzen.First().Model;
      this.Model = notentendenz;
      //App.UnitOfWork.Context.Notentendenzen.Add(notentendenz);
      //App.MainViewModel.Notentendenzen.Add(this);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NotentendenzViewModel"/> Klasse. 
    /// </summary>
    /// <param name="note">
    /// The underlying note this ViewModel is to be based on
    /// </param>
    public NotentendenzViewModel(Notentendenz note)
    {
      if (note == null)
      {
        throw new ArgumentNullException("note");
      }

      this.Model = note;

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
        this.UndoablePropertyChanging(this, "NotentendenzBezeichnung", this.Model.Bezeichnung, value);
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
        this.UndoablePropertyChanging(this, "NotentendenzDatum", this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("NotentendenzDatum");
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
    /// Holt oder setzt die Tendenztyp currently assigned to this note
    /// </summary>
    public TendenztypViewModel NotentendenzTendenztyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Tendenztyp == null)
        {
          return null;
        }

        if (this.tendenztyp == null || this.tendenztyp.Model != this.Model.Tendenztyp)
        {
          this.tendenztyp = App.MainViewModel.Tendenztypen.SingleOrDefault(d => d.Model == this.Model.Tendenztyp);
        }

        return this.tendenztyp;
      }

      set
      {
        if (value.TendenztypBezeichnung == this.tendenztyp.TendenztypBezeichnung) return;
        this.UndoablePropertyChanging(this, "NotentendenzTendenztyp", this.tendenztyp, value);
        this.tendenztyp = value;
        this.Model.Tendenztyp = value.Model;
        this.RaisePropertyChanged("NotentendenzTendenztyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public TendenzViewModel NotentendenzTendenz
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Tendenz == null)
        {
          return null;
        }

        if (this.tendenz == null || this.tendenz.Model != this.Model.Tendenz)
        {
          this.tendenz = App.MainViewModel.Tendenzen.SingleOrDefault(d => d.Model == this.Model.Tendenz);
        }

        return this.tendenz;
      }

      set
      {
        if (value.TendenzBezeichnung == this.tendenz.TendenzBezeichnung) return;
        this.UndoablePropertyChanging(this, "NotentendenzTendenz", this.tendenz, value);
        this.tendenz = value;
        this.Model.Tendenz = value.Model;
        this.RaisePropertyChanged("NotentendenzTendenz");
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
          this.Model.Schülereintrag.Schülerliste.Fach.Bezeichnung +
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
