using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interfaces;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using LearningDelivery = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDelivery;
using LearningDeliveryPeriodisedValue = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue;
using LearningDeliveryValue = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryValue;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class AllbProviderService : IAllbProviderService
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private ALBGlobal _fundingOutputs;

        public AllbProviderService(
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

                int ukPrn = _intUtilitiesService.ObjectToInt(jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]);

                if (await _redis.ContainsAsync(albFilename, cancellationToken))
                {
                    string alb = await _redis.GetAsync(albFilename, cancellationToken);

                    if (string.IsNullOrEmpty(alb))
                    {
                        _fundingOutputs = null;
                        return _fundingOutputs;
                    }

                    _fundingOutputs = _jsonSerializationService.Deserialize<ALBGlobal>(alb);
                }
                else
                {
                    ALBGlobal albGlobal = new ALBGlobal();
                    using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                    {
                        var albGlobalDb = await ilrContext.FM35_global.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                        using (var ilrValidContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreValidConnectionString))
                        {
                            ALB_LearningDelivery[] res = await ilrContext.ALB_LearningDelivery
                                .Where(x => x.UKPRN == ukPrn)
                                .Include(x => x.ALB_LearningDelivery_PeriodisedValues).ToArrayAsync(cancellationToken);

                            IGrouping<string, ALB_LearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                            albGlobal.Learners = new System.Collections.Generic.List<ALBLearner>();

                            foreach (IGrouping<string, ALB_LearningDelivery> albLearningDeliveries in learners)
                            {
                                var learningDeliveryDto = new List<LearningDelivery>();
                                foreach (var ld in albLearningDeliveries)
                                {
                                    var ldPeriodisedValues = ld.ALB_LearningDelivery_PeriodisedValues.Select(ldpv =>
                                        new LearningDeliveryPeriodisedValue()
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

                                    learningDeliveryDto.Add(new LearningDelivery()
                                    {
                                        AimSeqNumber = ld.AimSeqNumber,
                                        LearningDeliveryPeriodisedValues = ldPeriodisedValues,
                                        LearningDeliveryValue = new LearningDeliveryValue()
                                        {
                                            FundLine = ld.FundLine // todo: finish the entire LearningDeliveryValue here
                                        }
                                    });
                                }

                                ALBLearner learner = new ALBLearner()
                                {
                                    LearnRefNumber = albLearningDeliveries.Key,
                                    LearningDeliveries = learningDeliveryDto
                                };

                                albGlobal.Learners.Add(learner);
                            }
                        }

                        if (albGlobalDb != null)
                        {
                            albGlobal.LARSVersion = albGlobalDb.LARSVersion;
                            albGlobal.PostcodeAreaCostVersion = albGlobalDb.PostcodeDisadvantageVersion;
                            albGlobal.RulebaseVersion = albGlobalDb.RulebaseVersion;
                            albGlobalDb.OrgVersion = albGlobalDb.OrgVersion;
                            albGlobalDb.CurFundYr = albGlobalDb.CurFundYr;
                            albGlobalDb.FM35_LearningDelivery = albGlobalDb.FM35_LearningDelivery;
                            albGlobal.UKPRN = albGlobalDb.UKPRN;
                        }
                    }

                    _fundingOutputs = albGlobal;
                }
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

        public async Task<List<ALBLearningDeliveryValues>> GetALBDataFromDataStore(
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

                var ukPrn = _intUtilitiesService.ObjectToInt(jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]);
                using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                {
                    albLearningDeliveryPeriodisedValues = (from pv in ilrContext.ALB_LearningDelivery_PeriodisedValues
                                                           join ld in ilrContext.ALB_LearningDelivery
                                                               on new { pv.LearnRefNumber, pv.AimSeqNumber, pv.UKPRN } equals new { ld.LearnRefNumber, ld.AimSeqNumber, ld.UKPRN }
                                                           where pv.UKPRN == ukPrn
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