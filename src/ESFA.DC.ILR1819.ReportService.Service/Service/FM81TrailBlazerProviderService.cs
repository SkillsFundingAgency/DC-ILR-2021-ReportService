using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class FM81TrailBlazerProviderService : IFM81TrailBlazerProviderService
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;

        private readonly SemaphoreSlim _getDataLock;

        private bool _loadedDataAlready;

        private FM81Global _fundingOutputs;

        public FM81TrailBlazerProviderService(
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

        public async Task<FM81Global> GetFM81Data(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
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

                int ukPrn = _intUtilitiesService.ObjectToInt(jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]);
                if (jobContextMessage.KeyValuePairs.ContainsKey(JobContextMessageKey.FundingFm81Output)
                    && await _redis.ContainsAsync(jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm81Output].ToString(), cancellationToken))
                {
                    string fm81Filename = jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm81Output].ToString();
                    string fm81 = await _redis.GetAsync(fm81Filename, cancellationToken);

                    if (string.IsNullOrEmpty(fm81))
                    {
                        _fundingOutputs = null;
                        return _fundingOutputs;
                    }

                    _fundingOutputs = _jsonSerializationService.Deserialize<FM81Global>(fm81);
                }
                else
                {
                    FM81Global fm81Global = new FM81Global();
                    using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                    {
                        var fm81GlobalDb = await ilrContext.TBL_global.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                        using (var ilrValidContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreValidConnectionString))
                        {
                            TBL_LearningDelivery[] res = await ilrContext.TBL_LearningDelivery.Where(x => x.UKPRN == ukPrn)
                                .Include(x => x.TBL_LearningDelivery_PeriodisedValues).ToArrayAsync(cancellationToken);

                            IGrouping<string, TBL_LearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                            fm81Global.Learners = new List<FM81Learner>();

                            foreach (IGrouping<string, TBL_LearningDelivery> fm81LearningDeliveries in learners)
                            {
                                var learningDeliveryDto = new List<ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery>();
                                foreach (var ld in fm81LearningDeliveries)
                                {
                                    var ldPeriodisedValues = ld.TBL_LearningDelivery_PeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValue()
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
                                        LearningDeliveryValues = new LearningDeliveryValue()
                                        {
                                            FundLine = ld.FundLine,
                                            AchApplicDate = ld.AchApplicDate // todo: finish the entire LearningDeliveryValue here
                                        }
                                    });
                                }

                                FM81Learner learner = new FM81Learner()
                                {
                                    LearnRefNumber = fm81LearningDeliveries.Key,
                                    LearningDeliveries = learningDeliveryDto
                                };

                                fm81Global.Learners.Add(learner);
                            }
                        }

                        if (fm81GlobalDb != null)
                        {
                            fm81Global.LARSVersion = fm81GlobalDb.LARSVersion;
                            fm81Global.CurFundYr = fm81GlobalDb.CurFundYr;
                            fm81Global.RulebaseVersion = fm81GlobalDb.RulebaseVersion;
                            fm81Global.UKPRN = fm81GlobalDb.UKPRN;
                        }
                    }

                    _fundingOutputs = fm81Global;
                }
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise FM81 funding data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}