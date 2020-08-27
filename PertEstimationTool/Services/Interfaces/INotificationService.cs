using Avalonia.Controls;
using PertEstimationTool.Enums;
using System.Threading.Tasks;
using MessageBox.Avalonia.Enums;

namespace PertEstimationTool.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> ShowNotification(string message, string title = null, string header = null, Window parentWindow = null,
                                    MessageBoxType messageBoxType = MessageBoxType.Ok, bool showInCenter = true,
                                    WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner,
                                    Style style = Style.UbuntuLinux, Icon icon = Icon.Info, WindowIcon windowIcon = null, bool canResize = false);
    }
}
