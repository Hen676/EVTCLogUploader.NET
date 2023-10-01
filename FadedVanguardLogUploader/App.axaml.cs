using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using EVTCLogUploader.Services;
using EVTCLogUploader.ViewModels;
using EVTCLogUploader.Views;
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
            UploaderService uploaderService = new();
            SettingService settingService = new();
            LocalDatabaseService localDatabaseService = new();

            RequestedThemeVariant = settingService.ModeToggle ? ThemeVariant.Dark : ThemeVariant.Light;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(settingService.Language);


            MainWindowViewModel mainWindowViewModel = new(uploaderService, settingService, localDatabaseService);

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
