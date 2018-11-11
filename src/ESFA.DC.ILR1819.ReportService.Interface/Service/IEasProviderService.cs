namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.JobContextManager.Model.Interface;
    using Model.Eas;

    public interface IEasProviderService
    {
        Task<DateTime> GetLastEasUpdate(int ukprn, CancellationToken cancellationToken);

        List<EasPaymentTypes> GetAllPaymentTypes();

        Task<List<EasSubmissionValues>> GetEasSubmissionValuesAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}
