using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.FCS;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReportModelBuilder : IModelBuilder<IEnumerable<NonContractedAppsActivityReportModel>>
    {
        public ICollection<KeyValuePair<string, string[]>> ValidContractMappings = new List<KeyValuePair<string, string[]>>
        {
            new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
            new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
            new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 }),
            new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 }),
            new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 }),
            new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 })
        };

        private ICollection<string> _learningDeliveryFundedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment, AttributeConstants.Fm36LearnSuppFundCash };
        private ICollection<string> _priceEpisodeFundedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName,
            AttributeConstants.Fm3PriceEpisodeBalancePaymentAttributeName,
            AttributeConstants.Fm36PriceEpisodeCompletionPaymentAttributeName,
            AttributeConstants.Fm36PriceEpisodeLSFCashAttributeName,
            AttributeConstants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName,
            AttributeConstants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName,
            AttributeConstants.Fm36PriceEpisodeFirstEmp1618PayAttributeName,
            AttributeConstants.Fm36PriceEpisodeSecondEmp1618PayAttributeName,
            AttributeConstants.Fm36PriceEpisodeFirstProv1618PayAttributeName,
            AttributeConstants.Fm36PriceEpisodeSecondProv1618PayAttributeName,
            AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName,
            AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName,
            AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName,
            AttributeConstants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName
        };

        private readonly IIlrModelMapper _ilrModelMapper;

        public NonContractedAppsActivityReportModelBuilder(IIlrModelMapper ilrModelMapper)
        {
            _ilrModelMapper = ilrModelMapper;
        }

        public IEnumerable<NonContractedAppsActivityReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm36Data = reportServiceDependentData.Get<FM36Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var censusEndDates = referenceData.MetaDatas.CollectionDates.CensusDates.ToDictionary(p => p.Period, e => (DateTime?)e.End);
            var validContractsDictionary = BuildValidContractMapping();
            var larsLearningDeliveryDictionary = BuildLARSDictionary(referenceData.LARSLearningDeliveries);
            var fcsContractsDictionary = BuildFcsContractMapping(referenceData.FCSContractAllocations, message);

            var fm36LearnerModel = BuildFm36Learners(message, fm36Data, fcsContractsDictionary, validContractsDictionary);

            var reportRows = BuildReportRows(fm36LearnerModel, larsLearningDeliveryDictionary, censusEndDates);

            return reportRows;
        }

        public IEnumerable<NonContractedAppsActivityReportModel> BuildReportRows(IEnumerable<FM36LearnerData> fm36LearnerData, IDictionary<string, LARSLearningDelivery> larsDictionary, IReadOnlyDictionary<int, DateTime?> censusEndDates)
        {
            var models = new List<NonContractedAppsActivityReportModel>();

            if (fm36LearnerData != null)
            {
                foreach (var learner in fm36LearnerData)
                {
                    foreach (var learningDelivery in learner.LearningDeliveries)
                    {
                        if (learningDelivery.LearnDelMathEng)
                        {
                            models.AddRange(
                               learningDelivery.FM36LearningDelivery?.FundLineValues.SelectMany(fv =>
                               BuildLearningDeliveryACTValues(learningDelivery.LearnActEndDate, learningDelivery.LearningDeliveryFAMs_ACT, fv.ReportTotals, censusEndDates)
                               .Select(ldFamAct =>
                                 new NonContractedAppsActivityReportModel
                                 {
                                     Learner = learner.Learner,
                                     ProviderSpecLearnerMonitoring = learner.ProviderSpecLearnerMonitoringModels,
                                     LarsLearningDelivery = larsDictionary[learningDelivery.LearnAimRef],
                                     ProviderSpecDeliveryMonitoring = learningDelivery.ProviderSpecDeliveryMonitoringModels,
                                     LearningDeliveryFAMs = learningDelivery.LearningDeliveryFAMsModels,
                                     LearningDelivery = learningDelivery.LearningDelivery,
                                     FundingLineType = fv.FundLineType,
                                     Fm36LearningDelivery = learningDelivery.FM36LearningDelivery?.LearningDeliveryValues,
                                     AugustTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[1]) ? fv.ReportTotals.AugustTotal : 0m,
                                     SeptemberTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[2]) ? fv.ReportTotals.SeptemberTotal : 0m,
                                     OctoberTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[3]) ? fv.ReportTotals.OctoberTotal : 0m,
                                     NovemberTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[4]) ? fv.ReportTotals.NovemberTotal : 0m,
                                     DecemberTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[5]) ? fv.ReportTotals.DecemberTotal : 0m,
                                     JanuaryTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[6]) ? fv.ReportTotals.JanuaryTotal : 0m,
                                     FebruaryTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[7]) ? fv.ReportTotals.FebruaryTotal: 0m,
                                     MarchTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[8]) ? fv.ReportTotals.MarchTotal : 0m,
                                     AprilTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[9]) ? fv.ReportTotals.AprilTotal : 0m,
                                     MayTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[10]) ? fv.ReportTotals.MayTotal : 0m,
                                     JuneTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[11]) ? fv.ReportTotals.JuneTotal : 0m,
                                     JulyTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[12]) ? fv.ReportTotals.JulyTotal : 0m,
                                     LearningDeliveryFAM_ACTs = ldFamAct
                                 })).ToList());
                        }

                        else if (!learningDelivery.LearnDelMathEng)
                        {
                            models.AddRange(
                                learningDelivery.FM36PriceEpisodes?.Select(pe =>
                                new NonContractedAppsActivityReportModel
                                {
                                    Learner = learner.Learner,
                                    ProviderSpecLearnerMonitoring = learner.ProviderSpecLearnerMonitoringModels,
                                    LarsLearningDelivery = larsDictionary[learningDelivery.LearnAimRef],
                                    ProviderSpecDeliveryMonitoring = learningDelivery.ProviderSpecDeliveryMonitoringModels,
                                    LearningDeliveryFAMs = learningDelivery.LearningDeliveryFAMsModels,
                                    LearningDelivery = learningDelivery.LearningDelivery,
                                    FundingLineType = pe.PriceEpisodeValue.PriceEpisodeFundLineType,
                                    PriceEpisodeValues = pe.PriceEpisodeValue,
                                    PriceEpisodeStartDate = pe.PriceEpisodeValue.EpisodeStartDate,
                                    AugustTotal = pe.FundLineValues.ReportTotals.AugustTotal,
                                    SeptemberTotal = pe.FundLineValues.ReportTotals.SeptemberTotal,
                                    OctoberTotal = pe.FundLineValues.ReportTotals.OctoberTotal,
                                    NovemberTotal = pe.FundLineValues.ReportTotals.NovemberTotal,
                                    DecemberTotal = pe.FundLineValues.ReportTotals.DecemberTotal,
                                    JanuaryTotal = pe.FundLineValues.ReportTotals.JanuaryTotal,
                                    FebruaryTotal = pe.FundLineValues.ReportTotals.FebruaryTotal,
                                    MarchTotal = pe.FundLineValues.ReportTotals.MarchTotal,
                                    AprilTotal = pe.FundLineValues.ReportTotals.AprilTotal,
                                    MayTotal = pe.FundLineValues.ReportTotals.MayTotal,
                                    JuneTotal = pe.FundLineValues.ReportTotals.JuneTotal,
                                    JulyTotal = pe.FundLineValues.ReportTotals.JulyTotal,
                                    LearningDeliveryFAM_ACTs = BuildPriceEpisodeACTValues(pe.PriceEpisodeValue.EpisodeStartDate, learningDelivery.LearningDeliveryFAMs_ACT),
                                }).ToList());
                        }
                    }
                }
            }

            return ApplySort(models);
        }

        public IEnumerable<NonContractedAppsActivityReportModel> ApplySort(IEnumerable<NonContractedAppsActivityReportModel> models)
        {
            return models.OrderBy(m => m.LearnRefNumber).ThenBy(m => m.AimSeqNumber);
        }

        public bool ReportRowTotalApplicable(ILearningDeliveryFAM learningDeliveryFAM, DateTime? censusDate)
        {
            if (!learningDeliveryFAM.LearnDelFAMDateToNullable.HasValue)
            {
                return learningDeliveryFAM.LearnDelFAMDateFromNullable <= censusDate;
            }

            return learningDeliveryFAM.LearnDelFAMDateFromNullable <= censusDate && learningDeliveryFAM.LearnDelFAMDateToNullable >= censusDate;
        }

        public ILearningDeliveryFAM BuildPriceEpisodeACTValues(DateTime? episodeStartDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            var learningDeliveryFAMDateFrom = learningDeliveryFAMs?.Where(f => episodeStartDate >= f.LearnDelFAMDateFromNullable).Max(x => x.LearnDelFAMDateFromNullable);
            var learningDeliveryFAMDateTo = learningDeliveryFAMs?.Where(f => episodeStartDate <= f.LearnDelFAMDateToNullable).Min(x => x.LearnDelFAMDateToNullable);

            var learningDeliveryFAMCode = learningDeliveryFAMs?
              .FirstOrDefault(f => episodeStartDate >= learningDeliveryFAMDateFrom && episodeStartDate <= (learningDeliveryFAMDateTo.HasValue ? learningDeliveryFAMDateTo.Value : episodeStartDate))?
              .LearnDelFAMCode;

            return new LearningDeliveryACT
            {
                LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                LearnDelFAMCode = learningDeliveryFAMCode,
                LearnDelFAMDateFromNullable = learningDeliveryFAMDateFrom,
                LearnDelFAMDateToNullable = learningDeliveryFAMDateTo
            };
        }

        public ICollection<ILearningDeliveryFAM> BuildLearningDeliveryACTValues(DateTime? learnActEndDate, ICollection<ILearningDeliveryFAM> learningDeliveryFAMs, ReportTotals reportTotals, IReadOnlyDictionary<int, DateTime?> censusEndDates)
        {
            if (learnActEndDate.HasValue)
            {
                return learningDeliveryFAMs?.Where(f => learnActEndDate.Value == f.LearnDelFAMDateToNullable).ToList();                    
            }

            var endDates = new DateTime?[]
            {
                reportTotals.AugustTotal.HasValue ? censusEndDates[1] : null,
                reportTotals.SeptemberTotal.HasValue ? censusEndDates[2] : null,
                reportTotals.OctoberTotal.HasValue ? censusEndDates[3] : null,
                reportTotals.NovemberTotal.HasValue ? censusEndDates[4] : null,
                reportTotals.DecemberTotal.HasValue ? censusEndDates[5] : null,
                reportTotals.JanuaryTotal.HasValue ? censusEndDates[6] : null,
                reportTotals.FebruaryTotal.HasValue ? censusEndDates[7] : null,
                reportTotals.MarchTotal.HasValue ? censusEndDates[8] : null,
                reportTotals.AprilTotal.HasValue ? censusEndDates[9] : null,
                reportTotals.MayTotal.HasValue ? censusEndDates[10] : null,
                reportTotals.JuneTotal.HasValue ? censusEndDates[11] : null,
                reportTotals.JulyTotal.HasValue ? censusEndDates[12] : null
            };

             var fams = endDates.Where(e => e != null).SelectMany(x => learningDeliveryFAMs.Where(l => l.LearnDelFAMDateFromNullable <= x && l.LearnDelFAMDateToNullable >= x)) // Closed Fams
                .Union(endDates.Where(e => e != null).SelectMany(x => learningDeliveryFAMs.Where(l => l.LearnDelFAMDateFromNullable <= x && !l.LearnDelFAMDateToNullable.HasValue))) // Open ended Fams
                .Distinct().ToList();

            return fams;
        }

        public IEnumerable<FM36LearnerData> BuildFm36Learners(IMessage message, FM36Global fm36Data, IDictionary<string, Dictionary<int, string>> fcsContractsDictionary, IDictionary<string, string[]> validContractsDictionary)
        {
            var learnerData = new List<FM36LearnerData>();
            var messageLearnerDictionary = BuildLearnerDictionary(message);
            var messageLearningDeliveryDictionary = BuildFm36LearningDeliveryDictionary(message);

            return fm36Data?.Learners?
                .Select(l => new FM36LearnerData
                {
                    Learner = messageLearnerDictionary[l.LearnRefNumber],
                    ProviderSpecLearnerMonitoringModels = _ilrModelMapper.MapProviderSpecLearnerMonitorings(messageLearnerDictionary[l.LearnRefNumber]?.ProviderSpecLearnerMonitorings),
                    LearningDeliveries = l.LearningDeliveries?.Select(ld => new FM36LearningDeliveryData
                    {
                        LearningDelivery = messageLearningDeliveryDictionary[l.LearnRefNumber][ld.AimSeqNumber],
                        FundingStreamPeriodCode = fcsContractsDictionary[l.LearnRefNumber][ld.AimSeqNumber],
                        ProviderSpecDeliveryMonitoringModels = _ilrModelMapper.MapProviderSpecDeliveryMonitorings(messageLearningDeliveryDictionary[l.LearnRefNumber][ld.AimSeqNumber]?.ProviderSpecDeliveryMonitorings),
                        LearningDeliveryFAMsModels = _ilrModelMapper.MapLearningDeliveryFAMs(messageLearningDeliveryDictionary[l.LearnRefNumber][ld.AimSeqNumber]?.LearningDeliveryFAMs),
                        LearningDeliveryFAMs_ACT = messageLearningDeliveryDictionary[l.LearnRefNumber][ld.AimSeqNumber]?.LearningDeliveryFAMs?.Where(fam => fam.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ACT).ToList(),
                        FM36PriceEpisodes =
                            l.PriceEpisodes?.Where(p => p.PriceEpisodeValues?.PriceEpisodeAimSeqNumber == ld.AimSeqNumber)
                            .Select(p => BuildNonContractedPriceEpisode(p, l.LearnRefNumber, fcsContractsDictionary[l.LearnRefNumber][ld.AimSeqNumber], validContractsDictionary)).ToList(),
                        FM36LearningDelivery = BuildNonContractedLearningDelivery(ld, l.LearnRefNumber, fcsContractsDictionary[l.LearnRefNumber][ld.AimSeqNumber], validContractsDictionary)
                    }).ToList()
                }).ToList();
        }

        public FM36PriceEpisodeValue BuildNonContractedPriceEpisode(PriceEpisode priceEpisode, string learnRefNumber, string fundingStreamPeriodCode, IDictionary<string, string[]> validContractsDictionary)
        {
            if (priceEpisode?.PriceEpisodeValues.PriceEpisodeFundLineType != null)
            {
                var fundLineType = validContractsDictionary.GetValueOrDefault(priceEpisode?.PriceEpisodeValues.PriceEpisodeFundLineType);

                if (fundLineType != null && !fundLineType.Contains(fundingStreamPeriodCode))
                {
                    return new FM36PriceEpisodeValue
                    {
                        PriceEpisodeValue = priceEpisode?.PriceEpisodeValues,
                        FundLineValues = new FundLineValue
                        {
                            FundLineType = priceEpisode?.PriceEpisodeValues.PriceEpisodeFundLineType,
                            ReportTotals = BuildPriceEpisodeReportTotals(priceEpisode.PriceEpisodePeriodisedValues, learnRefNumber, priceEpisode.PriceEpisodeValues.PriceEpisodeAimSeqNumber.Value)
                        }
                    };
                }
            }

            return null;
        }

        public FM36LearningDeliveryValue BuildNonContractedLearningDelivery(LearningDelivery learningDelivery, string learnRefNumber, string fundingStreamPeriodCode, IDictionary<string, string[]> validContractsDictionary)
        {
            var fundlinesDctionary = BuildNonContractedFundLinesDictionary(learningDelivery?.LearningDeliveryPeriodisedTextValues, learnRefNumber, learningDelivery.AimSeqNumber, fundingStreamPeriodCode, validContractsDictionary);
            var totals = BuildLearningDeliveryReportTotals(learningDelivery?.LearningDeliveryPeriodisedValues, learnRefNumber, learningDelivery.AimSeqNumber);

            var fundlinesToReturn = new List<FundLineValue>();

            foreach (var fl in fundlinesDctionary)
            {
                fundlinesToReturn.Add(new FundLineValue
                {
                    FundLineType = fl.Key,
                    ReportTotals = new ReportTotals
                    {
                        LearnRefNumber = learnRefNumber,
                        AimSeqNumber = learningDelivery.AimSeqNumber,
                        AugustTotal = fl.Value.AugustFundLine != null ? totals.AugustTotal : 0m,
                        SeptemberTotal = fl.Value.SeptemberFundLine != null ? totals.SeptemberTotal : 0m,
                        OctoberTotal = fl.Value.OctoberFundLine != null ? totals.OctoberTotal : 0m,
                        NovemberTotal = fl.Value.NovemberFundLine != null ? totals.NovemberTotal : 0m,
                        DecemberTotal = fl.Value.DecemberFundLine != null ? totals.DecemberTotal : 0m,
                        JanuaryTotal = fl.Value.JanuaryFundLine != null ? totals.JanuaryTotal : 0m,
                        FebruaryTotal = fl.Value.FebruaryFundLine != null ? totals.FebruaryTotal : 0m,
                        MarchTotal = fl.Value.MarchFundLine != null ? totals.MarchTotal : 0m,
                        AprilTotal = fl.Value.AprilFundLine != null ? totals.AprilTotal : 0m,
                        MayTotal = fl.Value.MarchFundLine != null ? totals.MayTotal : 0m,
                        JuneTotal = fl.Value.JuneFundLine != null ? totals.JuneTotal : 0m,
                        JulyTotal = fl.Value.JulyFundLine != null ? totals.JulyTotal : 0m,
                    }
                });
            }

            return new FM36LearningDeliveryValue
            {
                LearningDeliveryValues = learningDelivery.LearningDeliveryValues,
                FundLineValues = fundlinesToReturn
            };
        }

        public IDictionary<string, FundLines> BuildNonContractedFundLinesDictionary(IEnumerable<LearningDeliveryPeriodisedTextValues> learningDeliveryPeriodisedValues, string learnRefNumber, int aimSeqNumber, string fundingStreamPeriodCode, IDictionary<string, string[]> validContractsDictionary)
        {
            var fundlines = learningDeliveryPeriodisedValues?
                .Where(a => a.AttributeName == AttributeConstants.Fm36FundLineType)
                .Select(pv => new FundLines
                {
                    LearnRefNumber = learnRefNumber,
                    AimSeqNumber = aimSeqNumber,
                    AugustFundLine = GetNonContractedFundLine(pv?.Period1, fundingStreamPeriodCode, validContractsDictionary),
                    SeptemberFundLine = GetNonContractedFundLine(pv?.Period2, fundingStreamPeriodCode, validContractsDictionary),
                    OctoberFundLine = GetNonContractedFundLine(pv?.Period3, fundingStreamPeriodCode, validContractsDictionary),
                    NovemberFundLine = GetNonContractedFundLine(pv?.Period4, fundingStreamPeriodCode, validContractsDictionary),
                    DecemberFundLine = GetNonContractedFundLine(pv?.Period5, fundingStreamPeriodCode, validContractsDictionary),
                    JanuaryFundLine = GetNonContractedFundLine(pv?.Period6, fundingStreamPeriodCode, validContractsDictionary),
                    FebruaryFundLine = GetNonContractedFundLine(pv?.Period7, fundingStreamPeriodCode, validContractsDictionary),
                    MarchFundLine = GetNonContractedFundLine(pv?.Period8, fundingStreamPeriodCode, validContractsDictionary),
                    AprilFundLine = GetNonContractedFundLine(pv?.Period9, fundingStreamPeriodCode, validContractsDictionary),
                    MayFundLine = GetNonContractedFundLine(pv?.Period10, fundingStreamPeriodCode, validContractsDictionary),
                    JuneFundLine = GetNonContractedFundLine(pv?.Period11, fundingStreamPeriodCode, validContractsDictionary),
                    JulyFundLine = GetNonContractedFundLine(pv?.Period12, fundingStreamPeriodCode, validContractsDictionary),
                }).FirstOrDefault();

            return new List<string>
            {
                fundlines.AugustFundLine,
                fundlines.SeptemberFundLine,
                fundlines.OctoberFundLine,
                fundlines.NovemberFundLine,
                fundlines.DecemberFundLine,
                fundlines.JanuaryFundLine,
                fundlines.FebruaryFundLine,
                fundlines.MarchFundLine,
                fundlines.AprilFundLine,
                fundlines.MayFundLine,
                fundlines.JuneFundLine,
                fundlines.JulyFundLine
            }.Where(x => !string.IsNullOrWhiteSpace(x))
            .GroupBy(x => x)
            .ToDictionary(
                k => k.Key,
                v => v.Select(fundline => new FundLines
                {
                    LearnRefNumber = learnRefNumber,
                    AimSeqNumber = aimSeqNumber,
                    AugustFundLine = fundlines.AugustFundLine == fundline ? fundlines.AugustFundLine : null,
                    SeptemberFundLine = fundlines.SeptemberFundLine == fundline ? fundlines.SeptemberFundLine : null,
                    OctoberFundLine = fundlines.OctoberFundLine == fundline ? fundlines.OctoberFundLine : null,
                    NovemberFundLine = fundlines.NovemberFundLine == fundline ? fundlines.NovemberFundLine : null,
                    DecemberFundLine = fundlines.DecemberFundLine == fundline ? fundlines.DecemberFundLine : null,
                    JanuaryFundLine = fundlines.JanuaryFundLine == fundline ? fundlines.JanuaryFundLine : null,
                    FebruaryFundLine = fundlines.FebruaryFundLine == fundline ? fundlines.FebruaryFundLine : null,
                    MarchFundLine = fundlines.MarchFundLine == fundline ? fundlines.MarchFundLine : null,
                    AprilFundLine = fundlines.AprilFundLine == fundline ? fundlines.AprilFundLine : null,
                    MayFundLine = fundlines.MayFundLine == fundline ? fundlines.MayFundLine : null,
                    JuneFundLine = fundlines.JuneFundLine == fundline ? fundlines.JuneFundLine : null,
                    JulyFundLine = fundlines.JulyFundLine == fundline ? fundlines.JulyFundLine : null,
                }).FirstOrDefault(), StringComparer.OrdinalIgnoreCase);
        }

        public FundLines BuildNonContractedFundLines(IEnumerable<LearningDeliveryPeriodisedTextValues> learningDeliveryPeriodisedValues, string learnRefNumber, int aimSeqNumber, string fundingStreamPeriodCode, IDictionary<string, string[]> validContractsDictionary)
        {
            return learningDeliveryPeriodisedValues?
                .Where(a => a.AttributeName == AttributeConstants.Fm36FundLineType)
                .Select(pv => new FundLines
                {
                    LearnRefNumber = learnRefNumber,
                    AimSeqNumber = aimSeqNumber,
                    AugustFundLine = GetNonContractedFundLine(pv?.Period1, fundingStreamPeriodCode, validContractsDictionary),
                    SeptemberFundLine = GetNonContractedFundLine(pv?.Period2, fundingStreamPeriodCode, validContractsDictionary),
                    OctoberFundLine = GetNonContractedFundLine(pv?.Period3, fundingStreamPeriodCode, validContractsDictionary),
                    NovemberFundLine = GetNonContractedFundLine(pv?.Period4, fundingStreamPeriodCode, validContractsDictionary),
                    DecemberFundLine = GetNonContractedFundLine(pv?.Period5, fundingStreamPeriodCode, validContractsDictionary),
                    JanuaryFundLine = GetNonContractedFundLine(pv?.Period6, fundingStreamPeriodCode, validContractsDictionary),
                    FebruaryFundLine = GetNonContractedFundLine(pv?.Period7, fundingStreamPeriodCode, validContractsDictionary),
                    MarchFundLine = GetNonContractedFundLine(pv?.Period8, fundingStreamPeriodCode, validContractsDictionary),
                    AprilFundLine = GetNonContractedFundLine(pv?.Period9, fundingStreamPeriodCode, validContractsDictionary),
                    MayFundLine = GetNonContractedFundLine(pv?.Period10, fundingStreamPeriodCode, validContractsDictionary),
                    JuneFundLine = GetNonContractedFundLine(pv?.Period11, fundingStreamPeriodCode, validContractsDictionary),
                    JulyFundLine = GetNonContractedFundLine(pv?.Period12, fundingStreamPeriodCode, validContractsDictionary),
                }).FirstOrDefault();
        }

        public string GetNonContractedFundLine(string periodValue, string fundingStreamPeriodCode, IDictionary<string, string[]> validContractsDictionary)
        {
            if (periodValue != null && periodValue != "None")
            {
                var fspCodes = validContractsDictionary.GetValueOrDefault(periodValue);

                return fspCodes != null && !fspCodes.Contains(fundingStreamPeriodCode, StringComparer.OrdinalIgnoreCase) ? periodValue : string.Empty;
            }

            return string.Empty;
        }

        public ReportTotals BuildPriceEpisodeReportTotals(IEnumerable<PriceEpisodePeriodisedValues> priceEpisodePeriodisedValues, string learnRefNumber, int aimSeqNumber)
        {
            var periodisedValues = priceEpisodePeriodisedValues?.Where(a => _priceEpisodeFundedAttributes.Contains(a.AttributeName))?
                .Select(p => new decimal?[]
                {
                    p.Period1,
                    p.Period2,
                    p.Period3,
                    p.Period4,
                    p.Period5,
                    p.Period6,
                    p.Period7,
                    p.Period8,
                    p.Period9,
                    p.Period10,
                    p.Period11,
                    p.Period12,
                }).ToArray();

            return SumPeriods(periodisedValues, learnRefNumber, aimSeqNumber);
        }

        public ReportTotals BuildLearningDeliveryReportTotals(IEnumerable<LearningDeliveryPeriodisedValues> learningDeliveryPeriodisedValues, string learnRefNumber, int aimSeqNumber)
        {
            var periodisedValues = learningDeliveryPeriodisedValues?.Where(a => _learningDeliveryFundedAttributes.Contains(a.AttributeName))?
               .Select(p => new decimal?[]
               {
                    p.Period1,
                    p.Period2,
                    p.Period3,
                    p.Period4,
                    p.Period5,
                    p.Period6,
                    p.Period7,
                    p.Period8,
                    p.Period9,
                    p.Period10,
                    p.Period11,
                    p.Period12,
               }).ToArray();

            return SumPeriods(periodisedValues, learnRefNumber, aimSeqNumber);
        }

        public ReportTotals SumPeriods(decimal?[][] periodisedValues, string learnRefNumber, int aimSeqNumber)
        {
            return new ReportTotals
            {
                LearnRefNumber = learnRefNumber,
                AimSeqNumber = aimSeqNumber,
                AugustTotal = periodisedValues?.Sum(p => p[0]) ?? 0m,
                SeptemberTotal = periodisedValues?.Sum(p => p[1]) ?? 0m,
                OctoberTotal = periodisedValues?.Sum(p => p[2]) ?? 0m,
                NovemberTotal = periodisedValues?.Sum(p => p[3]) ?? 0m,
                DecemberTotal = periodisedValues?.Sum(p => p[4]) ?? 0m,
                JanuaryTotal = periodisedValues?.Sum(p => p[5]) ?? 0m,
                FebruaryTotal = periodisedValues?.Sum(p => p[6]) ?? 0m,
                MarchTotal = periodisedValues?.Sum(p => p[7]) ?? 0m,
                AprilTotal = periodisedValues?.Sum(p => p[8]) ?? 0m,
                MayTotal = periodisedValues?.Sum(p => p[9]) ?? 0m,
                JuneTotal = periodisedValues.Sum(p => p[10]) ?? 0m,
                JulyTotal = periodisedValues.Sum(p => p[11]) ?? 0m,
            };
        }

        public IDictionary<string, LARSLearningDelivery> BuildLARSDictionary(IReadOnlyCollection<LARSLearningDelivery> larsLearningDeliveries)
        {
            return larsLearningDeliveries.ToDictionary(k => k.LearnAimRef, v => v, StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, string[]> BuildValidContractMapping()
        {
            var contractsDictionary = new Dictionary<string, string[]>();

            foreach (var keyValuePair in ValidContractMappings)
            {
                contractsDictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return contractsDictionary;
        }

        public IDictionary<string, Dictionary<int, string>> BuildFcsContractMapping(IEnumerable<FcsContractAllocation> fcsContractAllocations, IMessage message)
        {
            var conRefNumbers = new HashSet<string>(
                message?.Learners?.SelectMany(l => l.LearningDeliveries
                .Where(ld => ld.FundModel == FundModelConstants.FM36)
                .Select(ld => ld.ConRefNumber)), StringComparer.OrdinalIgnoreCase);

            var periodCodesDictionary =
                fcsContractAllocations?
                .Where(f => conRefNumbers.Contains(f.ContractAllocationNumber))
                .ToDictionary(k => k.ContractAllocationNumber, v => v.FundingStreamPeriodCode, StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, string>();

            return message?.Learners?
                .ToDictionary(
                    l => l.LearnRefNumber,
                    l => l.LearningDeliveries.Where(fm => fm.FundModel == FundModelConstants.FM36)
                    .GroupBy(a => a.AimSeqNumber)
                    .ToDictionary(
                        k => k.Key,
                        v => v.Select(ld => ld.ConRefNumber == null ? null : periodCodesDictionary[ld.ConRefNumber]).FirstOrDefault()), StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, Dictionary<int, string>>();
        }

        public IDictionary<string, Dictionary<int, ILearningDelivery>> BuildFm36LearningDeliveryDictionary(IMessage message)
        {
            return message?.Learners?
                .ToDictionary(
                    l => l.LearnRefNumber,
                    l => l.LearningDeliveries.Where(fm => fm.FundModel == FundModelConstants.FM36)
                    .GroupBy(a => a.AimSeqNumber)
                    .ToDictionary(
                        k => k.Key,
                        v => v.Select(ld => ld).FirstOrDefault())
                    , StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, Dictionary<int, ILearningDelivery>>();
        }

        public IDictionary<string, ILearner> BuildLearnerDictionary(IMessage message)
        {
            return message.Learners.ToDictionary(l => l.LearnRefNumber, l => l, StringComparer.OrdinalIgnoreCase);
        }
    }
}
