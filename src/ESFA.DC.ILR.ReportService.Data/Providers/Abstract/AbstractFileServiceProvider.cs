using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers.Abstract
{
    public abstract class AbstractFileServiceProvider
    {
        private readonly IFileService _fileService;
        private readonly ISerializationService _serializationService;

        protected AbstractFileServiceProvider(IFileService fileService, ISerializationService serializationService)
        {
            _fileService = fileService;
            _serializationService = serializationService;
        }

        protected async Task<object> ProvideAsync<T>(string fileName, string container, CancellationToken cancellationToken)
        {
            using (var fileStream = await _fileService.OpenReadStreamAsync(fileName, container, cancellationToken))
            {
                return _serializationService.Deserialize<T>(fileStream);
            }
        }
    }
}
