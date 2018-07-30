using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Model.Report;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Reports
{
    public interface IReport
    {
        ReportType ReportType { get; }

        Task GenerateReport(IJobContextMessage jobContextMessage);
    }
}
