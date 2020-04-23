using System;
using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReport : AbstractCsvReport<NonContractedAppsActivityReportModel, NonContractedAppsActivityReportClassMap>, IReport
    {
        public NonContractedAppsActivityReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<NonContractedAppsActivityReportModel>> nonContractedAPpsActivityModelBuilder,
            ICsvFileService csvService)
            : base(fileNameService, nonContractedAPpsActivityModelBuilder, csvService, ReportTaskNameConstants.NonContractedAppsActivityReport, ReportNameConstants.NonContractedAppsActivity)
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
