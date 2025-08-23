using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MasterSlaves.Core.Models;

namespace MasterSlaves.Avalonia.Converters;

public class GasTypeUnitConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is GasType gasType)
        {
            return gasType switch
            {
                GasType.Vacuum => "mmHg",
                _ => "PSI"
            };
        }
        return "PSI";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 