using System;

namespace ESFA.DC.ILR.ReportService.Reports.Model.Interface
{
    public interface ISummaryPage
    {
        string ProviderName { get; }

        int UKPRN { get; }

        string ILRFile { get; }

        string LastILRFileUpdate { get; }

        string LastEASUpdate { get; }

        string SecurityClassification { get; }

        string ApplicationVersion { get; }

        string FilePreparationDate { get; }

        string LARSVersion { get; }

        string PostcodeVersion { get; }

        string OrganisationVersion { get; }

        string LargeEmployersVersion { get; }

        string ReportGeneratedAt { get; }
    }
}
