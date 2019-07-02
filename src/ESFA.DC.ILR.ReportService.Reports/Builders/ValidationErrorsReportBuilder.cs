using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Comparer;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Builders
{
    public class ValidationErrorsReportBuilder : IValidationErrorsReportBuilder
    {
        private static readonly ValidationErrorsModelComparer ValidationErrorsModelComparer = new ValidationErrorsModelComparer();

        public IEnumerable<ValidationErrorModel> Build(
            IEnumerable<ValidationError> ilrValidationErrors,
            IMessage message,
            IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrorsMetadata)
        {
            List<ValidationErrorModel> validationErrorModels = new List<ValidationErrorModel>();

            foreach (ValidationError validationError in ilrValidationErrors)
            {
                ILearner learner = message.Learners?.FirstOrDefault(x => x.LearnRefNumber == validationError.LearnerReferenceNumber);
                ILearningDelivery learningDelivery = learner?.LearningDeliveries?.FirstOrDefault(x => x.AimSeqNumber == validationError.AimSequenceNumber);

                validationErrorModels.Add(new ValidationErrorModel()
                {
                    AimSequenceNumber = validationError.AimSequenceNumber,
                    ErrorMessage = validationErrorsMetadata.FirstOrDefault(x => string.Equals(x.RuleName, validationError.RuleName, StringComparison.OrdinalIgnoreCase))?.Message,
                    FieldValues = validationError.ValidationErrorParameters == null
                        ? string.Empty
                        : GetValidationErrorParameters(validationError.ValidationErrorParameters.ToList()),
                    FundModel = learningDelivery?.FundModel,
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
