using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Model.FCS;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IFCSProviderService
    {
        Task<List<ContractAllocationInfo>> GetContractAllocationsForProviderAsync(int ukPrn, CancellationToken cancellationToken);
    }
}
