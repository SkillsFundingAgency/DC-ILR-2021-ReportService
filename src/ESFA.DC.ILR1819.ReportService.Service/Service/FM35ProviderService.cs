using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class FM35ProviderService : IFM35ProviderService
    {
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly Func<IIlr1819ValidContext> _ilrValidContextFactory;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private FM35Global _fundingOutputs;

        public FM35ProviderService(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IJsonSerializationService jsonSerializationService,
            IIntUtilitiesService intUtilitiesService,
            Func<IIlr1819ValidContext> ilrValidContextFactory,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        {
            _logger = logger;
            _storage = storage;
            _jsonSerializationService = jsonSerializationService;
            _intUtilitiesService = intUtilitiesService;
            _ilrValidContextFactory = ilrValidContextFactory;
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
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
                    _logger.LogWarning($"Reading {fm35Filename}; Storage is {_storage}; CancellationToken is {cancellationToken}");
                    if (await _storage.ContainsAsync(fm35Filename, cancellationToken))
                    {
                        _logger.LogWarning($"Available {fm35Filename}");
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await _storage.GetAsync(fm35Filename, ms, cancellationToken);
                            _logger.LogWarning($"Deserialising {fm35Filename} with {ms.Length}");
                            _fundingOutputs = _jsonSerializationService.Deserialize<FM35Global>(ms);
                        }
                    }

                    _logger.LogWarning($"Finished {fm35Filename}");
                }
                else
                {
                    FM35Global fm35Global = new FM35Global();
                    using (var ilrContext = _ilrRulebaseContextFactory())
                    {
                        var fm35GlobalDb = await ilrContext.Fm35Globals.FirstOrDefaultAsync(x => x.Ukprn == ukPrn, cancellationToken);
                        Fm35LearningDelivery[] res = await ilrContext.Fm35LearningDeliveries.Where(x => x.Ukprn == ukPrn)
                            .Include(x => x.Fm35LearningDeliveryPeriodisedValues).ToArrayAsync(cancellationToken);

                        IGrouping<string, Fm35LearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                        fm35Global.Learners = new List<FM35Learner>();

                        foreach (IGrouping<string, Fm35LearningDelivery> fm35LearningDeliveries in learners)
                        {
                            var learningDeliveryDto = new List<LearningDelivery>();
                            foreach (var ld in fm35LearningDeliveries)
                            {
                                var ldPeriodisedValues = ld.Fm35LearningDeliveryPeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValue()
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
                            fm35Global.LARSVersion = fm35GlobalDb.Larsversion;
                            fm35Global.OrgVersion = fm35GlobalDb.OrgVersion;
                            fm35Global.PostcodeDisadvantageVersion = fm35GlobalDb.PostcodeDisadvantageVersion;
                            fm35Global.RulebaseVersion = fm35GlobalDb.RulebaseVersion;
                            fm35Global.UKPRN = fm35GlobalDb.Ukprn;
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

                using (var ilrContext = _ilrRulebaseContextFactory())
                {
                    fm35LearningDeliveryPeriodisedValues = (from pv in ilrContext.Fm35LearningDeliveryPeriodisedValues
                                                            join ld in ilrContext.Fm35LearningDeliveries
                                                                on new { pv.LearnRefNumber, pv.AimSeqNumber, pv.Ukprn } equals new { ld.LearnRefNumber, ld.AimSeqNumber, ld.Ukprn }
                                                            where pv.Ukprn == UkPrn
                                                            select new FM35LearningDeliveryValues()
                                                            {
                                                                AttributeName = pv.AttributeName,
                                                                FundLine = ld.FundLine,
                                                                UKPRN = pv.Ukprn,
                                                                LearnRefNumber = pv.LearnRefNumber,
                                                                AimSeqNumber = pv.AimSeqNumber,
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
                                                                Period12 = pv.Period12,
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
            using (var ilrContext = _ilrValidContextFactory())
            {
                learners = ilrContext.Learners.Where(x => x.Ukprn == ukPrn).Select(x => x.LearnRefNumber).ToList();
            }

            return learners;
        }
    }
}