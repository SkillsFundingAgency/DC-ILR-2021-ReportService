using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IIlrFileHelper
    {
        bool CheckIlrFileNameIsValid(IJobContextMessage jobContextMessage);
    }
}