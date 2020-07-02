using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main
{
    public class MainOccupancyReportModelBuilder : AbstractOccupancyReportModelBuilder, IModelBuilder<IEnumerable<MainOccupancyReportModel>>
    {
        public MainOccupancyReportModelBuilder(IIlrModelMapper ilrModelMapper)
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

        public IEnumerable<MainOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();
            var fm25 = reportServiceDependentData.Get<FM25Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            var fm35LearningDeliveries = BuildFm35LearningDeliveryDictionary(fm35);
            var fm25Learners = BuildFm25LearnerDictionary(fm25);

            var models = new List<MainOccupancyReportModel>();

            foreach (var learner in message?.Learners?.Where(l => l != null) ?? Enumerable.Empty<ILearner>())
            {
                foreach (var learningDelivery in learner.LearningDeliveries?.Where(ld => ld != null) ?? Enumerable.Empty<ILearningDelivery>())
                {
                    if (FundModelLearningDeliveryFilter(learningDelivery, FundModelConstants.FM35))
                    { 
                        var fm35LearningDelivery = fm35LearningDeliveries.GetValueOrDefault(learner.LearnRefNumber).GetValueOrDefault(learningDelivery.AimSeqNumber);
                        var larsLearningDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                        var larsFrameworkAim = larsLearningDelivery?.LARSFrameworks?.FirstOrDefault(lf => lf.LARSFrameworkAim.LearnAimRef.CaseInsensitiveEquals(learningDelivery.LearnAimRef))?.LARSFrameworkAim ?? new LARSFrameworkAim();
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
                            TraineeshipWorkPlacementOrWorkPreparation = traineeshipWorkPlacementOrWorPreparation,
                            HigherApprenticeshipPrescribedHeAim = higherApprenticeshipPrescribedHeAim,
                            LarsFrameworkAim = larsFrameworkAim
                        });
                    }
                }

                if (learner.LearningDeliveries?.Any(ld => FundModelLearningDeliveryFilter(ld, FundModelConstants.FM25)) == true)
                {
                    var fm25Learner = fm25Learners.GetValueOrDefault(learner.LearnRefNumber);
                    var providerSpecLearnerMonitoring = _ilrModelMapper.MapProviderSpecLearnerMonitorings(learner.ProviderSpecLearnerMonitorings);
                    var periodisedValues = BuildFm25PeriodisedValuesModel(fm25Learner?.LearnerPeriodisedValues);

                    var fundModelAgnosticModel = new FundModelAgnosticModel()
                    {
                        FundModel = FundModelConstants.FM25,
                        ApplicableFundingRate = fm25Learner?.NatRate,
                        FundLine = fm25Learner?.FundLine,
                        LearningActualEndDate = fm25Learner?.LearnerActEndDate,
                        LearningPlannedEndDate = fm25Learner?.LearnerPlanEndDate,
                        LearningStartDate = fm25Learner?.LearnerStartDate,
                    };

                    models.Add(new MainOccupancyReportModel()
                    {
                        Learner = learner,
                        ProviderSpecLearnerMonitoring = providerSpecLearnerMonitoring,
                        PeriodisedValues = periodisedValues,
                        FundModelAgnosticModel = fundModelAgnosticModel,
                    });
                }
            }

            return Order(models);
        }

        public bool FundModelLearningDeliveryFilter(ILearningDelivery learningDelivery, int fundModel)
        {
            if (learningDelivery != null)
            {
                return learningDelivery.FundModel == fundModel
                       && Sof105LearningDeliveryFilter(learningDelivery)
                       && !LdmLearningDeliveryFilter(learningDelivery);
            }

            return false;
        }

        private bool Sof105LearningDeliveryFilter(ILearningDelivery learningDelivery)
        {
            return learningDelivery
                .LearningDeliveryFAMs?
                .Any(
                    f =>
                    f.LearnDelFAMType == LearningDeliveryFAMTypeConstants.SOF
                    && f.LearnDelFAMCode == LearningDeliveryFAMCodeConstants.SOF_ESFA) == true;
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
