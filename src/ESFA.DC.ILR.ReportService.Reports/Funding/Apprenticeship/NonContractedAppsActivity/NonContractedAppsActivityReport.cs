using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReport : AbstractCsvReport<NonContractedAppsActivityReportModel, NonContractedAppsActivityReportClassMap>, IReport
    {
        public NonContractedAppsActivityReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<NonContractedAppsActivityReportModel>> appsIndicativeEarningsModelBuilder,
            ICsvService csvService)
            : base(fileNameService, appsIndicativeEarningsModelBuilder, csvService, ReportTaskNameConstants.NonContractedAppsActivityReport, "Non-Contracted Apprenticeships Activity Report")
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
