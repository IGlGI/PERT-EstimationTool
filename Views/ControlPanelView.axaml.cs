using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PertEstimationTool.Views
{
    public class ControlPanelView : UserControl
    {
        public ControlPanelView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
