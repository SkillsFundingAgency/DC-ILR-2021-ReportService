using System;
using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;

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
            ICsvFileService csvService)
             : base(fileNameService,
                 modelBuilder,
                 csvService,
                 ReportTaskNameConstants.SummaryOfFm35FundingReport,
                 ReportNameConstants.SummaryOfFM35Funding)
        {
        }
    }
}
