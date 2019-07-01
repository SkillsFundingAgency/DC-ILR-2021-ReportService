using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.Serialization.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Reports.Providers
{
    public class IlrFileServiceProvider : IExternalDataProvider
    {
        private readonly IFileService _fileService;
        private readonly IXmlSerializationService _xmlSerializationService;

        public IlrFileServiceProvider(IFileService fileService, IXmlSerializationService xmlSerializationService)
        {
            _fileService = fileService;
            _xmlSerializationService = xmlSerializationService;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            Message message;

            using (var fileStream = await _fileService.OpenReadStreamAsync(reportServiceContext.Filename, reportServiceContext.Container, cancellationToken))
            {
                message = _xmlSerializationService.Deserialize<Message>(fileStream);
            }

            return message;
        }
    }
}
