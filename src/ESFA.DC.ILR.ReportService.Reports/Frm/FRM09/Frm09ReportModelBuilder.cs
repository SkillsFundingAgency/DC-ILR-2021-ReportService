using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM09
{
    public class Frm09ReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IEnumerable<Frm09ReportModel>>
    {
        private readonly int _withdrawnCompStatus = 3;
        private readonly int _withdrawnReasonCode = 40;
        private readonly int _excludedAimType = 3;
        private readonly HashSet<int> _excludedFundModel = new HashSet<int> { 25, 99 };
        private readonly HashSet<int> _excludedCategories = new HashSet<int> { 23, 24, 27, 28, 29, 34, 35, 36 };

        public IEnumerable<Frm09ReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var models = new List<Frm09ReportModel>();
            var returnPeriod = reportServiceContext.ReturnPeriodName;

            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationNameDictionary = referenceData.Organisations.ToDictionary(k => k.UKPRN, v => v.Name);
            var learnAimDictionary = referenceData.LARSLearningDeliveries.ToDictionary(k => k.LearnAimRef, v => v, StringComparer.OrdinalIgnoreCase);
            var sofCodeDictionary = referenceData.DevolvedPostocdes.McaGlaSofLookups.Where(l => DevolvedCodes.Contains(l.SofCode)).ToDictionary(k => k.SofCode, v => v.McaGlaShortCode);
            var mcaDictionary = referenceData.McaDevolvedContracts.ToDictionary(k => k.McaGlaShortCode, v => v.Ukprn,
                StringComparer.OrdinalIgnoreCase);

            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);

            var currentReturnEndDate = referenceData.MetaDatas.CollectionDates.ReturnPeriods.FirstOrDefault(d =>
                reportServiceContext.SubmissionDateTimeUtc >= d.Start && reportServiceContext.SubmissionDateTimeUtc <= d.End).End;

            var withdrawanDeliveries = message.Learners
                ?.SelectMany(l => l.LearningDeliveries.Where(ld =>
                        ld.CompStatus == _withdrawnCompStatus
                        && ld.WithdrawReasonNullable == _withdrawnReasonCode
                        && !ExcludedDelivery(ld, referenceData.LARSLearningDeliveries)
                        && ld.AimType != _excludedAimType
                        && !_excludedFundModel.Contains(ld.FundModel))
                    .Select(ld => new { Learner = l, LearningDelivery = ld }));

            if (withdrawanDeliveries == null)
            {
                return models;
            }

            foreach (var delivery in withdrawanDeliveries)
            {
                if (!HasRestartDelivery(delivery.LearningDelivery, delivery.Learner, message))
                {
                    if (delivery.LearningDelivery.LearnActEndDateNullable != null && DaysBetween(delivery.LearningDelivery.LearnActEndDateNullable.Value, currentReturnEndDate) >= 90)
                    {
                        var pmOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PMUKPRNNullable.GetValueOrDefault());
                        var prevOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PrevUKPRNNullable.GetValueOrDefault());

                        models.Add(BuildModelForLearningDelivery(reportServiceContext, delivery.LearningDelivery, delivery.Learner, sofCodeDictionary, mcaDictionary, organisationNameDictionary, learnAimDictionary, returnPeriod, orgName, pmOrgName, prevOrgName));
                    }
                }
            }

            return models;
        }

        private Frm09ReportModel BuildModelForLearningDelivery(IReportServiceContext reportServiceContext, ILearningDelivery learningDelivery, ILearner learner,
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

            return new Frm09ReportModel
            {
                UKPRN = reportServiceContext.Ukprn,
                Return = returnPeriod,
                OrgName = orgName,
                PartnerUKPRN = learningDelivery.PartnerUKPRNNullable,
                PartnerOrgName = partnerOrgName,
                PrevUKPRN = learner.PrevUKPRNNullable,
                PrevOrgName = prevOrgName,
                PMUKPRN = learner.PMUKPRNNullable,
                PMOrgName = pmOrgName,
                DAUKPRN = sofUkprn != 0 ? sofUkprn : (int?)null,
                DAOrgName = sofOrgName,
                ULN = learner.ULN,
                LearnRefNumber = learner.LearnRefNumber,
                PrevLearnRefNumber = learner.PrevLearnRefNumber,
                LearnAimRef = learningDelivery.LearnAimRef,
                AimSeqNumber = learningDelivery.AimSeqNumber,
                AimTypeCode = learningDelivery.AimType,
                LearnAimTitle = learningAim.LearnAimRefTitle,
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
                WithdrawalCode = learningDelivery.WithdrawReasonNullable,
                AdvancedLoansIndicator = advancedLoansIndicator,
                FundingStream = CalculateFundingStream(learningDelivery.FundModel, learningDelivery.ProgTypeNullable, advancedLoansIndicator, sofCode),
                ResIndicator = resIndicator,
                SWSupAimId = learningDelivery.SWSupAimId,
                ProvSpecLearnDelMon = ProviderSpecLearningMonitorings(learner.ProviderSpecLearnerMonitorings),
                ProvSpecDelMon = ProviderSpecDeliveryMonitorings(learningDelivery.ProviderSpecDeliveryMonitorings),
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

        private bool HasRestartDelivery(ILearningDelivery withdrawnLearningDelivery, ILearner withdrawnLearner, IMessage message)
        {
            return message.Learners.Where(l =>
                    (l.LearnRefNumber == withdrawnLearner.LearnRefNumber) ||
                    (l.PrevLearnRefNumber == withdrawnLearner.LearnRefNumber))
                .SelectMany(l => l.LearningDeliveries.Where(ld =>
                    ld.LearnAimRef.CaseInsensitiveEquals(withdrawnLearningDelivery.LearnAimRef)
                    && ld.ProgTypeNullable == withdrawnLearningDelivery.ProgTypeNullable
                    && ld.StdCodeNullable == withdrawnLearningDelivery.StdCodeNullable
                    && ld.FworkCodeNullable == withdrawnLearningDelivery.FworkCodeNullable
                    && ld.LearnStartDate >= withdrawnLearningDelivery.LearnActEndDateNullable)).Any();
        }

        public double DaysBetween(DateTime start, DateTime end)
        {
            return (end - start).TotalDays;
        }
    }
}
