using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy
{
    public class DevolvedAdultEducationOccupancyReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>> _devolvedAdultEducationOccupancyReportModelBuilder;
        private readonly ICsvService _csvService;

        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.Ilr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm35,
        };

        public DevolvedAdultEducationOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>> devolvedAdultEducationOccupancyReportModelBuilder,
            ICsvService csvService)
            : base(ReportTaskNameConstants.DevolvedAdultEducationOccupancyReport, "Devolved Adult Education Occupancy Report")
        {
            _fileNameService = fileNameService;
            _devolvedAdultEducationOccupancyReportModelBuilder = devolvedAdultEducationOccupancyReportModelBuilder;

            _csvService = csvService;
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, FileName, OutputTypes.Csv);

            var model = _devolvedAdultEducationOccupancyReportModelBuilder.Build(reportServiceContext, reportsDependentData);

            await _csvService.WriteAsync<DevolvedAdultEducationOccupancyReportModel, DevolvedAdultEducationOccupancyReportClassMap>(model, fileName, reportServiceContext.Container, cancellationToken);

            return new[] { fileName };
        }
    }
}
