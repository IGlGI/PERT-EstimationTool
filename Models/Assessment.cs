
namespace PertEstimationTool.Models
{
    public class Assessment
    {
        public double Optimistic { get; set; }

        public double MostLikely { get; set; }

        public double Pessimistic { get; set; }

        public double Estimation { get; set; }

        public double StDeviation { get; set; }

        public double Variance { get; set; }
    }
}
