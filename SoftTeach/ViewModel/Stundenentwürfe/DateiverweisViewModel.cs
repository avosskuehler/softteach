namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Windows.Media;

  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual dateiverweis
  /// </summary>
  public class DateiverweisViewModel : ViewModelBase
  {
    /// <summary>
    /// The dateityp currently assigned to this dateiverweis
    /// </summary>
    private DateitypViewModel dateityp;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="DateiverweisViewModel"/> Klasse. 
    /// </summary>
    /// <param name="dateiverweis">
    /// The underlying dateiverweis this ViewModel is to be based on
    /// </param>
    public DateiverweisViewModel(Dateiverweis dateiverweis)
    {
      if (dateiverweis == null)
      {
        throw new ArgumentNullException("dateiverweis");
      }

      this.Model = dateiverweis;
      this.OpenDateiCommand = new DelegateCommand(this.OpenDatei);
    }

    /// <summary>
    /// Holt den Befehl um die Datei zu öffnen
    /// </summary>
    public DelegateCommand OpenDateiCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Dateiverweis this ViewModel is based on
    /// </summary>
    public Dateiverweis Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Dateiname
    /// </summary>
    public string DateiverweisDateiname
    {
      get
      {
        return this.Model.Dateiname;
      }

      set
      {
        if (value == this.Model.Dateiname) return;
        this.UndoablePropertyChanging(this, "DateiverweisDateiname", this.Model.Dateiname, value);
        this.Model.Dateiname = value;
        this.RaisePropertyChanged("DateiverweisDateiname");
      }
    }

    /// <summary>
    /// Holt den Dateiname of this dateiverweis without path.
    /// </summary>
    [DependsUpon("DateiverweisDateiname")]
    public string DateiverweisDateinameOhnePfad
    {
      get
      {
        return Path.GetFileName(this.Model.Dateiname);
      }
    }

    /// <summary>
    /// Holt den Bild for this dateiverweis
    /// </summary>
    [DependsUpon("DateiverweisDateityp")]
    public ImageSource DateiverweisBild
    {
      get
      {
        switch (this.DateiverweisDateityp.DateitypKürzel)
        {
          case "OH":
            return App.GetImageSource("Folie32.png");
          case "SP":
            return App.GetImageSource("Spiel32.png");
          case "AB":
            return App.GetImageSource("Arbeitsblatt32.png");
          case "KL":
            return App.GetImageSource("Klausur32.png");
          case "KA":
            return App.GetImageSource("Klassenarbeit32.png");
          case "TE":
            return App.GetImageSource("Test32.png");
          case "TÜ":
            return App.GetImageSource("TäglicheÜbungen32.png");
          case "TA":
            return App.GetImageSource("Tandembogen32.png");
          case "PR":
            return App.GetImageSource("Projekt32.png");
          case "PP":
            return App.GetImageSource("Präsentation32.png");
          case "WP":
            return App.GetImageSource("Wochenplan32.png");
          case "HA":
            return App.GetImageSource("Hausaufgabe32.png");
          case "UE":
            return App.GetImageSource("Unterrichtsentwurf32.png");
        }

        return App.GetImageSource("Unbekannt32.png");
      }
    }

    /// <summary>
    /// Holt den Overlay for this dateiverweis
    /// </summary>
    [DependsUpon("DateiverweisDateityp")]
    public ImageSource DateiverweisBildOverlay
    {
      get
      {
        switch (Path.GetExtension(this.DateiverweisDateinameOhnePfad))
        {
          case ".pdf":
            return App.GetImageSource("AdobeOverlay32.png");
          case ".txt":
          case ".doc":
          case ".docx":
            return App.GetImageSource("WordOverlay32.png");
          case ".ppt":
          case ".pptx":
            return App.GetImageSource("PowerpointOverlay32.png");
          case ".xls":
          case ".xlsx":
            return App.GetImageSource("ExcelOverlay32.png");
          case ".odt":
            return App.GetImageSource("OpenTextOverlay32.png");
          case ".odp":
            return App.GetImageSource("OpenPräsentationOverlay32.png");
          case ".ggb":
            return App.GetImageSource("GeogebraOverlay32.png");
          case ".psd":
            return App.GetImageSource("PhotoshopOverlay32.png");
          case ".png":
          case ".jpg":
          case ".bmp":
            return App.GetImageSource("ImageOverlay32.png");
          case ".avi":
          case ".mp4":
          case ".wmv":
            return App.GetImageSource("VideoOverlay32.png");
          case ".html":
          case ".htm":
            return App.GetImageSource("InternetOverlay32.png");
        }

        return App.GetImageSource("UnbekanntOverlay32.png");
      }
    }

    /// <summary>
    /// Holt oder setzt die halbjahr currently assigned to this Termin
    /// </summary>
    public DateitypViewModel DateiverweisDateityp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Dateityp == null)
        {
          return null;
        }

        if (this.dateityp == null || this.dateityp.Model != this.Model.Dateityp)
        {
          this.dateityp = App.MainViewModel.Dateitypen.SingleOrDefault(d => d.Model == this.Model.Dateityp);
        }

        return this.dateityp;
      }

      set
      {
        if (value.DateitypBezeichnung == this.dateityp.DateitypBezeichnung) return;
        this.UndoablePropertyChanging(this, "DateiverweisDateityp", this.dateityp, value);
        this.dateityp = value;
        this.Model.Dateityp = value.Model;
        this.RaisePropertyChanged("DateiverweisDateityp");
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Dateiverweis: " + this.DateiverweisDateinameOhnePfad;
    }

    /// <summary>
    /// Öffnet die Datei
    /// </summary>
    private void OpenDatei()
    {
      // Wenn die Datei verschoben wurde, aber nicht zu finden ist, suche unter Standardverzeichnis
      if (!File.Exists(this.DateiverweisDateiname))
      {
        var resolvedPfad = StundenentwurfViewModel.GetDateiverweispfad(this.Model.Stundenentwurf);
        var newLocationTest = Path.Combine(resolvedPfad, this.DateiverweisDateinameOhnePfad);
        if (File.Exists(newLocationTest)) this.DateiverweisDateiname = newLocationTest;
      }

      App.OpenFile(this.DateiverweisDateiname);
    }
  }
}
