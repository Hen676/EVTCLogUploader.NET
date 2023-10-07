using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using EVTCLogUploader.ViewModels;
using ReactiveUI;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EVTCLogUploader.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));

            // Events
            Closing += ClosingMainWindow;
            Loaded += MainWindow_Loaded;
            TopRow.PointerPressed += MainWindow_PointerPressed;
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.Load();
        }

        private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e) => BeginMoveDrag(e);

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
            if (!e.Cancel && ViewModel != null)
                ViewModel.Save();
        }
    }
}
