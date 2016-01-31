namespace SoftTeach.Resources.Converter
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class InverseBooleanToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var valueToConvert = (bool)value;
      return valueToConvert ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var valueToConvertBack = (Visibility)value;
      return valueToConvertBack != Visibility.Visible;
    }
  }
}
