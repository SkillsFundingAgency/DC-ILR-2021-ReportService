using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Model.FCS;
using ESFA.DC.ILR.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR.ReportService.Interface.Builders
{
   public interface INonContractedAppsActivityModelBuilder
    {
        List<NonContractedAppsActivityModel> BuildModel(List<ContractAllocationInfo> fcsContractAllocationInfo);
    }
}