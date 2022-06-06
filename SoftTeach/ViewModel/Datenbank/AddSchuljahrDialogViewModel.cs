namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;

  using SoftTeach.UndoRedo;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual ferien
  /// </summary>
  public class AddSchuljahrDialogViewModel : ViewModelBase
  {
    /// <summary>
    /// The Ferien currently selected
    /// </summary>
    private FerienViewModel currentFerien;

    /// <summary>
    /// Die Bezeichnung des en Schuljahrs
    /// </summary>
    private string schuljahrBezeichnung;

    /// <summary>
    /// Initialisiert eine e Instanz der <see cref="AddSchuljahrDialogViewModel"/> Klasse. 
    /// </summary>
    public AddSchuljahrDialogViewModel()
    {
      this.Ferien = new ObservableCollection<FerienViewModel>();
      this.CreateNewSchuljahr();
    }

    /// <summary>
    /// Holt die Ferien für das Schuljahr
    /// </summary>
    public ObservableCollection<FerienViewModel> Ferien { get; private set; }

    /// <summary>
    /// Holt den schuljahr des Dialogs
    /// </summary>
    public SchuljahrViewModel Schuljahr { get; private set; }

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
      using (new UndoBatch(App.MainViewModel, string.Format("Ferien angelegt kopiert"), false))
      {
        var ferien = new Ferien
        {
          Schuljahr = this.Schuljahr.Model,
          Bezeichnung = bezeichnung,
          ErsterFerientag = starttermin,
          LetzterFerientag = starttermin.AddDays(dauerInTagen)
        };
        //App.UnitOfWork.Context.Ferien.Add(ferien);
        var vm = new FerienViewModel(ferien);
        this.Ferien.Add(vm);
        this.CurrentFerien = vm;
      }
    }

    private void CreateNewSchuljahr()
    {
      using (new UndoBatch(App.MainViewModel, string.Format("es Schuljahr angelegt."), false))
      {
        var nextYear = DateTime.Now;
        if (App.MainViewModel.Schuljahre.Count >= 0)
        {
          var lastEntry = App.MainViewModel.Schuljahre[App.MainViewModel.Schuljahre.Count - 1];
          var lastEntrysYear = lastEntry.SchuljahrJahr;
          nextYear = new DateTime(lastEntrysYear, 1, 1).AddYears(1);
        }

        var bezeichnung = nextYear.ToString("yyyy") + "/" + nextYear.AddYears(1).ToString("yyyy");
        this.Schuljahrbezeichnung = bezeichnung;

        // Check for existing jahresplan
        if (App.MainViewModel.Schuljahre.Any(vorhandenerSchuljahr => vorhandenerSchuljahr.SchuljahrJahr == nextYear.Year))
        {
          Log.ProcessMessage(
            "Schuljahr bereits vorhanden",
            string.Format("Das Schuljahr {0} ist bereits in der Datenbank vorhanden und kann nicht doppelt angelegt werden.", bezeichnung));
          return;
        }

        var schuljahr = new Schuljahr { Bezeichnung = bezeichnung, Jahr = nextYear.Year };
        //App.UnitOfWork.Context.Schuljahre.Add(schuljahr);
        var vm = new SchuljahrViewModel(schuljahr);
        this.Schuljahr = vm;
        App.MainViewModel.Schuljahre.Add(vm);

        Selection.Instance.Schuljahr = this.Schuljahr;

        var herbstTermin = new DateTime(this.Schuljahr.SchuljahrJahr, 10, 1);
        this.AddFerien("Herbstferien", herbstTermin, 14);
        var weihnachtTermin = new DateTime(this.Schuljahr.SchuljahrJahr, 12, 20);
        this.AddFerien("Weihnachtsferien", weihnachtTermin, 10);
        var winterTermin = new DateTime(this.Schuljahr.SchuljahrJahr + 1, 2, 1);
        this.AddFerien("Winterferien", winterTermin, 7);
        var osterTermin = new DateTime(this.Schuljahr.SchuljahrJahr + 1, 4, 15);
        this.AddFerien("Osterferien", osterTermin, 14);
        var himmelTermin = new DateTime(this.Schuljahr.SchuljahrJahr + 1, 5, 1);
        this.AddFerien("Himmelfahrt", himmelTermin, 3);
        var pfingstTermin = new DateTime(this.Schuljahr.SchuljahrJahr + 1, 5, 15);
        this.AddFerien("Pfingsten", pfingstTermin, 3);
        var sommerTermin = new DateTime(this.Schuljahr.SchuljahrJahr + 1, 7, 1);
        this.AddFerien("Sommerferien", sommerTermin, 42);
      }
    }
  }
}
