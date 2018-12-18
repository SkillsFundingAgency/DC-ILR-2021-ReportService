using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Newtonsoft.Json;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class FM35ProviderService : IFM35ProviderService
    {
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _redis;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private FM35Global _fundingOutputs;

        public FM35ProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)]
            IStreamableKeyValuePersistenceService redis,
            IJsonSerializationService jsonSerializationService,
            IIntUtilitiesService intUtilitiesService,
            DataStoreConfiguration dataStoreConfiguration)
        {
            _logger = logger;
            _redis = redis;
            _jsonSerializationService = jsonSerializationService;
            _intUtilitiesService = intUtilitiesService;
            _dataStoreConfiguration = dataStoreConfiguration;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<FM35Global> GetFM35Data(
            IReportServiceContext reportServiceContext,
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
                int ukPrn = reportServiceContext.Ukprn;

                if (string.Equals(reportServiceContext.CollectionName, "ILR1819", StringComparison.OrdinalIgnoreCase))
                {
                    string fm35Filename = reportServiceContext.FundingFM35OutputKey;

                    if (await _redis.ContainsAsync(fm35Filename, cancellationToken))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await _redis.GetAsync(fm35Filename, ms, cancellationToken);
                            _fundingOutputs = _jsonSerializationService.Deserialize<FM35Global>(ms);
                        }
                    }
                }
                else
                {
                    FM35Global fm35Global = new FM35Global();
                    using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                    {
                        var fm35GlobalDb = await ilrContext.FM35_global.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                        FM35_LearningDelivery[] res = await ilrContext.FM35_LearningDelivery.Where(x => x.UKPRN == ukPrn)
                            .Include(x => x.FM35_LearningDelivery_PeriodisedValues).ToArrayAsync(cancellationToken);

                        IGrouping<string, FM35_LearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                        fm35Global.Learners = new List<FM35Learner>();

                        foreach (IGrouping<string, FM35_LearningDelivery> fm35LearningDeliveries in learners)
                        {
                            var learningDeliveryDto = new List<LearningDelivery>();
                            foreach (var ld in fm35LearningDeliveries)
                            {
                                var ldPeriodisedValues = ld.FM35_LearningDelivery_PeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValue()
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
                                        FundLine = ld.FundLine,
                                        AchApplicDate = ld.AchApplicDate // todo: finish the entire LearningDeliveryValue here
                                    }
                                });
                            }

                            FM35Learner learner = new FM35Learner()
                            {
                                LearnRefNumber = fm35LearningDeliveries.Key,
                                LearningDeliveries = learningDeliveryDto
                            };

                            fm35Global.Learners.Add(learner);
                        }

                        if (fm35GlobalDb != null)
                        {
                            fm35Global.LARSVersion = fm35GlobalDb.LARSVersion;
                            fm35Global.OrgVersion = fm35GlobalDb.OrgVersion;
                            fm35Global.PostcodeDisadvantageVersion = fm35GlobalDb.PostcodeDisadvantageVersion;
                            fm35Global.RulebaseVersion = fm35GlobalDb.RulebaseVersion;
                            fm35Global.UKPRN = fm35GlobalDb.UKPRN;
                        }
                    }

                    _fundingOutputs = fm35Global;
                }
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

        public async Task<List<FM35LearningDeliveryValues>> GetFM35DataFromDataStore(
            IReportServiceContext reportServiceContext,
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

                var UkPrn = reportServiceContext.Ukprn;

                using (ILR1819_DataStoreEntities _ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                {
                    fm35LearningDeliveryPeriodisedValues = (from pv in _ilrContext.FM35_LearningDelivery_PeriodisedValues
                                                            join ld in _ilrContext.FM35_LearningDelivery
                                                                on new { pv.LearnRefNumber, pv.AimSeqNumber, pv.UKPRN } equals new { ld.LearnRefNumber, ld.AimSeqNumber, ld.UKPRN }
                                                            where pv.UKPRN == UkPrn
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
                                                                Period12 = pv.Period_12,
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

        private List<string> GetValidLearners(int ukPrn)
        {
            List<string> learners;
            using (var ilrContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreValidConnectionString))
            {
                learners = ilrContext.Learners.Where(x => x.UKPRN == ukPrn).Select(x => x.LearnRefNumber).ToList();
            }

            return learners;
        }
    }
}