using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM07
{
    public class Frm07ReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IEnumerable<Frm07ReportModel>>
    {
        private readonly int _pausedCompStatus = 6;
        private readonly int _fundModel99 = 99;
        private readonly string _fundModel99ADLCode = "1";

        private readonly HashSet<int> _excludedFundModel = new HashSet<int> { 25 };
        private readonly HashSet<int> _excludedCategories = new HashSet<int> { 23, 24, 27, 28, 29, 34, 35, 36 };

        public IEnumerable<Frm07ReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var models = new List<Frm07ReportModel>();
            var returnPeriod = reportServiceContext.ReturnPeriodName;

            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationNameDictionary = referenceData.Organisations.ToDictionary(k => k.UKPRN, v => v.Name);
            var learnAimDictionary = referenceData.LARSLearningDeliveries.ToDictionary(k => k.LearnAimRef, v => v, StringComparer.OrdinalIgnoreCase);
            var sofCodeDictionary = referenceData.DevolvedPostocdes.McaGlaSofLookups.Where(l => DevolvedCodes.Contains(l.SofCode)).ToDictionary(k => k.SofCode, v => v.McaGlaShortCode);
            var mcaDictionary = referenceData.McaDevolvedContracts.ToDictionary(k => k.McaGlaShortCode, v => v.Ukprn,
                StringComparer.OrdinalIgnoreCase);

            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);

            var pausedDeliveries = message.Learners
                ?.SelectMany(l => l.LearningDeliveries.Where(ld =>
                        ld.CompStatus == _pausedCompStatus
                        && !ExcludedDelivery(ld, referenceData.LARSLearningDeliveries)
                        && FundModel99Rule(ld)
                        && !_excludedFundModel.Contains(ld.FundModel))
                    .Select(ld => new { Learner = l, LearningDelivery = ld }));

            if (pausedDeliveries == null)
            {
                return models;
            }

            var learnerDeliveries = pausedDeliveries.GroupBy(x => x.Learner);

            foreach (var deliverySet in learnerDeliveries)
            {
                foreach(var delivery in deliverySet.OrderByDescending(x => x.LearningDelivery.LearnStartDate))
                {
                    var restartDelivery = GetRestartDelivery(delivery.LearningDelivery, delivery.Learner);

                    if (restartDelivery != null)
                    {
                        var pmOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PMUKPRNNullable.GetValueOrDefault());
                        var prevOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PrevUKPRNNullable.GetValueOrDefault());

                        models.Add(BuildModelForLearningDelivery(reportServiceContext, restartDelivery, delivery.Learner, sofCodeDictionary, mcaDictionary, organisationNameDictionary, learnAimDictionary, returnPeriod, orgName, pmOrgName, prevOrgName));
                    }

                }
            }

            return models;
        }

        private Frm07ReportModel BuildModelForLearningDelivery(IReportServiceContext reportServiceContext, ILearningDelivery learningDelivery, ILearner learner,
            Dictionary<string, string> sofCodeDictionary, Dictionary<string, int> mcaDictionary, Dictionary<int, string> organisationNameDictionary,
            Dictionary<string, LARSLearningDelivery> learnAimDictionary, string returnPeriod, string orgName, string pmOrgName, string prevOrgName)
        {
            var advancedLoansIndicator = RetrieveFamCodeForType(learningDelivery.LearningDeliveryFAMs, ADLLearnDelFamType);
            var resIndicator = RetrieveFamCodeForType(learningDelivery.LearningDeliveryFAMs, RESLearnDelFamType);

            var sofCode = RetrieveFamCodeForType(learningDelivery.LearningDeliveryFAMs, SOFLearnDelFamType);
            var mcaShortCode = sofCodeDictionary.GetValueOrDefault(sofCode);
            var sofUkprn = mcaDictionary.GetValueOrDefault(mcaShortCode);

            var partnerOrgName = organisationNameDictionary.GetValueOrDefault(learningDelivery.PartnerUKPRNNullable.GetValueOrDefault());
            var learningAim = learnAimDictionary.GetValueOrDefault(learningDelivery.LearnAimRef);

            var sofOrgName = organisationNameDictionary.GetValueOrDefault(sofUkprn);

            return new Frm07ReportModel
            {
                Return = returnPeriod,
                UKPRN = reportServiceContext.Ukprn,
                OrgName = orgName,
                PartnerUKPRN = learningDelivery.PartnerUKPRNNullable,
                PartnerOrgName = partnerOrgName,
                PrevUKPRN = learner.PrevUKPRNNullable,
                PrevOrgName = prevOrgName,
                PMUKPRN = learner.PMUKPRNNullable,
                PMOrgName = pmOrgName,
                DevolvedUKPRN = sofUkprn != 0 ? sofUkprn : (int?)null,
                DevolvedOrgName = sofOrgName,
                ULN = learner.ULN,
                LearnRefNumber = learner.LearnRefNumber,
                PrevLearnRefNumber = learner.PrevLearnRefNumber,
                SWSupAimId = learningDelivery.SWSupAimId,
                LearnAimRef = learningDelivery.LearnAimRef,
                LearnAimTitle = learningAim.LearnAimRefTitle,
                AimSeqNumber = learningDelivery.AimSeqNumber,
                AimTypeCode = learningDelivery.AimType,
                LearnAimType = learningAim.LearnAimRefTypeDesc,
                StdCode = learningDelivery.StdCodeNullable,
                FworkCode = learningDelivery.FworkCodeNullable,
                PwayCode = learningDelivery.PwayCodeNullable,
                ProgType = learningDelivery.ProgTypeNullable,
                LearnStartDate = learningDelivery.LearnStartDate,
                OrigLearnStartDate = learningDelivery.OrigLearnStartDateNullable,
                LearnPlanEndDate = learningDelivery.LearnPlanEndDate,
                LearnActEndDate = learningDelivery.LearnActEndDateNullable,
                CompStatus = learningDelivery.CompStatus,
                Outcome = learningDelivery.OutcomeNullable,
                FundModel = learningDelivery.FundModel,
                SOFCode = sofCode,
                AdvancedLoansIndicator = advancedLoansIndicator,
                ResIndicator = resIndicator,
                ProvSpecLearnDelMon = ProviderSpecDeliveryMonitorings(learningDelivery.ProviderSpecDeliveryMonitorings),
                ProvSpecDelMon = ProviderSpecLearningMonitorings(learner.ProviderSpecLearnerMonitorings),
                PriorLearnFundAdj = learningDelivery.PriorLearnFundAdjNullable,
                OtherFundAdj = learningDelivery.OtherFundAdjNullable,
            };
        }

        private bool ExcludedDelivery(ILearningDelivery learner, IReadOnlyCollection<LARSLearningDelivery> larsLearningDeliveries)
        {
            return larsLearningDeliveries
                .Any(x => x.LearnAimRef.CaseInsensitiveEquals(learner.LearnAimRef)
                          && x.LARSLearningDeliveryCategories.Any(ldc => _excludedCategories.Contains(ldc.CategoryRef)));
        }

        private ILearningDelivery GetRestartDelivery(ILearningDelivery breakLearningDelivery, ILearner learner)
        {
            return learner.LearningDeliveries.FirstOrDefault(ld => ld.LearnPlanEndDate == breakLearningDelivery.LearnPlanEndDate
                                                             && ld.LearnAimRef.CaseInsensitiveEquals(breakLearningDelivery.LearnAimRef)
                                                             && ld.ProgTypeNullable == breakLearningDelivery.ProgTypeNullable
                                                             && ld.StdCodeNullable == breakLearningDelivery.StdCodeNullable
                                                             && ld.FworkCodeNullable == breakLearningDelivery.FworkCodeNullable
                                                             && HasRestartFAM(ld.LearningDeliveryFAMs)
                                                             && WithMatchingStartDates(breakLearningDelivery, ld));
        }

        private bool HasRestartFAM(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return learningDeliveryFams?.Any(f => f.LearnDelFAMType.CaseInsensitiveEquals(RESLearnDelFamType)) ?? false;
        }

        private bool FundModel99Rule(ILearningDelivery delivery)
        {
            return delivery.FundModel != _fundModel99 || RetrieveFamCodeForType(delivery.LearningDeliveryFAMs, ADLLearnDelFamType) == _fundModel99ADLCode;
        }

        private bool WithMatchingStartDates(ILearningDelivery breakLearningDelivery, ILearningDelivery learningDelivery)
        {
            if (learningDelivery.OrigLearnStartDateNullable == null)
            {
                return false;
            }

            return (learningDelivery.OrigLearnStartDateNullable == breakLearningDelivery.LearnStartDate
                    || learningDelivery.OrigLearnStartDateNullable == breakLearningDelivery.OrigLearnStartDateNullable)
                   && learningDelivery.LearnStartDate >= breakLearningDelivery.LearnActEndDateNullable;
        }
    }
}
