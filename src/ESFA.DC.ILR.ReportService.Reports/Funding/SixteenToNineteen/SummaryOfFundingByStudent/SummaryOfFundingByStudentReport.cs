using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.SummaryOfFundingByStudent
{
    public class SummaryOfFundingByStudentReport : AbstractSixteenToNineteenReport<SummaryOfFundingByStudentReportModel, SummaryOfFundingByStudentReportClassMap>
    {
        public SummaryOfFundingByStudentReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<SummaryOfFundingByStudentReportModel>> modelBuilder,
            ICsvFileService csvService) 
            : base(
                fileNameService,
                modelBuilder,
                csvService,
                ReportTaskNameConstants.SummaryOfFundingByStudentReport,
                ReportNameConstants.SummaryOfFundingByStudent)
        {
        }
    }
}
