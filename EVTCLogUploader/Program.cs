using Avalonia;
using Avalonia.ReactiveUI;
using EVTCLogUploader.Services;
using Splat;
using System;

namespace EVTCLogUploader
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

            Locator.CurrentMutable.RegisterConstant(new LocalDatabaseService(), typeof(ILocalDatabaseService));
            Locator.CurrentMutable.RegisterConstant(new SettingService(), typeof(ISettingService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new UploaderService(), typeof(IUploaderService));
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
