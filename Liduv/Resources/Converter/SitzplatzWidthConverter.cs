﻿namespace Liduv.Resources.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  /// <summary>
  /// Dieser Konverter vergrößert die ActualWidth des Containers
  /// für den Sitzplatz.
  /// </summary>
  [ValueConversion(typeof(double), typeof(double))]
  public class SitzplatzWidthConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var width = (double)value;
      return width;
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((double)value);
    }

    #endregion
  }

}
