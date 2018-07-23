using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Reports
{
    public interface IFundingSummaryReport
    {
        Task GenerateReport(IJobContextMessage jobContextMessage);
    }
}
