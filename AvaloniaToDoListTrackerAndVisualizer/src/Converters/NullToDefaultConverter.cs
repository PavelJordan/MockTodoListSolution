using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaToDoListTrackerAndVisualizer.Converters;


/// <summary>
/// If value is null, actually use the value in the parameter.
/// Both ways
/// </summary>
public class NullToDefaultConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return parameter;
        }

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