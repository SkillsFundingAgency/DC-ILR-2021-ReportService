using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Builders
{
    public interface IValidationSchemaErrorsReportBuilder
    {
        List<ValidationErrorModel> Build(List<ValidationError> ilrValidationErrors);
    }
}
