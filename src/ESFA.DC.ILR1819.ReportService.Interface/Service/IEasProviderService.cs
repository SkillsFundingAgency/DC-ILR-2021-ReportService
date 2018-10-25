using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IEasProviderService
    {
        Task<DateTime> GetLastEasUpdate(int ukprn, CancellationToken cancellationToken);
    }
}
