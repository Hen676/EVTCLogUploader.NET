using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FadedVanguardLogUploader.ViewModels;
using ReactiveUI;
using System.Threading;
using System.Threading.Tasks;

namespace FadedVanguardLogUploader.Views
{
    public partial class ListView : ReactiveUserControl<ListViewModel>
    {
        public ListView()
        {
            if (Avalonia.Controls.Design.IsDesignMode)
            {
                return;
            }
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
            Initialized += OnInitialized;
        }

        private void OnInitialized(object? sender, System.EventArgs e)
        {
            if (DataContext != null && DataContext is ListViewModel model)
            {
                // TODO: Redo threading
                Thread thread = new Thread(() => model.Load());
                thread.Start();
                //model.Load();
            }
        }

        private async Task DoShowDialogAsync(InteractionContext<PopupViewModel, bool> interaction)
        {
            Popup dialog = new()
            {
                DataContext = interaction.Input
            };
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
