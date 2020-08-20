using Avalonia.Controls;
using PertEstimationTool.Enums;
using PertEstimationTool.Events;
using PertEstimationTool.Models;
using PertEstimationTool.Services;
using PertEstimationTool.Services.Interfaces;
using PertEstimationTool.Views;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Caching;
using Unity;

namespace PertEstimationTool.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        private IRegionManager _regionManager;

        private IUnityContainer _container;

        public ShellViewModel(Window window, IEventAggregator eventAggregator, IRegionManager regionManager, IUnityContainer container)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _container = container;

            var tasksCollection = new ObservableCollection<TaskItem>();
            var cachePolicy = new CacheItemPolicy();

            RegisterEvents();
            RegisterSupportedLanguages();
            RegisterSupportedEstimationTimes();

            _container.RegisterInstance<CacheItemPolicy>(cachePolicy, InstanceLifetime.Singleton);
            _container.RegisterInstance<MemoryCache>(MemoryCache.Default, InstanceLifetime.Singleton);
            _container.RegisterInstance(typeof(ObservableCollection<TaskItem>), nameof(ObservableCollection<TaskItem>), tasksCollection, InstanceLifetime.Singleton);

            var memoryCache = container.Resolve<MemoryCache>();
            memoryCache.Set(nameof(ObservableCollection<TaskItem>), tasksCollection, cachePolicy);

            _container.RegisterType<INotificationService, NotificationService>();

            container.RegisterInstance<List<FileDialogFilter>>("SupportedExtensions", RegisterSupportExtensions());
            _container.RegisterInstance<IExcelFileService>(new ExcelFileService(), InstanceLifetime.Singleton);
            _container.RegisterInstance<ICalculateService>(new CalculateService(), InstanceLifetime.Singleton);
            _container.RegisterInstance<IReportService>(new ReportService(_container), InstanceLifetime.Singleton);
            _container.RegisterInstance<ICacheService>(new CacheService(_container), InstanceLifetime.Singleton);
            _container.RegisterInstance<IControlService>(new ControlService(_container), InstanceLifetime.Singleton);
            _container.RegisterInstance<ITaskService>(new TaskService(_container), InstanceLifetime.Singleton);

            _container.RegisterInstance<Window>("ShellView", window, InstanceLifetime.Singleton);

            _regionManager.RegisterViewWithRegion("ControlPanelRegion", typeof(ControlPanelView));
            _regionManager.RegisterViewWithRegion("TasksTableRegion", typeof(TasksTableView));
        }

        private List<FileDialogFilter> RegisterSupportExtensions()
        {
            var suppotedFileExtensions = new List<FileDialogFilter>();

            var excelExtension = new FileDialogFilter
            {
                Name = "Microsoft Excel 2007-2020",
                Extensions = new List<string>
                    {
                        ReportExtensions.xlsx
                    }
            };

            suppotedFileExtensions.Add(excelExtension);
            return suppotedFileExtensions;
        }

        private void RegisterSupportedLanguages()
        {
            var languages = new List<SupportedLanguage>();
            languages.Add(SupportedLanguage.En);
            languages.Add(SupportedLanguage.Ru);

            _container.RegisterInstance<List<SupportedLanguage>>("SupportedLanguages", languages, InstanceLifetime.Singleton);
        }

        private void RegisterSupportedEstimationTimes()
        {
            var timeFormats = new EstimationTimeFormats();
            _container.RegisterInstance<EstimationTimeFormats>(timeFormats, InstanceLifetime.Singleton);
            _container.RegisterInstance<List<string>>(nameof(EstimationTimeFormats), timeFormats.SupportedTimeFormats, InstanceLifetime.Singleton);
        }

        private void RegisterEvents()
        {
            _container.RegisterInstance<IEventAggregator>(_eventAggregator, InstanceLifetime.Singleton);
            _container.RegisterType<TasksCollectionMessage>();
            _container.RegisterType<NotificationMessage>();
        }
    }
}
