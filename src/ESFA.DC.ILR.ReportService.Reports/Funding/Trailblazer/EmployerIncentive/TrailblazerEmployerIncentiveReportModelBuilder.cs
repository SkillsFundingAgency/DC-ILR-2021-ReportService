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
        public IEnumerable<TrailblazerEmployerIncentivesReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var fm81 = reportServiceDependentData.Get<FM81Global>();

            var employerIds = BuildFm81EmployersCollection(fm81);
            var periodisedValues = BuildFm81PeriodisedValuesList(fm81).ToList();

            var reportModels = employerIds
                .Select(
                    empId =>
                new TrailblazerEmployerIncentivesReportModel
                {
                    EmployerIdentifier = empId,
                    AugustSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 0, empId),
                    August1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 0, empId),
                    AugustAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 0, empId),

                    SeptemberSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 1, empId),
                    September1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 1, empId),
                    SeptemberAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 1, empId),

                    OctoberSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 2, empId),
                    October1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 2, empId),
                    OctoberAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 2, empId),

                    NovemberSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 3, empId),
                    November1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 3, empId),
                    NovemberAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 3, empId),

                    DecemberSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 4, empId),
                    December1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 4, empId),
                    DecemberAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 4, empId),

                    JanuarySmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 5, empId),
                    January1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 5, empId),
                    JanuaryAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 5, empId),

                    FebruarySmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 6, empId),
                    February1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 6, empId),
                    FebruaryAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 6, empId),

                    MarchSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 7, empId),
                    March1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 7, empId),
                    MarchAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 7, empId),

                    AprilSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 8, empId),
                    April1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 8, empId),
                    AprilAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 8, empId),

                    MaySmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 9, empId),
                    May1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 9, empId),
                    MayAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 9, empId),

                    JuneSmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 10, empId),
                    June1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 10, empId),
                    JuneAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 10, empId),

                    JulySmallEmployerIncentive = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81SmallBusPayment, 11, empId),
                    July1618ApprenticeIncentive = CalculatePaymentValue(periodisedValues, new[] { AttributeConstants.Fm81YoungAppFirstPayment, AttributeConstants.Fm81YoungAppSecondPayment }, 11, empId),
                    JulyAchievementPayment = CalculatePaymentValue(periodisedValues, AttributeConstants.Fm81AchPayment, 11, empId),
                });

            return Order(reportModels);
        }

        public decimal CalculatePaymentValue(ICollection<TrailblazerLearningDeliveryPeriodisedValues> periodisedValues, IEnumerable<string> attributeConstants, int periodIndex, int empId)
        {
            return attributeConstants.Sum(a => CalculatePaymentValue(periodisedValues, a, periodIndex, empId));
        }

        public decimal CalculatePaymentValue(ICollection<TrailblazerLearningDeliveryPeriodisedValues> periodisedValues, string attributeConstant, int periodIndex, int empId)
        {
            return periodisedValues
                .Where(pv =>
                    pv.EmployerIds[attributeConstant] == empId 
                    && pv.AttributeName == attributeConstant)
                .Sum(s => s.Values[periodIndex] ?? 0);
        }

        public IEnumerable<int> BuildFm81EmployersCollection(FM81Global fm81Global)
        {
            var employerIdHashSet = new HashSet<int>();

            foreach (var learner in fm81Global?.Learners.Where(l => l != null) ?? Enumerable.Empty<FM81Learner>())
            {
                foreach (var learningDelivery in learner.LearningDeliveries?.Where(ld => ld?.LearningDeliveryValues != null) ?? Enumerable.Empty<LearningDelivery>())
                {
                    if (learningDelivery.LearningDeliveryValues.EmpIdSmallBusDate.HasValue)
                    {
                        employerIdHashSet.Add(learningDelivery.LearningDeliveryValues.EmpIdSmallBusDate.Value);
                    }

                    if (learningDelivery.LearningDeliveryValues.EmpIdFirstYoungAppDate.HasValue)
                    {
                        employerIdHashSet.Add(learningDelivery.LearningDeliveryValues.EmpIdFirstYoungAppDate.Value);
                    }

                    if (learningDelivery.LearningDeliveryValues.EmpIdSecondYoungAppDate.HasValue)
                    {
                        employerIdHashSet.Add(learningDelivery.LearningDeliveryValues.EmpIdSecondYoungAppDate.Value);
                    }

                    if (learningDelivery.LearningDeliveryValues.EmpIdAchDate.HasValue)
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
                                Values = new[]
                                {
                                    ldpv.Period1,
                                    ldpv.Period2,
                                    ldpv.Period3,
                                    ldpv.Period4,
                                    ldpv.Period5,
                                    ldpv.Period6,
                                    ldpv.Period7,
                                    ldpv.Period8,
                                    ldpv.Period9,
                                    ldpv.Period10,
                                    ldpv.Period11,
                                    ldpv.Period12
                                }
                            })
                    ));
        }

        public IEnumerable<TrailblazerEmployerIncentivesReportModel> Order(IEnumerable<TrailblazerEmployerIncentivesReportModel> models)
        {
            return models.OrderBy(m => m.EmployerIdentifier);
        }
    }
}
