using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;

namespace EVTCLogUploader.ViewModels
{
    public class PopupViewModel : ViewModelBase
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public ReactiveCommand<Window, Unit> CloseCommand { get; private set; }
        public PopupViewModel()
        {
            Title = "Error: Popup failed to get message";
            Body = string.Empty;
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
