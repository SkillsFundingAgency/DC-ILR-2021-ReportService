using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Postcodes.Model;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider.SQL
{
    public sealed class PostcodeSqlProvider : IPostcodeProviderService
    {
        private readonly ILogger _logger;
        private readonly IReportServiceConfiguration _reportServiceConfiguration;

        private readonly SemaphoreSlim _getVersionLock;

        private string _version;

        public PostcodeSqlProvider(ILogger logger, IReportServiceConfiguration reportServiceConfiguration)
        {
            _logger = logger;
            _reportServiceConfiguration = reportServiceConfiguration;
            _version = null;
            _getVersionLock = new SemaphoreSlim(1, 1);
        }

        public async Task<string> GetVersionAsync(CancellationToken cancellationToken)
        {
            await _getVersionLock.WaitAsync(cancellationToken);

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(_version))
                {
                    IPostcodes postcodesContext = new Postcodes(_reportServiceConfiguration.PostcodeConnectionString);
                    _version = (await postcodesContext.VersionInfos.SingleOrDefaultAsync(cancellationToken))?.VersionNumber ?? "NA";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get postcode version information", ex);
            }
            finally
            {
                _getVersionLock.Release();
            }

            return _version;
        }
    }
}
