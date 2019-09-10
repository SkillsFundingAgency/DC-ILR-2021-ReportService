using System;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Model
{
    public class SummaryPageModel : ISummaryPage
    {
        public string ProviderName { get; set; }

        public int UKPRN { get; set; }

        public string ILRFile { get; set; }

        public string LastILRFileUpdate { get; set; }

        public string LastEASUpdate { get; set; }

        public string SecurityClassification { get; set; }

        public string ApplicationVersion { get; set; }

        public string FilePreparationDate { get; set; }

        public string LARSVersion { get; set; }

        public string PostcodeVersion { get; set; }

        public string OrganisationVersion { get; set; }

        public string LargeEmployersVersion { get; set; }

        public string ReportGeneratedAt { get; set; }
    }
}
