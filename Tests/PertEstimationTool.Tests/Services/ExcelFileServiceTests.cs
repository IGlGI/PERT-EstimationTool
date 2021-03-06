﻿using AutoFixture;
using ClosedXML.Excel;
using FluentAssertions;
using PertEstimationTool.Extensions;
using PertEstimationTool.Services;
using PertEstimationTool.Services.Interfaces;
using PertEstimationTool.Tests.Helpers;
using System;
using System.IO;
using System.Linq;
using Unity;
using Xunit;

namespace PertEstimationTool.Tests.Services
{
    public class ExcelFileServiceTests : IDisposable
    {
        private TestsHelper _helper;

        private IUnityContainer _container;

        private ExcelFileService _excelFileService;

        private ICalculateService _calculateService;

        private Fixture _fixture;

        private string _testsFilesPath;

        public ExcelFileServiceTests()
        {
            _helper = new TestsHelper();
            _container = _helper.GetContainer();
            _excelFileService = _container.Resolve<ExcelFileService>();
            _testsFilesPath = Path.Combine(_container.Resolve<string>("TestsFilesPath") + $@"\{nameof(ExcelFileServiceTests)}\");
            _fixture = _container.Resolve<Fixture>();
        }

        public void Dispose()
        {
            _helper.ClearDirectory(_testsFilesPath);
        }

        [Fact]
        public async void ExcelFileServiceSaveShouldSaveExcelOutputFile()
        {
            //Arrange
            var fileName = "ExcelFileServiceSaveShouldSaveExcelOutputFileTest.xlsx";
            var testFilePath = Path.Combine(_testsFilesPath, fileName);
            var workBook = new XLWorkbook();
            var dataSheet = workBook.Worksheets.Add("TestList_1");
            var cellA1 = dataSheet.Cell("A1");
            cellA1.Value = "Test data";

            //Act

            if (Directory.Exists(testFilePath))
                _helper.ClearDirectory(testFilePath);

            await _excelFileService.Save(workBook, testFilePath);
            var result = Directory.EnumerateFiles(_testsFilesPath).First();

            //Assert
            result.Should().NotBeNullOrEmpty();
            Path.GetFileName(result).GetFileName().Should().Be(fileName);
        }
    }
}
