namespace SoftTeach.ViewModel.Datenbank
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Globalization;
  using System.Linq;
  using System.Windows.Media;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Schulwoche
  /// </summary>
  public class SchulwocheViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The Schultag currently selected
    /// </summary>
    private SchultagViewModel currentSchultag;

    /// <summary>
    /// The jahrtyp currently assigned to this SchulwocheViewModel
    /// </summary>
    private JahrtypViewModel jahrtyp;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SchulwocheViewModel"/> Klasse. 
    /// </summary>
    /// <param name="schulwoche">
    /// The schulwoche jahrtyp this ViewModel is to be based on
    /// </param>
    public SchulwocheViewModel(Schulwoche schulwoche)
    {
      if (schulwoche == null)
      {
        throw new ArgumentNullException("schulwoche");
      }

      this.Model = schulwoche;

      // Build data structures for schultage
      this.Schultage = new ObservableCollection<SchultagViewModel>();
      foreach (var schultag in schulwoche.Schultage)
      {
        var vm = new SchultagViewModel(schultag);
        App.MainViewModel.Schultage.Add(vm);
        this.Schultage.Add(vm);
      }

      this.Schultage.CollectionChanged += this.SchultageCollectionChanged;
    }

    /// <summary>
    /// Holt die Schultage dieser Schulwoche
    /// </summary>
    public ObservableCollection<SchultagViewModel> Schultage { get; private set; }

    /// <summary>
    /// Holt den underlying Schulwoche this ViewModel is based on
    /// </summary>
    public Schulwoche Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected Schultag
    /// </summary>
    public SchultagViewModel CurrentSchultag
    {
      get
      {
        return this.currentSchultag;
      }

      set
      {
        this.currentSchultag = value;
        this.RaisePropertyChanged("CurrentSchultag");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Stundenentwurf
    /// </summary>
    public JahrtypViewModel SchulwocheJahrtyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Jahrtyp == null)
        {
          return null;
        }

        if (this.jahrtyp == null || this.jahrtyp.Model != this.Model.Jahrtyp)
        {
          this.jahrtyp = App.MainViewModel.Jahrtypen.SingleOrDefault(d => d.Model == this.Model.Jahrtyp);
        }

        return this.jahrtyp;
      }

      set
      {
        this.jahrtyp = value;
        this.Model.Jahrtyp = (value == null) ? null : value.Model;
        this.RaisePropertyChanged("SchulwocheJahrtyp");
      }
    }

    /// <summary>
    /// Holt oder setzt die week index from the beginning
    /// of this schuljahr of this Schulwoche
    /// </summary>
    public int SchulwocheIndex
    {
      get
      {
        var index = 0;
        var ferientyp = App.MainViewModel.Termintypen.First(o => o.TermintypBezeichnung == "Ferien");
        foreach (var schulwoche in this.Model.Jahrtyp.Schulwochen)
        {
          var ferienCount = schulwoche.Schultage.Count(schultag => schultag.Termintyp == ferientyp.Model);

          if (ferienCount < 3)
          {
            index++;
          }

          if (this.Model == schulwoche)
          {
            if (ferienCount > 2)
            {
              index = -1;
            }

            break;
          }
        }

        return index;
      }
    }

    /// <summary>
    /// Holt oder setzt die date of the first day of
    /// this week.
    /// </summary>
    public DateTime SchulwocheMontagsdatum
    {
      get
      {
        return this.Model.Montagsdatum;
      }

      set
      {
        this.Model.Montagsdatum = value;
        this.RaisePropertyChanged("SchulwocheMontagsdatum");
      }
    }

    public string SchulwocheMonat
    {
      get
      {
        return this.SchulwocheMontagsdatum.ToString("MMMM", new CultureInfo("de-DE"));
      }
    }

    public string SchulwocheBeschreibung
    {
      get
      {
        var objCal = CultureInfo.CurrentCulture.Calendar;
        var weekofyear = objCal.GetWeekOfYear(this.SchulwocheMontagsdatum.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        if (this.SchulwocheIndex == -1)
        {
          return "(KW " + weekofyear + ")";
        }

        return this.SchulwocheIndex + ".Woche (KW " + weekofyear + ")";
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    public int SchulwocheBreite
    {
      get
      {
        //var stundenProWoche =
        //  App.MainViewModel.Fachstundenanzahl.First(
        //    o =>
        //    o.FachstundenanzahlFach == Selection.Instance.Fach
        //    && o.FachstundenanzahlKlassenstufe.Model == Selection.Instance.Klasse.Model.Klassenstufe);

        //return (stundenProWoche.FachstundenanzahlStundenzahl + stundenProWoche.FachstundenanzahlTeilungsstundenzahl) * (Properties.Settings.Default.Stundenbreite);
        return Properties.Settings.Default.Wochenbreite;
      }
    }

    public SolidColorBrush MondayBrush
    {
      get
      {
        if (this.Schultage.Count > 0)
        {
          return new SolidColorBrush(this.Schultage[0].SchultagTermintyp.TermintypKalenderfarbe);
        }

        return Brushes.Black;
      }
    }

    public SolidColorBrush TuesdayBrush
    {
      get
      {
        if (this.Schultage.Count > 1)
        {
          return new SolidColorBrush(this.Schultage[1].SchultagTermintyp.TermintypKalenderfarbe);
        }

        return Brushes.Black;
      }
    }

    public SolidColorBrush WednesdayBrush
    {
      get
      {
        if (this.Schultage.Count > 2)
        {
          return new SolidColorBrush(this.Schultage[2].SchultagTermintyp.TermintypKalenderfarbe);
        }

        return Brushes.Black;
      }
    }

    public SolidColorBrush ThursdayBrush
    {
      get
      {
        if (this.Schultage.Count > 3)
        {
          return new SolidColorBrush(this.Schultage[3].SchultagTermintyp.TermintypKalenderfarbe);
        }

        return Brushes.Black;
      }
    }

    public SolidColorBrush FridayBrush
    {
      get
      {
        if (this.Schultage.Count > 4)
        {
          return new SolidColorBrush(this.Schultage[4].SchultagTermintyp.TermintypKalenderfarbe);
        }

        return Brushes.Black;
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      var objCal = CultureInfo.CurrentCulture.Calendar;
      var weekofyear = objCal.GetWeekOfYear(this.SchulwocheMontagsdatum.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
      return weekofyear + ".Woche startend am " + this.SchulwocheMontagsdatum.ToShortDateString();
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
      var compareSchulwoche = viewModel as SchulwocheViewModel;
      if (compareSchulwoche == null)
      {
        return -1;
      }

      return this.Model.Montagsdatum.CompareTo(compareSchulwoche.SchulwocheMontagsdatum);
    }

    /// <summary>
    /// Tritt auf, wenn die SchultageCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SchultageCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Schultage", this.Schultage, e, "Änderung der Schultage");
    }
  }
}
