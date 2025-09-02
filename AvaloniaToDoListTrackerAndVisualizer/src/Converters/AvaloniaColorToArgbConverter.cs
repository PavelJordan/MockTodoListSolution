using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AvaloniaToDoListTrackerAndVisualizer.Converters;

public class AvaloniaColorToArgbConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is uint argb)
        {
            return Color.FromUInt32(argb);
        }
        

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return color.ToUInt32();
        }

        return null;
    }
}
