using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FadedVanguardLogUploader.Util
{
    public class FileCountConverter : IValueConverter
    {
        public static readonly FileCountConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int sourceInt && targetType.IsAssignableTo(typeof(string)))
            {
                return sourceInt + " EVTC Files";
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
