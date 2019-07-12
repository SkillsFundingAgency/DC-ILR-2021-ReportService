using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public sealed class InvalidLearnRefNumbersProvider : BaseLearnRefNumbersProvider, IInvalidLearnersService
    {
        public InvalidLearnRefNumbersProvider(
            ILogger logger,
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            IReportServiceConfiguration reportServiceConfiguration)
        : base("InvalidLearnRefNumbers", logger, fileService, jsonSerializationService, reportServiceConfiguration)
        {
        }
    }
}
