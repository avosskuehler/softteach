namespace SoftTeach.ViewModel.Stundenentwürfe
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Media;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;
  using SoftTeach.ViewModel.Termine;

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
    /// Initialisiert eine e Instanz der <see cref="DateiverweisViewModel"/> Klasse. 
    /// </summary>
    /// <param name="dateiverweis">
    /// The underlying dateiverweis this ViewModel is to be based on
    /// </param>
    public DateiverweisViewModel(Dateiverweis dateiverweis)
    {
      this.Model = dateiverweis ?? throw new ArgumentNullException(nameof(dateiverweis));
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
        this.UndoablePropertyChanging(this, nameof(DateiverweisDateiname), this.Model.Dateiname, value);
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
    /// Holt den Dateiname of this dateiverweis without path.
    /// </summary>
    [DependsUpon("DateiverweisDateiname")]
    public string DateiverweisPfad
    {
      get
      {
        return Path.GetDirectoryName(this.Model.Dateiname);
      }
    }

    /// <summary>
    /// Holt den Bild for this dateiverweis
    /// </summary>
    [DependsUpon("DateiverweisDateityp")]
    public Style DateiverweisBild
    {
      get
      {
        switch (this.DateiverweisDateityp.DateitypKürzel)
        {
          case "OH":
            return App.GetIconStyle("Projektor32");
          case "SP":
            return App.GetIconStyle("Spiel32");
          case "AB":
            return App.GetIconStyle("Arbeitsblatt32");
          case "KL":
          case "KA":
          case "TE":
            return App.GetIconStyle("Klausur32");
          case "TÜ":
            return App.GetIconStyle("TäglicheÜbungen32");
          case "TA":
            return App.GetIconStyle("Tandembogen32");
          case "PR":
            return App.GetIconStyle("Projekt32");
          case "PP":
            return App.GetIconStyle("Präsentation32");
          case "WP":
            return App.GetIconStyle("Wochenplan32");
          case "HA":
            return App.GetIconStyle("Hausaufgabe32");
          case "UE":
            return App.GetIconStyle("Unterrichtsentwurf32");
        }

        return App.GetIconStyle("Unbekannt32");
      }
    }

    /// <summary>
    /// Holt den Overlay for this dateiverweis
    /// </summary>
    [DependsUpon("DateiverweisDateityp")]
    public Style DateiverweisBildOverlay
    {
      get
      {
        switch (Path.GetExtension(this.DateiverweisDateinameOhnePfad))
        {
          case ".pdf":
            return App.GetIconStyle("AdobeOverlay32");
          case ".txt":
          case ".doc":
          case ".docx":
            return App.GetIconStyle("WordOverlay32");
          case ".ppt":
          case ".pptx":
            return App.GetIconStyle("PowerpointOverlay32");
          case ".xls":
          case ".xlsx":
            return App.GetIconStyle("ExcelOverlay32");
          case ".odt":
            return App.GetIconStyle("OpenTextOverlay32");
          case ".odp":
            return App.GetIconStyle("OpenPräsentationOverlay32");
          case ".ggb":
            return App.GetIconStyle("GeogebraOverlay32");
          case ".psd":
            return App.GetIconStyle("PhotoshopOverlay32");
          case ".png":
          case ".jpg":
          case ".bmp":
            return App.GetIconStyle("ImageOverlay32");
          case ".avi":
          case ".mp4":
          case ".wmv":
            return App.GetIconStyle("VideoOverlay32");
          case ".html":
          case ".htm":
            return App.GetIconStyle("InternetOverlay32");
        }

        return App.GetIconStyle("UnbekanntOverlay32");
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
        this.UndoablePropertyChanging(this, nameof(DateiverweisDateityp), this.dateityp, value);
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
        InformationDialog.Show("Datei nicht gefunden", string.Format("Die Datei wurde nicht am angegebenen Ort {0} gefunden", this.DateiverweisDateiname), false);
        return;
        //var resolvedPfad = StundeViewModel.GetDateiverweispfad(this.Model.Stunde);
        //var newLocationTest = Path.Combine(resolvedPfad, this.DateiverweisDateinameOhnePfad);
        //if (File.Exists(newLocationTest)) this.DateiverweisDateiname = newLocationTest;
      }

      App.OpenFile(this.DateiverweisDateiname);
    }
  }
}
