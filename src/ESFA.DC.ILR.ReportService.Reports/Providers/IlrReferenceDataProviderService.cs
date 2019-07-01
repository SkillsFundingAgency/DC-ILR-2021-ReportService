using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Reports.Providers
{
    public class IlrReferenceDataProviderService : IExternalDataProvider
    {
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public IlrReferenceDataProviderService(IFileService fileService, IJsonSerializationService jsonSerializationService)
        {
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            ReferenceDataRoot referenceData;

            using (var fileStream = await _fileService.OpenReadStreamAsync(reportServiceContext.IlrReferenceDataKey, reportServiceContext.Container, cancellationToken))
            {
                referenceData = _jsonSerializationService.Deserialize<ReferenceDataRoot>(fileStream);
            }

            return referenceData;
        }
    }
}
