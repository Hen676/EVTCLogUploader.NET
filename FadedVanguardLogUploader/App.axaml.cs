using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using FadedVanguardLogUploader.Settings;
using FadedVanguardLogUploader.ViewModels;
using FadedVanguardLogUploader.Views;
using System;

namespace FadedVanguardLogUploader
{
    public partial class App : Application
    {
        public static AppSettings settings = new();
        public static FluentTheme Fluent = new(new Uri("avares://ControlCatalog/Styles"));
        public override void Initialize()
        {
            Fluent.Mode = settings.ModeToggle ? FluentThemeMode.Dark : FluentThemeMode.Light;
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
