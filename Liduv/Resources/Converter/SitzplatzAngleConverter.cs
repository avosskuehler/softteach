namespace Liduv.Resources.Converter
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// Dieser Konverter vergrößert die ActualWidth des Containers
  /// für den Sitzplatz.
  /// </summary>
  public class SitzplatzAngleConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var sitzplatzAngle = (double)values[0];
      var overallAngle = (double)values[1];

      return sitzplatzAngle + overallAngle;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

}
