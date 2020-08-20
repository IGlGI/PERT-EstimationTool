﻿using Accord;
using Accord.Statistics.Distributions.Univariate;
using PertEstimationTool.Services.Interfaces;
using System;

namespace PertEstimationTool.Services
{
    public class CalculateService : ICalculateService
    {
        public double CalculateEstimation(double optimistic, double mostLikely, double pessimistic) => Math.Round((optimistic + (4 * mostLikely) + pessimistic) / 6, 4);

        public double CalculateStDeviation(double estimation, double optimistic, double mostLikely, double pessimistic) => Math.Round((optimistic - pessimistic) / 6, 4);

        public double CalculatePercentageOfCompletion(double sumEstimations, double sumVariances, double desiredCompletionTime, double zScorePow = 0.5)
        {
            double percentage = 0;
            double zScore = (desiredCompletionTime - sumEstimations) / Math.Pow(sumVariances, zScorePow);

            if (!double.IsNaN(zScore))
            {
                var normal = new NormalDistribution();
                percentage = normal.DistributionFunction(zScore) * 100;

                percentage = percentage < 0 ? 0 : percentage;
                percentage = percentage > 100 ? 100 : percentage;
            }
            else
            {
                percentage = 100;
            }

            return Math.Round(percentage, 3);
        }

        public double CalculateVariance(double sumStDeviations) => Math.Round(sumStDeviations * sumStDeviations, 4);
    }
}
