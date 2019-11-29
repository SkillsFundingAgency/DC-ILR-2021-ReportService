using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Detail
{
    public class ValidationErrorsDetailReportBuilder : IValidationErrorsReportBuilder
    {
        public IEnumerable<ValidationErrorRow> Build(
            IEnumerable<ValidationError> ilrValidationErrors,
            ILooseMessage message,
            IReadOnlyCollection<Models.ReferenceData.MetaData.ValidationError> validationErrorsMetadata)
        {
            List<ValidationErrorRow> validationErrorModels = new List<ValidationErrorRow>();

            var learnerDictionary = BuildLearnerDictionary(message);

            foreach (ValidationError validationError in ilrValidationErrors)
            {
                ILooseLearner learner = learnerDictionary?.GetValueOrDefault(validationError.LearnerReferenceNumber?.Trim());
                ILooseLearningDelivery learningDelivery = learner?.LearningDeliveries?.FirstOrDefault(x => x.AimSeqNumberNullable == validationError.AimSequenceNumber);

                validationErrorModels.Add(new ValidationErrorRow()
                {
                    AimSequenceNumber = validationError.AimSequenceNumber,
                    ErrorMessage = validationErrorsMetadata.FirstOrDefault(x => x.RuleName.CaseInsensitiveEquals(validationError.RuleName))?.Message,
                    FieldValues = validationError.ValidationErrorParameters == null
                        ? string.Empty
                        : GetValidationErrorParameters(validationError.ValidationErrorParameters),
                    FundModel = learningDelivery?.FundModelNullable,
                    LearnAimRef = learningDelivery?.LearnAimRef,
                    LearnerReferenceNumber = validationError.LearnerReferenceNumber,
                    PartnerUKPRN = learningDelivery?.PartnerUKPRNNullable,
                    ProviderSpecDelOccurA = learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur.CaseInsensitiveEquals("A"))?.ProvSpecDelMon,
                    ProviderSpecDelOccurB = learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur.CaseInsensitiveEquals("B"))?.ProvSpecDelMon,
                    ProviderSpecDelOccurC = learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur.CaseInsensitiveEquals("C"))?.ProvSpecDelMon,
                    ProviderSpecDelOccurD = learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur.CaseInsensitiveEquals("D"))?.ProvSpecDelMon,
                    ProviderSpecLearnOccurA = learner?.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => x.ProvSpecLearnMonOccur.CaseInsensitiveEquals("A"))?.ProvSpecLearnMon,
                    ProviderSpecLearnOccurB = learner?.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => x.ProvSpecLearnMonOccur.CaseInsensitiveEquals("B"))?.ProvSpecLearnMon,
                    RuleName = validationError.RuleName,
                    SWSupAimId = learningDelivery?.SWSupAimId,
                    Severity = validationError.Severity
                });
            }

            return validationErrorModels
                .OrderBy(e => e.Severity)
                .ThenBy(e => e.RuleName);
        }
        
        public IDictionary<string, ILooseLearner> BuildLearnerDictionary(ILooseMessage message)
        {
            return message?
                .Learners?
                .Where(l => l.LearnRefNumber != null)
                .GroupBy(l => l.LearnRefNumber.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(l => l.Key, l => l.FirstOrDefault(), StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, ILooseLearner>();
        }

        private string GetValidationErrorParameters(IEnumerable<ValidationErrorParameter> validationErrorParameters)
        {
            StringBuilder result = new StringBuilder();

            foreach (var validationErrorParameter in validationErrorParameters)
            {
                result.Append($"{validationErrorParameter.PropertyName}={validationErrorParameter.Value}|");
            }

            return result.ToString();
        }
    }
}
