using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaToDoListTrackerAndVisualizer.Converters;

public class NullToDefaultConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return parameter;
        }

        return value;
    }
}