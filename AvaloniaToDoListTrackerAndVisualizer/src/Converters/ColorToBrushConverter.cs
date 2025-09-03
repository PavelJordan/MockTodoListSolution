using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AvaloniaToDoListTrackerAndVisualizer.Converters;

/// <summary>
/// Can convert avalonia color or uint argb color to avalonia solid color brush.
/// Used mostly when you want to draw with that color (backgrounds, text, etc).
/// Only one way (incoming)
/// </summary>
public class ColorToBrushConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return new SolidColorBrush(color);
        }
        
        if (value is uint argb)
        {
            return new SolidColorBrush(Color.FromUInt32(argb));
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}