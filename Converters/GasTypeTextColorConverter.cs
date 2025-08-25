using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MasterSlaves.Core.Models;

namespace MasterSlaves.Avalonia.Converters;

public class GasTypeTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is GasType gasType)
        {
            return gasType switch
            {
                GasType.Oxygen => "#000000", // Black text for off-white background
                GasType.InstrumentAir => "#FFFFFF", // White text for black background
                GasType.Vacuum => "#000000", // Black text for yellow background
                _ => "#FFFFFF" // White text for dark backgrounds
            };
        }
        return "#FFFFFF";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
