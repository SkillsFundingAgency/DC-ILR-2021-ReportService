using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IJobContextMessageKeysMutator
    {
        Task<IDictionary<string, object>> Mutate(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken);
    }
}
