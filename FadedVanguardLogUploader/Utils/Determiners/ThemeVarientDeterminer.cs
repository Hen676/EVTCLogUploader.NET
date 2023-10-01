using Avalonia.Styling;

namespace EVTCLogUploader.Utils.Determiners
{
    public static class ThemeVarientDeterminer
    {
        public static ThemeVariant Result(bool toggle)
        {
            return toggle ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }
}
