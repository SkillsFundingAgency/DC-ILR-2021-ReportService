using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Detail
{
    public class ValidationErrorsDetailReportBuilder : IValidationErrorsReportBuilder
    {
        private static readonly ValidationErrorsModelComparer ValidationErrorsModelComparer = new ValidationErrorsModelComparer();

        public IEnumerable<ValidationErrorRow> Build(
            IEnumerable<ValidationError> ilrValidationErrors,
            ILooseMessage message,
            IReadOnlyCollection<Models.ReferenceData.MetaData.ValidationError> validationErrorsMetadata)
        {
            List<ValidationErrorRow> validationErrorModels = new List<ValidationErrorRow>();

            foreach (ValidationError validationError in ilrValidationErrors)
            {
                ILooseLearner learner = message.Learners?.FirstOrDefault(x => x.LearnRefNumber == validationError.LearnerReferenceNumber);
                ILooseLearningDelivery learningDelivery = learner?.LearningDeliveries?.FirstOrDefault(x => x.AimSeqNumberNullable == validationError.AimSequenceNumber);

                validationErrorModels.Add(new ValidationErrorRow()
                {
                    AimSequenceNumber = validationError.AimSequenceNumber,
                    ErrorMessage = validationErrorsMetadata.FirstOrDefault(x => string.Equals(x.RuleName, validationError.RuleName, StringComparison.OrdinalIgnoreCase))?.Message,
                    FieldValues = validationError.ValidationErrorParameters == null
                        ? string.Empty
                        : GetValidationErrorParameters(validationError.ValidationErrorParameters.ToList()),
                    FundModel = learningDelivery?.FundModelNullable,
                    LearnAimRef = learningDelivery?.LearnAimRef,
                    LearnerReferenceNumber = validationError.LearnerReferenceNumber,
                    PartnerUKPRN = learningDelivery?.PartnerUKPRNNullable,
                    ProviderSpecDelOccurA = learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                    ProviderSpecDelOccurB = learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                    ProviderSpecDelOccurC = learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "C", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                    ProviderSpecDelOccurD = learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "D", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                    ProviderSpecLearnOccurA = learner?.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecLearnMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                    ProviderSpecLearnOccurB = learner?.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecLearnMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                    RuleName = validationError.RuleName,
                    SWSupAimId = learningDelivery?.SWSupAimId,
                    Severity = validationError.Severity
                });
            }

            validationErrorModels.Sort(ValidationErrorsModelComparer);
            return validationErrorModels;
        }

        private string GetValidationErrorParameters(List<ValidationErrorParameter> validationErrorParameters)
        {
            StringBuilder result = new StringBuilder();
            validationErrorParameters.ForEach(x =>
            {
                result.Append($"{x.PropertyName}={x.Value}|");
            });

            return result.ToString();
        }
    }
}
