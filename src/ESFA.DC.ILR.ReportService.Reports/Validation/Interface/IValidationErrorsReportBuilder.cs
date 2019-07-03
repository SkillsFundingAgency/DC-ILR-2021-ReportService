using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Interface
{
    public interface IValidationErrorsReportBuilder
    {
        IEnumerable<ValidationErrorRow> Build(
            IEnumerable<ValidationError> ilrValidationErrors,
            IMessage message,
            IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrorsMetadata);
    }
}
