using Avalonia.Input;
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
            Closing += ClosingMainWindow;
            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
            TopRow.PointerPressed += PointerPressedMainWindow;
        }

        private void PointerPressedMainWindow(object? sender, PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }

        private async Task DoShowDialogAsync(InteractionContext<PopupViewModel, bool> interaction)
        {
            Popup dialog = new()
            {
                DataContext = interaction.Input
            };
            await dialog.ShowDialog(this);
            interaction.SetOutput(true);
        }

        private void ClosingMainWindow(object? sender, CancelEventArgs e)
        {
            App.settings.Save();
        }
    }
}
