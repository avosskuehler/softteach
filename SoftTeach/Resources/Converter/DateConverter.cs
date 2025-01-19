using System;
using System.Globalization;
using System.Windows.Data;


namespace SoftTeach.Resources.Converter
{
  public class DateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      DateTime date = (DateTime)value;

      string param = (string)parameter;

      switch (param.ToUpper())
      {
        case "MONTH":
          return date.Month;
        case "YEAR":
          return date.Year;
        case "DAY":
          return date.Day;
        case "NRDAY":
           return date.ToString("dd ddd");
        default:
          return date.ToString();
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
