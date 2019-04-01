using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Model.Eas;
using EasSubmissionValues = ESFA.DC.ILR1819.ReportService.Model.Eas.EasSubmissionValues;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IEasProviderService
    {
        Task<DateTime> GetLastEasUpdate(int ukprn, CancellationToken cancellationToken);

        List<EasPaymentType> GetAllPaymentTypes();

        Task<List<EasSubmissionValues>> GetEasSubmissionValuesAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
