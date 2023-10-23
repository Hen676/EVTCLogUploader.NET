using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Styling;
using EVTCLogUploader.Utils.Determiners;
using System;
using System.Globalization;

namespace EVTCLogUploader.Utils.Converter
{
    public class ThemeConverter : IValueConverter
    {
        public static readonly ThemeConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ThemeVariant theme && targetType.IsAssignableTo(typeof(bool?)))
            {
                return theme == ThemeVariant.Dark;
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool toggle && targetType.IsAssignableTo(typeof(ThemeVariant)))
            {
                return ThemeVarientDeterminer.Result(toggle);
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }
    }
}
