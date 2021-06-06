using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FinalE.UI_Test.Views
{
    public partial class JoinLobby : Window
    {
        public JoinLobby()
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
