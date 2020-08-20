using Avalonia.Controls;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Models;
using PertEstimationTool.Enums;
using PertEstimationTool.Services.Interfaces;
using System.Threading.Tasks;

namespace PertEstimationTool.Services
{
    public class NotificationService : INotificationService
    {
        public async Task<bool> ShowNotification(string message, string title = null, string header = null, Window parentWindow = null,
                                                 MessageBoxType messageBoxType = MessageBoxType.Ok, bool showInCenter = true,
                                                 WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner,
                                                 Style style = Style.Windows, bool canResize = false)
        {
            var msgBox = new MessageBoxCustomParams
            {
                Style = style,
                WindowStartupLocation = windowStartupLocation,
                ShowInCenter = showInCenter,
                ContentTitle = title ?? Properties.Resources.notification,
                ContentHeader = header,
                CanResize = canResize,
                ContentMessage = message,
            };

            switch (messageBoxType)
            {
                case MessageBoxType.Generate_Cancel:
                    msgBox.ButtonDefinitions = new[] { new ButtonDefinition { Name = Properties.Resources.generateReport }, new ButtonDefinition { Name = Properties.Resources.cancel, Type = ButtonType.Colored } };
                    break;
                case MessageBoxType.Yes_No:
                    msgBox.ButtonDefinitions = new[] { new ButtonDefinition { Name = Properties.Resources.yes }, new ButtonDefinition { Name = Properties.Resources.no, Type = ButtonType.Colored } };
                    break;

                default:
                    msgBox.ButtonDefinitions = new[] { new ButtonDefinition { Name = Properties.Resources.ok } };
                    break;
            }

            var result = await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(msgBox).ShowDialog(parentWindow);

            if (result == Properties.Resources.yes || result == Properties.Resources.generateReport)
                return true;

            return false;
        }
    }
}
