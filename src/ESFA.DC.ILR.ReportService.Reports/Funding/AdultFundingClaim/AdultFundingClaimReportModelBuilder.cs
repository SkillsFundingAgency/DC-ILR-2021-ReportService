using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim
{
    public class AdultFundingClaimReportModelBuilder : IModelBuilder<AdultFundingClaimReportModel>
    {
        public AdultFundingClaimReportModel Build(IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportServiceDependentData)
        {
            return new AdultFundingClaimReportModel();
        }
    }
}
