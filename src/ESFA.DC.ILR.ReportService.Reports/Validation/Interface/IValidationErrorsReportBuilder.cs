using System.Collections.Generic;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Interface
{
    public interface IValidationErrorsReportBuilder
    {
        IEnumerable<ValidationErrorRow> Build(
            IEnumerable<ValidationError> ilrValidationErrors,
            ILooseMessage message,
            IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrorsMetadata);
    }
}
