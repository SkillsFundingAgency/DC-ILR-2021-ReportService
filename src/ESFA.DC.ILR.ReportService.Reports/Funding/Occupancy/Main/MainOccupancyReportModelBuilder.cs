using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main
{
    public class MainOccupancyReportModelBuilder : AbstractOccupancyReportModelBuilder, IModelBuilder<IEnumerable<MainOccupancyReportModel>>
    {
        private readonly IIlrModelMapper _ilrModelMapper;

        public MainOccupancyReportModelBuilder(IIlrModelMapper ilrModelMapper)
        {
            _ilrModelMapper = ilrModelMapper;
        }


        public IEnumerable<MainOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();
            var fm25 = reportServiceDependentData.Get<FM25Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            var fm35LearningDeliveries = BuildFm35LearningDeliveryDictionary(fm35);

            var models = new List<MainOccupancyReportModel>();

            foreach (var learner in message?.Learners?.Where(l => l != null) ?? Enumerable.Empty<ILearner>())
            {
                foreach (var learningDelivery in learner.LearningDeliveries?.Where(Fm35LearningDeliveryFilter) ?? Enumerable.Empty<ILearningDelivery>())
                {
                    var fm35LearningDelivery = fm35LearningDeliveries.GetValueOrDefault(learner.LearnRefNumber).GetValueOrDefault(learningDelivery.AimSeqNumber);
                    var larsLearningDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                    var providerSpecLearnerMonitoring = _ilrModelMapper.MapProviderSpecLearnerMonitorings(learner.ProviderSpecLearnerMonitorings);
                    var providerSpecDeliveryMonitoring = _ilrModelMapper.MapProviderSpecDeliveryMonitorings(learningDelivery.ProviderSpecDeliveryMonitorings);
                    var learningDeliveryFams = _ilrModelMapper.MapLearningDeliveryFAMs(learningDelivery.LearningDeliveryFAMs);
                    var periodisedValues = BuildPeriodisedValuesModel(fm35LearningDelivery?.LearningDeliveryPeriodisedValues);

                    var fundModelAgnosticModel = new FundModelAgnosticModel()
                    {
                        ApplicableFundingRate = fm35LearningDelivery?.LearningDeliveryValue.ApplicWeightFundRate,
                        FundLine = fm35LearningDelivery?.LearningDeliveryValue.FundLine,
                        LearningActualEndDate = learningDelivery.LearnActEndDateNullable,
                        LearningPlannedEndDate = learningDelivery.LearnPlanEndDate,
                        LearningStartDate = learningDelivery.LearnStartDate,
                    };
                    

                    models.Add(new MainOccupancyReportModel()
                    {
                        Learner = learner,
                        ProviderSpecLearnerMonitoring = providerSpecLearnerMonitoring,
                        LearningDelivery = learningDelivery,
                        ProviderSpecDeliveryMonitoring = providerSpecDeliveryMonitoring,
                        LearningDeliveryFAMs = learningDeliveryFams,
                        Fm35LearningDelivery = fm35LearningDelivery?.LearningDeliveryValue,
                        LarsLearningDelivery = larsLearningDelivery,
                        PeriodisedValues = periodisedValues,
                        FundModelAgnosticModel = fundModelAgnosticModel,
                    });
                }
            }

            return Order(models);
        }

        public bool Fm35LearningDeliveryFilter(ILearningDelivery learningDelivery)
        {
            if (learningDelivery != null)
            {
                return learningDelivery.FundModel == FundModelConstants.FM35
                       && learningDelivery
                           .LearningDeliveryFAMs?
                           .Any(
                               f =>
                                   f.LearnDelFAMType == LearningDeliveryFAMTypeConstants.SOF
                                   && f.LearnDelFAMCode == LearningDeliveryFAMCodeConstants.SOF_ESFA) == true;
            }

            return false;
        }
    }
}
