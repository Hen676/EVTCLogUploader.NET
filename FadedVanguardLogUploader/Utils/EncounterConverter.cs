using Avalonia.Data;
using Avalonia.Data.Converters;
using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.Utils.Determiners;
using System;
using System.Globalization;

namespace FadedVanguardLogUploader.Utils
{
    public class EncounterConverter : IValueConverter
    {
        public static readonly EncounterConverter Instance = new();
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Encounter enc && targetType.IsAssignableTo(typeof(string)))
            {
                return EncounterNameDeterminer.Result(enc);
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
