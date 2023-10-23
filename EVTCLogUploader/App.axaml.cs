using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using EVTCLogUploader.Services;
using EVTCLogUploader.ViewModels;
using EVTCLogUploader.Views;
using Splat;
using System.Globalization;
using System.Threading;

namespace EVTCLogUploader
{
    public partial class App : Application
    {
        public const string Version = "1.1.1";
        public const string ProgramName = "EVTC Log Uploader";
        private ISettingService _settingService;

        public App()
        {
            Name = ProgramName;
            _settingService = Locator.Current.GetService<SettingService>() ?? new SettingService();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            RequestedThemeVariant = _settingService.ModeToggle ? ThemeVariant.Dark : ThemeVariant.Light;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(_settingService.Language);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
