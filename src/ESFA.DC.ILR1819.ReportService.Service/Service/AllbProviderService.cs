using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using LearningDelivery = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDelivery;
using LearningDeliveryPeriodisedValue = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue;
using LearningDeliveryValue = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryValue;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class AllbProviderService : IAllbProviderService
    {
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private ALBGlobal _fundingOutputs;

        public AllbProviderService(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IJsonSerializationService jsonSerializationService,
            IIntUtilitiesService intUtilitiesService,
            DataStoreConfiguration dataStoreConfiguration)
        {
            _logger = logger;
            _storage = storage;
            _jsonSerializationService = jsonSerializationService;
            _intUtilitiesService = intUtilitiesService;
            _dataStoreConfiguration = dataStoreConfiguration;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<ALBGlobal> GetAllbData(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
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
                    string albFilename = reportServiceContext.FundingALBOutputKey;
                    _logger.LogWarning($"Reading {albFilename}; Storage is {_storage}; CancellationToken is {cancellationToken}");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await _storage.GetAsync(albFilename, ms, cancellationToken);
                        _fundingOutputs = _jsonSerializationService.Deserialize<ALBGlobal>(ms);
                    }
                }
                else
                {
                    ALBGlobal albGlobal = new ALBGlobal();
                    DbContextOptions<ILR1819_DataStoreEntities> options = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
                    using (var ilrContext = new ILR1819_DataStoreEntities(options))
                    {
                        var albGlobalDb = await ilrContext.AlbGlobals.FirstOrDefaultAsync(x => x.Ukprn == ukPrn, cancellationToken);
                        DbContextOptions<ILR1819_DataStoreEntitiesValid> validContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
                        using (var ilrValidContext = new ILR1819_DataStoreEntitiesValid(validContextOptions))
                        {
                            AlbLearningDelivery[] res = await ilrContext.AlbLearningDeliveries
                                .Where(x => x.Ukprn == ukPrn)
                                .Include(x => x.AlbLearningDeliveryPeriodisedValues).ToArrayAsync(cancellationToken);

                            IGrouping<string, AlbLearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                            albGlobal.Learners = new System.Collections.Generic.List<ALBLearner>();

                            foreach (IGrouping<string, AlbLearningDelivery> albLearningDeliveries in learners)
                            {
                                var learningDeliveryDto = new List<LearningDelivery>();
                                foreach (var ld in albLearningDeliveries)
                                {
                                    var ldPeriodisedValues = ld.AlbLearningDeliveryPeriodisedValues.Select(ldpv =>
                                        new LearningDeliveryPeriodisedValue()
                                        {
                                            AttributeName = ldpv.AttributeName,
                                            Period1 = ldpv.Period1,
                                            Period2 = ldpv.Period2,
                                            Period3 = ldpv.Period3,
                                            Period4 = ldpv.Period4,
                                            Period5 = ldpv.Period5,
                                            Period6 = ldpv.Period6,
                                            Period7 = ldpv.Period7,
                                            Period8 = ldpv.Period8,
                                            Period9 = ldpv.Period9,
                                            Period10 = ldpv.Period10,
                                            Period11 = ldpv.Period11,
                                            Period12 = ldpv.Period12
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
                            albGlobal.LARSVersion = albGlobalDb.Larsversion;
                            albGlobal.PostcodeAreaCostVersion = albGlobalDb.PostcodeAreaCostVersion;
                            albGlobal.RulebaseVersion = albGlobalDb.RulebaseVersion;
                            albGlobal.UKPRN = albGlobalDb.Ukprn;
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
            IReportServiceContext reportServiceContext,
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

                var ukPrn = reportServiceContext.Ukprn;
                DbContextOptions<ILR1819_DataStoreEntities> options = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
                using (var ilrContext = new ILR1819_DataStoreEntities(options))
                {
                    albLearningDeliveryPeriodisedValues = (from pv in ilrContext.AlbLearningDeliveryPeriodisedValues
                                                           join ld in ilrContext.AlbLearningDeliveries
                                                               on new { pv.LearnRefNumber, pv.AimSeqNumber, pv.Ukprn } equals new { ld.LearnRefNumber, ld.AimSeqNumber, ld.Ukprn }
                                                           where pv.Ukprn == ukPrn
                                                           select new ALBLearningDeliveryValues()
                                                           {
                                                               AttributeName = pv.AttributeName,
                                                               UKPRN = pv.Ukprn,
                                                               LearnRefNumber = pv.LearnRefNumber,
                                                               AimSeqNumber = pv.AimSeqNumber,
                                                               FundLine = ld.FundLine,
                                                               Period1 = pv.Period1,
                                                               Period2 = pv.Period2,
                                                               Period3 = pv.Period3,
                                                               Period4 = pv.Period4,
                                                               Period5 = pv.Period5,
                                                               Period6 = pv.Period6,
                                                               Period7 = pv.Period7,
                                                               Period8 = pv.Period8,
                                                               Period9 = pv.Period9,
                                                               Period10 = pv.Period10,
                                                               Period11 = pv.Period11,
                                                               Period12 = pv.Period12
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