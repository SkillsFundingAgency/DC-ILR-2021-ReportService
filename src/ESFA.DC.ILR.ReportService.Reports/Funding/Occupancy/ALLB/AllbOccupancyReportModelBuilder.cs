using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm99;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.ALLB
{
    public class AllbOccupancyReportModelBuilder : AbstractOccupancyReportModelBuilder, IModelBuilder<IEnumerable<AllbOccupancyReportModel>>
    {
        public AllbOccupancyReportModelBuilder(IIlrModelMapper ilrModelMapper)
            : base(ilrModelMapper)
        {
        }

        public IEnumerable<AllbOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm99 = reportServiceDependentData.Get<ALBGlobal>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            var fm99LearningDeliveryDictionary = BuildFm99LearningDeliveryDictionary(fm99);

            var models = new List<AllbOccupancyReportModel>();

            foreach (var learner in message?.Learners?.Where(l => l != null) ?? Enumerable.Empty<ILearner>())
            {
                foreach (var learningDelivery in learner.LearningDeliveries ?? Enumerable.Empty<ILearningDelivery>())
                {
                    var fm99LearningDelivery = fm99LearningDeliveryDictionary.GetValueOrDefault(learner.LearnRefNumber).GetValueOrDefault(learningDelivery.AimSeqNumber);

                    if (Filter(learningDelivery, fm99LearningDelivery))
                    {
                        var larsLearningDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                        var providerSpecLearnerMonitoring = _ilrModelMapper.MapProviderSpecLearnerMonitorings(learner.ProviderSpecLearnerMonitorings);
                        var providerSpecDeliveryMonitoring = _ilrModelMapper.MapProviderSpecDeliveryMonitorings(learningDelivery.ProviderSpecDeliveryMonitorings);
                        var learningDeliveryFams = _ilrModelMapper.MapLearningDeliveryFAMs(learningDelivery.LearningDeliveryFAMs);
                        var periodisedValues = BuildFm99PeriodisedValuesModel(fm99LearningDelivery?.LearningDeliveryPeriodisedValues);

                        models.Add(new AllbOccupancyReportModel()
                        {
                            Learner = learner,
                            ProviderSpecLearnerMonitoring = providerSpecLearnerMonitoring,
                            LearningDelivery = learningDelivery,
                            ProviderSpecDeliveryMonitoring = providerSpecDeliveryMonitoring,
                            LearningDeliveryFAMs = learningDeliveryFams,
                            Fm99LearningDelivery = fm99LearningDelivery?.LearningDeliveryValue,
                            LarsLearningDelivery = larsLearningDelivery,
                            PeriodisedValues = periodisedValues,
                        });
                    }
                }
            }

            return Order(models);
        }

        public bool Filter(ILearningDelivery learningDelivery, LearningDelivery albLearningDelivery)
        {
            if (learningDelivery?.LearningDeliveryFAMs != null && albLearningDelivery?.LearningDeliveryValue != null)
            {
                return learningDelivery.FundModel == FundModelConstants.FM99
                   && (learningDelivery.LearningDeliveryFAMs.Any(fam =>
                       fam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ADL) &&
                       fam.LearnDelFAMCode == "1") == true)
                   && (albLearningDelivery.LearningDeliveryValue.AreaCostFactAdj > 0
                        || learningDelivery.LearningDeliveryFAMs.Any(fam =>
                           fam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ALB)) == true);
            }

            return false;
        }

        private Dictionary<string, Dictionary<int, LearningDelivery>> BuildFm99LearningDeliveryDictionary(ALBGlobal albGlobal)
        {
            return albGlobal?
                       .Learners?
                       .ToDictionary(
                           l => l.LearnRefNumber,
                           l => l.LearningDeliveries
                               .ToDictionary(
                                   ld => ld.AimSeqNumber,
                                   ld => ld),
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, Dictionary<int, LearningDelivery>>();
        }
    }
}
