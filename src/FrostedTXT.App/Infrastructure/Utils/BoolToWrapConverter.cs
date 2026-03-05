using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FrostedTXT.App.Infrastructure.Utils;

public sealed class BoolToWrapConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? TextWrapping.Wrap : TextWrapping.NoWrap;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TextWrapping wrapping && wrapping == TextWrapping.Wrap;
    }
}
