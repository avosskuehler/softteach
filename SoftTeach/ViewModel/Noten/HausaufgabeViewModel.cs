namespace SoftTeach.ViewModel.Noten
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Media;

  using MahApps.Metro.Controls.Dialogs;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Noten;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual hausaufgabe
  /// </summary>
  public class HausaufgabeViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="HausaufgabeViewModel"/> Klasse. 
    /// </summary>
    public HausaufgabeViewModel()
    {
      var hausaufgabe = new HausaufgabeNeu();
      hausaufgabe.Datum = DateTime.Now;
      hausaufgabe.Bezeichnung = string.Empty;
      hausaufgabe.IstNachgereicht = false;
      this.Model = hausaufgabe;
      //App.UnitOfWork.Context.Hausaufgaben.Add(hausaufgabe);
      //App.MainViewModel.Hausaufgaben.Add(this);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="HausaufgabeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="hausaufgabe">
    /// The underlying hausaufgabe this ViewModel is to be based on
    /// </param>
    public HausaufgabeViewModel(HausaufgabeNeu hausaufgabe)
    {
      if (hausaufgabe == null)
      {
        throw new ArgumentNullException("hausaufgabe");
      }

      this.Model = hausaufgabe;

      this.EditHausaufgabeCommand = new DelegateCommand(this.EditHausaufgabe);
      this.ChangeHausaufgabeNichtGemachtCommand = new DelegateCommand(this.ChangeHausaufgabeNichtGemacht);
    }

    /// <summary>
    /// Holt das DatenbankmodellHolt den underlying Hausaufgabe this ViewModel is based on
    /// </summary>
    public HausaufgabeNeu Model { get; private set; }

    /// <summary>
    /// Holt das Command zur Änderung einer einzelnen nicht gemachten Hausaufgabe
    /// </summary>
    public DelegateCommand EditHausaufgabeCommand { get; private set; }

    /// <summary>
    /// Holt das Command zur Änderung einer einzelnen nicht gemachten Hausaufgabe
    /// </summary>
    public DelegateCommand ChangeHausaufgabeNichtGemachtCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die Datum
    /// </summary>
    public DateTime HausaufgabeDatum
    {
      get
      {
        return this.Model.Datum;
      }

      set
      {
        if (value == this.Model.Datum) return;
        this.UndoablePropertyChanging(this, "HausaufgabeDatum", this.Model.Datum, value);
        this.Model.Datum = value;
        this.RaisePropertyChanged("HausaufgabeDatum");
      }
    }

    /// <summary>
    /// Holt das Datum der Hausaufgabe als String.
    /// </summary>
    [DependsUpon("HausaufgabeDatum")]
    public string HausaufgabeDatumString
    {
      get
      {
        return this.Model.Datum.ToShortDateString();
      }
    }

    /// <summary>
    /// Holt den Monat des Hausaufgabendatums.
    /// </summary>
    [DependsUpon("HausaufgabeDatum")]
    public string HausaufgabeMonat
    {
      get
      {
        return this.HausaufgabeDatum.ToString("MMMM", new CultureInfo("de-DE"));
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string HausaufgabeBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "HausaufgabeBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("HausaufgabeBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob die Hausaufgabe nachgereicht wurde.
    /// </summary>
    public bool HausaufgabeIstNachgereicht
    {
      get
      {
        return this.Model.IstNachgereicht;
      }

      set
      {
        if (value == this.Model.IstNachgereicht) return;
        this.UndoablePropertyChanging(this, "HausaufgabeIstNachgereicht", this.Model.IstNachgereicht, value);
        this.Model.IstNachgereicht = value;
        this.RaisePropertyChanged("HausaufgabeIstNachgereicht");
      }
    }

    /// <summary>
    /// Holt die Hintergrundfarbe für die Hausaufgabe.
    /// </summary>
    [DependsUpon("HausaufgabeIstNachgereicht")]
    public SolidColorBrush HausaufgabeIstNachgereichtFarbe
    {
      get
      {
        return this.HausaufgabeIstNachgereicht ?
          Application.Current.FindResource("HausaufgabeNachgereichtBrush") as SolidColorBrush :
          Application.Current.FindResource("HausaufgabeNichtGemachtBrush") as SolidColorBrush;
      }
    }

    /// <summary>
    /// Holt den Titel der Hausaufgabe.
    /// </summary>
    public string HausaufgabeTitel
    {
      get
      {
        return
          this.Model.Schülereintrag.Lerngruppe.Fach.Bezeichnung +
          "hausaufgabe von " +
          this.Model.Schülereintrag.Person.Vorname +
          " " +
          this.Model.Schülereintrag.Person.Nachname;
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Hausaufgabe: " + this.HausaufgabeBezeichnung;
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="viewModel">The object to be compared with this instance</param>
    /// <returns>Less than zero if This object is less than the other parameter. 
    /// Zero if This object is equal to other. Greater than zero if This object is greater than other.
    /// </returns>
    public int CompareTo(object viewModel)
    {
      var compareHausaufgabe = viewModel as HausaufgabeViewModel;
      if (compareHausaufgabe == null)
      {
        return -1;
      }

      return this.Model.Datum.CompareTo(compareHausaufgabe.HausaufgabeDatum);
    }

    /// <summary>
    /// Ändert eine Hausaufgabe
    /// </summary>
    private async void EditHausaufgabe()
    {
      if (Configuration.Instance.IsMetroMode)
      {
        var metroWindow = Configuration.Instance.MetroWindow;
        var dialog = new MetroHausaufgabeDialog(this);
        await metroWindow.ShowMetroDialogAsync(dialog);
        return;
      }

      var dlg = new AddHausaufgabeDialog();
      dlg.Datum = this.HausaufgabeDatum;
      dlg.Bezeichnung = this.HausaufgabeBezeichnung;
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgabe {0} geändert.", this), false))
      {
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          this.HausaufgabeDatum = dlg.Datum;
          this.HausaufgabeBezeichnung = dlg.Bezeichnung;
          Selection.Instance.Schülereintrag.UpdateNoten();
        }
      }
    }

    /// <summary>
    /// Ändert den IstnachgereichtStatus der Hausaufgabe.
    /// </summary>
    private void ChangeHausaufgabeNichtGemacht()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Hausaufgabenstatus von {0} geändert.", this), false))
      {
        this.HausaufgabeIstNachgereicht = !this.HausaufgabeIstNachgereicht;
        Selection.Instance.Schülereintrag.UpdateNoten();
      }
    }
  }
}
