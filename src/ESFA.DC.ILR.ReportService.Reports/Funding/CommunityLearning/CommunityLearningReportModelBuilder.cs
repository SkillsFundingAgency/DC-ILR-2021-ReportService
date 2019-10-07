using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning
{
    public class CommunityLearningReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<CommunityLearningReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public CommunityLearningReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public CommunityLearningReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            return BuildModel(reportServiceContext, message, referenceData);
        }

        public CommunityLearningReportModel BuildModel(IReportServiceContext reportServiceContext, IMessage message, ReferenceDataRoot referenceData)
        {
            var headerData = BuildHeaderData(reportServiceContext, referenceData);
            var footerData = BuildFooterData(reportServiceContext, message, referenceData);

            var categoryData = BuildCategories(message);

            return new CommunityLearningReportModel(headerData, categoryData, footerData);
        }

        public List<ICategory> BuildCategories(IMessage message)
        {
            var applicableLearners = message.Learners
                .SelectMany(l => l.LearningDeliveries
                .Where(ld => ld.FundModel == FundModelConstants.FM10
                && ld.LearningDeliveryFAMs.Any(ldf => ldf.LearnDelFAMType == LearningDeliveryFAMTypeConstants.SOF
                && ldf.LearnDelFAMCode == LearningDeliveryFAMCodeConstants.SOF_ESFA))
                .Select(
                    ld => new CommunityLearningData
                    {
                        LearnerRefNumber = l.LearnRefNumber,
                        DateOfBirth = l.DateOfBirthNullable,
                        AimSeqNumber = ld.AimSeqNumber,
                        LearnStartDate = ld.LearnStartDate,
                        SixteenToEighteen = true,
                        Adult = true,
                        LearnStartDateIsInYear = true,
                        PersonalAndCommunityDevelopmentLearning = true,
                        NeighbourhoodLearningInDeprivedCommunities = true,
                        FamilyEnglishMathsAndLanguage = true,
                        WiderFamilyLearning = true
                    })).ToList();
                    

            return new List<ICategory>
            {
                BuildTotalLearnersCategory(applicableLearners)
            };
        }

        public ICategory BuildTotalLearnersCategory(ICollection<CommunityLearningData> communityLearningData)
        {
            return new Category
            {
                CategoryName = ReportCategoryConstants.TotalCommunityLearning,
                SubCategories = new List<ISubCategory>
                {
                    new SubCategory
                    {
                        SubCategoryName = ReportCategoryConstants.TotalCommunityLearning1618,
                        TotalLearners = DistinctCount(communityLearningData.Where(x => x.SixteenToEighteen == true).Select(l => l.LearnerRefNumber))
                    },
                    new SubCategory
                    {
                        SubCategoryName = ReportCategoryConstants.TotalCommunityLearningAdult,
                        TotalLearners = DistinctCount(communityLearningData.Where(x => x.Adult == true).Select(l => l.LearnerRefNumber))
                    }
                }
            };
        }

        private IDictionary<string, string> BuildFooterData(IReportServiceContext reportServiceContext, IMessage message, ReferenceDataRoot referenceDataRoot)
        {
            var filePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString();
            var orgVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            var larsVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            var employersVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            var postcodesVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            var reportGeneratedAt = dateTimeNowUk.ToString(reportGeneratedTimeStringFormat);

            return new Dictionary<string, string>()
            {
                {SummaryPageConstants.ApplicationVersion, reportServiceContext.ServiceReleaseVersion},
                {SummaryPageConstants.FilePreparationDate, filePreparationDate},
                {SummaryPageConstants.LARSVersion, larsVersion},
                {SummaryPageConstants.PostcodeVersion, postcodesVersion},
                {SummaryPageConstants.OrganisationVersion, orgVersion},
                {SummaryPageConstants.LargeEmployersVersion, employersVersion},
                {SummaryPageConstants.ReportGeneratedAt, reportGeneratedAt},
                {SummaryPageConstants.Page, "1 of 1"} // ToDo: find best place for this
            };
        }

        public IDictionary<string, string> BuildHeaderData(IReportServiceContext reportServiceContext, ReferenceDataRoot referenceDataRoot)
        {
            var organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;
            var easLastUpdate = referenceDataRoot.MetaDatas.ReferenceDataVersions?.EasUploadDateTime.UploadDateTime.ToString();

            return new Dictionary<string, string>()
            {
                {SummaryPageConstants.ProviderName, organisationName},
                {SummaryPageConstants.UKPRN, reportServiceContext.Ukprn.ToString()},
                {SummaryPageConstants.ILRFile, reportServiceContext.OriginalFilename},
                {SummaryPageConstants.Year, reportServiceContext.CollectionYear},
                {SummaryPageConstants.SecurityClassification, ReportingConstants.OfficialSensitive}
            };
        }

        public int AgeAtStart(ILearner learner)
        {
            return -1;
        }

        public int YearsBetween(DateTime start, DateTime end)
        {
            var years = end.Year - start.Year;

            return end < start.AddYears(years) ? years - 1 : years;
        }

        public bool IsSixteenToEighteen(ILearner learner)
        {
            if (learner.DateOfBirthNullable == null)
            {
                return false;
            }

            return learner.LearningDeliveries.Any(ld => YearsBetween(learner.DateOfBirthNullable.Value, ld.LearnStartDate) < 19);
        }
        public int DistinctCount<T>(IEnumerable<T> objectToCount)
        {
            return objectToCount.Distinct().Count();
        }
    }
}
