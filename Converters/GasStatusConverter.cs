using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MasterSlaves.Core.Models;

namespace MasterSlaves.Avalonia.Converters;

public class GasStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is GasStatus status)
        {
            return status switch
            {
                GasStatus.Normal => "#4CAF50", // Green
                GasStatus.Low => "#FF9800",    // Orange
                GasStatus.High => "#F44336",   // Red
                _ => "#9E9E9E"                 // Gray
            };
        }
        return "#9E9E9E";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 