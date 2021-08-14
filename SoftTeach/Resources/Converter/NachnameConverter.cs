namespace SoftTeach.Resources.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  public class NachnameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var s = (string)value;
      return string.Format("{0}.", s.Substring(0, 1));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}