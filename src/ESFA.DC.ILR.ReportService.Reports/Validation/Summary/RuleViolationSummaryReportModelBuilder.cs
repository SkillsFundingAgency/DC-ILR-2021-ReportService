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
            ILooseMessage message = reportServiceDependentData.Get<ILooseMessage>();
            ReferenceDataRoot referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();
            List<ValidationError> validationErrors = reportServiceDependentData.Get<List<ValidationError>>();
            var validationErrorsMetadata = referenceDataRoot.MetaDatas.ValidationErrors;
            var model = new RuleViolationSummaryReportModel();
            var organisation = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn);
            var organisationName = organisation?.Name ?? string.Empty;

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            var reportGeneratedAt = "Report generated at: " + dateTimeNowUk.ToString(ReportGeneratedTimeStringFormat);
            var looseLearners = message?.Learners?.ToList() ?? Enumerable.Empty<ILooseLearner>().ToList();
            var looseLearnerDestinationAndProgressions = message?.LearnerDestinationAndProgressions?.ToList() ?? Enumerable.Empty<ILooseLearnerDestinationAndProgression>().ToList();

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn;
            model.IlrFile = ExtractFileName(reportServiceContext.OriginalFilename);
            model.Year = ReportingConstants.Year;

            // body
            var looseLearnersList = looseLearners.DistinctBy(x => x.LearnRefNumber).ToList();
            var validationErrorsList = validationErrors.Where(x => x.Severity.CaseInsensitiveEquals("E")).ToList();
            var validationErrorWarningsList = validationErrors.Where(x => x.Severity.CaseInsensitiveEquals("W")).ToList();

            var learnersWithValidationErrors = validationErrors.Select(x => x.LearnerReferenceNumber).Distinct().ToList();
            var learnersWithErrors = validationErrorsList.Select(x => x.LearnerReferenceNumber).Distinct().ToList();
            var learnersWithWarnings = validationErrorWarningsList.Select(x => x.LearnerReferenceNumber).Distinct().ToList();

            model.TotalNoOfErrors = validationErrorsList.Count;
            model.TotalNoOfWarnings = validationErrorWarningsList.Count;

            model.TotalNoOfLearners = looseLearnersList.DistinctByCount(x => x.LearnRefNumber);
            model.TotalNoOfLearnersWithWarnings = learnersWithWarnings.Count();

            List<ILooseLearner> fullyValidLearners = looseLearnersList.Where(x => !learnersWithValidationErrors.Contains(x.LearnRefNumber)).ToList();
            List<ILooseLearner> inValidLearners = looseLearnersList.Where(x => learnersWithErrors.Contains(x.LearnRefNumber)).ToList();

            var learningDeliveries = looseLearnersList.SelectMany(x => x.LearningDeliveries).ToList();

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
                Total = learningDeliveries.Count,
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
                learningDeliveries.Where(x => x.LearningDeliveryFAMs != null && x.FundModelNullable != null)
                                  .Count(x => x.FundModelNullable.Value == FundModelConstants.FM99 &&
                                            x.LearningDeliveryFAMs.Any(y => y.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ADL)));


            model.LearnerDestinationProgressionSummary = new LearnerDestinationProgressionSummary()
            {
                Total = looseLearnerDestinationAndProgressions.Count,
                ValidLearnerDestinationProgressions = looseLearnerDestinationAndProgressions.Count(x => !learnersWithValidationErrors.Contains(x.LearnRefNumber)),
                InValidLearnerDestinationProgressions = looseLearnerDestinationAndProgressions.Count(x => learnersWithValidationErrors.Contains(x.LearnRefNumber)),
                LearnerDestinationProgressionsWithWarnings = looseLearnerDestinationAndProgressions.Count(x => learnersWithWarnings.Contains(x.LearnRefNumber)),
            };

            model.Errors = GetValidationErrorMessageModels(validationErrorsList, validationErrorsMetadata);
            model.Warnings = GetValidationErrorMessageModels(validationErrorWarningsList, validationErrorsMetadata);

            // Footer
            model.ReportGeneratedAt = reportGeneratedAt;
            model.ApplicationVersion = reportServiceContext.ServiceReleaseVersion;
            model.OrganisationData = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            model.LargeEmployerData = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            model.LarsData = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            model.PostcodeData = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            model.FilePreparationDate = message?.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString("dd/MM/yyyy");
            //Todo: 
            //model.DevolvedPostcodesData
            return model;
        }

        private List<ValidationErrorMessageModel> GetValidationErrorMessageModels(List<ValidationError> validationErrors, IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrorsMetadata)
        {
            return (from errors in validationErrors
                    group errors by errors.RuleName
                into validationErrorGroup
                    select new ValidationErrorMessageModel()
                    {
                        RuleName = validationErrorGroup.Key,
                        Message = validationErrorsMetadata.FirstOrDefault(x =>
                                string.Equals(x.RuleName, validationErrorGroup.Key, StringComparison.OrdinalIgnoreCase))
                            ?.Message,
                        Occurrences = validationErrorGroup.Count()
                    }).ToList();
        }

        private int GetLearningDeliveriesFundModelCount(List<ILooseLearningDelivery> learningDeliveries, int fundModel)
        {
            return learningDeliveries.Where(x => x.FundModelNullable != null).Count(x => x.FundModelNullable.Value == fundModel);
        }

        private int GetFundModelCount(List<ILooseLearner> learners, int fundModel)
        {
            return learners.Where(x => x.LearningDeliveries != null)
                .Where(x => x.LearningDeliveries.Any(y => y.FundModelNullable.GetValueOrDefault() == fundModel))
                .DistinctByCount(x => x.LearnRefNumber);
        }
    }
}
