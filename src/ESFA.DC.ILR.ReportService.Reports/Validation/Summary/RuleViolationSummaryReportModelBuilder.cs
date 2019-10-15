using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Validation.Summary.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary
{
    public class RuleViolationSummaryReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<RuleViolationSummaryReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private const string ReportGeneratedTimeStringFormat = "HH:mm:ss on dd/MM/yyyy";

        public RuleViolationSummaryReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public RuleViolationSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            //return FakeRuleViolationSummaryReportModel();

            ILooseMessage message = reportServiceDependentData.Get<ILooseMessage>();
            ReferenceDataRoot referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();
            List<ValidationError> validationErrors = reportServiceDependentData.Get<List<ValidationError>>();

            var model = new RuleViolationSummaryReportModel();

            var organisation = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn);
            var organisationName = organisation?.Name ?? string.Empty;


            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            var reportGeneratedAt = "Report generated at: " + dateTimeNowUk.ToString(ReportGeneratedTimeStringFormat);
            var learners = message?.Learners.ToList() ?? Enumerable.Empty<ILooseLearner>().ToList();
            var looseLearnerDestinationAndProgressions = message?.LearnerDestinationAndProgressions.ToList() ?? Enumerable.Empty<ILooseLearnerDestinationAndProgression>().ToList();

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn;
            model.IlrFile = ExtractFileName(reportServiceContext.OriginalFilename);
            model.Year = ReportingConstants.Year;

            // body
            var looseLearners = learners.DistinctBy(x => x.LearnRefNumber).ToList();
            var learnersWithValidationErrors = validationErrors.Select(x => x.LearnerReferenceNumber).Distinct().ToList();
            var learnersWithErrors = validationErrors.Where(x => x.Severity.CaseInsensitiveEquals("E")).Select(x => x.LearnerReferenceNumber).Distinct().ToList();
            var learnersWithWarnings = validationErrors.Where(x => x.Severity.CaseInsensitiveEquals("W")).Select(x => x.LearnerReferenceNumber).Distinct().ToList();
            
            model.TotalNoOfErrors = validationErrors.Count(x => x.Severity.CaseInsensitiveEquals("E"));
            model.TotalNoOfWarnings = validationErrors.Count(x => x.Severity.CaseInsensitiveEquals("W"));

            model.TotalNoOfLearners = looseLearners.DistinctByCount(x => x.LearnRefNumber);
            model.TotalNoOfLearnersWithWarnings = learnersWithWarnings.Count();

            List<ILooseLearner> fullyValidLearners = looseLearners.Where(x => !learnersWithValidationErrors.Contains(x.LearnRefNumber)).ToList();
            List<ILooseLearner> inValidLearners = looseLearners.Where(x => learnersWithErrors.Contains(x.LearnRefNumber)).ToList();

            var learningDeliveries = learners.SelectMany(x => x.LearningDeliveries).ToList();

            model.FullyValidLearners = new RuleViolationsTotalModel
            {
                Total = fullyValidLearners.DistinctByCount(x => x.LearnRefNumber),
                Apprenticeships = GetFundModelCount(fullyValidLearners, FundModelConstants.FM36),
                Funded1619 = GetFundModelCount(fullyValidLearners, FundModelConstants.FM25),
                AdultSkilledFunded = GetFundModelCount(fullyValidLearners, FundModelConstants.FM35),
                CommunityLearningFunded = GetFundModelCount(fullyValidLearners, FundModelConstants.FM10),
                ESFFunded = GetFundModelCount(fullyValidLearners, FundModelConstants.FM70),
                OtherAdultFunded = GetFundModelCount(fullyValidLearners, FundModelConstants.FM81),
                Other1619Funded = GetFundModelCount(fullyValidLearners, FundModelConstants.FM82),
                NonFunded = GetFundModelCount(fullyValidLearners, FundModelConstants.FM99)
            };

            model.InvalidLearners = new RuleViolationsTotalModel
            {
                Total = inValidLearners.DistinctByCount(x => x.LearnRefNumber),
                Apprenticeships = GetFundModelCount(inValidLearners, FundModelConstants.FM36),
                Funded1619 = GetFundModelCount(inValidLearners, FundModelConstants.FM25),
                AdultSkilledFunded = GetFundModelCount(inValidLearners, FundModelConstants.FM35),
                CommunityLearningFunded = GetFundModelCount(inValidLearners, FundModelConstants.FM10),
                ESFFunded = GetFundModelCount(inValidLearners, FundModelConstants.FM70),
                OtherAdultFunded = GetFundModelCount(inValidLearners, FundModelConstants.FM81),
                Other1619Funded = GetFundModelCount(inValidLearners, FundModelConstants.FM82),
                NonFunded = GetFundModelCount(inValidLearners, FundModelConstants.FM99)
            };

            model.LearningDeliveries = new RuleViolationsTotalModel
            {
                Total = learningDeliveries.Count(),
                Apprenticeships = GetLearningDeliveriesFundModelCount(learningDeliveries, FundModelConstants.FM36),
                Funded1619 = GetLearningDeliveriesFundModelCount(learningDeliveries, FundModelConstants.FM25),
                AdultSkilledFunded = GetLearningDeliveriesFundModelCount(learningDeliveries, FundModelConstants.FM35),
                CommunityLearningFunded = GetLearningDeliveriesFundModelCount(learningDeliveries, FundModelConstants.FM10),
                ESFFunded = GetLearningDeliveriesFundModelCount(learningDeliveries, FundModelConstants.FM70),
                OtherAdultFunded = GetLearningDeliveriesFundModelCount(learningDeliveries, FundModelConstants.FM81),
                Other1619Funded = GetLearningDeliveriesFundModelCount(learningDeliveries, FundModelConstants.FM82),
                NonFunded = GetLearningDeliveriesFundModelCount(learningDeliveries, FundModelConstants.FM99),
            };

            model.LearningDeliveries.AdvancedLoanLearningDeliveries =
                learningDeliveries.Where(x => x.LearningDeliveryFAMs != null && x.FundModelNullable!=null)
                                  .Count(x => x.FundModelNullable.Value == FundModelConstants.FM99 &&
                                            x.LearningDeliveryFAMs.Any(y => y.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ADL)));


            model.LearnerDestinationProgressionSummary = new LearnerDestinationProgressionSummary()
            {
                 Total = looseLearnerDestinationAndProgressions.Count,
                ValidLearnerDestinationProgressions = looseLearnerDestinationAndProgressions.Count(x => !learnersWithValidationErrors.Contains(x.LearnRefNumber)),
                InValidLearnerDestinationProgressions = looseLearnerDestinationAndProgressions.Count(x => learnersWithValidationErrors.Contains(x.LearnRefNumber)),
                LearnerDestinationProgressionsWithWarnings = looseLearnerDestinationAndProgressions.Count(x => learnersWithWarnings.Contains(x.LearnRefNumber)),
            };

            // Footer
            model.ReportGeneratedAt = reportGeneratedAt;
            model.ApplicationVersion = reportServiceContext.ServiceReleaseVersion;
            model.ComponentSetVersion = "NA";
            model.OrganisationData = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            model.LargeEmployerData = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            model.LarsData = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            model.PostcodeData = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            model.FilePreparationDate = message?.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString("dd/MM/yyyy");
            model.CofRemovalData = referenceDataRoot.MetaDatas.ReferenceDataVersions.CoFVersion.Version;
            //Todo: 
            //model.DevolvedPostcodesData
            return model;
        }

        private int GetLearningDeliveriesFundModelCount(List<ILooseLearningDelivery> learningDeliveries, int fundModel)
        {
            return learningDeliveries.Where(x =>x.FundModelNullable!=null).Count(x => x.FundModelNullable.Value == fundModel);
        }

        private int GetFundModelCount(List<ILooseLearner> learners, int fundModel)
        {
            return learners.Where(x => x.LearningDeliveries != null).Where(x => x.LearningDeliveries.Any(y => y.FundModelNullable.GetValueOrDefault() == fundModel)).DistinctByCount( x=> x.LearnRefNumber);
            //int count = 0;
            //foreach (var learner in learners.Where(x => x.LearningDeliveries != null))
            //{
            //    if (learner.LearningDeliveries.Any(x => x.FundModelNullable.GetValueOrDefault() == fundModel))
            //    {
            //        count++;
            //    }
            //}

            //return count;
        }

        private static RuleViolationSummaryReportModel FakeRuleViolationSummaryReportModel()
        {
            return new RuleViolationSummaryReportModel()
            {
                //Header
                ProviderName = "Provider XYZ",
                Ukprn = 987654321,
                IlrFile = "ILR-12345678-1920-20191005-151322-01.xml",
                Year = "2019/20",
                TotalNoOfErrors = 176,
                TotalNoOfWarnings = 193,
                TotalNoOfLearners = 177,
                TotalNoOfLearnersWithWarnings = 18,

                FullyValidLearners = new RuleViolationsTotalModel()
                {
                    Total = 41,
                    Apprenticeships = 1,
                    Funded1619 = 2,
                    AdultSkilledFunded = 3,
                    CommunityLearningFunded = 4,
                    ESFFunded = 5,
                    OtherAdultFunded = 6,
                    Other1619Funded = 7,
                    NonFunded = 8
                },
                InvalidLearners = new RuleViolationsTotalModel()
                {
                    Total = 180,
                    Apprenticeships = 9,
                    Funded1619 = 10,
                    AdultSkilledFunded = 11,
                    CommunityLearningFunded = 12,
                    ESFFunded = 13,
                    OtherAdultFunded = 14,
                    Other1619Funded = 15,
                    NonFunded = 16
                },
                LearningDeliveries = new RuleViolationsTotalModel()
                {
                    Total = 336,
                    Apprenticeships = 17,
                    Funded1619 = 18,
                    AdultSkilledFunded = 19,
                    CommunityLearningFunded = 20,
                    ESFFunded = 21,
                    OtherAdultFunded = 22,
                    Other1619Funded = 23,
                    NonFunded = 24,
                    AdvancedLoanLearningDeliveries = 25
                },
                LearnerDestinationProgressionSummary = new LearnerDestinationProgressionSummary()
                {
                    Total = 401,
                    ValidLearnerDestinationProgressions = 26,
                    InValidLearnerDestinationProgressions = 27,
                    LearnerDestinationProgressionsWithWarnings = 28
                },
                Errors = new List<ValidationErrorMessageModel>()
                {
                    new ValidationErrorMessageModel()
                    {
                        RuleName = "Rule1",
                        Message = "Message1",
                        Occurrences = 1
                    },
                    new ValidationErrorMessageModel()
                    {
                        RuleName = "Rule2",
                        Message = "Message2",
                        Occurrences = 2
                    }
                },
                Warnings = new List<ValidationErrorMessageModel>()
                {
                    new ValidationErrorMessageModel()
                    {
                        RuleName = "Rule3",
                        Message = "Message3",
                        Occurrences = 3
                    },
                    new ValidationErrorMessageModel()
                    {
                        RuleName = "Rule4",
                        Message = "Message4",
                        Occurrences = 4
                    },
                    new ValidationErrorMessageModel()
                    {
                        RuleName = "Rule54",
                        Message = "Message5",
                        Occurrences = 5
                    }
                },
                ApplicationVersion = "11.22.3300.4321",
                FilePreparationDate = "06/11/2019",
                LarsData = "LVersion 3.0.0: 17 Sep 2019 08:13:35:643",
                OrganisationData = "OVersion 3.0.0: 17 Sep 2019 08:13:35:643",
                LargeEmployerData = "LEVersion 3.0.0: 17 Sep 2019 08:13:35:643",
                CampusIdData = "12",
                PostcodeData = "PVersion 3.0.0: 17 Sep 2019 08:13:35:643",
                ReportGeneratedAt = "today",
                DevolvedPostcodesData = "devolved data"
            };
        }
    }
}
