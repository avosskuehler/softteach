namespace SoftTeach.Resources.Converter
{
  using System;
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// Workaround for a bug in the WPF DataGrid (WPF Toolkit version).
  /// </summary>
  /// <remarks>The WPF DataGrid (WPF Toolkit version) throws a FormatException if a view model
  /// includes a SelectedItem property. This value converter works aroun that bug. The value
  /// converter is taken from Nigel Spencer's Blog, which identifies the bug and provides this
  /// solution. See: http://blog.spencen.com/2009/04/30/problems-binding-to-selectedvalue-with-microsoftrsquos-wpf-datagrid.aspx?results=1(</remarks>
  public class IgnoreNewItemPlaceHolderConverter : IValueConverter
  {
    private const string NewItemPlaceholderName = "{NewItemPlaceholder}";

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null && value.ToString() == NewItemPlaceholderName)
      {
        value = DependencyProperty.UnsetValue;
      }
      return value;
    }
  }
}