using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Interface
{
    public interface IValidationSchemaErrorsReportBuilder
    {
        IEnumerable<ValidationErrorRow> Build(IEnumerable<ValidationError> ilrValidationErrors);
    }
}
