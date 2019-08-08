using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive
{
    public class TrailblazerEmployerIncentiveReportModelBuilder : IModelBuilder<IEnumerable<TrailblazerEmployerIncentivesReportModel>>
    {
        protected IEnumerable<FundingDataSources> FundingDataSources { private get; set; }

        public TrailblazerEmployerIncentiveReportModelBuilder()
        {
            FundingDataSources = new[]
            {
                Funding.FundingDataSources.FM81
            };
        }
        public IEnumerable<TrailblazerEmployerIncentivesReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var reportModelList = new List<TrailblazerEmployerIncentivesReportModel>();
            var fm81 = reportServiceDependentData.Get<FM81Global>();

            var employerIds = BuildFm81EmployersHashSet(fm81);
            var periodisedValues = BuildFm81PeriodisedValuesList(fm81);

            foreach (var empId in employerIds)
            {
                reportModelList.Add(new TrailblazerEmployerIncentivesReportModel
                {
                    EmployerIdentifier = empId,
                    AugustSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period1, empId),
                    August1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new [] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period1, empId),
                    AugustAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period1, empId),

                    SeptemberSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period2, empId),
                    September1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period2, empId),
                    SeptemberAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period2, empId),

                    OctoberSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period3, empId),
                    October1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period3, empId),
                    OctoberAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period3, empId),

                    NovemberSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period4, empId),
                    November1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period4, empId),
                    NovemberAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period4, empId),

                    DecemberSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period5, empId),
                    December1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period5, empId),
                    DecemberAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period5, empId),

                    JanuarySmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period6, empId),
                    January1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period6, empId),
                    JanuaryAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period6, empId),

                    FebruarySmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period7, empId),
                    February1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period7, empId),
                    FebruaryAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period7, empId),

                    MarchSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period8, empId),
                    March1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period8, empId),
                    MarchAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period8, empId),

                    AprilSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period9, empId),
                    April1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period9, empId),
                    AprilAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period9, empId),

                    MaySmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period10, empId),
                    May1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period10, empId),
                    MayAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period10, empId),

                    JuneSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period11, empId),
                    June1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period11, empId),
                    JuneAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period11, empId),

                    JulySmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, PeriodConstants.Period12, empId),
                    July1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, PeriodConstants.Period12, empId),
                    JulyAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, PeriodConstants.Period12, empId),
                });
            }

            return reportModelList;
        }

        public decimal CalculatePaymentValue(IEnumerable<TrailblazerLearningDeliveryPeriodisedValues> periodisedValues, string[] attributeConstants, string period, int empId)
        {
            decimal paymentValue = 0.0m;

            foreach (var constant in attributeConstants)
            {
                paymentValue += CalculatePaymentValue(periodisedValues, constant, period, empId);
            }

            return paymentValue;
        }

        public decimal CalculatePaymentValue(IEnumerable<TrailblazerLearningDeliveryPeriodisedValues> periodisedValues, string attributeConstant, string period, int empId)
        {
            return periodisedValues
                .Where(pv =>
                    pv.EmployerIds[attributeConstant].GetValueOrDefault() == empId &&
                    pv.AttributeName == attributeConstant).Sum(s => s.ValuesDictionary[period] ?? 0);
        }

        public HashSet<int> BuildFm81EmployersHashSet(FM81Global fm81Global)
        {
            var employerIdHashSet = new HashSet<int>();

            foreach (var learner in fm81Global?.Learners ?? new List<FM81Learner>())
            {
                if (!learner.LearningDeliveries.Any())
                {
                    continue;
                }

                foreach (var learningDelivery in learner.LearningDeliveries)
                {
                    if (learningDelivery?.LearningDeliveryValues.EmpIdSmallBusDate.HasValue ?? false)
                    {
                        employerIdHashSet.Add(learningDelivery.LearningDeliveryValues.EmpIdSmallBusDate.Value);
                    }

                    if (learningDelivery?.LearningDeliveryValues.EmpIdFirstYoungAppDate.HasValue ?? false)
                    {
                        employerIdHashSet.Add(learningDelivery.LearningDeliveryValues.EmpIdFirstYoungAppDate.Value);
                    }

                    if (learningDelivery?.LearningDeliveryValues.EmpIdSecondYoungAppDate.HasValue ?? false)
                    {
                        employerIdHashSet.Add(learningDelivery.LearningDeliveryValues.EmpIdSecondYoungAppDate.Value);
                    }

                    if (learningDelivery?.LearningDeliveryValues.EmpIdAchDate.HasValue ?? false)
                    {
                        employerIdHashSet.Add(learningDelivery.LearningDeliveryValues.EmpIdAchDate.Value);
                    }
                }
            }

            return employerIdHashSet;
        }

        public IEnumerable<TrailblazerLearningDeliveryPeriodisedValues> BuildFm81PeriodisedValuesList(FM81Global fm81Global)
        {
            return fm81Global
                .Learners?
                .SelectMany(l =>
                    l.LearningDeliveries.SelectMany(ld =>
                        ld.LearningDeliveryPeriodisedValues.Select(ldpv =>
                            new TrailblazerLearningDeliveryPeriodisedValues
                            {
                                EmployerIds = new Dictionary<string, int?>
                                {
                                    { AttributeConstants.Fm81SmallBusPayment, ld.LearningDeliveryValues.EmpIdSmallBusDate },
                                    { AttributeConstants.Fm81YoungAppFirstPayment, ld.LearningDeliveryValues.EmpIdFirstYoungAppDate },
                                    { AttributeConstants.Fm81YoungAppSecondPayment, ld.LearningDeliveryValues.EmpIdSecondYoungAppDate },
                                    { AttributeConstants.Fm81AchPayment, ld.LearningDeliveryValues.EmpIdAchDate }
                                },
                                AttributeName = ldpv.AttributeName,
                                ValuesDictionary = new Dictionary<string, decimal?>
                                {
                                    { PeriodConstants.Period1, ldpv.Period1 },
                                    { PeriodConstants.Period2, ldpv.Period2 },
                                    { PeriodConstants.Period3, ldpv.Period3 },
                                    { PeriodConstants.Period4, ldpv.Period4 },
                                    { PeriodConstants.Period5, ldpv.Period5 },
                                    { PeriodConstants.Period6, ldpv.Period6 },
                                    { PeriodConstants.Period7, ldpv.Period7 },
                                    { PeriodConstants.Period8, ldpv.Period8 },
                                    { PeriodConstants.Period9, ldpv.Period9 },
                                    { PeriodConstants.Period10, ldpv.Period10 },
                                    { PeriodConstants.Period11, ldpv.Period11 },
                                    { PeriodConstants.Period12, ldpv.Period12 }
                                }
                            })
                    ));
        }
    }
}
