using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData
{
    public interface IFcsContractAllocationMapper
    {
        IReadOnlyCollection<FcsContractAllocation> MapData(IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation> fcsContractAllocations);
    }
}
