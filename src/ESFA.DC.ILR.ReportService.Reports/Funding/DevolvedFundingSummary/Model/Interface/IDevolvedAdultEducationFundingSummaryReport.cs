using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface
{
    public interface IDevolvedAdultEducationFundingSummaryReport
    {
        string SofCode { get; }

        int Ukprn { get; }

        string ProviderName { get; }

        string IlrFile { get; }

        string LastSubmittedIlrFileName { get; }

        List<IDevolvedAdultEducationFundingCategory> FundingCategories { get; }
    }
}
