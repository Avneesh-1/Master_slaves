using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MasterSlaves.Core.Models;

namespace MasterSlaves.Avalonia.Converters;

public class GasTypeDisplayNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is GasType gasType)
        {
            return gasType switch
            {
                GasType.Oxygen => "OXYGEN",
                GasType.Vacuum => "VACUUM",
                GasType.CarbonDioxide => "CARBON DIOXIDE",
                GasType.MedicalAir => "MEDICAL AIR",
                GasType.NitrousOxide => "NITROUS OXIDE",
                GasType.InstrumentAir => "INSTRUMENT AIR",
                _ => gasType.ToString()
            };
        }
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 