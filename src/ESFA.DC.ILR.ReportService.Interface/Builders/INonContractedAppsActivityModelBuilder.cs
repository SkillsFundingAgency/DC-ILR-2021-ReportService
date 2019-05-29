using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Model.FCS;
using ESFA.DC.ILR.ReportService.Model.Lars;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR.ReportService.Interface.Builders
{
   public interface INonContractedAppsActivityModelBuilder
   {
       List<NonContractedAppsActivityModel> BuildModel(
           NonContractedAppsActivityILRInfo nonContractedAppsActivityIlrInfo,
           NonContractedActivityRuleBaseInfo fm36Info,
           List<ContractAllocationInfo> fcsContractAllocationInfo,
           Dictionary<string, LarsLearningDelivery> larsLearningDeliveries,
           int returnPeriod);
   }
}