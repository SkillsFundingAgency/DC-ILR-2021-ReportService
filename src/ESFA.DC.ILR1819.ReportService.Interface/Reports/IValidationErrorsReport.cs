using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Reports
{
    public interface IValidationErrorsReport
    {
        Task<List<ValidationErrorDto>> ReadAndDeserialiseValidationErrorsAsync(IJobContextMessage jobContextMessage);

        Task PeristValuesToStorage(IJobContextMessage jobContextMessage, List<ValidationErrorDto> validationErrorDtos);
    }
}
