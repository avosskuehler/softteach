namespace SoftTeach.Resources.Converter
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
      if (values[0] is double && values[1] is double)
      {
        var sitzplatzAngle = (double)values[0];
        var overallAngle = (double)values[1];

        return sitzplatzAngle + overallAngle;
      }

      return 0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

}
