using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MasterSlaves.Core.Models;

namespace MasterSlaves.Avalonia.Converters;

public class GasTypeColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is GasType gasType)
        {
            return gasType switch
            {
                GasType.Oxygen => "#F5F5F5", // Off White
                GasType.NitrousOxide => "#2196F3", // Blue
                GasType.CarbonDioxide => "#9E9E9E", // Grey
                GasType.MedicalAir => "#000000", // Black
                GasType.InstrumentAir => "#000000", // Black
                GasType.Vacuum => "#FFEB3B", // Yellow
                _ => "#000000" // Black
            };
        }
        return "#000000";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 