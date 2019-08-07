using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface
{
    public interface IDevolvedAdultEducationFundingSummaryReport
    {
        string SofCode { get; }

        int Ukprn { get; }

        string ProviderName { get; }

        string IlrFile { get; }

        string LastSubmittedIlrFileName { get; }

        DateTime FilePreparationDate { get; }

        string OrganisationVersion { get; }

        string LARSVersion { get; }

        string PostcodeVersion { get; }

        string EmployersVersion { get; }

        string ApplicationVersion { get; }

        string ReportGeneratedAt { get; }

        List<IDevolvedAdultEducationFundingCategory> FundingCategories { get; }
    }
}
