using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Helper
{
    public class IlrFileHelper : IIlrFileHelper
    {
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly ILogger _logger;

        private string _ilrFileName;
        private string _ilrFileDateTime;

        public IlrFileHelper(IStringUtilitiesService stringUtilitiesService, ILogger logger)
        {
            _stringUtilitiesService = stringUtilitiesService;
            _logger = logger;
        }

        public bool CheckIlrFileNameIsValid(IReportServiceContext reportServiceContext)
        {
            _ilrFileName = reportServiceContext.Filename;
            _ilrFileDateTime = _stringUtilitiesService.GetIlrFileDate(_ilrFileName)?.ToString("yyyy-MM-dd HH:mm:ssy");

            if (_ilrFileDateTime != null)
            {
                return true;
            }

            _logger.LogError("ILR file name contains invalid date time format");
            return false;
        }
    }
}
