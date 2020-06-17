using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main.AEBSTFInitiativesOccupancy
{
    public class AEBSTFInitiativesOccupancyReportModelBuilder : AbstractOccupancyReportModelBuilder, IModelBuilder<IEnumerable<AEBSTFInitiativesOccupancyReportModel>>
    {
        public AEBSTFInitiativesOccupancyReportModelBuilder(IIlrModelMapper ilrModelMapper)
            : base(ilrModelMapper)
        {
        }

        private readonly IEnumerable<string> _ldmLearnDelFamCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_370,
            LearningDeliveryFAMCodeConstants.LDM_371,
            LearningDeliveryFAMCodeConstants.LDM_372,
            LearningDeliveryFAMCodeConstants.LDM_373,
        };

        public IEnumerable<AEBSTFInitiativesOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            var fm35LearningDeliveries = BuildFm35LearningDeliveryDictionary(fm35);

            var models = new List<AEBSTFInitiativesOccupancyReportModel>();

            foreach (var learner in message?.Learners?.Where(l => l != null) ?? Enumerable.Empty<ILearner>())
            {
                foreach (var learningDelivery in learner.LearningDeliveries?.Where(ld => ld != null) ?? Enumerable.Empty<ILearningDelivery>())
                {
                    if (LearningDeliveryFilter(learningDelivery))
                    { 
                        var fm35LearningDelivery = fm35LearningDeliveries.GetValueOrDefault(learner.LearnRefNumber).GetValueOrDefault(learningDelivery.AimSeqNumber);
                        var larsLearningDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                        var providerSpecLearnerMonitoring = _ilrModelMapper.MapProviderSpecLearnerMonitorings(learner.ProviderSpecLearnerMonitorings);
                        var providerSpecDeliveryMonitoring = _ilrModelMapper.MapProviderSpecDeliveryMonitorings(learningDelivery.ProviderSpecDeliveryMonitorings);
                        var learningDeliveryFams = _ilrModelMapper.MapLearningDeliveryFAMs(learningDelivery.LearningDeliveryFAMs);
                        var periodisedValues = BuildFm35PeriodisedValuesModel(fm35LearningDelivery?.LearningDeliveryPeriodisedValues);

                        var traineeshipWorkPlacementOrWorPreparation =
                            fm35LearningDelivery?.LearningDeliveryValue?.TrnWorkPlaceAim == true
                            || fm35LearningDelivery?.LearningDeliveryValue?.TrnWorkPrepAim == true
                                ? ReportingConstants.Yes
                                : ReportingConstants.No;

                        var higherApprenticeshipPrescribedHeAim = fm35LearningDelivery?.LearningDeliveryValue?.PrscHEAim == true ? ReportingConstants.Yes : ReportingConstants.No;

                        var fundModelAgnosticModel = new FundModelAgnosticModel()
                        {
                            AimSequenceNumber = learningDelivery.AimSeqNumber,
                            AimType = learningDelivery.AimType,
                            CompStatus = learningDelivery.CompStatus,
                            FundModel = learningDelivery.FundModel,
                            ApplicableFundingRate = fm35LearningDelivery?.LearningDeliveryValue?.ApplicWeightFundRate,
                            FundLine = fm35LearningDelivery?.LearningDeliveryValue?.FundLine,
                            LearningActualEndDate = learningDelivery.LearnActEndDateNullable,
                            LearningPlannedEndDate = learningDelivery.LearnPlanEndDate,
                            LearningStartDate = learningDelivery.LearnStartDate,
                        };

                        models.Add(new AEBSTFInitiativesOccupancyReportModel()
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
                            TraineeshipWorkPlacementOrWorkPreparation = traineeshipWorkPlacementOrWorPreparation,
                            HigherApprenticeshipPrescribedHeAim = higherApprenticeshipPrescribedHeAim
                        });
                    }
                }
            }

            return Order(models);
        }

        public bool LearningDeliveryFilter(ILearningDelivery learningDelivery)
        {
            return learningDelivery != null && LdmLearningDeliveryFilter(learningDelivery);
        }

        private bool LdmLearningDeliveryFilter(ILearningDelivery learningDelivery)
        {
            return learningDelivery
                       .LearningDeliveryFAMs?
                       .Any(
                           f =>
                               f.LearnDelFAMType == LearningDeliveryFAMTypeConstants.LDM
                               && _ldmLearnDelFamCodes.Contains(f.LearnDelFAMCode)) == true;
        }
    }
}
