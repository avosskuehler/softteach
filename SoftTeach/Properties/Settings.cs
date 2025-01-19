namespace SoftTeach.Properties
{
  using System.IO;
  using System.Windows.Media;
  using SoftTeach.Setting;

  // This class allows you to handle specific events on the settings class:
  //  The SettingChanging event is raised before a setting's value is changed.
  //  The PropertyChanged event is raised after a setting's value is changed.
  //  The SettingsLoaded event is raised after the setting values are loaded.
  //  The SettingsSaving event is raised before the setting values are saved.
  internal sealed partial class Settings
  {
    public static Color DefaultBasisfarbe = (Color)ColorConverter.ConvertFromString("#FF008A00");
    private Color basisfarbe;

    public Settings()
    {

      // Setup LogfilePath
      this.LogfilePath = Configuration.GetLocalApplicationDataPath() +
        Path.DirectorySeparatorChar + "Logs" +
        Path.DirectorySeparatorChar;

      // Create directory if not already existing.
      if (!Directory.Exists(this.LogfilePath))
      {
        Directory.CreateDirectory(this.LogfilePath);
      }

      this.basisfarbe = DefaultBasisfarbe;
    }

    /// <summary>
    /// Holt oder setzt eine Einstellung, die die Basisfarbe der Anwendung bestimmt
    /// </summary>
    public Color Basisfarbe
    {
      get => this.basisfarbe;

      set
      {
        this.basisfarbe = value;
      }
    }

  }
}
