using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IJobContextMessageKeysMutator
    {
        Task<IDictionary<string, object>> MutateAsync(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken);
    }
}
