using System;
using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave
{
    public class AppsIndicativeEarningsReport : AbstractCsvReport<AppsIndicativeEarningsReportModel, AppsIndicativeEarningsReportClassMap>, IReport
    {
        public AppsIndicativeEarningsReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>> appsIndicativeEarningsModelBuilder,
            ICsvFileService csvService)
            : base(fileNameService, appsIndicativeEarningsModelBuilder, csvService, ReportTaskNameConstants.AppsIndicativeEarningsReport, ReportNameConstants.AppsIndicativeEarnings)
        {
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm36,
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
            };
    }
}
