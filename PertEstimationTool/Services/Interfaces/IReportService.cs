using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PertEstimationTool.Models;

namespace PertEstimationTool.Services.Interfaces
{
    public interface IReportService
    {
        Task GenerateReport(ObservableCollection<TaskItem> tasks, SummaryAssessment totalAssessments, string pathWithFileName);
    }
}
