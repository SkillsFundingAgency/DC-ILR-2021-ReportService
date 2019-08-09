using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail
{
    public class HighNeedsStudentDetailReport : AbstractSixteenToNineteenReport<HighNeedsStudentDetailReportModel, HighNeedsStudentDetailReportClassMap>, IReport
    {
        public HighNeedsStudentDetailReport(IFileNameService fileNameService, IModelBuilder<IEnumerable<HighNeedsStudentDetailReportModel>> modelBuilder, ICsvService csvService)
            : base(fileNameService, modelBuilder, csvService, ReportTaskNameConstants.HNSReport, "High Needs Students Detail Report")
        {
        }
    }
}
