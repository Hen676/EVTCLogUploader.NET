using Avalonia;
using Avalonia.ReactiveUI;
using FadedVanguardLogUploader.ViewModels;
using ReactiveUI;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FadedVanguardLogUploader.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            if (true)
            {
                //Code that throws the exception
            }

            Closing += Closeing;
            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
        }

        private async Task DoShowDialogAsync(InteractionContext<PopupViewModel, bool> interaction)
        {
            Popup dialog = new();
            dialog.DataContext = interaction.Input;
            await dialog.ShowDialog(this);
            interaction.SetOutput(true);
        }

        private void Closeing(object? sender, CancelEventArgs e)
        {
            App.settings.Save();
        }
    }
}
