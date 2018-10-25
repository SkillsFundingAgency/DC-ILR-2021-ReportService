using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class EasProviderService : IEasProviderService
    {
        private readonly ILogger _logger;

        private readonly EasConfiguration _easConfiguration;
        private readonly Dictionary<int, DateTime> _loadedLastEasUpdate;
        private readonly SemaphoreSlim _getLastEastUpdateLock;

        public EasProviderService(ILogger logger, EasConfiguration easConfiguration)
        {
            _logger = logger;
            _easConfiguration = easConfiguration;
            _loadedLastEasUpdate = new Dictionary<int, DateTime>();

            _getLastEastUpdateLock = new SemaphoreSlim(1, 1);
        }

        public async Task<DateTime> GetLastEasUpdate(int ukprn, CancellationToken cancellationToken)
        {
            await _getLastEastUpdateLock.WaitAsync(cancellationToken);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!_loadedLastEasUpdate.ContainsKey(ukprn))
                {
                    _loadedLastEasUpdate[ukprn] = DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get LARS learning deliveries", ex);
            }
            finally
            {
                _getLastEastUpdateLock.Release();
            }

            return _loadedLastEasUpdate[ukprn];
        }
    }
}
