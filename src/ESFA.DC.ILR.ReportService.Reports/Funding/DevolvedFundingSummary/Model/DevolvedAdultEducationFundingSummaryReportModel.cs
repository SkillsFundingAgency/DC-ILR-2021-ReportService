using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model
{
    public class DevolvedAdultEducationFundingSummaryReportModel : IDevolvedAdultEducationFundingSummaryReport
    {
        public DevolvedAdultEducationFundingSummaryReportModel(string sofCode, int ukprn, string providerName, string ilrFile, string lastSubmittedIlrFileName, List<IDevolvedAdultEducationFundingCategory> fundingCategories)
        {
            SofCode = sofCode;
            Ukprn = ukprn;
            ProviderName = providerName;
            IlrFile = ilrFile;
            LastSubmittedIlrFileName = lastSubmittedIlrFileName;
            FundingCategories = fundingCategories ?? new List<IDevolvedAdultEducationFundingCategory>();
        }

        public string SofCode { get; }

        public int Ukprn { get; }

        public string ProviderName { get; }

        public string IlrFile { get; }

        public string LastSubmittedIlrFileName { get; }

        public List<IDevolvedAdultEducationFundingCategory> FundingCategories { get; }
    }
}
