using PertEstimationTool.Events;
using PertEstimationTool.Models;
using PertEstimationTool.Services.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Unity;

namespace PertEstimationTool.ViewModels
{
    public class TasksTableViewModel : BindableBase
    {
        private ObservableCollection<TaskItem> _tasksItems;
        public ObservableCollection<TaskItem> TasksItems
        {
            get => _tasksItems;
            set
            {
                if (value != null)
                {
                    SetProperty(ref _tasksItems, value);
                    _eventAggregator.GetEvent<TasksCollectionMessage>().Publish(TasksItems);
                }
            }
        }

        private IEventAggregator _eventAggregator;

        private ITaskService _taskService;

        public ICommand DeleteCommand { get; private set; }

        public TasksTableViewModel(IEventAggregator eventAggregator, IUnityContainer container)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TasksCollectionMessage>().Subscribe(CollectionChanged);
            _taskService = container.Resolve<ITaskService>();
            _tasksItems = _taskService.GetTasksList().GetAwaiter().GetResult();
            DeleteCommand = new DelegateCommand<Guid?>(async x => await _taskService.DeleteTask(x));
        }

        private void CollectionChanged(ObservableCollection<TaskItem> tasksItems)
        {
            SetProperty(ref _tasksItems, tasksItems);
            RaisePropertyChanged(nameof(TasksItems));
        }
    }
}
