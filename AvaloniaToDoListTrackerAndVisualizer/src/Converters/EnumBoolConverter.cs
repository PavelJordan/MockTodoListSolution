using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AvaloniaToDoListTrackerAndVisualizer.Converters;

public class EnumBoolConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not null && parameter is not null)
        {
            return value.ToString() == parameter.ToString();
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true)
        {
            return parameter;
        }
        return BindingOperations.DoNothing;
    }
}