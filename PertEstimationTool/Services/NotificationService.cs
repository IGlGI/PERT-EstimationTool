using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Models;
using MessageBox.Avalonia.Views;
using PertEstimationTool.Enums;
using PertEstimationTool.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PertEstimationTool.Services
{
    public class NotificationService : INotificationService
    {
        public async Task<bool> ShowNotification(string message, string title = null, string header = null, Window parentWindow = null,
                                                 MessageBoxType messageBoxType = MessageBoxType.Ok, bool showInCenter = true,
                                                 WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner,
                                                 Style style = Style.UbuntuLinux, Icon icon = Icon.Info, WindowIcon windowIcon = null, bool canResize = false)
        {
            var msgBox = new MessageBoxCustomParams
            {
                Icon = icon,
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

            //The feature is temporarily unavailable 
            //if (windowIcon == null)
            //{
            //    var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            //    msgBox.WindowIcon = new WindowIcon(assets.Open(new Uri("avares://PertEstimationTool/Assets/main.ico")));
            //}

            var result = await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(msgBox).ShowDialog(parentWindow);

            if (result == Properties.Resources.yes || result == Properties.Resources.generateReport)
                return true;

            return false;
        }
    }
}
