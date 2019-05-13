using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Model.FCS;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public class FCSProviderService : IFCSProviderService
    {
        private readonly Func<IFcsContext> _fcsContextFactory;

        public FCSProviderService(
            ILogger logger,
            Func<IFcsContext> fcsContextFactory)
        {
            _fcsContextFactory = fcsContextFactory;
        }

        public async Task<List<ContractAllocationInfo>> GetContractAllocationsForProviderAsync(int ukPrn, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<ContractAllocationInfo> contractAllocationInfoList;

            using (var fcsContext = _fcsContextFactory())
            {
                contractAllocationInfoList = await fcsContext.ContractAllocations.Where(x => x.DeliveryUkprn == ukPrn)
                    .Select(ca => new ContractAllocationInfo
                    {
                        DeliveryUkprn = ca.DeliveryUkprn.GetValueOrDefault(),
                        ContractId = ca.ContractId.GetValueOrDefault(),
                        FundingStreamPeriodCode = ca.FundingStreamPeriodCode
                    }).ToListAsync(cancellationToken);
            }

            return contractAllocationInfoList;
        }
    }
}
