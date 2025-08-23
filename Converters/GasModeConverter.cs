using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MasterSlaves.Avalonia.Converters;

public class GasModeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int mode && parameter is int selectedMode)
        {
            return mode == selectedMode ? "#2196F3" : "#9E9E9E"; // Blue if selected, Gray if not
        }
        return "#9E9E9E";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 