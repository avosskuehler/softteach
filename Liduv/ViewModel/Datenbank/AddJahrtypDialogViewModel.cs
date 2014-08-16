namespace Liduv.ViewModel.Datenbank
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;

  using Liduv.ExceptionHandling;
  using Liduv.Model;
  using Liduv.Model.EntityFramework;
  using Liduv.Setting;
  using Liduv.UndoRedo;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual ferien
  /// </summary>
  public class AddJahrtypDialogViewModel : ViewModelBase
  {
    /// <summary>
    /// The Ferien currently selected
    /// </summary>
    private FerienViewModel currentFerien;

    /// <summary>
    /// Die Bezeichnung des neuen Schuljahrs
    /// </summary>
    private string schuljahrBezeichnung;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="AddJahrtypDialogViewModel"/> Klasse. 
    /// </summary>
    public AddJahrtypDialogViewModel()
    {
      this.Ferien = new ObservableCollection<FerienViewModel>();
      this.CreateNewJahrtyp();
    }

    /// <summary>
    /// Holt die Ferien für das Schuljahr
    /// </summary>
    public ObservableCollection<FerienViewModel> Ferien { get; private set; }

    /// <summary>
    /// Holt den jahrtyp des Dialogs
    /// </summary>
    public JahrtypViewModel Jahrtyp { get; private set; }

    /// <summary>
    /// Holt oder setzt die ferien currently selected in this workspace
    /// </summary>
    public FerienViewModel CurrentFerien
    {
      get
      {
        return this.currentFerien;
      }

      set
      {
        this.currentFerien = value;
        this.RaisePropertyChanged("CurrentFerien");
      }
    }

    /// <summary>
    /// Holt oder setzt die Schuljahrbezeichnung
    /// </summary>
    public string Schuljahrbezeichnung
    {
      get
      {
        return this.schuljahrBezeichnung;
      }

      set
      {
        this.schuljahrBezeichnung = value;
        this.RaisePropertyChanged("Schuljahrbezeichnung");
      }
    }

    /// <summary>
    /// Handles addition a new Ferien to the workspace and model
    /// </summary>
    private void AddFerien(string bezeichnung, DateTime starttermin, int dauerInTagen)
    {
      var ferien = new Ferien();
      ferien.Jahrtyp = this.Jahrtyp.Model;
      ferien.Bezeichnung = bezeichnung;
      ferien.ErsterFerientag = starttermin;
      ferien.LetzterFerientag = starttermin.AddDays(dauerInTagen);
      var vm = new FerienViewModel(ferien);
      this.Ferien.Add(vm);
      this.CurrentFerien = vm;
    }

    private void CreateNewJahrtyp()
    {
      var nextYear = DateTime.Now;
      if (App.MainViewModel.Jahrtypen.Count >= 0)
      {
        var lastEntry = App.MainViewModel.Jahrtypen[App.MainViewModel.Jahrtypen.Count - 1];
        var lastEntrysYear = lastEntry.JahrtypJahr;
        nextYear = new DateTime(lastEntrysYear, 1, 1).AddYears(1);
      }

      var bezeichnung = nextYear.ToString("yyyy") + "/" + nextYear.AddYears(1).ToString("yyyy");
      this.Schuljahrbezeichnung = bezeichnung;

      // Check for existing jahresplan
      if (App.MainViewModel.Jahrtypen.Any(vorhandenerJahrtyp => vorhandenerJahrtyp.JahrtypJahr == nextYear.Year))
      {
        Log.ProcessMessage(
          "Schuljahr bereits vorhanden",
          string.Format("Das Schuljahr {0} ist bereits in der Datenbank vorhanden und kann nicht doppelt angelegt werden.", bezeichnung));
        return;
      }

      var jahrtyp = new Jahrtyp { Bezeichnung = bezeichnung, Jahr = nextYear.Year };
      var vm = new JahrtypViewModel(jahrtyp);
      this.Jahrtyp = vm;
      App.MainViewModel.Jahrtypen.Add(vm);

      Selection.Instance.Jahrtyp = this.Jahrtyp;

      var herbstTermin = new DateTime(this.Jahrtyp.JahrtypJahr, 10, 1);
      this.AddFerien("Herbstferien", herbstTermin, 14);
      var weihnachtTermin = new DateTime(this.Jahrtyp.JahrtypJahr, 12, 20);
      this.AddFerien("Weihnachtsferien", weihnachtTermin, 10);
      var winterTermin = new DateTime(this.Jahrtyp.JahrtypJahr + 1, 2, 1);
      this.AddFerien("Winterferien", winterTermin, 7);
      var osterTermin = new DateTime(this.Jahrtyp.JahrtypJahr + 1, 4, 15);
      this.AddFerien("Osterferien", osterTermin, 14);
      var himmelTermin = new DateTime(this.Jahrtyp.JahrtypJahr + 1, 5, 1);
      this.AddFerien("Himmelfahrt", himmelTermin, 3);
      var pfingstTermin = new DateTime(this.Jahrtyp.JahrtypJahr + 1, 5, 15);
      this.AddFerien("Pfingsten", pfingstTermin, 3);
      var sommerTermin = new DateTime(this.Jahrtyp.JahrtypJahr + 1, 7, 1);
      this.AddFerien("Sommerferien", sommerTermin, 42);
    }
  }
}
