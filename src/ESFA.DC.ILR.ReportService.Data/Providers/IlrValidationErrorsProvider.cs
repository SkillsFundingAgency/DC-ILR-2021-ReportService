using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class IlrValidationErrorsProvider : IExternalDataProvider
    {
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public IlrValidationErrorsProvider(IFileService fileService, IJsonSerializationService jsonSerializationService)
        {
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
        }

       public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            List<ValidationError> message;

            using (var fileStream = await _fileService.OpenReadStreamAsync(reportServiceContext.ValidationErrorsKey, reportServiceContext.Container, cancellationToken))
            {
                message = _jsonSerializationService.Deserialize<List<ValidationError>>(fileStream);
            }

            return message;
        }
    }
}
