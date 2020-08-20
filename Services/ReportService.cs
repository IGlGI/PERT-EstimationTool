using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using ClosedXML.Excel;
using PertEstimationTool.Enums;
using PertEstimationTool.Models;
using PertEstimationTool.Services.Interfaces;
using Unity;

namespace PertEstimationTool.Services
{
    public class ReportService : IReportService
    {
        private IExcelFileService _excelFileService;

        public ReportService(IUnityContainer container)
        {
            _excelFileService = container.Resolve<IExcelFileService>();
        }

        public async Task GenerateReport(ObservableCollection<TaskItem> tasks, SummaryAssessment totalAssessments, string pathWithFileName)
        {
            if (!string.IsNullOrEmpty(pathWithFileName))
            {
                if (tasks == null || !tasks.Any())
                    throw ThrowIfNullOrEmpry("tasks");

                if (totalAssessments == null)
                    throw ThrowIfNullOrEmpry("total assessments");

                var workBook = new XLWorkbook();

                workBook = await PrepareSourceDataTable(workBook, tasks, totalAssessments);
                workBook = await PrepareResultDataTable(workBook, tasks, totalAssessments);

                await _excelFileService.Save(workBook, pathWithFileName);
            }
        }

        private async Task<XLWorkbook> PrepareResultDataTable(XLWorkbook workBook, ObservableCollection<TaskItem> tasks, SummaryAssessment total)
        {
            var resultDataSheet = workBook.Worksheets.Add(Properties.Resources.calculationResult);

            var columnA = resultDataSheet.Column("A");
            columnA.Width = 70;

            var columnB = resultDataSheet.Column("B");
            columnB.Width = 25;

            var columnC = resultDataSheet.Column("C");
            columnC.Width = 30;

            var columnD = resultDataSheet.Column("D");
            columnD.Width = 25;


            var tableHeader = resultDataSheet.Cell("A1");
            tableHeader = await SetStyleToCell(tableHeader, backgroundColor: XLColor.Green, fontSize: 16);
            tableHeader.Value = Properties.Resources.calculationResult;
            resultDataSheet.Range("A1:D1").Merge();

            var taskDescriptionHead = resultDataSheet.Cell("A2");
            taskDescriptionHead = await SetStyleToCell(taskDescriptionHead, backgroundColor: XLColor.Almond);
            taskDescriptionHead.Value = Properties.Resources.taskDescription;

            var expectedTimeHead = resultDataSheet.Cell("B2");
            expectedTimeHead = await SetStyleToCell(expectedTimeHead, backgroundColor: XLColor.Almond);
            expectedTimeHead.Value = Properties.Resources.expectedTime;

            var stDeviationHead = resultDataSheet.Cell("C2");
            stDeviationHead = await SetStyleToCell(stDeviationHead, backgroundColor: XLColor.Almond);
            stDeviationHead.Value = Properties.Resources.stDeviation;

            var varianceHead = resultDataSheet.Cell("D2");
            varianceHead = await SetStyleToCell(varianceHead, backgroundColor: XLColor.Almond);
            varianceHead.Value = Properties.Resources.variance;

            var itemNumber = 3;

            for (int i = 0; i < tasks.Count; i++)
            {
                var descriptionCell = resultDataSheet.Cell($"A{itemNumber}");
                var expectedTimeCell = resultDataSheet.Cell($"B{itemNumber}");
                var stDeviationCell = resultDataSheet.Cell($"C{itemNumber}");
                var varianceCell = resultDataSheet.Cell($"D{itemNumber}");

                descriptionCell = await SetStyleToCell(descriptionCell);
                descriptionCell.Value = tasks[i].Description;

                expectedTimeCell = await SetStyleToCell(expectedTimeCell);
                expectedTimeCell.Value = tasks[i].Assessments.Estimation;

                stDeviationCell = await SetStyleToCell(stDeviationCell);
                stDeviationCell.Value = tasks[i].Assessments.StDeviation;

                varianceCell = await SetStyleToCell(varianceCell);
                varianceCell.Value = tasks[i].Assessments.Variance;

                itemNumber++;
            }

            var endOfTable = itemNumber++;

            var expectedTimeSumCell = resultDataSheet.Cell($"B{endOfTable}");
            expectedTimeSumCell = await SetStyleToCell(expectedTimeSumCell, backgroundColor: XLColor.GoldenYellow, fontSize: 14);
            expectedTimeSumCell.Value = total.SumEstimation;

            var stDeviationSumCell = resultDataSheet.Cell($"C{endOfTable}");
            stDeviationSumCell = await SetStyleToCell(stDeviationSumCell, backgroundColor: XLColor.GoldenYellow, fontSize: 14);
            stDeviationSumCell.Value = total.SumStDeviation;

            var varianceSumCell = resultDataSheet.Cell($"D{endOfTable}");
            varianceSumCell = await SetStyleToCell(varianceSumCell, backgroundColor: XLColor.GoldenYellow, fontSize: 14);
            varianceSumCell.Value = total.SumVariance;

            var probabilityOfCompletionCell = resultDataSheet.Cell($"A{endOfTable++}");
            probabilityOfCompletionCell = await SetStyleToCell(probabilityOfCompletionCell, backgroundColor: XLColor.GoldenYellow, fontSize: 14, isFontBold: true);
            probabilityOfCompletionCell.Value = $"{Properties.Resources.probabilityCompletion}: {total.PercentageOfCompletion} %";

            return workBook;
        }
        private async Task<XLWorkbook> PrepareSourceDataTable(XLWorkbook workBook, ObservableCollection<TaskItem> tasks, SummaryAssessment total)
        {
            var sourceDataSheet = workBook.Worksheets.Add(Properties.Resources.sourceData);

            var columnA = sourceDataSheet.Column("A");
            columnA.Width = 70;

            var columnB = sourceDataSheet.Column("B");
            columnB.Width = 25;

            var columnC = sourceDataSheet.Column("C");
            columnC.Width = 25;

            var columnD = sourceDataSheet.Column("D");
            columnD.Width = 25;


            var tableHeader = sourceDataSheet.Cell("A1");
            tableHeader = await SetStyleToCell(tableHeader, backgroundColor: XLColor.Orange, fontSize: 16);
            tableHeader.Value = Properties.Resources.sourceData;
            sourceDataSheet.Range("A1:D1").Merge();

            var taskDescriptionHead = sourceDataSheet.Cell("A2");
            taskDescriptionHead = await SetStyleToCell(taskDescriptionHead, backgroundColor: XLColor.Almond);
            taskDescriptionHead.Value = Properties.Resources.taskDescription;

            var optimisticHead = sourceDataSheet.Cell("B2");
            optimisticHead = await SetStyleToCell(optimisticHead, backgroundColor: XLColor.Almond);
            optimisticHead.Value = Properties.Resources.optimistic;

            var mostLikelyHead = sourceDataSheet.Cell("C2");
            mostLikelyHead = await SetStyleToCell(mostLikelyHead, backgroundColor: XLColor.Almond);
            mostLikelyHead.Value = Properties.Resources.mostLikely;

            var pessimisticHead = sourceDataSheet.Cell("D2");
            pessimisticHead = await SetStyleToCell(pessimisticHead, backgroundColor: XLColor.Almond);
            pessimisticHead.Value = Properties.Resources.pessimistic;

            var itemNumber = 3;

            for (int i = 0; i < tasks.Count; i++)
            {
                var descriptionCell = sourceDataSheet.Cell($"A{itemNumber}");
                var optimisticCell = sourceDataSheet.Cell($"B{itemNumber}");
                var mostLikelyCell = sourceDataSheet.Cell($"C{itemNumber}");
                var pessimisticCell = sourceDataSheet.Cell($"D{itemNumber}");

                descriptionCell = await SetStyleToCell(descriptionCell);
                descriptionCell.Value = tasks[i].Description;

                optimisticCell = await SetStyleToCell(optimisticCell);
                optimisticCell.Value = tasks[i].Assessments.Optimistic;

                mostLikelyCell = await SetStyleToCell(mostLikelyCell);
                mostLikelyCell.Value = tasks[i].Assessments.MostLikely;

                pessimisticCell = await SetStyleToCell(pessimisticCell);
                pessimisticCell.Value = tasks[i].Assessments.Pessimistic;

                itemNumber++;
            }

            var endOfTable = itemNumber++;

            var desiredTimeCell = sourceDataSheet.Cell($"A{endOfTable}");
            desiredTimeCell = await SetStyleToCell(desiredTimeCell, backgroundColor: XLColor.Almond, fontSize: 14, isFontBold: true);
            await SetStyleToCell(sourceDataSheet.Cell($"B{endOfTable}"), backgroundColor: XLColor.Almond, fontSize: 14, isFontBold: true);
            desiredTimeCell.Value = $"{Properties.Resources.desiredCompletionTime}: {total.DesiredCompletionTime}";
            sourceDataSheet.Range($"A{endOfTable}:B{endOfTable}").Merge();

            var setTimeFormatCell = sourceDataSheet.Cell($"C{endOfTable}");
            setTimeFormatCell = await SetStyleToCell(setTimeFormatCell, backgroundColor: XLColor.Almond, fontSize: 14);
            await SetStyleToCell(sourceDataSheet.Cell($"D{endOfTable}"), backgroundColor: XLColor.Almond, fontSize: 14, isFontBold: true);
            setTimeFormatCell.Value = $"{Properties.Resources.timeUnit}: {total.EstimationTimeFormat}";
            sourceDataSheet.Range($"C{endOfTable}:D{endOfTable}").Merge();

            return workBook;
        }

        private async Task<IXLCell> SetStyleToCell(IXLCell cell, int fontSize = 12, string fontName = "Segoe UI", bool isFontBold = false,
                                                   XLColor backgroundColor = null, XLAlignmentHorizontalValues horizontalAlignment = XLAlignmentHorizontalValues.Center,
                                                   XLColor outsideBorderColor = null, XLBorderStyleValues outsideBorder = XLBorderStyleValues.Dashed)
        {
            cell.Style.Font.FontSize = fontSize;
            cell.Style.Alignment.Horizontal = horizontalAlignment;
            cell.Style.Font.FontName = fontName;
            cell.Style.Font.Bold = isFontBold;
            cell.Style.Border.OutsideBorder = outsideBorder;
            cell.Style.Border.OutsideBorderColor = outsideBorderColor ?? XLColor.LightSlateGray;

            if (backgroundColor != null)
                cell.Style.Fill.BackgroundColor = backgroundColor;

            return cell;
        }

        private Exception ThrowIfNullOrEmpry(string itemName) => new Exception($"The {itemName} cannot be null or empty");

    }
}
