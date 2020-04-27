﻿using System;
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
            string lastSubmittedIlrFileName, 
            DateTime? filePreparationDate,
            string latestEasFileName,
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
            LastSubmittedIlrFileName = lastSubmittedIlrFileName;
            LatestEasFileName = latestEasFileName;
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

        public string LastSubmittedIlrFileName { get; }

        public string LatestEasFileName { get; }

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
