using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail
{
    public class HighNeedsStudentDetailReport : AbstractSixteenToNineteenReport<HighNeedsStudentDetailReportModel, HighNeedsStudentDetailReportClassMap>, IReport
    {
        public HighNeedsStudentDetailReport(IFileNameService fileNameService, IModelBuilder<IEnumerable<HighNeedsStudentDetailReportModel>> modelBuilder, ICsvFileService csvService)
            : base(fileNameService, modelBuilder, csvService, ReportTaskNameConstants.HNSReport, "High Needs Students Detail Report")
        {
        }
    }
}
