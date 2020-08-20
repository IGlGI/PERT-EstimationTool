
namespace PertEstimationTool.Services.Interfaces
{
    public interface ICalculateService
    {
        double CalculateVariance(double sumStDeviations);

        double CalculateEstimation(double optimistic, double mostLikely, double pessimistic);

        double CalculateStDeviation(double estimation, double optimistic, double mostLikely, double pessimistic);

        double CalculatePercentageOfCompletion(double sumEstimations, double sumVariances, double desiredCompletionTime, double zScorePow = 0.5);
    }
}
