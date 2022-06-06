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
  [ValueConversion(typeof(Stunde), typeof(int))]
  public class StundeStundenzahlConverter : IValueConverter
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
      if (value is not Stunde)
      {
        throw new InvalidOperationException("The source must be a Stunde");
      }

      var valueToConvert = (Stunde)value;
      return valueToConvert.LetzteUnterrichtsstunde.Stundenindex - valueToConvert.ErsteUnterrichtsstunde.Stundenindex + 1;
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
