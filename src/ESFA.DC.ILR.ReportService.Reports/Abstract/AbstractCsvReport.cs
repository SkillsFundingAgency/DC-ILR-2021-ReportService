using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Abstract
{
    public abstract class AbstractCsvReport<TModel, TClassMap> : AbstractReport
         where TClassMap : ClassMap<TModel>
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<IEnumerable<TModel>> _modelBuilder;
        private readonly ICsvService _csvService;

        protected AbstractCsvReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<TModel>> modelBuilder,
            ICsvService csvService,
            string taskName,
            string fileName) 
            : base(taskName, fileName)
        {
            _fileNameService = fileNameService;
            _modelBuilder = modelBuilder;
            _csvService = csvService;
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, FileName, OutputTypes.Csv);

            var model = _modelBuilder.Build(reportServiceContext, reportsDependentData);

            await _csvService.WriteAsync<TModel, TClassMap>(model, fileName, reportServiceContext.Container, cancellationToken);

            return new[] { fileName };
        }
    }
}
