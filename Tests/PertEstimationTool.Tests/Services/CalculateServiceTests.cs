using Unity;
using Xunit;
using System;
using AutoFixture;
using FluentAssertions;
using PertEstimationTool.Tests.Helpers;
using PertEstimationTool.Services.Interfaces;

namespace PertEstimationTool.Tests.Services
{
    /*The Program(sometimes Project) Evaluation and Review Technique,
      frequently shortened to PERT, is a way of analyzing the tasks required
      to complete a specific project, particularly the time each task requires,
      and to identify the minimum time required for completion of the whole project.PERT 
      is most frequently employed for research projects when the length of any specific
      activity cannot be predicted, and so work is planned around a series of milestones.
      
      PERT Estimation formula - (Optimistic assessment + (4 * MostLikely assessment) + Pessimistic assessment) / 6
      Standard Deviation formula - (Optimistic assessment - Pessimistic assessment) / 6
      Variance formula - Sum Standard Deviation * Sum Standard Deviation

      Copied from here - https://goodcalculators.com/pert-calculator/ */

    public class CalculateServiceTests
    {
        private IUnityContainer _container;

        private ICalculateService _calculateService;

        private Fixture _fixture;

        public CalculateServiceTests()
        {
            _container = new ContainerHelper().GetContainer();
            _calculateService = _container.Resolve<ICalculateService>();
            _fixture = _container.Resolve<Fixture>();
        }

        [Fact]
        public async void CalculateServiceCalculateEstimationShouldCalculateEstimation()
        {
            //Arrange
            var optimistic = _fixture.Create<double>();
            var mostLikely = _fixture.Create<double>();
            var pessimistic = _fixture.Create<double>();

            //Act
            var assertResult = Math.Round((optimistic + (4 * mostLikely) + pessimistic) / 6, 4); //According to its formula (See the description upper)
            var result = await _calculateService.CalculateEstimation(optimistic, mostLikely, pessimistic);

            //Assert
            result.Should().Be(assertResult);
        }

        [Fact]
        public async void CalculateServiceCalculateStandardDeviationShouldCalculateStandardDeviation()
        {
            //Arrange
            var optimistic = _fixture.Create<double>();
            var pessimistic = _fixture.Create<double>();

            //Act
            var assertResult = Math.Round((optimistic - pessimistic) / 6, 4); //According to its formula (See the description upper)
            var result = await _calculateService.CalculateStDeviation(optimistic, pessimistic);

            //Assert
            result.Should().Be(assertResult);
        }

        [Fact]
        public async void CalculateServiceCalculateVarianceShouldCalculateVariance()
        {
            //Arrange
            var optimistic = _fixture.Create<double>();
            var pessimistic = _fixture.Create<double>();

            //Act
            var sumStDeviations = await _calculateService.CalculateStDeviation(optimistic, pessimistic);
            var assertResult = Math.Round(sumStDeviations * sumStDeviations, 4); //According to its formula (See the description upper)
            var result = await _calculateService.CalculateVariance(sumStDeviations);

            //Assert
            result.Should().Be(assertResult);
        }
    }
}
