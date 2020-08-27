
using System.Threading.Tasks;

namespace PertEstimationTool.Services.Interfaces
{
    public interface ICalculateService
    {
        Task<double> CalculateVariance(double sumStDeviations);

        Task<double> CalculateEstimation(double optimistic, double mostLikely, double pessimistic);

        Task<double> CalculateStDeviation(double optimistic, double pessimistic);

        Task<double> CalculatePercentageOfCompletion(double sumEstimations, double sumVariances, double desiredCompletionTime, double zScorePow = 0.5);
    }
}
