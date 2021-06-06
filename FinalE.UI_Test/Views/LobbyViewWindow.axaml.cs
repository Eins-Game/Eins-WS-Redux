using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FinalE.UI_Test.ViewModels;
using System.ComponentModel;

namespace FinalE.UI_Test.Views
{
    public partial class LobbyViewWindow : Window
    {
        public LobbyViewWindow()
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
        bool calledBefore = false;
        protected override async void OnClosing(CancelEventArgs e)
        {
            if (DataContext == null) return;
            var dc = (LobbyViewWindowViewModel)DataContext;
            if (!calledBefore)
                await dc.Leave();
            calledBefore = true;
            base.OnClosing(e);
        }
    }
}
