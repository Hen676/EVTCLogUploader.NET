using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
            PointerPressed += PointerPressedPopUp;
        }

        private void PointerPressedPopUp(object? sender, PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
