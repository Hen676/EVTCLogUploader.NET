using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;

namespace EVTCLogUploader.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public ReactiveCommand<Window, Unit> CloseCommand { get; }

        public void Close(Window window) => window.Close();

        public ViewModelBase() 
        {
            CloseCommand = ReactiveCommand.Create<Window>(Close);
        }
    }
}
