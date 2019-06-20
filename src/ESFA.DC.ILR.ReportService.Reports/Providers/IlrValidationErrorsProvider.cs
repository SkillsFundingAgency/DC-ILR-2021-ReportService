using System.Collections.Generic;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.Serialization.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Providers
{
    public class IlrValidationErrorsProvider : IFileProviderService<List<ValidationError>>
    {
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public IlrValidationErrorsProvider(IFileService fileService, IJsonSerializationService jsonSerializationService)
        {
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<List<ValidationError>> ProvideAsync(IReportServiceContext fundingServiceContext, CancellationToken cancellationToken)
        {
            List<ValidationError> message;

            using (var fileStream = await _fileService.OpenReadStreamAsync(fundingServiceContext.ValidationErrorsKey, fundingServiceContext.Container, cancellationToken))
            {
                message = _jsonSerializationService.Deserialize<List<ValidationError>>(fileStream);
            }

            return message;
        }
    }
}
