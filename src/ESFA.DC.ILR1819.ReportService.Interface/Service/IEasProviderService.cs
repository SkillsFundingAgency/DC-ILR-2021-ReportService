using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IEasProviderService
    {
        Task<DateTime> GetLastEasUpdate(int ukprn, CancellationToken cancellationToken);

        List<ESFA.DC.EAS1819.EF.PaymentTypes> GetAllPaymentTypes();

        Task<List<EasSubmissionValues>> GetEasSubmissionValuesAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}
