using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM08
{
    public class Frm08ReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IEnumerable<Frm08ReportModel>>
    {
        private readonly int _pausedCompStatus = 6;
        private readonly int _fundModel99 = 99;
        private readonly string _fundModel99ADLCode = "1";
        private readonly HashSet<int> _excludedFundModel = new HashSet<int> { 25 };
        private readonly HashSet<int> _excludedCategories = new HashSet<int> { 23, 24, 27, 28, 29, 34, 35, 36 };

        public IEnumerable<Frm08ReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var models = new List<Frm08ReportModel>();
            var returnPeriod = reportServiceContext.ReturnPeriodName;

            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationNameDictionary = referenceData.Organisations.ToDictionary(x => x.UKPRN, x => x.Name);

            var learnAimDictionary = referenceData.LARSLearningDeliveries.ToDictionary(x => x.LearnAimRef, x => x, StringComparer.OrdinalIgnoreCase);

            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);

            var pausedDeliveries = message.Learners
                                        ?.SelectMany(l => l.LearningDeliveries.Where(ld => 
                                            ld.CompStatus == _pausedCompStatus 
                                            && !ExcludedDelivery(ld, referenceData.LARSLearningDeliveries)
                                            && !_excludedFundModel.Contains(ld.FundModel)
                                            && FundModel99Rule(ld))
                                            .Select(ld => new {Learner = l, LearningDelivery = ld}));

            var currentReturnEndDate = referenceData.MetaDatas.CollectionDates.ReturnPeriods.FirstOrDefault(d =>
                                                                                    reportServiceContext.SubmissionDateTimeUtc >= d.Start 
                                                                                    && reportServiceContext.SubmissionDateTimeUtc <= d.End).End;

            if (pausedDeliveries == null)
            {
                return Enumerable.Empty<Frm08ReportModel>();
            }

            foreach (var delivery in pausedDeliveries)
            {
                var restartDelivery = GetRestartDelivery(delivery.LearningDelivery, delivery.Learner);

                if (restartDelivery != null)
                {
                    continue;
                }

                var learnActEndDate = delivery.LearningDelivery.LearnActEndDateNullable;

                if (learnActEndDate.HasValue && DaysBetween(learnActEndDate.Value, currentReturnEndDate) >= 365)
                {
                    var advancedLoansIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, ADLLearnDelFamType);
                    var sofCode = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, SOFLearnDelFamType);
                    var resIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, RESLearnDelFamType);

                    var pmOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PMUKPRNNullable
                        .GetValueOrDefault());
                    var prevOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PrevUKPRNNullable
                            .GetValueOrDefault());
                    var partnerOrgName = organisationNameDictionary.GetValueOrDefault(delivery.LearningDelivery.PartnerUKPRNNullable.GetValueOrDefault());
                    var learnAim = learnAimDictionary.GetValueOrDefault(delivery.LearningDelivery.LearnAimRef);

                    models.Add(new Frm08ReportModel
                    {
                        Return = returnPeriod,
                        UKPRN = reportServiceContext.Ukprn,
                        OrgName = orgName,
                        PartnerUKPRN = delivery.LearningDelivery.PartnerUKPRNNullable,
                        PartnerOrgName = partnerOrgName,
                        PrevUKPRN = delivery.Learner.PrevUKPRNNullable,
                        PrevOrgName = prevOrgName,
                        PMUKPRN = delivery.Learner.PMUKPRNNullable,
                        PMOrgName = pmOrgName,
                        ULN = delivery.Learner.ULN,
                        LearnRefNumber = delivery.Learner.LearnRefNumber,
                        SWSupAimId = delivery.LearningDelivery.SWSupAimId,
                        LearnAimRef = delivery.LearningDelivery.LearnAimRef,
                        LearnAimTitle = learnAim.LearnAimRefTitle,
                        AimSeqNumber = delivery.LearningDelivery.AimSeqNumber,
                        AimTypeCode = delivery.LearningDelivery.AimType,
                        LearnAimType = learnAim.LearnAimRefTypeDesc,
                        StdCode = delivery.LearningDelivery.StdCodeNullable,
                        FworkCode = delivery.LearningDelivery.FworkCodeNullable,
                        PwayCode = delivery.LearningDelivery.PwayCodeNullable,
                        ProgType = delivery.LearningDelivery.ProgTypeNullable,
                        LearnStartDate = delivery.LearningDelivery.LearnStartDate,
                        OrigLearnStartDate = delivery.LearningDelivery.OrigLearnStartDateNullable,
                        LearnPlanEndDate = delivery.LearningDelivery.LearnPlanEndDate,
                        LearnActEndDate = delivery.LearningDelivery.LearnActEndDateNullable,
                        CompStatus = delivery.LearningDelivery.CompStatus,
                        Outcome = delivery.LearningDelivery.OutcomeNullable,
                        FundModel = delivery.LearningDelivery.FundModel,
                        SOFCode = sofCode,
                        AdvancedLoansIndicator = advancedLoansIndicator,
                        ResIndicator = resIndicator,
                        ProvSpecLearnDelMon = ProviderSpecDeliveryMonitorings(delivery.LearningDelivery.ProviderSpecDeliveryMonitorings),
                        ProvSpecDelMon = ProviderSpecLearningMonitorings(delivery.Learner.ProviderSpecLearnerMonitorings),
                        PriorLearnFundAdj = delivery.LearningDelivery.PriorLearnFundAdjNullable,
                        OtherFundAdj = delivery.LearningDelivery.OtherFundAdjNullable,
                    });
                }
            }

            return models;
        }

        private ILearningDelivery GetRestartDelivery(ILearningDelivery breakLearningDelivery, ILearner learner)
        {
            return learner?.LearningDeliveries?.FirstOrDefault(ld => ld.LearnAimRef.CaseInsensitiveEquals(breakLearningDelivery.LearnAimRef)
                                                                   && ld.ProgTypeNullable == breakLearningDelivery.ProgTypeNullable
                                                                   && ld.StdCodeNullable == breakLearningDelivery.StdCodeNullable
                                                                   && ld.FworkCodeNullable == breakLearningDelivery.FworkCodeNullable
                                                                   && HasRestartFAM(ld.LearningDeliveryFAMs)
                                                                   && WithMatchingStartDates(breakLearningDelivery, ld));
        }

        private bool HasRestartFAM(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return learningDeliveryFams?.Any(f => f.LearnDelFAMType.Equals(RESLearnDelFamType, StringComparison.OrdinalIgnoreCase))
                ?? false;
        }

        private bool FundModel99Rule(ILearningDelivery delivery)
        {
            return delivery.FundModel != _fundModel99 || RetrieveFamCodeForType(delivery.LearningDeliveryFAMs, ADLLearnDelFamType) == _fundModel99ADLCode;
        }

        private bool WithMatchingStartDates(ILearningDelivery breakLearningDelivery, ILearningDelivery learningDelivery)
        {
            if (learningDelivery?.OrigLearnStartDateNullable == null)
            {
                return false;
            }

            return (learningDelivery.OrigLearnStartDateNullable.Value == breakLearningDelivery.LearnStartDate 
                || learningDelivery.OrigLearnStartDateNullable.Value == breakLearningDelivery.OrigLearnStartDateNullable)
                && learningDelivery.LearnStartDate >= breakLearningDelivery.LearnActEndDateNullable;
        }

        private bool ExcludedDelivery(ILearningDelivery learner, IReadOnlyCollection<LARSLearningDelivery> larsLearningDeliveries)
        {
            return larsLearningDeliveries?
                .Any(x => x.LearnAimRef.CaseInsensitiveEquals(learner.LearnAimRef)
                          && x.LARSLearningDeliveryCategories.Any(ldc => _excludedCategories.Contains(ldc.CategoryRef)))
                ?? false;
        }

        public double DaysBetween(DateTime start, DateTime end)
        {
            return (end - start).TotalDays;
        }
    }
}
