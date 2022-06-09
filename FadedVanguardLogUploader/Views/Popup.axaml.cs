using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FadedVanguardLogUploader.Views
{
    public partial class Popup : Window
    {
        public Popup()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
