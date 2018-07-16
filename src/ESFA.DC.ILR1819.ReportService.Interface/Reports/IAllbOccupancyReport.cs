using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Reports
{
    public interface IAllbOccupancyReport
    {
        Task GenerateReport(IJobContextMessage jobContextMessage);
    }
}
