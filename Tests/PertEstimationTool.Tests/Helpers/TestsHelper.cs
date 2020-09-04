using AutoFixture;
using Avalonia.Controls;
using Moq;
using PertEstimationTool.Enums;
using PertEstimationTool.Events;
using PertEstimationTool.Extensions;
using PertEstimationTool.Models;
using PertEstimationTool.Services;
using PertEstimationTool.Services.Interfaces;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Caching;
using System.Threading;
using Unity;

namespace PertEstimationTool.Tests.Helpers
{
    public class TestsHelper
    {
        public IUnityContainer GetContainer() => RegisterContainer(new UnityContainer());

        public void ClearDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var directory = new DirectoryInfo(path);

                foreach (var file in directory.EnumerateFiles())
                {
                    file.Delete();
                }

                directory.Delete(true);
            }
        }

        private IUnityContainer RegisterContainer(IUnityContainer _container)
        {
            var tasksCollection = new ObservableCollection<TaskItem>();
            var cachePolicy = new CacheItemPolicy();
            var eventAggregator = new Mock<IEventAggregator>().Object;

            var testsFilesPath = Path.Combine(Directory.GetCurrentDirectory() + @"\TestsTempFiles");
            testsFilesPath.CreateDirectory();

            RegisterEvents(ref _container, eventAggregator);
            RegisterSupportedLanguages(ref _container);
            RegisterSupportedEstimationTimes(ref _container);

            _container.RegisterInstance("TestsFilesPath", testsFilesPath);
            _container.RegisterInstance(new Fixture());
            _container.RegisterInstance(new CancellationTokenSource());
            _container.RegisterInstance<IEventAggregator>(eventAggregator, InstanceLifetime.Singleton);
            _container.RegisterInstance<CacheItemPolicy>(cachePolicy, InstanceLifetime.Singleton);
            _container.RegisterInstance<MemoryCache>(MemoryCache.Default, InstanceLifetime.Singleton);
            _container.RegisterInstance(typeof(ObservableCollection<TaskItem>), nameof(ObservableCollection<TaskItem>), tasksCollection, InstanceLifetime.Singleton);

            _container.RegisterType<INotificationService, NotificationService>();

            _container.RegisterInstance<List<FileDialogFilter>>("SupportedExtensions", RegisterSupportExtensions());
            _container.RegisterInstance<IExcelFileService>(new ExcelFileService(), InstanceLifetime.Singleton);
            _container.RegisterInstance<ICalculateService>(new CalculateService(), InstanceLifetime.Singleton);
            _container.RegisterInstance<IReportService>(new ReportService(_container), InstanceLifetime.Singleton);
            _container.RegisterInstance<ICacheService>(new CacheService(_container), InstanceLifetime.Singleton);
            _container.RegisterInstance<IControlService>(new ControlService(_container), InstanceLifetime.Singleton);
            _container.RegisterInstance<ITaskService>(new TaskService(_container), InstanceLifetime.Singleton);

            return _container;
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

        private void RegisterSupportedLanguages(ref IUnityContainer _container)
        {
            var languages = new List<SupportedLanguage>();
            languages.Add(SupportedLanguage.En);
            languages.Add(SupportedLanguage.Ru);

            _container.RegisterInstance<List<SupportedLanguage>>("SupportedLanguages", languages, InstanceLifetime.Singleton);
        }

        private void RegisterSupportedEstimationTimes(ref IUnityContainer _container)
        {
            var timeFormats = new EstimationTimeFormats();
            _container.RegisterInstance<EstimationTimeFormats>(timeFormats, InstanceLifetime.Singleton);
            _container.RegisterInstance<List<string>>(nameof(EstimationTimeFormats), timeFormats.SupportedTimeFormats, InstanceLifetime.Singleton);
        }

        private void RegisterEvents(ref IUnityContainer _container, IEventAggregator _eventAggregator)
        {
            _container.RegisterInstance<IEventAggregator>(_eventAggregator, InstanceLifetime.Singleton);
            _container.RegisterType<TasksCollectionMessage>();
            _container.RegisterType<NotificationMessage>();
        }
    }
}
