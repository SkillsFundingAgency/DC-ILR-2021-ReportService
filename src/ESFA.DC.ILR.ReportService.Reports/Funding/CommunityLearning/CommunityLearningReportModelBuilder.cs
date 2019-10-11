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

            var categoryData = BuildCategoryData(message);

            var categories = BuildCategories(categoryData);

            return new CommunityLearningReportModel(headerData, categories, footerData);
        }

        public List<ICategory> BuildCategories(List<CommunityLearningData> communityLearningData)
        {
            return new List<ICategory>
            {
                new Category
                {
                    CategoryName = ReportCategoryConstants.TotalCommunityLearning,
                    SubCategories = new List<ISubCategory>
                    {
                        new SubCategory
                        {
                            SubCategoryName = ReportCategoryConstants.SixteenToEighteen,
                            TotalLearners = DistinctCount(communityLearningData.Where(x => x.SixteenToEighteen).Select(l => l.LearnerRefNumber)),
                            TotalStartedInFundingYear = DistinctCount(communityLearningData.Where(x => x.SixteenToEighteen && x.EarliestStartDate && x.LearnStartDateIsInYear).Select(l => l.LearnerRefNumber)),
                            TotalEnrolmentsInFundingYear = communityLearningData.Where(x => x.SixteenToEighteen && x.LearnStartDateIsInYear).Count(),
                        },
                        new SubCategory
                        {
                            SubCategoryName = ReportCategoryConstants.Adult,
                            TotalLearners = DistinctCount(communityLearningData.Where(x => x.Adult).Select(l => l.LearnerRefNumber)),
                            TotalStartedInFundingYear = DistinctCount(communityLearningData.Where(x => x.Adult && x.EarliestStartDate && x.LearnStartDateIsInYear).Select(l => l.LearnerRefNumber)),
                            TotalEnrolmentsInFundingYear = communityLearningData.Where(x => x.Adult && x.LearnStartDateIsInYear).Count(),
                        }
                    }
                },
                new Category
                {
                    CategoryName = ReportCategoryConstants.PersonalAndCommunity,
                    SubCategories = new List<ISubCategory>
                    {
                        new SubCategory
                        {
                            SubCategoryName = ReportCategoryConstants.PersonalAndCommunity,
                            TotalLearners = DistinctCount(communityLearningData.Where(x => x.PersonalAndCommunityDevelopmentLearning).Select(l => l.LearnerRefNumber)),
                            TotalStartedInFundingYear = DistinctCount(communityLearningData.Where(x => x.PersonalAndCommunityDevelopmentLearning && x.EarliestStartDate && x.LearnStartDateIsInYear).Select(l => l.LearnerRefNumber)),
                            TotalEnrolmentsInFundingYear = communityLearningData.Where(x => x.PersonalAndCommunityDevelopmentLearning && x.LearnStartDateIsInYear).Count(),
                        }
                    }
                },
                new Category
                {
                    CategoryName = ReportCategoryConstants.NeighbourhoodLearning,
                    SubCategories = new List<ISubCategory>
                    {
                        new SubCategory
                        {
                            SubCategoryName = ReportCategoryConstants.NeighbourhoodLearning,
                            TotalLearners = DistinctCount(communityLearningData.Where(x => x.NeighbourhoodLearningInDeprivedCommunities).Select(l => l.LearnerRefNumber)),
                            TotalStartedInFundingYear = DistinctCount(communityLearningData.Where(x => x.NeighbourhoodLearningInDeprivedCommunities && x.EarliestStartDate && x.LearnStartDateIsInYear).Select(l => l.LearnerRefNumber)),
                            TotalEnrolmentsInFundingYear = communityLearningData.Where(x => x.NeighbourhoodLearningInDeprivedCommunities && x.LearnStartDateIsInYear).Count(),
                        }
                    }
                },
                new Category
                {
                    CategoryName = ReportCategoryConstants.FamilyEnglishMathsAndLanguage,
                    SubCategories = new List<ISubCategory>
                    {
                        new SubCategory
                        {
                            SubCategoryName = ReportCategoryConstants.SixteenToEighteen,
                            TotalLearners = DistinctCount(communityLearningData.Where(x => x.SixteenToEighteen && x.FamilyEnglishMathsAndLanguage).Select(l => l.LearnerRefNumber)),
                            TotalStartedInFundingYear = DistinctCount(communityLearningData.Where(x => x.SixteenToEighteen && x.FamilyEnglishMathsAndLanguage && x.EarliestStartDate && x.LearnStartDateIsInYear).Select(l => l.LearnerRefNumber)),
                            TotalEnrolmentsInFundingYear = communityLearningData.Where(x => x.SixteenToEighteen && x.FamilyEnglishMathsAndLanguage && x.LearnStartDateIsInYear).Count(),
                        },
                        new SubCategory
                        {
                            SubCategoryName = ReportCategoryConstants.Adult,
                            TotalLearners = DistinctCount(communityLearningData.Where(x => x.Adult && x.FamilyEnglishMathsAndLanguage).Select(l => l.LearnerRefNumber)),
                            TotalStartedInFundingYear = DistinctCount(communityLearningData.Where(x => x.Adult && x.FamilyEnglishMathsAndLanguage && x.EarliestStartDate && x.LearnStartDateIsInYear).Select(l => l.LearnerRefNumber)),
                            TotalEnrolmentsInFundingYear = communityLearningData.Where(x => x.Adult && x.FamilyEnglishMathsAndLanguage && x.LearnStartDateIsInYear).Count(),
                        }
                    }
                },
                new Category
                {
                    CategoryName = ReportCategoryConstants.WiderFamilyLearning,
                    SubCategories = new List<ISubCategory>
                    {
                        new SubCategory
                        {
                            SubCategoryName = ReportCategoryConstants.SixteenToEighteen,
                            TotalLearners = DistinctCount(communityLearningData.Where(x => x.SixteenToEighteen && x.WiderFamilyLearning).Select(l => l.LearnerRefNumber)),
                            TotalStartedInFundingYear = DistinctCount(communityLearningData.Where(x => x.SixteenToEighteen  && x.WiderFamilyLearning && x.EarliestStartDate && x.LearnStartDateIsInYear).Select(l => l.LearnerRefNumber)),
                            TotalEnrolmentsInFundingYear = communityLearningData.Where(x => x.SixteenToEighteen  && x.WiderFamilyLearning && x.LearnStartDateIsInYear).Count(),
                        },
                        new SubCategory
                        {
                            SubCategoryName = ReportCategoryConstants.Adult,
                            TotalLearners = DistinctCount(communityLearningData.Where(x => x.Adult && x.WiderFamilyLearning).Select(l => l.LearnerRefNumber)),
                            TotalStartedInFundingYear = DistinctCount(communityLearningData.Where(x => x.Adult && x.WiderFamilyLearning && x.EarliestStartDate && x.LearnStartDateIsInYear).Select(l => l.LearnerRefNumber)),
                            TotalEnrolmentsInFundingYear = communityLearningData.Where(x => x.Adult && x.WiderFamilyLearning && x.LearnStartDateIsInYear).Count(),
                        }
                    }
                },
            };
        }

        public List<CommunityLearningData> BuildCategoryData(IMessage message)
        {
            return message.Learners?
                .SelectMany(l => l.LearningDeliveries?
                .Where(LearningDeliveryFilter)
                .Select(
                    ld => new CommunityLearningData
                    {
                        LearnerRefNumber = l.LearnRefNumber,
                        AimSeqNumber = ld.AimSeqNumber,
                        LearnStartDate = ld.LearnStartDate,
                        SixteenToEighteen = IsSixteenToEighteen(l.DateOfBirthNullable, ld.LearnStartDate),
                        Adult = IsAdult(l.DateOfBirthNullable, ld.LearnStartDate),
                        EarliestStartDate = IsEarliestStartDateForDeliveries(l.LearningDeliveries, ld.LearnStartDate),
                        LearnStartDateIsInYear = LearnStartDateIsWithinYear(ld.LearnStartDate),
                        PersonalAndCommunityDevelopmentLearning = HasAnyASLFamTypeForFamCode(ld.LearningDeliveryFAMs, LearningDeliveryFAMCodeConstants.ASL_Personal),
                        NeighbourhoodLearningInDeprivedCommunities = HasAnyASLFamTypeForFamCode(ld.LearningDeliveryFAMs, LearningDeliveryFAMCodeConstants.ASL_Neighbour),
                        FamilyEnglishMathsAndLanguage = HasAnyASLFamTypeForFamCode(ld.LearningDeliveryFAMs, LearningDeliveryFAMCodeConstants.ASL_FamilyEnglishMathsLanguage),
                        WiderFamilyLearning = HasAnyASLFamTypeForFamCode(ld.LearningDeliveryFAMs, LearningDeliveryFAMCodeConstants.ASL_WiderFamily),
                    })).ToList();
        }

        public bool IsSixteenToEighteen(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            if (!dateOfBirth.HasValue)
            {
                return false;
            }

            return YearsBetween(dateOfBirth.Value, learnStartDate) < 19;
        }

        public bool IsAdult(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            if (!dateOfBirth.HasValue)
            {
                return true;
            }

            return dateOfBirth.HasValue && YearsBetween(dateOfBirth.Value, learnStartDate) >= 19;
        }

        public bool IsEarliestStartDateForDeliveries(IReadOnlyCollection<ILearningDelivery> learningDeliveries, DateTime learnStartDate)
        {
            return learningDeliveries?.Where(LearningDeliveryFilter).Min(ld => ld.LearnStartDate) == learnStartDate;
        }

        public bool LearnStartDateIsWithinYear(DateTime learnStartDate)
        {
            return learnStartDate >= ReportingConstants.BeginningOfYear && learnStartDate <= ReportingConstants.EndOfYear;
        }

        public bool HasAnyASLFamTypeForFamCode(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famCode)
        {
            return learningDeliveryFAMs.Any(ldf => ldf.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ASL && ldf.LearnDelFAMCode == famCode);
        }

        public IDictionary<string, string> BuildFooterData(IReportServiceContext reportServiceContext, IMessage message, ReferenceDataRoot referenceDataRoot)
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
                {SummaryPageConstants.Page, SummaryPageConstants.DefaultPageNumber}
            };
        }

        public IDictionary<string, string> BuildHeaderData(IReportServiceContext reportServiceContext, ReferenceDataRoot referenceDataRoot)
        {
            var organisationName = referenceDataRoot.Organisations?.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name;

            return new Dictionary<string, string>()
            {
                {SummaryPageConstants.ProviderName, organisationName},
                {SummaryPageConstants.UKPRN, reportServiceContext.Ukprn.ToString()},
                {SummaryPageConstants.ILRFile, reportServiceContext.OriginalFilename},
                {SummaryPageConstants.Year, reportServiceContext.CollectionYear},
                {SummaryPageConstants.SecurityClassification, ReportingConstants.OfficialSensitive}
            };
        }

        private bool LearningDeliveryFilter(ILearningDelivery ld) =>
            ld.FundModel == FundModelConstants.FM10
            && ld.LearningDeliveryFAMs.Any(ldf => ldf.LearnDelFAMType == LearningDeliveryFAMTypeConstants.SOF
            && ldf.LearnDelFAMCode == LearningDeliveryFAMCodeConstants.SOF_ESFA);

        private int YearsBetween(DateTime start, DateTime end)
        {
            var years = end.Year - start.Year;

            return end < start.AddYears(years) ? years - 1 : years;
        }

        private int DistinctCount<T>(IEnumerable<T> objectToCount)
        {
            return objectToCount.Distinct().Count();
        }

    }
}
