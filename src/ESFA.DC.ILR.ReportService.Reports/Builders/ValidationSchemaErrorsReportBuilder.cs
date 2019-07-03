using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Comparer;
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Builders
{
    public class ValidationSchemaErrorsReportBuilder : IValidationSchemaErrorsReportBuilder
    {
        private static readonly ValidationErrorsModelComparer ValidationErrorsModelComparer = new ValidationErrorsModelComparer();

        public List<ValidationErrorModel> Build(List<ValidationError> ilrValidationErrors)
        {
            List<ValidationErrorModel> validationErrorModels = new List<ValidationErrorModel>();

            foreach (ValidationError validationError in ilrValidationErrors)
            {
                validationErrorModels.Add(new ValidationErrorModel()
                {
                    AimSequenceNumber = validationError.AimSequenceNumber,
                    ErrorMessage = string.Empty,
                    FieldValues = validationError.ValidationErrorParameters == null
                        ? string.Empty
                        : GetValidationErrorParameters(validationError.ValidationErrorParameters.ToList()),
                    LearnerReferenceNumber = validationError.LearnerReferenceNumber,
                      RuleName = validationError.RuleName,
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
