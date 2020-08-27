using Avalonia;
using Avalonia.Markup.Xaml;
using Unity;
using Prism.Ioc;
using Prism.Unity;
using PertEstimationTool.Views;
using PertEstimationTool.Enums;

namespace PertEstimationTool
{
    public partial class App : PrismApplication
    {
        public override void Initialize()
        {
            SetCultureInfo();
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        protected override IAvaloniaObject CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        private void SetCultureInfo()
        {
            switch (Properties.Settings.Default.Language)
            {
                case (int)SupportedLanguage.Ru:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
                    break;
                case (int)SupportedLanguage.En:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                    break;
                default:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                    break;
            }
        }
    }
}
