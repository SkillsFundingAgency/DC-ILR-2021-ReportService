using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider.Abstract
{
    public abstract class AbstractFundModelProviderService
    {
        protected readonly ILogger _logger;

        protected readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;

        protected readonly ISerializationService _serializationService;

        protected AbstractFundModelProviderService(IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService, ISerializationService serializationService, ILogger logger)
        {
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            _serializationService = serializationService;
            _logger = logger;
        }
    }
}
