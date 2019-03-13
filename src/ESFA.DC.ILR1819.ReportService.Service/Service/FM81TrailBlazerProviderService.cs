using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class FM81TrailBlazerProviderService : IFM81TrailBlazerProviderService
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly Func<IIlr1819ValidContext> _ilrValidContextFactory;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private FM81Global _fundingOutputs;

        public FM81TrailBlazerProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService blob,
            IJsonSerializationService jsonSerializationService,
            IIntUtilitiesService intUtilitiesService,
            Func<IIlr1819ValidContext> ilrValidContextFactory,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        {
            _logger = logger;
            _redis = redis;
            _blob = blob;
            _jsonSerializationService = jsonSerializationService;
            _intUtilitiesService = intUtilitiesService;
            _ilrValidContextFactory = ilrValidContextFactory;
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<FM81Global> GetFM81Data(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_loadedDataAlready)
                {
                    return _fundingOutputs;
                }

                cancellationToken.ThrowIfCancellationRequested();

                _loadedDataAlready = true;

                int ukPrn = reportServiceContext.Ukprn;
                if (string.Equals(reportServiceContext.CollectionName, "ILR1819", StringComparison.OrdinalIgnoreCase))
                {
                    string fm81Filename = reportServiceContext.FundingFM81OutputKey;
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
                    using (var ilrContext = _ilrRulebaseContextFactory())
                    {
                        var fm81GlobalDb = await ilrContext.TblGlobals.FirstOrDefaultAsync(x => x.Ukprn == ukPrn, cancellationToken);
                        using (var ilrValidContext = _ilrValidContextFactory())
                        {
                            TblLearningDelivery[] res = await ilrContext.TblLearningDeliveries.Where(x => x.Ukprn == ukPrn)
                                .Include(x => x.TblLearningDeliveryPeriodisedValues).ToArrayAsync(cancellationToken);

                            IGrouping<string, TblLearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                            fm81Global.Learners = new List<FM81Learner>();

                            foreach (IGrouping<string, TblLearningDelivery> fm81LearningDeliveries in learners)
                            {
                                var learningDeliveryDto = new List<ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery>();
                                foreach (var ld in fm81LearningDeliveries)
                                {
                                    var ldPeriodisedValues = ld.TblLearningDeliveryPeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValue()
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
                            fm81Global.LARSVersion = fm81GlobalDb.Larsversion;
                            fm81Global.CurFundYr = fm81GlobalDb.CurFundYr;
                            fm81Global.RulebaseVersion = fm81GlobalDb.RulebaseVersion;
                            fm81Global.UKPRN = fm81GlobalDb.Ukprn;
                        }
                    }

                    _fundingOutputs = fm81Global;
                }
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}