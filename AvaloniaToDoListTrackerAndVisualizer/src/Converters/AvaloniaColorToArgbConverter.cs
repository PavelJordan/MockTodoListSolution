using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AvaloniaToDoListTrackerAndVisualizer.Converters;

/// <summary>
/// Converts from uint argb value to avalonia color and back.
/// If types are different return null.
/// Used to save the color into model which is decoupled from UI.
/// </summary>
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
