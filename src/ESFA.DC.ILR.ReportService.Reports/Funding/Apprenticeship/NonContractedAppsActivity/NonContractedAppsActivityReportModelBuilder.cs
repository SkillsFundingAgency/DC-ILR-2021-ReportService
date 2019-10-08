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
        public IDictionary<string, string[]> ValidContractsDictionary = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
            { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
            { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
            { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
            { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
            { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
        };

        public IDictionary<string, string> ActCodeContractDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { FundLineConstants.ApprenticeshipEmployerOnAppService1618, "1" },
            { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, "1" },
            { FundLineConstants.NonLevyApprenticeship1618NonProcured, "2" },
            { FundLineConstants.NonLevyApprenticeship1618Procured, "2" },
            { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, "2" },
            { FundLineConstants.NonLevyApprenticeship19PlusProcured, "2" }
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

        private TimeSpan _timeSpanForActFilter = new TimeSpan(0, 23, 59, 59);

        private readonly IAcademicYearService _academicYearService;
        private readonly IIlrModelMapper _ilrModelMapper;

        public NonContractedAppsActivityReportModelBuilder(IAcademicYearService academicYearService, IIlrModelMapper ilrModelMapper)
        {
            _academicYearService = academicYearService;
            _ilrModelMapper = ilrModelMapper;
        }

        public IEnumerable<NonContractedAppsActivityReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm36Data = reportServiceDependentData.Get<FM36Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var censusEndDates = referenceData.MetaDatas.CollectionDates.CensusDates.ToDictionary(p => p.Period, e => (DateTime?)e.End);
            var larsLearningDeliveryDictionary = BuildLARSDictionary(referenceData.LARSLearningDeliveries);
            var fundingStreamPeriodCodesForUkprn = BuildFcsFundingStreamPeriodCodes(referenceData.FCSContractAllocations);

            var fm36LearnerModel = BuildFm36Learners(message, fm36Data, fundingStreamPeriodCodesForUkprn);

            var reportRows = BuildReportRows(fm36LearnerModel, larsLearningDeliveryDictionary, censusEndDates);

            return reportRows;
        }

        public IEnumerable<NonContractedAppsActivityReportModel> BuildReportRows(
            IEnumerable<FM36LearnerData> fm36LearnerData,
            IDictionary<string, LARSLearningDelivery> larsDictionary,
            IReadOnlyDictionary<int, DateTime?> censusEndDates)
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
                               learningDelivery.FM36LearningDelivery?.FundLineValues.SelectMany(fundlineValue =>
                               BuildLearningDeliveryACTValues(learningDelivery.LearnActEndDate, learningDelivery.LearningDeliveryFAMs_ACT, fundlineValue, censusEndDates, ActCodeContractDictionary[fundlineValue.FundLineType])
                               .Select(ldFamAct =>
                                 new NonContractedAppsActivityReportModel
                                 {
                                     Learner = learner.Learner,
                                     ProviderSpecLearnerMonitoring = learner.ProviderSpecLearnerMonitoringModels,
                                     LarsLearningDelivery = larsDictionary[learningDelivery.LearnAimRef],
                                     ProviderSpecDeliveryMonitoring = learningDelivery.ProviderSpecDeliveryMonitoringModels,
                                     LearningDeliveryFAMs = learningDelivery.LearningDeliveryFAMsModels,
                                     LearningDelivery = learningDelivery.LearningDelivery,
                                     FundingLineType = fundlineValue.FundLineType,
                                     Fm36LearningDelivery = learningDelivery.FM36LearningDelivery?.LearningDeliveryValues,
                                     AugustTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[1]) ? fundlineValue.ReportTotals.AugustTotal : 0m,
                                     SeptemberTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[2]) ? fundlineValue.ReportTotals.SeptemberTotal : 0m,
                                     OctoberTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[3]) ? fundlineValue.ReportTotals.OctoberTotal : 0m,
                                     NovemberTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[4]) ? fundlineValue.ReportTotals.NovemberTotal : 0m,
                                     DecemberTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[5]) ? fundlineValue.ReportTotals.DecemberTotal : 0m,
                                     JanuaryTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[6]) ? fundlineValue.ReportTotals.JanuaryTotal : 0m,
                                     FebruaryTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[7]) ? fundlineValue.ReportTotals.FebruaryTotal : 0m,
                                     MarchTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[8]) ? fundlineValue.ReportTotals.MarchTotal : 0m,
                                     AprilTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[9]) ? fundlineValue.ReportTotals.AprilTotal : 0m,
                                     MayTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[10]) ? fundlineValue.ReportTotals.MayTotal : 0m,
                                     JuneTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[11]) ? fundlineValue.ReportTotals.JuneTotal : 0m,
                                     JulyTotal = ReportRowTotalApplicable(ldFamAct, censusEndDates[12]) ? fundlineValue.ReportTotals.JulyTotal : 0m,
                                     LearningDeliveryFAM_ACTs = ldFamAct
                                 })));
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
                                    Fm36LearningDelivery = learningDelivery.FM36LearningDelivery?.LearningDeliveryValues,
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
                                }));
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

            return learningDeliveryFAM.LearnDelFAMDateFromNullable <= censusDate && learningDeliveryFAM.LearnDelFAMDateToNullable.Value.Add(_timeSpanForActFilter) >= censusDate;
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

        public ICollection<ILearningDeliveryFAM> BuildLearningDeliveryACTValues(
            DateTime? learnActEndDate,
            ICollection<ILearningDeliveryFAM> learningDeliveryFAMs,
            FundLineValue fundLineValue,
            IReadOnlyDictionary<int, DateTime?> censusEndDates,
            string actCode)
        {
            if (learnActEndDate.HasValue)
            {
                return learningDeliveryFAMs?
                    .Where(f => 
                    actCode.CaseInsensitiveEquals(f.LearnDelFAMCode) &&
                    learnActEndDate.Value == f.LearnDelFAMDateToNullable)
                    .ToList();
            }

            var endDates = new DateTime?[]
            {
                fundLineValue.ReportTotals.AugustTotal.HasValue ? censusEndDates[1] : null,
                fundLineValue.ReportTotals.SeptemberTotal.HasValue ? censusEndDates[2] : null,
                fundLineValue.ReportTotals.OctoberTotal.HasValue ? censusEndDates[3] : null,
                fundLineValue.ReportTotals.NovemberTotal.HasValue ? censusEndDates[4] : null,
                fundLineValue.ReportTotals.DecemberTotal.HasValue ? censusEndDates[5] : null,
                fundLineValue.ReportTotals.JanuaryTotal.HasValue ? censusEndDates[6] : null,
                fundLineValue.ReportTotals.FebruaryTotal.HasValue ? censusEndDates[7] : null,
                fundLineValue.ReportTotals.MarchTotal.HasValue ? censusEndDates[8] : null,
                fundLineValue.ReportTotals.AprilTotal.HasValue ? censusEndDates[9] : null,
                fundLineValue.ReportTotals.MayTotal.HasValue ? censusEndDates[10] : null,
                fundLineValue.ReportTotals.JuneTotal.HasValue ? censusEndDates[11] : null,
                fundLineValue.ReportTotals.JulyTotal.HasValue ? censusEndDates[12] : null
            };

            var fams =
               endDates.Where(e => e != null).SelectMany(x => learningDeliveryFAMs
               .Where(l => (l.LearnDelFAMDateFromNullable <= x && l.LearnDelFAMDateToNullable >= x) // Closed Fams
               | (l.LearnDelFAMDateFromNullable <= x && !l.LearnDelFAMDateToNullable.HasValue))) // Closed Fams
              .Where(f => actCode.CaseInsensitiveEquals(f.LearnDelFAMCode))
              .Distinct()
              .ToList();

            return fams;
        }

        public IEnumerable<FM36LearnerData> BuildFm36Learners(IMessage message, FM36Global fm36Data, ICollection<string> fundingStreamPeriodCodes)
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
                        ProviderSpecDeliveryMonitoringModels = _ilrModelMapper.MapProviderSpecDeliveryMonitorings(messageLearningDeliveryDictionary[l.LearnRefNumber][ld.AimSeqNumber]?.ProviderSpecDeliveryMonitorings),
                        LearningDeliveryFAMsModels = _ilrModelMapper.MapLearningDeliveryFAMs(messageLearningDeliveryDictionary[l.LearnRefNumber][ld.AimSeqNumber]?.LearningDeliveryFAMs),
                        LearningDeliveryFAMs_ACT = messageLearningDeliveryDictionary[l.LearnRefNumber][ld.AimSeqNumber]?.LearningDeliveryFAMs?.Where(fam => fam.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ACT).ToList(),
                        FM36PriceEpisodes =
                            l.PriceEpisodes?.Where(p => PriceEpisodeFilter(p.PriceEpisodeValues, ld.AimSeqNumber))
                            .Select(p => BuildNonContractedPriceEpisode(p, l.LearnRefNumber, fundingStreamPeriodCodes)).Where(pe => pe != null).ToList(),
                        FM36LearningDelivery = BuildNonContractedLearningDelivery(ld, l.LearnRefNumber, fundingStreamPeriodCodes)
                    }).ToList()
                }).ToList()
                ?? Enumerable.Empty<FM36LearnerData>();
        }

        public bool PriceEpisodeFilter(PriceEpisodeValues priceEpisodeValues, int aimSeqNumber)
        {
            if (priceEpisodeValues == null)
            {
                return false;
            }

            return priceEpisodeValues.PriceEpisodeAimSeqNumber == aimSeqNumber &&
                _academicYearService.YearStart <= priceEpisodeValues.EpisodeStartDate && _academicYearService.YearEnd >= priceEpisodeValues.EpisodeStartDate;
        }

        public FM36PriceEpisodeValue BuildNonContractedPriceEpisode(PriceEpisode priceEpisode, string learnRefNumber, ICollection<string> fundingStreamPeriodCodes)
        {
            if (priceEpisode?.PriceEpisodeValues.PriceEpisodeFundLineType != null)
            {
                var fspCodesForFundLineType = ValidContractsDictionary.GetValueOrDefault(priceEpisode?.PriceEpisodeValues.PriceEpisodeFundLineType);

                if (fspCodesForFundLineType != null && !fspCodesForFundLineType.Any(x => fundingStreamPeriodCodes.Contains(x)))
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

        public FM36LearningDeliveryValue BuildNonContractedLearningDelivery(LearningDelivery learningDelivery, string learnRefNumber, ICollection<string> fundingStreamPeriodCodes)
        {
            var fundlinesDctionary = BuildNonContractedFundLinesDictionary(learningDelivery?.LearningDeliveryPeriodisedTextValues, learnRefNumber, learningDelivery.AimSeqNumber, fundingStreamPeriodCodes);
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
                        MayTotal = fl.Value.MayFundLine != null ? totals.MayTotal : 0m,
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

        public IDictionary<string, FundLines> BuildNonContractedFundLinesDictionary(IEnumerable<LearningDeliveryPeriodisedTextValues> learningDeliveryPeriodisedValues, string learnRefNumber, int aimSeqNumber, ICollection<string> fundingStreamPeriodCodes)
        {
            var fundlines = learningDeliveryPeriodisedValues?
                .Where(a => a.AttributeName == AttributeConstants.Fm36FundLineType)
                .Select(pv => new FundLines
                {
                    LearnRefNumber = learnRefNumber,
                    AimSeqNumber = aimSeqNumber,
                    AugustFundLine = GetNonContractedFundLine(pv?.Period1, fundingStreamPeriodCodes),
                    SeptemberFundLine = GetNonContractedFundLine(pv?.Period2, fundingStreamPeriodCodes),
                    OctoberFundLine = GetNonContractedFundLine(pv?.Period3, fundingStreamPeriodCodes),
                    NovemberFundLine = GetNonContractedFundLine(pv?.Period4, fundingStreamPeriodCodes),
                    DecemberFundLine = GetNonContractedFundLine(pv?.Period5, fundingStreamPeriodCodes),
                    JanuaryFundLine = GetNonContractedFundLine(pv?.Period6, fundingStreamPeriodCodes),
                    FebruaryFundLine = GetNonContractedFundLine(pv?.Period7, fundingStreamPeriodCodes),
                    MarchFundLine = GetNonContractedFundLine(pv?.Period8, fundingStreamPeriodCodes),
                    AprilFundLine = GetNonContractedFundLine(pv?.Period9, fundingStreamPeriodCodes),
                    MayFundLine = GetNonContractedFundLine(pv?.Period10, fundingStreamPeriodCodes),
                    JuneFundLine = GetNonContractedFundLine(pv?.Period11, fundingStreamPeriodCodes),
                    JulyFundLine = GetNonContractedFundLine(pv?.Period12, fundingStreamPeriodCodes),
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

        public string GetNonContractedFundLine(string periodValue, ICollection<string> fundingStreamPeriodCodes)
        {
            if (periodValue != null && periodValue != "None")
            {
                var fspCodes = ValidContractsDictionary.GetValueOrDefault(periodValue);

                return fspCodes != null && !fspCodes.Any(x => fundingStreamPeriodCodes.Contains(x)) ? periodValue : string.Empty;
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

        public ICollection<string> BuildFcsFundingStreamPeriodCodes(IEnumerable<FcsContractAllocation> fcsContractAllocations)
        {
            return new HashSet<string>(fcsContractAllocations?
                .Select(f => f.FundingStreamPeriodCode).Distinct().ToList(), StringComparer.OrdinalIgnoreCase);
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
            return message?.Learners?.ToDictionary(l => l.LearnRefNumber, l => l, StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, ILearner>();
        }
    }
}
