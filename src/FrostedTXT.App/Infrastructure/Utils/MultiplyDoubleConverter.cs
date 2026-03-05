using System.Globalization;
using System.Windows.Data;

namespace FrostedTXT.App.Infrastructure.Utils;

public sealed class MultiplyDoubleConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return 14d;
        }

        var a = values[0] is double d1 ? d1 : 14d;
        var b = values[1] is double d2 ? d2 : 1d;
        return a * b;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return new object[] { value, 1d };
    }
}
