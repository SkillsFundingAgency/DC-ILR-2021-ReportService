using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Model.FCS;
using ESFA.DC.ILR.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR.ReportService.Service.Builders
{
    public class NonContractedAppsActivityModelBuilder : INonContractedAppsActivityModelBuilder
    {
        public List<NonContractedAppsActivityModel> BuildModel(List<ContractAllocationInfo> fcsContractAllocationInfo)
        {
            var nonContractedAppsActivityModels = new List<NonContractedAppsActivityModel>();
            return nonContractedAppsActivityModels;
        }
    }
}
