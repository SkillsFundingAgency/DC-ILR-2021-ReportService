using ESFA.DC.ILR1819.ReportService.Interface.Context;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Model.Eas;

    public interface IEasProviderService
    {
        Task<DateTime> GetLastEasUpdate(int ukprn, CancellationToken cancellationToken);

        List<EasPaymentType> GetAllPaymentTypes();

        Task<List<EasSubmissionValues>> GetEasSubmissionValuesAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
