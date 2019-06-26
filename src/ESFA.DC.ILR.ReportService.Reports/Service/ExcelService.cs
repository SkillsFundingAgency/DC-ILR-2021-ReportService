using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IFileService _fileService;

        public ExcelService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public Workbook NewWorkbook() => new Workbook();

        public Worksheet GetWorksheetFromWorkbook(Workbook workbook, int index) => workbook.Worksheets[index];

        public void WriteRowsToWorksheet<T>(Worksheet worksheet, IEnumerable<T> rows)
        {
            throw new NotImplementedException();
        }

        public async Task WriteRowsAndSaveNewWorkbookAsync<T>(IEnumerable<T> rows, string fileName, string container, CancellationToken cancellationToken)
        {
            using (var workbook = NewWorkbook())
            {
                var worksheet = GetWorksheetFromWorkbook(workbook, 0);

                WriteRowsToWorksheet(worksheet, rows);

                await SaveWorkbookAsync(workbook, fileName, container, cancellationToken);
            }
        }

        public async Task SaveWorkbookAsync(Workbook workbook, string fileName, string container, CancellationToken cancellationToken)
        {
            using (Stream ms = await _fileService.OpenWriteStreamAsync(fileName, container, cancellationToken))
            {
                workbook.Save(ms, SaveFormat.Xlsx);
            }
        }
    }
}
