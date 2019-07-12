using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider.SQL
{
    public sealed class ValidLearnersSqlProvider : BaseLearnRefNumbersSqlProvider, IValidLearnersService
    {
        public ValidLearnersSqlProvider(
            ILogger logger,
            IReportServiceConfiguration reportServiceConfiguration)
        : base("ValidLearnRefNumbers", logger, reportServiceConfiguration)
        {
        }
    }
}
