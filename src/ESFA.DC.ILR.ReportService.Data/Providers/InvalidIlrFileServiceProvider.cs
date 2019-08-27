using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model.Loose;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class InvalidIlrFileServiceProvider : IExternalDataProvider
    {
        private readonly IFileService _fileService;
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public InvalidIlrFileServiceProvider(IFileService fileService, IXmlSerializationService xmlSerializationService, IJsonSerializationService jsonSerializationService)
        {
            _fileService = fileService;
            _xmlSerializationService = xmlSerializationService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var message = await ProvideXmlAsync<Message>(reportServiceContext.OriginalFilename, reportServiceContext.Container, cancellationToken) as Message;
            var validationErrors = await ProvideJsonAsync<List<ValidationError>>(reportServiceContext.ValidationErrorsKey, reportServiceContext.Container, cancellationToken) as List<ValidationError>;

            if (message == null) { return message; }

            var learnRefNumbers = validationErrors?.Select(x => x.LearnerReferenceNumber) ?? Enumerable.Empty<string>();

            message.Learner = message.Learner?.Where(l => learnRefNumbers.Contains(l.LearnRefNumber)).ToArray();
            message.LearnerDestinationandProgression = message.LearnerDestinationandProgression?.Where(l => learnRefNumbers.Contains(l.LearnRefNumber)).ToArray();

            return message;

        }

        private async Task<object> ProvideXmlAsync<T>(string fileName, string container, CancellationToken cancellationToken)
        {
            using (var fileStream = await _fileService.OpenReadStreamAsync(fileName, container, cancellationToken))
            {
                return _xmlSerializationService.Deserialize<T>(fileStream);
            }
        }

        private async Task<object> ProvideJsonAsync<T>(string fileName, string container, CancellationToken cancellationToken)
        {
            using (var fileStream = await _fileService.OpenReadStreamAsync(fileName, container, cancellationToken))
            {
                return _jsonSerializationService.Deserialize<T>(fileStream);
            }
        }
    }
}
