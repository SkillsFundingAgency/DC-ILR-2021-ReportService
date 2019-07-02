using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Interface
{
    public interface IValidationErrorsReportBuilder
    {
        IEnumerable<ValidationErrorModel> Build(
            IEnumerable<ValidationError> ilrValidationErrors,
            IMessage message,
            IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrorsMetadata);
    }
}
