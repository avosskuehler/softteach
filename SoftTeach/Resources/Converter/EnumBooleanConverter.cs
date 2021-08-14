
namespace SoftTeach.Resources.Converter
{
  using System;
  using System.Windows.Data;

  /// <summary>
  /// Die Klasse EnumBooleanConverter konvertiert Enumerations in Boolsche Werte für Radioboxes
  /// </summary>
  public class EnumBooleanConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value.Equals(true) ? parameter : Binding.DoNothing;
    }

    ///// <summary>
    ///// Converts a value.
    ///// </summary>
    ///// <param name="value">The value produced by the binding source.</param>
    ///// <param name="targetType">The type of the binding target property.</param>
    ///// <param name="parameter">The converter parameter to use.</param>
    ///// <param name="culture">The culture to use in the converter.</param>
    ///// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //{
    //  var parameterString = parameter as string;
    //  if (parameterString == null)
    //  {
    //    return DependencyProperty.UnsetValue;
    //  }

    //  if (Enum.IsDefined(value.GetType(), value) == false)
    //  {
    //    return DependencyProperty.UnsetValue;
    //  }

    //  var parameterValue = Enum.Parse(value.GetType(), parameterString);

    //  return parameterValue.Equals(value);
    //}

    ///// <summary>
    ///// Converts a value.
    ///// </summary>
    ///// <param name="value">The value that is produced by the binding target.</param>
    ///// <param name="targetType">The type to convert to.</param>
    ///// <param name="parameter">The converter parameter to use.</param>
    ///// <param name="culture">The culture to use in the converter.</param>
    ///// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    //public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //{
    //  var parameterString = parameter as string;
    //  if (parameterString == null || value.Equals(false))
    //  {
    //    return DependencyProperty.UnsetValue;
    //  }

    //  return Enum.Parse(targetType, parameterString);
    //}
  }
}
