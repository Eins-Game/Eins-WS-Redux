using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FinalE.UI_Test.ViewModels;

namespace FinalE.UI_Test.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.DataContext = new MainWindowViewModel(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
