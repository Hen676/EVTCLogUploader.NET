using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;

namespace EVTCLogUploader.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public ReactiveCommand<Window, Unit> CloseCommand { get; }
        public ReactiveCommand<Window, Unit> MinimizeCommand { get; }
        public ReactiveCommand<Window, Unit> MaximizeCommand { get; }

        public ViewModelBase()
        {
            CloseCommand = ReactiveCommand.Create<Window>(Close);
            MinimizeCommand = ReactiveCommand.Create<Window>(Minimize);
            MaximizeCommand = ReactiveCommand.Create<Window>(Maximize);
        }

        private void Maximize(Window window) => window.WindowState = (window.WindowState == WindowState.FullScreen || window.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        private void Minimize(Window window) => window.WindowState = WindowState.Minimized;
        private void Close(Window window) => window.Close();
    }
}
