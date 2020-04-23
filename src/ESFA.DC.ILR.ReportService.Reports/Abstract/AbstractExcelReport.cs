using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Abstract
{
    public abstract class AbstractExcelReport<TModel> : AbstractReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<TModel> _modelBuilder;
        private readonly IExcelFileService _excelFileService;

        protected AbstractExcelReport(IFileNameService fileNameService, IModelBuilder<TModel> modelBuilder, IExcelFileService excelFileService, string templateFileName, string dataSource, string taskName, string fileName)
            : base(taskName, fileName)
        {
            _fileNameService = fileNameService;
            _modelBuilder = modelBuilder;
            _excelFileService = excelFileService;
            TemplateFileName = templateFileName;
            DataSource = dataSource;
        }

        public string TemplateFileName { get; }

        public string DataSource { get; }

        public async Task<IEnumerable<string>> GenerateExcelAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken, int? fisInfoRowToDelete = null)
        {
            var model = _modelBuilder.Build(reportServiceContext, reportsDependentData);

            return new[]
            {
                await WriteWorkbookAsync(
                    reportServiceContext,
                    model,
                    TemplateFileName,
                    DataSource,
                    fisInfoRowToDelete,
                    cancellationToken)
            };
        }

        public virtual void RenderIndicativeMessage(Workbook workbook, int fisInfoRowToDelete)
        {
            workbook.Worksheets[0].Cells.DeleteRow(fisInfoRowToDelete);
        }

        private async Task<string> WriteWorkbookAsync(IReportServiceContext reportServiceContext, TModel model, string templateFileName, string dataSource, int? fisInfoRow, CancellationToken cancellationToken)
        {
            var reportFileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(templateFileName));

            using (Stream manifestResourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                var workbook = _excelFileService.BindExcelTemplateToWorkbook(model, dataSource, manifestResourceStream);

                if (fisInfoRow.HasValue)
                {
                    RenderIndicativeMessage(workbook, fisInfoRow.Value);
                }

                await _excelFileService.SaveWorkbookAsync(workbook, reportFileName, reportServiceContext.Container, cancellationToken);
            }

            return reportFileName;
        }

    }
}
