using Avalonia.Data;
using Avalonia.Data.Converters;
using FadedVanguardLogUploader.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FadedVanguardLogUploader.Utils.Converter
{
    public class EnumConverter : IValueConverter
    {
        public static readonly EnumConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : BindingOperations.DoNothing;
        }
    }
}
