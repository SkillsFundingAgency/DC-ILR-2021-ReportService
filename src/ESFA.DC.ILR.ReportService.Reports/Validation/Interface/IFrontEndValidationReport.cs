using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Interface
{
    public interface IFrontEndValidationReport
    {
        Task GenerateAsync(IReportServiceContext reportServiceContext,
            IEnumerable<ValidationErrorDto> validationErrorDtos,
            string externalFileName,
            CancellationToken cancellationToken)
    }
}
