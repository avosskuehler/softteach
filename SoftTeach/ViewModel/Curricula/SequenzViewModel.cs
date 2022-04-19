namespace SoftTeach.ViewModel.Curricula
{
  using System;
  using System.Linq;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Setting;
  using SoftTeach.View.Curricula;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual sequenz
  /// </summary>
  public class SequenzViewModel : SequencedViewModel
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SequenzViewModel"/> Klasse. 
    /// </summary>
    /// <param name="sequenz">
    /// The underlying sequenz this ViewModel is to be based on
    /// </param>
    public SequenzViewModel(SequenzNeu sequenz)
    {
      if (sequenz == null)
      {
        throw new ArgumentNullException("sequenz");
      }

      this.Model = sequenz;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="SequenzViewModel"/> Klasse. 
    /// </summary>
    /// <param name="reihe">
    /// The reihe.
    /// </param>
    /// <param name="sequenz">
    /// The underlying sequenz this ViewModel is to be based on 
    /// </param>
    public SequenzViewModel(ReiheViewModel reihe, SequenzNeu sequenz)
    {
      if (sequenz == null)
      {
        throw new ArgumentNullException("sequenz");
      }

      this.Model = sequenz;
      this.SequenzReihe = reihe;

      this.EditSequenzCommand = new DelegateCommand(this.EditSequenz);
      this.LengthenSequenzCommand = new DelegateCommand(this.LengthenSequenz);
      this.ShortenSequenzCommand = new DelegateCommand(this.ShortenSequenz);
    }

    /// <summary>
    /// Holt den underlying Sequenz this ViewModel is based on
    /// </summary>
    public SequenzNeu Model { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Bearbeitung der Sequenz
    /// </summary>
    public DelegateCommand EditSequenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur lengthen this sequenz for 1 hour
    /// </summary>
    public DelegateCommand LengthenSequenzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur shorten this sequenz with 1 hour
    /// </summary>
    public DelegateCommand ShortenSequenzCommand { get; private set; }

    /// <summary>
    /// Holt die Reihe that owns this sequenz
    /// </summary>
    public ReiheViewModel SequenzReihe { get; private set; }

    /// <summary>
    /// Holt oder setzt die index for the abfolge in this sequenz
    /// </summary>
    public override int Reihenfolge
    {
      get
      {
        return this.Model.Reihenfolge;
      }

      set
      {
        if (value == this.Model.Reihenfolge)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "Reihenfolge", this.Model.Reihenfolge, value);
        this.Model.Reihenfolge = value;
        this.RaisePropertyChanged("Reihenfolge");
      }
    }

    /// <summary>
    /// Holt oder setzt die Stundenbedarf of this reihe
    /// </summary>
    public int SequenzStundenbedarf
    {
      get
      {
        return this.Model.Stundenbedarf;
      }

      set
      {
        if (value == this.Model.Stundenbedarf)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "SequenzStundenbedarf", this.Model.Stundenbedarf, value);
        this.Model.Stundenbedarf = value;
        this.RaisePropertyChanged("SequenzStundenbedarf");
        var vm = App.MainViewModel.Curricula.FirstOrDefault(o => o.Model == this.Model.Reihe.Curriculum);

        // Wenn zur Übertragung in den Jahresplan ein temporäres Curriculum angelegt wird,
        // dann ist es nicht im MainViewModel
        if (vm != null)
        {
          vm.UpdateUsedStunden();
        }
      }
    }

    /// <summary>
    /// Holt den Stundenbedarf as a string
    /// </summary>
    [DependsUpon("SequenzStundenbedarf")]
    public string SequenzStundenbedarfString
    {
      get
      {
        return this.SequenzStundenbedarf + "h";
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    [DependsUpon("SequenzStundenbedarf")]
    public float SequenzBreite
    {
      get
      {
        var fachstundenanzahl =
  App.MainViewModel.Fachstundenanzahl.FirstOrDefault(
    o =>
    o.FachstundenanzahlFach.FachBezeichnung == Selection.Instance.Fach.FachBezeichnung
    && o.FachstundenanzahlJahrgang == Selection.Instance.Lerngruppe.LerngruppeJahrgang);

        if (fachstundenanzahl == null)
        {
          Console.WriteLine("Keine Fachstundenanzahl gefunden für {0} {1}", Selection.Instance.Fach.FachBezeichnung, Selection.Instance.Lerngruppe.LerngruppeJahrgang);
          return 40;
        }

        var wochenstunden = fachstundenanzahl.FachstundenanzahlStundenzahl
                            + fachstundenanzahl.FachstundenanzahlTeilungsstundenzahl;
        return (float)(this.SequenzStundenbedarf / (float)wochenstunden * Properties.Settings.Default.Wochenbreite);
      }
    }

    /// <summary>
    /// Holt den stundenbedarf als breite
    /// </summary>
    [DependsUpon("SequenzStundenbedarf")]
    public float SequenzDetailBreite
    {
      get
      {
        return this.SequenzBreite * 4;
      }
    }

    /// <summary>
    /// Holt die Breite der Sequenz für die Curriculumsanpassung
    /// </summary>
    [DependsUpon("SequenzStundenbedarf")]
    public int SequenzStundenbreite
    {
      get
      {
        return this.SequenzStundenbedarf * Properties.Settings.Default.Stundenbreite;
      }
    }

    /// <summary>
    /// Holt oder setzt das Thema der Reihe
    /// </summary>
    public string SequenzThema
    {
      get
      {
        return this.Model.Thema;
      }

      set
      {
        if (value == this.Model.Thema)
        {
          return;
        }

        this.UndoablePropertyChanging(this, "SequenzThema", this.Model.Thema, value);
        this.Model.Thema = value;
        this.RaisePropertyChanged("SequenzThema");
      }
    }

    /// <summary>
    /// Holt eine Abkürzung für das Module der Sequenz
    /// </summary>
    [DependsUpon("SequenzReihe")]
    public string SequenzModulAbkürzung
    {
      get
      {
        return this.SequenzReihe.ReiheModul.ModulBezeichnung.Substring(0, 3);
      }
    }

    /// <summary>
    /// Holt eine Kurzbezeichnung für das Thema der Sequenz
    /// </summary>
    [DependsUpon("SequenzThema")]
    public string SequenzKurzbezeichnung
    {
      get
      {
        return this.SequenzThema;
      }
    }

    /// <summary>
    /// Dummy, um Binding-Fehlermeldungen beim Zuweisen von Curricula zu Stunden zu vermeiden
    /// </summary>
    public string LerngruppenterminMonat
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Dummy, um Binding-Fehlermeldungen beim Zuweisen von Curricula zu Stunden zu vermeiden
    /// </summary>
    public DateTime LerngruppenterminDatum
    {
      get
      {
        return new DateTime(2020, 1, 1);
      }
    }

    /// <summary>
    /// Holt die Bezeichnung des Moduls der Sequenz
    /// </summary>
    [DependsUpon("SequenzReihe")]
    public string SequenzModul
    {
      get
      {
        if (this.SequenzReihe != null)
        {
          return this.SequenzReihe.ReiheModul.ModulKurzbezeichnung;
        }

        return "Keine Modulzuordnung";
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.SequenzKurzbezeichnung;
    }

    /// <summary>
    /// Mit dieser Methode wird die aktuelle Sequenz bearbeitet.
    /// </summary>
    private void EditSequenz()
    {
      var dlg = new SequenzDialog { Sequenz = this };
      dlg.ShowDialog();
    }

    /// <summary>
    /// This is the method that is called when the user selects to decrease
    /// the number of stunden this sequenz should last during adaption
    /// of the curriculum
    /// </summary>
    private void ShortenSequenz()
    {
      if (this.SequenzStundenbedarf > 1)
      {
        this.SequenzStundenbedarf--;
      }
    }

    /// <summary>
    /// This is the method that is called when the user selects to increase
    /// the number of stunden this sequenz should last during adaption
    /// of the curriculum
    /// </summary>
    private void LengthenSequenz()
    {
      this.SequenzStundenbedarf++;
    }
  }
}
