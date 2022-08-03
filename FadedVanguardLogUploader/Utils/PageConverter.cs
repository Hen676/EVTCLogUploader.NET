using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FadedVanguardLogUploader.Utils
{
    public class PageConverter : IValueConverter
    {
        public static readonly PageConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int sourceInt && targetType.IsAssignableTo(typeof(string)))
            {
                return (sourceInt + 1).ToString();
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
