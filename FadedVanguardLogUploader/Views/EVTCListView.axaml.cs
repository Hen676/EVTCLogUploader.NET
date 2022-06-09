using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FadedVanguardLogUploader.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace FadedVanguardLogUploader.Views
{
    public partial class EVTCListView : ReactiveUserControl<EVTCListViewModel>
    {
        public EVTCListView()
        {
            InitializeComponent();

            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
        }

        private async Task DoShowDialogAsync(InteractionContext<PopupViewModel, bool> interaction)
        {
            var dialog = new Popup();
            dialog.DataContext = interaction.Input;
            if (Application.Current == null)
                return;
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await dialog.ShowDialog(desktop.MainWindow);
                interaction.SetOutput(true);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
