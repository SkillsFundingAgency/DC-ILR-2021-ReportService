using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class FM25ProviderService : IFM25ProviderService
    {
        private readonly ILogger _logger;

        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private FM25Global _fundingOutputs;

        public FM25ProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService blob,
            IJsonSerializationService jsonSerializationService,
            IIntUtilitiesService intUtilitiesService,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        {
            _logger = logger;
            _redis = redis;
            _blob = blob;
            _jsonSerializationService = jsonSerializationService;
            _intUtilitiesService = intUtilitiesService;
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<FM25Global> GetFM25Data(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
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
                    string fm25Filename = reportServiceContext.FundingFM25OutputKey;
                    string fm25 = await _redis.GetAsync(fm25Filename, cancellationToken);

                    if (string.IsNullOrEmpty(fm25))
                    {
                        _fundingOutputs = null;
                        return _fundingOutputs;
                    }

                    _fundingOutputs = _jsonSerializationService.Deserialize<FM25Global>(fm25);
                }
                else
                {
                    FM25Global fm25Global = new FM25Global();

                    using (var ilrContext = _ilrRulebaseContextFactory())
                    {
                        var fm25GlobalDb = await ilrContext.Fm25Globals.FirstOrDefaultAsync(x => x.Ukprn == ukPrn, cancellationToken);
                        Fm25Learner[] learners = await ilrContext.Fm25Learners.Where(x => x.Ukprn == ukPrn).Include(x => x.Fm25Fm35LearnerPeriodisedValues).ToArrayAsync(cancellationToken);
                        foreach (Fm25Learner fm25Learner in learners)
                        {
                            List<LearnerPeriodisedValues> learnerPeriodisedValues = new List<LearnerPeriodisedValues>();
                            foreach (var learnerPeriodisedValue in fm25Learner.Fm25Fm35LearnerPeriodisedValues)
                            {
                                learnerPeriodisedValues.Add(new LearnerPeriodisedValues
                                {
                                    AttributeName = learnerPeriodisedValue.AttributeName,
                                    LearnRefNumber = learnerPeriodisedValue.LearnRefNumber,
                                    Period1 = learnerPeriodisedValue.Period1,
                                    Period2 = learnerPeriodisedValue.Period2,
                                    Period3 = learnerPeriodisedValue.Period3,
                                    Period4 = learnerPeriodisedValue.Period4,
                                    Period5 = learnerPeriodisedValue.Period5,
                                    Period6 = learnerPeriodisedValue.Period6,
                                    Period7 = learnerPeriodisedValue.Period7,
                                    Period8 = learnerPeriodisedValue.Period8,
                                    Period9 = learnerPeriodisedValue.Period9,
                                    Period10 = learnerPeriodisedValue.Period10,
                                    Period11 = learnerPeriodisedValue.Period11,
                                    Period12 = learnerPeriodisedValue.Period12
                                });
                            }

                            fm25Global.Learners.Add(new FM25Learner
                            {
                                FundLine = fm25Learner.FundLine,
                                LearnerPeriodisedValues = learnerPeriodisedValues
                            });
                        }

                        if (fm25GlobalDb != null)
                        {
                            fm25Global.LARSVersion = fm25GlobalDb.Larsversion;
                            fm25Global.OrgVersion = fm25GlobalDb.OrgVersion;
                            fm25Global.PostcodeDisadvantageVersion = fm25GlobalDb.PostcodeDisadvantageVersion;
                            fm25Global.RulebaseVersion = fm25GlobalDb.RulebaseVersion;
                            fm25Global.UKPRN = fm25GlobalDb.Ukprn;
                        }
                    }

                    _fundingOutputs = fm25Global;
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