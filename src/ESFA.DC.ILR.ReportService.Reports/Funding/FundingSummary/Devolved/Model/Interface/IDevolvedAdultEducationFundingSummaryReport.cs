using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface
{
    public interface IDevolvedAdultEducationFundingSummaryReport
    {
        McaGlaSofLookup SofLookup { get; }

        int Ukprn { get; }

        string ProviderName { get; }

        string IlrFile { get; }

        string LastSubmittedIlrFileName { get; }

        string EasLastUpdated { get; }

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
