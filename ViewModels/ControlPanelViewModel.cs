using Avalonia.Controls;
using Avalonia.Threading;
using PertEstimationTool.Enums;
using PertEstimationTool.Events;
using PertEstimationTool.Models;
using PertEstimationTool.Services.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Unity;

namespace PertEstimationTool.ViewModels
{
    public class ControlPanelViewModel : BindableBase
    {
        #region Properties

        private double _desiredCompletionTime = 1;

        public double DesiredCompletionTime
        {
            get => _desiredCompletionTime;
            set
            {
                SetProperty(ref _desiredCompletionTime, value);
                TryEnableAddCommand();
            }
        }

        private double _optimistic = 1;

        public double Optimistic
        {
            get => _optimistic;
            set
            {
                SetProperty(ref _optimistic, value);
                TryEnableAddCommand();
            }
        }

        private double _mostLikely = 1;

        public double MostLikely
        {
            get => _mostLikely;
            set
            {
                SetProperty(ref _mostLikely, value);
                TryEnableAddCommand();
            }
        }

        private double _pessimistic = 1;

        public double Pessimistic
        {
            get => _pessimistic;
            set
            {
                SetProperty(ref _pessimistic, value);
                TryEnableAddCommand();
            }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                TryEnableAddCommand();
            }
        }

        private List<string> _estimationTimeFormats;

        public List<string> EstimationTimeFormats
        {
            get => _estimationTimeFormats;
            set
            {
                SetProperty(ref _estimationTimeFormats, value);
            }
        }

        private string _setTimeFormat;

        public string SetTimeFormat
        {
            get => _setTimeFormat;
            set
            {
                SetProperty(ref _setTimeFormat, value);
            }
        }

        private List<SupportedLanguage> _languages;

        public List<SupportedLanguage> Languages
        {
            get => _languages;
            set
            {
                SetProperty(ref _languages, value);
            }
        }

        private int _setLanguage;

        public int SetLanguage
        {
            get => _setLanguage;
            set
            {
                SetProperty(ref _setLanguage, value);
                ChangeLanguage(value);
            }
        }

        private bool _isEnableAddCommand;

        public bool IsEnableAddCommand
        {
            get => _isEnableAddCommand;
            set
            {
                SetProperty(ref _isEnableAddCommand, value);
            }
        }

        private bool _isEnableCalculateCommand;

        public bool IsEnableCalculateCommand
        {
            get => _isEnableCalculateCommand;
            set
            {
                SetProperty(ref _isEnableCalculateCommand, value);
            }
        }

        private bool _isEnableResetCommand;

        public bool IsEnableResetCommand
        {
            get => _isEnableResetCommand;
            set
            {
                SetProperty(ref _isEnableResetCommand, value);
            }
        }

        #endregion

        #region Commands
        public ICommand CalculateCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand AddCommand { get; private set; }

        #endregion

        private IEventAggregator _eventAggregator;

        private Window _shellWindow;

        private ITaskService _taskService;

        private IReportService _reportService;

        private List<FileDialogFilter> _reportSupportedExtensions;

        private INotificationService _notificationService;

        private ObservableCollection<TaskItem> _tasksItems;

        private bool _firstLoadSetLanguage;

        public ControlPanelViewModel(IEventAggregator eventAggregator, IUnityContainer container)
        {
            _shellWindow = container.Resolve<Window>("ShellView");
            _taskService = container.Resolve<ITaskService>();
            _reportService = container.Resolve<IReportService>();
            _reportSupportedExtensions = container.Resolve<List<FileDialogFilter>>("SupportedExtensions");
            _notificationService = container.Resolve<INotificationService>();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TasksCollectionMessage>().Subscribe(CollectionChanged);
            _languages = container.Resolve<List<SupportedLanguage>>("SupportedLanguages");

            _estimationTimeFormats = container.Resolve<List<string>>(nameof(EstimationTimeFormats));
            _setTimeFormat = _estimationTimeFormats.First();
            RaisePropertyChanged(nameof(SetTimeFormat));

            AddCommand = new DelegateCommand<Guid?>(async x => await _taskService.CreateTask(_optimistic, _mostLikely, _pessimistic, _description));
            CalculateCommand = new DelegateCommand(CalculateResult);
            ResetCommand = new DelegateCommand<Guid?>(async x => await _taskService.ResetTasks());
        }

        private async void CalculateResult()
        {
            var calculatedResult = await _taskService.CalculateTasks(_desiredCompletionTime, _setTimeFormat);
            var textResult = $@"{Properties.Resources.expectedTime}: {calculatedResult.SumEstimation} {Environment.NewLine}" +
                             $@"{Properties.Resources.variance}: {calculatedResult.SumVariance} {Environment.NewLine}" +
                             $@"{Properties.Resources.stDeviation}: {calculatedResult.SumStDeviation} {Environment.NewLine}" +
                             $@"{Properties.Resources.probabilityCompletion}: {calculatedResult.PercentageOfCompletion} %";

            var userResponse = await _notificationService.ShowNotification(textResult, header: Properties.Resources.result, parentWindow: _shellWindow, windowStartupLocation: WindowStartupLocation.CenterScreen, title: Properties.Resources.calculationResult, messageBoxType: MessageBoxType.Generate_Cancel);

            if (userResponse)
            {
                GenerateReport();
            }
        }

        private async void GenerateReport()
        {
            var tasks = await _taskService.GetTasksList();
            var assessments = await _taskService.GetResultTasksList();

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filters = _reportSupportedExtensions;

            var pathWithFileName = await saveFileDialog.ShowAsync(_shellWindow);

            await _reportService.GenerateReport(tasks, assessments, pathWithFileName);
        }

        private async void ChangeLanguage(int value)
        {
            switch (value)
            {
                case (int)SupportedLanguage.Ru:
                    Properties.Settings.Default.Language = (int)SupportedLanguage.Ru;
                    break;
                case (int)SupportedLanguage.En:
                    Properties.Settings.Default.Language = (int)SupportedLanguage.En;
                    break;
                default:
                    break;
            }

            _setLanguage = Properties.Settings.Default.Language;
            RaisePropertyChanged(nameof(SetLanguage));

            if (_firstLoadSetLanguage)
            {
                await _notificationService.ShowNotification(Properties.Resources.saveChangesNotify, header: Properties.Resources.warning, parentWindow: _shellWindow, windowStartupLocation: WindowStartupLocation.CenterScreen);
                Properties.Settings.Default.Save();
            }
            else
            {
                _firstLoadSetLanguage = true;
            }
        }

        private void CollectionChanged(ObservableCollection<TaskItem> tasksItems)
        {
            _tasksItems = tasksItems;
            ChangeBoolProperty(ref _isEnableResetCommand, _tasksItems.Any(), nameof(IsEnableResetCommand));
            ChangeBoolProperty(ref _isEnableCalculateCommand, _tasksItems.Any(), nameof(IsEnableCalculateCommand));
        }

        private void TryEnableAddCommand()
        {
            var commandRule = !string.IsNullOrEmpty(_description) && !string.IsNullOrWhiteSpace(_description) && _optimistic != 0 && _mostLikely != 0 && _pessimistic != 0;
            ChangeBoolProperty(ref _isEnableAddCommand, commandRule, nameof(IsEnableAddCommand));
        }

        private void ChangeBoolProperty(ref bool prop, bool value, string name)
        {
            SetProperty(ref prop, value);
            RaisePropertyChanged(name);
        }
    }
}
