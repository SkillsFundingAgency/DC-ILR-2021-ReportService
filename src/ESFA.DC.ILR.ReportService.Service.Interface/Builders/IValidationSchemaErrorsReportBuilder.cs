using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Builders
{
    public interface IValidationSchemaErrorsReportBuilder
    {
        IEnumerable<ValidationErrorModel> Build(IEnumerable<ValidationError> ilrValidationErrors);
    }
}
