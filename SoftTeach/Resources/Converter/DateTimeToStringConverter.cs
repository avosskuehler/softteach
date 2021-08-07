namespace SoftTeach.Resources.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;


  /// <summary>
  /// Konvertiert ein DateTime in einen String abhängig vom Parameter
  /// </summary>
  public class DateTimeToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return "Datum";
      }

      var valueToConvert = (DateTime)value;

      if (parameter == null)
      {
        return valueToConvert.ToShortDateString();
      }

      var converterParam = (DateTimeConvertFormat)Enum.Parse(typeof(DateTimeConvertFormat), parameter.ToString());

      switch (converterParam)
      {
        case DateTimeConvertFormat.DatumTTMM:
          return valueToConvert.ToString("dd.MM");
        case DateTimeConvertFormat.DatumKurz:
          return valueToConvert.ToString("dd.MM.yyyy");
        case DateTimeConvertFormat.DatumLang:
          return valueToConvert.ToString("dd. MMMM yyyy");
        case DateTimeConvertFormat.DatumLangMitWochentag:
          return valueToConvert.ToLongDateString();
        case DateTimeConvertFormat.DatumKurzMitZeit:
          return valueToConvert.ToString("dd.MM.yy, HH:mm");
        case DateTimeConvertFormat.DatumLangMitZeit:
          return valueToConvert.ToString("dd. MMMM yyyy, HH:mm");
        case DateTimeConvertFormat.DatumLangFR:
          var ci = new CultureInfo("fr-FR");
          return valueToConvert.ToString("dd MMMM yyyy", ci.DateTimeFormat);
      }

      return valueToConvert.ToShortDateString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}
