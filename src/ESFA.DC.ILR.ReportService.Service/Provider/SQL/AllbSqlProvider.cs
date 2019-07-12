using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;
using LearningDelivery = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDelivery;
using LearningDeliveryPeriodisedValue = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue;
using LearningDeliveryValue = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryValue;

namespace ESFA.DC.ILR.ReportService.Service.Provider.SQL
{
    public sealed class AllbSqlProvider : IAllbProviderService
    {
        private readonly ILogger _logger;
        private readonly Func<IIlr1819ValidContext> _ilrValidContextFactory;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);
        private bool _loadedDataAlready;
        private ALBGlobal _fundingOutputs;

        public AllbSqlProvider(
            ILogger logger,
            Func<IIlr1819ValidContext> ilrValidContextFactory,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        {
            _logger = logger;
            _ilrValidContextFactory = ilrValidContextFactory;
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
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

                cancellationToken.ThrowIfCancellationRequested();

                _loadedDataAlready = true;
                int ukPrn = reportServiceContext.Ukprn;

                _logger.LogWarning($"ReportServiceCollectionName {reportServiceContext.CollectionName};");


                ALBGlobal albGlobal = new ALBGlobal();
                using (var ilrContext = _ilrRulebaseContextFactory())
                {
                    _logger.LogWarning($"AllbProviderService - Accessing Db;");
                    var albGlobalDb = await ilrContext.ALB_globals.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                    using (var ilrValidContext = _ilrValidContextFactory())
                    {
                        ALB_LearningDelivery[] res = await ilrContext.ALB_LearningDeliveries
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
                        albGlobal.PostcodeAreaCostVersion = albGlobalDb.PostcodeAreaCostVersion;
                        albGlobal.RulebaseVersion = albGlobalDb.RulebaseVersion;
                        albGlobal.UKPRN = albGlobalDb.UKPRN;
                    }

                    _fundingOutputs = albGlobal;
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