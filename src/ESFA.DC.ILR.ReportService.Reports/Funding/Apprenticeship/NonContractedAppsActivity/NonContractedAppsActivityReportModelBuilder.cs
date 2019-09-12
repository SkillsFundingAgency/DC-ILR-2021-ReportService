using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.FCS;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
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
            new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.C1618nlap2018 }),
            new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 }),
            new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 })
        };

        private ICollection<string> _learningDeliveryFundedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment, AttributeConstants.Fm36LearnSuppFundCash };

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

            var validContractsDictionary = BuildValidContractMapping();
            var fcsContractsDictionary = BuildFcsContractMapping(referenceData.FCSContractAllocations, message);
            var fm36MessageDictionary = BuildFm36MessageDictionary(message);           
            var fm36Dictionary = BuildFm36LearnerDeliveryDictionary(fm36Data);
            // var fm36ContractsDictionary = BuildFm36LearnersContractMapping(message, fm36Dictionary, referenceData.FCSContractAllocations);

            IDictionary<string, LARSLearningDelivery> larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            IDictionary<string, FM36LearningDeliveryTotals> nonContractedLearningDeliveries = BuildNonContractedLearningDeliveryDictionary(fm36Data, fm36MessageDictionary, fcsContractsDictionary, validContractsDictionary);

            throw new NotImplementedException();
        }

        public IDictionary<string, FM36LearningDeliveryTotals> BuildNonContractedLearningDeliveryDictionary(FM36Global global, IDictionary<string, Dictionary<int, ILearningDelivery>> fm36MessageDictionary, IDictionary<string, string> fcsContractsDictionary, IDictionary<string, string[]> validContracts)
        {
            var nonContractedLearningDeliveries = new List<FM36LearningDeliveryTotals>();

            var periodisedTextValuesForFundLines = global?.Learners?
                .SelectMany(l => l.LearningDeliveries?
                .Where(ldv => ldv.LearningDeliveryValues.LearnDelMathEng == true)
                .SelectMany(ld => ld.LearningDeliveryPeriodisedTextValues
                .Where(ldpt => ldpt.AttributeName == AttributeConstants.Fm36FundLineType)
                .Select(x => new FM36LearningDeliveryFundLine
                {
                    LearnRefNumber = l.LearnRefNumber,
                    AimSeqNumber = ld.AimSeqNumber,
                    ConRefNumber = fm36MessageDictionary[l.LearnRefNumber][ld.AimSeqNumber].ConRefNumber,
                    AppAdjLearnStartDate = ld.LearningDeliveryValues.AppAdjLearnStartDate,
                    AgeAtProgStart = ld.LearningDeliveryValues.AgeAtProgStart,
                    LearnDelMathEng = ld.LearningDeliveryValues.LearnDelMathEng,
                    Period1 = x.Period1,
                    Period2 = x.Period2,
                    Period3 = x.Period3,
                    Period4 = x.Period4,
                    Period5 = x.Period5,
                    Period6 = x.Period6,
                    Period7 = x.Period7,
                    Period8 = x.Period8,
                    Period9 = x.Period9,
                    Period10 = x.Period10,
                    Period11 = x.Period11,
                    Period12 = x.Period12,
                }).SelectMany(PivotPeriods)));

            var learningDeliveryPeriodisedValues = global?.Learners?
               .SelectMany(l => l.LearningDeliveries?
               .Where(ldv => ldv.LearningDeliveryValues.LearnDelMathEng == true)
               .SelectMany(ld => ld.LearningDeliveryPeriodisedValues
               .Where(ldpv => _learningDeliveryFundedAttributes.Contains(ldpv.AttributeName))
               .Select(x => new FM36LearningDelivery
               {
                   LearnRefNumber = l.LearnRefNumber,
                   AimSeqNumber = ld.AimSeqNumber,
                   AppAdjLearnStartDate = ld.LearningDeliveryValues.AppAdjLearnStartDate,
                   AgeAtProgStart = ld.LearningDeliveryValues.AgeAtProgStart,
                   LearnDelMathEng = ld.LearningDeliveryValues.LearnDelMathEng,
                   Period1 = x.Period1,
                   Period2 = x.Period2,
                   Period3 = x.Period3,
                   Period4 = x.Period4,
                   Period5 = x.Period5,
                   Period6 = x.Period6,
                   Period7 = x.Period7,
                   Period8 = x.Period8,
                   Period9 = x.Period9,
                   Period10 = x.Period10,
                   Period11 = x.Period11,
                   Period12 = x.Period12,
               })))
               .GroupBy(lrn => lrn.LearnRefNumber)
               .ToDictionary(
                   k1 => k1.Key,
                   v1 => v1.Select(x => x)
                   .GroupBy(asn => asn.AimSeqNumber)
                   .ToDictionary(
                       k2 => k2.Key,
                       v2 => new FM36LearningDeliveryTotals
                       {
                           LearnRefNumber = v2.FirstOrDefault().LearnRefNumber,
                           AimSeqNumber = v2.FirstOrDefault().AimSeqNumber,
                           AppAdjLearnStartDate = v2.FirstOrDefault().AppAdjLearnStartDate,
                           AgeAtProgStart = v2.FirstOrDefault().AgeAtProgStart,
                           LearnDelMathEng = v2.FirstOrDefault().LearnDelMathEng,
                           AugustTotal = v2.Sum(p => p.Period1) ?? 0m,
                           SeptemberTotal = v2.Sum(p => p.Period2) ?? 0m,
                           OctoberTotal = v2.Sum(p => p.Period3) ?? 0m,
                           NovemberTotal = v2.Sum(p => p.Period4) ?? 0m,
                           DecemberTotal = v2.Sum(p => p.Period5) ?? 0m,
                           JanuaryTotal = v2.Sum(p => p.Period6) ?? 0m,
                           FebruaryTotal = v2.Sum(p => p.Period7) ?? 0m,
                           MarchTotal = v2.Sum(p => p.Period8) ?? 0m,
                           AprilTotal = v2.Sum(p => p.Period9) ?? 0m,
                           MayTotal = v2.Sum(p => p.Period10) ?? 0m,
                           JuneTotal = v2.Sum(p => p.Period11) ?? 0m,
                           JulyTotal = v2.Sum(p => p.Period12) ?? 0m,
                       })
                   ,StringComparer.OrdinalIgnoreCase);

            foreach (var value in periodisedTextValuesForFundLines.Where(f => f.FundLineType != "None"))
            {
                var fudingStreamPeriodCode = fcsContractsDictionary[value.ConRefNumber];

                validContracts.TryGetValue(value.FundLineType, out var fundingStreamPeriodCodes);

                if (fundingStreamPeriodCodes == null || !fundingStreamPeriodCodes.Contains(fudingStreamPeriodCode))
                {
                    nonContractedLearningDeliveries.Add(learningDeliveryPeriodisedValues[value.LearnRefNumber][value.AimSeqNumber]);
                }
            }

            return new Dictionary<string, FM36LearningDeliveryTotals>();
        }

        public IEnumerable<FM36LearningDeliveryFundLineByPeriod> PivotPeriods(FM36LearningDeliveryFundLine delivery)
        {
            var learnRefNumber = delivery.LearnRefNumber;
            var aimSeqNumber = delivery.AimSeqNumber;
            var appAdjLearnStartDate = delivery.AppAdjLearnStartDate;
            var ageAtProgStart = delivery.AgeAtProgStart;
            var conRefNumber = delivery.ConRefNumber;
            var learnDelMathEng = delivery.LearnDelMathEng;

            return new List<FM36LearningDeliveryFundLineByPeriod>()
            {
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 1, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period1),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 2, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period2),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 3, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period3),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 4, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period4),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 5, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period5),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 6, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period6),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 7, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period7),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 8, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period8),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 9, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period9),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 10, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period10),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 11, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period11),
                new FM36LearningDeliveryFundLineByPeriod(learnRefNumber, aimSeqNumber, 12, conRefNumber, appAdjLearnStartDate, ageAtProgStart, learnDelMathEng, delivery.Period12)
            };
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

        public IDictionary<string, string> BuildFcsContractMapping(IEnumerable<FcsContractAllocation> fcsContractAllocations, IMessage message)
        {
            var conRefNumbers = new HashSet<string>(
                message?.Learners?.SelectMany(l => l.LearningDeliveries
                .Where(ld => ld.FundModel == FundModelConstants.FM36)
                .Select(ld => ld.ConRefNumber)), StringComparer.OrdinalIgnoreCase);

            return
                fcsContractAllocations?
                .Where(f => conRefNumbers.Contains(f.ContractAllocationNumber))
                .ToDictionary(k => k.ContractAllocationNumber, v => v.FundingStreamPeriodCode, StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, LARSLearningDelivery> BuildLarsLearningDeliveryDictionary(ReferenceDataRoot referenceDataRoot)
        {
            return referenceDataRoot?.LARSLearningDeliveries?.ToDictionary(ld => ld.LearnAimRef, ld => ld, StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, LARSLearningDelivery>();
        }

        public IDictionary<string, Dictionary<int, ILearningDelivery>> BuildFm36MessageDictionary(IMessage message)
        {
            return message?.Learners?
                .ToDictionary(
                    l => l.LearnRefNumber,
                    l => l.LearningDeliveries.Where(fm => fm.FundModel == FundModelConstants.FM36)
                    .GroupBy(a => a.AimSeqNumber)
                    .ToDictionary(
                        k => k.Key,
                        v => v.Select(ld => ld).FirstOrDefault())
                    , StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, Dictionary<int, ILearningDelivery>>;
        }
        

        public IDictionary<string, FM36Learner> BuildFm36LearnerDeliveryDictionary(FM36Global fm36Global)
        {
            return fm36Global?.Learners?.ToDictionary(ld => ld.LearnRefNumber, ld => ld, StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, FM36Learner>();
        }
    }
}
