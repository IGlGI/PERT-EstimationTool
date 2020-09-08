using AutoFixture;
using FluentAssertions;
using PertEstimationTool.Extensions;
using PertEstimationTool.Models;
using PertEstimationTool.Services;
using PertEstimationTool.Services.Interfaces;
using PertEstimationTool.Tests.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Unity;
using Xunit;

namespace PertEstimationTool.Tests.Services
{
    public class ReportServiceTests : IDisposable
    {
        private TestsHelper _helper;

        private IUnityContainer _container;

        private ReportService _reportService;

        private ICalculateService _calculateService;

        private Fixture _fixture;

        private string _testsFilesPath;

        public ReportServiceTests()
        {
            _helper = new TestsHelper();
            _container = _helper.GetContainer();
            _reportService = _container.Resolve<ReportService>();
            _testsFilesPath = Path.Combine(_container.Resolve<string>("TestsFilesPath") + $@"\{nameof(ReportServiceTests)}\");
            _fixture = _container.Resolve<Fixture>();
        }

        public void Dispose()
        {
            _helper.ClearDirectory(_testsFilesPath);
        }

        [Fact]
        public async void ReportServiceGenerateReportShouldSaveGeneratedReportToOutputFile()
        {
            //Arrange
            var fileName = "TestGeneratedReport.xlsx";
            var testFilePath = Path.Combine(_testsFilesPath, fileName);

            if (Directory.Exists(testFilePath))
                _helper.ClearDirectory(testFilePath);

            //Act
            await _reportService.GenerateReport(_fixture.Create<ObservableCollection<TaskItem>>(), _fixture.Create<SummaryAssessment>(), testFilePath);
            var result = Directory.EnumerateFiles(_testsFilesPath).First();

            //Assert
            result.Should().NotBeNullOrEmpty();
            Path.GetFileName(result).GetFileName().Should().Be(fileName);
        }
    }
}
