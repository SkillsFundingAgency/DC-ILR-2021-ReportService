using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
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
using LearningDelivery = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR.ReportService.Service.Provider.SQL
{
    public class FM35SqlProvider : IFM35ProviderService
    {
        private readonly ILogger _logger;
        private readonly Func<IIlr1819ValidContext> _ilrValidContextFactory;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);
        private bool _loadedDataAlready;
        private FM35Global _fundingOutputs;

        public FM35SqlProvider(
            ILogger logger,
            Func<IIlr1819ValidContext> ilrValidContextFactory,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        {
            _logger = logger;
            _ilrValidContextFactory = ilrValidContextFactory;
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
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

                cancellationToken.ThrowIfCancellationRequested();

                _loadedDataAlready = true;
                int ukPrn = reportServiceContext.Ukprn;
                
                FM35Global fm35Global = new FM35Global();
                using (var ilrContext = _ilrRulebaseContextFactory())
                {
                    var fm35GlobalDb = await ilrContext.FM35_globals.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                    FM35_LearningDelivery[] res = await ilrContext.FM35_LearningDeliveries.Where(x => x.UKPRN == ukPrn)
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

                    _fundingOutputs = fm35Global;
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