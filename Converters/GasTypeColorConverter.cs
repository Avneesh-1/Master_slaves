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
                GasType.Oxygen => "#4CAF50", // Green
                GasType.Vacuum => "#9E9E9E", // Gray
                GasType.CarbonDioxide => "#2196F3", // Blue
                GasType.MedicalAir => "#FFC107", // Yellow
                GasType.NitrousOxide => "#3F51B5", // Dark Blue
                GasType.InstrumentAir => "#F44336", // Red
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