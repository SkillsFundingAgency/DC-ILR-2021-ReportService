using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim
{
    public class AdultFundingClaimReportModel
    {
        //Header
        public string ProviderName { get; set; }

        public int Ukprn { get; set; }

        public string IlrFile { get; set; }

        public string Year { get; set; }

        // Body

        public ActualEarnings AEBProgrammeFunding { get; set; }

        public ActualEarnings AEBLearningSupport { get; set; }

        public ActualEarnings AEBProgrammeFunding1924 { get; set; }

        public ActualEarnings AEBLearningSupport1924 { get; set; }

        public ActualEarnings ALBBursaryFunding { get; set; }

        public ActualEarnings ALBAreaCosts { get; set; }

        public ActualEarnings ALBExcessSupport { get; set; }
        // footer

        public string ApplicationVersion { get; set; }

        public string LarsData { get; set; }

        public string PostcodeData { get; set; }

        public string OrganisationData { get; set; }

        public string CampusIdData { get; set; }

        public string LastILRFileUpdate { get; set; }

        public string LastEASFileUpdate { get; set; }

        public string ReportGeneratedAt { get; set; }
        

    }
}
