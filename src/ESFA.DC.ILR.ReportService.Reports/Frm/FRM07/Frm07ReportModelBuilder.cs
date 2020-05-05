using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MCAGLA;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM07
{
    public class Frm07ReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IEnumerable<Frm07ReportModel>>
    {
        private readonly int _pausedCompStatus = 6;
        private readonly int _excludedAimType = 3;
        private readonly HashSet<int> _excludedFundModel = new HashSet<int> { 25, 99 };
        private readonly HashSet<int> _excludedCategories = new HashSet<int> { 23, 24, 27, 28, 29, 34, 35, 36 };

        public IEnumerable<Frm07ReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var models = new List<Frm07ReportModel>();
            var returnPeriod = reportServiceContext.ReturnPeriodName;

            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationNameDictionary = referenceData.Organisations.ToDictionary(x => x.UKPRN, x => x.Name);
            var learnAimDictionary = referenceData.LARSLearningDeliveries.ToDictionary(x => x.LearnAimRef, x => x, StringComparer.OrdinalIgnoreCase);
            var mcaDictionary = referenceData.McaDevolvedContracts.ToDictionary(x => x.McaGlaShortCode, x => x.Ukprn,
                StringComparer.OrdinalIgnoreCase);

            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);

            var pausedDeliveries = message.Learners
                ?.SelectMany(l => l.LearningDeliveries.Where(ld =>
                        ld.CompStatus == _pausedCompStatus
                        && !ExcludedDelivery(ld, referenceData.LARSLearningDeliveries)
                        && ld.AimType != _excludedAimType
                        && !_excludedFundModel.Contains(ld.FundModel))
                    .Select(ld => new { Learner = l, LearningDelivery = ld }));

            foreach (var delivery in pausedDeliveries)
            {
                var restartDelivery = GetRestartDelivery(delivery.LearningDelivery, delivery.Learner);

                if (restartDelivery != null)
                {
                    continue;
                }

                if (delivery.LearningDelivery.LearnPlanEndDate == restartDelivery.LearnPlanEndDate)
                {
                    var advancedLoansIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, ADLLearnDelFamType);
                    var sofCode = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, SOFLearnDelFamType);
                    var resIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, RESLearnDelFamType);

                    var sofUkprn = mcaDictionary.GetValueOrDefault(sofCode);

                    var partnerOrgName = organisationNameDictionary.GetValueOrDefault(delivery.LearningDelivery.PartnerUKPRNNullable.GetValueOrDefault());
                    var learningAim = learnAimDictionary.GetValueOrDefault(delivery.LearningDelivery.LearnAimRef);
                    var pmOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PMUKPRNNullable.GetValueOrDefault());
                    var prevOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PrevUKPRNNullable.GetValueOrDefault());
                    var sofOrgName = organisationNameDictionary.GetValueOrDefault(sofUkprn);


                    models.Add(new Frm07ReportModel
                    {
                        UKPRN = reportServiceContext.Ukprn,
                        Return = returnPeriod,
                        OrgName = orgName,
                        PartnerUKPRN = delivery.LearningDelivery.PartnerUKPRNNullable,
                        PartnerOrgName = partnerOrgName,
                        PrevUKPRN = delivery.Learner.PrevUKPRNNullable,
                        PrevOrgName = prevOrgName,
                        PMUKPRN = delivery.Learner.PMUKPRNNullable,
                        PMOrgName = pmOrgName,
                        DevolvedUKPRN = sofUkprn,
                        DevolvedOrgName = sofOrgName,
                        ULN = delivery.Learner.ULN,
                        LearnRefNumber = delivery.Learner.LearnRefNumber,
                        PrevLearnRefNumber = delivery.Learner.PrevLearnRefNumber,
                        LearnAimRef = delivery.LearningDelivery.LearnAimRef,
                        AimSeqNumber = delivery.LearningDelivery.AimSeqNumber,
                        AimTypeCode = delivery.LearningDelivery.AimType,
                        LearnAimTitle = learningAim.LearnAimRefTitle,
                        LearnAimType = learningAim.LearnAimRefTypeDesc,
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
                        FundingStream = CalculateFundingStream(delivery.LearningDelivery.FundModel, delivery.LearningDelivery.ProgTypeNullable, advancedLoansIndicator, sofCode),
                        ResIndicator = resIndicator,
                        SWSupAimId = delivery.LearningDelivery.SWSupAimId,
                        ProvSpecLearnDelMon = ProviderSpecDeliveryMonitorings(delivery.LearningDelivery.ProviderSpecDeliveryMonitorings),
                        ProvSpecDelMon = ProviderSpecLearningMonitorings(delivery.Learner.ProviderSpecLearnerMonitorings),
                        PriorLearnFundAdj = delivery.LearningDelivery.PriorLearnFundAdjNullable,
                        OtherFundAdj = delivery.LearningDelivery.OtherFundAdjNullable,
                    });
                }
            }

            return models;
        }

        private bool ExcludedDelivery(ILearningDelivery learner, IReadOnlyCollection<LARSLearningDelivery> larsLearningDeliveries)
        {
            return larsLearningDeliveries
                .Any(x => x.LearnAimRef.CaseInsensitiveEquals(learner.LearnAimRef)
                          && x.LARSLearningDeliveryCategories.Any(ldc => _excludedCategories.Contains(ldc.CategoryRef)));
        }

        private ILearningDelivery GetRestartDelivery(ILearningDelivery breakLearningDelivery, ILearner learner)
        {
            return learner.LearningDeliveries.FirstOrDefault(ld => ld.LearnAimRef.CaseInsensitiveEquals(breakLearningDelivery.LearnAimRef)
                                                                   && ld.ProgTypeNullable == breakLearningDelivery.ProgTypeNullable
                                                                   && ld.StdCodeNullable == breakLearningDelivery.StdCodeNullable
                                                                   && ld.FworkCodeNullable == breakLearningDelivery.FworkCodeNullable
                                                                   && HasRestartFAM(ld.LearningDeliveryFAMs)
                                                                   && WithMatchingStartDates(breakLearningDelivery, ld));
        }

        private bool HasRestartFAM(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return learningDeliveryFams.Any(f => f.LearnDelFAMType.Equals(RESLearnDelFamType, StringComparison.OrdinalIgnoreCase));
        }

        private bool WithMatchingStartDates(ILearningDelivery breakLearningDelivery, ILearningDelivery learningDelivery)
        {
            if (learningDelivery.OrigLearnStartDateNullable == null)
            {
                return false;
            }

            return (learningDelivery.OrigLearnStartDateNullable.Value == breakLearningDelivery.LearnStartDate
                    || learningDelivery.OrigLearnStartDateNullable.Value == breakLearningDelivery.OrigLearnStartDateNullable)
                   && learningDelivery.LearnStartDate > breakLearningDelivery.LearnActEndDateNullable;
        }

    }
}
