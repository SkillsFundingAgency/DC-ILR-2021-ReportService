using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model
{
    public class DevolvedAdultEducationFundingSummaryReportModel : IDevolvedAdultEducationFundingSummaryReport
    {
        public DevolvedAdultEducationFundingSummaryReportModel(
            McaGlaSofLookup sofLookup, 
            int ukprn, 
            string providerName, 
            string ilrFile, 
            string lastIlrFileUpdate,
            DateTime? filePreparationDate,
            string lastEasFileUpdate,
            string easFile,
            string organisationVersion,
            string larsVersion,
            string postcodeVersion,
            string employersVersion,
            string applicationVersion,
            string reportGeneratedAt,
            List<IDevolvedAdultEducationFundingCategory> fundingCategories)
        {
            SofLookup = sofLookup;
            Ukprn = ukprn;
            ProviderName = providerName;
            IlrFile = ilrFile;
            EasFile = easFile;
            LastIlrFileUpdate = lastIlrFileUpdate;
            LastEasFileUpdate = lastEasFileUpdate;
            FilePreparationDate = filePreparationDate;
            OrganisationVersion = organisationVersion;
            LARSVersion = larsVersion;
            PostcodeVersion = postcodeVersion;
            EmployersVersion = employersVersion;
            ApplicationVersion = applicationVersion;
            ReportGeneratedAt = reportGeneratedAt;
            FundingCategories = fundingCategories ?? new List<IDevolvedAdultEducationFundingCategory>();
        }

        public McaGlaSofLookup SofLookup { get; }

        public int Ukprn { get; }

        public string ProviderName { get; }

        public string IlrFile { get; }

        public string LastIlrFileUpdate { get; }

        public string EasFile { get; }

        public string LastEasFileUpdate { get; }

        public DateTime? FilePreparationDate { get; }

        public string OrganisationVersion { get; }

        public string LARSVersion { get; }

        public string PostcodeVersion { get; }

        public string EmployersVersion { get; }

        public string ApplicationVersion { get; }

        public string ReportGeneratedAt { get; }

        public List<IDevolvedAdultEducationFundingCategory> FundingCategories { get; }
    }
}
