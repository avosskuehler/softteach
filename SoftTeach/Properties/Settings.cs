﻿namespace SoftTeach.Properties
{
  using System.IO;

  using SoftTeach.Setting;

  // This class allows you to handle specific events on the settings class:
  //  The SettingChanging event is raised before a setting's value is changed.
  //  The PropertyChanged event is raised after a setting's value is changed.
  //  The SettingsLoaded event is raised after the setting values are loaded.
  //  The SettingsSaving event is raised before the setting values are saved.
  internal sealed partial class Settings
  {

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
    }
  }
}