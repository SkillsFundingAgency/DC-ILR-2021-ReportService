using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.SummaryOfFundingByStudent
{
    public class SummaryOfFundingByStudentReport : AbstractSixteenToNineteenReport<SummaryOfFundingByStudentReportModel, SummaryOfFundingByStudentReportClassMap>
    {
        public SummaryOfFundingByStudentReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<SummaryOfFundingByStudentReportModel>> modelBuilder,
            ICsvService csvService) 
            : base(
                fileNameService,
                modelBuilder,
                csvService,
                ReportTaskNameConstants.SummaryOfFundingByStudentReport,
                "16-19 Summary of Funding by Student Report")
        {
        }
    }
}
