using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Service
{
    public class ExcelService : IExcelService
    {
        private const string LicenseResource = "ESFA.DC.ILR.ReportService.Reports.Resources.Aspose.Cells.lic";

        private readonly IFileService _fileService;

        public ExcelService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public Workbook NewWorkbook() => new Workbook();

        public Worksheet GetWorksheetFromWorkbook(Workbook workbook, string sheetName) => workbook.Worksheets[sheetName] ?? workbook.Worksheets.Add(sheetName);

        public Worksheet GetWorksheetFromWorkbook(Workbook workbook, int index) => workbook.Worksheets[index];

        public async Task SaveWorkbookAsync(Workbook workbook, string fileName, string container, CancellationToken cancellationToken)
        {
            using (Stream ms = await _fileService.OpenWriteStreamAsync(fileName, container, cancellationToken))
            {
                workbook.Save(ms, SaveFormat.Xlsx);
            }
        }

        public Workbook BindExcelTemplateToWorkbook<T>(T model, string templateName, string dataSource)
        {
            var designer = new WorkbookDesigner
            {
                Workbook = GetWorkbookFromTemplate(templateName)
            };
            designer.SetDataSource(dataSource, new List<T> { model });
            designer.Process();

            designer.Workbook.CalculateFormula();

            return designer.Workbook;
        }

        public Workbook GetWorkbookFromTemplate(string templateFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(templateFileName));

            using (Stream manifestResourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                return new Workbook(manifestResourceStream);
            }
        }
        public void ApplyLicense()
        {
            var workbook = new Workbook();

            if (!workbook.IsLicensed)
            {
                var license = new License();

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(LicenseResource))
                {
                    license.SetLicense(stream);
                }
            }
        }
    }
}
