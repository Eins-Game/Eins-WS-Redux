using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FinalE.UI_Test.Views
{
    public partial class GameViewWindow : Window
    {
        public GameViewWindow()
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
