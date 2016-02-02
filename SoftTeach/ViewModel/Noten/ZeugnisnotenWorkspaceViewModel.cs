namespace SoftTeach.ViewModel.Noten
{
  using System;

  using Helper;

  using Personen;
  using Setting;

  /// <summary>
  /// ViewModel für den Zeugnisnotendialog.
  /// </summary>
  public class ZeugnisnotenWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Schülerliste currently selected
    /// </summary>
    private SchülerlisteViewModel currentSchülerliste;

    /// <summary>
    /// The Schülereintrag currently selected
    /// </summary>
    private SchülereintragViewModel currentSchülereintrag;

    private ZeugnisnotenTyp zeugnisnotenTyp;

    private DateTime notendatum;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ZeugnisnotenWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public ZeugnisnotenWorkspaceViewModel(SchülerlisteViewModel schülerliste)
    {
      this.CurrentSchülerliste = schülerliste;
      if (this.CurrentSchülerliste != null)
      {
        this.CurrentSchülereintrag = this.currentSchülerliste.CurrentSchülereintrag;
      }

      //this.AddHausaufgabenCommand = new DelegateCommand(this.AddHausaufgaben);
      //this.AddSonstigeNotenCommand = new DelegateCommand(this.AddSonstigeNoten);
      //this.PrintNotenlisteCommand = new DelegateCommand(this.PrintNotenliste);
      //this.AddZeugnisnotenCommand = new DelegateCommand(this.AddZeugnisnoten);
    }

    ///// <summary>
    ///// Holt den Befehl, um fehlende Hausaufgaben einzutragen.
    ///// </summary>
    //public DelegateCommand AddHausaufgabenCommand { get; private set; }

    ///// <summary>
    ///// Holt den Befehl, um sonstige Noten anzulegen.
    ///// </summary>
    //public DelegateCommand AddSonstigeNotenCommand { get; private set; }

    ///// <summary>
    ///// Holt den Befehl, um die Notenliste der aktuellen Schülerliste auszudrucken
    ///// </summary>
    //public DelegateCommand PrintNotenlisteCommand { get; private set; }

    ///// <summary>
    ///// Holt den Befehl, um Zeugnisnoten zu machen
    ///// </summary>
    //public DelegateCommand AddZeugnisnotenCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die Schülereintrag currently selected in this workspace
    /// </summary>
    public SchülereintragViewModel CurrentSchülereintrag
    {
      get
      {
        return this.currentSchülereintrag;
      }

      set
      {
        this.currentSchülereintrag = value;
        Selection.Instance.Schülereintrag = value;
        this.RaisePropertyChanged("CurrentSchülereintrag");
      }
    }

    /// <summary>
    /// Holt oder setzt die Schülerliste currently selected in this workspace
    /// </summary>
    public SchülerlisteViewModel CurrentSchülerliste
    {
      get
      {
        return this.currentSchülerliste;
      }

      set
      {
        this.currentSchülerliste = value;
        Selection.Instance.Schülerliste = value;
        this.RaisePropertyChanged("CurrentSchülerliste");
      }
    }

    public ZeugnisnotenTyp ZeugnisnotenTyp
    {
      get
      {
        return this.zeugnisnotenTyp;
      }
      set
      {
        this.zeugnisnotenTyp = value;
        this.RaisePropertyChanged("ZeugnisnotenTyp");
      }
    }

    /// <summary>
    /// Gets the datums label.
    /// </summary>
    /// <value>The datums label.</value>
    [DependsUpon("ZeugnisnotenTyp")]
    public string DatumsLabel
    {
      get
      {
        switch (this.zeugnisnotenTyp)
        {
          case ZeugnisnotenTyp.Zwischenstand:
            return "Datum des Zwischenstands";
          case ZeugnisnotenTyp.Halbjahr:
            return "Datum des Halbjahreszeugnisses";
          case ZeugnisnotenTyp.Ganzjahr:
            return "Datum des Zeugnisses für das ganze Jahr";
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the notendatum.
    /// </summary>
    /// <value>The notendatum.</value>
    public DateTime Notendatum
    {
      get
      {
        return this.notendatum;
      }

      set
      {
        this.notendatum = value;
        this.RaisePropertyChanged("Notendatum");
      }
    }
  }
}
