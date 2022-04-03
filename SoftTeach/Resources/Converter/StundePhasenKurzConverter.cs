namespace SoftTeach.Resources.Converter
{
  using SoftTeach.Model.TeachyModel;
  using System;
  using System.Globalization;
  using System.Windows.Data;

  /// <summary>
  /// Konvertiert einen Dezimalwert in einen Prozentwert und zurück,
  /// inklusive %-Zeichen Formatierung
  /// </summary>
  [ValueConversion(typeof(StundeNeu), typeof(string))]
  public class StundePhasenKurzConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Converts a value
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is StundeNeu))
      {
        throw new InvalidOperationException("The source must be a StundeNeu");
      }

      var valueToConvert = (StundeNeu)value;
      var kurzform = string.Empty;
      foreach (var phaseViewModel in valueToConvert.Phasen)
      {
        var inhalt = phaseViewModel.Inhalt;
        kurzform += phaseViewModel.Zeit + "' ";
        kurzform += phaseViewModel.Sozialform + ": ";
        kurzform += inhalt.Length > 0 ? inhalt.Substring(0, Math.Min(100, inhalt.Length)) : string.Empty;
        if (inhalt.Length > 100)
        {
          kurzform += " ... ";
        }

        kurzform += Environment.NewLine;
      }

      return kurzform;
    }

    /// <summary>
    /// Converts a value,
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }

    #endregion
  }

}
