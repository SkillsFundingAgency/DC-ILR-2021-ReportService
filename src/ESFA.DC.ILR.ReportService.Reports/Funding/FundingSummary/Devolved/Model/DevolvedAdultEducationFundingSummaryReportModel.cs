using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model
{
    public class DevolvedAdultEducationFundingSummaryReportModel : IDevolvedAdultEducationFundingSummaryReport
    {
        public DevolvedAdultEducationFundingSummaryReportModel(
            string sofCode, 
            int ukprn, 
            string providerName, 
            string ilrFile, 
            string lastSubmittedIlrFileName, 
            DateTime filePreparationDate,
            string organisationVersion,
            string larsVersion,
            string postcodeVersion,
            string employersVersion,
            string applicationVersion,
            string reportGeneratedAt,
            List<IDevolvedAdultEducationFundingCategory> fundingCategories)
        {
            SofCode = sofCode;
            Ukprn = ukprn;
            ProviderName = providerName;
            IlrFile = ilrFile;
            LastSubmittedIlrFileName = lastSubmittedIlrFileName;
            FilePreparationDate = filePreparationDate;
            OrganisationVersion = organisationVersion;
            LARSVersion = larsVersion;
            PostcodeVersion = postcodeVersion;
            EmployersVersion = employersVersion;
            ApplicationVersion = applicationVersion;
            ReportGeneratedAt = reportGeneratedAt;
            FundingCategories = fundingCategories ?? new List<IDevolvedAdultEducationFundingCategory>();
        }

        public string SofCode { get; }

        public int Ukprn { get; }

        public string ProviderName { get; }

        public string IlrFile { get; }

        public string LastSubmittedIlrFileName { get; }

        public DateTime FilePreparationDate { get; }

        public string OrganisationVersion { get; }

        public string LARSVersion { get; }

        public string PostcodeVersion { get; }

        public string EmployersVersion { get; }

        public string ApplicationVersion { get; }

        public string ReportGeneratedAt { get; }

        public List<IDevolvedAdultEducationFundingCategory> FundingCategories { get; }
    }
}
