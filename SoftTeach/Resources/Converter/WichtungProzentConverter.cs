namespace SoftTeach.Resources.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  [ValueConversion(typeof(Single), typeof(string))]
  public class WichtungProzentConverter : IValueConverter
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
      if (value is not float)
      {
        throw new InvalidOperationException("The source must be a float");
      }

      var valueToConvert = (float)value;
      var percentage = valueToConvert * 100;
      return "(" + percentage.ToString("N0") + " %)";
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
      if (value is not string)
      {
        throw new InvalidOperationException("The source must be a string");
      }

      var valueToConvert = (string)value;
      valueToConvert = valueToConvert.Replace("%", string.Empty);
      var percentage = int.Parse(valueToConvert) / 100f;
      return percentage;
    }

    #endregion
  }

}
