using PertEstimationTool.Events;
using PertEstimationTool.Models;
using PertEstimationTool.Services.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace PertEstimationTool.Services
{
    public class TaskService : ITaskService
    {
        private IUnityContainer _container;

        private ICalculateService _calculateService;

        private IControlService _controlService;

        private ICacheService _cacheService;

        private IEventAggregator _eventAggregator;

        public TaskService(IUnityContainer container)
        {
            _container = container;
            _calculateService = container.Resolve<ICalculateService>();
            _controlService = container.Resolve<IControlService>();
            _cacheService = container.Resolve<ICacheService>();
            _eventAggregator = container.Resolve<IEventAggregator>();

        }

        public async Task<SummaryAssessment> CalculateTasks(double desiredCompletionTime, string estimationTimeFormat)
        {
            var tasksItems = await GetTasksList();
            var totalAssessments = new SummaryAssessment();

            totalAssessments.DesiredCompletionTime = desiredCompletionTime;
            totalAssessments.EstimationTimeFormat = estimationTimeFormat;

            foreach (var taskItem in tasksItems)
            {
                taskItem.Assessments.Estimation = 0;
                taskItem.Assessments.StDeviation = 0;
                taskItem.Assessments.Variance = 0;
            }

            foreach (var taskItem in tasksItems)
            {
                var optimistic = taskItem.Assessments.Optimistic;
                var mostLikely = taskItem.Assessments.MostLikely;
                var pessimistic = taskItem.Assessments.Pessimistic;
                var deviation = _calculateService.CalculateStDeviation(taskItem.Assessments.Estimation, optimistic, mostLikely, pessimistic);

                taskItem.Assessments.Estimation = _calculateService.CalculateEstimation(optimistic, mostLikely, pessimistic);
                taskItem.Assessments.StDeviation += deviation;
                taskItem.Assessments.Variance += _calculateService.CalculateVariance(deviation);

                totalAssessments.SumEstimation += taskItem.Assessments.Estimation;
                totalAssessments.SumStDeviation += taskItem.Assessments.StDeviation;
                totalAssessments.SumVariance += taskItem.Assessments.Variance;
            }

            totalAssessments.PercentageOfCompletion = _calculateService.CalculatePercentageOfCompletion(totalAssessments.SumEstimation, totalAssessments.SumVariance, desiredCompletionTime);
            await _cacheService.Upadate<SummaryAssessment>(nameof(SummaryAssessment), totalAssessments);

            return totalAssessments;
        }

        public async Task CreateTask(double optimistic, double mostLikely, double pessimistic, string decription)
        {
            if (string.IsNullOrEmpty(decription))
                throw new Exception("The task's description cannot be null.");

            var task = new TaskItem
            {
                Id = await _controlService.GenerateId(),
                Description = decription,
                Assessments = new Assessment
                {
                    Optimistic = optimistic,
                    MostLikely = mostLikely,
                    Pessimistic = pessimistic
                }
            };

            await AddTask(task);
        }

        public async Task DeleteTask(Guid? id)
        {
            if (id == null)
                throw new Exception("Id cannot be null");

            var tasksItems = await GetTasksList();
            var toDelete = tasksItems?.Where(x => x.Id == id)?.First();

            if (toDelete != null)
            {
                tasksItems.Remove(toDelete);
                await _cacheService.Upadate<ObservableCollection<TaskItem>>(nameof(ObservableCollection<TaskItem>), tasksItems);
                _eventAggregator.GetEvent<TasksCollectionMessage>().Publish(tasksItems);
            }
            else
            {
                throw new KeyNotFoundException($"Cannot to find the item with id:{id}");
            }
        }

        public async Task ResetTasks()
        {
            var tasksItems = await GetTasksList();

            if (tasksItems.Any())
            {
                tasksItems = await _controlService.Reset<ObservableCollection<TaskItem>>(nameof(ObservableCollection<TaskItem>));
                _eventAggregator.GetEvent<TasksCollectionMessage>().Publish(tasksItems);
            }
        }

        public async Task<ObservableCollection<TaskItem>> GetTasksList() => await _controlService.Get<ObservableCollection<TaskItem>>(nameof(ObservableCollection<TaskItem>));

        public async Task<SummaryAssessment> GetResultTasksList() => await _controlService.Get<SummaryAssessment>(nameof(SummaryAssessment));

        private async Task AddTask(TaskItem task)
        {
            if (task == null)
                throw new Exception("Task cannot be null");

            var tasksItems = await GetTasksList();

            tasksItems.Add(task);
            await _cacheService.Upadate<ObservableCollection<TaskItem>>(nameof(ObservableCollection<TaskItem>), tasksItems);
            _eventAggregator.GetEvent<TasksCollectionMessage>().Publish(tasksItems);
        }
    }
}
