using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.DAS.Model;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.DasCommitments;
using ESFA.DC.ILR1819.ReportService.Service.Comparer;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class DasCommitmentsService : IDasCommitmentsService
    {
        private static readonly IEqualityComparer<DasCommitments> Comparer = new DasCommitmentsComparer();

        private readonly DasCommitmentsConfiguration _dasCommitmentsConfiguration;
        private readonly IDasCommitmentBuilder _dasCommitmentBuilder;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private List<DasCommitment> _dasCommitments;

        public DasCommitmentsService(
            DasCommitmentsConfiguration dasCommitmentsConfiguration,
            IDasCommitmentBuilder dasCommitmentBuilder,
            ILogger logger)
        {
            _dasCommitmentsConfiguration = dasCommitmentsConfiguration;
            _dasCommitmentBuilder = dasCommitmentBuilder;
            _logger = logger;

            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<List<DasCommitment>> GetCommitments(
            long ukPrn,
            List<long> ulns,
            CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_loadedDataAlready)
                {
                    return _dasCommitments;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                _loadedDataAlready = true;

                var dasCommitments = new List<DasCommitments>();

                DbContextOptions<DAS_commitmentsRefContext> options = new DbContextOptionsBuilder<DAS_commitmentsRefContext>().UseSqlServer(_dasCommitmentsConfiguration.DasCommitmentsConnectionString).Options;

                using (DAS_commitmentsRefContext dasCommitmentsRefContext = new DAS_commitmentsRefContext(options))
                {
                    IEnumerable<long> ulnsPage = ulns.Take(1000);
                    int position = 1000;

                    DasCommitments[] results = await dasCommitmentsRefContext.DasCommitments
                        .Where(x => x.Ukprn == ukPrn || ulnsPage.Contains(x.Uln))
                        .ToArrayAsync(cancellationToken);
                    dasCommitments.AddRange(results);

                    while (position < ulns.Count)
                    {
                        ulnsPage = ulns.Skip(position).Take(1000);

                        results = await dasCommitmentsRefContext.DasCommitments
                            .Where(x => ulnsPage.Contains(x.Uln))
                            .ToArrayAsync(cancellationToken);
                        dasCommitments.AddRange(results);

                        position += 1000;
                    }

                    _dasCommitments = _dasCommitmentBuilder.Build(dasCommitments.Distinct(Comparer));
                }
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get DAS Commitments data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _dasCommitments;
        }
    }
}
