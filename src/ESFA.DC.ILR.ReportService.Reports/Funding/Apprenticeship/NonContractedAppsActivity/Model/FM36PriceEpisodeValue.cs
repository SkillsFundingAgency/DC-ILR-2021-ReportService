using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model
{
    public class FM36PriceEpisodeValue
    {
        public PriceEpisodeValues PriceEpisodeValue { get; set; }

        public FundLineValue FundLineValues { get; set; }
    }
}
