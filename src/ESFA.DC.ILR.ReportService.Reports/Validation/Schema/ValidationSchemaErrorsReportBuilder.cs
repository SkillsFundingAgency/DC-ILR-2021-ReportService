using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Schema
{
    public class ValidationSchemaErrorsReportBuilder : IValidationSchemaErrorsReportBuilder
    {
        public IEnumerable<ValidationErrorRow> Build(IEnumerable<ValidationError> ilrValidationErrors)
        {
            return ilrValidationErrors
                .Select(e => new ValidationErrorRow()
                {
                    AimSequenceNumber = e.AimSequenceNumber,
                    ErrorMessage = string.Empty,
                    FieldValues = e.ValidationErrorParameters == null
                        ? string.Empty
                        : GetValidationErrorParameters(e.ValidationErrorParameters),
                    LearnerReferenceNumber = e.LearnerReferenceNumber,
                    RuleName = e.RuleName,
                    Severity = e.Severity
                })
                .OrderBy(e => e.Severity)
                .ThenBy(e => e.RuleName);
        }

        private string GetValidationErrorParameters(IEnumerable<ValidationErrorParameter> validationErrorParameters)
        {
            StringBuilder result = new StringBuilder();

            foreach (var parameter in validationErrorParameters)
            {
                result.Append($"{parameter.PropertyName}={parameter.Value}|");
            }
          
            return result.ToString();
        }
    }
}
