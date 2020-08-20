using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PertEstimationTool.Models;

namespace PertEstimationTool.Services.Interfaces
{
    public interface ITaskService
    {
        Task CreateTask(double optimistic, double mostLikely, double pessimistic, string decription);

        Task DeleteTask(Guid? id);

        Task<SummaryAssessment> CalculateTasks(double desiredCompletionTime, string estimationTimeFormat);

        Task<ObservableCollection<TaskItem>> GetTasksList();

        Task<SummaryAssessment> GetResultTasksList();

        Task ResetTasks();
    }
}
