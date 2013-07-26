namespace Liduv.Resources.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  /// <summary>
  /// Dieser Konverter verkleinert die ActualWidth des Containers
  /// für das WrapPanel in der ListBox.
  /// </summary>
  [ValueConversion(typeof(double), typeof(double))]
  public class WrapPanelWidthConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var width = (double)value;
      return width > 10 ? width - 10 : width;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((double)value) + 10;
    }

    #endregion
  }

}
