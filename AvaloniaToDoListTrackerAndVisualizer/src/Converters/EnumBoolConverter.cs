using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AvaloniaToDoListTrackerAndVisualizer.Converters;

/// <summary>
/// Can receive enum choice as parameter and then checks, whether the incoming value
/// is that enum with that choice selected. In that case, returns true, otherwise false.
/// If parameter or value is missing, returns null.
/// Used for when you want to enable/disable some options based on enum value,
/// or maybe more. This goes both way (out-coming is when value is true, it converts
/// to the correct enum in parameter, otherwise, do nothing)
/// </summary>
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