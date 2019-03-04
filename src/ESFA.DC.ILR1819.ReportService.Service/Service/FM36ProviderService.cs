using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class FM36ProviderService : IFM36ProviderService
    {
        private readonly ILogger _logger;

        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;

        private readonly SemaphoreSlim _getDataLock;

        private bool _loadedDataAlready;

        private FM36Global _fundingOutputs;

        public FM36ProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService blob,
            IJsonSerializationService jsonSerializationService,
            IIntUtilitiesService intUtilitiesService,
            DataStoreConfiguration dataStoreConfiguration)
        {
            _logger = logger;
            _redis = redis;
            _blob = blob;
            _jsonSerializationService = jsonSerializationService;
            _intUtilitiesService = intUtilitiesService;
            _dataStoreConfiguration = dataStoreConfiguration;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<FM36Global> GetFM36Data(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
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

                int ukPrn = reportServiceContext.Ukprn;
                if (string.Equals(reportServiceContext.CollectionName, "ILR1819", StringComparison.OrdinalIgnoreCase))
                {
                    string fm36Filename = reportServiceContext.FundingFM36OutputKey;
                    string fm36 = await _redis.GetAsync(fm36Filename, cancellationToken);

                    if (string.IsNullOrEmpty(fm36))
                    {
                        _fundingOutputs = null;
                        return _fundingOutputs;
                    }

                    // await _blob.SaveAsync($"{jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]}_{jobContextMessage.JobId.ToString()}_Fm36.json", fm36, cancellationToken);
                    _fundingOutputs = _jsonSerializationService.Deserialize<FM36Global>(fm36);
                }
                else
                {
                    FM36Global fm36Global = new FM36Global();
                    using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                    {
                        var fm36GlobalDb = await ilrContext.AEC_global.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                        //AEC_LearningDelivery[] res = await ilrContext.AEC_LearningDelivery_Period.Where(x => x.UKPRN == ukPrn).Select(x => x.AEC_LearningDelivery)
                        //    .Include(x => x.AEC_LearningDelivery_PeriodisedValues).ToArrayAsync(cancellationToken);

                        AEC_LearningDelivery[] res = await ilrContext.AEC_LearningDelivery.Where(x => x.UKPRN == ukPrn)
                            .Include(x => x.AEC_LearningDelivery_PeriodisedValues).ToArrayAsync(cancellationToken);

                        IGrouping<string, AEC_LearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                        fm36Global.Learners = new List<FM36Learner>();

                        foreach (IGrouping<string, AEC_LearningDelivery> albLearningDeliveries in learners)
                        {
                            var learningDeliveryDto = new List<ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery>();
                            foreach (var ld in albLearningDeliveries)
                            {
                                var ldPeriodisedValues = ld.AEC_LearningDelivery_PeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValues()
                                {
                                    AttributeName = ldpv.AttributeName,
                                    Period1 = ldpv.Period_1,
                                    Period2 = ldpv.Period_2,
                                    Period3 = ldpv.Period_3,
                                    Period4 = ldpv.Period_4,
                                    Period5 = ldpv.Period_5,
                                    Period6 = ldpv.Period_6,
                                    Period7 = ldpv.Period_7,
                                    Period8 = ldpv.Period_8,
                                    Period9 = ldpv.Period_9,
                                    Period10 = ldpv.Period_10,
                                    Period11 = ldpv.Period_11,
                                    Period12 = ldpv.Period_12
                                }).ToList();

                                learningDeliveryDto.Add(new ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery()
                                {
                                    AimSeqNumber = ld.AimSeqNumber,
                                    LearningDeliveryPeriodisedValues = ldPeriodisedValues,
                                    LearningDeliveryValues = new LearningDeliveryValues()
                                    {
                                        LearnDelInitialFundLineType = ld.LearnDelInitialFundLineType // todo : rest of the properties
                                    }
                                });
                            }

                            FM36Learner learner = new FM36Learner()
                            {
                                LearnRefNumber = albLearningDeliveries.Key,
                                LearningDeliveries = learningDeliveryDto
                            };

                            fm36Global.Learners.Add(learner);
                        }

                        if (fm36GlobalDb != null)
                        {
                            fm36Global.LARSVersion = fm36GlobalDb.LARSVersion;
                            fm36Global.RulebaseVersion = fm36GlobalDb.RulebaseVersion;
                            fm36Global.UKPRN = fm36GlobalDb.UKPRN;
                        }
                    }

                    _fundingOutputs = fm36Global;
                }
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise FM36 funding data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}