using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
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

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class AllbProviderService : IAllbProviderService
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private ALBGlobal _fundingOutputs;

        public AllbProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService blob,
            IJsonSerializationService jsonSerializationService,
            DataStoreConfiguration dataStoreConfiguration)
        {
            _logger = logger;
            _redis = redis;
            _blob = blob;
            _jsonSerializationService = jsonSerializationService;
            _dataStoreConfiguration = dataStoreConfiguration;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<ALBGlobal> GetAllbData(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
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
                string albFilename = jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput].ToString();
                string alb = await _redis.GetAsync(albFilename, cancellationToken);

                if (string.IsNullOrEmpty(alb))
                {
                    _fundingOutputs = null;
                    return _fundingOutputs;
                }

                _fundingOutputs = _jsonSerializationService.Deserialize<ALBGlobal>(alb);
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise ALB funding data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }

        public async Task<List<ALBLearningDeliveryValues>> GetALBFM35AdultFundingLineDataFromDataStore(
            IJobContextMessage jobContextMessage,
            CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);
            var albLearningDeliveryPeriodisedValues = new List<ALBLearningDeliveryValues>();
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                //var UkPrn = int.Parse(jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString());
                var UkPrn = 10000046;

                using (var _ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                {
                    albLearningDeliveryPeriodisedValues = (from pv in _ilrContext.ALB_LearningDelivery_PeriodisedValues
                        join ld in _ilrContext.ALB_LearningDelivery
                            on new { pv.LearnRefNumber, pv.AimSeqNumber, pv.UKPRN } equals new { ld.LearnRefNumber, ld.AimSeqNumber, ld.UKPRN }
                        where pv.UKPRN == UkPrn &&
                              new[]
                              {
                                  "Advanced Learner Loans Bursary"
                              }.Contains(ld.FundLine) &&
                              new[]
                              {
                                  "ALBSupportPayment",
                                  "AreaUpliftBalPayment",
                                  "AreaUpliftOnProgPayment"
                              }.Contains(pv.AttributeName)
                        select new ALBLearningDeliveryValues()
                        {
                            AttributeName = pv.AttributeName,
                            UKPRN = pv.UKPRN,
                            LearnRefNumber = pv.LearnRefNumber,
                            AimSeqNumber = pv.AimSeqNumber,
                            FundLine = ld.FundLine,
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
                _logger.LogError("Failed to get Alb Learning delivery Periodised values", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return albLearningDeliveryPeriodisedValues.ToList();
        }
    }
}
