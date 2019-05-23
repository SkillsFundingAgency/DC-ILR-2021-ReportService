using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Model.FCS;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;
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

        public List<NonContractedAppsActivityModel> BuildModel(
            NonContractedAppsActivityILRInfo nonContractedAppsActivityIlrInfo,
            NonContractedActivityRuleBaseInfo nonContractedActivityRuleBaseInfo,
            List<ContractAllocationInfo> fcsContractAllocationInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}
