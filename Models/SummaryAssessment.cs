
namespace PertEstimationTool.Models
{
    public class SummaryAssessment
    {
        public double SumEstimation { get; set; }

        public double SumStDeviation { get; set; }

        public double SumVariance { get; set; }

        public double DesiredCompletionTime { get; set; }

        public string EstimationTimeFormat { get; set; }

        public double PercentageOfCompletion { get; set; }
    }
}
