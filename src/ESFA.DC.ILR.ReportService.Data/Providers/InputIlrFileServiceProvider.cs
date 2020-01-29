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
    public class InputIlrFileServiceProvider : IExternalDataProvider
    {
        private readonly IFileService _fileService;
        private readonly IXmlSerializationService _xmlSerializationService;

        public InputIlrFileServiceProvider(IFileService fileService, IXmlSerializationService xmlSerializationService)
        {
            _fileService = fileService;
            _xmlSerializationService = xmlSerializationService;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            return await ProvideXmlAsync<Message>(reportServiceContext.IlrReportingFilename, reportServiceContext.Container, cancellationToken) as Message;
        }

        private async Task<object> ProvideXmlAsync<T>(string fileName, string container, CancellationToken cancellationToken)
        {
            using (var fileStream = await _fileService.OpenReadStreamAsync(fileName, container, cancellationToken))
            {
                return _xmlSerializationService.Deserialize<T>(fileStream);
            }
        }
    }
}
