using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public sealed class InvalidLearnersService : BaseLearnRefNumbersProvider, IInvalidLearnersService
    {
        public InvalidLearnersService(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IJsonSerializationService jsonSerializationService,
            IReportServiceConfiguration reportServiceConfiguration)
        : base("InvalidLearnRefNumbers", logger, storage, jsonSerializationService, reportServiceConfiguration)
        {
        }
    }
}
