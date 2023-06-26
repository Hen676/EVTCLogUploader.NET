using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using EVTCLogUploader.Settings;
using EVTCLogUploader.ViewModels;
using EVTCLogUploader.Views;
using System;
using System.Globalization;
using System.Threading;

namespace EVTCLogUploader
{
    public partial class App : Application
    {
        public static AppSettings Settings = new();
        private static FluentTheme Fluent = new(new Uri("avares://ControlCatalog/Styles"));
        public const string Version = "1.1.0";
        public const string ProgramName = "EVTC Log Uploader";

        public override void Initialize()
        {
            Fluent.Mode = Settings.ModeToggle ? FluentThemeMode.Dark : FluentThemeMode.Light;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(Settings.Lang);

            Styles.Insert(0, Fluent);
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var settingsService = new SettingsService();


            var mainViewModel = new MainWindowViewModel(settingsService);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
