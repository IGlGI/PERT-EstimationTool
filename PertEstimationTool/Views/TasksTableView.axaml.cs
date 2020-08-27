using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PertEstimationTool.Views
{
    public class TasksTableView : UserControl
    {
        public TasksTableView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
