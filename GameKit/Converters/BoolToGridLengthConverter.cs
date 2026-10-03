using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GameKit.Converters;

public class BoolToGridLengthConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true)
        {
            var width = parameter is string stringWidth && double.TryParse(stringWidth, out double doubleWidth) ? doubleWidth : 240;
            return new GridLength(width);
        }
        
        return new GridLength(0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("BoolToGridLengthConverter does not support ConvertBack.");
    }
}