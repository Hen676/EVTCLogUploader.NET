using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;

namespace FadedVanguardLogUploader.ViewModels
{
    public class PopupViewModel : ViewModelBase
    {
        public string Message { get; set; }
        public ReactiveCommand<Window, Unit> CloseCommand { get; private set; }
        public PopupViewModel()
        {
            Message = "Error: Popup failed to get message";
            CloseCommand = ReactiveCommand.Create<Window>(CloseWindow);
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
