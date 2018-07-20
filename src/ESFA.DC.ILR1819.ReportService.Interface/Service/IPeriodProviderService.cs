using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IPeriodProviderService
    {
        int GetPeriod(IJobContextMessage jobContextMessage);
    }
}
