using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider.Abstract
{
    public abstract class AbstractFundModelProviderService
    {
        protected readonly ILogger _logger;

        protected readonly ISerializationService _serializationService;

        private readonly IFileService _fileService;

        protected AbstractFundModelProviderService(IFileService fileService, ISerializationService serializationService, ILogger logger)
        {
            _fileService = fileService;
            _serializationService = serializationService;
            _logger = logger;
        }

        protected async Task<TModel> Provide<TModel>(string fileReference, string container, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"File Service Starting Provide Container : {container} FileReference : {fileReference}");

            using (var stream = await _fileService.OpenReadStreamAsync(fileReference, container, cancellationToken))
            {
                var model = _serializationService.Deserialize<TModel>(stream);

                _logger.LogInfo($"File Service Starting Provide Container : {container} FileReference : {fileReference}");

                return model;
            }
        }
    }
}
