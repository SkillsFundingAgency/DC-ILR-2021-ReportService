using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR.ReportService.Interface.Builders
{
   public interface INonContractedAppsActivityModelBuilder
    {
        List<NonContractedAppsActivityModel> BuildModel();
    }
}