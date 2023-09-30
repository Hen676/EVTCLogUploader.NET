using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using EVTCLogUploader.Services;
using EVTCLogUploader.ViewModels;
using EVTCLogUploader.Views;
using System;
using System.Globalization;
using System.Threading;

namespace EVTCLogUploader
{
    public partial class App : Application
    {
        public const string Version = "1.1.0";
        public const string ProgramName = "EVTC Log Uploader";
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            UploaderService uploaderService = new UploaderService();
            SettingService settingService = new SettingService();
            LocalDatabaseService localDatabaseService = new LocalDatabaseService();

            RequestedThemeVariant = settingService.ModeToggle ? ThemeVariant.Dark : ThemeVariant.Light;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(settingService.Language);


            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(uploaderService, settingService, localDatabaseService);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
