using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding
{
    public class SummaryOfFM35FundingReport : AbstractCsvReport<SummaryOfFM35FundingReportModel, SummaryOfFM35FundingReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.Fm35,
        };

        public SummaryOfFM35FundingReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<SummaryOfFM35FundingReportModel>> modelBuilder,
            ICsvService csvService)
             : base(fileNameService,
                 modelBuilder,
                 csvService,
                 ReportTaskNameConstants.SummaryOfFm35FundingReport,
                 "Summary of Funding Model 35 Funding Report")
        {
        }
    }
}
