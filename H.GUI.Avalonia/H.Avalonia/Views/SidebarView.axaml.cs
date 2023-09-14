using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace H.Avalonia.Views
{
    public partial class SidebarView : UserControl
    {
        private Button? _navigationButton;
        public SidebarView()
        {
            InitializeComponent();
            HighlightButton(this.AboutButton);
            _navigationButton = this.AboutButton;
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (_navigationButton != null)
            {
                _navigationButton.Background = Brushes.Transparent;
            }
            _navigationButton = button;
            HighlightButton(button);
        }

        private void HighlightButton(Button button)
        {
            button.Background = new SolidColorBrush()
            {
                Color = Color.FromRgb(175, 225, 175),
                Opacity = 0.25,
            };
        }

    }
}