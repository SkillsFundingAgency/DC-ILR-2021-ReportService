namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR1819.ReportService.Model.ILR;
    using ESFA.DC.JobContextManager.Model.Interface;

    public interface IIlrProviderService
    {
        Task<IMessage> GetIlrFile(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);

        Task<ILRSourceFileInfo> GetLastSubmittedIlrFile(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}
