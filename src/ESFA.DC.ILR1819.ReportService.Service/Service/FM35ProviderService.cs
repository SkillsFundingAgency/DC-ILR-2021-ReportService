using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interfaces;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Remotion.Linq.Clauses;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class FM35ProviderService : IFM35ProviderService
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly ILRConfiguration _ilrConfiguration;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private FM35Global _fundingOutputs;

        public FM35ProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)]
            IKeyValuePersistenceService redis,
            [KeyFilter(PersistenceStorageKeys.Blob)]
            IKeyValuePersistenceService blob,
            IJsonSerializationService jsonSerializationService,
            ILRConfiguration ilrConfiguration)
        {
            _logger = logger;
            _redis = redis;
            _blob = blob;
            _jsonSerializationService = jsonSerializationService;
            _ilrConfiguration = ilrConfiguration;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<FM35Global> GetFM35Data(
            IJobContextMessage jobContextMessage,
            CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_loadedDataAlready)
                {
                    return _fundingOutputs;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                _loadedDataAlready = true;
                string fm35Filename =
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output].ToString();
                string fm35 = await _redis.GetAsync(fm35Filename, cancellationToken);

                if (string.IsNullOrEmpty(fm35))
                {
                    _fundingOutputs = null;
                    return _fundingOutputs;
                }

                // await _blob.SaveAsync($"{jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]}_{jobContextMessage.JobId.ToString()}_Fm35.json", fm35, cancellationToken);
                _fundingOutputs = _jsonSerializationService.Deserialize<FM35Global>(fm35);
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise FM35 funding data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }

        public async Task<List<FM35LearningDeliveryValues>> GetFM35AdultFundingLineDataFromDataStore(
            IJobContextMessage jobContextMessage,
            CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);
            var fm35LearningDeliveryPeriodisedValues = new List<FM35LearningDeliveryValues>();
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                var UkPrn = int.Parse(jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString());

                using (var _ilrContext = new ILR1819_DataStoreEntities(_ilrConfiguration.ILRConnectionString))
                {
                    fm35LearningDeliveryPeriodisedValues = (from pv in _ilrContext.FM35_LearningDelivery_PeriodisedValues
                        join ld in _ilrContext.FM35_LearningDelivery
                            on new { pv.LearnRefNumber, pv.AimSeqNumber, pv.UKPRN } equals new { ld.LearnRefNumber, ld.AimSeqNumber, ld.UKPRN }
                        where pv.UKPRN == UkPrn &&
                              new[]
                              {
                                  "AEB – Other Learning",
                                  "AEB – Other Learning (non-procured)",
                                  "19-24 Traineeship",
                                  "19-24 Traineeship (non-procured)"
                              }.Contains(ld.FundLine) &&
                              new[]
                              {
                                  "OnProgPayment",
                                  "BalancePayment ",
                                  "AchievePayment ",
                                  "EmpOutcomePay",
                                  "LearnSuppFundCash"
                              }.Contains(pv.AttributeName)
                        select new FM35LearningDeliveryValues()
                        {
                            AttributeName = pv.AttributeName,
                            FundLine = ld.FundLine,
                            UKPRN = pv.UKPRN,
                            LearnRefNumber = pv.LearnRefNumber,
                            AimSeqNumber = pv.AimSeqNumber,
                            Period1 = pv.Period_1,
                            Period2 = pv.Period_2,
                            Period3 = pv.Period_3,
                            Period4 = pv.Period_4,
                            Period5 = pv.Period_5,
                            Period6 = pv.Period_6,
                            Period7 = pv.Period_7,
                            Period8 = pv.Period_8,
                            Period9 = pv.Period_9,
                            Period10 = pv.Period_10,
                            Period11 = pv.Period_11,
                            Period12 = pv.Period_12
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get FM35 Learning Delivery Periodised Values", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return fm35LearningDeliveryPeriodisedValues.ToList();
        }
    }
}
