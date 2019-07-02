using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Interface
{
    public interface IFrontEndValidationReport
    {
        Task GenerateAsync(IReportServiceContext reportServiceContext, IEnumerable<ValidationErrorRow> validationErrors, CancellationToken cancellationToken);
    }
}
