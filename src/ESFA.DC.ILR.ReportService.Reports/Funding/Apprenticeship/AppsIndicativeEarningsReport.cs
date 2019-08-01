using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship
{
    public class AppsIndicativeEarningsReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>> _appsIndicativeEarningsModelBuilder;
        private readonly ICsvService _csvService;

        public AppsIndicativeEarningsReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>> appsIndicativeEarningsModelBuilder,
            ICsvService csvService)
            : base(ReportTaskNameConstants.AppsIndicativeEarningsReport, "Apps Indicative Earnings Report")
        {
            _fileNameService = fileNameService;
            _appsIndicativeEarningsModelBuilder = appsIndicativeEarningsModelBuilder;
            _csvService = csvService;
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm36,
                DependentDataCatalog.Ilr,
                DependentDataCatalog.ReferenceData
            };

        public async Task<IEnumerable<string>> GenerateAsync(
            IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var appsIndicativeEarningsReportModels = _appsIndicativeEarningsModelBuilder.Build(reportServiceContext, reportsDependentData);

            var fileName = _fileNameService.GetFilename(reportServiceContext, FileName, OutputTypes.Excel);

            await _csvService.WriteAsync<AppsIndicativeEarningsReportModel, AppsIndicativeEarningsReportMapper>(appsIndicativeEarningsReportModels, fileName, reportServiceContext.Container, cancellationToken);
            return new[] { fileName };
        }

       
    }
}
