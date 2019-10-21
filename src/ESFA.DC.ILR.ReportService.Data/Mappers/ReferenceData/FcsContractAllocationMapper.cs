using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class FcsContractAllocationMapper : IMapper<IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation>, IReadOnlyCollection<FcsContractAllocation>>
    {
        public IReadOnlyCollection<FcsContractAllocation> MapData(IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation> fcsContractAllocations)
        {
            return fcsContractAllocations?.Select(MapFcsContractAllocation).ToList();
        }

        private FcsContractAllocation MapFcsContractAllocation(ReferenceDataService.Model.FCS.FcsContractAllocation fcsContractAllocation)
        {
            return new FcsContractAllocation()
            {
                ContractAllocationNumber = fcsContractAllocation.ContractAllocationNumber,
                FundingStreamPeriodCode = fcsContractAllocation.FundingStreamPeriodCode,
            };
        }
    }
}
