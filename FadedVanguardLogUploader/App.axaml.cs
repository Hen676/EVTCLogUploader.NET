using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using FadedVanguardLogUploader.IO;
using FadedVanguardLogUploader.Settings;
using FadedVanguardLogUploader.Views;
using System;
using System.Globalization;
using System.Threading;

namespace FadedVanguardLogUploader
{
    public partial class App : Application
    {
        public static AppSettings Settings = new();
        public static FluentTheme Fluent = new(new Uri("avares://ControlCatalog/Styles"));

        public override void Initialize()
        {
            if (Settings.ApiToggle)
                GW2ApiHttps.Init();

            Fluent.Mode = Settings.ModeToggle ? FluentThemeMode.Dark : FluentThemeMode.Light;

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(Settings.Lang);

            Styles.Insert(0, Fluent);
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
