﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public sealed class FM81TrailBlazerProviderService : AbstractFundModelProviderService, IFM81TrailBlazerProviderService
    {
        private readonly Func<IIlr1819ValidContext> _ilrValidContextFactory;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private FM81Global _fundingOutputs;

        public FM81TrailBlazerProviderService(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IJsonSerializationService jsonSerializationService,
            Func<IIlr1819ValidContext> ilrValidContextFactory,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        : base(storage, jsonSerializationService, logger)
        {
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
                    string fm81 = await _streamableKeyValuePersistenceService.GetAsync(fm81Filename, cancellationToken);

                    if (string.IsNullOrEmpty(fm81))
                    {
                        _fundingOutputs = null;
                        return _fundingOutputs;
                    }

                    _fundingOutputs = _serializationService.Deserialize<FM81Global>(fm81);
                }
                else
                {
                    FM81Global fm81Global = new FM81Global();
                    using (var ilrContext = _ilrRulebaseContextFactory())
                    {
                        var fm81GlobalDb = await ilrContext.TBL_globals.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                        using (var ilrValidContext = _ilrValidContextFactory())
                        {
                            TBL_LearningDelivery[] res = await ilrContext.TBL_LearningDeliveries.Where(x => x.UKPRN == ukPrn)
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
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}