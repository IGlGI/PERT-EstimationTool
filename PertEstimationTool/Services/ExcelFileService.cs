using Accord.IO;
using ClosedXML.Excel;
using PertEstimationTool.Extensions;
using PertEstimationTool.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PertEstimationTool.Services
{
    public class ExcelFileService : IExcelFileService
    {
        public async Task Save(XLWorkbook data, string fileName, string fileExtension = null)
        {
            if (data == null)
                throw new Exception("The data cannot be null");

            fileName.CheckPath(fileExtension: fileExtension);

            using (data)
            {
                try
                {
                    data.SaveAs(fileName);
                }
                catch
                {
                    //Add logging
                    throw;
                }
            }
        }
    }
}
