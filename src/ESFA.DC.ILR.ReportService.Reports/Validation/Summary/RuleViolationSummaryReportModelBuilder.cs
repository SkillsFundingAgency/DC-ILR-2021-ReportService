using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Validation.Summary.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary
{
    public class RuleViolationSummaryReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<RuleViolationSummaryReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private const string Error = "E";
        private const string Warning = "W";

        public RuleViolationSummaryReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public RuleViolationSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            ILooseMessage looseMessage = reportServiceDependentData.Get<ILooseMessage>();
            ReferenceDataRoot referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();
            List<ValidationError> validationErrors = reportServiceDependentData.Get<List<ValidationError>>();

            var validationErrorMessageDictionary = referenceDataRoot.MetaDatas.ValidationErrors.ToDictionary(ve => ve.RuleName, ve => ve.Message, StringComparer.OrdinalIgnoreCase);
            var model = new RuleViolationSummaryReportModel();
            var organisation = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn);
            var organisationName = organisation?.Name ?? string.Empty;

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            var reportGeneratedAt = "Report generated at: " + FormatReportGeneratedAtDateTime(dateTimeNowUk);
            var looseLearners = looseMessage?.Learners?.ToList() ?? new List<ILooseLearner>();
            var looseLearnerDestinationAndProgressions = looseMessage?.LearnerDestinationAndProgressions?.ToList() ?? new List<ILooseLearnerDestinationAndProgression>();

            // Header
            model.ProviderName = $"Provider: {organisationName}";
            model.Ukprn = $"UKPRN: {reportServiceContext.Ukprn}";
            model.IlrFile =$"ILR File: {ExtractFileName(reportServiceContext.IlrReportingFilename)}";
            model.Year = ReportingConstants.Year;

            // body
            var validationErrorsList = validationErrors.Where(x => x.Severity.CaseInsensitiveEquals(Error)).ToList();
            var validationErrorWarningsList = validationErrors.Where(x => x.Severity.CaseInsensitiveEquals(Warning)).ToList();
            
            var learnRefNumbersWithErrors = new HashSet<string>(validationErrorsList.Select(x => x.LearnerReferenceNumber), StringComparer.OrdinalIgnoreCase);
            var learnRefNumbersWithWarnings = new HashSet<string>(validationErrorWarningsList.Select(x => x.LearnerReferenceNumber), StringComparer.OrdinalIgnoreCase);

            var validLearners = looseLearners.Where(l => !learnRefNumbersWithErrors.Contains(l.LearnRefNumber)).ToList();
            var invalidLearners = looseLearners.Where(l => learnRefNumbersWithErrors.Contains(l.LearnRefNumber)).ToList();

            var validLearnerDestinationAndProgressions = looseLearnerDestinationAndProgressions.Where(ldp => !learnRefNumbersWithErrors.Contains(ldp.LearnRefNumber)).ToList();
            var invalidLearnerDestinationAndProgressions = looseLearnerDestinationAndProgressions.Where(ldp => learnRefNumbersWithErrors.Contains(ldp.LearnRefNumber)).ToList();
            
            model.TotalNoOfErrors = validationErrorsList.Count;
            model.TotalNoOfWarnings = validationErrorWarningsList.Count;

            model.TotalNoOfLearners = looseLearners.DistinctByCount(x => x.LearnRefNumber);
            model.TotalNoOfLearnersWithWarnings = learnRefNumbersWithWarnings.Count(l => !learnRefNumbersWithErrors.Contains(l));
            
            var learningDeliveries = looseLearners.Where(x => x.LearningDeliveries != null).SelectMany(x => x.LearningDeliveries).ToList();

            model.FullyValidLearners = new RuleViolationsTotalModel
            {
                Total = validLearners.DistinctByCount(x => x.LearnRefNumber),
                Apprenticeships = LearnersWithFundModelLearningDeliveryFundModelCount(validLearners, FundModelConstants.FM36),
                Funded1619 = LearnersWithFundModelLearningDeliveryFundModelCount(validLearners, FundModelConstants.FM25),
                AdultSkilledFunded = LearnersWithFundModelLearningDeliveryFundModelCount(validLearners, FundModelConstants.FM35),
                CommunityLearningFunded = LearnersWithFundModelLearningDeliveryFundModelCount(validLearners, FundModelConstants.FM10),
                ESFFunded = LearnersWithFundModelLearningDeliveryFundModelCount(validLearners, FundModelConstants.FM70),
                OtherAdultFunded = LearnersWithFundModelLearningDeliveryFundModelCount(validLearners, FundModelConstants.FM81),
                Other1619Funded = LearnersWithFundModelLearningDeliveryFundModelCount(validLearners, FundModelConstants.FM82),
                NonFunded = LearnersWithFundModelLearningDeliveryFundModelCount(validLearners, FundModelConstants.FM99)
            };

            model.InvalidLearners = new RuleViolationsTotalModel
            {
                Total = invalidLearners.DistinctByCount(x => x.LearnRefNumber),
                Apprenticeships = LearnersWithFundModelLearningDeliveryFundModelCount(invalidLearners, FundModelConstants.FM36),
                Funded1619 = LearnersWithFundModelLearningDeliveryFundModelCount(invalidLearners, FundModelConstants.FM25),
                AdultSkilledFunded = LearnersWithFundModelLearningDeliveryFundModelCount(invalidLearners, FundModelConstants.FM35),
                CommunityLearningFunded = LearnersWithFundModelLearningDeliveryFundModelCount(invalidLearners, FundModelConstants.FM10),
                ESFFunded = LearnersWithFundModelLearningDeliveryFundModelCount(invalidLearners, FundModelConstants.FM70),
                OtherAdultFunded = LearnersWithFundModelLearningDeliveryFundModelCount(invalidLearners, FundModelConstants.FM81),
                Other1619Funded = LearnersWithFundModelLearningDeliveryFundModelCount(invalidLearners, FundModelConstants.FM82),
                NonFunded = LearnersWithFundModelLearningDeliveryFundModelCount(invalidLearners, FundModelConstants.FM99)
            };

            model.LearningDeliveries = new RuleViolationsTotalModel
            {
                Total = learningDeliveries.Count,
                Apprenticeships = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM36),
                Funded1619 = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM25),
                AdultSkilledFunded = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM35),
                CommunityLearningFunded = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM10),
                ESFFunded = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM70),
                OtherAdultFunded = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM81),
                Other1619Funded = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM82),
                NonFunded = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM99),
                AdvancedLoanLearningDeliveries = GetLearningDeliveriesFundModelWithFAMCount(learningDeliveries, FundModelConstants.FM99, LearningDeliveryFAMTypeConstants.ADL),
            };
            
            model.LearnerDestinationProgressionSummary = new LearnerDestinationProgressionSummary()
            {
                Total = looseLearnerDestinationAndProgressions.Count,
                ValidLearnerDestinationProgressions = validLearnerDestinationAndProgressions.Count,
                InValidLearnerDestinationProgressions = invalidLearnerDestinationAndProgressions.Count,
            };

            model.Errors = GetValidationErrorMessageModels(validationErrorsList, validationErrorMessageDictionary);
            model.Warnings = GetValidationErrorMessageModels(validationErrorWarningsList, validationErrorMessageDictionary);

            // Footer
            model.ReportGeneratedAt = reportGeneratedAt;
            model.ApplicationVersion = reportServiceContext.ServiceReleaseVersion;
            model.OrganisationData = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            model.LargeEmployerData = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            model.LarsData = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            model.PostcodeData = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            model.FilePreparationDate = FormatFilePreparationDate(looseMessage?.HeaderEntity.CollectionDetailsEntity.FilePreparationDate);
            model.CampusIdData = referenceDataRoot.MetaDatas.ReferenceDataVersions.CampusIdentifierVersion.Version;
            model.DevolvedPostcodesData = referenceDataRoot.MetaDatas.ReferenceDataVersions.DevolvedPostcodesVersion?.Version;
            return model;
        }

        private List<ValidationErrorMessageModel> GetValidationErrorMessageModels(IEnumerable<ValidationError> validationErrors, IDictionary<string, string> validationErrorMessageDictionary)
        {
            return validationErrors
                .GroupBy(ve => ve.RuleName, StringComparer.OrdinalIgnoreCase)
                .Select(g => new ValidationErrorMessageModel()
                {
                    RuleName = g.Key,
                    Message = validationErrorMessageDictionary.GetValueOrDefault(g.Key),
                    Occurrences = g.Count()
                })
                .OrderBy(ve => ve.RuleName)
                .ToList();
        }

        private int GetLearningDeliveriesFundModelWithFAMCount(IEnumerable<ILooseLearningDelivery> learningDeliveries, int fundModel, string famType = null)
        {
            var query = learningDeliveries.Where(x => x.FundModelNullable == fundModel);

            if (famType != null)
            {
                query = query.Where(x => x.LearningDeliveryFAMs != null && x.LearningDeliveryFAMs.Any(y => y.LearnDelFAMType.CaseInsensitiveEquals(famType)));
            }

            return query.Count();
        }

        private bool HasLearningDeliveryWithFundModel(ILooseLearner learner, int fundModel)
        {
            return learner.LearningDeliveries != null
                   && learner.LearningDeliveries.Any(ld => ld.FundModelNullable == fundModel);
        }

        private int LearnersWithFundModelLearningDeliveryFundModelCount(IEnumerable<ILooseLearner> learners, int fundModel)
        {
            return learners.Where(l => HasLearningDeliveryWithFundModel(l, fundModel))
                .DistinctByCount(x => x.LearnRefNumber);
        }
    }
}
