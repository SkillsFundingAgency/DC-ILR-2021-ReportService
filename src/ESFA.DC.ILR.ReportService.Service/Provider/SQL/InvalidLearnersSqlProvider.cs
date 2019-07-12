using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider.SQL
{
    public sealed class InvalidLearnersSqlProvider : BaseLearnRefNumbersSqlProvider, IInvalidLearnersService
    {
        public InvalidLearnersSqlProvider(
            ILogger logger,
            IReportServiceConfiguration reportServiceConfiguration)
        : base("InvalidLearnRefNumbers", logger, reportServiceConfiguration)
        {
        }
    }
}
